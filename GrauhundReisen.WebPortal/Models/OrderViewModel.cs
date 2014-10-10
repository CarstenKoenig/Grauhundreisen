using System;

namespace GrauhundReisen.WebPortal
{
	public class OrderViewModel
	{
		public String TravellerFirstName {
			get;
			set;
		}

		public String TravellerLastName {
			get;
			set;
		}

		public String TravellerEmail {
			get;
			set;
		}

		public String PaymentCreditCardType {
			get;
			set;
		}

		public String PaymentCreditCardNumber {
			get;
			set;
		}

		public String Destination {
			get;
			set;
		}
	}
}