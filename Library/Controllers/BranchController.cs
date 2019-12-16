using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class BranchController:Controller
    {
        IBranch _branch;

        public BranchController (IBranch branch)
        {
            _branch = branch;
        }
        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(a => new BranchDetailModel
            {
                Id = a.Id,
                BranchName = a.Name,
                isOpen = _branch.isBranchOpen(a.Id),
                NumberOfAssets = _branch.GetLibraryAssets(a.Id).Count(),
                NumberOfPatrons=_branch.GetPatrons(a.Id).Count()
                
            }).ToList();

            var model = new BranchIndexModel
            {
                Branches=branches
            };
            return View(model); 
        }
        public IActionResult Detail(int Id)
        {
            var a= _branch.GetById(Id);

            var model = new BranchDetailModel
            {
                Id=a.Id,
                BranchName=a.Name,
                Address=a.Address,
                TelephoneNumber=a.Telephone,
                Description=a.Description,
                isOpen = _branch.isBranchOpen(a.Id),
                NumberOfAssets = _branch.GetLibraryAssets(a.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(a.Id).Count(),
                ImgUrl=a.ImageUrl,
                OpenDate=a.OpenDate.ToString("yyyy-MM-dd"),
                TotalAssetValue=_branch.GetLibraryAssets(Id).Sum(c=>c.Cost),
                HoursOpen=_branch.GetBranchHours(a.Id)
            };

            return View(model);
        }
    }
}
