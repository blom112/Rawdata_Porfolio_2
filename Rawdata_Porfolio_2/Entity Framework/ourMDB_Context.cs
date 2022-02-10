using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rawdata_Porfolio_2.Pages.Entity_Framework.Domain;
using System.IO;


namespace Rawdata_Porfolio_2.Entity_Framework
{
    public class OurMDB_Context : DbContext
    {
        public DbSet<Bookmarks_Personality> Bookmark_Personalities { get; set; }
        public DbSet<Bookmarks_Title> Bookmarks { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Personality> Personalities { get; set; }
        public DbSet<Personality_Profession> Personality_Professions { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Search_Queries> Search_Queries { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Title_Genre> Title_Genres { get; set; }
        public DbSet<Title_Localization> Title_Localizations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wi> Wis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();

           
            string connStringFromFile;

            using (StreamReader readtext = new StreamReader("C:/Login/Login.txt"))
            {
                connStringFromFile = readtext.ReadLine();
            }
            optionsBuilder.UseNpgsql(connStringFromFile);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
              
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseSerialColumns();
            modelBuilder.Entity<Bookmarks_Personality>().ToTable("bookmarks_personality");
            modelBuilder.Entity<Bookmarks_Personality>().Property(x => x.User_Id).HasColumnName("user_ID");
            modelBuilder.Entity<Bookmarks_Personality>().Property(x => x.Personality_Id).HasColumnName("personality_ID");
            modelBuilder.Entity<Bookmarks_Personality>().Property(x => x.Note).HasColumnName("note");
            modelBuilder.Entity<Bookmarks_Personality>().Property(x => x.Timestamp).HasColumnName("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            


            modelBuilder.Entity<Bookmarks_Title>().ToTable("bookmarks_title");
            modelBuilder.Entity<Bookmarks_Title>().Property(x => x.User_Id).HasColumnName("user_ID");
            modelBuilder.Entity<Bookmarks_Title>().Property(x => x.Title_Id).HasColumnName("title_ID");
            modelBuilder.Entity<Bookmarks_Title>().Property(x => x.Note).HasColumnName("note");
            modelBuilder.Entity<Bookmarks_Title>().Property(x => x.Timestamp).HasColumnName("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Character>().ToTable("characters");
            modelBuilder.Entity<Character>().Property(x => x.Personality_Id).HasColumnName("personality_ID");
            modelBuilder.Entity<Character>().Property(x => x.Title_Id).HasColumnName("title_ID");
            modelBuilder.Entity<Character>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<Character>().Property(x => x.CharacterOfPersonality).HasColumnName("character");
            modelBuilder.Entity<Character>().Property(x => x.Known_For).HasColumnName("known_for");
          

            modelBuilder.Entity<Episode>().ToTable("episode");
            modelBuilder.Entity<Episode>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<Episode>().Property(x => x.Parent_Id).HasColumnName("parent_ID");
            modelBuilder.Entity<Episode>().Property(x => x.Season).HasColumnName("season");
            modelBuilder.Entity<Episode>().Property(x => x.Ep_Number).HasColumnName("ep_number");


            modelBuilder.Entity<Personality>().ToTable("personality");
            modelBuilder.Entity<Personality>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<Personality>().Property(x => x.Name).HasColumnName("name");
            modelBuilder.Entity<Personality>().Property(x => x.Year_Birth).HasColumnName("year_birth");
            modelBuilder.Entity<Personality>().Property(x => x.Year_Death).HasColumnName("year_death");



            modelBuilder.Entity<Personality_Profession>().ToTable("personality_profession");
            modelBuilder.Entity<Personality_Profession>().Property(x => x.Personality_Id).HasColumnName("ID");
            modelBuilder.Entity<Personality_Profession>().Property(x => x.Profession).HasColumnName("profession");
            

            modelBuilder.Entity<Rating>().ToTable("ratings");
            modelBuilder.Entity<Rating>().Property(x => x.User_Id).HasColumnName("user_ID");
            modelBuilder.Entity<Rating>().Property(x => x.Title_Id).HasColumnName("title_ID");
            modelBuilder.Entity<Rating>().Property(x => x.RatingOfTitle).HasColumnName("rating");
            modelBuilder.Entity<Rating>().Property(x => x.Timestamp).HasColumnName("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<Role>().Property(x => x.Personality_Id).HasColumnName("personality_ID");
            modelBuilder.Entity<Role>().Property(x => x.Title_Id).HasColumnName("title_ID");
            modelBuilder.Entity<Role>().Property(x => x.RoleOfPersonality).HasColumnName("role");
            modelBuilder.Entity<Role>().HasKey(x => new { x.Title_Id, x.Personality_Id });

            modelBuilder.Entity<Search_Queries>().ToTable("search_queries");
            modelBuilder.Entity<Search_Queries>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<Search_Queries>().Property(x => x.Query).HasColumnName("query");
            modelBuilder.Entity<Search_Queries>().Property(x => x.Timestamp).HasColumnName("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Search_Queries>().Property(x => x.User_Id).HasColumnName("userID");
            

            modelBuilder.Entity<Title>().ToTable("title");
            modelBuilder.Entity<Title>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<Title>().Property(x => x.Type).HasColumnName("type");
            modelBuilder.Entity<Title>().Property(x => x.IsAdult).HasColumnName("isadult");
            modelBuilder.Entity<Title>().Property(x => x.Year_Start).HasColumnName("year_start");
            modelBuilder.Entity<Title>().Property(x => x.Year_End).HasColumnName("year_end");
            modelBuilder.Entity<Title>().Property(x => x.Runtime).HasColumnName("runtime");
            modelBuilder.Entity<Title>().Property(x => x.AvgRating).HasColumnName("avg_rating");
            modelBuilder.Entity<Title>().Property(x => x.Poster).HasColumnName("poster");
            modelBuilder.Entity<Title>().Property(x => x.Plot).HasColumnName("plot");
            modelBuilder.Entity<Title>().Property(x => x.Awards).HasColumnName("awards");


            modelBuilder.Entity<Title_Genre>().ToTable("title_genres");
            modelBuilder.Entity<Title_Genre>().Property(x => x.Title_Id).HasColumnName("title_ID");
            modelBuilder.Entity<Title_Genre>().Property(x => x.Genre).HasColumnName("genre");
           

            modelBuilder.Entity<Title_Localization>().ToTable("title_localization");
            modelBuilder.Entity<Title_Localization>().Property(x => x.Title_Id).HasColumnName("title_ID");
            modelBuilder.Entity<Title_Localization>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<Title_Localization>().Property(x => x.Name).HasColumnName("name");
            modelBuilder.Entity<Title_Localization>().Property(x => x.Language).HasColumnName("language");
            modelBuilder.Entity<Title_Localization>().Property(x => x.Region).HasColumnName("region");
            modelBuilder.Entity<Title_Localization>().Property(x => x.Type).HasColumnName("type");
            modelBuilder.Entity<Title_Localization>().Property(x => x.Attribute).HasColumnName("attribute");
            modelBuilder.Entity<Title_Localization>().Property(x => x.PrimaryTitle).HasColumnName("primary_title");
          

            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<User>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<User>().Property(x => x.Username).HasColumnName("username");
            modelBuilder.Entity<User>().Property(x => x.Password).HasColumnName("password");
            modelBuilder.Entity<User>().Property(x => x.Email).HasColumnName("email");
            modelBuilder.Entity<User>().Property(x => x.DateOfBirth).HasColumnName("dob");
            modelBuilder.Entity<User>().Property(x => x.Created).HasColumnName("created").HasDefaultValueSql("CURRENT_TIMESTAMP"); 
        //  modelBuilder.Entity<User>().Property(x => x.Is_Admin).HasColumnName("isAdmin"); if we have time to make admin roles :)
    




            modelBuilder.Entity<Wi>().ToTable("wi");
            modelBuilder.Entity<Wi>().Property(x => x.Title_Id).HasColumnName("title_ID");
            modelBuilder.Entity<Wi>().Property(x => x.Word).HasColumnName("word");
            modelBuilder.Entity<Wi>().Property(x => x.Field).HasColumnName("field");
            modelBuilder.Entity<Wi>().Property(x => x.Lexeme).HasColumnName("Lexeme");

        }

    }
   
}
