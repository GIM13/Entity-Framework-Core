using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class EmployeeDTO
    {
        [StringLength(40, MinimumLength = 3), Required]
        public string Username { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [Phone, Required]
        public string Phone { get; set; }

        public ICollection<int> Tasks { get; set; } = new HashSet<int>();
    }
}
