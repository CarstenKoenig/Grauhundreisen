using System;
using System.Collections.Generic;

namespace Grauhhundreisen.Infrastructure
{
	
	public class EventStoreClientException : Exception
	{
		public EventStoreClientException (string message) : base(message)
		{

		}
	}
}