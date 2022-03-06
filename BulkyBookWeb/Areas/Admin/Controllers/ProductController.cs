using System;
using System.IO;
using System.Linq;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using BulkyBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var productViewModel = new ProductViewModel
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
            };

            if (id == null)
                return View(productViewModel);

            productViewModel.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());

            if (productViewModel.Product == null)
                return NotFound();

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (productViewModel.Product.ImageUrl != null)
                    {
                        // this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var fileStream =
                           new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productViewModel.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    if (productViewModel.Product.Id != 0)
                    {
                        var product = _unitOfWork.Product.Get(productViewModel.Product.Id);
                        productViewModel.Product.ImageUrl = product.ImageUrl;
                    }
                }

                if (productViewModel.Product.Id == 0)
                    _unitOfWork.Product.Add(productViewModel.Product);
                else
                    _unitOfWork.Product.Update(productViewModel.Product);

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            productViewModel.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            if (productViewModel.Product.Id != 0)
            {
                productViewModel.Product = _unitOfWork.Product.Get(productViewModel.Product.Id);
            }
            return View(productViewModel);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productFromDb = _unitOfWork.Product.Get(id);

            if (productFromDb == null)
                return Json(new { success = false, message = "Error while deleting!" });

            var webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, productFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Product.Remove(productFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}