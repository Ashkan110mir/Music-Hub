using System.ComponentModel.DataAnnotations;

namespace Music_Website.Models
{
    public class Contact_us
    {
        public int Request_Id { get; set; }
       
        public string? Request_Type { get; set; }

        public bool? see_by_admin { get; set; }
        public DateTime CreatedDate { get; set; }
        [EmailAddress]
        public string? Email_address { get; set; }
        [Phone]
        public string? Phone_Number { get; set; }

        public string? Request_Text { get;set; }


    }
}
