using System.Collections.Generic;
using Nancy;
using GrauhundReisen.DomainFunktional;

namespace GrauhundReisen.WebPortal.Controller
{
    public class Admin : NancyModule
    {
        public Admin(GrauhundReisen.DomainFunktional.Booking.Service.T bookingService)
        {
            Get["show-all-my-events"] = _ =>
            {
                var events = GrauhundReisen.DomainFunktional.Booking.Service.GetAllEventsAsString(bookingService);

				return View["show-all-my-events", events];
            };
        }
    }
}