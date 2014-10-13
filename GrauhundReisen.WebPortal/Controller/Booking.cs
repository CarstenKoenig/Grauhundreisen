using Nancy;
using DatabaseLayer;

namespace GrauhundReisen.WebPortal
{
	public class Booking : NancyModule
	{
		readonly DatabaseServer _db;

		public Booking (DatabaseServer db)
		{
			_db = db;

			Get ["change-booking/{id}"] = _ => GetBookingFormFor ((string)_.id);
			Post ["change-booking"] = _ => UpdateBookingFor ();
		}

		object GetBookingFormFor (string bookingId)
		{
			var booking = _db.GetBookingBy (bookingId);

			return View ["change-booking", booking];
		}

		object UpdateBookingFor ()
		{
			var bookingId = this.Request.Form ["BookingId"].Value;
			var travellerEmail = this.Request.Form ["TravellerEMail"].Value;
			var creditCardNumber = this.Request.Form ["PaymentCreditCardNumber"].Value;

			var bookingToUpdate = _db.GetBookingBy (bookingId);

			bookingToUpdate.Traveller.EMail = travellerEmail;
			bookingToUpdate.Payment.CreditCardNumber = creditCardNumber;

			_db.Update (bookingToUpdate);

			return View ["change-confirmation"];
		}
	}
}
