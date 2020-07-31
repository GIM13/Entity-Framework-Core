using System.Xml.Serialization;

namespace CarDealer.DTO
{
    [XmlType("partId")]
    public class PartId
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
 