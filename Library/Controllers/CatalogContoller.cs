using Library.Models.Catalog;
using Library.Models.Checkout;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
    
namespace Library.Controllers
{
    public class CatalogController:Controller
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly ICheckout _checkoutsService;
        public CatalogController(ILibraryAssetService library,ICheckout checkout)
        {
            _assetsService = library;
            _checkoutsService = checkout;
        }
        public IActionResult Index() // Ovdje se ulazi u home
        {
            var assetModels = _assetsService.GetAll();

            var listingResults = assetModels
                .Select(result => new AssetIndexListingModel
            {
                Id = result.Id,
                ImgUrl = result.ImageUrl,
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(result.Id),
                Title = result.Title,
                Type = _assetsService.GetType(result.Id)
            }).ToList();

            var model = new AssetIndexModel
            {
                Assets = listingResults
             };

             return View(model);
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
                IsCheckedOut = _checkoutsService.IsCheckedOut(id)
            };
            return View(model);
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
                HoldCount = _checkoutsService.GetCurrentHolds(id).Count()
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
