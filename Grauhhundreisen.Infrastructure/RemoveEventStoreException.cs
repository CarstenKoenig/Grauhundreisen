using System;
using System.Collections.Generic;

namespace Grauhhundreisen.Infrastructure
{

	public class RemoveEventStoreException : Exception
	{
		public RemoveEventStoreException (string message) : base(message)
		{

		}
	}
}