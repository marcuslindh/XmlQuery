using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using XmlQuery.Core;

namespace XmlQuery
{
    public class Element
    {
        /// <summary>
        /// The name of the element
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// namespace of the element
        /// </summary>
        public string Namespace { get; set; } = "";

        /// <summary>
        /// namespace and name of the element
        /// </summary>
        public string NamespaceAndName
        {
            get
            {
                if (string.IsNullOrEmpty(Namespace))
                {
                    return Name;
                }

                return $"{Namespace}:{Name}";
            }
        }

        /// <summary>
        /// The attributs on the element
        /// </summary>
        public List<Attribut> Attributs { get; set; } = new List<Attribut>();

        /// <summary>
        /// The children of the element
        /// </summary>
        public List<Element> Children { get; set; } = new List<Element>();

        /// <summary>
        /// The parent element of the element
        /// </summary>
        public Element? ParentElement { get; set; }

        /// <summary>
        /// The value of the element
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// If the element has an end tag
        /// </summary>
        public bool HasEndTag { get; set; } = true;

        /// <summary>
        /// If the element is a document
        /// </summary>
        public bool IsDocument { get; set; } = false;

        /// <summary>
        /// Get the attribut if it exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attribut"></param>
        /// <returns></returns>
        public bool GetAttribut([NotNull] string name, [NotNullWhen(true)] out Attribut? attribut)
        {
            attribut = Attributs.FirstOrDefault(x => x.Name == name);
            return attribut != null;
        }

        /// <summary>
        /// Get the attribut value if it exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetAttributValue([NotNull] string name, [NotNull] out string value)
        {
            if (GetAttribut(name, out Attribut attribut))
            {
                value = attribut.Value;
                return true;
            }

            value = "";
            return false;
        }

        /// <summary>
        /// Get the attribut value or an empty string if it does not exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAttributValue([NotNull] string name)
        {
            if (GetAttributValue(name, out string value))
            {
                return value;
            }

            return "";
        }

        /// <summary>
        /// Get attribut value as a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetAttributValueAs<T>([NotNull] string name) where T : IConvertible
        {
            if (GetAttributValue(name, out string attributValue))
            {
                return (T)System.Convert.ChangeType(attributValue, typeof(T));

            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)String.Empty;
            }

            return default(T);
        }

        /// <summary>
        /// Get all elements that matches the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Element> Query([NotNull] string query)
        {
            return QueryEngine.Query(this, query);
        }

        /// <summary>
        /// Get the first element that matches the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Element? QueryFirst([NotNull] string query)
        {
            return QueryEngine.Query(this, query).FirstOrDefault();
        }

        /// <summary>
        /// Get the value of the first element that matches the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string QueryValue([NotNull] string query)
        {
            return QueryFirst(query)?.Value ?? "";
        }

        /// <summary>
        /// Get xml representation of the element and its children
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            StringBuilder xml = new StringBuilder();
            ToXml(xml);
            return xml.ToString();

        }

        /// <summary>
        /// Get xml representation of the element and its children
        /// </summary>
        public void ToXml(StringBuilder xml, int indent = 0)
        {
            string endTag = HasEndTag ? $">" : "/>";



            string indentString = indent == 0 ? "" : new string(' ', indent * 4);

            if (IsDocument) indent = -1;

            if (IsDocument == false)
            {
                if (Attributs.Count > 0)
                {
                    string _xml = $"{indentString}<{NamespaceAndName} {string.Join(" ", Attributs.Select(x => $"{x.Name}=\"{x.Value}\""))}{endTag}";
                    if (Children.Count > 0 || HasEndTag == false) xml.AppendLine(_xml);
                    if (Children.Count == 0 && HasEndTag) xml.Append(_xml);

                }
                else
                {
                    string _xml = $"{indentString}<{NamespaceAndName}{endTag}";
                    if (Children.Count > 0 || HasEndTag == false) xml.AppendLine(_xml);
                    if (Children.Count == 0 && HasEndTag) xml.Append(_xml);
                }
            }

            if (Children.Count > 0)
            {
                foreach (Element child in Children)
                {
                    child.ToXml(xml, indent + 1);
                }
            }
            else
            {
                if (Value.IndexOfAny(new char[] { '<', '>', '&' }) != -1)
                {
                    xml.Append($"<![CDATA[{Value}]]>");
                }
                else
                {
                    xml.Append($"{Value.Trim()}");
                }
            }

            if (IsDocument == false && Children.Count > 0 && HasEndTag)
            {
                xml.AppendLine($"{indentString}</{NamespaceAndName}>");
            }
            else if (IsDocument == false && Children.Count == 0 && HasEndTag)
            {
                xml.AppendLine($"</{NamespaceAndName}>");
            }


        }

        public override string ToString()
        {
            return $"<{Name} {string.Join(" ", Attributs.Select(x => $"{x.Name}=\"{x.Value}\""))}>";
        }
    }
}
