using BookShop.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class AuthorDTO
    {
        [StringLength(30, MinimumLength = 3), Required]
        public string FirstName { get; set; }

        [StringLength(30, MinimumLength = 3), Required]
        public string LastName { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [Phone, Required]
        public string Phone { get; set; }

        [Required]
        public ICollection<BooksIdsDTO> Books { get; set; } = new HashSet<BooksIdsDTO>();
    }
}
