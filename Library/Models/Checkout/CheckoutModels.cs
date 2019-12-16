using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Checkout
{
    public class CheckoutModels
    {
        [Required]
        public string LibraryCardId { get; set; }

        public string Title { get; set; }
        public int AssetId { get; set; }
        public string ImageUrl { get; set; }
        public int HoldCount { get; set; }
        public bool IsCheckedOut { get; set; }
        public IEnumerable<LibraryData.Models.Patron> cards { get; set; }
    }
}
