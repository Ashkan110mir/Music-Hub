
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Music_Website.Models;
using System.Collections.Generic;

namespace Music_Website.Data.Singer
{
    public class SingerData : ISingerData
    {
        Context db;
        public SingerData(Context db)
        {
            this.db = db;
        }
        public string Add_Singer(Models.Singer addsinger)
        {
            try
            {
                var is_singer_exist = db.singers.Where(e => e.SingerName == addsinger.SingerName
                && e.Singer_Lastname == addsinger.Singer_Lastname && e.artistName == addsinger.artistName)
                    .SingleOrDefault();
                if (is_singer_exist == null)
                {
                    db.Add(addsinger);
                    db.SaveChanges();
                    return "true";
                }
                else
                {
                    return "exist";
                }
            }
            catch
            {
                return "false";
            }
        }

        public bool Delete_Singer(int id)
        {
            try
            {
                Models.Singer singer = new Models.Singer();
                singer = db.singers.Where(e => e.SingerId == id).First();
                db.Remove(singer);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Models.Singer> Get_All_Singer()
        {
            return db.singers.ToList();
        }

        public Models.Singer Get_singer_by_id(int id)
        {
            Models.Singer singer = new Models.Singer();
            singer = db.singers.Where(e => e.SingerId == id).Include(e=>e.Albums).First();
            return singer;
        }

        public List<Models.Singer> Search_singer(string searchname)
        {
            int number;
            if (int.TryParse(searchname, out number) != true)
            {
                return db.singers.Where
                (e => e.SingerName.Contains(searchname)
                || e.Singer_Lastname.Contains(searchname)
                || e.artistName.Contains(searchname)).ToList();
            }
            else
            {
                return db.singers.Where(e=>e.SingerId==int.Parse(searchname)).ToList();
            }
            
        }
        public bool Edit_Singer(Models.Singer singer)
        {
            try
            {
                Models.Singer changesinger = db.singers.Where(e => e.SingerId == singer.SingerId).Single();
                changesinger.SingerName = singer.SingerName;
                changesinger.Singer_Lastname = singer.Singer_Lastname;
                changesinger.artistName = singer.artistName;
                changesinger.profile_picture_name = singer.profile_picture_name;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int singer_count()
        {
            int count = db.singers.Count();
            return count;


        }

        public List<Singers_Name_ViewModel> GetSingers_Name_ViewModels()
        {
            List<Models.Singers_Name_ViewModel> singers_s = new List<Singers_Name_ViewModel>();
            var b = db.singers.Select(e => new { e.SingerName, e.Singer_Lastname, e.SingerId, e.artistName }).ToList();
            foreach (var singer in b)
            {
                singers_s.Add(new Singers_Name_ViewModel
                {
                    Name = singer.SingerName,
                    LastName = singer.Singer_Lastname,
                    ArtistName = singer.artistName,
                    Id = singer.SingerId
                });
            }
            return singers_s;

        }

        public List<Models.Singer> Get_Singer_by_Music_Id(int musicid)
        {
            var singers= db.musics.Where(e => e.SongId == musicid).SelectMany(e => e.singers).Include(e => e.Albums).ToList();
            return singers;
        }

        public Models.Singer Get_Singer_Full_Info(int singerid)
        {
            return db.singers.Where(e => e.SingerId == singerid).Include(e => e.Albums).Include(e => e.musics).Include(e => e.music_Videos).Include(e=>e.is_main_singer).FirstOrDefault();
        }

        public List<Models.Singer> Get_Paging_Singer(int pageid,int?take)
        {
            if (take.HasValue)
            {
                int takenumber = int.Parse(take.ToString());
                return db.singers.Skip((pageid - 1) * 5).Take(takenumber).ToList();
            }
            else
            {
                return db.singers.Skip((pageid - 1) * 5).Take(5).ToList();
            }
        }

        public int Search_singer_user_count(string? Searchname, string? searchmusic, string? searchmv, string? searchalbum)
        {
            IQueryable<Models.Singer> singers = db.singers;
            if (Searchname != null)
            {
                singers = singers.Where(e => e.artistName.Contains(Searchname));
            }
            if (searchmusic != null)
            {
                singers = singers.Where(e => e.musics.Where(e => e.Song_Name.Contains(searchmusic)).Any());
            }
            if (searchmv != null)
            {
                singers = singers.Where(e => e.music_Videos.Where(e => e.Mvname.Contains(searchmv)).Any());
            }
            if (searchalbum != null)
            {
                singers = singers.Where(e => e.Albums.Where(e => e.AlbumName.Contains(searchalbum)).Any());
            }
            return singers.Count();
        }
        public List<Models.Singer> Search_singer_user(string? Searchname, string? searchmusic, string? searchmv, string? searchalbum, string? orderby, int pageid )
        {
            IQueryable<Models.Singer> singers = db.singers;
            if(Searchname!= null)
            {
                singers = singers.Where(e => e.artistName.Contains(Searchname));
            }
            if(searchmusic != null)
            {
                singers = singers.Where(e => e.musics.Where(e => e.Song_Name.Contains(searchmusic)).Any());
            }
            if(searchmv!= null)
            {
                singers = singers.Where(e => e.music_Videos.Where(e => e.Mvname.Contains(searchmv)).Any());
            }
            if(searchalbum != null)
            {
                singers=singers.Where(e=>e.Albums.Where(e=>e.AlbumName.Contains(searchalbum)).Any() || e.is_main_singer.Where(e=>e.AlbumName.Contains(searchalbum)).Any());
            }
            switch(orderby)
            {
                case "orderbyname":
                    singers=singers.OrderBy(e=>e.artistName); 
                    break;

                case "orderbylast":
                    singers = singers.OrderByDescending(e => e.SingerId);
                    break;

                case "orderbyfirst":
                    singers = singers.OrderBy(e => e.SingerId);
                    break;
            }
            return singers.Skip((pageid - 1) * 5).Take(5).ToList();
        }


    }
}

