using Microsoft.EntityFrameworkCore;
using Music_Website.Models;
using NuGet.Packaging;

namespace Music_Website.Data.Albums_Data
{
    public class AlbumsData : IAlbumsData
    {
        private Context db;
        public AlbumsData(Context db)
        {
            this.db = db;
        }

        public bool Add_Album(string name, Models.Singer singer)
        {
            try
            {
                Albums albums = new Albums()
                {
                    AlbumCount = 0,
                    AlbumName = name,
                    main_singer = singer
                };
                db.Albums.Add(albums);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Albums> Get_All_Albums()
        {
            return db.Albums.Include(e => e.singers).ToList();
        }

        public int albums_count()
        {
            return db.Albums.Count();
        }

        public List<Albums> Search_Albums(string name)
        {
            int num;
            if (int.TryParse(name, out num) == false)
            {
                return db.Albums.Where(e => e.AlbumName.Contains(name)).Include(e => e.singers).ToList();
            }
            else
            {
                return db.Albums.Where(e => e.AlbumId == num).Include(e => e.singers).ToList();
            }

        }

        public bool delete_Album(int albumid)
        {
            try
            {
                Albums albums = db.Albums.Where(e => e.AlbumId == albumid).First();
                db.Albums.Remove(albums);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Albums> GetAlbumsByid(int albumid)
        {
            return db.Albums.Where(e => e.AlbumId == albumid).Include(e => e.singers).ToList();
        }
        public Albums GetAlbumByid(int? albumid)
        {
            return db.Albums.Where(e => e.AlbumId == albumid).Include(e => e.singers).Include(e=>e.main_singer).SingleOrDefault();
        }
        public bool add_or_remove_more_singer(int albumid,List<Models.Singer> singersid, int more_or_less)
        {
            try
            {
                var album = db.Albums.Where(e => e.AlbumId == albumid).FirstOrDefault();
                if (more_or_less == 1)
                {
                    album.singers.AddRange(singersid);
                }
                else
                {
                    foreach(var singer in singersid)
                    {
                        album.singers.Remove(singer);
                    }       
                }
                db.SaveChanges(); 
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Edit_Albums(Albums editalbum)
        {
            try
            {
                var album=db.Albums.Where(e=>e.AlbumId==editalbum.AlbumId).FirstOrDefault();
                if(album==null)
                {
                    return false;
                }
                else
                {
                    album.AlbumName = editalbum.AlbumName;
                    db.SaveChanges();
                    return true;
                }
                

            }
            catch
            {
                return false;
            }


        }

        public List<Albums> get_albums_by_main_singers(List<int> singerid)
        {
            List<Models.Albums> albums = new List<Models.Albums>();
            foreach (var singer in singerid)
            {
                var album = db.Albums.Where(e => e.main_singer.SingerId == singer).Include(e => e.main_singer).ToList();
                if (album != null)
                {
                    albums.AddRange(album);
                }
            }
            return albums;


        }
        public List<Albums> getAlbumsBySingersId(List<int> singersid)
        {
            List<Albums> albums = new List<Albums>();
            foreach (var singer in singersid)
            {
                albums.AddRange(db.singers.Where(e => e.SingerId == singer).SelectMany(e => e.Albums).ToList());
                ;
            }
            albums = albums.Distinct().ToList();
            return albums;


        }

        public bool change_count(Albums? albums, int change)
        {
            try
            {
                if (change == 1)
                {
                    var album = db.Albums.Where(e => e.AlbumId == albums.AlbumId).Single();
                    album.AlbumCount = albums.AlbumCount + 1;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    var album = db.Albums.Where(e => e.AlbumId == albums.AlbumId).Single();
                    album.AlbumCount = albums.AlbumCount - 1;
                    db.SaveChanges();
                    return true;

                }

            }
            catch
            {
                return false;
            }
        }

        public Albums Get_Album_Deteil(int albumid)
        {
            return db.Albums.Where(e => e.AlbumId == albumid).Include(e => e.musics).Include(e => e.singers).Include(e=>e.main_singer).FirstOrDefault();
        }

        public List<Albums> Get_paging_album(int pageid)
        {
            return db.Albums.Skip((pageid - 1) * 5).Take(5).Include(e => e.singers).Include(e=>e.main_singer).ToList();
        }
        public List<Albums> Search_album_user(string? Searchname, string? searchmusic, string? searchsinger, string? orderby, int pageid)
        {
            IQueryable<Albums> albums = db.Albums;
            if (Searchname != null)
            {
                albums = albums.Where(e => e.AlbumName.Contains(Searchname));
            }
            if (searchmusic != null)
            {
                albums = albums.Where(e => e.musics.Where(e => e.Song_Name.Contains(searchmusic)).Any());
            }
            if (searchsinger != null)
            {
                albums = albums.Where(e => e.singers.Where(e => e.artistName.Contains(searchsinger)).Any() || e.main_singer.artistName.Contains(searchsinger));
            }
            switch (orderby)
            {
                case "orderbyname":
                    albums = albums.OrderBy(e => e.AlbumName);
                    break;
                case "orderbylast":
                    albums = albums.OrderByDescending(e => e.AlbumId);
                    break;
                case "orderbyfirst":
                    albums = albums.OrderBy(e => e.AlbumId);
                    break;
            }
            int skip = (pageid - 1) * 5;
            return albums.Skip(skip).Take(5).Include(e => e.singers).Include(e=>e.main_singer).ToList();
        }

        public int Search_album_user_count(string? Searchname, string? searchmusic, string? searchsinger)
        {
            IQueryable<Albums> albums = db.Albums;
            if (Searchname != null)
            {
                albums = albums.Where(e => e.AlbumName.Contains(Searchname));
            }
            if (searchmusic != null)
            {
                albums = albums.Where(e => e.musics.Where(e => e.Song_Name.Contains(searchmusic)).Any());
            }
            if (searchsinger != null)
            {
                albums = albums.Where(e => e.singers.Where(e => e.artistName.Contains(searchsinger)).Any());
            }
            return albums.Count();
        }

        public List<Albums> Get_paging_album_admin(int pageid)
        {
            int skip = (pageid - 1) * 20;
            return db.Albums.OrderBy(e => e.AlbumId).Skip(skip).Take(20).Include(e => e.singers).Include(e => e.main_singer).ToList();
        }

        
    }
}
