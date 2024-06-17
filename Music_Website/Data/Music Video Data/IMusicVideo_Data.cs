using Music_Website.Models;

namespace Music_Website.Data.Music_Video_Data
{
    public interface IMusicVideo_Data
    {
        public int music_video_count();

        public List<Music_Video> Get_All_Music_video();
        public bool Add_music_video(Music_Video music_Video);

        public bool Remove_music_video(int mvid);

        public List<Music_Video> search_mv(string searchname);

        public Music_Video get_musicvideo_by_id(int mvid);

        public bool edit_mv(Music_Video mv);

        public Music_Video Get_Full_mv_info(int mvid);

        public Music_Video Get_main_menu_mv();

        public List<Music_Video> Get_Mv_Paging(int pageid);
        public int Search_mv_for_user_count(string? searchname, string? searnsinger);
        public List<Music_Video> Search_mv_for_user(string? searchname, string? searnsinger, string? orderby,int pageid=1);

        public List<Music_Video> Get_Mv_paging_admin(int pageid);

        public List<Music_Video> Get_not_main_menu_selected_mv();


    }
}
