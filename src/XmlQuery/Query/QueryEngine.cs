namespace XmlQuery.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using XmlQuery.Xml;

    /// <summary>
    /// Query parser and engine for CSS selectors.
    /// </summary>
    public static class QueryEngine
    {
        /// <summary>
        /// Query the document by CSS selector.
        /// </summary>
        /// <param name="document">Element to query in.</param>
        /// <param name="query">css selecter.</param>
        /// <returns>Elements that matches css selector.</returns>
        public static List<Element> Query(Element document, string query)
        {
            List<QueryEngineToken> tokens = ParseQuery(query);

            List<QueryEngineTokenGroup> queryEngineGroupToken = GroupTokens(tokens);

            List<Element> elements = new List<Element>();

            List<QueryEngineFilter> filters = new List<QueryEngineFilter>();

            QueryEngineFilter filterItem = new QueryEngineFilter();

            ActionOnElement actionOnElement = new ActionOnElement();

            bool firstMatch = false;

            foreach (QueryEngineTokenGroup groupToken in queryEngineGroupToken)
            {
                if (groupToken.Type == QueryEngineTokenGroup.GroupType.Element)
                {
                    int pos = 0;

                    while (pos < groupToken.Tokens.Count)
                    {
                        QueryEngineToken token = groupToken.Tokens[pos];

                        if (token.Type == QueryEngineToken.TokenType.TagName)
                        {
                            actionOnElement = new ActionOnElement();
                            filterItem.ActionOnElement.Add(actionOnElement);

                            if (firstMatch)
                            {
                                actionOnElement.FirstMatch = true;
                                firstMatch = false;
                            }

                            string tagName = token.Value;

                            if (string.Equals(tagName, "*", StringComparison.OrdinalIgnoreCase))
                            {
                                actionOnElement.Func.Add("Match all tags", (element) => { return true; });
                            }
                            else
                            {
                                actionOnElement.Func.Add($"Match tag by name {tagName}", (element) =>
                                {
                                    if (string.Equals(element.Name, tagName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        return true;
                                    }

                                    return false;
                                });
                            }

                            pos++;
                        }
                        else if (token.Type == QueryEngineToken.TokenType.StartAttributFilter)
                        {
                            pos++;
                            token = groupToken.Tokens[pos];

                            string attributName = token.Value;

                            pos++;
                            token = groupToken.Tokens[pos];

                            string attributValue = token.Value;

                            actionOnElement.Func.Add($"Match Attribut {attributName} = {attributValue}", (element) =>
                            {
                                if (element.Attributs.Exists(attr => string.Equals(attr.Name, attributName, StringComparison.OrdinalIgnoreCase))
                                    && element.Attributs.First(attr => attr.Name == attributName).Value == attributValue)
                                {
                                    return true;
                                }

                                return false;
                            });

                            pos++;
                            pos++;
                        }
                        else if (token.Type == QueryEngineToken.TokenType.FirstTagAfterArrow)
                        {
                            firstMatch = true;
                            pos++;
                        }
                    }
                }
                else if (groupToken.Type == QueryEngineTokenGroup.GroupType.Or)
                {
                    filters.Add(filterItem);
                    filterItem = new QueryEngineFilter();
                }
                else if (groupToken.Type == QueryEngineTokenGroup.GroupType.FirstTagAfterArrow)
                {
                    firstMatch = true;
                }
            }

            filters.Add(filterItem);

            foreach (QueryEngineFilter filter in filters)
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

        /// <summary>
        /// Parse the CSS selector to tokens.
        /// </summary>
        /// <param name="query">CSS selector.</param>
        /// <returns>List of tokens from CSS selector.</returns>
        public static List<QueryEngineToken> ParseQuery(string query)
        {
            List<QueryEngineToken> tokens = new List<QueryEngineToken>();
            int pos = 0;

            bool inAttributFilter = false;

            while (pos < query.Length)
            {
                char c = query[pos];

                if (c == '>')
                {
                    tokens.Add(new QueryEngineToken() { Pos = pos, Type = QueryEngineToken.TokenType.FirstTagAfterArrow, Value = c.ToString() });
                }
                else if (c == '=')
                {
                    tokens.Add(new QueryEngineToken() { Pos = pos, Type = QueryEngineToken.TokenType.Equal, Value = c.ToString() });
                }
                else if (c == '"')
                {
                    pos++;
                    int start = pos;
                    while (pos < query.Length && query[pos] != '"' && query[pos] != ']')
                    {
                        pos++;
                    }

                    tokens.Add(new QueryEngineToken { Pos = start, Type = QueryEngineToken.TokenType.AttributValue, Value = query.Substring(start, pos - start) });
                }
                else if (c == '[')
                {
                    tokens.Add(new QueryEngineToken() { Pos = pos, Type = QueryEngineToken.TokenType.StartAttributFilter, Value = c.ToString() });
                    inAttributFilter = true;
                }
                else if (c == ']')
                {
                    tokens.Add(new QueryEngineToken() { Pos = pos, Type = QueryEngineToken.TokenType.EndAttributFilter, Value = c.ToString() });
                    inAttributFilter = false;
                }
                else if (c == '*')
                {
                    tokens.Add(new QueryEngineToken() { Pos = pos, Type = QueryEngineToken.TokenType.TagName, Value = c.ToString() });
                }
                else if (c == '|')
                {
                    tokens.Add(new QueryEngineToken() { Pos = pos, Type = QueryEngineToken.TokenType.Or, Value = c.ToString() });
                }
                else if (c == ' ')
                {
                    // skip space
                }
                else
                {
                    int start = pos;
                    while (pos < query.Length && query[pos] != '>' && query[pos] != '=' && query[pos] != ' ' && query[pos] != '[')
                    {
                        pos++;
                    }

                    if (inAttributFilter)
                    {
                        tokens.Add(new QueryEngineToken { Pos = start, Type = QueryEngineToken.TokenType.AttributName, Value = query.Substring(start, pos - start) });
                    }
                    else
                    {
                        tokens.Add(new QueryEngineToken { Pos = start, Type = QueryEngineToken.TokenType.TagName, Value = query.Substring(start, pos - start) });

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

        /// <summary>
        /// Group the tokens to groups.
        /// </summary>
        /// <param name="tokens">Tokens to group.</param>
        /// <returns>Token groups.</returns>
        public static List<QueryEngineTokenGroup> GroupTokens(List<QueryEngineToken> tokens)
        {
            List<QueryEngineTokenGroup> queryEngineGroupToken = new List<QueryEngineTokenGroup>();

            int pos = 0;

            QueryEngineTokenGroup groupToken = new QueryEngineTokenGroup();
            groupToken.Type = QueryEngineTokenGroup.GroupType.Element;

            queryEngineGroupToken.Add(groupToken);

            while (pos < tokens.Count)
            {
                QueryEngineToken token = tokens[pos];

                if (token.Type == QueryEngineToken.TokenType.TagName)
                {
                    if (groupToken.Type == QueryEngineTokenGroup.GroupType.Element && groupToken.Tokens.Count > 0)
                    {
                        groupToken = new QueryEngineTokenGroup();
                        queryEngineGroupToken.Add(groupToken);
                    }

                    groupToken.Type = QueryEngineTokenGroup.GroupType.Element;
                    groupToken.Tokens.Add(token);
                }
                else if (token.Type == QueryEngineToken.TokenType.StartAttributFilter)
                {
                    groupToken.Tokens.Add(token);

                    while (pos < tokens.Count && tokens[pos].Type != QueryEngineToken.TokenType.EndAttributFilter)
                    {
                        pos++;
                        groupToken.Tokens.Add(tokens[pos]);
                    }
                }
                else if (token.Type == QueryEngineToken.TokenType.Or)
                {
                    groupToken = new QueryEngineTokenGroup();
                    groupToken.Type = QueryEngineTokenGroup.GroupType.Or;

                    groupToken.Tokens.Add(token);

                    queryEngineGroupToken.Add(groupToken);

                    groupToken = new QueryEngineTokenGroup();
                    groupToken.Type = QueryEngineTokenGroup.GroupType.Element;

                    queryEngineGroupToken.Add(groupToken);
                }
                else if (token.Type == QueryEngineToken.TokenType.FirstTagAfterArrow)
                {
                    groupToken = new QueryEngineTokenGroup();
                    groupToken.Type = QueryEngineTokenGroup.GroupType.FirstTagAfterArrow;

                    groupToken.Tokens.Add(token);

                    queryEngineGroupToken.Add(groupToken);

                    groupToken = new QueryEngineTokenGroup();
                    groupToken.Type = QueryEngineTokenGroup.GroupType.Element;

                    queryEngineGroupToken.Add(groupToken);
                }

                pos++;
            }

            return queryEngineGroupToken;
        }

        /// <summary>
        /// Walk the tree and Search for elements that matches the CSS selector.
        /// </summary>
        /// <param name="element">Element to traverse the tree in.</param>
        /// <param name="actionOnElement">All the filters to use to find the elements.</param>
        /// <returns>matched elements.</returns>
        private static List<Element> TreeWalker(Element element, ActionOnElement actionOnElement)
        {
            List<Element> elements = new List<Element>();
            foreach (Element item in element.Children)
            {
                bool match = true;
                foreach (KeyValuePair<string, Func<Element, bool>> func in actionOnElement.Func)
                {
                    if (func.Value(item) == false)
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
    }
}