namespace CineFlow.Areas.Admin.Data.Services
{
    public class MovieService : EntityBaseRepository<Movie>, IMovieService
    {
        private readonly AppDbContext _context;
        public MovieService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.MovieCategory)
                .Include(m => m.Actors_Movies)
                    .ThenInclude(am => am.Actor)
                .Include(m => m.SubImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<NewMovieDropdownsVM> GetNewMovieDropdownsValues()
        {
            // Same as before
            var response = new NewMovieDropdownsVM()
            {
                MovieCategories = await _context.MovieCategories
                    .OrderBy(mc => mc.Name)
                    .Select(mc => new SelectListItem
                    {
                        Text = mc.Name,
                        Value = mc.Id.ToString()
                    }).ToListAsync(),

                Cinemas = await _context.Cinemas
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }).ToListAsync(),

                Actors = await _context.Actors
                    .OrderBy(a => a.FullName)
                    .Select(a => new SelectListItem
                    {
                        Text = a.FullName,
                        Value = a.Id.ToString()
                    }).ToListAsync()
            };

            return response;
        }

        public async Task AddNewMovieAsync(MovieVM data)
        {
            var newMovie = new Movie()
            {
                Title = data.Title,
                Description = data.Description,
                Price = data.Price,
                ImageURL = data.ImageURL,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                MovieCategoryId = data.MovieCategoryId,
                CinemaId = data.CinemaId
            };

            await _context.Movies.AddAsync(newMovie);
            await _context.SaveChangesAsync();

            // Add Movie Actors
            if (data.ActorIds != null && data.ActorIds.Any())
            {
                foreach (var actorId in data.ActorIds)
                {
                    var newActorMovie = new Actor_Movie()
                    {
                        MovieId = newMovie.Id,
                        ActorId = actorId
                    };
                    await _context.Actors_Movies.AddAsync(newActorMovie);
                }
            }

            // Add Sub Images if provided
            if (data.SubImageFiles != null && data.SubImageFiles.Any())
            {
                var displayOrder = 0;
                for (int i = 0; i < data.SubImageFiles.Count; i++)
                {
                    var subImageFile = data.SubImageFiles[i];
                    var caption = data.NewSubImageCaptions?[i] ?? string.Empty;

                    var subImage = new MovieSubImage
                    {
                        MovieId = newMovie.Id,
                        ImageURL = await SaveSubImageAsync(subImageFile),
                        Caption = caption,
                        DisplayOrder = displayOrder++
                    };

                    await _context.MovieSubImages.AddAsync(subImage);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateMovieAsync(MovieVM data)
        {
            var dbMovie = await _context.Movies
                .Include(m => m.Actors_Movies)
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == data.Id);

            if (dbMovie != null)
            {
                dbMovie.Title = data.Title;
                dbMovie.Description = data.Description;
                dbMovie.Price = data.Price;
                dbMovie.ImageURL = data.ImageURL;
                dbMovie.StartDate = data.StartDate;
                dbMovie.EndDate = data.EndDate;
                dbMovie.MovieCategoryId = data.MovieCategoryId;
                dbMovie.CinemaId = data.CinemaId;

                // Remove existing actors
                var existingActors = _context.Actors_Movies.Where(am => am.MovieId == data.Id);
                _context.Actors_Movies.RemoveRange(existingActors);

                // Add new actors
                if (data.ActorIds != null && data.ActorIds.Any())
                {
                    foreach (var actorId in data.ActorIds)
                    {
                        var newActorMovie = new Actor_Movie()
                        {
                            MovieId = data.Id,
                            ActorId = actorId
                        };
                        await _context.Actors_Movies.AddAsync(newActorMovie);
                    }
                }

                // Handle existing sub images
                if (data.ExistingSubImages != null)
                {
                    foreach (var existingImage in data.ExistingSubImages)
                    {
                        if (existingImage.ToDelete)
                        {
                            var subImage = await _context.MovieSubImages
                                .FirstOrDefaultAsync(si => si.Id == existingImage.Id);

                            if (subImage != null)
                            {
                                // Delete physical file
                                DeleteImage(subImage.ImageURL);
                                _context.MovieSubImages.Remove(subImage);
                            }
                        }
                        else
                        {
                            // Update caption and order
                            var subImage = await _context.MovieSubImages
                                .FirstOrDefaultAsync(si => si.Id == existingImage.Id);

                            if (subImage != null)
                            {
                                subImage.Caption = existingImage.Caption;
                                subImage.DisplayOrder = existingImage.DisplayOrder;
                            }
                        }
                    }
                }

                // Add new sub images
                if (data.SubImageFiles != null && data.SubImageFiles.Any())
                {
                    var maxOrder = dbMovie.SubImages.Any()
                        ? dbMovie.SubImages.Max(si => si.DisplayOrder)
                        : -1;

                    for (int i = 0; i < data.SubImageFiles.Count; i++)
                    {
                        var subImageFile = data.SubImageFiles[i];
                        var caption = data.NewSubImageCaptions?[i] ?? string.Empty;

                        var subImage = new MovieSubImage
                        {
                            MovieId = dbMovie.Id,
                            ImageURL = await SaveSubImageAsync(subImageFile),
                            Caption = caption,
                            DisplayOrder = ++maxOrder
                        };

                        await _context.MovieSubImages.AddAsync(subImage);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<MovieVM> GetMovieForEditAsync(int id)
        {
            var movieDetails = await _context.Movies
                .Include(m => m.Actors_Movies)
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movieDetails == null)
                return null;

            var response = new MovieVM()
            {
                Id = movieDetails.Id,
                Title = movieDetails.Title,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                ImageURL = movieDetails.ImageURL,
                StartDate = movieDetails.StartDate,
                EndDate = movieDetails.EndDate,
                MovieCategoryId = movieDetails.MovieCategoryId,
                CinemaId = movieDetails.CinemaId,
                ActorIds = movieDetails.Actors_Movies.Select(am => am.ActorId).ToList(),
                ExistingSubImages = movieDetails.SubImages
                    .Select(si => new SubImageVM
                    {
                        Id = si.Id,
                        ImageURL = si.ImageURL,
                        Caption = si.Caption,
                        DisplayOrder = si.DisplayOrder
                    }).ToList()
            };

            return response;
        }

        private async Task<string> SaveSubImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            // Validate and save the image
            var uploadPath = Path.Combine("wwwroot", "img", "movies", "subimages");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type");

            if (file.Length > 2 * 1024 * 1024)
                throw new ArgumentException("File size must be less than 2MB");

            var fileName = Guid.NewGuid().ToString().Substring(0, 8) +
                           DateTime.UtcNow.ToString("_ddMMyyyy_hhmmss") +
                           extension;

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/img/movies/subimages/{fileName}";
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

        public async Task<Movie> GetMovieForEditWithTrackingAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.Actors_Movies)
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task UpdateMovieActorsAsync(int movieId, List<int>? actorIds)
        {
            // Remove existing actors
            var existingActors = _context.Actors_Movies.Where(am => am.MovieId == movieId);
            _context.Actors_Movies.RemoveRange(existingActors);

            // Add new actors if provided
            if (actorIds != null && actorIds.Any())
            {
                foreach (var actorId in actorIds)
                {
                    var newActorMovie = new Actor_Movie()
                    {
                        MovieId = movieId,
                        ActorId = actorId
                    };
                    await _context.Actors_Movies.AddAsync(newActorMovie);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<Movie>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize)
        {
            // use in MovieController Index method for searching movies
            return await SearchPaginatedAsync(m => EF.Functions.Like(m.Title, $"%{searchString}%") ||
                                                   EF.Functions.Like(m.Description!, $"%{searchString}%") ||
                                                   EF.Functions.Like(m.MovieCategory!.Name, $"%{searchString}%") ||
                                                   EF.Functions.Like(m.Cinema!.Name, $"%{searchString}%"),
                                               pageIndex,
                                               pageSize,
                                               m => m.Cinema,
                                               m => m.MovieCategory);
        }
    }
}
