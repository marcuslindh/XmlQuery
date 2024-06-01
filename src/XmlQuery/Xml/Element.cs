namespace XmlQuery.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using XmlQuery.Query;

    /// <summary>
    /// An element in an xml document.
    /// </summary>
    public class Element
    {
        /// <summary>
        /// Gets a value indicating whether the element has a name.
        /// </summary>
        public bool HasName => !string.IsNullOrEmpty(this.Name);

        /// <summary>
        /// Gets a value indicating whether the element children or a value.
        /// </summary>
        public bool IsEmpty => this.Children.Count == 0 && string.IsNullOrEmpty(this.Value);

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets namespace of the element.
        /// </summary>
        public string Namespace { get; set; } = string.Empty;

        /// <summary>
        /// Gets namespace and name of the element.
        /// </summary>
        public string NamespaceAndName
        {
            get
            {
                if (string.IsNullOrEmpty(this.Namespace))
                {
                    return this.Name;
                }

                return $"{this.Namespace}:{this.Name}";
            }
        }

        /// <summary>
        /// Gets or sets the attributs on the element.
        /// </summary>
        public List<Attribut> Attributs { get; set; } = new List<Attribut>();

        /// <summary>
        /// Gets a value indicating whether the element has attributs.
        /// </summary>
        public bool HasAttributs => this.Attributs.Count > 0;

        /// <summary>
        /// Gets or sets the children of the element.
        /// </summary>
        public List<Element> Children { get; set; } = new List<Element>();

        /// <summary>
        /// Gets or sets the parent element of the element.
        /// </summary>
        public Element? ParentElement { get; set; }

        /// <summary>
        /// Gets or sets the value of the element.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the element has an end tag.
        /// </summary>
        public bool HasEndTag { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether if the element is a document.
        /// </summary>
        public bool IsDocument { get; set; } = false;

        /// <summary>
        /// Get the attribut if it exists.
        /// </summary>
        /// <param name="name">Attribut name to find.</param>
        /// <param name="attribut">Matched attribut.</param>
        /// <returns>return true if attribut exist.</returns>
        public bool GetAttribut([NotNull] string name, [NotNullWhen(true)] out Attribut attribut)
        {
            attribut = this.Attributs.FirstOrDefault(attr => string.Equals(attr.Name, name, StringComparison.OrdinalIgnoreCase));
            return attribut != null;
        }

        /// <summary>
        /// Get the attribut value if it exists.
        /// </summary>
        /// <param name="name">Attribut name to find.</param>
        /// <param name="value">value of the attribut.</param>
        /// <returns>if attribut exist.</returns>
        public bool GetAttributValue([NotNull] string name, [NotNull] out string value)
        {
            if (this.GetAttribut(name, out Attribut? attribut))
            {
                value = attribut.Value;
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Get the attribut value or an empty string if it does not exist.
        /// </summary>
        /// <param name="name">name of the attribut.</param>
        /// <returns>value of the attribut.</returns>
        public string GetAttributValue([NotNull] string name)
        {
            if (this.GetAttributValue(name, out string value))
            {
                return value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get attribut value as a specific type.
        /// </summary>
        /// <typeparam name="T">Type to convert the value to.</typeparam>
        /// <param name="name">name of the attribut.</param>
        /// <returns>Attribut value of the element that matches css selector.</returns>
        public T GetAttributValueAs<T>([NotNull] string name) where T : IConvertible
        {
            if (this.GetAttributValue(name, out string attributValue))
            {
                return (T)Convert.ChangeType(attributValue, typeof(T));
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)string.Empty;
            }

#pragma warning disable CS8603 // Possible null reference return.
            return default;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Get all elements that matches the query.
        /// </summary>
        /// <param name="query">css selecter.</param>
        /// <returns>Elements that matches css selector.</returns>
        public List<Element> Query([NotNull] string query)
        {
            return QueryEngine.Query(this, query);
        }

        /// <summary>
        /// Get the first element that matches the query.
        /// </summary>
        /// <param name="query">css selecter.</param>
        /// <returns>Element that matches css selector.</returns>
        public Element? QueryFirst([NotNull] string query)
        {
            return QueryEngine.Query(this, query).FirstOrDefault();
        }

        /// <summary>
        /// Get the first element that matches the query.
        /// </summary>
        /// <param name="query">CSS selecter.</param>
        /// <param name="element">Element that matches css selector.</param>
        /// <returns>return true if element exist.</returns>
        public bool QueryFirst([NotNull] string query, [NotNullWhen(true)] out Element element)
        {
            element = QueryEngine.Query(this, query).FirstOrDefault();
            return element != null;
        }

        /// <summary>
        /// Get the value of the first element that matches the query.
        /// </summary>
        /// <param name="query">css selecter.</param>
        /// <returns>value of the element that matches css selector.</returns>
        public string QueryValue([NotNull] string query)
        {
            return this.QueryFirst(query)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Get xml representation of the element and its children.
        /// </summary>
        /// <returns>Generated xml from element.</returns>
        public string ToXml()
        {
            StringBuilder xml = new StringBuilder();
            this.ToXml(xml);
            return xml.ToString();
        }

        /// <summary>
        /// Get xml representation of the element and its children.
        /// </summary>
        /// <param name="xml">writer to write xml to.</param>
        /// <param name="indent">indent level.</param>
        public void ToXml(StringBuilder xml, int indent = 0)
        {
            string endTag = this.HasEndTag ? $">" : "/>";

            string indentString = indent == 0 ? string.Empty : new string(' ', indent * 4);

            if (this.IsDocument)
            {
                indent = -1;
            }

            if (this.IsDocument == false)
            {
                if (this.Attributs.Count > 0)
                {
                    string startTagXML = $"{indentString}<{this.NamespaceAndName} {string.Join(' ', this.Attributs.Select(attr => $"{attr.Name}=\"{attr.Value}\""))}{endTag}";
                    if (this.Children.Count > 0 || this.HasEndTag == false)
                    {
                        xml.AppendLine(startTagXML);
                    }

                    if (this.Children.Count == 0 && this.HasEndTag)
                    {
                        xml.Append(startTagXML);
                    }
                }
                else
                {
                    string endTagXML = $"{indentString}<{this.NamespaceAndName}{endTag}";
                    if (this.Children.Count > 0 || this.HasEndTag == false)
                    {
                        xml.AppendLine(endTagXML);
                    }

                    if (this.Children.Count == 0 && this.HasEndTag)
                    {
                        xml.Append(endTagXML);
                    }
                }
            }

            if (this.Children.Count > 0)
            {
                foreach (Element child in this.Children)
                {
                    child.ToXml(xml, indent + 1);
                }
            }
            else
            {
                if (this.Value.IndexOfAny(new char[] { '<', '>', '&' }) != -1)
                {
                    xml.Append($"<![CDATA[{this.Value}]]>");
                }
                else
                {
                    xml.Append($"{this.Value.Trim()}");
                }
            }

            if (this.IsDocument == false && this.Children.Count > 0 && this.HasEndTag)
            {
                xml.AppendLine($"{indentString}</{this.NamespaceAndName}>");
            }
            else if (this.IsDocument == false && this.Children.Count == 0 && this.HasEndTag)
            {
                xml.AppendLine($"</{this.NamespaceAndName}>");
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"<{this.Name} {string.Join(' ', this.Attributs.Select(attr => $"{attr.Name}=\"{attr.Value}\""))}>";
        }
    }
}
