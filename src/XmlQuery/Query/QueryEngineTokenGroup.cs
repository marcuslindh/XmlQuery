namespace XmlQuery.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Group of tokens.
    /// </summary>
    public class QueryEngineTokenGroup
    {
        /// <summary>
        /// Gets or sets tokens in the group.
        /// </summary>
        public List<QueryEngineToken> Tokens { get; set; } = new List<QueryEngineToken>();

        /// <summary>
        /// Gets or sets type of the group.
        /// </summary>
        public GroupType Type { get; set; } = GroupType.Element;

        /// <summary>
        /// all types of the group that it can be.
        /// </summary>
        public enum GroupType
        {
            /// <summary>
            /// The group is an element.
            /// </summary>
            Element,

            /// <summary>
            /// the group is an or operator.
            /// </summary>
            Or,

            /// <summary>
            /// The group is the first tag after the arrow.
            /// </summary>
            FirstTagAfterArrow,
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Type}: {string.Join(", ", this.Tokens.Select(token => token.ToString()))}";
        }
    }
}