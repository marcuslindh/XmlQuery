namespace XmlQuery.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using XmlQuery.Xml;

    /// <summary>
    /// Action on element.
    /// </summary>
    public class ActionOnElement
    {
        /// <summary>
        /// Gets or sets a value indicating whether match only the first element.
        /// </summary>
        public bool FirstMatch { get; set; } = false;

        /// <summary>
        /// Gets or sets the function to execute on the element.
        /// </summary>
        public Dictionary<string, Func<Element, bool>> Func { get; set; } = new Dictionary<string, Func<Element, bool>>();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"FirstMatch: {this.FirstMatch}, {string.Join(", ", this.Func.Select(func => func.Key))}";
        }
    }
}