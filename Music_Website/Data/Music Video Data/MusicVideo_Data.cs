using Microsoft.EntityFrameworkCore;
using Music_Website.Models;
using System.Drawing.Printing;

namespace Music_Website.Data.Music_Video_Data
{
    public class MusicVideo_Data : IMusicVideo_Data
    {
        private Context db;
        public MusicVideo_Data(Context db)
        {
            this.db = db;
        }

        public int music_video_count()
        {
            int count = db.music_Videos.Count();
            return count;
        }
        public bool Add_music_video(Music_Video music_Video)
        {
            try
            {
                db.music_Videos.Add(music_Video);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Music_Video> Get_All_Music_video()
        {
            return db.music_Videos.Include(e => e.admin).Include(e => e.singers).ToList();
        }
        public List<Music_Video> search_mv(string? searchname)
        {
            int itsid;
            int.TryParse(searchname, out itsid);
            if (itsid == 0)
            {
                return db.music_Videos.Where(e => e.Mvname.Contains(searchname) || e.singers.Where(e => e.artistName.Contains(searchname)).Any()).Include(e => e.admin).Include(e => e.singers).ToList();
            }
            else
            {
                return db.music_Videos.Where(e => e.MVId == itsid).Include(e => e.singers).Include(e => e.admin).ToList();
            }
        }
        public bool Remove_music_video(int mvid)
        {
            try
            {
                Music_Video mv = new Music_Video();
                mv = db.music_Videos.Where(e => e.MVId == mvid).Single();
                db.Remove(mv);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public Music_Video get_musicvideo_by_id(int mvid)
        {
            return db.music_Videos.Where(e => e.MVId == mvid).Include(e => e.singers).Single();
        }

        public bool edit_mv(Music_Video mv)
        {
            try
            {
                var editmv = db.music_Videos.Where(e => e.MVId == mv.MVId).SingleOrDefault();
                if (editmv != null)
                {
                    editmv.MVId = mv.MVId;
                    editmv.Mvname = mv.Mvname;
                    editmv.Mv_Describe = mv.Mv_Describe;
                    editmv.comment_status = mv.comment_status;
                    if (mv.File_Name != null)
                    {
                        editmv.File_Name = mv.File_Name;
                    }
                    if (mv.MvposterName != null)
                    {
                        editmv.MvposterName = mv.MvposterName;
                    }
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public Music_Video Get_Full_mv_info(int mvid)
        {
            return db.music_Videos.Where(e => e.MVId == mvid).Include(e => e.admin).Include(e => e.singers).FirstOrDefault();
        }

        public Music_Video Get_main_menu_mv()
        {
            return db.music_Videos.Where(e => e.in_main_index == true).FirstOrDefault();
        }

        public List<Music_Video> Get_Mv_Paging(int pageid)
        {
            int skip = (pageid - 1) * 5;
            return db.music_Videos.OrderByDescending(e => e.MVId).Skip(skip).Take(5).Include(e => e.singers).ToList();
        }
        public int Search_mv_for_user_count(string? searchname, string? searnsinger)
        {
            IQueryable<Music_Video> music_Videos = db.music_Videos;
            if (searchname != null)
            {
                music_Videos = music_Videos.Where(e => e.Mvname.Contains(searchname));
            }
            if (searnsinger != null)
            {
                music_Videos = music_Videos.Where(e => e.singers.Where(e => e.artistName.Contains(searnsinger)).Any());
            }
            return music_Videos.Count();
        }
        public List<Music_Video> Search_mv_for_user(string? searchname, string? searnsinger, string? orderby, int pageid = 1)
        {
            IQueryable<Music_Video> music_Videos = db.music_Videos;
            if (searchname != null)
            {
                music_Videos = music_Videos.Where(e => e.Mvname.Contains(searchname));
            }
            if (searnsinger != null)
            {
                music_Videos = music_Videos.Where(e => e.singers.Where(e => e.artistName.Contains(searnsinger)).Any());
            }
            switch (orderby)
            {
                case "orderbyname":
                    music_Videos = music_Videos.OrderBy(e => e.Mvname);
                    break;
                case "orderbylast":
                    music_Videos = music_Videos.OrderByDescending(e => e.MVId);
                    break;
                case "orderbyfirst":
                    music_Videos = music_Videos.OrderBy(e => e.MVId);
                    break;
            }
            int skip = (pageid - 1) * 5;
            return music_Videos.Skip(skip).Take(5).Include(e=>e.singers).ToList();
        }

        public List<Music_Video> Get_Mv_paging_admin(int pageid)
        {
            int skip=(pageid-1) * 20;
            return db.music_Videos.OrderBy(e => e.MVId).Skip(skip).Take(20).Include(e=>e.singers).Include(e=>e.admin).ToList();
        }

        public List<Music_Video> Get_not_main_menu_selected_mv()
        {
            return db.music_Videos.Where(e => e.in_main_index == false).ToList();
        }

    }
}
