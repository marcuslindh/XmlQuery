

using XmlQuery;

Console.WriteLine("SpeedTest");

string path = @"I:\Temp\19\andrewlock.net\145884EB4AF604D110E533BE6012508517BAB58F.xml";
XmlReader xmlReader = new XmlReader();
xmlReader.Parse(File.ReadAllText(path));

foreach (Element item in xmlReader.Query("item"))
{

    Element? link = item.QueryFirst("link");

    Console.WriteLine(link?.Value);

}
