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

		public Booking ReadBookingFromFile (string bookingId)
		{
			var bookingPath = Path.Combine (bookingsPath, bookingId);

            if (!File.Exists(bookingPath))
                return null;

			var bookingAsString = File.ReadAllText (bookingPath);

			var booking = JsonConvert.DeserializeObject<Booking> (bookingAsString);

			return booking;
		}

        public void SaveBookingAsFile(Booking booking)
        {
            var savePath = Path.Combine(bookingsPath, booking.Id);
            var bookingAsJson = JsonConvert.SerializeObject(booking);

            File.WriteAllText(savePath, bookingAsJson);
        }

        public void DeleteBooking(String bookingId)
        {
            var bookingPath = Path.Combine(bookingsPath, bookingId);

            File.Delete(bookingPath);
        }
	}
}