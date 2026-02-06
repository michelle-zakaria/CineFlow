namespace CineFlow.Areas.Admin.Data.ViewModels
{
    public class SubImageVM
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
        public bool ToDelete { get; set; } = false;
    }
}
