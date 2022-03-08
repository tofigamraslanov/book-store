﻿using System.Linq;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Concrete
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryFromDb = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (categoryFromDb == null) 
                return;

            categoryFromDb.Name = category.Name;
        }
    }
}