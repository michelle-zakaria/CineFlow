namespace CineFlow.Areas.Admin.Models
{
    public class MovieCategory : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Category description is required")]
        public string? Description { get; set; }

        //Relationships
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
