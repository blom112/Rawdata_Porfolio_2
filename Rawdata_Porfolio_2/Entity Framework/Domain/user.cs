using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Rawdata_Porfolio_2.Pages.Entity_Framework.Domain
{
    public class User
    {
        [Key]
        public int? Id { get; set; }
        public string Username { get; set; } 
        public byte[] Password { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? Created { get; set; }
        public List<Bookmarks_Personality> Bookmarks_Personalities { get; set; }
        public List<Bookmarks_Title> Bookmarks_Titles { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<Search_Queries> Search_Queries { get; set; }

        public override string ToString()
        {
            return $"User ID = {Id}, Username = {Username}, Password = {Password}, Email = {Email}, Date of Birth {DateOfBirth}, Created {Created}";
        }

    }
}
