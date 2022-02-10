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
    [Route("api/title")]
    public class TitleController : Controller
    {
        private IDataService _dataService;
        private LinkGenerator _linkGenerator;

        public TitleController(IDataService dataService, LinkGenerator linkGenerator)
        {
            _dataService = dataService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet("{id}", Name = nameof(GetTitleById))]
        public IActionResult GetTitleById(long id)
        {
            var title = _dataService.GetTitleById(id);

            if (title == null)
            {
                return NotFound();
            }

            return Ok(GetTitleViewModel(title));
        }

        [HttpGet("allTitles")]
        public IActionResult GetTitles()
        {
            var titles = _dataService.GetTitles();

            return Ok(titles.Select(x => GetTitleViewModel(x)));
        }
        
        [HttpGet("newTitles")]
        public IActionResult GetNewTitles()
        {
            var titles = _dataService.GetNewTitles();

            if (titles == null)
            {
                return NotFound();
            }    
            
            return Ok(titles.Select(x => GetTitleViewModel(x)));
        }
        
        [HttpGet("favTitles")]
        public IActionResult GetFavTitles(int uid)
        {
            var titles = _dataService.GetUserFavTitles(uid);

            if (titles == null)
            {
                return NotFound();
            }    
            
            return Ok(titles.Select(x => GetTitleViewModel(x)));
        }
        
        [HttpGet("trTitles")]
        public IActionResult GetTrTitles()
        {
            var titles = _dataService.GetTrTitles();

            if (titles == null)
            {
                return NotFound();
            }    
            
            return Ok(titles.Select(x => GetTitleViewModel(x)));
        }
        
        [HttpGet("randTitles")]
        public IActionResult GetRandTitles()
        {
            var titles = _dataService.GetRandTitles();

            if (titles == null)
            {
                return NotFound();
            }    
            
            return Ok(titles.Select(x => GetTitleViewModel(x)));
        }

        [HttpGet("{id}/CharactersFromTitle")]
        public IActionResult GetCharactersFromTitle(long id)
        {
            List<Character> characters = _dataService.GetCharactersFromTitleById(id);

            if (characters == null)
            {
                return NotFound();
            }

            return Ok(characters);
        }

        private object CreateResultModel(QueryString queryString, int total, IEnumerable<TitleViewModel> model)
        {
            return new
            {
                total,
                prev = CreateNextPageLink(queryString),
                cur = CreateCurrentPageLink(queryString),
                next = CreateNextPageLink(queryString, total),
                items = model
            };
        }

        private string CreateNextPageLink(QueryString queryString, int total)
        {
            var lastPage = GetLastPage(queryString.PageSize, total);
            return queryString.Page >= lastPage ? null : GetTitlesUrl(queryString.Page + 1, queryString.PageSize, queryString.OrderBy);
        }

        private string CreateCurrentPageLink(QueryString queryString)
        {
            return GetTitlesUrl(queryString.Page, queryString.PageSize, queryString.OrderBy);
        }

        private string CreateNextPageLink(QueryString queryString)
        {
            return queryString.Page <= 0 ? null : GetTitlesUrl(queryString.Page - 1, queryString.PageSize, queryString.OrderBy);
        }

        private static int GetLastPage(int pageSize, int total)
        {
            return (int)Math.Ceiling(total / (double)pageSize) - 1;
        }

        private string GetTitlesUrl(int page, int pageSize, string orderBy)
        {
            return _linkGenerator.GetUriByName(
                HttpContext,
                nameof(GetTitles),
                new { page, pageSize, orderBy });
        }

        private TitleViewModel GetTitleViewModel(Title title)
        {
            Console.WriteLine("heyo1");

            //String titleGenresFromTable = string.Join(",", title.Title_Genres);
            Console.WriteLine("heyo");
            return new TitleViewModel
            {
                Url = _linkGenerator.GetUriByName(HttpContext, nameof(GetTitleById), new { title.Id }).Replace("%20", ""),
                Type = title.Type,
                Name = title.Name,

                Genres = title.Genres,
                //  Primary_Title = title.Primary_Title,
                Is_Adult = title.IsAdult,
                Year_Start = title.Year_Start,
                Year_End = title.Year_End,
                Runtime = title.Runtime,
                //  Avg_Rating = title.AvgRating,
                Poster = title.Poster,
                Plot = title.Plot,
                Awards = title.Awards,
                // Language = title.Language,
                //   Region = title.Region,
            };
        }
    }
}