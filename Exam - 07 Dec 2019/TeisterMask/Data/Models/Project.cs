using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeisterMask.Data.Models
{
    public class Project
    {
        public int Id { get; set; }
       
        [StringLength(40, MinimumLength = 2), Required]
        public string Name  { get; set; }

        [Required]
        public DateTime OpenDate { get; set; }

        public DateTime? DueDate { get; set; }

        public ICollection<Task> Tasks { get; set; } = new HashSet<Task>();
    }
}
