namespace CineFlow.Areas.Admin.Models
{
    public class Actor_Movie
    {
        public int ActorId { get; set; }
        [ForeignKey(nameof(ActorId))]
        public Actor Actor { get; set; }
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }
    }
}
