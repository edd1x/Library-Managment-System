using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
   public interface IBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<Patron> GetPatrons(int Id);
        IEnumerable<LibraryAsset> GetLibraryAssets(int Id);
        LibraryBranch GetById(int Id);
        IEnumerable<string> GetBranchHours(int Id);
        void Add(LibraryBranch branch);
        bool isBranchOpen(int Id);
    }
}
