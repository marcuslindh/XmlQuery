using System.Collections.Generic;

namespace XmlQuery
{
    namespace Core
    {
        public class TokenGroup
        {
            public List<Token> Tokens { get; set; } = new List<Token>();

            public TokenGroupType Type { get; set; } = TokenGroupType.Unknown;

            public string Name { get; set; } = "";

            public enum TokenGroupType
            {
                StartTag,
                EndTag,
                StartAndEndTag,
                Value,
                Unknown
            }

            public override string ToString()
            {
                return $"{Type} {Name}";
            }
        }
    }
}
