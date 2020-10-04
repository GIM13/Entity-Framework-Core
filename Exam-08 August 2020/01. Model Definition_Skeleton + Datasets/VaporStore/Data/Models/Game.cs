using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(typeof(decimal),"0", "79228162514264337593543950335"), Required]
        public decimal Price { get; set; }                            

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public int DeveloperId { get; set; }
        [Required]
        public Developer Developer { get; set; }

        [Required]
        public int GenreId { get; set; }
        [Required]
        public Genre Genre { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();

        public ICollection<GameTag> GameTags { get; set; } = new HashSet<GameTag>();
        //•	GameTags - collection of type GameTag.Each game must have at least one tag.
    }
}
