using System.Collections.Generic;
using GrauhundReisen.Domain.Services;
using Nancy;

namespace GrauhundReisen.WebPortal.Controller
{
    public class Admin : NancyModule
    {
        public Admin(BookingService bookingService)
        {
            Get["show-all-my-events"] = _ =>
            {
                var events = bookingService.GetAllEvents();

                return View["show-all-my-events", events];
            };
        }
    }
}