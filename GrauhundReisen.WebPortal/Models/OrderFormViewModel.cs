using System.Collections.Generic;
using DatabaseLayer.DataObjects;

namespace GrauhundReisen.WebPortal
{

	public class OrderFormViewModel
	{
		public IEnumerable<Destination> Destinations {
			get;
			set;
		}

		public IEnumerable<CreditCardType> CreditCardTypes {
			get;
			set;
		}
	}
}