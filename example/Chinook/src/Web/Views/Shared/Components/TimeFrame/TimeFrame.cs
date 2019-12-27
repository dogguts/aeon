using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chinook.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.Web.Views.Shared.Components {
    public class TimeFrame : ViewComponent {
        private static readonly List<(int days, string name)> _timeframes = new List<(int, string)>(){
            (7,"1w"),
            (30,"1m"),
            (180,"6m"),
            (360,"1y"),
            (0 ,"all")
        };

        public TimeFrame() { }

        public IViewComponentResult Invoke(string OnClientClick) {
            return View((OnClientClick, TimeFrames: _timeframes));
        }
    }
}