namespace CineFlow.Areas.Admin.Models
{
    public class Movie : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Movie Name")]
        [Required(ErrorMessage = "Movie name is required")]
        public string Title { get; set; }

        [Display(Name = "Movie Description")]
        [Required(ErrorMessage = "Movie description is required")]
        public string Description { get; set; }

        [Display(Name = "Movie Price in $")]
        [Required(ErrorMessage = "Movie price is required")]
        public decimal Price { get; set; }

        [Display(Name = "Movie Image URL")]
        [Required(ErrorMessage = "Movie image URL is required")]
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

        [ForeignKey(nameof(MovieCategoryId))]
        public MovieCategory MovieCategory { get; set; }

        public int CinemaId { get; set; }

        [ForeignKey(nameof(CinemaId))]
        public Cinema Cinema { get; set; }

        // Relationships
        public List<Actor_Movie> Actors_Movies { get; set; }

        // Multiple sub-images relationship
        public List<MovieSubImage> SubImages { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
