using Microsoft.AspNetCore.Mvc;
using Music_Website.Models;

namespace Music_Website.Data.Contact_Us_Data
{
    public interface IContactusDATA
    {

        public bool add_request(Contact_us contact);
        public int not_see_count();

        public int see_count();

        public List<Contact_us> Get_All_Req();

        public Contact_us Get_Req_Detail(int id);

        public bool Change_visit_status(int id);

        public bool Delete_Req(int reqid);

        public List<Contact_us> Get_seen_req();
        
        public List<Contact_us> Get_not_seen_Req();
        
    }
}
