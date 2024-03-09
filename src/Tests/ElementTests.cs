using XmlQuery;
using XmlQuery.Core;

namespace Tests
{
    public class ElementTests
    {

        [Fact]
        public void TestGetAttribut()
        {
            Element element = new Element();
            element.Attributs.Add(new Attribut { Name = "foo", Value = "bar" });
            element.Attributs.Add(new Attribut { Name = "bar", Value = "foo" });

            Assert.True(element.GetAttribut("foo", out Attribut attribut));
            Assert.Equal("bar", attribut.Value);

            Assert.True(element.GetAttribut("bar", out attribut));
            Assert.Equal("foo", attribut.Value);

            Assert.False(element.GetAttribut("baz", out attribut));
            Assert.Null(attribut);
        }

        [Fact]
        public void TestGetAttributValue()
        {
            Element element = new Element();
            element.Attributs.Add(new Attribut { Name = "foo", Value = "bar" });
            element.Attributs.Add(new Attribut { Name = "bar", Value = "foo" });

            Assert.True(element.GetAttributValue("foo", out string value));
            Assert.Equal("bar", value);

            Assert.True(element.GetAttributValue("bar", out value));
            Assert.Equal("foo", value);

            Assert.False(element.GetAttributValue("baz", out value));
            Assert.Equal("", value);
        }

        [Fact]
        public void TestGetAttributValueAs()
        {
            Element element = new Element();
            element.Attributs.Add(new Attribut { Name = "foo", Value = "bar" });
            element.Attributs.Add(new Attribut { Name = "bar", Value = "50" });

            Assert.Equal("bar", element.GetAttributValueAs<string>("foo"));

            Assert.Equal("50", element.GetAttributValueAs<string>("bar"));

            Assert.Equal("", element.GetAttributValueAs<string>("baz"));

            Assert.Equal(0, element.GetAttributValueAs<int>("baz"));
        }

        [Fact]
        public void TestNamespaceAndName()
        {
            Element element = new Element();
            element.Name = "foo";
            element.Namespace = "bar";

            Assert.Equal("bar:foo", element.NamespaceAndName);

            element.Namespace = "";

            Assert.Equal("foo", element.NamespaceAndName);
        }

        [Fact]
        public void TestHasEndTag()
        {
            Element element = new Element();
            Assert.True(element.HasEndTag);

            element.HasEndTag = false;
            Assert.False(element.HasEndTag);
        }

        [Fact]
        public void TestValue()
        {
            Element element = new Element();
            Assert.Equal("", element.Value);

            element.Value = "foo";
            Assert.Equal("foo", element.Value);
        }

        [Fact]
        public void TestParentElement()
        {
            Element element = new Element();
            Assert.Null(element.ParentElement);

            element.ParentElement = new Element();
            Assert.NotNull(element.ParentElement);
        }

        [Fact]
        public void TestChildren()
        {
            Element element = new Element();
            Assert.Empty(element.Children);

            element.Children.Add(new Element());
            Assert.Single(element.Children);
        }

        [Fact]
        public void TestName()
        {
            Element element = new Element();
            Assert.Equal("", element.Name);

            element.Name = "foo";
            Assert.Equal("foo", element.Name);
        }

        [Fact]
        public void TestNamespace()
        {
            Element element = new Element();
            Assert.Equal("", element.Namespace);

            element.Namespace = "foo";
            Assert.Equal("foo", element.Namespace);
        }

        [Fact]
        public void TestQuery()
        {
            Element element = new Element();
            element.Children.Add(new Element { Name = "foo" });
            element.Children.Add(new Element { Name = "foo" });
            element.Children.Add(new Element { Name = "foo" });
            element.Children.Add(new Element { Name = "bar" });
            element.Children.Add(new Element { Name = "baz" });
            element.Children.Add(new Element { Name = "baz" });

            Assert.Equal(3, element.Query("foo").Count);
            Assert.Equal(2, element.Query("baz").Count);
            Assert.Single(element.Query("bar"));
            Assert.Empty(element.Query("qux"));
        }

        [Theory]
        [InlineData("item", 21)]
        [InlineData("image", 1)]
        [InlineData("lastBuildDate", 1)]
        public void TestQueryTag(string query, int count)
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "nyaprojekt_se.xml")));

            Element root = xmlReader.Query("rss").First();

            List<Element> elements = root.Query(query);

            Assert.Equal(count, elements.Count);

        }

        [Fact]
        public void TestGetAttributValueAsInt()
        {

            string xml = """
                <item>
                    <Title type="markdown" count="50">test</Title>
                </item>
                """;

            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(xml);

            Element root = xmlReader.Query("item Title").First();


            Assert.Equal(50, root.GetAttributValueAs<int>("count"));
            Assert.Equal("markdown", root.GetAttributValueAs<string>("type"));



        }

        [Fact]
        public void TestParseRss()
        {
            string path = @"I:\Temp\19\andrewlock.net\145884EB4AF604D110E533BE6012508517BAB58F.xml";
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(path));

            List<Element> items = xmlReader.Query("item");

            Assert.Equal(25, items.Count);

            foreach (Element item in items)
            {

                List<Element> links = item.Query("link");

                Assert.True(item.Query("title").Count > 0);
                Assert.NotEmpty(item.Query("title").First().Value);

            }
        }

        [Fact]
        public void TestCData()
        {
            string xml = """
                <item>
                    <Title type="markdown" count="50"><![CDATA[<h1>test</h1>]]></Title>
                </item>
                """;

            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(xml);

            Element root = xmlReader.Query("item Title").First();

            Assert.Equal("<h1>test</h1>", root.Value);
        }

        [Fact]
        public void TestRssItem()
        {
            string xml = """
                <channel>
                    <title><![CDATA[Andrew Lock | .NET Escapades]]></title>
                    <description><![CDATA[Hi, my name is Andrew, or ‘Sock’ to most people. This blog is where I share my experiences as I journey into ASP.NET Core.]]></description>
                    <link>https://andrewlock.net/</link>
                    <generator>andrewlock-blog-engine</generator>
                    <lastBuildDate>Tue, 13 Feb 2024 10:15:41 GMT</lastBuildDate>
                    <atom:link href="https://andrewlock.net/rss/" rel="self" type="application/rss+xml"/>
                    <ttl>60</ttl>
                    <item>
                	    <title><![CDATA[8 ways to set the URLs for an ASP.NET Core app]]></title>
                	    <description><![CDATA[In this post I describe 8 different ways to set which URLs your ASP.NET Core application listens on.]]></description>
                	    <link>https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/</link>
                	    <guid isPermaLink="true">https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/</guid>
                	    <pubDate>Tue, 13 Feb 2024 10:00:00 GMT</pubDate>
                	    <dc:creator><![CDATA[Andrew Lock]]></dc:creator>
                    </item>
                </channel>
                """;


            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(xml);

            //Assert.NotEmpty(xmlReader.Query("channel title").First().Value);
            Assert.NotEmpty(xmlReader.Query("channel item title").First().Value);

        }

        [Fact]
        public void TestMatchElementInElement()
        {
            string xml = """
                <channel>
                    <title><![CDATA[Andrew Lock | .NET Escapades]]></title>
                    <description><![CDATA[Hi, my name is Andrew, or ‘Sock’ to most people. This blog is where I share my experiences as I journey into ASP.NET Core.]]></description>
                    <link>https://andrewlock.net/</link>
                    <generator>andrewlock-blog-engine</generator>
                    <lastBuildDate>Tue, 13 Feb 2024 10:15:41 GMT</lastBuildDate>
                    <atom:link href="https://andrewlock.net/rss/" rel="self" type="application/rss+xml"/>
                    <ttl>60</ttl>
                    <item>
                	    <title><![CDATA[8 ways to set the URLs for an ASP.NET Core app]]></title>
                	    <description><![CDATA[In this post I describe 8 different ways to set which URLs your ASP.NET Core application listens on.]]></description>
                	    <link>https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/</link>
                	    <guid isPermaLink="true">https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/</guid>
                	    <pubDate>Tue, 13 Feb 2024 10:00:00 GMT</pubDate>
                	    <dc:creator><![CDATA[Andrew Lock]]></dc:creator>
                    </item>
                     <item>
                	    <title><![CDATA[7 ways to set the URLs for an ASP.NET Core app]]></title>
                	    <description><![CDATA[In this post I describe 8 different ways to set which URLs your ASP.NET Core application listens on.]]></description>
                	    <link>https://andrewlock.net/7-ways-to-set-the-urls-for-an-aspnetcore-app/</link>
                	    <guid isPermaLink="true">https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/</guid>
                	    <pubDate>Tue, 13 Feb 2024 10:00:00 GMT</pubDate>
                	    <dc:creator><![CDATA[Andrew Lock]]></dc:creator>
                    </item>
                </channel>
                """;


            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(xml);

            Assert.True(xmlReader.Query("item").Count == 2);

            foreach (Element item in xmlReader.Query("item"))
            {
                List<Element> link = item.Query("link");

                Assert.True(link.Count == 1);

            }
        }

        [Fact]
        public void TestCDATAValue()
        {
            string xml = """
                <channel>
                    <title><![CDATA[Andrew Lock | .NET Escapades]]></title>
                    <description><![CDATA[Hi, my name is Andrew, or ‘Sock’ to most people. This blog is where I share my experiences as I journey into ASP.NET Core.]]></description>
                    <link>https://andrewlock.net/</link>
                    <generator>andrewlock-blog-engine</generator>
                    <lastBuildDate>Tue, 13 Feb 2024 10:15:41 GMT</lastBuildDate>
                    <atom:link href="https://andrewlock.net/rss/" rel="self" type="application/rss+xml"/>
                    <ttl>60</ttl>
                    <item>
                	    <title><![CDATA[8 ways to set the URLs for an ASP.NET Core app]]></title>
                	    <description><![CDATA[In this post I describe 8 different ways to set which URLs your ASP.NET Core application listens on.]]></description>
                	    <link>https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/</link>
                	    <guid isPermaLink="true">https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/</guid>
                	    <pubDate>Tue, 13 Feb 2024 10:00:00 GMT</pubDate>
                	    <dc:creator><![CDATA[Andrew Lock]]></dc:creator>
                    </item>
                </channel>
                """;


            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(xml);



            foreach (Element item in xmlReader.Query("item"))
            {
                Element link = item.QueryFirst("link");

                Assert.True(link.Value == "https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/");

            }
        }

        [Fact]
        public void TestRssAnandtech()
        {
            string path = @"I:\Temp\19\anandtech.com\ABC886DF1B20DE50D645457CA95D2A735866A4CA.xml";
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(path));

            foreach (Element item in xmlReader.Query("item"))
            {

                List<Element> links = item.Query("link");

                Assert.True(links.Count > 0);
                Assert.NotEmpty(item.Query("title").First().Value);

            }
        }

        [Fact]
        public void TestRss2()
        {
            XmlReader xmlReader = new XmlReader();
            //string xml = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "nyaprojekt_se.xml"));
            //string xml = File.ReadAllText(@"I:\Temp\19\andrewlock.net\145884EB4AF604D110E533BE6012508517BAB58F.xml");
            //string xml = File.ReadAllText(@"I:\Temp\19\code.blender.org\B6877AE8F1C2D1915612FA184216041335AA9D2C.xml");
            string xml = File.ReadAllText(@"I:\Temp\19\medium.com\838328CD8FB5539DF34DC7DD841C5332023366A3.xml");

            List<Token> tokens = Parser.ParseTokens(xml);

            File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "tokens.txt"), tokens.Select(t => t.ToString()));

            tokens = Parser.CategorizeTokens(tokens);

            File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "CategorizeTokens.txt"), tokens.Select(t => t.ToString()));

            List<TokenGroup> tokenGroups = Parser.GroupTokens(tokens);

            File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "tokenGroups.txt"), tokenGroups.Select(t => t.ToString()));

            Element Document = Parser.GetDocument(tokenGroups);

            File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "XML.txt"), new string[] { Document.ToXml() });


            List<Element> items = QueryEngine.Query(Document, "item");

            Assert.Equal(10, items.Count);
        }

        [Fact]
        public void TestNewReader()
        {
            string xml = """
                <item>
                    <Title type="markdown" count="50"><![CDATA[<h1>test</h1>]]></Title>
                </item>
                """;

            XmlReader xmlReader = new XmlReader(xml);         

            Element root = xmlReader.Query("item Title").First();

            Assert.Equal("<h1>test</h1>", root.Value);
        }




    }
}
