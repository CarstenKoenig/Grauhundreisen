using Nancy;
using GrauhundReisen.Contracts.Events;
using GrauhundReisen.ReadModel.EventHandler;
using GrauhundReisen.ReadModel.Repositories;
using System.Threading.Tasks;

namespace GrauhundReisen.WebPortal
{
	public class Booking : NancyModule
	{
		Bookings _bookings;
		readonly BookingHandler _bookingHandler;

		public Booking (Bookings bookings, BookingHandler bookingHandler)
		{
			_bookings = bookings;
			_bookingHandler = bookingHandler;

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
			var bookingUpdated = new BookingUpdated { 
				BookingId = this.Request.Form ["BookingId"].Value,
				Email = this.Request.Form ["TravellerEMail"].Value,
				CreditCardNumber = this.Request.Form ["PaymentCreditCardNumber"].Value
			};

			await _bookingHandler.Handle (bookingUpdated);

			return View ["change-confirmation"];
		}
	}
}
