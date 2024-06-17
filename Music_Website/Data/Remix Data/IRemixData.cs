using Music_Website.Models;

namespace Music_Website.Data.Remix_Data
{
    public interface IRemixData
    {
        public int RemixCount();

        public List<Remix> Get_all_remix();

        public bool add_remix(Remix addremix);

        public List<Remix> search_remix(string searchname);

        public bool delete_remix(Remix remix);

        public Remix get_remix_by_id(int id);

        public bool Edit_remix(Remix remix);

        public Remix Get_Full_Remix_Info(int remixid);

        public List<Remix> Get_Paging_remix(int pageid);
        public int search_remix_user_count(string? searchname, string? searchcreator, string? searchsong);
        public List<Remix> Search_remix_user(string? searchname, string? searchcreator, string? searchsong, string? orderby, int pageid = 1);

        public List<Remix> Get_Paging_Remix_admin(int pageid);

    }
}
