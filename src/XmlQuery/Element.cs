﻿using System;
using System.Collections.Generic;
using System.Linq;
using XmlQuery.Core;

namespace XmlQuery
{
    public class Element
    {
        public string Name { get; set; } = "";
        public string Namespace { get; set; } = "";
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
        public List<Attribut> Attributs { get; set; } = new List<Attribut>();
        public List<Element> Children { get; set; } = new List<Element>();
        public Element? ParentElement { get; set; }
        public string Value { get; set; } = "";
        public bool HasEndTag { get; set; } = true;

        public bool GetAttribut(string name, out Attribut attribut)
        {
            attribut = Attributs.FirstOrDefault(x => x.Name == name);
            return attribut != null;
        }

        public bool GetAttributValue(string name, out string value)
        {
            if (GetAttribut(name, out Attribut attribut))
            {
                value = attribut.Value;
                return true;
            }

            value = "";
            return false;
        }

        public T GetAttributValueAs<T>(string name) where T : IConvertible
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

        public List<Element> Query(string query)
        {
            return QueryEngine.Query(this, query);
        }

        public override string ToString()
        {
            return $"<{Name} {string.Join(" ", Attributs.Select(x => $"{x.Name}=\"{x.Value}\""))}>";
        }
    }
}
