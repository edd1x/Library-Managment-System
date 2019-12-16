using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace LibraryData
{
   public interface ILibraryAssetService
    {
        IEnumerable<LibraryAsset> GetAll();
        LibraryAsset Get(int id);
        void deleteAsset(int Id);
        List<Status> GetStatuses();
        List<LibraryBranch> GetBranches();
        void AddAsset(string author, string title, string year, int statusId
            , string imgUrl, string isbn, string deweyIndex, int locationId, decimal cost, int numberOfCopies);
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
