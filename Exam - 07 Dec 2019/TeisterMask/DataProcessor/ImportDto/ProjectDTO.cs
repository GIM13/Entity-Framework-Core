using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ProjectDTO
    {
        [XmlElement]
        [StringLength(40, MinimumLength = 2), Required]
        public string Name { get; set; }

        [XmlElement]
        [Required]
        public string OpenDate { get; set; }

        [XmlElement]
        public string DueDate { get; set; }

        [XmlArray]
        public HashSet<TaskDTO> Tasks { get; set; } = new HashSet<TaskDTO>();
    }
}
