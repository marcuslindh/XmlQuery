using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using XmlQuery.Core;

namespace XmlQuery
{
    /// <summary>
    /// Xml reader
    /// </summary>
    public class XmlReader
    {
        /// <summary>
        /// The document
        /// </summary>
        public Element Document { get; set; }

        /// <summary>
        /// Xml reader
        /// </summary>
        public XmlReader()
        {
            Document = new Element() { Name = "Document", IsDocument = true };
        }

        /// <summary>
        /// Create a new XmlReader and read the xml document
        /// </summary>
        /// <param name="xml"></param>
        public XmlReader([NotNull] string xml)
        {
            Document = new Element() { Name = "Document", IsDocument = true };
            Parse(xml);
        }

        /// <summary>
        /// Parse the xml document
        /// </summary>
        /// <param name="xml"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Parse([NotNull] string xml)
        {
            if (xml == null)
            {
                throw new System.ArgumentNullException(nameof(xml));
            }

            List<Token> tokens = Parser.ParseTokens(xml);

            tokens = Parser.CategorizeTokens(tokens);

            List<TokenGroup> tokenGroups = Parser.GroupTokens(tokens);

            this.Document = Parser.GetDocument(tokenGroups);
        }

        /// <summary>
        /// Get all elements that matches the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Element> Query([NotNull] string query)
        {
            return QueryEngine.Query(this.Document, query);
        }

    }
}
