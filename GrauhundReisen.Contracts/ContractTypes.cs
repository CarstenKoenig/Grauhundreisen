using System;

namespace GrauhundReisen.Contracts
{
	public static class ContractTypes
	{
		public static Type Resolve(String contractName)
		{
			var contractType = Type.GetType (contractName);

			return contractType;
		}
	}
}

