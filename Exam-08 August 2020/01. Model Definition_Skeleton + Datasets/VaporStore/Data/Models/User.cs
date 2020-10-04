using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(20, MinimumLength = 3), Required]
        public string Username { get; set; }

        [Required/*, RegularExpression()*/]
        public string FullName { get; set; }
        //•	FullName – text, which has two words, consisting of Latin letters.Both start with an upper letter and are followed by lower letters.The two words are separated by a single space (ex. "John Smith") (required)

        [EmailAddress, Required]
        public string Email { get; set; }

        [Range(3,103), Required]
        public int Age { get; set; }

        public ICollection<Card> Cards { get; set; } = new HashSet<Card>();
    }
}
