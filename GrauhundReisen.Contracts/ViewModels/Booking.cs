using System;

namespace GrauhundReisen.Contracts.ViewModels
{
	public class Booking
	{
		public Booking(){
		}

		public Booking (String id, Traveller traveller, TravellersBankAccount payment, String destination)
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