namespace CineFlow.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
        private readonly ILogger<CinemaController> _logger;
        private readonly ICinemaService _service;
        private const int PageSize = 10;

        public CinemaController(ILogger<CinemaController> logger, ICinemaService service)
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
            var cinema = await _service.GetByIdNoTrackingAsync(id);

            if (cinema == null)
            {
                return NotFound();
            }

            // Get movies for this cinema
            var movies = await _service.GetMoviesByCinemaId(id);
            ViewBag.Movies = movies;
            ViewBag.MoviesCount = movies?.Count ?? 0;

            return View(cinema);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile LogoFile)
        {
            if (ModelState.IsValid)
            {
                // Handle logo file upload if provided
                if (LogoFile != null && LogoFile.Length > 0)
                {
                    // Validate file
                    if (LogoFile.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("LogoFile", "File size must be less than 2MB");
                        return View(cinema);
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = Path.GetExtension(LogoFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("LogoFile", "Only image files are allowed");
                        return View(cinema);
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString().Substring(0, 8) +
                                   DateTime.UtcNow.ToString("_ddMMyyyy_hhmmss") +
                                   extension;

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "cinema");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await LogoFile.CopyToAsync(stream);
                    }

                    cinema.Logo = $"/img/cinema/{fileName}";
                }
                else
                {
                    cinema.Logo = "/img/default-cinema.png"; // Default logo
                }

                await _service.AddAsync(cinema);
                TempData["Success"] = "Cinema created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(cinema);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _service.GetByIdNoTrackingAsync(id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema, IFormFile? LogoFile)
        {
            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get WITH tracking
                    var existingCinema = await _service.GetByIdAsync(id);

                    if (existingCinema == null)
                    {
                        return NotFound();
                    }

                    // Handle logo file upload if new file provided
                    if (LogoFile != null && LogoFile.Length > 0)
                    {
                        // Validate file
                        if (LogoFile.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("LogoFile", "File size must be less than 2MB");
                            return View(cinema);
                        }

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var extension = Path.GetExtension(LogoFile.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("LogoFile", "Only image files are allowed");
                            return View(cinema);
                        }

                        // Generate unique filename using your format
                        var fileName = Guid.NewGuid().ToString().Substring(0, 8) +
                                       DateTime.UtcNow.ToString("_ddMMyyyy_hhmmss") +
                                       extension;

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "cinema");

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await LogoFile.CopyToAsync(stream);
                        }

                        // Delete old logo file if exists
                        if (!string.IsNullOrEmpty(existingCinema.Logo) && existingCinema.Logo.StartsWith("/img/cinema/"))
                        {
                            var oldFileName = Path.GetFileName(existingCinema.Logo);
                            var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Update logo on tracked entity
                        existingCinema.Logo = $"/img/cinema/{fileName}";
                    }
                    // Note: If no new logo, keep existing logo (already set on existingCinema)

                    // Update other properties on tracked entity
                    existingCinema.Name = cinema.Name;
                    existingCinema.Description = cinema.Description;

                    // Save changes
                    await _service.SaveChangesAsync();

                    TempData["Success"] = "Cinema updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CinemaExists(cinema.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If ModelState is invalid
            return View(cinema);
        }

        private async Task<bool> CinemaExists(int id)
        {
            var cinema = await _service.GetByIdNoTrackingAsync(id);
            return cinema != null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _service.GetByIdNoTrackingAsync(id);
            if (cinema == null) return NotFound();
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
