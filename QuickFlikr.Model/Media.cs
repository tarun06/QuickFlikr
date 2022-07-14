using System.Runtime.Serialization;

namespace QuickFlikr.Model
{
    [DataContract()]
    public class Media
    {
        [DataMember(Name = "m", IsRequired = true)]
        public string Path { get; set; }
    }
}