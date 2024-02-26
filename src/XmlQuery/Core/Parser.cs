using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlQuery
{
    namespace Core
    {
        public static class Parser
        {
            /// <summary>
            /// Parse the xml string into tokens
            /// </summary>
            /// <param name="xml"></param>
            /// <returns></returns>
            public static List<Token> Parse(string xml)
            {
                List<Token> tokens = new List<Token>();
                int pos = 0;

                while (pos < xml.Length)
                {
                    char c = xml[pos];

                    if (c == '<')
                    {
                        tokens.Add(new Token { pos = pos, type = Token.TokenType.StartArrow, value = "<" });
                    }
                    else if (c == '>')
                    {
                        tokens.Add(new Token { pos = pos, type = Token.TokenType.EndArrow, value = ">" });
                    }
                    else if (c == '"')
                    {
                        pos++;
                        int start = pos;
                        while (pos < xml.Length && xml[pos] != '"')
                        {
                            pos++;
                        }


                        tokens.Add(new Token { pos = start, type = Token.TokenType.String, value = xml.Substring(start, pos - start) });

                        pos++;

                        if (xml[pos] == '>')
                        {
                            tokens.Add(new Token { pos = pos, type = Token.TokenType.EndArrow, value = ">" });
                            pos++;
                        }


                    }
                    else if (c == '/')
                    {
                        tokens.Add(new Token { pos = pos, type = Token.TokenType.Slash, value = "/" });
                    }
                    else if (c == '=')
                    {
                        tokens.Add(new Token { pos = pos, type = Token.TokenType.Equal, value = "=" });
                    }
                    else
                    {
                        if (c == '\n' || c == '\t' || c == ' ')
                        {
                            tokens.Add(new Token { pos = pos, type = Token.TokenType.Character, value = c.ToString() });
                        }
                        else
                        {
                            int start = pos;
                            while (pos < xml.Length && xml[pos] != '<' && xml[pos] != '>' && xml[pos] != '=' &&
                                xml[pos] != '/' && xml[pos] != ' ' && xml[pos] != '\n' && xml[pos] != '\t')
                            {
                                pos++;
                            }

                            tokens.Add(new Token { pos = start, type = Token.TokenType.ContiguousCharacters, value = xml.Substring(start, pos - start) });


                            c = xml[pos];

                            if (c == '<')
                            {
                                tokens.Add(new Token { pos = pos, type = Token.TokenType.StartArrow, value = "<" });
                            }
                            else if (c == '>')
                            {
                                tokens.Add(new Token { pos = pos, type = Token.TokenType.EndArrow, value = ">" });
                            }
                            else if (c == '=')
                            {
                                tokens.Add(new Token { pos = pos, type = Token.TokenType.Equal, value = "=" });
                            }
                            else if (c == '/')
                            {
                                tokens.Add(new Token { pos = pos, type = Token.TokenType.Slash, value = "/" });
                            }
                            else
                            {
                                tokens.Add(new Token { pos = pos, type = Token.TokenType.Character, value = xml[pos].ToString() });

                            }


                        }


                    }

                    pos++;
                }

                return tokens;
            }

            /// <summary>
            /// Categorize tokens by more specific types
            /// </summary>
            /// <param name="tokens"></param>
            /// <returns></returns>
            public static List<Token> CategorizeTokens(List<Token> tokens)
            {
                int pos = 0;

                while (pos < tokens.Count)
                {
                    Token token = tokens[pos];

                    if (token.type == Token.TokenType.StartArrow)
                    {
                        if (tokens[pos + 1].type == Token.TokenType.Slash)
                        {
                            token.type = Token.TokenType.EndTag;

                            pos++;
                            pos++;
                            token = tokens[pos];

                            if (token.type == Token.TokenType.ContiguousCharacters)
                            {
                                token.type = Token.TokenType.TagName;
                            }

                            pos++;
                        }
                        else
                        {
                            token.type = Token.TokenType.StartTag;

                            pos++;
                            token = tokens[pos];

                            if (token.type == Token.TokenType.ContiguousCharacters && (token.value[0] != '!' && token.value.StartsWith("![CDATA[") == false))
                            {
                                token.type = Token.TokenType.TagName;
                            }
                            else if (token.type == Token.TokenType.ContiguousCharacters && (token.value[0] == '!' && token.value.StartsWith("![CDATA[")))
                            {

                                StringBuilder cdataString = new StringBuilder();
                                cdataString.Append(token.value.Substring(8));

                                string LastcdataString = token.value.Substring(8);

                                int _pos = pos + 1;
                                //.EndsWith("]]")
                                while (_pos < tokens.Count && (LastcdataString.Length >= 2 && LastcdataString[LastcdataString.Length - 1] == ']' && LastcdataString[LastcdataString.Length - 2] == ']'))
                                {
                                    cdataString.Append(tokens[_pos].value);
                                    LastcdataString = tokens[_pos].value;
                                    _pos++;
                                }

                                string CDataValue = cdataString.ToString();

                                if (CDataValue.EndsWith("]]"))
                                {
                                    CDataValue = CDataValue.Substring(0, cdataString.Length - 2);
                                }

                                Token t2 = tokens[pos];
                                Token t3 = tokens[_pos];

                                //for (int i = 0; i < (_pos + 1) - (pos - 1); i++)
                                //{
                                //    tokens.RemoveAt(pos - 1);
                                //}
                                tokens.RemoveRange(pos - 1, ((_pos + 1) - (pos - 1)) - 1);

                                Token startCData = new Token()
                                {
                                    pos = pos - 1,
                                    value = "<![CDATA[",
                                    type = Token.TokenType.StartCData
                                };
                                tokens.Insert(pos - 1, startCData);

                                Token cDataValue = new Token()
                                {
                                    pos = pos,
                                    value = CDataValue,
                                    type = Token.TokenType.CDataValue
                                };
                                tokens.Insert(pos, cDataValue);

                                Token endCData = new Token()
                                {
                                    pos = pos + 1,
                                    value = "]]>",
                                    type = Token.TokenType.EndCData
                                };
                                tokens.Insert(pos + 1, endCData);

                            }

                        }
                    }
                    else if (token.type == Token.TokenType.ContiguousCharacters)
                    {
                        if (tokens[pos + 1].type == Token.TokenType.Equal)
                        {
                            token.type = Token.TokenType.AttributName;

                            pos++;
                            pos++;

                            token = tokens[pos];

                            if (token.type == Token.TokenType.String)
                            {
                                token.type = Token.TokenType.AttributValue;
                            }
                        }
                        else if (token.value[0] == '!' && token.value.StartsWith("![CDATA["))
                        {
                            //tokens.RemoveRange(pos, 1);

                            token = tokens[pos];

                            token.type = Token.TokenType.StartCData;

                            while (pos < tokens.Count && token.value.EndsWith("]]") == false)
                            {
                                pos++;
                                token = tokens[pos];
                                token.type = Token.TokenType.CDataValue;
                            }

                            token = tokens[pos];
                            token.type = Token.TokenType.EndCData;

                            tokens.RemoveRange(pos + 1, 1);
                        }
                        else if (token.value.Equals("https:", StringComparison.InvariantCultureIgnoreCase) || token.value.Equals("http:", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (tokens[pos + 1].type == Token.TokenType.Slash)
                            {

                                List<Token> adressTokens = new List<Token>();

                                adressTokens.Add(token);
                                int start = pos;
                                int localPos = pos + 1;

                                while (localPos < tokens.Count && tokens[localPos].type != Token.TokenType.StartArrow && tokens[localPos].value != " ")
                                {
                                    Token at = tokens[localPos];
                                    adressTokens.Add(at);
                                    localPos++;
                                }

                                token.type = Token.TokenType.Url;
                                token.value = string.Join("", adressTokens.Select(t => t.value));

                                tokens.RemoveRange(start + 1, adressTokens.Count - 1);
                            }
                        }
                    }

                    pos++;
                }


                return tokens;
            }

            /// <summary>
            /// group tokens into groups
            /// </summary>
            /// <param name="tokens"></param>
            /// <returns></returns>
            public static List<TokenGroup> GroupTokens(List<Token> tokens)
            {
                List<TokenGroup> tokenGroups = new List<TokenGroup>();

                TokenGroup tokenGroup = new TokenGroup();

                int pos = 0;

                while (pos < tokens.Count)
                {
                    Token token = tokens[pos];

                    if (token.type == Token.TokenType.StartTag)
                    {
                        tokenGroup = new TokenGroup();
                        tokenGroup.Type = TokenGroup.TokenGroupType.StartTag;

                        tokenGroup.Tokens.Add(token);
                        pos++;

                        while (pos < tokens.Count && tokens[pos].type != Token.TokenType.EndArrow)
                        {
                            tokenGroup.Tokens.Add(tokens[pos]);
                            pos++;
                        }

                        if (tokenGroup.Tokens.Where(x => x.type == Token.TokenType.TagName).First().value == "?xml")
                        {
                            tokenGroup.Type = TokenGroup.TokenGroupType.StartAndEndTag;
                        }

                        tokenGroup.Tokens.Add(tokens[pos]);

                        tokenGroup.Name = tokenGroup.Tokens.Where(x => x.type == Token.TokenType.TagName).First().value;

                        if (tokenGroup.Tokens[tokenGroup.Tokens.Count - 2].type == Token.TokenType.Slash)
                        {
                            tokenGroup.Type = TokenGroup.TokenGroupType.StartAndEndTag;
                        }

                        tokenGroups.Add(tokenGroup);
                    }
                    else if (token.type == Token.TokenType.EndTag)
                    {
                        tokenGroup = new TokenGroup();
                        tokenGroup.Type = TokenGroup.TokenGroupType.EndTag;

                        tokenGroup.Tokens.Add(token);
                        pos++;

                        while (pos < tokens.Count && tokens[pos].type != Token.TokenType.EndArrow)
                        {
                            tokenGroup.Tokens.Add(tokens[pos]);
                            pos++;
                        }


                        tokenGroup.Tokens.Add(tokens[pos]);

                        tokenGroup.Name = tokenGroup.Tokens.Where(x => x.type == Token.TokenType.TagName).First().value;

                        tokenGroups.Add(tokenGroup);
                    }
                    else if (token.type == Token.TokenType.StartCData)
                    {
                        tokenGroup = new TokenGroup();
                        tokenGroup.Type = TokenGroup.TokenGroupType.Value;

                        pos++;

                        while (pos < tokens.Count && tokens[pos].type != Token.TokenType.EndCData)
                        {
                            tokenGroup.Tokens.Add(tokens[pos]);
                            pos++;
                        }

                        tokenGroups.Add(tokenGroup);
                    }
                    else
                    {
                        tokenGroup = new TokenGroup();
                        tokenGroup.Type = TokenGroup.TokenGroupType.Value;
                        tokenGroup.Tokens.Add(token);

                        pos++;

                        while (pos < tokens.Count && tokens[pos].type != Token.TokenType.StartTag && tokens[pos].type != Token.TokenType.EndTag)
                        {
                            if (string.IsNullOrEmpty(tokens[pos].value) == false)
                            {
                                tokenGroup.Tokens.Add(tokens[pos]);
                            }

                            pos++;
                        }

                        tokenGroups.Add(tokenGroup);

                        pos--;
                    }

                    pos++;
                }

                if (tokenGroups.Last().Type == TokenGroup.TokenGroupType.Value)
                {
                    tokenGroups.Remove(tokenGroups.Last());
                }

                return tokenGroups;
            }

            /// <summary>
            /// Get the document tree from the token groups
            /// </summary>
            /// <param name="tokenGroups"></param>
            /// <returns></returns>
            public static Element GetDocument(List<TokenGroup> tokenGroups)
            {
                Element doc = new Element() { Name = "Document" };

                GenerateTagTree(tokenGroups, 0, doc);

                return doc;
            }

            private static int GenerateTagTree(List<TokenGroup> tokenGroups, int pos, Element root)
            {
                Element CurrentTag = null;

                while (pos < tokenGroups.Count)
                {
                    TokenGroup tokenGroup = tokenGroups[pos];


                    if (tokenGroup.Type == TokenGroup.TokenGroupType.StartTag && CurrentTag == null)
                    {

                        string[] NameAndNamespace = tokenGroup.Name.Split(":");

                        string name = "";
                        string ns = "";

                        if (NameAndNamespace.Length == 1)
                        {
                            name = NameAndNamespace[0];
                        }
                        else if (NameAndNamespace.Length == 2)
                        {
                            ns = NameAndNamespace[0];
                            name = NameAndNamespace[1];
                        }

                        Element tag = new Element()
                        {
                            Name = name,
                            Namespace = ns,
                            ParentElement = root
                        };

                        foreach (Token token in tokenGroup.Tokens)
                        {
                            if (token.type == Token.TokenType.AttributName)
                            {
                                Attribut attribut = new Attribut()
                                {
                                    Name = token.value
                                };

                                tag.Attributs.Add(attribut);
                            }
                            else if (token.type == Token.TokenType.AttributValue)
                            {
                                tag.Attributs.Last().Value = token.value;
                            }
                        }

                        root.Children.Add(tag);
                        CurrentTag = tag;

                        pos++;
                    }
                    else if (tokenGroup.Type == TokenGroup.TokenGroupType.StartTag && CurrentTag != null)
                    {
                        pos = GenerateTagTree(tokenGroups, pos, CurrentTag);
                    }
                    else if (tokenGroup.Type == TokenGroup.TokenGroupType.StartAndEndTag)
                    {
                        Element tag = new Element()
                        {
                            Name = tokenGroup.Name,
                            HasEndTag = false
                        };

                        foreach (Token token in tokenGroup.Tokens)
                        {
                            if (token.type == Token.TokenType.AttributName)
                            {
                                Attribut attribut = new Attribut()
                                {
                                    Name = token.value
                                };

                                tag.Attributs.Add(attribut);
                            }
                            else if (token.type == Token.TokenType.AttributValue)
                            {
                                tag.Attributs.Last().Value = token.value;
                            }
                        }

                        if (CurrentTag != null)
                        {
                            CurrentTag.Children.Add(tag);
                        }
                        else
                        {
                            root.Children.Add(tag);
                        }

                        pos++;
                    }
                    else if (tokenGroup.Type == TokenGroup.TokenGroupType.Value)
                    {
                        if (CurrentTag != null)
                        {
                            CurrentTag.Value += string.Join("", tokenGroup.Tokens.Select(x => x.value)).Trim();
                        }
                        else
                        {
                            root.Value += string.Join("", tokenGroup.Tokens.Select(x => x.value)).Trim();
                        }

                        pos++;
                    }
                    else if (tokenGroup.Type == TokenGroup.TokenGroupType.EndTag)
                    {
                        pos++;
                        return pos;
                    }
                }

                return pos;
            }
        }
    }
}
