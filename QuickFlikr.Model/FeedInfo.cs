using System.Runtime.Serialization;

namespace QuickFlikr.Model
{
    [DataContract()]
    public class FeedInfo
    {
        [DataMember(Name = "author", IsRequired = true)]
        public string Author { get; set; }

        [DataMember(Name = "author_id", IsRequired = true)]
        public string Author_Id { get; set; }

        [DataMember(Name = "date_taken", IsRequired = true)]
        public object Date_Taken { get; set; }

        [DataMember(Name = "description", IsRequired = true)]
        public string Description { get; set; }

        [DataMember(Name = "link", IsRequired = true)]
        public string Link { get; set; }

        [DataMember(Name = "media", IsRequired = true)]
        public Media Media { get; set; }

        [DataMember(Name = "published", IsRequired = true)]
        public DateTime Published { get; set; }

        [DataMember(Name = "tags", IsRequired = true)]
        public string Tags { get; set; }

        [DataMember(Name = "title", IsRequired = true)]
        public string Title { get; set; }
    }
}