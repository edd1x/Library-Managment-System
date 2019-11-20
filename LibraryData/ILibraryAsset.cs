using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
   public interface ILibraryAssetService
    {
        IEnumerable<LibraryAsset> GetAll();
        LibraryAsset Get(int id);

        void Add(LibraryAsset newAsset);
        string GetAuthorOrDirector(int Id);
        string GetDeweyIndex(int Id);
        string GetType(int Id);
        string GetTitle(int Id);
        string GetIsbn(int Id);

        LibraryBranch GetCurrentLocation(int Id);
        LibraryCard GetLibraryCardByAssetId(int id);

    }
}
