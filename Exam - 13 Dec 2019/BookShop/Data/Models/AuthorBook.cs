using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class AuthorBook
    {
        [Required, Key]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        [Required, Key]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
