using System;
using DatabaseLayer;
using Nancy;
using GrauhundReisen.EventHandler;
using GrauhundReisen.Contracts.Events;

namespace GrauhundReisen.WebPortal
{
	public class Index : NancyModule
	{
		readonly Bookings _bookings;

		public Index (ViewModels viewModels, Bookings bookings)
		{
			_bookings = bookings;

			Get [""] = _ => View ["index", new { viewModels.CreditCardTypes, viewModels.Destinations }];

			Post [""] = _ => ProceedBooking ();
		}

		object ProceedBooking ()
		{
			var bookingOrdered = new BookingOrdered () {
				BookingId = Guid.NewGuid ().ToString (),
				Destination = this.Request.Form ["Destination"].Value,
				CreditCardNumber = this.Request.Form ["PaymentCreditCardNumber"].Value,
				CreditCardType = this.Request.Form ["PaymentCreditCardType"].Value,
				Email = this.Request.Form ["TravellerEmail"].Value,
				FirstName = this.Request.Form ["TravellerFirstName"].Value,
				LastName = this.Request.Form ["TravellerLastName"].Value
			};

			_bookings.Handle (bookingOrdered);

			return View ["confirmation", new {bookingOrdered.BookingId}];
		}
	}
}