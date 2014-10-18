using System;
using System.IO;
using GrauhundReisen.ReadModelFunktional;
using Microsoft.FSharp.Core;
using Newtonsoft.Json;

namespace GrauhundReisen.ReadModel.Repositories
{
	public class Bookings : Booking.IRepository
	{
		readonly String _bookingsPath;

		public Bookings (string connectionString)
		{
			_bookingsPath = connectionString;
		}

		public Booking.T GetBookingBy(String bookingId){

			return ReadBookingFromFile(bookingId);
		}

        public Booking.T ReadBookingFromFile(string bookingId)
		{
			var bookingPath = Path.Combine (_bookingsPath, bookingId);

            if (!File.Exists(bookingPath))
                return null;

			var bookingAsString = File.ReadAllText (bookingPath);

            var booking = JsonConvert.DeserializeObject<Booking.T>(bookingAsString);

			return booking;
		}

        public void SaveBookingAsFile(Booking.T booking)
        {
            var savePath = Path.Combine(_bookingsPath, booking.Id);
            var bookingAsJson = JsonConvert.SerializeObject(booking);

            File.WriteAllText(savePath, bookingAsJson);
        }

        public void DeleteBooking(String bookingId)
        {
            var bookingPath = Path.Combine(_bookingsPath, bookingId);

            File.Delete(bookingPath);
        }

	    public FSharpOption<Booking.T> GetBy(string id)
	    {
	        var rm = ReadBookingFromFile(id);
	        if (rm == null) return Booking.none;
	        return Booking.some(rm);
	    }

	    public void Delete(string id)
	    {
	        DeleteBooking(id);
	    }

	    public void Save(Booking.T booking)
	    {
	        SaveBookingAsFile(booking);
	    }
	}
}