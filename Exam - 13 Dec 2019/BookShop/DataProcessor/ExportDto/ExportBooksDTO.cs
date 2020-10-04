using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ExportDto
{
    [XmlType("Book")]
    public class ExportBooksDTO
    {
        [XmlElement]
        [StringLength(30, MinimumLength = 3), Required]
        public string Name { get; set; }

        [XmlAttribute]
        [Range(50, 5000)]
        public int Pages { get; set; }

        [XmlElement]
        [Required]
        public string Date { get; set; } 
    }
}
