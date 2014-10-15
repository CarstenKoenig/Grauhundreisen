using System;
using System.Collections.Generic;

namespace Grauhundreisen.Infrastructure
{

	public class RemoveEventStoreException : Exception
	{
		public RemoveEventStoreException (string message) : base(message)
		{

		}
	}
}