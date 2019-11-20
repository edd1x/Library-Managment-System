

using System.Collections.Generic;
using System.Linq;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class PatronService : IPatrons
    {
        private LibraryContext _context;

        public PatronService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Patron patron)
        {
            _context.Add(patron);
            _context.SaveChanges();
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryBranch);
        }

        public Patron GetById(int id)
        {
            return GetAll()
                .FirstOrDefault(a => a.Id == id);
                
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = GetById(patronId).LibraryCard.Id;

            return _context.CheckoutHistory
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(a => a.LibraryCard.Id == cardId)
                .OrderByDescending(a => a.CheckedOut);
        }

        public IEnumerable<Checkouts> GetCheckouts(int patronId)
        {
            var cardId = GetById(patronId).LibraryCard.Id;
             

            return _context.Checkouts
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(a => a.LibraryCard.Id == cardId);
        }

        public IEnumerable<Holds> GetHolds(int patronId)
        {
            var cardId = GetById(patronId).LibraryCard.Id;

            return _context.Holds
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(a=>a.LibraryCard.Id==cardId);

        }
    }
}
