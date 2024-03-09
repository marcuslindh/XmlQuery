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
        [InlineData("channel item title", 21)]
        [InlineData("item > guid[isPermaLink=\"false\"]", 21)]
        public void GetElementsByName(string tagName, int count)
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "nyaprojekt_se.xml")));

            List<Element> elements = xmlReader.Query(tagName);

            Assert.Equal(count, elements.Count);
        }

        [Fact]
        public void QueryParser_TagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss");

            Assert.True(tokens.Count == 1);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].Value == "rss");
        }

        [Fact]
        public void QueryParser_TagNameAndAllAfter()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss *");

            Assert.True(tokens.Count == 2);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].Value == "rss");

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[1].Value == "*");
        }

        [Fact]
        public void QueryParser_3TagNames()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss item title");

            Assert.True(tokens.Count == 3);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].Value == "rss");

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[1].Value == "item");

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[2].Value == "title");
        }

        [Fact]
        public void QueryParser_TagNameAndFirstTagMustBeTitle()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel > title");

            Assert.True(tokens.Count == 3);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].Value == "channel");

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.FirstTagAfterArrow);
            Assert.True(tokens[1].Value == ">");

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[2].Value == "title");
        }

        [Fact]
        public void QueryParser_TagNameOrTagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel | title");

            Assert.True(tokens.Count == 3);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].Value == "channel");

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.Or);
            Assert.True(tokens[1].Value == "|");

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[2].Value == "title");
        }

        [Fact]
        public void QueryParser_TagNameAndAttribut()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("link[type=\"application/rss+xml\"]");

            Assert.True(tokens.Count == 5);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].Value == "link");

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.StartAttributFilter);
            Assert.True(tokens[1].Value == "[");

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.AttributName);
            Assert.True(tokens[2].Value == "type");

            Assert.True(tokens[3].Type == QueryEngineToken.TokenType.AttributValue);
            Assert.True(tokens[3].Value == "application/rss+xml");

            Assert.True(tokens[4].Type == QueryEngineToken.TokenType.EndAttributFilter);
            Assert.True(tokens[4].Value == "]");
        }

        [Fact]
        public void QueryParser_TagUnderTag()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel title");

            Assert.True(tokens.Count == 2);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[0].Value == "channel");

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(tokens[1].Value == "title");
        }

        [Fact]
        public void GroupTokens_TagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 1);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].Value == "rss");
        }

        [Fact]
        public void GroupTokens_TagNameAndAllAfter()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss *");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 2);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].Value == "rss");
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[1].Tokens[0].Value == "*");
        }

        [Fact]
        public void GroupTokens_TagNameAndFirstTagMustBeTitle()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel > title");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 3);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].Value == "channel");

            Assert.True(groupTokens[1].Type == QueryEngineGroupToken.GroupType.FirstTagAfterArrow);
            Assert.True(groupTokens[1].Tokens.Count == 1);
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.FirstTagAfterArrow);
            Assert.True(groupTokens[1].Tokens[0].Value == ">");

            Assert.True(groupTokens[2].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[2].Tokens.Count == 1);
            Assert.True(groupTokens[2].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[2].Tokens[0].Value == "title");
        }

        [Fact]
        public void GroupTokens_TagNameOrTagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel | title");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 3);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].Value == "channel");

            Assert.True(groupTokens[1].Type == QueryEngineGroupToken.GroupType.Or);
            Assert.True(groupTokens[1].Tokens.Count == 1);
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.Or);
            Assert.True(groupTokens[1].Tokens[0].Value == "|");

            Assert.True(groupTokens[2].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[2].Tokens.Count == 1);
            Assert.True(groupTokens[2].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[2].Tokens[0].Value == "title");
        }

        [Fact]
        public void GroupTokens_TagNameAndAttribut()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("link[type=\"application/rss+xml\"]");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 1);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 5);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].Value == "link");

            Assert.True(groupTokens[0].Tokens[1].Type == QueryEngineToken.TokenType.StartAttributFilter);
            Assert.True(groupTokens[0].Tokens[1].Value == "[");

            Assert.True(groupTokens[0].Tokens[2].Type == QueryEngineToken.TokenType.AttributName);
            Assert.True(groupTokens[0].Tokens[2].Value == "type");

            Assert.True(groupTokens[0].Tokens[3].Type == QueryEngineToken.TokenType.AttributValue);
            Assert.True(groupTokens[0].Tokens[3].Value == "application/rss+xml");

            Assert.True(groupTokens[0].Tokens[4].Type == QueryEngineToken.TokenType.EndAttributFilter);
            Assert.True(groupTokens[0].Tokens[4].Value == "]");
        }

        [Fact]
        public void GroupTokens_TagUnderTag()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel title");

            List<QueryEngineGroupToken> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.True(groupTokens.Count == 2);
            Assert.True(groupTokens[0].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[0].Tokens.Count == 1);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[0].Tokens[0].Value == "channel");

            Assert.True(groupTokens[1].Type == QueryEngineGroupToken.GroupType.Element);
            Assert.True(groupTokens[1].Tokens.Count == 1);
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.True(groupTokens[1].Tokens[0].Value == "title");
        }

    }
}
