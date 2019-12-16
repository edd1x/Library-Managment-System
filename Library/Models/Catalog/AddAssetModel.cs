using LibraryData.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Library.Models.Catalog
{
    public class AddAssetModel
    {
        [Required(ErrorMessage = "Required field")]
        public string Title { get; set; }
        public string Author { get; set; }
        [Range(13,13,ErrorMessage ="ISBN must have 13 numbers")]
        public string ISBN { get; set; }
        [Required(ErrorMessage = "Required field")]
        [Range(-999, 2019, ErrorMessage = "Invalid year")]
        public string Year { get; set; }
        public string deweyIndex { get; set; }

       

        [Required(ErrorMessage ="Required field")]
        public decimal Cost { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Cant enter negativ number")]
        public int NumberOfCopies { get; set; }

        public List<LibraryBranch> Location{ get; set; }
        public string LocationId { get; set; }

        public string StatusId { get; set; }
        public List<Status> Status { get; set; }

        public string ImgUrl { get; set; }
    }
}
