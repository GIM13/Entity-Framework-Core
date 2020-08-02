using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class Author
    {
        public int Id { get; set; }

        [StringLength(30,MinimumLength =3), Required]
        public string FirstName { get; set; }

        [StringLength(30, MinimumLength = 3), Required]
        public string LastName { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [Phone, Required]
        public string Phone { get; set; }

        public ICollection<AuthorBook> AuthorsBooks { get; set; } = new HashSet<AuthorBook>();
    }
}
