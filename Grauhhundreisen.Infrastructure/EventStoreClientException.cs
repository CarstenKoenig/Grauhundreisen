using System;
using System.Collections.Generic;

namespace Grauhundreisen.Infrastructure
{
	
	public class EventStoreClientException : Exception
	{
		public EventStoreClientException (string message) : base(message)
		{

		}
	}
}