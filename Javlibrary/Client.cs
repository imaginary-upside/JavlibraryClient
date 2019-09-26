using AngleSharp;
using CloudflareSolverRe;
using System;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Javlibrary
{
    public class Client
    {
        private readonly HttpClient httpClient;
        private readonly IBrowsingContext context;

        public Client()
        {
            var handler = new ClearanceHandler
            {
                MaxTries = 5
            };

            httpClient = new HttpClient(handler);
            context = BrowsingContext.New();
        }

        public async Task<IEnumerable<(string code, Uri url)>> Search(string code)
        {
            var response = await httpClient.GetAsync("http://www.javlibrary.com/en/vl_searchbyid.php?keyword=" + code);
            var html = await response.Content.ReadAsStringAsync();
            var doc = await context.OpenAsync(req => req.Content(html));

            // if only one result was found, and so we were taken directly to the video page.
            if (doc.QuerySelector("#video_id") != null)
            {
                var resultCode = doc.QuerySelector("#video_id .text")?.TextContent;
                var url = new Uri("https://www.javlibrary.com" + doc.QuerySelector("#video_title a")?.GetAttribute("href"));
                return new[] { (resultCode, url) };
            }

            return doc.QuerySelectorAll(".video").Select(n =>
            {
                var resultCode = n.QuerySelector(".id").TextContent;
                var url = new Uri("https://www.javlibrary.com/en/" + n.QuerySelector("a")?.GetAttribute("href"));
                return (resultCode, url);
            });
        }

        public async Task<Video> Load(string id)
        {
            return await Load(new Uri("http://www.javlibrary.com/en/?v=" + id));
        }

        public async Task<Video> Load(Uri url)
        {
            var response = await httpClient.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            var doc = await context.OpenAsync(req => req.Content(html));

            var id = HttpUtility.ParseQueryString(url.Query)["v"];
            var code = doc.QuerySelector("#video_id .text")?.TextContent;
            var actresses = doc.QuerySelectorAll(".star a").Select(n => n.TextContent);
            var title = doc.QuerySelector("#video_title a")?
                           .TextContent
                           .Replace(code, "")
                           // Replace refuses to take empty string as 1st param.
                           // So just using utf zero width char as fallback.
                           .Replace(actresses.FirstOrDefault() ?? "\u200B", "")
                           .Replace(ReverseName(actresses.FirstOrDefault() ?? "\u200B"), "")
                           .Trim();
            var genres = doc.QuerySelectorAll(".genre a").Select(n => n.TextContent);
            var studio = doc.QuerySelector("#video_maker a")?.TextContent;

            return new Video(
                id,
                code,
                title,
                actresses,
                genres,
                studio
            );
        }

        private string ReverseName(in string name)
        {
            return String.Join(" ", name.Split(' ').Reverse());
        }
    }

    public readonly struct Video
    {
        public readonly string Id;
        public readonly string Code;
        public readonly string Title;
        private readonly IEnumerable<string> Actresses;
        private readonly IEnumerable<string> Genres;
        private readonly string studio;

        public Video(string id, string code, string title, IEnumerable<string> actresses, IEnumerable<string> genres, string studio)
        {
            this.Id = id;
            this.Code = code;
            this.Title = title;
            this.Actresses = actresses;
            this.Genres = genres;
            this.studio = studio;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^
                   Code.GetHashCode() ^
                   Title.GetHashCode() ^
                   Actresses.GetHashCode() &
                   Genres.GetHashCode() &
                   studio.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Video o && this == o;
        }

        public static bool operator ==(Video v1, Video v2)
        {
            return v1.Id == v2.Id &&
                   v1.Code == v2.Code &&
                   v1.Title == v2.Title &&
                   v1.Actresses.SequenceEqual(v2.Actresses) &&
                   v1.Genres.SequenceEqual(v2.Genres) &&
                   v1.studio == v2.studio;
        }

        public static bool operator !=(Video v1, Video v2)
        {
            return !(v1 == v2);
        }
    }
}