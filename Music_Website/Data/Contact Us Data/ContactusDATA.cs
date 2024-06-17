using Microsoft.AspNetCore.Mvc;
using Music_Website.Models;

namespace Music_Website.Data.Contact_Us_Data
{
    public class ContactusDATA : IContactusDATA
    {
        private Context db;
        public ContactusDATA(Context db)
        {
            this.db = db;
        }

        public bool add_request(Contact_us contact)
        {
            try
            {
                db.contact_Us.Add(contact);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Change_visit_status(int id)
        {
            try
            {
                var req=db.contact_Us.Where(e=>e.Request_Id== id).FirstOrDefault();
                req.see_by_admin= true;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete_Req(int reqid)
        {
            try
            {
                var req=db.contact_Us.Where(e=>e.Request_Id==reqid).First();
                db.Remove(req);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Contact_us> Get_All_Req()
        {
            return db.contact_Us.ToList();
        }

        public List<Contact_us> Get_not_seen_Req()
        {
            return db.contact_Us.Where(e => e.see_by_admin == false).ToList();
        }

        public Contact_us Get_Req_Detail(int id)
        {
            return db.contact_Us.Where(e => e.Request_Id == id).FirstOrDefault();
        }

        public List<Contact_us> Get_seen_req()
        {
            return db.contact_Us.Where(e=>e.see_by_admin==true).ToList();
        }

        public int not_see_count()
        {
            return db.contact_Us.Where(e=>e.see_by_admin==false).Count();
        }

        public int see_count()
        {
            return db.contact_Us.Where(e => e.see_by_admin == true).Count();
        }

    }
}
