using System;
using Nancy;
using GrauhundReisen.Contracts.Events;
using GrauhundReisen.ReadModel.EventHandler;
using GrauhundReisen.ReadModel.Repositories;

namespace GrauhundReisen.WebPortal
{
	public class Index : NancyModule
	{
		readonly BookingHandler _bookingHandler;

		public Index (BookingForm bookingForm, BookingHandler bookingHandler)
		{
			_bookingHandler = bookingHandler;

			Get [""] = _ => View ["index", new { bookingForm.CreditCardTypes, bookingForm.Destinations }];

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

			_bookingHandler.Handle (bookingOrdered);

			return View ["confirmation", new {bookingOrdered.BookingId}];
		}
	}
}