using Nancy;
using GrauhundReisen.Contracts.Events;
using GrauhundReisen.ReadModel.EventHandler;
using GrauhundReisen.ReadModel.Repositories;

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
			Post ["change-booking"] = _ => UpdateBooking ();
		}

		object GetBookingFormFor (string bookingId)
		{
			var booking = _bookings.GetBookingBy (bookingId);

			return View ["change-booking", booking];
		}

		object UpdateBooking ()
		{
			var bookingUpdated = new BookingUpdated { 
				BookingId = this.Request.Form ["BookingId"].Value,
				Email = this.Request.Form ["TravellerEMail"].Value,
				CreditCardNumber = this.Request.Form ["PaymentCreditCardNumber"].Value
			};

			_bookingHandler.Handle (bookingUpdated);

			return View ["change-confirmation"];
		}
	}
}
