using XmlQuery;
namespace Tests
{
    public class ParsingTests
    {
        [Fact]
        public void ParsingBooks()
        {      
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data","books.xml")));

            Assert.True(xmlReader.Document != null);
        }

        [Fact]
        public void ParsingNyProjekt()
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "nyaprojekt_se.xml")));

            Assert.True(xmlReader.Document != null);
        }
    }
}