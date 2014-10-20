using System.Web.UI.WebControls;
using Nancy;
using System.Threading.Tasks;
using GrauhundReisen.DomainFunktional;

namespace GrauhundReisen.WebPortal
{
	public class Booking : NancyModule
	{
	    readonly ReadModelFunktional.Booking.ReadModel _bookings;
	    readonly DomainFunktional.Booking.Service.T _bookingService;

        public Booking(ReadModelFunktional.Booking.ReadModel bookings, DomainFunktional.Booking.Service.T bookingService)
		{
			_bookings = bookings;
            _bookingService = bookingService;

			Get ["change-booking/{id}"] = _ => GetBookingFormFor ((string)_.id);
			Post ["change-booking", true] = async(parameters,cancel) => await UpdateBooking ();
		}

		object GetBookingFormFor (string bookingId)
		{

			var booking = _bookings.Load(bookingId);

			return View ["change-booking", booking];
		}

		async Task<object> UpdateBooking ()
		{
		    var bookingId = this.Request.Form["BookingId"].Value;
		    var email = this.Request.Form["TravellerEMail"].Value;
		    var creditCardNumber = this.Request.Form["PaymentCreditCardNumber"].Value;

            await _bookingService.UpdateBookingDetail(
                    bookingId, 
                    email, creditCardNumber);

			return View ["change-confirmation"];
		}
	}
}
