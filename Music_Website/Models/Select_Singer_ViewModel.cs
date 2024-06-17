namespace Music_Website.Models
{
    public class Select_Singer_ViewModel
    {
        public int Id { get; set; }

        public string SingerName { get; set; }

        public string Singer_Lastname { get; set; }

        public string artistName { get; set; }
        
        ICollection<Albums> albums { get; set; }
    }
}
