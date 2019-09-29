# Javlibrary Client
A javlibrary.com scraper library implemented in C#.

# Usage
```csharp
var client = new Javlibrary.Client();

var video1 = await client.SearchFirst("HND-723");

var results = await client.Search("HND-723");
var video2 = await client.LoadVideo(results.FirstOrDefault().url);

// video1 == video2

foreach(var video in await client.Search("ABP"))
    System.Console.WriteLine(video.Title);
```

# License
Licensed under AGPL-3.0-only
