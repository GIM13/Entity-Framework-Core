using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        public PurchaseType Type { get; set; }

        [Required/*, RegularExpression()*/]
        public string ProductKey { get; set; }
        //•	ProductKey – text, which consists of 3 pairs of 4 uppercase Latin letters and digits, separated by dashes(ex. “ABCD-EFGH-1J3L”) (required)

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int CardId { get; set; }
        [Required]
        public Card Card { get; set; }

        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }
    }
}
