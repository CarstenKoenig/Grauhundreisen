using System;
using GrauhundReisen.Contracts.Events;
using System.IO;
using Newtonsoft.Json;
using GrauhundReisen.Contracts.ViewModels;
using System.Threading.Tasks;

namespace GrauhundReisen.ReadModel.EventHandler
{
	public class BookingHandler
	{
		readonly string bookingsPath;

		public BookingHandler (string connectionString)
		{
			bookingsPath = connectionString;
		}

		public async Task Handle(dynamic @event){
			await HandleEvent (@event);
		}

		async Task HandleEvent(BookingOrdered bookingOrdered){

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

			await SaveBookingAsFile (booking);
		}

		async Task HandleEvent(BookingUpdated bookingUpdated){

			var booking = ReadBookingFromFile (bookingUpdated.BookingId);

			booking.Traveller.EMail = bookingUpdated.Email;
			booking.Payment.CreditCardNumber = bookingUpdated.CreditCardNumber;

			await DeleteBooking (bookingUpdated.BookingId);
			await SaveBookingAsFile (booking);
		}

		async Task HandleEvent(object @event){
		
			await Task.Factory.StartNew(() => 
				{
					throw new ArgumentException(
						String.Format("This type ({0}) of event has no handler", @event.GetType()));
				});
		}

		private async Task SaveBookingAsFile (Booking booking)
		{
			var savePath = Path.Combine (bookingsPath, booking.Id);
			var bookingAsJson = JsonConvert.SerializeObject (booking);

			await Task.Factory.StartNew(()=> File.WriteAllText (savePath, bookingAsJson));
		}

		private Booking ReadBookingFromFile (String bookingId)
		{
			var bookingPath = Path.Combine (bookingsPath, bookingId);

			var bookingAsString = File.ReadAllText (bookingPath);

			var booking = JsonConvert.DeserializeObject<Booking> (bookingAsString);

			return booking;
		}

		private async Task DeleteBooking(String bookingId){
			var bookingPath = Path.Combine (bookingsPath, bookingId);

			await Task.Factory.StartNew(() => File.Delete (bookingPath));
		}
	}
}

