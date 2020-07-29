using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("CategoryProduct")]
    public class ImportCategoryProductsDTO
    {
        [XmlElement("CategoryId")]
        public int CategoryId { get; set; }

        public int ProductId { get; set; }
    }
}
