using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace CineFlow.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IMovieCategoryService _service;
        private const int PageSize = 10;

        public CategoryController(ILogger<CategoryController> logger,
            IMovieCategoryService service)
        {
            _logger = logger;
            _service = service;
        }
        public async Task<IActionResult> Index(int page = 1, string searchString = null)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                // Handle search with pagination
                var searchResults = await _service.SearchItemPaginatedAsync(
                    searchString: searchString,
                    pageIndex: page,
                    pageSize: PageSize
                );

                ViewBag.SearchString = searchString;
                return View(searchResults);
            }
            else
            {
                // Handle regular pagination
                var paginatedUnit = await _service.GetPaginatedAsync(
                    pageIndex: page,
                    pageSize: PageSize);

                return View(paginatedUnit);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCategory category)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieCategory category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
