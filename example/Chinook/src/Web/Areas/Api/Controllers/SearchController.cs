using System.Threading.Tasks;
using Chinook.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Chinook.Web.Areas.Api.Controllers {
    [Area("Api")]
    [Route("[Area]/[controller]")]
    public class SearchController : Controller {

        private readonly IGlobalSearchService _globalSearchService;

        public SearchController(IGlobalSearchService globalSearchService) {
            _globalSearchService = globalSearchService;

        }
        [HttpGet("[Action]/{q}")]
        public async Task<IActionResult> Artist(string q) {
            //Enum as camelcased strings as opposed to int value 
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(typeof(CamelCaseNamingStrategy)));

            return Json(await _globalSearchService.SearchArtist(q), jsonSerializerSettings);
        }
        
        [HttpGet("{searchString}")]
        public async Task<IActionResult> Index(string searchString) {
            //Enum as camelcased strings as opposed to int value 
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(typeof(CamelCaseNamingStrategy)));

            return Json(await _globalSearchService.Search(searchString), jsonSerializerSettings);
        }

    }
}

