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

            Assert.NotNull(xmlReader.Document);
        }

        [Fact]
        public void ParsingNyProjekt()
        {
            XmlReader xmlReader = new XmlReader();
            xmlReader.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data", "nyaprojekt_se.xml")));

            Assert.NotNull(xmlReader.Document);
        }
    }
}