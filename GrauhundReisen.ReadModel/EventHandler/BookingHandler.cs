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
		readonly string _bookingsPath;

		public BookingHandler (String connectionString)
		{
			_bookingsPath = connectionString;
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

		async Task HandleEvent(BookingEmailChanged emailChanged){

            var booking = ReadBookingFromFile(emailChanged.BookingId);

            booking.Traveller.EMail = emailChanged.Email;

            await DeleteBooking(emailChanged.BookingId);
			await SaveBookingAsFile (booking);
		}

        async Task HandleEvent(BookingCreditCardNumberChanged creditCardChanged)
        {
            var booking = ReadBookingFromFile(creditCardChanged.BookingId);

            booking.Payment.CreditCardNumber = creditCardChanged.CreditCardNumber;

            await DeleteBooking(creditCardChanged.BookingId);
            await SaveBookingAsFile(booking);
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
			var savePath = Path.Combine (_bookingsPath, booking.Id);
			var bookingAsJson = JsonConvert.SerializeObject (booking);

			await Task.Factory.StartNew(() => File.WriteAllText (savePath, bookingAsJson));
		}

		private Booking ReadBookingFromFile (String bookingId)
		{
			var bookingPath = Path.Combine (_bookingsPath, bookingId);

			var bookingAsString = File.ReadAllText (bookingPath);

			var booking = JsonConvert.DeserializeObject<Booking> (bookingAsString);

			return booking;
		}

		private async Task DeleteBooking(String bookingId){
			var bookingPath = Path.Combine (_bookingsPath, bookingId);

			await Task.Factory.StartNew(() => File.Delete (bookingPath));
		}
	}
}

