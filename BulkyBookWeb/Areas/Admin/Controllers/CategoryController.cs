using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var category = new Category();

            if (id == null)
                return View(category);

            category = _unitOfWork.Category.Get(id.GetValueOrDefault());

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            if (category.Id == 0)
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _unitOfWork.Category.GetAll();
            return Json(new { data = categories });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.Category.Get(id);
            if (category == null)
                return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}