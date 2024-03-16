namespace XmlQuery
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using XmlQuery.Parsing;
    using XmlQuery.Query;
    using XmlQuery.Xml;

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReader"/> class.
        /// </summary>
        public XmlReader()
        {
            this.Document = new Element() { Name = "Document", IsDocument = true };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReader"/> class.
        /// </summary>
        /// <param name="xml">Xml document.</param>
        public XmlReader([NotNull] string xml)
        {
            this.Document = new Element() { Name = "Document", IsDocument = true };
            this.Parse(xml);
        }

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        public Element Document { get; set; }

        /// <summary>
        /// Parse the xml document.
        /// </summary>
        /// <param name="xml">Xml document.</param>
        /// <exception cref="System.ArgumentNullException">if xml is null.</exception>
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
        /// Get all elements that matches the query.
        /// </summary>
        /// <param name="query">CSS selector.</param>
        /// <returns>Elements thar match CSS selector.</returns>
        public List<Element> Query([NotNull] string query)
        {
            return QueryEngine.Query(this.Document, query);
        }
    }
}
