using System;

namespace DatabaseLayer.DataObjects
{
	public class Booking
	{
		public static Booking CreateFrom(Order order){
			var bookingId = Guid.NewGuid ().ToString ();

			return new Booking (bookingId, order.Traveller, order.Payment, order.Destination);
		}

		protected Booking (String id, Traveller traveller, TravellersBankAccount payment, String destination)
		{
			Id = id;

			Traveller = traveller;
			Payment = payment;

			Destination = destination;
		}

		public String Id {
			get;
			private set;
		}

		public Traveller Traveller {
			get;
			private set;
		}

		public TravellersBankAccount Payment {
			get;
			private set;
		}

		public String Destination {
			get;
			private set;
		}
	}
}