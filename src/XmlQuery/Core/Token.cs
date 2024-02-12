namespace XmlQuery
{
    namespace Core
    {
        public class Token
        {
            public int pos { get; set; } = 0;
            public string value { get; set; } = "";
            public TokenType type { get; set; } = TokenType.Unknown;

            public enum TokenType
            {
                String,
                StartArrow,
                EndArrow,
                Slash,
                Unknown,
                Character,
                Equal,
                ContiguousCharacters,
                TagName,
                StartTag,
                EndTag,
                AttributName,
                AttributValue,
                Url,
                StartCData,
                CDataValue,
                EndCData
            }

            public override string ToString()
            {
                return $"{pos}: {type} = '{value}'";
            }
        }
    }
}
