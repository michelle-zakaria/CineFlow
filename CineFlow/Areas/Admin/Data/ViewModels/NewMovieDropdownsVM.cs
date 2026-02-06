namespace CineFlow.Areas.Admin.Data.ViewModels
{
    public class NewMovieDropdownsVM
    {
        public List<SelectListItem>? MovieCategories { get; set; }
        public List<SelectListItem>? Cinemas { get; set; }
        public List<SelectListItem>? Actors { get; set; }
    }
}
