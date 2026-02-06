namespace CineFlow.Areas.Admin.Models
{
    public class Actor : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Actor Full Name")]
        [Required(ErrorMessage = "Actor full name is required")]
        public string FullName { get; set; }

        [Display(Name = "Biography")]
        public string? Bio { get; set; }

        [Display(Name = "Profile Picture URL")]
        [Required(ErrorMessage = "Actor profile picture URL is required")]
        public string ProfilePictureURL { get; set; }
        
        //Relationships
        public List<Actor_Movie> Actors_Movies { get; set; } = new List<Actor_Movie>(); 
    }
}
