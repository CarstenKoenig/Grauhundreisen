using System;
using System.IO;
using GrauhundReisen.ReadModel.Models;
using Newtonsoft.Json;

namespace GrauhundReisen.ReadModel.Repositories
{
	public class Bookings
	{
		readonly String bookingsPath;

		public Bookings (string connectionString)
		{
			bookingsPath = connectionString;
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