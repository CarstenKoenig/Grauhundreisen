using System.Collections.Generic;
using System.Linq;
using GrauhundReisen.ReadModel.Models;

namespace GrauhundReisen.ReadModel.Repositories
{

	class CreditCardTypes{
	
		public IEnumerable<CreditCardType> GetAll(){
			var ccTypes = new List<CreditCardType>{ 
				new CreditCardType{Name="Master Card"},
				new CreditCardType{Name="Visa"},
				new CreditCardType{Name="American Express"}
			};

			return ccTypes.OrderBy (ccType => ccType.Name);
		}
	}
}
