

using LibraryData.Models;
using System.Collections.Generic;

namespace LibraryData
{
    public interface IPatrons
    {
        Patron GetById(int id);
        void Add(Patron patron);

        IEnumerable<Patron> GetAll();
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId);
        IEnumerable<Holds> GetHolds(int patronId);
        IEnumerable<Checkouts> GetCheckouts(int patronId);
    }
}
