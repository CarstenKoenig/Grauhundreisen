using System;
using DatabaseLayer.Repositories;
using System.Collections.Generic;
using DatabaseLayer.DataObjects;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace DatabaseLayer
{
	public class DatabaseServer
	{
		IList<Order> _orders = new List<Order> ();
		IList<Booking> _bookings = new List<Booking> ();

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

			_orders.Add (order);
			_bookings.Add (booking);

			return booking.Id;
		}

		public Booking GetBookingBy(String bookingId){
		
			return _bookings.SingleOrDefault (b => b.Id.Equals (bookingId));
		}

		static String HashOf(String toHash){
			var md5 = new MD5CryptoServiceProvider();

			var textToHash = Encoding.Default.GetBytes (toHash);
			var result = md5.ComputeHash(textToHash); 

			return BitConverter.ToString(result).Replace("-",""); 
		}
	}
}

