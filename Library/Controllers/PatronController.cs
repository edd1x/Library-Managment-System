using Library.Models.Patron;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private IPatrons _patron;
        public PatronController(IPatrons patrons)
        {
            _patron = patrons;
        }

        public IActionResult Index()
        {
            var allPatrons = _patron.GetAll();

            var patronModels = allPatrons.Select(a => new PatronDetailModel
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                LibraryCardId = a.LibraryCard.Id,
                OverdueFees = a.LibraryCard.Fees,
                HomeLibraryBranch = a.LibraryBranch.Name
            }).ToList();

            var model = new PatronIndexModel()
            {
                Patrons=patronModels
            };
            return View(model);
        }
        public IActionResult Detail(int Id)
        {
            var a = _patron.GetById(Id);

            var model = new PatronDetailModel
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                LibraryCardId = a.LibraryCard.Id,
                Adress = a.Address,
                OverdueFees = a.LibraryCard.Fees,
                HomeLibraryBranch = a.LibraryBranch.Name,
                MemberSince = a.LibraryCard.Created,
                Telephone = a.TelephoneNumber,
                AssetsCheckouts = _patron.GetCheckouts(Id).ToList() ?? new List<Checkouts>(),
                checkoutHistory = _patron.GetCheckoutHistory(Id),
                Holds=_patron.GetHolds(Id)
            };
            return View(model);
        }
    }
}
