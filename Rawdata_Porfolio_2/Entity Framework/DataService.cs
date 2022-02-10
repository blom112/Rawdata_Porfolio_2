using Rawdata_Porfolio_2.Pages.Entity_Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

//using System.Threading.Tasks;
using Npgsql;
using System.IO;
using Rawdata_Porfolio_2.Pages.Entity_Framework;

//using System.Reflection;
//using System.Text;
using Rawdata_Porfolio_2.Entity_Framework.Domain;
using System.Security.Cryptography;

namespace Rawdata_Porfolio_2.Entity_Framework   // There's a typo in "Portfolio" - is that an issue, or is that namespace referenced in other places under that name already, so should not be changed?
{
    public interface IDataService
    {
        ////////////////////////////////////////////////////////////
        //                        TITLES                          //
        ////////////////////////////////////////////////////////////

        List<Title> GetTitles();

        List<Title> GetNewTitles();
        
        List<Title> GetRandTitles();
        
        List<Title> GetTrTitles();
        
        List<Title> GetUserFavTitles(int userID);

        Title GetTitleById(long Id);

        public List<Character> GetKnownCharactersFromTitleById(long title_Id);

        public List<Character> GetCharactersFromTitleById(long title_Id);

        public List<Episode> GetEpisode(long titleID);

        public List<Episode> GetAllEpisodes(long titleID);

        public List<Title_Genre> GetTitleGenre(long title_Id);

        public List<Title_Localization> GetTitleLocalization(long title_id);

        ////////////////////////////////////////////////////////////
        //                      PERSONALITY                       //
        ////////////////////////////////////////////////////////////

        Personality GetPersonalityById(int personality_Id);

        public List<Personality_Profession> GetPersonalityProfession(int personality_Id);

        List<Character> GetCharactersFromPersonalityById(int personality_Id);

        public List<Character> GetKnownCharactersFromPersonalityById(int personality_Id);

        ////////////////////////////////////////////////////////////
        //                       BOOKMARKS                        //
        ////////////////////////////////////////////////////////////

        public void CreatePersonalityBM(int userID, int personalityID, string note);

        //  IList<Bookmarks_Personality> GetPersonalityBMsByUserID(int userID);

        // old
        public List<Bookmarks_Personality> GetPersonalityBMsByUserID(int userID);

        public void DeletePersonalityBM(int userID, int personalityID);

        public void UpdatePersonalityBM(int userID, int personalityID, string note);

        public void CreateTitleBM(int userID, long titleID, string note);

        public List<Bookmarks_Title> GetTitleBMsByUserID(int userID);

        public void DeleteTitleBM(int userID, long titleID);

        public void UpdateTitleBM(int userID, long titleID, string note);

        ////////////////////////////////////////////////////////////
        //                          USER                          //
        ////////////////////////////////////////////////////////////

        public User CreateUser(string username, byte[] password, string email, DateTime dob);

        User GetUser(int userID);

        User GetUserByName(string username);

        public void DeleteUser(int userID);

        public void UpdateUser(int userID, string email, string username, Byte[] password, DateTime? dob);

        public bool Login(string username, Byte[] password);

        ////////////////////////////////////////////////////////////
        //                         RATINGS                        //
        ////////////////////////////////////////////////////////////

        public List<Title> GetAvgRatingFromTitleId(long title_ID);

        public void CreateRating(int user_ID, long title_ID, short rating);

        List<Rating> GetRating(int userID);

        public void UpdateRating(int user_ID, long title_ID, short rating);

        public void DeleteRating(int user_ID, long title_ID);

        ////////////////////////////////////////////////////////////
        //                          SEARCH                        //
        ////////////////////////////////////////////////////////////

        List<Search_results> ActorSearch(int? user_Id, string query);

        public List<Search_Queries> GetSQ(int userID);

        public void DeleteSQ(int queryID);

        List<Search_results> StringSearch(int? userId, string query);

        List<Search_results> SS_Search(int? userid, string title_Query, string plot_Query, string character_Query, string name_Query);

        ////////////////////////////////////////////////////////////
        //                          OTHER                         //
        ////////////////////////////////////////////////////////////

        public List<Role> GetRolesFromTitleById(long title_Id);

        //  List<Role> GetRole(int titleID, int personalityID);

        ////////////////////////////////////////////////////////////
    }

    public class DataService : IDataService
    {
        // making our context so we can add to it all the time
        // instead of creating a new one in each method
        public OurMDB_Context ctx = new OurMDB_Context();

        public ConnString connection = new ConnString();

        public class ConnString
        {
            public NpgsqlConnection Connect()
            {
                string connStringFromFile;
                using (var readtext = new StreamReader("C:/Login/Login.txt"))
                {
                    connStringFromFile = readtext.ReadLine();
                }
                var connection = new NpgsqlConnection(connStringFromFile);
                connection.Open();
                return connection;
            }
        }

        // Password hashing functions adapted from Christos Matskas, https://cmatskas.com/-net-password-hashing-using-pbkdf2/
        private static byte[] HashPassword(byte[] plainPass)
        {
            var cryptoProvider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[24];
            cryptoProvider.GetBytes(salt);
            var hash = GetPbkdf2Bytes(plainPass, salt, 310000, 32); // Iteration count chosen based on recommendation by Open Web Application Security Project, https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
            return salt.Concat(hash).ToArray();
        }

        private static bool ValidatePassword(byte[] testPass, byte[] passBytes)
        {
            var salt = passBytes.Take(24).ToArray();
            var hash = passBytes.Skip(24).ToArray();
            var testHash = GetPbkdf2Bytes(testPass, salt, 310000, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] GetPbkdf2Bytes(byte[] password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(outputBytes);
        }

        ////////////////////////////////////////////////////////////
        //                        TITLES                          //
        ////////////////////////////////////////////////////////////

        public Title GetTitleById(long Id) // This SQL query should return the title details including its primary name, but I'm not sure how to implement it without breaking stuff in webservice: SELECT title.*, title_localization.name FROM public.title, public.title_localization WHERE title.\"ID\" = @TID AND title.\"ID\" = title_localization.\"title_ID\" AND title_localization.primary_title = true;
        {
            return ctx.Titles.Find(Id);
        }

        public List<Title> GetTitles()
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM title", connection.Connect()))
            {
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Title> result = new List<Title>();
                while (reader.Read())
                {
                    Title row = new Title()
                    {
                        Id = (long)reader["ID"],
                        Type = reader["type"].ToString(),
                        Name = reader["name"].ToString(),
                        IsAdult = (bool)reader["isadult"],
                        Year_Start = Convert.IsDBNull(reader["year_start"]) ? null : (short?) reader["year_start"],
                        Year_End = Convert.IsDBNull(reader["year_end"]) ? null : (short?) reader["year_end"],
                        Runtime = Convert.IsDBNull(reader["runtime"]) ? null : (int?) reader["runtime"],
                        AvgRating = Convert.IsDBNull(reader["avg_rating"]) ? null : (double?) reader["avg_rating"],
                        Poster = reader["poster"].ToString(),
                        Plot = reader["plot"].ToString(),
                        Awards = reader["awards"].ToString(),
                        Genres = reader["genres"].ToString()
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }

            //return ctx.Titles.ToList();
        }
        
        public List<Title> GetNewTitles()
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM select_new_titles();", connection.Connect()))
            {
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Title> result = new List<Title>();
                while (reader.Read())
                {
                    Title row = new Title()
                    {
                        Id = (long)reader["ID"],
                        Type = reader["type"].ToString(),
                        Name = reader["name"].ToString(),
                        IsAdult = (bool)reader["isadult"],
                        Year_Start = Convert.IsDBNull(reader["year_start"]) ? null : (short?) reader["year_start"],
                        Year_End = Convert.IsDBNull(reader["year_end"]) ? null : (short?) reader["year_end"],
                        Runtime = Convert.IsDBNull(reader["runtime"]) ? null : (int?) reader["runtime"],
                        AvgRating = Convert.IsDBNull(reader["avg_rating"]) ? null : (double?) reader["avg_rating"],
                        Poster = reader["poster"].ToString(),
                        Plot = reader["plot"].ToString(),
                        Awards = reader["awards"].ToString(),
                        Genres = reader["genres"].ToString()
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                Console.WriteLine(result);
                return result;
            }
        }
        
        public List<Title> GetTrTitles()
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM select_top_rated_titles();", connection.Connect()))
            {
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Title> result = new List<Title>();
                while (reader.Read())
                {
                    Title row = new Title()
                    {
                        Id = (long)reader["ID"],
                        Type = reader["type"].ToString(),
                        Name = reader["name"].ToString(),
                        IsAdult = (bool)reader["isadult"],
                        Year_Start = Convert.IsDBNull(reader["year_start"]) ? null : (short?) reader["year_start"],
                        Year_End = Convert.IsDBNull(reader["year_end"]) ? null : (short?) reader["year_end"],
                        Runtime = Convert.IsDBNull(reader["runtime"]) ? null : (int?) reader["runtime"],
                        AvgRating = Convert.IsDBNull(reader["avg_rating"]) ? null : (double?) reader["avg_rating"],
                        Poster = reader["poster"].ToString(),
                        Plot = reader["plot"].ToString(),
                        Awards = reader["awards"].ToString(),
                        Genres = reader["genres"].ToString()
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                Console.WriteLine(result);
                return result;
            }
        }
        
        public List<Title> GetRandTitles()
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM select_random_titles();", connection.Connect()))
            {
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Title> result = new List<Title>();
                while (reader.Read())
                {
                    Title row = new Title()
                    {
                        Id = (long)reader["ID"],
                        Type = reader["type"].ToString(),
                        Name = reader["name"].ToString(),
                        IsAdult = (bool)reader["isadult"],
                        Year_Start = Convert.IsDBNull(reader["year_start"]) ? null : (short?) reader["year_start"],
                        Year_End = Convert.IsDBNull(reader["year_end"]) ? null : (short?) reader["year_end"],
                        Runtime = Convert.IsDBNull(reader["runtime"]) ? null : (int?) reader["runtime"],
                        AvgRating = Convert.IsDBNull(reader["avg_rating"]) ? null : (double?) reader["avg_rating"],
                        Poster = reader["poster"].ToString(),
                        Plot = reader["plot"].ToString(),
                        Awards = reader["awards"].ToString(),
                        Genres = reader["genres"].ToString()
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                Console.WriteLine(result);
                return result;
            }
        }
        
        public List<Title> GetUserFavTitles(int userID)
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM select_fav_titles(@UID);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("UID", userID);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Title> result = new List<Title>();
                while (reader.Read())
                {
                    Title row = new Title()
                    {
                        Id = (long)reader["ID"],
                        Type = reader["type"].ToString(),
                        Name = reader["name"].ToString(),
                        IsAdult = (bool)reader["isadult"],
                        Year_Start = Convert.IsDBNull(reader["year_start"]) ? null : (short?) reader["year_start"],
                        Year_End = Convert.IsDBNull(reader["year_end"]) ? null : (short?) reader["year_end"],
                        Runtime = Convert.IsDBNull(reader["runtime"]) ? null : (int?) reader["runtime"],
                        AvgRating = Convert.IsDBNull(reader["avg_rating"]) ? null : (double?) reader["avg_rating"],
                        Poster = reader["poster"].ToString(),
                        Plot = reader["plot"].ToString(),
                        Awards = reader["awards"].ToString(),
                        Genres = reader["genres"].ToString()
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                Console.WriteLine(result);
                return result;
            }
        }

        public List<Character> GetCharactersFromTitleById(long title_Id)
        {
            using (var cmd = new NpgsqlCommand("SELECT DISTINCT characters.\"title_ID\", personality.\"ID\", personality.\"name\", characters.\"character\" FROM public.personality, public.characters " +
                "WHERE characters.\"title_ID\" = @TID AND characters.\"personality_ID\" = personality.\"ID\";", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("TID", title_Id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Character> result = new List<Character>();
                while (reader.Read())
                {
                    Character row = new Character()
                    {
                        Title_Id = (long)reader["title_ID"],
                        CharacterOfPersonality = reader["character"].ToString(),
                        Name = reader["name"].ToString(),
                        Personality_Id = (int)reader["ID"],
                        
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        public List<Character> GetKnownCharactersFromTitleById(long title_Id)
        {
            using (var cmd = new NpgsqlCommand("SELECT DISTINCT characters.\"title_ID\", personality.\"ID\", personality.\"name\", characters.\"character\", characters.known_for FROM public.personality, public.characters " +
                "WHERE characters.\"title_ID\" = @TID AND characters.known_for = true AND characters.\"personality_ID\" = personality.\"ID\";", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("TID", title_Id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Character> result = new List<Character>();
                while (reader.Read())
                {
                    Character row = new Character()
                    {
                        Title_Id = (long)reader["title_ID"],
                        CharacterOfPersonality = reader["character"].ToString(),
                        Name = reader["name"].ToString(),
                        Personality_Id = (int)reader["ID"],
                        Known_For = (bool)reader["known_for"],
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        public List<Episode> GetEpisode(long titleID)
        {
            using (var cmd = new NpgsqlCommand("SELECT episode.\"parent_ID\", ep_number, season FROM public.episode WHERE episode.\"ID\" = @TID;", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("TID", titleID);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Episode> result = new List<Episode>();
                while (reader.Read())
                {
                    Episode row = new Episode()
                    {
                        Parent_Id = (long)reader["parent_ID"],
                        Ep_Number = (short)reader["ep_number"],
                        Season = (short)reader["season"],
                    };
                    result.Add(row);
                }
                return result;
            }
        }

        public List<Episode> GetAllEpisodes(long titleID)
        {
            using (var cmd = new NpgsqlCommand("SELECT Episode.\"parent_ID\", title_localization.name, episode.\"ID\", episode.ep_number, episode.season FROM public.title_localization, public.episode " +
                                               "WHERE episode.\"parent_ID\" = @TID AND title_localization.\"title_ID\" = episode.\"ID\" AND title_localization.primary_title = true;", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("TID", titleID);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Episode> result = new List<Episode>();
                while (reader.Read())
                {
                    Episode row = new Episode()
                    {
                        Parent_Id = (long)reader["parent_ID"],
                        Name = reader["name"].ToString(),
                        Ep_Number = (short)reader["ep_number"],
                        Season = (short)reader["season"],
                        Id = (int)reader["ID"],
                    };
                    result.Add(row);
                }
                return result;
            }
        }

        public List<Title_Genre> GetTitleGenre(long title_id)
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM public.title_genres WHERE \"title_ID\" = @TID;", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("TID", title_id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Title_Genre> result = new List<Title_Genre>();
                while (reader.Read())
                {
                    Title_Genre row = new Title_Genre()
                    {
                        Title_Id = (long)reader["title_ID"],
                        Genre = reader["genre"].ToString(),
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        public List<Title_Localization> GetTitleLocalization(long title_id)
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM title_localization WHERE @TID = \"title_ID\";", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("TID", title_id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Title_Localization> result = new List<Title_Localization>();
                while (reader.Read())
                {
                    Title_Localization row = new Title_Localization()
                    {
                        Title_Id = (long)reader["title_ID"],
                        Id = (long)reader["ID"],
                        Name = reader["name"].ToString(),
                        Language = reader["langauge"].ToString(),
                        Region = reader["region"].ToString(),
                        Type = reader["type"].ToString(),
                        Attribute = reader["attribute"].ToString(),
                        PrimaryTitle = (bool)reader["primary_title"],
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        ////////////////////////////////////////////////////////////
        //                      PERSONALITY                       //
        ////////////////////////////////////////////////////////////

        public Personality GetPersonalityById(int personality_Id)
        {
            return ctx.Personalities.Find(personality_Id);
        }

        public List<Character> GetCharactersFromPersonalityById(int personality_Id)
        {
            using (var cmd = new NpgsqlCommand("SELECT DISTINCT characters.\"personality_ID\", characters.\"title_ID\", characters.character, title_localization.name FROM public.characters, public.title_localization " +
                                               "WHERE characters.\"personality_ID\" = @PID AND title_localization.\"title_ID\" = characters.\"title_ID\";", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("PID", personality_Id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Character> result = new List<Character>();
                while (reader.Read())
                {
                    Character row = new Character()
                    {
                        Personality_Id = (int)reader["personality_ID"],
                        CharacterOfPersonality = reader["character"].ToString(),
                        Title_Id = (long)reader["title_ID"],
                        Name = reader["name"].ToString(),
                       
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        public List<Character> GetKnownCharactersFromPersonalityById(int personality_Id)
        {
            using (var cmd = new NpgsqlCommand("SELECT DISTINCT characters.\"title_ID\", characters.character, title_localization.name FROM public.characters, public.title_localization " +
                                               "WHERE characters.\"personality_ID\" = @PID AND characters.known_for = true AND title_localization.\"title_ID\" = characters.\"title_ID\";", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("PID", personality_Id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Character> result = new List<Character>();
                while (reader.Read())
                {
                    Character row = new Character()
                    {
                        CharacterOfPersonality = reader["character"].ToString(),
                        Title_Id = (long)reader["title_ID"],
                        Name = reader["name"].ToString(),
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        public List<Personality_Profession> GetPersonalityProfession(int personality_Id)
        {
            using (var cmd = new NpgsqlCommand("SELECT profession,  personality_professions.\"personality_ID\" FROM public.personality_professions WHERE \"personality_ID\" = @PID;", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("PID", personality_Id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Personality_Profession> result = new List<Personality_Profession>();
                while (reader.Read())
                {
                    Personality_Profession row = new Personality_Profession()
                    {
                        Personality_Id = (int)reader["personality_ID"],
                        Profession = reader["profession"].ToString(),
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        ////////////////////////////////////////////////////////////
        //                       BOOKMARKS                        //
        ////////////////////////////////////////////////////////////

        public void CreatePersonalityBM(int userID, int personalityID, string note)
        {
            using (var cmd = new NpgsqlCommand("CALL update_bookmark('n', 'p', @ID, @PID, @note);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", userID);
                cmd.Parameters.AddWithValue("PID", personalityID);
                cmd.Parameters.AddWithValue("note", note);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
        }

        //public IList<Bookmarks_Personality> GetPersonalityBMsByUserID(int userID)
        //{
        //    IList<Bookmarks_Personality> allPersonalityBMs = ctx.Bookmark_Personalities.ToList();

        //    return allPersonalityBMs.Where(x => x.User_Id == userID).ToList();
        //}

        // we try a new method - this is an old version

        public List<Bookmarks_Personality> GetPersonalityBMsByUserID(int userID)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM select_user_bookmarks('p', @ID);", connection.Connect());

            cmd.Parameters.AddWithValue("ID", userID);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Bookmarks_Personality> result = new List<Bookmarks_Personality>();
            while (reader.Read())
            {
                Bookmarks_Personality row = new Bookmarks_Personality()
                {
                    User_Id = (int)reader["user_ID"],
                    Name = reader["name"].ToString(),
                    Note = reader["note"].ToString(),
                    Timestamp = (DateTime)reader["timestamp"],
                    Personality_Id = (int)reader["ID"]
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        public void DeletePersonalityBM(int UserId, int PersonalityId)
        {
            using (var cmd = new NpgsqlCommand("CALL update_bookmark('d','p', @ID, @PID);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", UserId);
                cmd.Parameters.AddWithValue("PID", PersonalityId);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
        }

        public void UpdatePersonalityBM(int UserId, int PersonalityId, string note)
        {
            using (var cmd = new NpgsqlCommand("CALL update_bookmark('u','p', @ID, @PID, @NOTE);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", UserId);
                cmd.Parameters.AddWithValue("PID", PersonalityId);
                cmd.Parameters.AddWithValue("NOTE", note);

                NpgsqlDataReader result = cmd.ExecuteReader();
            }
        }

        public void CreateTitleBM(int userID, long titleID, string note)
        {
            using (var cmd = new NpgsqlCommand("CALL update_bookmark('n', 't', @ID, @PID, @note);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", userID);
                cmd.Parameters.AddWithValue("PID", titleID);
                cmd.Parameters.AddWithValue("note", note);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
        }

        public List<Bookmarks_Title> GetTitleBMsByUserID(int userID)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM select_user_bookmarks('t', @ID);", connection.Connect());
            cmd.Parameters.AddWithValue("ID", userID);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Bookmarks_Title> result = new List<Bookmarks_Title>();
            while (reader.Read())
            {
                Bookmarks_Title row = new Bookmarks_Title()
                {
                    User_Id = (int)reader["user_ID"],
                    Name = reader["name"].ToString(),
                    Note = reader["note"].ToString(),
                    Timestamp = (DateTime)reader["timestamp"],
                    Title_Id = (long)reader["title_ID"]
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        public void DeleteTitleBM(int userID, long titleID)
        {
            using (var cmd = new NpgsqlCommand("CALL update_bookmark('d','t', @ID, @PID);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", userID);
                cmd.Parameters.AddWithValue("PID", titleID);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
        }

        public void UpdateTitleBM(int userID, long titleID, string note)
        {
            using (var cmd = new NpgsqlCommand("CALL update_bookmark('u','t', @ID, @TID, @NOTE);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", userID);
                cmd.Parameters.AddWithValue("TID", titleID);
                cmd.Parameters.AddWithValue("NOTE", note);

                NpgsqlDataReader result = cmd.ExecuteReader();
            }
        }

        ////////////////////////////////////////////////////////////
        //                          USER                          //
        ////////////////////////////////////////////////////////////

        public User CreateUser(string username, byte[] password, string email, DateTime dob)
        {
            var user = new User
            {
                Username = username,
                Password = HashPassword(password),
                Email = email,
                DateOfBirth = dob
            };
            ctx.Add(user);
            ctx.SaveChanges();
            return user;
        }

        public User GetUser(int userId)
        {
            return ctx.Users.Find(userId);
        }
        
        public User GetUserByName(string username)
        {
            return ctx.Users.SingleOrDefault(user => user.Username == username);
        }

        public void UpdateUser(int userID, string email, string username, byte[] password, DateTime? dob)
        {
            using (var cmd = new NpgsqlCommand("CALL update_user('u', @ID, @USERNAME, @PASSWORD, @MAIL, @DOB);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", userID);
                cmd.Parameters.AddWithValue("USERNAME", username);
                cmd.Parameters.AddWithValue("PASSWORD", password.Length == 0 ? password : HashPassword(password));
                cmd.Parameters.AddWithValue("MAIL", email);
                if (dob.HasValue)
                {
                    cmd.Parameters.AddWithValue("DOB", dob);
                }
                else
                {
                    cmd.Parameters.AddWithValue("DOB", DBNull.Value);
                }
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
            connection.Connect().Close();
        }

        public void DeleteUser(int userId)
        {
            using (var cmd = new NpgsqlCommand("CALL update_user('d', @ID);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", userId);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
            connection.Connect().Close();
        }

        public bool Login(string username, byte[] password)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT password FROM public.user WHERE @USER = username", connection.Connect()))
                {
                    cmd.Parameters.AddWithValue("USER", username);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        User user = new User()
                        {
                            Password = (byte[])reader["password"],
                        };

                        return ValidatePassword(password, user.Password);
                    }
                    return false;
                }
            }
            catch (NpgsqlException e)
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////
        //                         RATINGS                        //
        ////////////////////////////////////////////////////////////

        public void CreateRating(int user_ID, long title_ID, short rating)
        {
            using (var cmd = new NpgsqlCommand("CALL update_rating(@UID, @TID, @RATING, @DEL);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("UID", user_ID);
                cmd.Parameters.AddWithValue("TID", title_ID);
                cmd.Parameters.AddWithValue("RATING", rating);
                cmd.Parameters.AddWithValue("DEL", false);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
            connection.Connect().Close();
        }

        public List<Rating> GetRating(int userID)
        {
            var cmd = new NpgsqlCommand("SELECT ratings.rating, ratings.timestamp, ratings.\"user_ID\", ratings.\"title_ID\" FROM ratings WHERE \"user_ID\" = @UID;", connection.Connect());
            cmd.Parameters.AddWithValue("UID", userID);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Rating> result = new List<Rating>();
            while (reader.Read())
            {
                Rating row = new Rating()
                {
                    User_Id = (int)reader["user_ID"],
                    Title_Id = (long)reader["title_ID"],
                    RatingOfTitle = (short)reader["rating"],
                    Timestamp = (DateTime)reader["timestamp"],
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        public void UpdateRating(int user_ID, long title_ID, short rating)
        {
            using (var cmd = new NpgsqlCommand("CALL update_rating(@UID, @TID, @RATING, @DEL);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("UID", user_ID);
                cmd.Parameters.AddWithValue("TID", title_ID);
                cmd.Parameters.AddWithValue("RATING", rating);
                cmd.Parameters.AddWithValue("DEL", false);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
            connection.Connect().Close();
        }

        public void DeleteRating(int user_ID, long title_ID)
        {
            using (var cmd = new NpgsqlCommand("CALL update_rating(@UID, @TID, @RATING, @DEL);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("UID", user_ID);
                cmd.Parameters.AddWithValue("TID", title_ID);
                cmd.Parameters.AddWithValue("RATING", DBNull.Value);
                cmd.Parameters.AddWithValue("DEL", true);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
            connection.Connect().Close();
        }

        public List<Title> GetAvgRatingFromTitleId(long title_ID)
        {
            var cmd = new NpgsqlCommand("SELECT avg_rating, \"ID\" FROM title WHERE \"ID\" = @TID;", connection.Connect());
            cmd.Parameters.AddWithValue("TID", title_ID);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Title> result = new List<Title>();
            while (reader.Read())
            {
                Title row = new Title()
                {
                    Id = (long)reader["ID"],
                    AvgRating = (double)reader["avg_rating"],
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        ////////////////////////////////////////////////////////////
        //                          SEARCH                        //
        ////////////////////////////////////////////////////////////
        public List<Search_Queries> GetSQ(int userID)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM search_queries WHERE \"user_ID\" = @UID ORDER BY timestamp DESC;", connection.Connect());
            cmd.Parameters.AddWithValue("UID", userID);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Search_Queries> result = new List<Search_Queries>();
            while (reader.Read())
            {
                Search_Queries row = new Search_Queries()
                {
                    User_Id = (int)reader["user_ID"],
                    Query = reader["query"].ToString(), // we have to do some separating the string up some time, should we do it now?
                    Timestamp = (DateTime)reader["timestamp"],
                    Id = (int)reader["ID"]
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        //TEST THIS
        // this dont seem to make sense - from sigita and mads, where is the userid, we need a userid to know from who the
        // search should be deleted, which it doesnt take
        // 25/11: Should be resolved - you use the query ID to identify which query is to be deleted, which is now returned by the GetSQ method
        public void DeleteSQ(int queryID)
        {
            using (var cmd = new NpgsqlCommand("CALL update_search_queries(@UID, @QID, @QUERY, @DEL);", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("UID", DBNull.Value);
                cmd.Parameters.AddWithValue("QID", queryID);
                cmd.Parameters.AddWithValue("QUERY", DBNull.Value);
                cmd.Parameters.AddWithValue("DEL", true);
                NpgsqlDataReader result = cmd.ExecuteReader();
            }
            connection.Connect().Close();
        }

        public List<Search_results> ActorSearch(int? user_Id, string query)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM actor_search(@UID, @QUERY);", connection.Connect());
            if (user_Id.HasValue)
            {
                cmd.Parameters.AddWithValue("UID", user_Id);
            }
            else
            {
                cmd.Parameters.AddWithValue("UID", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("QUERY", query);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Search_results> result = new List<Search_results>();
            while (reader.Read())
            {
                Search_results row = new Search_results()
                {
                    User_Id = (int)reader["user_ID"],
                    Personality_ID = (int)reader["personality_ID"],
                    Character_Name = reader["personality_name"].ToString(),
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        public List<Search_results> StringSearch(int? userId, string query)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM string_search(@UID, @QUERY);", connection.Connect());
            if (userId.HasValue)
            {
                cmd.Parameters.AddWithValue("UID", userId);
            }
            else
            {
                cmd.Parameters.AddWithValue("UID", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("QUERY", query);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Search_results> result = new List<Search_results>();
            while (reader.Read())
            {
                Search_results row = new Search_results()
                {
                    User_Id = (int)reader["user_ID"],
                    Title_ID = (long)reader["title_ID"],
                    Title_Name = reader["title_name"].ToString(),
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        public List<Search_results> SS_Search(int? userid, string title_Query, string plot_Query, string character_Query, string name_Query)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM structured_string_search(@UID, @TQUERY, @PQUERY, @CQUERY, @NQUERY)", connection.Connect());
            if (userid.HasValue)
            {
                cmd.Parameters.AddWithValue("UID", userid);
            }
            else
            {
                cmd.Parameters.AddWithValue("UID", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("TQUERY", title_Query);
            cmd.Parameters.AddWithValue("PQUERY", plot_Query);
            cmd.Parameters.AddWithValue("CQUERY", character_Query);
            cmd.Parameters.AddWithValue("NQUERY", name_Query);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Search_results> result = new List<Search_results>();
            while (reader.Read())
            {
                Search_results row = new Search_results()
                {
                    User_Id = (int)reader["user_ID"],
                    Title_ID = (long)reader["title_ID"],
                    Title_Name = reader["title_name"].ToString(),
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }

        ////////////////////////////////////////////////////////////
        //                          OTHER                         //
        ////////////////////////////////////////////////////////////

        public List<Role> GetRolesFromTitleById(long title_Id)
        {
            using (var cmd = new NpgsqlCommand("SELECT DISTINCT roles.\"title_ID\", personality.\"ID\", personality.\"name\", roles.\"role\" FROM public.title_localization, public.roles " +
                                               "WHERE roles.\"title_ID\" = @TID AND title_localization.primary_title = true AND roles.\"personality_ID\" = personality.\"ID\";", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("TID", title_Id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Role> result = new List<Role>();
                while (reader.Read())
                {
                    Role row = new Role()
                    {
                        Title_Id = (long)reader["title_ID"],
                        RoleOfPersonality = reader["role"].ToString(),
                        Name = reader["name"].ToString(),
                        Personality_Id = (int)reader["ID"],
                    };
                    result.Add(row);
                }
                connection.Connect().Close();
                return result;
            }
        }

        /*
        Personality ReadPersonality(int userID)
        {
            using (var cmd = new NpgsqlCommand("Select username, password, email, dob, created FROM user WHERE \"ID\" = @ID;", connection.Connect()))
            {
                cmd.Parameters.AddWithValue("ID", userID);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Personality> personalities = new List<Personality>();
                while (reader.Read())
                {
                    Personality Row = new Personality()
                    {
                        Username = reader.GetString["username"],
                        passwor
                    }
                }
            }
        }
        */
        /*
           public List<Role> GetRole(int titleID, int personalityID)
        {
            var cmd = new NpgsqlCommand("select role from roles where \"title_ID\" = @TID AND \"personality_ID\" = @PID;", connection.Connect());
            cmd.Parameters.AddWithValue("TID", titleID);
            cmd.Parameters.AddWithValue("PID", personalityID);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Role> result = new List<Role>();
            while (reader.Read())
            {
                Role row = new Role()
                {
                    RoleOfPersonality = reader["role"].ToString(),
                };
                result.Add(row);
            }
            connection.Connect().Close();
            return result;
        }*/

        ////////////////////////////////////////////////////////////
    }
}