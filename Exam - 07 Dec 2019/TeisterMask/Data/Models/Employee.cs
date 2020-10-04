using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeisterMask.Data.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [StringLength(40, MinimumLength = 3), Required]
        public string Username { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [Phone, Required]
        public string Phone { get; set; }

        public ICollection<EmployeeTask> EmployeesTasks { get; set; } = new HashSet<EmployeeTask>();
    }
}
