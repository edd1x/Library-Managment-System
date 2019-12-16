using Library.Models.Catalog;
using Library.Models.Checkout;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;


namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly ICheckout _checkoutsService;
        private readonly IPatrons _patrons;
        public CatalogController(ILibraryAssetService library, ICheckout checkout, IPatrons patrons)
        {
            _assetsService = library;
            _checkoutsService = checkout;
            _patrons = patrons;
        }
        public IActionResult Index()
        {
            var assetModels = _assetsService.GetAll();

            var listingResult = assetModels
                .Select(a => new AssetIndexListingModel
                {
                    Id = a.Id,
                    ImgUrl = a.ImageUrl,
                    AuthorOrDirector = _assetsService.GetAuthorOrDirector(a.Id),
                    DeweyIndex = _assetsService.GetDeweyIndex(a.Id),
                    Title = _assetsService.GetTitle(a.Id),
                    Type = _assetsService.GetType(a.Id),
                    NumberOfCopies = _checkoutsService.GetNumberOfCopies(a.Id)
                }).ToList();

            var model = new AssetIndexModel
            {
                Assets = listingResult
            };

            return View(model);
        }

        public IActionResult AddAsset()
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return PartialView("AddAsset");
            }
            


            var model = new AddAssetModel
            {
                Author = "",
                ISBN="",
                deweyIndex="",
                Title = "",
                Year = "",
                Cost = 0,
                Status=_assetsService.GetStatuses(),
                Location=_assetsService.GetBranches(),
                NumberOfCopies = 0,
                ImgUrl = ""
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult PlaceAsset(string author, string title, string year, int statusId
            , string imgUrl, string isbn, string deweyIndex, int locationId, decimal cost, int numberOfCopies)
        {

            _assetsService.AddAsset( author,  title,  year,  statusId
            ,  imgUrl,  isbn,  deweyIndex,  locationId,  cost,  numberOfCopies);
            return RedirectToAction("Index");
        }
        public IActionResult Detail(int id)
        {
            var asset = _assetsService.Get(id);

            var currentHolds = _checkoutsService.GetCurrentHolds(id).Select(a => new AssetHoldModel
            {
                HoldPlace = _checkoutsService.GetCurrentHoldPlaced(a.Id),
                PatronName = _checkoutsService.GetCurrentHoldPatron(a.Id)
            });

            var model = new AssetDetailModel
            {
                Id = id,
                Title = asset.Title,
                Type = _assetsService.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(id),
                CurrentLocation = _assetsService.GetCurrentLocation(id)?.Name,
                DeweyCallNumber = _assetsService.GetDeweyIndex(id),
                CheckoutHistory = _checkoutsService.GetCheckoutHistory(id),
                CurrentAssociatedLibraryCard = _assetsService.GetLibraryCardByAssetId(id),
                ISBN = _assetsService.GetIsbn(id),
                LatestCheckout = _checkoutsService.GetLatestCheckout(id),
                CurrentHolds = currentHolds,
                PatronName = _checkoutsService.GetCurrentPatron(id)
            };

            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = _assetsService.Get(id);
            var model = new CheckoutModels
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkoutsService.IsCheckedOut(id),
                cards = _checkoutsService.getLibraryCards() 

        };
            return View(model);
        }
        public IActionResult DeleteAsset(int id)
        {
            _assetsService.deleteAsset(id);

            return Redirect("/Catalog/Index");
        }
        public IActionResult Hold(int id)
        {
            var asset = _assetsService.Get(id);

            var model = new CheckoutModels
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                HoldCount = _checkoutsService.GetCurrentHolds(id).Count(),
                cards = _checkoutsService.getLibraryCards()
            };
            return View(model);
        }

        public IActionResult CheckIn(int id)
        {
            _checkoutsService.CheckInItem(id);
            return RedirectToAction("Detail", new { id });
        }

        public IActionResult MarkLost(int id)
        {
            _checkoutsService.MarkLost(id);
            return RedirectToAction("Detail", new { id });
        }

        public IActionResult MarkFound(int id)
        {
            _checkoutsService.MarkFound(id);
            return RedirectToAction("Detail", new { id });
        }
       
        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            
            _checkoutsService.CheckoutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkoutsService.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}
