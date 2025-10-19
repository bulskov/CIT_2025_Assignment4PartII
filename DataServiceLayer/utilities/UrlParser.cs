using System.Linq;

namespace DataServiceLayer.Utilities
{
    public class UrlParser
    {
        public bool HasId { get; private set; }
        public string? Id { get; private set; }
        public string? Path { get; private set; }

        public bool ParseUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            // remove any leading or trailing slashes and split
            var parts = url.Trim('/').Split('/');

            // must have at least "api" + resource
            if (parts.Length < 2)
                return false;

            // reconstruct path with first two segments
            Path = "/" + string.Join("/", parts.Take(2));

            // if there’s an extra segment, treat it as ID
            if (parts.Length > 2)
            {
                HasId = true;
                Id = parts.Last();
            }

            return true;
        }
    }
}
