using Nancy;

namespace GrauhundReisen.WebPortal.Controller
{
    public class Admin : NancyModule
    {
        public Admin(GrauhundReisen.DomainFunktional.Booking.Service.T bookingService)
        {
            Get["show-all-my-events"] = _ =>
            {
                var events = bookingService.GetAllEventsAsString();

				return View["show-all-my-events", events];
            };
        }
    }
}