using XmlQuery;
using XmlQuery.Query;
using XmlQuery.Xml;

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

            Assert.Equal(1, tokens.Count);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("rss", tokens[0].Value);
        }

        [Fact]
        public void QueryParser_TagNameAndAllAfter()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss *");

            Assert.Equal(2, tokens.Count);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("rss", tokens[0].Value);

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("*", tokens[1].Value);
        }

        [Fact]
        public void QueryParser_3TagNames()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss item title");

            Assert.Equal(3, tokens.Count);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("rss", tokens[0].Value);

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("item", tokens[1].Value);

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("title", tokens[2].Value);
        }

        [Fact]
        public void QueryParser_TagNameAndFirstTagMustBeTitle()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel > title");

            Assert.Equal(3, tokens.Count);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("channel", tokens[0].Value);

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.FirstTagAfterArrow);
            Assert.Equal(">", tokens[1].Value);

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("title", tokens[2].Value);
        }

        [Fact]
        public void QueryParser_TagNameOrTagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel | title");

            Assert.Equal(3, tokens.Count);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("channel", tokens[0].Value);

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.Or);
            Assert.Equal("|", tokens[1].Value);

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("title", tokens[2].Value);
        }

        [Fact]
        public void QueryParser_TagNameAndAttribut()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("link[type=\"application/rss+xml\"]");

            Assert.Equal(5, tokens.Count);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("link", tokens[0].Value);

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.StartAttributFilter);
            Assert.Equal("[", tokens[1].Value);

            Assert.True(tokens[2].Type == QueryEngineToken.TokenType.AttributName);
            Assert.Equal("type", tokens[2].Value);

            Assert.True(tokens[3].Type == QueryEngineToken.TokenType.AttributValue);
            Assert.Equal("application/rss+xml", tokens[3].Value);

            Assert.True(tokens[4].Type == QueryEngineToken.TokenType.EndAttributFilter);
            Assert.Equal("]", tokens[4].Value);
        }

        [Fact]
        public void QueryParser_TagUnderTag()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel title");

            Assert.Equal(2, tokens.Count);
            Assert.True(tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("channel", tokens[0].Value);

            Assert.True(tokens[1].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("title", tokens[1].Value);
        }

        [Fact]
        public void GroupTokens_TagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss");

            List<QueryEngineTokenGroup> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.Equal(1, groupTokens.Count);
            Assert.True(groupTokens[0].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(1, groupTokens[0].Tokens.Count);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("rss", groupTokens[0].Tokens[0].Value);
        }

        [Fact]
        public void GroupTokens_TagNameAndAllAfter()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("rss *");

            List<QueryEngineTokenGroup> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.Equal(2, groupTokens.Count);
            Assert.True(groupTokens[0].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("rss", groupTokens[0].Tokens[0].Value);
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("*", groupTokens[1].Tokens[0].Value);
        }

        [Fact]
        public void GroupTokens_TagNameAndFirstTagMustBeTitle()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel > title");

            List<QueryEngineTokenGroup> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.Equal(3, groupTokens.Count);
            Assert.True(groupTokens[0].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(1, groupTokens[0].Tokens.Count);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("channel", groupTokens[0].Tokens[0].Value);

            Assert.True(groupTokens[1].Type == QueryEngineTokenGroup.GroupType.FirstTagAfterArrow);
            Assert.Equal(1, groupTokens[1].Tokens.Count);
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.FirstTagAfterArrow);
            Assert.Equal(">", groupTokens[1].Tokens[0].Value);

            Assert.True(groupTokens[2].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(1, groupTokens[2].Tokens.Count);
            Assert.True(groupTokens[2].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("title", groupTokens[2].Tokens[0].Value);
        }

        [Fact]
        public void GroupTokens_TagNameOrTagName()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel | title");

            List<QueryEngineTokenGroup> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.Equal(3, groupTokens.Count);
            Assert.True(groupTokens[0].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(1, groupTokens[0].Tokens.Count);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("channel", groupTokens[0].Tokens[0].Value);

            Assert.True(groupTokens[1].Type == QueryEngineTokenGroup.GroupType.Or);
            Assert.Equal(1, groupTokens[1].Tokens.Count);
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.Or);
            Assert.Equal("|", groupTokens[1].Tokens[0].Value);

            Assert.True(groupTokens[2].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(1, groupTokens[2].Tokens.Count);
            Assert.True(groupTokens[2].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("title", groupTokens[2].Tokens[0].Value);
        }

        [Fact]
        public void GroupTokens_TagNameAndAttribut()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("link[type=\"application/rss+xml\"]");

            List<QueryEngineTokenGroup> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.Equal(1, groupTokens.Count);
            Assert.True(groupTokens[0].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(5, groupTokens[0].Tokens.Count);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("link", groupTokens[0].Tokens[0].Value);

            Assert.True(groupTokens[0].Tokens[1].Type == QueryEngineToken.TokenType.StartAttributFilter);
            Assert.Equal("[", groupTokens[0].Tokens[1].Value);

            Assert.True(groupTokens[0].Tokens[2].Type == QueryEngineToken.TokenType.AttributName);
            Assert.Equal("type", groupTokens[0].Tokens[2].Value);

            Assert.True(groupTokens[0].Tokens[3].Type == QueryEngineToken.TokenType.AttributValue);
            Assert.Equal("application/rss+xml", groupTokens[0].Tokens[3].Value);

            Assert.True(groupTokens[0].Tokens[4].Type == QueryEngineToken.TokenType.EndAttributFilter);
            Assert.Equal("]", groupTokens[0].Tokens[4].Value);
        }

        [Fact]
        public void GroupTokens_TagUnderTag()
        {
            List<QueryEngineToken> tokens = QueryEngine.ParseQuery("channel title");

            List<QueryEngineTokenGroup> groupTokens = QueryEngine.GroupTokens(tokens);

            Assert.Equal(2, groupTokens.Count);
            Assert.True(groupTokens[0].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(1, groupTokens[0].Tokens.Count);
            Assert.True(groupTokens[0].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("channel", groupTokens[0].Tokens[0].Value);

            Assert.True(groupTokens[1].Type == QueryEngineTokenGroup.GroupType.Element);
            Assert.Equal(1, groupTokens[1].Tokens.Count);
            Assert.True(groupTokens[1].Tokens[0].Type == QueryEngineToken.TokenType.TagName);
            Assert.Equal("title", groupTokens[1].Tokens[0].Value);
        }

    }
}
