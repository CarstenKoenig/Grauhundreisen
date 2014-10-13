using System;

namespace GrauhundReisen.Contracts.Events
{
	public class BookingOrdered
	{
		public String BookingId {
			get;
			set;
		}

		public String FirstName {
			get;
			set;
		}

		public String LastName {
			get;
			set;
		}

		public String Email {
			get;
			set;
		}

		public String CreditCardType {
			get;
			set;
		}

		public String CreditCardNumber {
			get;
			set;
		}

		public String Destination {
			get;
			set;
		}
	}
}

