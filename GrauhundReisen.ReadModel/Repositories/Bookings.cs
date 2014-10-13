using System;
using System.IO;
using Newtonsoft.Json;
using GrauhundReisen.Contracts.ViewModels;

namespace GrauhundReisen.ReadModel.Repositories
{
	public class Bookings
	{
		readonly String bookingsPath;

		public Bookings (string viewModelsConnection)
		{
			bookingsPath = viewModelsConnection;
		}

		public Booking GetBookingBy(String bookingId){

			return ReadBookingFromFile(bookingId);
		}

		private Booking ReadBookingFromFile (string bookingId)
		{
			var bookingPath = Path.Combine (bookingsPath, bookingId);

			var bookingAsString = File.ReadAllText (bookingPath);

			var booking = JsonConvert.DeserializeObject<Booking> (bookingAsString);

			return booking;
		}
	}
}