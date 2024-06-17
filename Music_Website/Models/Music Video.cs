namespace Music_Website.Models
{
    public class Music_Video
    {
        public int MVId { get; set; }

        public int? AdminId { get; set; }
        public string? Mvname { get; set; }

        
        public string? Mv_Describe { get; set; }

        public DateTime Mv_publishdate { get; set; }

        public string? File_Name { get; set; }

        public string? MvposterName { get;set; }
        public int? comment_status { get; set; }

        public bool? in_main_index { get; set; }

        public IFormFile? MvposterFile { get; set; }
        public IFormFile? MVfile { get; set; }
        //relation
        public ICollection<Singer>? singers { get; set; }

        public ICollection<Comments>? comments { get; set; }
        public Admin? admin { get; set; }

        

    }
}
