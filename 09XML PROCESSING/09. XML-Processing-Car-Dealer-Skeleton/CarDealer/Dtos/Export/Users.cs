using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class Users
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public User[] Userss { get; set; } 
    }
}
