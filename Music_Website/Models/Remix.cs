namespace Music_Website.Models
{
    public class Remix
    {
        public int RemixId { get; set; }

        public int? AdminId { get; set; }
        public string? RemixName { get; set; }

        public string? Remix_Creator { get; set; }

        public string? File_Name { get; set; }

        public int? comment_status { get; set; }

        public IFormFile? Remixfile { get; set; }
        //relation
        public ICollection<Comments>? comments { get; set; }
        public Music? music { get; set; }

        public Admin? admin { get; set; }
    }
}
