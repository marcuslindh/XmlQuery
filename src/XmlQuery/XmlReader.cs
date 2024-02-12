using System.Collections.Generic;
using XmlQuery.Core;

namespace XmlQuery
{
    public class XmlReader
    {
        public Element Document { get; set; }

        public void Parse(string xml)
        {
            List<Token> tokens = Parser.Parse(xml);

            tokens = Parser.CategorizeTokens(tokens);

            List<TokenGroup> tokenGroups = Parser.GroupTokens(tokens);

            this.Document =  Parser.GetDocument(tokenGroups);
        }

        public List<Element> Query(string query)
        {
            return QueryEngine.Query(this.Document, query);
        }

    }
}
