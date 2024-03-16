namespace XmlQuery.Parsing
{
    using System;

    /// <summary>
    /// Parsed token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="pos">Position of the token.</param>
        /// <param name="type">Type of the token.</param>
        /// <param name="value">Value of the token.</param>
        public Token(int pos, TokenType type, string value)
        {
            this.Pos = pos;
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets position of the token in the input string.
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
        /// Alla the possible token types.
        /// </summary>
        public enum TokenType
        {
            /// <summary>
            /// String value.
            /// </summary>
            String = 0,

            /// <summary>
            /// Start arrow.
            /// </summary>
            StartArrow = 1,

            /// <summary>
            /// End arrow.
            /// </summary>
            EndArrow = 2,

            /// <summary>
            /// Slash.
            /// </summary>
            Slash = 3,

            /// <summary>
            /// Unknown token.
            /// </summary>
            Unknown = 4,

            /// <summary>
            /// Single character.
            /// </summary>
            Character = 5,

            /// <summary>
            /// Equals token.
            /// </summary>
            Equal = 6,

            /// <summary>
            /// Contiguous characters.
            /// </summary>
            ContiguousCharacters = 7,

            /// <summary>
            /// Tag name.
            /// </summary>
            TagName = 8,

            /// <summary>
            /// left arrow.
            /// </summary>
            StartTag = 9,

            /// <summary>
            /// right arrow.
            /// </summary>
            EndTag = 10,

            /// <summary>
            /// attribut name.
            /// </summary>
            AttributName = 11,

            /// <summary>
            /// attribut value.
            /// </summary>
            AttributValue = 12,

            /// <summary>
            /// string that start with http:// or https://.
            /// </summary>
            Url = 13,

            /// <summary>
            /// cdata start.
            /// </summary>
            StartCData = 14,

            /// <summary>
            /// cdata value.
            /// </summary>
            CDataValue = 15,

            /// <summary>
            /// cdata end.
            /// </summary>
            EndCData = 16,

            /// <summary>
            /// comment.
            /// </summary>
            Comment = 17,
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Pos}: {this.Type} = '{this.Value}'";
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Token token &&
                   this.Pos == token.Pos &&
                   string.Equals(this.Value, token.Value, StringComparison.OrdinalIgnoreCase) &&
                   this.Type == token.Type;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Pos, this.Value, this.Type);
        }
    }
}
