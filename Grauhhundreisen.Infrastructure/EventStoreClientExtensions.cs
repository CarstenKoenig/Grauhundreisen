using System;
using RestSharp.Serializers;

namespace Grauhhundreisen.Infrastructure
{

	static class EventStoreClientExtensions
	{
		public static bool IsNullOrEmpty(this String source){
			return String.IsNullOrEmpty (source) || String.IsNullOrWhiteSpace (source);
		}

		public static bool IsNotNullOrEmpty(this String source){
			return source.IsNullOrEmpty() == false;
		}

		public static bool HasEmptyPath(this UriBuilder source)
		{
			return String.IsNullOrWhiteSpace (source.Path)
				|| (source.Path.EndsWith ("/") && source.Path.Length == 1);
		}

		public static String PathWithSlash(this UriBuilder source)
		{
			return source.Path.EndsWith ("/") ? source.Path : source.Path + "/";
		}

		public static Uri Add(this Uri source, String toAdd){
			var ub = new UriBuilder (source);

			if (ub.HasEmptyPath()) {
				ub.Path = toAdd;
			} else {
				ub.Path = (ub.PathWithSlash() + toAdd);
			}

			return ub.Uri;
		}

		public static EventBag ToEventBag(this object source)
		{
			var serializedSource = new JsonSerializer ().Serialize (source);

			return new EventBag{
				EventId = Guid.NewGuid().ToString(),
				EventData = serializedSource,
				EventType = source.GetType().ToString(),
				TimeStamp = DateTime.Now.ToUniversalTime ().ToString ("O")
			};
		}

		public static EventBag ToEventBag(this object source, String entityId)
		{
			var serializedSource = new JsonSerializer ().Serialize (source);

			return new EventBag{
				EventId = Guid.NewGuid().ToString(),
				EntityId = entityId,
				EventData = serializedSource,
				EventType = source.GetType().ToString(),
				TimeStamp = DateTime.Now.ToUniversalTime ().ToString ("O")
			};
		}

		public static string Serialize (this EventBag source)
		{
			var serializedSource = new JsonSerializer ().Serialize (source);

			return serializedSource;
		}
	}

}
