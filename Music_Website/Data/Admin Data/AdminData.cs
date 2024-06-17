

using Newtonsoft.Json.Serialization;

namespace Music_Website.Data.Admin_Data
{
    public class AdminData : IAdminData
    {
        private Context db;
        public AdminData(Context db)
        {
            this.db = db;
        }
        public Models.Admin get_admin(string username, string password)
        {
            return db.admins.Where(e=>e.Username==username && e.Password==password).SingleOrDefault();
        }

        public Models.Admin get_admin(int id)
        {
            return db.admins.Where(e => e.Admin_ID == id).First();
        }
    }
}
