using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using BulkyBookWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int productPage = 1)
        {
            var categoryViewModel = new CategoryViewModel
            {
                Categories = await _unitOfWork.CategoryRepositoryAsync.GetAllAsync(),
            };

            var count = categoryViewModel.Categories.Count();

            categoryViewModel.Categories = categoryViewModel.Categories
                .OrderBy(c => c.Name)
                .Skip((productPage - 1) * 2)
                .Take(2)
                .ToList();

            categoryViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = 2,
                TotalItem = count,
                UrlParameter = "/Admin/Category/Index?productPage=:"
            };

            return View(categoryViewModel);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var category = new Category();

            if (id == null)
                return View(category);

            category = await _unitOfWork.CategoryRepositoryAsync.GetAsync(id.GetValueOrDefault());

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            if (category.Id == 0)
                await _unitOfWork.CategoryRepositoryAsync.AddAsync(category);
            else
                await _unitOfWork.CategoryRepositoryAsync.UpdateAsync(category);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.CategoryRepositoryAsync.GetAllAsync();
            return Json(new { data = categories });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.CategoryRepositoryAsync.GetAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Error deleting Category";
                return Json(new { success = false, message = "Error while deleting" });
            }

            await _unitOfWork.CategoryRepositoryAsync.RemoveAsync(category);
            _unitOfWork.Save();

            TempData["Success"] = "Category successfully deleted";
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}