using System.Runtime.Serialization;

namespace QuickFlikr.Model
{
    [DataContract()]
    public class FlickrFeed
    {
        [DataMember(Name = "description", IsRequired = true)]
        public string Description { get; set; }

        [DataMember(Name = "generator", IsRequired = true)]
        public string Generator { get; set; }

        [DataMember(Name = "items", IsRequired = true)]
        public List<FeedInfo> Items { get; set; }

        [DataMember(Name = "link", IsRequired = true)]
        public string Link { get; set; }

        [DataMember(Name = "modified", IsRequired = true)]
        public DateTime Modified { get; set; }

        [DataMember(Name = "title", IsRequired = true)]
        public string title { get; set; }
    }
}