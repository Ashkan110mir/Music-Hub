using Microsoft.EntityFrameworkCore;
using Music_Website.Models;

namespace Music_Website.Data.Remix_Data
{
    public class RemixData : IRemixData
    {
        private Context db;
        public RemixData(Context db)
        {
            this.db = db;
        }

        public List<Remix> Get_all_remix()
        {
            return db.remixes.Include(e => e.music).Include(e => e.admin).ToList();
        }

        public int RemixCount()
        {
            return db.remixes.Count();
        }

        public bool add_remix(Remix addremix)
        {
            try
            {
                db.Add(addremix);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Remix> search_remix(string searchname)
        {
            int id;
            int.TryParse(searchname, out id);
            if (id == 0)
            {
                return db.remixes.Where(e => e.RemixName.Contains(searchname) || e.Remix_Creator.Contains(searchname) || e.music.Song_Name.Contains(searchname)).Include(e => e.admin).Include(e => e.music).ToList();
            }
            else
            {
                return db.remixes.Where(e => e.RemixId == id).Include(e => e.admin).Include(e => e.music).ToList();
            }
        }

        public bool delete_remix(Remix remix)
        {
            try
            {
                if (remix == null)
                {
                    return false;
                }
                else
                {
                    db.Remove(remix);
                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public Remix get_remix_by_id(int id)
        {
            return db.remixes.Where(e => e.RemixId == id).Include(e=>e.music).FirstOrDefault();
        }

        public bool Edit_remix(Remix remix)
        {
            try
            {
                if (remix != null)
                {
                    var editremix = db.remixes.Where(e => e.RemixId == remix.RemixId).Include(e => e.music).FirstOrDefault();
                    editremix.Remix_Creator = remix.Remix_Creator;
                    editremix.RemixName = remix.RemixName;
                    editremix.comment_status = remix.comment_status;
                    editremix.music = remix.music;
                    if (remix.File_Name != null)
                    {
                        editremix.File_Name = remix.File_Name;
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

        public Remix Get_Full_Remix_Info(int remixid)
        {
            return db.remixes.Where(e => e.RemixId == remixid).Include(e => e.admin).Include(e => e.music).FirstOrDefault();
        }

        public List<Remix> Get_Paging_remix(int pageid)
        {
            return db.remixes.Skip((pageid - 1) * 5).Take(5).ToList();
        }

        public int search_remix_user_count(string? searchname, string? searchcreator, string? searchsong)
        {
            IQueryable<Remix> Remixes = db.remixes;
            if (searchname != null)
            {
                Remixes = Remixes.Where(e => e.RemixName.Contains(searchname));
            }
            if (searchcreator != null)
            {
                Remixes = Remixes.Where(e => e.Remix_Creator.Contains(searchcreator));
            }
            if (searchsong != null)
            {
                Remixes = Remixes.Where(e => e.music.Song_Name.Contains(searchsong));
            }
            return Remixes.Count();
        }
        public List<Remix> Search_remix_user(string? searchname, string? searchcreator, string? searchsong, string? orderby, int pageid = 1)
        {
            IQueryable<Remix> Remixes = db.remixes;
            if (searchname != null)
            {
                Remixes = Remixes.Where(e => e.RemixName.Contains(searchname));
            }
            if (searchcreator != null)
            {
                Remixes = Remixes.Where(e => e.Remix_Creator.Contains(searchcreator));
            }
            if (searchsong != null)
            {
                Remixes = Remixes.Where(e => e.music.Song_Name.Contains(searchsong));
            }
            switch (orderby)
            {
                case "orderbyname":
                    Remixes = Remixes.OrderBy(e => e.RemixName);
                    break;
                case "orderbylast":
                    Remixes = Remixes.OrderByDescending(e => e.RemixId);
                    break;
                case "orderbyfirst":
                    Remixes = Remixes.OrderBy(e => e.RemixId);
                    break;
            }
            return Remixes.Skip((pageid - 1) * 5).Take(5).ToList();
        }

        public List<Remix> Get_Paging_Remix_admin(int pageid)
        {
            int skip=(pageid - 1)*5;
            return db.remixes.OrderBy(e=>e.RemixId).Skip(skip).Take(20).Include(e=>e.admin).Include(e=>e.music).ToList();
        }
    }
}
