﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using C_bool.BLL.DAL.Entities;
using C_bool.BLL.Helpers;
using C_bool.BLL.Models.GooglePlaces;
using C_bool.BLL.Repositories;
using C_bool.BLL.Services;
using C_bool.WebApp.Config;
using C_bool.WebApp.Helpers;
using C_bool.WebApp.Models;
using C_bool.WebApp.Models.Place;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace C_bool.WebApp.Controllers
{
    public class NewPlaceController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        private readonly IRepository<Place> _placesRepository;
        private readonly IRepository<User> _usersRepository;
        private readonly IRepository<GameTask> _gameTasksRepository;

        private readonly IPlaceService _placesService;
        private readonly IGooglePlaceService _googlePlaceService;
        private readonly IUserService _usersService;

        private readonly IHttpClientFactory _clientFactory;
        
        private readonly GoogleAPISettings _googleApiSettings = new();
        private readonly GoogleApiAsync _googleApiAsync;

        public NewPlaceController(
            ILogger<HomeController> logger,
            IMapper mapper,
            IConfiguration configuration,
            IRepository<Place> placesRepository,
            IRepository<User> usersRepository,
            IRepository<GameTask> gameTasksRepository,
            IPlaceService placesService,
            IUserService userService,
            IGooglePlaceService googlePlaceService,
            IHttpClientFactory clientFactory
        )
        {
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _configuration.GetSection(GoogleAPISettings.Position).Bind(_googleApiSettings);
            _placesRepository = placesRepository;
            _usersRepository = usersRepository;
            _gameTasksRepository = gameTasksRepository;
            _placesService = placesService;
            _usersService = userService;
            _clientFactory = clientFactory;
            _googlePlaceService = googlePlaceService;
            _googleApiAsync = new GoogleApiAsync(clientFactory, _googleApiSettings);
        }

        [Authorize]
        public ActionResult Index()
        {
            var user = _usersService.GetCurrentUser();
            ViewBag.Latitude = user.Latitude;
            ViewBag.Longitude = user.Longitude;
            ViewBag.Message = "Nie udało się utworzyć miejsca";
            ViewBag.Status = true;

            var model = _googlePlaceService.GetGooglePlacesForUser();
            return View(model);
        }

        [Authorize]
        public ActionResult SearchNearby()
        {
            var user = _usersService.GetCurrentUser();
            var model = new NearbySearchRequest
            {

                Latitude = user.Latitude.ToString(CultureInfo.InvariantCulture),
                Longitude = user.Longitude.ToString(CultureInfo.InvariantCulture),

            };
            return View(model);
        }

        [Authorize]
        public ActionResult SearchByName()
        {
            var model = new NameSearchRequest();

            return View(model);
        }


        [Authorize]
        public ActionResult Create()
        {
            //var model = _mapper.Map<Place, PlaceEditModel>(_placesRepository.GetById(id));
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlaceEditModel model, IFormFile file)
        {
            var placeModel = _mapper.Map<PlaceEditModel, Place>(model);
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Message = "Nie udało się utworzyć miejsca";
                    ViewBag.Status = false;
                    return View(model);
                }

                placeModel.IsUserCreated = true;
                placeModel.Photo = (ImageConverter.ConvertImage(file));
                _placesRepository.Add(placeModel);
                ViewBag.Message = $"Dodano nowe miejsce: {placeModel.Name}";
                ViewBag.Status = true;
                return RedirectToAction("Index", "Places");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task AddToFavs([FromBody] ReturnString request)
        {
            var googlePlace = _googlePlaceService.GetGooglePlaceById(request.Id);
            var place = _mapper.Map<GooglePlace, Place>(googlePlace);
            place.Photo = await _googleApiAsync.DownloadImageAsync(googlePlace, "600");
            _usersService.AddFavPlace(place);

        }

        [Authorize]
        [HttpPost]
        public async Task AddToRepository([FromBody] ReturnString request)
        {
            var googlePlace = _googlePlaceService.GetGooglePlaceById(request.Id);
            var place = _mapper.Map<GooglePlace, Place>(googlePlace);
            place.Photo = await _googleApiAsync.DownloadImageAsync(googlePlace, "600");
            _placesService.AddPlace(place);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchNearby(NearbySearchRequest request)
        {
            _googlePlaceService.CreateNewOrUpdateExisting(await _googleApiAsync.GetNearby(request.Latitude, request.Longitude, request.Radius, type: request.SelectedType, keyword: request.Keyword, region: "PL", language: "pl", loadAllPages: _googleApiSettings.GetAllPages));

            var model = _googlePlaceService.GetGooglePlacesForUser();

            var user = _usersService.GetCurrentUser();
            ViewBag.Latitude = user.Latitude;
            ViewBag.Longitude = user.Longitude;

            ViewBag.Message = _googleApiAsync.Message;
            ViewBag.QueryStatus = _googleApiAsync.QueryStatus;
            return View("~/Views/NewPlace/Index.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchByNameAsync(NameSearchRequest request)
        {
            _googlePlaceService.CreateNewOrUpdateExisting(await _googleApiAsync.GetBySearchQuery(query: request.SearchPhrase, language: "pl"));

            var user = _usersService.GetCurrentUser();
            ViewBag.Latitude = user.Latitude;
            ViewBag.Longitude = user.Longitude;

            var model = _googlePlaceService.GetGooglePlacesForUser();
            ViewBag.Message = _googleApiAsync.Message;
            ViewBag.QueryStatus = _googleApiAsync.QueryStatus;
            return View("~/Views/NewPlace/Index.cshtml", model);
        }

    }

    public class ReturnString
    {
        public string Id { get; set; }
    }
}
