using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAssetService
    {
        private readonly LibraryContext _context;

      
        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }
        
        public void deleteAsset(int Id)
        {
            var asset = _context.LibraryAsset.FirstOrDefault(a => a.Id == Id);
            var holds = _context.Holds.Include(a=>a.LibraryAsset).Where(a => a.LibraryAsset.Id == Id);
            foreach (Holds item in holds)
            {
                _context.Remove(item); 
            }
            _context.Remove(asset);
            _context.SaveChanges();
        }
        public LibraryAsset Get(int id)
        {
            return _context.LibraryAsset
                .Include(a => a.Status)
                .Include(a => a.Location)
                .FirstOrDefault(a => a.Id == id);
        }
        public List<Status> GetStatuses()
        {
            return _context.Status.ToList();
        }
        public List<LibraryBranch> GetBranches()
        {
            return _context.LibraryBranch.ToList();


        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAsset
                .Include(a => a.Status)
                .Include(a => a.Location);
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAsset
                .OfType<Book>().Any(a => a.Id == id);

            return isBook
                ? GetAuthor(id)
                : GetDirector(id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return _context.LibraryAsset.First(a => a.Id == id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (GetType(id) != "Book") return "N/A";
            var book = (Book)Get(id);
            return book.DeweyIndex;
        }

        public string GetIsbn(int id)
        {
            if (GetType(id) != "Book") return "N/A";
            var book = (Book)Get(id);
            return book.ISBN;
        }

        public LibraryCard GetLibraryCardByAssetId(int id)
        {
            return _context.LibraryCards
                .FirstOrDefault(c => c.Checkouts
                    .Select(a => a.LibraryAsset)
                    .Select(v => v.Id).Contains(id));
        }

        public string GetTitle(int id)
        {
            return _context.LibraryAsset.First(a => a.Id == id).Title;
        }

        public string GetType(int id)
        {
            // Hack
            var book = _context.LibraryAsset
                .OfType<Book>().SingleOrDefault(a => a.Id == id);
            return book != null ? "Book" : "Video";
        }

        private string GetAuthor(int id)
        {
            var book = (Book)Get(id);
            return book.Author;
        }

        private string GetDirector(int id)
        {
            var video = (Video)Get(id);
            return video.Director;
        }
        private bool isAlreadyAdded(string title)
        {
            var nesto = _context.LibraryAsset.Where(a => a.Title == title);

            if (nesto == null) return true;
            else return false;
        }
        public void AddAsset(string author, string title, string year,int statusId
            ,string imgUrl,string isbn,string deweyIndex,int locationId, decimal cost, int numberOfCopies)
        {
            if (isAlreadyAdded(title)) return;

            var stat = _context.Status.FirstOrDefault(a => a.Name == "Available");
            var locat = _context.LibraryBranch.FirstOrDefault(a => a.Id == 2);

            var book = new Book
            {
                Title = title,
                Author = author,
                Status = stat,
                NumberOfCopies = numberOfCopies,
                ISBN=isbn,
                DeweyIndex=deweyIndex,
                ImageUrl=imgUrl,
                Year = int.Parse(year),
                Location = locat,
                Cost = cost
            };

            _context.Add(book);
            _context.SaveChanges();
        }
    }
}
