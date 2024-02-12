using XmlQuery;
using XmlQuery.Core;

namespace Tests
{
    public class QueryEngineTests
    {

        [Theory]
        [InlineData("rss", 1)]
        [InlineData("item", 21)]
        [InlineData("item > title", 21)]
        [InlineData("item title", 21)]
        [InlineData("item > guid[isPermaLink=\"false\"]", 21)]
        public void GetElementsByName(string tagName, int count)
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "nyaprojekt_se.xml")));

            List<Element> elements = xmlReader.Query(tagName);

            Assert.True(elements.Count == count);
        }

        [Fact]
        public void QueryParser_TagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss");

            Assert.True(tokens.Count == 1);
            Assert.True(tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].value == "rss");
        }

        [Fact]
        public void QueryParser_TagNameAndAllAfter()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss *");

            Assert.True(tokens.Count == 2);
            Assert.True(tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].value == "rss");

            Assert.True(tokens[1].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[1].value == "*");
        }

        [Fact]
        public void QueryParser_TagNameAndFirstTagMustBeTitle()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel > title");

            Assert.True(tokens.Count == 3);
            Assert.True(tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].value == "channel");

            Assert.True(tokens[1].type == QueryEngineToken.TokenType.FirstTagAfterArrow);
            Assert.True(tokens[1].value == ">");

            Assert.True(tokens[2].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[2].value == "title");
        }

        [Fact]
        public void QueryParser_TagNameOrTagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel | title");

            Assert.True(tokens.Count == 3);
            Assert.True(tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].value == "channel");

            Assert.True(tokens[1].type == QueryEngineToken.TokenType.Or);
            Assert.True(tokens[1].value == "|");

            Assert.True(tokens[2].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[2].value == "title");
        }

        [Fact]
        public void QueryParser_TagNameAndAttribut()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("link[type=\"application/rss+xml\"]");

            Assert.True(tokens.Count == 5);
            Assert.True(tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].value == "link");

            Assert.True(tokens[1].type == QueryEngineToken.TokenType.StartAttributFilter);
            Assert.True(tokens[1].value == "[");

            Assert.True(tokens[2].type == QueryEngineToken.TokenType.AttributName);
            Assert.True(tokens[2].value == "type");  

            Assert.True(tokens[3].type == QueryEngineToken.TokenType.AttributValue);
            Assert.True(tokens[3].value == "application/rss+xml");

            Assert.True(tokens[4].type == QueryEngineToken.TokenType.EndAttributFilter);
            Assert.True(tokens[4].value == "]");
        }

        [Fact]
        public void QueryParser_TagUnderTag()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel title");

            Assert.True(tokens.Count == 2);
            Assert.True(tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].value == "channel");

            Assert.True(tokens[1].type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[1].value == "title");
        }

        [Fact]
        public void GroupTokens_TagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 1);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].value == "rss");
        }

        [Fact]
        public void GroupTokens_TagNameAndAllAfter()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss *");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 2);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].value == "rss");

            Assert.True(groupTokens[1].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[1].Tokens.Count == 1);
            Assert.True(groupTokens[1].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[1].Tokens[0].value == "*");
        }

        [Fact]
        public void GroupTokens_TagNameAndFirstTagMustBeTitle()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel > title");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 3);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].value == "channel");

            Assert.True(groupTokens[1].Type == QueryEngineGroupToken.GroupType.FirstTagAfterArrow);
            Assert.True(groupTokens[1].Tokens.Count == 1);
            Assert.True(groupTokens[1].Tokens[0].type == QueryEngineToken.TokenType.FirstTagAfterArrow);
            Assert.True(groupTokens[1].Tokens[0].value == ">");

            Assert.True(groupTokens[2].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[2].Tokens.Count == 1);
            Assert.True(groupTokens[2].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[2].Tokens[0].value == "title");
        }

        [Fact]
        public void GroupTokens_TagNameOrTagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel | title");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 3);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].value == "channel");

            Assert.True(groupTokens[1].Type == QueryEngineGroupToken.GroupType.Or);
            Assert.True(groupTokens[1].Tokens.Count == 1);
            Assert.True(groupTokens[1].Tokens[0].type == QueryEngineToken.TokenType.Or);
            Assert.True(groupTokens[1].Tokens[0].value == "|");

            Assert.True(groupTokens[2].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[2].Tokens.Count == 1);
            Assert.True(groupTokens[2].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[2].Tokens[0].value == "title");
        }

        [Fact]
        public void GroupTokens_TagNameAndAttribut()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("link[type=\"application/rss+xml\"]");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 1);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 5);
            Assert.True(groupTokens[0].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].value == "link");

            Assert.True(groupTokens[0].Tokens[1].type == QueryEngineToken.TokenType.StartAttributFilter);
            Assert.True(groupTokens[0].Tokens[1].value == "[");

            Assert.True(groupTokens[0].Tokens[2].type == QueryEngineToken.TokenType.AttributName);
            Assert.True(groupTokens[0].Tokens[2].value == "type");

            Assert.True(groupTokens[0].Tokens[3].type == QueryEngineToken.TokenType.AttributValue);
            Assert.True(groupTokens[0].Tokens[3].value == "application/rss+xml");

            Assert.True(groupTokens[0].Tokens[4].type == QueryEngineToken.TokenType.EndAttributFilter);
            Assert.True(groupTokens[0].Tokens[4].value == "]");
        }

        [Fact]
        public void GroupTokens_TagUnderTag()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel title");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 2);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].value == "channel");

            Assert.True(groupTokens[1].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[1].Tokens.Count == 1);
            Assert.True(groupTokens[1].Tokens[0].type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[1].Tokens[0].value == "title");
        }

    }
}
