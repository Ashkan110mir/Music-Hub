namespace Music_Website.Data.Admin_Data
{
    public interface IAdminData
    {
        public Models.Admin get_admin(string username,string password);

        public Models.Admin get_admin(int id);
    }
}
