using Nancy;
using DatabaseLayer;
using GrauhundReisen.EventHandler;
using GrauhundReisen.Contracts.Events;

namespace GrauhundReisen.WebPortal
{
	public class Booking : NancyModule
	{
		ViewModels _viewModels;
		readonly Bookings _bookings;

		public Booking (ViewModels viewModels, Bookings bookings)
		{
			_viewModels = viewModels;
			_bookings = bookings;

			Get ["change-booking/{id}"] = _ => GetBookingFormFor ((string)_.id);
			Post ["change-booking"] = _ => UpdateBooking ();
		}

		object GetBookingFormFor (string bookingId)
		{
			var booking = _viewModels.GetBookingBy (bookingId);

			return View ["change-booking", booking];
		}

		object UpdateBooking ()
		{
			var bookingUpdated = new BookingUpdated { 
				BookingId = this.Request.Form ["BookingId"].Value,
				Email = this.Request.Form ["TravellerEMail"].Value,
				CreditCardNumber = this.Request.Form ["PaymentCreditCardNumber"].Value
			};

			_bookings.Handle (bookingUpdated);

			return View ["change-confirmation"];
		}
	}
}
