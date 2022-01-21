﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using C_bool.WebApp.Models.Place;
using C_bool.WebApp.Models.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace C_bool.WebApp.ViewComponents
{
    public class MapView : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<PlaceViewModel> placesList, double range,
            bool noBounds = false)
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new PlaceProfile()); });
            var mapper = new Mapper(config);

            var model = mapper.Map<List<PlaceMapModel>>(placesList);

            var zoom = range switch
            {
                < 1000 => 16,
                > 1000 and <= 5000 => 13,
                > 5000 and <= 10000 => 13,
                > 10000 and <= 18000 => 12,
                > 18000 and <= 30000 => 11,
                > 30000 and <= 100000 => 10,
                > 100000 and <= 200000 => 9,
                > 200000 and <= 400000 => 8,
                > 400000 and <= 800000 => 7,
                > 800000 and <= 1600000 => 5,
                > 1600000 => 3,
                _ => 15
            };

            ViewBag.MapZoom = zoom;
            ViewBag.NoBounds = noBounds;

            return View("MapView", model);
        }
    }
}

