using System;

namespace Grauhundreisen.Infrastructure
{

	public class PostToServerException : Exception
	{
		public PostToServerException (string message) : base(message)
		{

		}
	}
}
