using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineFlow.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieService _service;
        private const int PageSize = 10;

        private readonly string _posterUploadPath = Path.Combine("wwwroot", "img", "movie", "posters");
        private readonly string _subImageUploadPath = Path.Combine("wwwroot", "img", "movie", "subimages");

        public MovieController(ILogger<MovieController> logger, IMovieService service)
        {
            _logger = logger;
            _service = service;

            // Ensure upload directories exist
            if (!Directory.Exists(_posterUploadPath))
                Directory.CreateDirectory(_posterUploadPath);

            if (!Directory.Exists(_subImageUploadPath))
                Directory.CreateDirectory(_subImageUploadPath);
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
                    pageSize: PageSize,
                    m => m.Cinema,
                    m => m.MovieCategory);

                return View(paginatedUnit);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _service.GetMovieByIdAsync(id);
            return View(movie);
        }

        public async Task<IActionResult> Create()
        {
            var movieDropdowns = await _service.GetNewMovieDropdownsValues();

            ViewBag.MovieCategories = new SelectList(movieDropdowns.MovieCategories, "Value", "Text");
            ViewBag.Cinemas = new SelectList(movieDropdowns.Cinemas, "Value", "Text");
            ViewBag.Actors = new SelectList(movieDropdowns.Actors, "Value", "Text");

            return View(new MovieVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieVM movieVM)
        {
            if (ModelState.IsValid)
            {
                // Handle poster image upload
                if (movieVM.PosterImageFile != null && movieVM.PosterImageFile.Length > 0)
                {
                    var posterResult = await UploadImage(movieVM.PosterImageFile, _posterUploadPath);
                    if (!posterResult.Success)
                    {
                        ModelState.AddModelError("PosterImageFile", posterResult.ErrorMessage);
                        return View(movieVM);
                    }
                    movieVM.ImageURL = $"/img/movie/posters/{posterResult.FileName}";
                }
                else
                {
                    movieVM.ImageURL = "/img/movie/default-poster.png";
                }

                // Prepare new sub image captions list
                movieVM.NewSubImageCaptions = new List<string>();

                // Handle multiple sub images
                if (movieVM.SubImageFiles != null && movieVM.SubImageFiles.Any(f => f.Length > 0))
                {
                    // Filter out empty files
                    movieVM.SubImageFiles = movieVM.SubImageFiles
                        .Where(f => f != null && f.Length > 0)
                        .ToList();

                    // Validate all sub images
                    foreach (var subImageFile in movieVM.SubImageFiles)
                    {
                        var subImageResult = await UploadImage(subImageFile, _subImageUploadPath);
                        if (!subImageResult.Success)
                        {
                            ModelState.AddModelError("SubImageFiles", subImageResult.ErrorMessage);
                            return View(movieVM);
                        }
                        movieVM.NewSubImageCaptions.Add(""); // Default empty caption
                    }
                }
                else
                {
                    movieVM.SubImageFiles = null;
                }

                await _service.AddNewMovieAsync(movieVM);
                TempData["Success"] = "Movie created successfully!";
                return RedirectToAction(nameof(Index));
            }

            var movieDropdowns = await _service.GetNewMovieDropdownsValues();
            ViewBag.MovieCategories = new SelectList(movieDropdowns.MovieCategories, "Value", "Text");
            ViewBag.Cinemas = new SelectList(movieDropdowns.Cinemas, "Value", "Text");
            ViewBag.Actors = new SelectList(movieDropdowns.Actors, "Value", "Text");

            return View(movieVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movieVM = await _service.GetMovieForEditAsync(id);

            if (movieVM == null)
                return NotFound();

            var movieDropdowns = await _service.GetNewMovieDropdownsValues();
            ViewBag.MovieCategories = new SelectList(movieDropdowns.MovieCategories, "Value", "Text");
            ViewBag.Cinemas = new SelectList(movieDropdowns.Cinemas, "Value", "Text");
            ViewBag.Actors = new SelectList(movieDropdowns.Actors, "Value", "Text");

            return View(movieVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MovieVM movieVM)
        {
            if (id != movieVM.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMovie = await _service.GetMovieForEditWithTrackingAsync(id);

                    if (existingMovie == null)
                        return NotFound();

                    // Handle poster image upload - FIXED: Ensure we only save if valid
                    if (movieVM.PosterImageFile != null && movieVM.PosterImageFile.Length > 0)
                    {
                        var posterResult = await UploadImage(movieVM.PosterImageFile, _posterUploadPath);
                        if (!posterResult.Success)
                        {
                            ModelState.AddModelError("PosterImageFile", posterResult.ErrorMessage);
                            return View(movieVM);
                        }

                        // Only delete old image if new one uploaded successfully
                        if (!string.IsNullOrEmpty(existingMovie.ImageURL) &&
                            existingMovie.ImageURL.StartsWith("/img/movie/posters/"))
                        {
                            DeleteImage(existingMovie.ImageURL);
                        }

                        existingMovie.ImageURL = $"/img/movie/posters/{posterResult.FileName}";
                    }
                    // If no new poster uploaded, keep the existing ImageURL
                    // Don't overwrite with null from movieVM.ImageURL

                    // Handle sub images - FIXED: Check if collection exists before iterating
                    if (movieVM.ExistingSubImages != null && movieVM.ExistingSubImages.Any())
                    {
                        foreach (var subImage in movieVM.ExistingSubImages)
                        {
                            if (subImage.ToDelete)
                            {
                                var existingSubImage = existingMovie.SubImages
                                    .FirstOrDefault(si => si.Id == subImage.Id);

                                if (existingSubImage != null)
                                {
                                    DeleteImage(existingSubImage.ImageURL);
                                    existingMovie.SubImages.Remove(existingSubImage);
                                }
                            }
                            else
                            {
                                var existingSubImage = existingMovie.SubImages
                                    .FirstOrDefault(si => si.Id == subImage.Id);

                                if (existingSubImage != null)
                                {
                                    existingSubImage.Caption = subImage.Caption;
                                    existingSubImage.DisplayOrder = subImage.DisplayOrder;
                                }
                            }
                        }
                    }

                    // Handle new sub images - FIXED: Check if files exist
                    if (movieVM.SubImageFiles != null)
                    {
                        // Filter out null files and empty files
                        var validFiles = movieVM.SubImageFiles
                            .Where(f => f != null && f.Length > 0)
                            .ToList();

                        if (validFiles.Any())
                        {
                            // Ensure NewSubImageCaptions list exists
                            movieVM.NewSubImageCaptions ??= new List<string>();

                            for (int i = 0; i < validFiles.Count; i++)
                            {
                                var subImageFile = validFiles[i];
                                var caption = i < movieVM.NewSubImageCaptions.Count
                                    ? movieVM.NewSubImageCaptions[i]
                                    : string.Empty;

                                var subImageResult = await UploadImage(subImageFile, _subImageUploadPath);
                                if (!subImageResult.Success)
                                {
                                    ModelState.AddModelError("SubImageFiles", subImageResult.ErrorMessage);
                                    return View(movieVM);
                                }

                                var newSubImage = new MovieSubImage
                                {
                                    MovieId = existingMovie.Id,
                                    ImageURL = $"/img/movie/subimages/{subImageResult.FileName}",
                                    Caption = caption,
                                    DisplayOrder = existingMovie.SubImages.Any()
                                        ? existingMovie.SubImages.Max(si => si.DisplayOrder) + 1
                                        : 0
                                };

                                existingMovie.SubImages.Add(newSubImage);
                            }
                        }
                    }

                    // Update other properties - FIXED: Don't overwrite ImageURL if no new file
                    existingMovie.Title = movieVM.Title;
                    existingMovie.Description = movieVM.Description;
                    existingMovie.Price = movieVM.Price;
                    existingMovie.StartDate = movieVM.StartDate;
                    existingMovie.EndDate = movieVM.EndDate;
                    existingMovie.MovieCategoryId = movieVM.MovieCategoryId;
                    existingMovie.CinemaId = movieVM.CinemaId;
                    // ImageURL is handled above based on file upload

                    // Update actors
                    await _service.UpdateMovieActorsAsync(existingMovie.Id, movieVM.ActorIds);

                    // FIXED: Use the correct SaveChanges method
                    await _service.SaveChangesAsync(); // Or your service method

                    TempData["Success"] = "Movie updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MovieExists(id))
                        return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    // Log error
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            // If we get here, validation failed or there was an error
            var movieDropdowns = await _service.GetNewMovieDropdownsValues();
            ViewBag.MovieCategories = new SelectList(movieDropdowns.MovieCategories, "Value", "Text");
            ViewBag.Cinemas = new SelectList(movieDropdowns.Cinemas, "Value", "Text");
            ViewBag.Actors = new SelectList(movieDropdowns.Actors, "Value", "Text");

            return View(movieVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _service.GetMovieByIdAsync(id);

            if (movie != null)
            {
                // Delete poster image if exists
                if (!string.IsNullOrEmpty(movie.ImageURL) && movie.ImageURL.StartsWith("/img/movie/"))
                {
                    DeleteImage(movie.ImageURL);
                }

                // Delete all sub images if they exist
                if (movie.SubImages != null && movie.SubImages.Any())
                {
                    foreach (var subImage in movie.SubImages)
                    {
                        if (!string.IsNullOrEmpty(subImage.ImageURL) && subImage.ImageURL.StartsWith("/img/movie/"))
                        {
                            DeleteImage(subImage.ImageURL);
                        }
                    }
                }

                await _service.DeleteAsync(id);
                TempData["Success"] = "Movie deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MovieExists(int id)
        {
            return (await _service.GetByIdAsync(id)) != null;
        }

        private async Task<(bool Success, string FileName, string ErrorMessage)> UploadImage(
        IFormFile file, string uploadPath)
        {
            try
            {
                if (file.Length > 2 * 1024 * 1024)
                    return (false, null, "File size must be less than 2MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    return (false, null, "Only image files are allowed (JPG, PNG, GIF, WEBP).");

                var fileName = Guid.NewGuid().ToString().Substring(0, 8) +
                               DateTime.UtcNow.ToString("_ddMMyyyy_hhmmss") +
                               extension;

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return (true, fileName, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Error uploading file: {ex.Message}");
            }
        }

        private void DeleteImage(string imageUrl)
        {
            try
            {
                var relativePath = imageUrl.TrimStart('/');
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception)
            {
                // Log error
            }
        }

    }
}
