using XmlQuery;

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

            Assert.True(elements.Count == count);

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



    }
}
