namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Items;

    public class ItemsController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public ItemsController(FastFoodContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Create()
        {
            //Option 1 - manual mapping

            //var categories = _context.Categories
            //    .Select(c => new CreateItemViewModel
            //    {
            //        CategoryId = c.Id,
            //        Name = c.Name
            //    })
            //    .ToList();

            var categories = _context.Categories
                .ProjectTo<CreateItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View(categories);
        }

        [HttpPost]
        public IActionResult Create(CreateItemInputModel model)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Error", "Home");
            }

            //Manual mapper
            //var newItem = new Item
            //{
            //    Name = model.Name,
            //    CategoryId = model.CategoryId,
            //    Price = model.Price
            //};

            var newItem = _mapper.Map<Item>(model);

            _context.Items.Add(newItem);
            _context.SaveChanges();

            return RedirectToAction("All");
        }

        public IActionResult All()
        {
            var items = _context.Items
                .ProjectTo<ItemsAllViewModels>(_mapper.ConfigurationProvider)
                .ToList();

            return View(items);
        }
    }
}
