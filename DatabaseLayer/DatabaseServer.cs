using System;
using DatabaseLayer.Repositories;
using System.Collections.Generic;
using DatabaseLayer.DataObjects;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace DatabaseLayer
{
	public class DatabaseServer
	{
		const String BookingsPath = "Content/DataStore/Bookings/";
		const String OrdersPath = "Content/DataStore/Orders/";

		public IEnumerable<CreditCardType> GetAlCreditCardTypes ()
		{
			return new CreditCardTypes ().GetAll ();
		}

		public IEnumerable<Destination> GetAllDestinations ()
		{
			return new Destinations ().GetAll ();
		}

		public String SaveOrder (Order order)
		{
			order.Traveller.ID = HashOf (order.Traveller.EMail);
			order.Payment.ID = HashOf (order.Payment.CreditCardNumber);
			order.Payment.TravellerID = order.Traveller.ID;

			var booking = Booking.CreateFrom(order);

			SaveBookingAsFile (booking);

			SaveOrderAsFile (order, booking.Id);

			return booking.Id;
		}

		public Booking GetBookingBy(String bookingId){
		
			return ReadBookingFromFile(bookingId);
		}

		public void Update (Booking booking)
		{
			var bookingPath = Path.Combine (BookingsPath, booking.Id);

			File.Delete (bookingPath);

			SaveBookingAsFile (booking);
		}

		static String HashOf(String toHash){
			var md5 = new MD5CryptoServiceProvider();

			var textToHash = Encoding.Default.GetBytes (toHash);
			var result = md5.ComputeHash(textToHash); 

			return BitConverter.ToString(result).Replace("-",""); 
		}

		static void SaveOrderAsFile (Order order, String bookingId)
		{
			var savePath = Path.Combine (OrdersPath, bookingId);
			var orderAsJson = JsonConvert.SerializeObject (order);

			File.WriteAllText (savePath, orderAsJson);
		}

		static void SaveBookingAsFile (Booking booking)
		{
			var savePath = Path.Combine (BookingsPath, booking.Id);
			var bookingAsJson = JsonConvert.SerializeObject (booking);

			File.WriteAllText (savePath, bookingAsJson);
		}

		static Booking ReadBookingFromFile (string bookingId)
		{
			var bookingPath = Path.Combine (BookingsPath, bookingId);

			var bookingAsString = File.ReadAllText (bookingPath);

			var booking = JsonConvert.DeserializeObject<Booking> (bookingAsString);

			return booking;
		}
	}
}

