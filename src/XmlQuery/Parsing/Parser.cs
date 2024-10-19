namespace XmlQuery.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using XmlQuery.Xml;

    /// <summary>
    /// XML parser.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parse the xml string into tokens.
        /// </summary>
        /// <param name="xml">xml document.</param>
        /// <returns>Toens from the xml document.</returns>
        public static List<Token> ParseTokens(string xml)
        {
            List<Token> tokens = new List<Token>();
            int pos = 0;

            while (pos < xml.Length)
            {
                char c = xml[pos];

                if (c == '<')
                {
                    tokens.Add(new Token(pos, Token.TokenType.StartArrow, "<"));
                }
                else if (c == '>')
                {
                    tokens.Add(new Token { Pos = pos, Type = Token.TokenType.EndArrow, Value = ">" });
                }
                else if (c == '"')
                {
                    pos++;
                    int start = pos;
                    while (pos < xml.Length && xml[pos] != '"')
                    {
                        pos++;
                    }

                    tokens.Add(new Token { Pos = start, Type = Token.TokenType.String, Value = xml.Substring(start, pos - start) });

                    if (xml[pos + 1] == '>')
                    {
                        tokens.Add(new Token { Pos = pos + 1, Type = Token.TokenType.EndArrow, Value = ">" });
                        pos++;
                    }
                }
                else if (c == '/')
                {
                    tokens.Add(new Token { Pos = pos, Type = Token.TokenType.Slash, Value = "/" });
                }
                else if (c == '=')
                {
                    tokens.Add(new Token { Pos = pos, Type = Token.TokenType.Equal, Value = "=" });
                }
                else
                {
                    if (c == '\n' || c == '\t' || c == ' ')
                    {
                        tokens.Add(new Token { Pos = pos, Type = Token.TokenType.Character, Value = c.ToString() });
                    }
                    else
                    {
                        int start = pos;
                        while (pos < xml.Length && xml[pos] != '<' && xml[pos] != '>' && xml[pos] != '=' &&
                            xml[pos] != '/' && xml[pos] != ' ' && xml[pos] != '\n' && xml[pos] != '\t')
                        {
                            pos++;
                        }

                        tokens.Add(new Token { Pos = start, Type = Token.TokenType.ContiguousCharacters, Value = xml.Substring(start, pos - start) });

                        c = xml[pos];

                        if (c == '<')
                        {
                            tokens.Add(new Token { Pos = pos, Type = Token.TokenType.StartArrow, Value = "<" });
                        }
                        else if (c == '>')
                        {
                            tokens.Add(new Token { Pos = pos, Type = Token.TokenType.EndArrow, Value = ">" });
                        }
                        else if (c == '=')
                        {
                            tokens.Add(new Token { Pos = pos, Type = Token.TokenType.Equal, Value = "=" });
                        }
                        else if (c == '/')
                        {
                            tokens.Add(new Token { Pos = pos, Type = Token.TokenType.Slash, Value = "/" });
                        }
                        else
                        {
                            if (c != default(char))
                            {
                                tokens.Add(new Token { Pos = pos, Type = Token.TokenType.Character, Value = xml[pos].ToString() });
                            }
                        }
                    }
                }

                pos++;
            }

            return tokens;
        }

        /// <summary>
        /// Categorize tokens by more specific types.
        /// </summary>
        /// <param name="tokens">Tokens.</param>
        /// <returns>Tokens with better types.</returns>
        public static List<Token> CategorizeTokens(List<Token> tokens)
        {
            int pos = 0;

            while (pos < tokens.Count)
            {
                Token token = tokens[pos];

                if (token.Type == Token.TokenType.StartArrow)
                {
                    if (tokens[pos + 1].Type == Token.TokenType.Slash)
                    {
                        token.Type = Token.TokenType.EndTag;

                        pos++;
                        pos++;
                        token = tokens[pos];

                        if (token.Type == Token.TokenType.ContiguousCharacters)
                        {
                            token.Type = Token.TokenType.TagName;
                        }

                        pos++;
                    }
                    else
                    {
                        token.Type = Token.TokenType.StartTag;

                        pos++;
                        token = tokens[pos];

                        if (token.Type == Token.TokenType.ContiguousCharacters && token.Value[0] != '!'
                            && token.Value.StartsWith("![CDATA[", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            token.Type = Token.TokenType.TagName;
                        }
                        else if (token.Type == Token.TokenType.ContiguousCharacters && token.Value[0] == '!'
                            && token.Value.StartsWith("![CDATA[", StringComparison.OrdinalIgnoreCase))
                        {
                            StringBuilder cdataString = new StringBuilder();

                            string lastcdataString = token.Value.Substring(8);

                            if (lastcdataString.EndsWith("]]", StringComparison.OrdinalIgnoreCase))
                            {
                                lastcdataString = lastcdataString.Substring(0, lastcdataString.Length - 2);
                            }

                            cdataString.Append(lastcdataString);

                            int _pos = pos;

                            if (token.Value.EndsWith("]]", StringComparison.OrdinalIgnoreCase) == false)
                            {
                                _pos++;

                            LoopNotFinished:
                                while (_pos < tokens.Count && !tokens[_pos].Value.EndsWith("]]", StringComparison.OrdinalIgnoreCase))
                                {
                                    cdataString.Append(tokens[_pos].Value);
                                    lastcdataString = tokens[_pos].Value;
                                    _pos++;
                                }

                                cdataString.Append(tokens[_pos].Value, 0, tokens[_pos].Value.Length - 2);

                                if (tokens[_pos + 1].Type != Token.TokenType.EndArrow)
                                {
                                    _pos++;
                                    goto LoopNotFinished;
                                }
                            }

                            string cDataValue = cdataString.ToString();

                            if (cDataValue.EndsWith("]]", StringComparison.OrdinalIgnoreCase))
                            {
                                cDataValue = cDataValue.Substring(0, cdataString.Length - 2);
                            }

                            tokens.RemoveRange(pos - 1, _pos + 1 - (pos - 1) + 1);

                            Token startCData = new Token()
                            {
                                Pos = pos - 1,
                                Value = "<![CDATA[",
                                Type = Token.TokenType.StartCData,
                            };
                            tokens.Insert(pos - 1, startCData);

                            Token cDataValueToken = new Token()
                            {
                                Pos = pos,
                                Value = cDataValue,
                                Type = Token.TokenType.CDataValue,
                            };
                            tokens.Insert(pos, cDataValueToken);

                            Token endCData = new Token()
                            {
                                Pos = pos + 1,
                                Value = "]]>",
                                Type = Token.TokenType.EndCData,
                            };
                            tokens.Insert(pos + 1, endCData);
                        }
                    }
                }
                else if (token.Type == Token.TokenType.ContiguousCharacters)
                {
                    if (tokens[pos + 1].Type == Token.TokenType.Equal)
                    {
                        token.Type = Token.TokenType.AttributName;

                        pos++;
                        pos++;

                        token = tokens[pos];

                        if (token.Type == Token.TokenType.String)
                        {
                            token.Type = Token.TokenType.AttributValue;
                        }
                    }
                    else if (token.Value[0] == '!' && token.Value.StartsWith("![CDATA[", StringComparison.OrdinalIgnoreCase))
                    {
                        token = tokens[pos];

                        token.Type = Token.TokenType.StartCData;

                        while (pos < tokens.Count && token.Value.EndsWith("]]", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            pos++;
                            token = tokens[pos];
                            token.Type = Token.TokenType.CDataValue;
                        }

                        token = tokens[pos];
                        token.Type = Token.TokenType.EndCData;

                        tokens.RemoveRange(pos + 1, 1);
                    }
                    else if (token.Value.Equals("https:", StringComparison.InvariantCultureIgnoreCase) || token.Value.Equals("http:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (tokens[pos + 1].Type == Token.TokenType.Slash)
                        {
                            List<Token> adressTokens = new List<Token>();

                            adressTokens.Add(token);
                            int start = pos;
                            int localPos = pos + 1;

                            while (localPos < tokens.Count && tokens[localPos].Type != Token.TokenType.StartArrow && tokens[localPos].Value != " ")
                            {
                                Token at = tokens[localPos];
                                adressTokens.Add(at);
                                localPos++;
                            }

                            token.Type = Token.TokenType.Url;
                            token.Value = string.Concat(adressTokens.Select(token => token.Value));

                            tokens.RemoveRange(start + 1, adressTokens.Count - 1);
                        }
                    }
                }

                pos++;
            }

            return tokens;
        }

        /// <summary>
        /// group tokens into groups.
        /// </summary>
        /// <param name="tokens">Tokens.</param>
        /// <returns>Grouped tokens.</returns>
        public static List<TokenGroup> GroupTokens(List<Token> tokens)
        {
            List<TokenGroup> tokenGroups = new List<TokenGroup>();

            TokenGroup tokenGroup = new TokenGroup();

            int pos = 0;

            while (pos < tokens.Count)
            {
                Token token = tokens[pos];

                if (token.Type == Token.TokenType.StartTag)
                {
                    if (pos + 1 < tokens.Count && tokens[pos + 1].Type == Token.TokenType.ContiguousCharacters &&
                        tokens[pos + 1].Value.StartsWith("!--", StringComparison.OrdinalIgnoreCase))
                    {
                        int start = pos;

                    RestartLoop:

                        while (pos < tokens.Count && tokens[pos].Type != Token.TokenType.ContiguousCharacters &&
                            tokens[pos].Value.EndsWith("--", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            pos++;
                        }

                        pos++;

                        if (tokens[pos].Type == Token.TokenType.EndArrow || pos >= tokens.Count)
                        {
                            StringBuilder comment = new StringBuilder();

                            for (int tokenIndex = start; tokenIndex < pos; tokenIndex++)
                            {
                                comment.Append(tokens[tokenIndex].Value);
                            }

                            tokenGroup = new TokenGroup();
                            tokenGroup.Type = TokenGroup.TokenGroupType.Comment;
                            tokenGroup.Tokens.Add(new Token() { Type = Token.TokenType.Comment, Value = comment.ToString() });
                            tokenGroups.Add(tokenGroup);

                            continue;
                        }
                        else
                        {
                            goto RestartLoop;
                        }
                    }

                    tokenGroup = new TokenGroup();
                    tokenGroup.Type = TokenGroup.TokenGroupType.StartTag;

                    tokenGroup.Tokens.Add(token);
                    pos++;

                    while (pos < tokens.Count && tokens[pos].Type != Token.TokenType.EndArrow)
                    {
                        tokenGroup.Tokens.Add(tokens[pos]);
                        pos++;
                    }

                    if (string.Equals(tokenGroup.Tokens.Find(token => token.Type == Token.TokenType.TagName)?.Value, "?xml", StringComparison.OrdinalIgnoreCase))
                    {
                        tokenGroup.Type = TokenGroup.TokenGroupType.StartAndEndTag;
                    }

                    tokenGroup.Tokens.Add(tokens[pos]);

                    tokenGroup.Name = tokenGroup.Tokens.First(token => token.Type == Token.TokenType.TagName).Value;

                    if (tokenGroup.Tokens[tokenGroup.Tokens.Count - 2].Type == Token.TokenType.Slash)
                    {
                        tokenGroup.Type = TokenGroup.TokenGroupType.StartAndEndTag;
                    }

                    tokenGroups.Add(tokenGroup);
                }
                else if (token.Type == Token.TokenType.EndTag)
                {
                    tokenGroup = new TokenGroup();
                    tokenGroup.Type = TokenGroup.TokenGroupType.EndTag;

                    tokenGroup.Tokens.Add(token);
                    pos++;

                    while (pos < tokens.Count && tokens[pos].Type != Token.TokenType.EndArrow)
                    {
                        tokenGroup.Tokens.Add(tokens[pos]);
                        pos++;
                    }

                    tokenGroup.Tokens.Add(tokens[pos]);

                    Token nameToken = tokenGroup.Tokens.Find(token => token.Type == Token.TokenType.TagName);
                    if (nameToken != null)
                    {
                        tokenGroup.Name = nameToken.Value;
                    }

                    tokenGroups.Add(tokenGroup);
                }
                else if (token.Type == Token.TokenType.StartCData)
                {
                    tokenGroup = new TokenGroup();
                    tokenGroup.Type = TokenGroup.TokenGroupType.Value;
                    tokenGroup.Name = "CDataValue";

                    pos++;

                    while (pos < tokens.Count && tokens[pos].Type != Token.TokenType.EndCData)
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

                    while (pos < tokens.Count && tokens[pos].Type != Token.TokenType.StartTag && tokens[pos].Type != Token.TokenType.EndTag)
                    {
                        if (string.IsNullOrEmpty(tokens[pos].Value) == false)
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
        /// Get the document tree from the token groups.
        /// </summary>
        /// <param name="tokenGroups">Token groups.</param>
        /// <returns>Generted document tree.</returns>
        public static Element GetDocument(List<TokenGroup> tokenGroups)
        {
            Element doc = new Element() { Name = "Document", IsDocument = true };

            GenerateTagTree(tokenGroups, 0, doc);

            return doc;
        }

        private static int GenerateTagTree(List<TokenGroup> tokenGroups, int pos, Element root)
        {
            Element currentTag = new Element();

            while (pos < tokenGroups.Count)
            {
                TokenGroup tokenGroup = tokenGroups[pos];

                if (tokenGroup.Type == TokenGroup.TokenGroupType.StartTag && currentTag.HasName == false)
                {
                    string[] nameAndNamespace = tokenGroup.Name.Split(":");

                    string name = string.Empty;
                    string ns = string.Empty;

                    if (nameAndNamespace.Length == 1)
                    {
                        name = nameAndNamespace[0];
                    }
                    else if (nameAndNamespace.Length == 2)
                    {
                        ns = nameAndNamespace[0];
                        name = nameAndNamespace[1];
                    }

                    Element tag = new Element()
                    {
                        Name = name,
                        Namespace = ns,
                        ParentElement = root,
                    };

                    foreach (Token token in tokenGroup.Tokens)
                    {
                        if (token.Type == Token.TokenType.AttributName)
                        {
                            Attribut attribut = new Attribut()
                            {
                                Name = token.Value,
                            };

                            tag.Attributs.Add(attribut);
                        }
                        else if (token.Type == Token.TokenType.AttributValue)
                        {
                            tag.Attributs.Last().Value = token.Value;
                        }
                    }

                    root.Children.Add(tag);
                    currentTag = tag;

                    pos++;
                }
                else if (tokenGroup.Type == TokenGroup.TokenGroupType.StartTag && currentTag.HasName)
                {
                    pos = GenerateTagTree(tokenGroups, pos, currentTag);
                }
                else if (tokenGroup.Type == TokenGroup.TokenGroupType.StartAndEndTag)
                {
                    Element tag = new Element()
                    {
                        Name = tokenGroup.Name,
                        HasEndTag = false,
                    };

                    foreach (Token token in tokenGroup.Tokens)
                    {
                        if (token.Type == Token.TokenType.AttributName)
                        {
                            Attribut attribut = new Attribut()
                            {
                                Name = token.Value,
                            };

                            tag.Attributs.Add(attribut);
                        }
                        else if (token.Type == Token.TokenType.AttributValue)
                        {
                            tag.Attributs.Last().Value = token.Value;
                        }
                    }

                    if (currentTag.HasName)
                    {
                        currentTag.Children.Add(tag);
                    }
                    else
                    {
                        root.Children.Add(tag);
                    }

                    pos++;
                }
                else if (tokenGroup.Type == TokenGroup.TokenGroupType.Value)
                {
                    if (currentTag.HasName)
                    {
                        currentTag.Value += string.Concat(tokenGroup.Tokens.Select(token => token.Value)).Trim();
                    }
                    else
                    {
                        root.Value += string.Concat(tokenGroup.Tokens.Select(token => token.Value)).Trim();
                    }

                    pos++;
                }
                else if (tokenGroup.Type == TokenGroup.TokenGroupType.EndTag)
                {
                    pos++;
                    return pos;
                }
                else if (tokenGroup.Type == TokenGroup.TokenGroupType.Comment)
                {
                    pos++;
                }
            }

            return pos;
        }
    }
}
