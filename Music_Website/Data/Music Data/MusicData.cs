using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Music_Website.Models;

namespace Music_Website.Data.Music_Data
{
    public class MusicData : IMusicData
    {
        private Context db;
        public MusicData(Context db)
        {
            this.db = db;
        }

        public List<Music> Get_All_Music()
        {
            return db.musics.Include(e => e.album).Include(e => e.singers).Include(e => e.admin).ToList();
        }

        public int Music_Count()
        {
            int count = db.musics.Count();
            return count;
        }

        public bool Add_Music(Music music)
        {
            try
            {
                db.musics.Add(music);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Music Get_Music_By_Id(int id)
        {
            return db.musics.Where(e => e.SongId == id).Include(e => e.album).Single();
        }
        public Music Get_Full_Music_info_by_id(int id)
        {
            return db.musics.Where(e => e.SongId == id).Include(e => e.album).Include(e => e.admin).Include(e => e.singers).SingleOrDefault();
        }

        public bool remove_Music(Music music)
        {
            try
            {
                var remixs = db.remixes.Where(e => e.music.SongId == music.SongId).ToList();
                var comments = db.comment.Where(e => e.music.SongId == music.SongId).ToList();
                foreach (var remix in remixs)
                {
                    remix.music = null;
                }
                db.musics.Remove(music);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public List<Music> Search_Music(string searchname)

        {
            int check_is_number;
            int.TryParse(searchname, out check_is_number);
            if (check_is_number == 0)
            {
                return db.musics.Where(e => e.Song_Name == searchname || e.Music_Style == searchname || e.singers.Where(e => e.artistName.Contains(searchname)).Any()).Include(e => e.singers).Include(e => e.album).Include(e => e.admin).ToList();
            }
            else
            {
                return db.musics.Where(e => e.SongId == check_is_number).Include(e => e.singers).Include(e => e.album).Include(e => e.admin).ToList();
            }
        }

        public bool edit_music(Music music)
        {
            //  try
            //  {
            Music bmusic = new Music();
            bmusic = db.musics.Where(e => e.SongId == music.SongId).Include(e => e.album).Single();
            bmusic.Music_Style = music.Music_Style;
            bmusic.Song_Name = music.Song_Name;
            bmusic.comment_status = music.comment_status;
            bmusic.album = music.album;
            bmusic.Song_Description = music.Song_Description;
            if (music.Image_Filename != null)
            {
                bmusic.Image_Filename = music.Image_Filename;
            }
            if (music.Song_FileName != null)
            {
                bmusic.Song_FileName = music.Song_FileName;
            }

            db.SaveChanges();
            return true;
            //  }
            //catch
            //{
            //    return false;
            //}
        }

        public List<Music_Id_Modelview> Get_Music_name()
        {
            return db.musics.Select(e => new Music_Id_Modelview
            {
                Id = e.SongId,
                Name = e.Song_Name
            }).ToList();
        }

        public List<Music> Get_Main_Index_musics()
        {
            return db.musics.Where(e => e.in_main_index == true).ToList();
        }
        public int Get_Index_music_Number()
        {
            return db.musics.Where(e => e.in_main_index == true).Count();
        }

        public List<Music> Get_paging_music(int pageid)
        {
            int skip = (pageid - 1) * 5;
            return db.musics.OrderByDescending(e => e.SongId).Skip(skip).Take(5).Include(e=>e.singers).ToList();
        }

        public int Search_Music_count(string searchname, string searchsinger, string searchstyle, string orderby)
        {
            IQueryable<Music> musics = db.musics;
            if (!string.IsNullOrEmpty(searchname))
            {
                musics = musics.Where(e => e.Song_Name.Contains(searchname));

            }
            if (!string.IsNullOrEmpty(searchstyle))
            {
                musics = musics.Where(e => e.Music_Style.Contains(searchstyle));
            }
            if (!string.IsNullOrEmpty(searchsinger))
            {
                musics = musics.Where(e => e.singers.Where(e => e.artistName.Contains(searchsinger)).Any());
            }
            if (orderby != null)
            {
                switch (orderby)
                {
                    case "orderbyname":
                        musics = musics.OrderBy(e => e.Song_Name);
                        break;
                    case "orderbydateLast":
                        musics = musics.OrderByDescending(e => e.Song_Date);
                        break;
                    case "orderbydateFirst":
                        musics.OrderBy(e => e.Song_Date);
                        break;
                }
            }
            return musics.Count();
        }
        public List<Music> Search_Music(string? searchname, string? searchsinger, string? searchstyle, string? orderby, int pageid = 1)
        {
            int skip = (pageid - 1) * 5;
            IQueryable<Music> musics = db.musics;
            if (!string.IsNullOrEmpty(searchname))
            {
                musics = musics.Where(e => e.Song_Name.Contains(searchname));

            }
            if (!string.IsNullOrEmpty(searchstyle))
            {
                musics = musics.Where(e => e.Music_Style.Contains(searchstyle));
            }
            if (!string.IsNullOrEmpty(searchsinger))
            {
                musics = musics.Where(e => e.singers.Where(e => e.artistName.Contains(searchsinger)).Any());
            }
            if (orderby != null)
            {
                switch (orderby)
                {
                    case "orderbyname":
                        musics = musics.OrderBy(e => e.Song_Name);
                        break;
                    case "orderbydateLast":
                        musics = musics.OrderByDescending(e => e.Song_Date);
                        break;
                    case "orderbydateFirst":
                        musics.OrderBy(e => e.Song_Date);
                        break;
                }
            }
            return musics.Skip(skip).Take(5).Include(e=>e.singers).ToList();

        }

        public List<Music> Get_Paging_Admin_music(int pageid)
        {
            int skip = (pageid - 1) * 20;
            return db.musics.OrderBy(e => e.SongId).Skip(skip).Take(20).Include(e => e.singers).Include(e=>e.album).Include(e=>e.admin).ToList();
        }

        public List<Music> Get_not_main_menu_selected_music()
        {
            return db.musics.Where(e => e.in_main_index == false).ToList();
        }

        public List<Music> Get_music_by_singerid(int singerid)
        {
            return db.musics.Where(e=>e.singers.Where(e=>e.SingerId== singerid).Any()).Include(e=>e.singers).Include(e=>e.album).ToList();
        }

        public List<Music> Get_Music_By_Album_id(int albumid)
        {
            var music = db.musics.Where(e => e.album.AlbumId == albumid).ToList();
            return music;
        }
    }

}
