using Microsoft.EntityFrameworkCore;
using Music_Website.Models;

namespace Music_Website.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
        //dbset
        #region dbset
        public DbSet<Models.Admin> admins { get; set; }
        public DbSet<Music> musics { get; set; }

        public DbSet<Comments> comment { get; set; }
        public DbSet<Contact_us> contact_Us { get; set; }

        public DbSet<Music_Video> music_Videos { get; set; }

        public DbSet<Remix> remixes { get; set; }


        public DbSet<Models.Singer> singers { get; set; }

        public DbSet<Albums> Albums { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Admin Config
            modelBuilder.Entity<Models.Admin>(b =>
            {
                b.HasKey(e => e.Admin_ID);
                b.Property(e => e.Admin_ID).UseIdentityColumn();
                b.Property(e => e.Admin_Name).IsRequired().HasMaxLength(50);
                b.Property(e => e.Admin_lastname).IsRequired().HasMaxLength(50);
                b.Property(e => e.Username).IsRequired();
                b.Property(e => e.Password).IsRequired();
                b.Property(e => e.Email).IsRequired().HasMaxLength(62);
                b.Property(e => e.Phone).IsRequired().HasMaxLength(15);
                b.Property(e => e.isprimary_admin).IsRequired();
                //Seed data
                b.HasData(new Models.Admin()
                {
                    Admin_ID = 1,
                    Admin_Name = "Ashkan",
                    Admin_lastname = "Mirdamadi",
                    Username = "Ashkan110mir",
                    Password = "12345678Asdf",
                    Email = "ashkanvirus11@gmail.com",
                    Phone = "09908752252",
                    isprimary_admin = true,
                });
                //relations
                b.HasMany(e => e.musics).WithOne(e => e.admin);

                b.HasMany(e => e.music_Videos).WithOne(e => e.admin)
                .HasForeignKey(e => e.AdminId).IsRequired();

                b.HasMany(e => e.remixes).WithOne(e => e.admin)
                .HasForeignKey(e => e.AdminId).IsRequired();


            });

            #endregion
            #region Song Config
            modelBuilder.Entity<Music>(b =>
            {
                b.HasKey(e => e.SongId);
                b.Property(e => e.SongId).UseIdentityColumn().IsRequired();
                b.Property(e => e.Song_Name).HasMaxLength(70).IsRequired();
                b.Property(e => e.Song_Description).IsRequired();
                b.Property(e => e.Music_Style).HasMaxLength(50).IsRequired();
                b.Property(e => e.Song_FileName).IsRequired();
                b.Property(e => e.Image_Filename).IsRequired();
                b.HasIndex(e => e.AlbumId);
                b.Ignore(e => e.Image_File);
                b.Ignore(e => e.Song_File);
                //relation
                b.HasOne(e => e.album).WithMany(e => e.musics)
                .HasForeignKey(e => e.AlbumId);
                b.HasMany(e => e.singers).WithMany(e => e.musics);
                b.HasMany(e => e.remixes).WithOne(e => e.music);
            });

            #endregion
            #region Singer Config
            modelBuilder.Entity<Models.Singer>(b =>
            {
                b.HasKey(e => e.SingerId);
                b.Property(e => e.SingerId).UseIdentityColumn().IsRequired();
                b.Property(e => e.SingerName).HasMaxLength(50).IsRequired();
                b.Property(e => e.Singer_Lastname).HasMaxLength(50).IsRequired();
                b.Property(e => e.artistName).HasMaxLength(50).IsRequired();
                b.Property(e => e.profile_picture_name).IsRequired();
                b.Ignore(e => e.Picture_File);
                //relation
                b.HasMany(e => e.music_Videos).WithMany(e => e.singers);
                b.HasMany(e => e.Albums).WithMany(e => e.singers);
                b.HasMany(e => e.is_main_singer).WithOne(e => e.main_singer);
            });
            #endregion
            #region Albums Config
            modelBuilder.Entity<Albums>(b =>
            {
                b.HasKey(e => e.AlbumId);
                b.Property(e => e.AlbumId).UseIdentityColumn().IsRequired();
                b.Property(e => e.AlbumName).HasMaxLength(50).IsRequired();
                //relatiom
               
            });
            #endregion
            #region Music Video Config
            modelBuilder.Entity<Music_Video>(b =>
            {
                b.HasKey(e => e.MVId);
                b.Property(e => e.MVId).UseIdentityColumn().IsRequired();
                b.Property(e => e.Mvname).HasMaxLength(60).IsRequired();
                b.Property(e => e.Mv_Describe).IsRequired();
                b.Property(e => e.File_Name).IsRequired();
                b.Property(e=>e.MvposterName).IsRequired();
                b.HasIndex(e => e.AdminId);
                b.Ignore(e => e.MVfile);
                b.Ignore(e => e.MvposterFile);
            });
            #endregion
            #region Remix Config
            modelBuilder.Entity<Remix>(b =>
            {
                b.HasKey(e => e.RemixId);
                b.Property(e => e.RemixId).UseIdentityColumn().IsRequired();
                b.Property(e => e.Remix_Creator).HasMaxLength(40);
                b.Property(e => e.File_Name).IsRequired().HasMaxLength(60);
                b.Ignore(e => e.Remixfile);
            });
            #endregion
            #region Contact Us
            modelBuilder.Entity<Contact_us>(b =>
            {
                b.HasKey(e => e.Request_Id);
                b.Property(e => e.Request_Id).UseIdentityColumn().IsRequired();
                b.Property(e => e.Request_Type).IsRequired();
                b.Property(e => e.Email_address).HasMaxLength(62);
                b.Property(e => e.Phone_Number).HasMaxLength(15);
                b.Property(e => e.Request_Text).IsRequired().HasMaxLength(500);
            });
            #endregion
            #region Comments Config
            modelBuilder.Entity<Comments>(b =>
            {
                b.HasKey(e => e.CommentId);
                b.Property(e => e.CommentId).UseIdentityColumn().IsRequired();
                b.Property(e=>e.comment_Name).IsRequired().HasMaxLength(200);
                b.Property(e => e.Email_Address).IsRequired();
                b.Property(e => e.Comment_Text).HasMaxLength(900).IsRequired();
                b.Property(e => e.Admin_Accepted).IsRequired();
                b.HasIndex(e => e.ParentId);
                b.HasIndex(e => e.MusicVideoId);
                b.HasIndex(e => e.SongId);
                b.HasIndex(e => e.RemixID);
                //relation itself   
                b.HasOne(e => e.Parent).WithMany(e => e.Child)
                .HasForeignKey(e => e.ParentId);
                //other relations
                b.HasOne(e => e.music).WithMany(e => e.comments)
                .HasForeignKey(e => e.SongId);
                b.HasOne(e => e.music_video).WithMany(e => e.comments)
                .HasForeignKey(e => e.MusicVideoId);
                b.HasOne(e => e.Remix).WithMany(e => e.comments)
                .HasForeignKey(e => e.RemixID);
            });
            #endregion
            base.OnModelCreating(modelBuilder);
        }
    }
}
