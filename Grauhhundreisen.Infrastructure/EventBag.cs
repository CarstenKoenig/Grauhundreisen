using System;

namespace Grauhundreisen.Infrastructure
{

	internal class EventBag
	{
		public String EventType {
			get;
			set;
		}

		public String EventData {
			get;
			set;
		}

		public String TimeStamp {
			get;
			set;
		}

		public String EntityId {
			get;
			set;
		}

		public String EventId {
			get;
			set;
		}
	}
}
