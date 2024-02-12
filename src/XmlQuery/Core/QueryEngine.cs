using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlQuery.Core
{
    public class QueryEngine
    {
        public static List<Element> Query(Element document, string query)
        {
            List<QueryEngineToken> tokens = ParseQuery(query);

            List<QueryEngineGroupToken> queryEngineGroupToken = GroupTokens(tokens);

            List<Element> elements = new List<Element>();

            List<Filter> filters = new List<Filter>();

            Filter filterItem = new Filter();

            ActionOnElement actionOnElement = new ActionOnElement();
            filterItem.ActionOnElement.Add(actionOnElement);

            foreach (QueryEngineGroupToken groupToken in queryEngineGroupToken)
            {

                if (groupToken.Type == QueryEngineGroupToken.GroupType.Element)
                {


                    int pos = 0;



                    while (pos < groupToken.Tokens.Count)
                    {
                        QueryEngineToken token = groupToken.Tokens[pos];

                        if (token.type == QueryEngineToken.TokenType.TagName)
                        {
                            if (pos > 0)
                            {
                                actionOnElement = new ActionOnElement();
                                filterItem.ActionOnElement.Add(actionOnElement);
                            }


                            string tagName = token.value;

                            if (tagName == "*")
                            {
                                actionOnElement.Func.Add("Match all tags", (element) => { return true; });
                            }
                            else
                            {
                                actionOnElement.Func.Add($"Match tag by name {tagName}", (element) =>
                                {
                                    if (element.Name == tagName)
                                    {
                                        return true;
                                    }

                                    return false;
                                });
                            }

                            pos++;
                        }
                        else if (token.type == QueryEngineToken.TokenType.StartAttributFilter)
                        {
                            pos++;
                            token = groupToken.Tokens[pos];

                            string attributName = token.value;

                            pos++;
                            token = groupToken.Tokens[pos];

                            string attributValue = token.value;

                            actionOnElement.Func.Add($"Match Attribut {attributName} = {attributValue}", (element) =>
                            {
                                if (element.Attributs.Exists(x => x.Name == attributName))
                                {
                                    if (element.Attributs.First(x => x.Name == attributName).Value == attributValue)
                                    {
                                        return true;
                                    }
                                }

                                return false;
                            });

                            pos++;
                            pos++;
                        }
                        else if (token.type == QueryEngineToken.TokenType.FirstTagAfterArrow)
                        {
                            actionOnElement = new ActionOnElement();
                            actionOnElement.FirstMatch = true;

                            filterItem.ActionOnElement.Add(actionOnElement);
                            pos++;
                        }

                    }



                }
                else if (groupToken.Type == QueryEngineGroupToken.GroupType.Or)
                {
                    filters.Add(filterItem);
                    filterItem = new Filter();
                }
                else if (groupToken.Type == QueryEngineGroupToken.GroupType.FirstTagAfterArrow)
                {
                    actionOnElement = new ActionOnElement();
                    actionOnElement.FirstMatch = true;

                    filterItem.ActionOnElement.Add(actionOnElement);
                }

            }

            filters.Add(filterItem);

            foreach (Filter filter in filters)
            {
                List<Element> filterdElements = new List<Element>();

                filterdElements.Add(document);

                foreach (ActionOnElement aoe in filter.ActionOnElement)
                {
                    List<Element> newFilterdElements = new List<Element>();

                    foreach (Element elm in filterdElements)
                    {
                        newFilterdElements.AddRange(TreeWalker(elm, aoe));
                    }

                    filterdElements = newFilterdElements;
                }

                elements.AddRange(filterdElements);
            }


            return elements;
        }

        private static List<Element> TreeWalker(Element element, ActionOnElement actionOnElement)
        {
            List<Element> elements = new List<Element>();
            foreach (Element item in element.Children)
            {
                bool match = true;
                foreach (KeyValuePair<string, Func<Element, bool>> f in actionOnElement.Func)
                {
                    if (f.Value(item) == false)
                    {
                        match = false;
                    }

                }

                if (match)
                {
                    elements.Add(item);

                    if (actionOnElement.FirstMatch)
                    {
                        break;
                    }
                }
                else
                {
                    elements.AddRange(TreeWalker(item, actionOnElement));
                }
            }

            return elements;
        }

        public static List<QueryEngineToken> ParseQuery(string query)
        {
            List<QueryEngineToken> tokens = new List<QueryEngineToken>();
            int pos = 0;

            bool InAttributFilter = false;

            while (pos < query.Length)
            {
                char c = query[pos];

                if (c == '>')
                {
                    tokens.Add(new QueryEngineToken() { pos = pos, type = QueryEngineToken.TokenType.FirstTagAfterArrow, value = c.ToString() });
                }
                else if (c == '=')
                {
                    tokens.Add(new QueryEngineToken() { pos = pos, type = QueryEngineToken.TokenType.Equal, value = c.ToString() });
                }
                else if (c == '"')
                {
                    pos++;
                    int start = pos;
                    while (pos < query.Length && query[pos] != '"' && query[pos] != ']')
                    {
                        pos++;
                    }

                    tokens.Add(new QueryEngineToken { pos = start, type = QueryEngineToken.TokenType.AttributValue, value = query.Substring(start, pos - start) });
                }
                else if (c == '[')
                {
                    tokens.Add(new QueryEngineToken() { pos = pos, type = QueryEngineToken.TokenType.StartAttributFilter, value = c.ToString() });
                    InAttributFilter = true;
                }
                else if (c == ']')
                {
                    tokens.Add(new QueryEngineToken() { pos = pos, type = QueryEngineToken.TokenType.EndAttributFilter, value = c.ToString() });
                    InAttributFilter = false;
                }
                else if (c == '*')
                {
                    tokens.Add(new QueryEngineToken() { pos = pos, type = QueryEngineToken.TokenType.TagName, value = c.ToString() });
                }
                else if (c == '|')
                {
                    tokens.Add(new QueryEngineToken() { pos = pos, type = QueryEngineToken.TokenType.Or, value = c.ToString() });
                }
                else if (c == ' ')
                {

                }
                else
                {
                    int start = pos;
                    while (pos < query.Length && query[pos] != '>' && query[pos] != '=' && query[pos] != ' ' && query[pos] != '[')
                    {
                        pos++;
                    }

                    if (InAttributFilter)
                    {
                        tokens.Add(new QueryEngineToken { pos = start, type = QueryEngineToken.TokenType.AttributName, value = query.Substring(start, pos - start) });


                    }
                    else
                    {
                        tokens.Add(new QueryEngineToken { pos = start, type = QueryEngineToken.TokenType.TagName, value = query.Substring(start, pos - start) });

                        if (pos < query.Length)
                        {
                            c = query[pos];

                            if (c == '[')
                            {
                                pos--;
                            }
                        }
                    }
                }

                pos++;
            }

            return tokens;
        }

        public static List<QueryEngineGroupToken> GroupTokens(List<QueryEngineToken> tokens)
        {
            List<QueryEngineGroupToken> queryEngineGroupToken = new List<QueryEngineGroupToken>();

            int pos = 0;

            while (pos < tokens.Count)
            {
                QueryEngineToken token = tokens[pos];

                if (token.type == QueryEngineToken.TokenType.TagName)
                {
                    QueryEngineGroupToken groupToken = new QueryEngineGroupToken();
                    groupToken.Type = QueryEngineGroupToken.GroupType.Element;

                    groupToken.Tokens.Add(token);

                    queryEngineGroupToken.Add(groupToken);



                    if (pos + 1 < tokens.Count)
                    {
                        if (tokens[pos + 1].type == QueryEngineToken.TokenType.StartAttributFilter)
                        {
                            pos++;
                            token = tokens[pos];

                            groupToken.Tokens.Add(token);

                            while (pos < tokens.Count && tokens[pos].type != QueryEngineToken.TokenType.EndAttributFilter)
                            {
                                pos++;
                                groupToken.Tokens.Add(tokens[pos]);
                            }
                        }
                    }

                }
                else if (token.type == QueryEngineToken.TokenType.Or)
                {
                    QueryEngineGroupToken groupToken = new QueryEngineGroupToken();
                    groupToken.Type = QueryEngineGroupToken.GroupType.Or;

                    groupToken.Tokens.Add(token);

                    queryEngineGroupToken.Add(groupToken);

                }
                else if (token.type == QueryEngineToken.TokenType.FirstTagAfterArrow)
                {
                    QueryEngineGroupToken groupToken = new QueryEngineGroupToken();
                    groupToken.Type = QueryEngineGroupToken.GroupType.FirstTagAfterArrow;

                    groupToken.Tokens.Add(token);

                    queryEngineGroupToken.Add(groupToken);

                }

                pos++;
            }

            return queryEngineGroupToken;
        }

        public class Filter
        {
            public List<ActionOnElement> ActionOnElement { get; set; } = new List<ActionOnElement>();

        }

        public class ActionOnElement
        {
            public bool FirstMatch { get; set; } = false;
            public Dictionary<string, Func<Element, bool>> Func { get; set; } = new Dictionary<string, Func<Element, bool>>();

            public override string ToString()
            {
                return $"FirstMatch: {FirstMatch}, {string.Join(", ", Func.Select(x => x.Key))}";
            }
        }

    }

    public class QueryEngineToken
    {
        public int pos { get; set; } = 0;
        public string value { get; set; } = "";
        public TokenType type { get; set; } = TokenType.Unknown;

        public enum TokenType
        {
            String,
            FirstTagAfterArrow,
            Unknown,
            Equal,
            TagName,
            StartAttributFilter,
            EndAttributFilter,
            AttributName,
            AttributValue,
            Or
        }

        public override string ToString()
        {
            return $"{pos}: {type} = '{value}'";
        }
    }

    public class QueryEngineGroupToken
    {

        public List<QueryEngineToken> Tokens { get; set; } = new List<QueryEngineToken>();
        public GroupType Type { get; set; } = GroupType.Element;

        public enum GroupType
        {
            Element,
            Or,
            FirstTagAfterArrow
        }

        public override string ToString()
        {
            return $"{Type}: {string.Join(", ", Tokens.Select(x => x.ToString()))}";
        }

    }
}
