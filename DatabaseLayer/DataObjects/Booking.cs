using System;

namespace DatabaseLayer.DataObjects
{
	public class Booking
	{
		public static Booking CreateFrom(Order order){
			var bookingId = Guid.NewGuid ().ToString ();

			return new Booking (bookingId, order.Traveller, order.Payment, order.Destination);
		}

		public Booking(){
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
			set;
		}

		public Traveller Traveller {
			get;
			set;
		}

		public TravellersBankAccount Payment {
			get;
			set;
		}

		public String Destination {
			get;
			set;
		}
	}
}