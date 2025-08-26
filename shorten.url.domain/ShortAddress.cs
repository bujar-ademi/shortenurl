using System;

namespace shorten.url.domain
{
    public class ShortAddress : BaseEntity
    {
        public string Domain { get; set; }
        public string UniqueId { get; set; }
        /// <summary>
        /// Short Url is combination of Domain and UniqueId
        /// </summary>
        public string ShortUrl { get; set; }
        public string RedirectUrl { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public int Hits { get; set; }

        public Guid ApiClientId { get; set; }
        public ApiClient ApiClient { get; set; }
    }
}
