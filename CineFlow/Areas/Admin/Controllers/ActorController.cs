using CineFlow.Areas.Admin.Models;
using NuGet.ProjectModel;

namespace CineFlow.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ActorController : Controller
    {
        private readonly ILogger<ActorController> _logger;
        private readonly IActorService _service;
        private const int PageSize = 10;
        public ActorController(ILogger<ActorController> logger, IActorService service)
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
            var actor = await _service.GetByIdNoTrackingAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            // Get movies for this actor
            var movies = await _service.GetMoviesByActorId(id);
            ViewBag.Movies = movies;
            @ViewData["MoviesCount"] = movies?.Count ?? 0;
            return View(actor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor, IFormFile profile)
        {
            if (ModelState.IsValid)
            {
                // Handle profile file upload if provided
                if (profile != null && profile.Length > 0)
                {
                    // Validate file
                    if (profile.Length > 2 * 1024 * 1024) // 2MB limit
                    {
                        ModelState.AddModelError("ProfilePictureURL", "File size must be less than 2MB.");
                        return View(actor);
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = Path.GetExtension(profile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("LogoFile", "Only image files are allowed");
                        return View(actor);
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString().Substring(0, 8) +
                                   DateTime.UtcNow.ToString("_ddMMyyyy_hhmmss") +
                                   extension;

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "actor");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profile.CopyToAsync(stream);
                    }

                    actor.ProfilePictureURL = $"/img/actor/{fileName}";
                }
                else
                {
                    actor.ProfilePictureURL = "/img/actor/default.png"; // Default image
                }

                await _service.AddAsync(actor);
                TempData["Success"] = "Actor created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _service.GetByIdNoTrackingAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor, IFormFile? ProfilePictureFile)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get WITH tracking
                    var existingactor = await _service.GetByIdAsync(id);

                    if (existingactor == null)
                    {
                        return NotFound();
                    }

                    // Handle logo file upload if new file provided
                    if (ProfilePictureFile != null && ProfilePictureFile.Length > 0)
                    {
                        // Validate file
                        if (ProfilePictureFile.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ProfilePictureFile", "File size must be less than 2MB");
                            return View(actor);
                        }

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var extension = Path.GetExtension(ProfilePictureFile.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("ProfilePictureFile", "Only image files are allowed");
                            return View(actor);
                        }

                        // Generate unique filename using your format
                        var fileName = Guid.NewGuid().ToString().Substring(0, 8) +
                                       DateTime.UtcNow.ToString("_ddMMyyyy_hhmmss") +
                                       extension;

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "actor");

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ProfilePictureFile.CopyToAsync(stream);
                        }

                        // Delete old logo file if exists
                        if (!string.IsNullOrEmpty(existingactor.ProfilePictureURL) && existingactor.ProfilePictureURL.StartsWith("/img/actor/"))
                        {
                            var oldFileName = Path.GetFileName(existingactor.ProfilePictureURL);
                            var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Update logo on tracked entity
                        existingactor.ProfilePictureURL = $"/img/actor/{fileName}";
                    }
                    // Note: If no new logo, keep existing logo (already set on existingactor)

                    // Update other properties on tracked entity
                    existingactor.FullName = actor.FullName;
                    existingactor.Bio = actor.Bio;

                    // Save changes
                    await _service.SaveChangesAsync();

                    TempData["Success"] = "actor updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await actorExists(actor.Id))
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
            return View(actor);
        }

        private async Task<bool> actorExists(int id)
        {
            var actor = await _service.GetByIdNoTrackingAsync(id);
            return actor != null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _service.GetByIdNoTrackingAsync(id);
            if (actor == null) return NotFound();
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
