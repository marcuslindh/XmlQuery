namespace XmlQuery.Parsing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Group of tokens.
    /// </summary>
    public class TokenGroup
    {
        /// <summary>
        /// Gets or sets tokens in the group.
        /// </summary>
        public List<Token> Tokens { get; set; } = new List<Token>();

        /// <summary>
        /// Gets or sets type of the token group.
        /// </summary>q
        public TokenGroupType Type { get; set; } = TokenGroupType.Unknown;

        /// <summary>
        /// Gets or sets name of the token group is often the tag name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Types of token groups.
        /// </summary>
        public enum TokenGroupType
        {
            /// <summary>
            /// The token group is a start tag.
            /// </summary>
            StartTag,

            /// <summary>
            /// The token group is an end tag.
            /// </summary>
            EndTag,

            /// <summary>
            /// The token group is a start and end tag.
            /// </summary>
            StartAndEndTag,

            /// <summary>
            /// The token group is a value.
            /// </summary>
            Value,

            /// <summary>
            /// The token group is a comment.
            /// </summary>
            Comment,

            /// <summary>
            /// The token group is Unknownn.
            /// </summary>
            Unknown,
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Type} {this.Name} Tokens: {this.Tokens.Count}";
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is TokenGroup group &&
                   EqualityComparer<List<Token>>.Default.Equals(this.Tokens, group.Tokens) &&
                   this.Type == group.Type &&
                   string.Equals(this.Name, group.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Tokens, this.Type, this.Name);
        }
    }
}
