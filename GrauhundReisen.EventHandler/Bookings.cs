using System;
using GrauhundReisen.Contracts.Events;
using DatabaseLayer.DataObjects;
using System.IO;
using Newtonsoft.Json;

namespace GrauhundReisen.EventHandler
{
	public class Bookings
	{
		readonly string bookingsPath;

		public Bookings (string connectionString)
		{
			bookingsPath = connectionString;
		}

		public void Handle(dynamic @event){
			HandleEvent (@event);
		}

		void HandleEvent(BookingOrdered bookingOrdered){

			var traveller = new Traveller{ 
				ID = Guid.NewGuid().ToString(),
				EMail =bookingOrdered.Email,
				FirstName = bookingOrdered.FirstName,
				LastName=bookingOrdered.LastName
			};

			var payment = new TravellersBankAccount{ 
				ID = Guid.NewGuid().ToString(),
				CreditCardType = bookingOrdered.CreditCardType,
				CreditCardNumber = bookingOrdered.CreditCardNumber,
				TravellerID = traveller.ID
			};

			var booking = new Booking {
				Id = bookingOrdered.BookingId,
				Destination = bookingOrdered.Destination,
				Traveller = traveller,
				Payment = payment
			};

			SaveBookingAsFile (booking);
		}

		void HandleEvent(BookingUpdated bookingUpdated){

			var booking = ReadBookingFromFile (bookingUpdated.BookingId);

			booking.Traveller.EMail = bookingUpdated.Email;
			booking.Payment.CreditCardNumber = bookingUpdated.CreditCardNumber;

			DeleteBooking (bookingUpdated.BookingId);
			SaveBookingAsFile (booking);
		}

		void HandleEvent(object @event){
		
			throw new ArgumentException ("This type of event has no handler");
		}

		private void SaveBookingAsFile (Booking booking)
		{
			var savePath = Path.Combine (bookingsPath, booking.Id);
			var bookingAsJson = JsonConvert.SerializeObject (booking);

			File.WriteAllText (savePath, bookingAsJson);
		}

		private Booking ReadBookingFromFile (String bookingId)
		{
			var bookingPath = Path.Combine (bookingsPath, bookingId);

			var bookingAsString = File.ReadAllText (bookingPath);

			var booking = JsonConvert.DeserializeObject<Booking> (bookingAsString);

			return booking;
		}

		private void DeleteBooking(String bookingId){
			var bookingPath = Path.Combine (bookingsPath, bookingId);

			File.Delete (bookingPath);
		}
	}
}

