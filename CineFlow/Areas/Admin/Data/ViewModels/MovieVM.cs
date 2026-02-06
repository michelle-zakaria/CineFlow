namespace CineFlow.Areas.Admin.Data.ViewModels
{
    public class MovieVM
    {
        public int Id { get; set; }

        [Display(Name = "Movie Name")]
        [Required(ErrorMessage = "Movie name is required")]
        public string Title { get; set; }

        [Display(Name = "Movie Description")]
        [Required(ErrorMessage = "Movie description is required")]
        public string Description { get; set; }

        [Display(Name = "Movie Price in $")]
        [Required(ErrorMessage = "Movie price is required")]
        [Range(0.01, 1000, ErrorMessage = "Price must be between $0.01 and $1000")]
        public decimal Price { get; set; }

        // Main poster
        [Display(Name = "Poster Image")]
        [Required(ErrorMessage = "Poster image is required")]
        public string ImageURL { get; set; }

        [Display(Name = "Movie Start Date")]
        [Required(ErrorMessage = "Movie start date is required")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Movie End Date")]
        [Required(ErrorMessage = "Movie end date is required")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Movie Category")]
        [Required(ErrorMessage = "Movie category is required")]
        public int MovieCategoryId { get; set; }

        [Display(Name = "Cinema")]
        [Required(ErrorMessage = "Cinema is required")]
        public int CinemaId { get; set; }

        [Display(Name = "Actors")]
        public List<int>? ActorIds { get; set; }

        // File upload for main poster
        [Display(Name = "Poster Image")]
        public IFormFile? PosterImageFile { get; set; }

        // Multiple sub-images
        [Display(Name = "Sub Images")]
        public List<IFormFile>? SubImageFiles { get; set; }

        // For editing - existing sub images
        public List<SubImageVM>? ExistingSubImages { get; set; }

        // For adding captions to new sub images
        public List<string>? NewSubImageCaptions { get; set; }

        // For dropdowns
        public List<SelectListItem>? MovieCategories { get; set; }
        public List<SelectListItem>? Cinemas { get; set; }
        public List<SelectListItem>? Actors { get; set; }
    }
}
