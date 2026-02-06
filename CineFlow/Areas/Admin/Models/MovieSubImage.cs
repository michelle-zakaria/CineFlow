namespace CineFlow.Areas.Admin.Models
{
    public class MovieSubImage : IEntityBase
    {
        public int Id { get; set; }

        [Display(Name = "Image URL")]
        [Required(ErrorMessage = "Image URL is required")]
        public string ImageURL { get; set; }

        [Display(Name = "Caption")]
        public string? Caption { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 0;

        public int MovieId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }
    }
}
