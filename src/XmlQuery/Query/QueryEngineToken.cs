namespace XmlQuery.Query
{
    /// <summary>
    /// Token of the query engine.
    /// </summary>
    public class QueryEngineToken
    {
        /// <summary>
        /// Gets or sets position of the token in the query.
        /// </summary>
        public int Pos { get; set; } = 0;

        /// <summary>
        /// Gets or sets value of the token.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets type of the token.
        /// </summary>
        public TokenType Type { get; set; } = TokenType.Unknown;

        /// <summary>
        /// all types of the token that it can be.
        /// </summary>
        public enum TokenType
        {
            /// <summary>
            /// String value
            /// </summary>
            String,

            /// <summary>
            /// right arrow that match the first tag after it.
            /// </summary>
            FirstTagAfterArrow,

            /// <summary>
            /// Unknown token.
            /// </summary>
            Unknown,

            /// <summary>
            /// Equals token.
            /// </summary>
            Equal,

            /// <summary>
            /// a tagname.
            /// </summary>
            TagName,

            /// <summary>
            /// [ token that start attribut match.
            /// </summary>
            StartAttributFilter,

            /// <summary>
            /// ] token that end attribut match.
            /// </summary>
            EndAttributFilter,

            /// <summary>
            /// attribut name
            /// </summary>
            AttributName,

            /// <summary>
            /// attribut value.
            /// </summary>
            AttributValue,

            /// <summary>
            /// | token.
            /// </summary>
            Or,
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Pos}: {this.Type} = '{this.Value}'";
        }
    }
}