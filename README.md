# XmlQuery

XmlQuery works similarly to the JavaScript DOM API. This library reads XML documents in the same way as HTML documents. If the XML document is incorrect or corrupt, the library ignores the errors and continues reading.

# Installation

Install the NuGet package

# Exempel

```csharp
XmlReader xmlReader = new XmlReader(xml);

foreach (Element item in xmlReader.Query("item"))
{
    RssPost post = new RssPost();
    post.Title = item.QueryFirst("title")?.Value;

    post.Description = item.QueryValue("description");

    if (item.QueryFirst("link", out Element link))
    {
        post.Link = link.Value;
    }

    foreach (Element category in item.Query("category")
    {
        post.Categorys.Add(category.Value);
    }
}
```

## Match tag with attribut

```csharp
xmlReader.Query("item[type=Page]")
```

## Match first tag

```csharp
xmlReader.Query("item > link")
```
