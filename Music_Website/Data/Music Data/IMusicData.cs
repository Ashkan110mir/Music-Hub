using Music_Website.Models;

namespace Music_Website.Data.Music_Data
{
    public interface IMusicData
    {
        public int Music_Count();

        public List<Music> Get_All_Music();

        public bool Add_Music(Music music);

        public Music Get_Music_By_Id(int id);
        public bool remove_Music(Music music);

        public List<Music> Get_music_by_singerid(int singerid);
        public Music Get_Full_Music_info_by_id(int id);
        public List<Music> Search_Music(string searchname);
        public int Search_Music_count(string searchname, string searchsinger, string searchstyle, string orderby);
        public List<Music> Search_Music(string searchname, string searchsinger, string searchstyle,string orderby,int pageid=1);

        public bool edit_music(Music music);

        public List<Music_Id_Modelview> Get_Music_name();

        public List<Music> Get_Main_Index_musics();

        public int Get_Index_music_Number();

        public List<Music> Get_paging_music(int pageid);

        public List<Music> Get_Paging_Admin_music(int pageid);

        public List<Music> Get_not_main_menu_selected_music();

        public List<Music> Get_Music_By_Album_id(int albumid);

    }
}
