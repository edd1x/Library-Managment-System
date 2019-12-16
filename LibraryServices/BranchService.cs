using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
   public class BranchService : IBranch
    {
        LibraryContext _context;
        public BranchService(LibraryContext libraryContext)
        {
            _context = libraryContext;
        }
        public void Add(LibraryBranch branch)
        {
            _context.Add(branch);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranch
                .Include(a => a.Patrons)
                .Include(a => a.LibraryAssets);
        }

        public IEnumerable<string> GetBranchHours(int Id)
        {
            var hours = _context.BranchHours.Where(a => a.Branch.Id == Id);

            return DataHelpers.HumanizeBusinessHours(hours);
        }

        public LibraryBranch GetById(int Id)
        {
            return GetAll().FirstOrDefault(a => a.Id == Id);
        }

        public IEnumerable<LibraryAsset> GetLibraryAssets(int Id)
        {
            return GetById(Id).LibraryAssets;
        }

        public IEnumerable<Patron> GetPatrons(int Id)
        {
            return _context.LibraryBranch
                .Include(p => p.Patrons)
                .FirstOrDefault(a => a.Id == Id).Patrons;
        }

        public bool isBranchOpen(int Id)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;
            var hours = _context.BranchHours.Where(a => a.Branch.Id == Id);
            var daysHours = hours.FirstOrDefault(a => a.DayOfWeek == currentDayOfWeek);

            return currentTimeHour < daysHours.CloseTime && currentTimeHour > daysHours.OpenTime;
        }
    }
}
