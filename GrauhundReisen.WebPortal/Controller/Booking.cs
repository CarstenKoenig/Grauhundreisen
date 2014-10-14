using System.Web.UI.WebControls;
using GrauhundReisen.Domain.Services;
using Nancy;
using GrauhundReisen.Contracts.Events;
using GrauhundReisen.ReadModel.EventHandler;
using GrauhundReisen.ReadModel.Repositories;
using System.Threading.Tasks;

namespace GrauhundReisen.WebPortal
{
	public class Booking : NancyModule
	{
	    readonly Bookings _bookings;
	    readonly BookingService _bookingService;

	    public Booking(Bookings bookings, BookingService bookingService)
		{
			_bookings = bookings;
            _bookingService = bookingService;

			Get ["change-booking/{id}"] = _ => GetBookingFormFor ((string)_.id);
			Post ["change-booking", true] = async(parameters,cancel) => await UpdateBooking ();
		}

		object GetBookingFormFor (string bookingId)
		{
			var booking = _bookings.GetBookingBy (bookingId);

			return View ["change-booking", booking];
		}

		async Task<object> UpdateBooking ()
		{
		    var bookingId = this.Request.Form["BookingId"].Value;
		    var email = this.Request.Form["TravellerEMail"].Value;
		    var creditCardNumber = this.Request.Form["PaymentCreditCardNumber"].Value;

		    await _bookingService.UpdateBookingDetails(bookingId, email, creditCardNumber);

			return View ["change-confirmation"];
		}
	}
}
