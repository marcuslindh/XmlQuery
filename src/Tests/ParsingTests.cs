using XmlQuery;
namespace Tests
{
    public class ParsingTests
    {
        [Fact]
        public void ParsingBooks()
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "books.xml")));

            Assert.NotNull(xmlReader.Document);
        }

        [Fact]
        public void ParsingNyProjekt()
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "nyaprojekt_se.xml")));

            Assert.NotNull(xmlReader.Document);
        }

        [Fact]
        public void ParsingTvShow()
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "tvshow.nfo")));

            Assert.NotNull(xmlReader.Document);

            foreach (XmlQuery.Xml.Element tvshow in xmlReader.Query("tvshow"))
            {
                Assert.NotEmpty(tvshow.QueryValue("title"));
                Assert.NotEmpty(tvshow.QueryValue("showtitle"));
                Assert.True(Convert.ToUInt16(tvshow.QueryValue("year")) > 0);
                Assert.NotEmpty(tvshow.QueryValue("plot"));
                Assert.Equal(0, int.Parse(tvshow.QueryValue("runtime")));
                Assert.NotEmpty(tvshow.QueryValue("premiered"));
            }
        }

        [Fact]
        public void ParsingTvShow2()
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "tvshow2.nfo")));

            Assert.NotNull(xmlReader.Document);

            foreach (XmlQuery.Xml.Element tvshow in xmlReader.Query("tvshow"))
            {
                Assert.NotEmpty(tvshow.QueryValue("title"));
                Assert.NotEmpty(tvshow.QueryValue("showtitle"));
                Assert.True(Convert.ToUInt16(tvshow.QueryValue("year")) > 0);
                Assert.NotEmpty(tvshow.QueryValue("plot"));
                Assert.Equal(43, int.Parse(tvshow.QueryValue("runtime")));

                string thumb = tvshow.QueryFirst("thumb").Value;

                Assert.Equal("https://image.tmdb.org/t/p/original/c5B9CdRs9MY6Fgknv5uZQKWtl6F.jpg", thumb);
                Assert.NotEmpty(tvshow.QueryValue("premiered"));
            }
        }
    }
}