using Music_Website.Models;

namespace Music_Website.Data.Singer
{
    public interface ISingerData
    {
        public string Add_Singer(Models.Singer addsinger);

        public int singer_count();

        public List<Models.Singer> Get_All_Singer();

        public List<Models.Singer> Search_singer(string searchname);

        public bool Delete_Singer(int id);

        public Models.Singer Get_singer_by_id(int id);

        public bool Edit_Singer(Models.Singer singer);

        public List<Models.Singers_Name_ViewModel> GetSingers_Name_ViewModels();

        public List<Models.Singer> Get_Singer_by_Music_Id(int musicid);

        public Models.Singer Get_Singer_Full_Info(int singerid);

        public List<Models.Singer> Get_Paging_Singer(int pageid,int?take);

        public int Search_singer_user_count(string? Searchname, string? searchmusic, string? searchmv, string? searchalbum);
        public List<Models.Singer> Search_singer_user(string? Searchname, string? seachmusic, string? searchmv, string? searchalbum, string? orderby, int pageid = 1);

       

    }
}
