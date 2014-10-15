using System;

namespace Grauhhundreisen.Infrastructure
{

	public struct EventStoreClientConfiguration{

		public Uri ServerUri;
		public String InitActionName;
		public String StoreActionName;
		public String RetrieveActionName;

		public String RemoveActionName;

		public String AccountId;

		public bool IsComplete(){

			return ServerUri.ToString ().IsNotNullOrEmpty ()
				&& InitActionName.IsNotNullOrEmpty ()
				&& StoreActionName.IsNotNullOrEmpty ()
				&& RetrieveActionName.IsNotNullOrEmpty ()
				&& AccountId.IsNotNullOrEmpty ();
		}
	}
}
