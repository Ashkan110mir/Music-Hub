using Music_Website.Models;

namespace Music_Website.Data.Albums_Data
{
    public interface IAlbumsData
    {
        public int albums_count();
        public List<Albums> Get_All_Albums();
        public bool Add_Album(string name, Models.Singer singer);

        public List<Albums> Search_Albums(string name);

        public bool add_or_remove_more_singer(int albumid,List<Models.Singer> singersid,int more_or_less);
        public bool delete_Album(int albumid);
        public List<Albums> get_albums_by_main_singers(List<int> singerid);
        public bool Edit_Albums(Albums albums);

        public bool change_count(Albums? albums,int change);
        public List<Albums> GetAlbumsByid(int albumid);
        public Albums GetAlbumByid(int? albumid);
        public List<Albums> getAlbumsBySingersId(List<int> singersid);

        public Albums Get_Album_Deteil(int albumid);

        public List<Albums> Get_paging_album(int pageid);

        public int Search_album_user_count(string? Searchname, string? searchmusic, string? searchsinger);
        public List<Albums> Search_album_user(string? Searchname, string? searchmusic, string? searchsinger, string? orderby, int pageid);

        public List<Albums> Get_paging_album_admin(int pageid);

    }
}
