using System;

namespace Grauhhundreisen.Infrastructure
{

	public class PostToServerException : Exception
	{
		public PostToServerException (string message) : base(message)
		{

		}
	}
}
