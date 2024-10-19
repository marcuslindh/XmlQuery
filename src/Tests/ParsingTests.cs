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
                string title = tvshow.QueryValue("title");
                string showtitle = tvshow.QueryValue("showtitle");
                ushort year = Convert.ToUInt16(tvshow.QueryValue("year"));
                decimal rating = Convert.ToDecimal(tvshow.QueryValue("rating"));
                decimal userrating = Convert.ToDecimal(tvshow.QueryValue("userrating"));
                ushort votes = Convert.ToUInt16(tvshow.QueryValue("votes"));
                string plot = tvshow.QueryValue("plot");
                byte runtime = Convert.ToByte(tvshow.QueryValue("runtime"));
                uint id = Convert.ToUInt32(tvshow.QueryValue("id"));
                string imdbid = tvshow.QueryValue("imdbid");
                DateTime premiered = Convert.ToDateTime(tvshow.QueryValue("premiered"));
                string status = tvshow.QueryValue("status");
                bool watched = Convert.ToBoolean(tvshow.QueryValue("watched"));
                string playcount = tvshow.QueryValue("playcount");
                string dateadded = tvshow.QueryValue("dateadded");
            }
        }
    }
}