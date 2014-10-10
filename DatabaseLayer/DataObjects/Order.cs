using System;

namespace DatabaseLayer.DataObjects
{
	public class Order
	{
		public Order ()
		{
			Traveller = new	Traveller ();
			Payment = new TravellersBankAccount ();
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

