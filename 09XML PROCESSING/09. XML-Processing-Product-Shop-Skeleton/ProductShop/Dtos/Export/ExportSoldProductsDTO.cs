using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class ExportSoldProductsDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public ExportProductsDTO[] Products { get; set; }
    }
}
