﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Service
{
    public class CheckoutService : ICheckout
    {
        private readonly LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }
        public IEnumerable<Patron> getLibraryCards()
        {
            return _context.Patrons;
        }

        public void Add(Checkouts newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public Checkouts Get(int id)
        {
            return _context.Checkouts.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Checkouts> GetAll()
        {
            return _context.Checkouts;
        }

        public void CheckoutItem(int id, int libraryCardId)
        {
            if (IsCheckedOut(id)) return;

            var item = _context.LibraryAsset
                .Include(a => a.Status)
                .FirstOrDefault(a => a.Id == id);

            _context.Update(item);

            item.Status = _context.Status
                .FirstOrDefault(a => a.Name == "Checked Out");

            var now = DateTime.Now;

            var libraryCard = _context.LibraryCards
                .Include(c => c.Checkouts)
                .FirstOrDefault(a => a.Id == libraryCardId);

            if (libraryCard == null) return;

            var checkout = new Checkouts
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        public void MarkLost(int id)
        {
            var item = _context.LibraryAsset
                .First(a => a.Id == id);

            _context.Update(item);

            item.Status = _context.Status.FirstOrDefault(a => a.Name == "Lost");

            _context.SaveChanges();
        }

        public void MarkFound(int id)
        {
            var item = _context.LibraryAsset
                .First(a => a.Id == id);

            _context.Update(item);
            item.Status = _context.Status.FirstOrDefault(a => a.Name == "Available");
            var now = DateTime.Now;

            // remove any existing checkouts on the item
            var checkout = _context.Checkouts
                .FirstOrDefault(a => a.LibraryAsset.Id == id);
            if (checkout != null) _context.Remove(checkout);

            // close any existing checkout history
            var history = _context.CheckoutHistory
                .FirstOrDefault(h =>
                    h.LibraryAsset.Id == id
                    && h.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }

            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAsset
                .Include(a => a.Status)
                .FirstOrDefault(a => a.Id == assetId);

            var card = _context.LibraryCards
                .First(a => a.Id == libraryCardId);

            _context.Update(asset);

            if (asset.Status.Name == "Available")
                asset.Status = _context.Status.FirstOrDefault(a => a.Name == "On Hold");

            var hold = new Holds
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public void CheckInItem(int id)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAsset
                .First(a => a.Id == id);

            _context.Update(item);

            // remove any existing checkouts on the item
            var checkout = _context.Checkouts
                .Include(c => c.LibraryAsset)
                .Include(c => c.LibraryCard)
                .FirstOrDefault(a => a.LibraryAsset.Id == id);
            if (checkout != null) _context.Remove(checkout);

            // close any existing checkout history
            var history = _context.CheckoutHistory
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h =>
                    h.LibraryAsset.Id == id
                    && h.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }

            // look for current holds
            var currentHolds = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(a => a.LibraryAsset.Id == id);

            // if there are current holds, check out the item to the earliest
            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(id, currentHolds);
                return;
            }

            // otherwise, set item status to available
            item.Status = _context.Status.FirstOrDefault(a => a.Name == "Available");

            _context.SaveChanges();
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistory
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(a => a.LibraryAsset.Id == id);
        }

        // Remove useless method and replace with finding latest CheckoutHistory if needed 
        public Checkouts GetLatestCheckout(int id)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == id)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }

        public int GetNumberOfCopies(int id)
        {
            return _context.LibraryAsset
                .First(a => a.Id == id)
                .NumberOfCopies;
        }

        public bool IsCheckedOut(int id)
        {
            var isCheckedOut = _context.Checkouts.Any(a => a.LibraryAsset.Id == id);

            return isCheckedOut;
        }

        public string GetCurrentHoldPatron(int id)
        {
            var hold = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(v => v.Id == id);

            var cardId = hold
                .Include(a => a.LibraryCard)
                .Select(a => a.LibraryCard.Id)
                .FirstOrDefault();

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        public string GetCurrentHoldPlaced(int id)
        {
            var hold = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(v => v.Id == id);

            return hold.Select(a => a.HoldPlaced)
                .FirstOrDefault().ToString(CultureInfo.InvariantCulture);
        }

        public IEnumerable<Holds> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(a => a.LibraryAsset.Id == id);
        }

        public string GetCurrentPatron(int id)
        {
            var checkout = _context.Checkouts
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .FirstOrDefault(a => a.LibraryAsset.Id == id);

            if (checkout == null) return "Not checked out.";

            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .First(c => c.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private void CheckoutToEarliestHold(int assetId, IEnumerable<Holds> currentHolds)
        {
            var earliestHold = currentHolds.OrderBy(a => a.HoldPlaced).FirstOrDefault();
            if (earliestHold == null) return;
            var card = earliestHold.LibraryCard;
            _context.Remove(earliestHold);
            _context.SaveChanges();

            CheckoutItem(assetId, card.Id);
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }
    }

}