using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rawdata_Porfolio_2;
using Rawdata_Porfolio_2.Entity_Framework;
using Rawdata_Porfolio_2.Pages.Entity_Framework.Domain;
using Microsoft.AspNetCore.Routing;
using Webservice.ViewModels;

namespace Webservice.Controllers
{
    [ApiController]
    [Route("api/user/bookmarks")] //??? dont remember
    public class BookmarkController : Controller
    {
        private IDataService _dataService;
        private LinkGenerator _linkGenerator;

        public BookmarkController(IDataService dataService, LinkGenerator linkGenerator)
        {
            _dataService = dataService;
            _linkGenerator = linkGenerator;
        }

        ////////////////////////////////////////////////////////////
        //                      TitleBookmarks                    //
        ////////////////////////////////////////////////////////////

        [HttpGet("titles")]
        public IActionResult GetBookmarkTitlesForUser(int userID)
        {
            var titleBMs = _dataService.GetTitleBMsByUserID(userID);

            if (titleBMs.Count == 0)
            {
                return NotFound("no BMs found");
            }
            //  .Select(x => GetBookmarkTitleViewModel(x))
            return Ok(titleBMs.Select(x => GetBookmarkTitleViewModel(x)));
        }

        [HttpPost("createTitleBookmark")]
        public IActionResult CreateBookmarkTitle(int userID, long titleID, string note)
        {
            // checks
            if (_dataService.GetUser(userID).Username == null)
            {
                return NotFound("No user found");
            }

            if (_dataService.GetTitleById(titleID) == null)
            {
                return NotFound("No title Found");
            }

            _dataService.CreateTitleBM(userID, titleID, note);

            //  ending may need change, this is just something i tried
            // Bookmarks_Title bookmark = new Bookmarks_Title();

            // I dont know what else to put here, but it works and adds to database
            return Created("", null);
        }

        private BookmarkTitleViewModel GetBookmarkTitleViewModel(Bookmarks_Title bookmarkTitle)
        {
            return new BookmarkTitleViewModel
            {
                // Url = _linkGenerator.GetUriByName(HttpContext, nameof(GetBookmarkTitlesForUser), new { bookmarkTitle.Title_Id }).Replace("%20", ""),
                UserID = bookmarkTitle.User_Id,
                TitleName = bookmarkTitle.Name,
                TitleNote = bookmarkTitle.Note
            };
        }

        ////////////////////////////////////////////////////////////
        //                  PersonalityBookmarks                  //
        ////////////////////////////////////////////////////////////

        [HttpGet("personalities")]
        public IActionResult GetBookmarkPersonalitiesForUser(int userID)
        {
            var personalityBMs = _dataService.GetPersonalityBMsByUserID(userID);

            if (personalityBMs.Count == 0)
            {
                return NotFound("No personalityBMs found");
                // return Ok(personalityBMs.Select(x => GetBookmarkPersonalityViewModel(x)));
            }

            // .Select(x => GetBookmarkPersonalityViewModel(x))
            return Ok(personalityBMs);
        }

        [HttpPost("createPersonalityBookmark")]
        public IActionResult CreateBookmarkPersonality(int userID, int personalityID, string note)
        {
            // checks
            if (_dataService.GetUser(userID).Username == null)
            {
                return NotFound("No user found");
            }

            if (_dataService.GetPersonalityById(personalityID) == null)
            {
                return NotFound("No title Found");
            }

            _dataService.CreatePersonalityBM(userID, personalityID, note);

            //  ending may need change, this is just something i tried
            // Bookmarks_Title bookmark = new Bookmarks_Title();

            // I dont know what else to put here, but it works and adds to database
            // Bookmarks_Personality x = new Bookmarks_Personality();

            return Created("", null);
        }

        private BookmarkPersonalityViewModel GetBookmarkPersonalityViewModel(Bookmarks_Personality bookmarkPersonality)
        {
            return new BookmarkPersonalityViewModel
            {
                Url = _linkGenerator.GetUriByName(HttpContext, nameof(GetBookmarkTitlesForUser), new { bookmarkPersonality.Personality_Id }),
                UserID = bookmarkPersonality.User_Id,
                PersonalityName = bookmarkPersonality.Name
            };
        }

        ////////////////////////////////////////////////////////////

        //[HttpGet("{id}", Name = nameof(GetUser))]
        //public IActionResult GetBookmarks()
        //{
        //    var bookmark = _dataService.GetBookmarks(id);

        //    if (bookmark == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(GetBookmarkViewModel(bookmarks));
        //}

        //private BookmarkViewModel GetPersonalityViewModel(Bookmarks personality)
        //{
        //    return new PersonalityViewModel
        //    {
        //        Url = (_linkGenerator.GetUriByName(HttpContext, nameof(GetPersonality), new { personality.Id })).Replace("%20", ""),
        //        Name = personality.Name,
        //        Year_Birth = personality.Year_Birth,
        //        Year_Death = personality.Year_Death,
        //        // Profession = personality.Profession,
        //    };
        //}
    }
}