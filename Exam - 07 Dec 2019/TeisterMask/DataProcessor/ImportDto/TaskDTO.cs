using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class TaskDTO
    {
        [XmlElement]
        [StringLength(40, MinimumLength = 2), Required]
        public string Name { get; set; }

        [XmlElement]
        [Required]
        public string OpenDate { get; set; }

        [XmlElement]
        [Required]
        public string DueDate { get; set; }

        [XmlElement]
        [Required,Range(0,3)]
        public int ExecutionType { get; set; }

        [XmlElement]
        [Required,Range(0,4)]
        public int LabelType { get; set; }
    }
}
