# Javlibrary Client
A javlibrary.com scraper library implemented in C#.

# Usage
    var client = new Javlibrary.Client();
    var results = await client.Search("HND-723");
    var video = await client.Load(results.FirstOrDefault().url);

# License
Licensed under AGPL-3.0-only
