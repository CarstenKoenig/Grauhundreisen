using System;
using System.Collections.Generic;
using DatabaseLayer.DataObjects;
using DatabaseLayer.Repositories;
using System.IO;
using Newtonsoft.Json;

namespace DatabaseLayer
{
	public class ViewModels
	{
		readonly String bookingsPath;

		public ViewModels (string viewModelsConnection)
		{
			bookingsPath = viewModelsConnection;
		}

		public IEnumerable<CreditCardType> CreditCardTypes
		{
			get {
				return new CreditCardTypes ().GetAll (); 
			}
		}

		public IEnumerable<Destination> Destinations
		{
			get{
				return new Destinations ().GetAll ();
			}
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