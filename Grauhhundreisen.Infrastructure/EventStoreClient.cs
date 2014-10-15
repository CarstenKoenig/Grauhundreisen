using System;
using System.Collections.Generic;
using System.Linq;
using GrauhundReisen.Contracts;
using GrauhundReisen.Contracts.Events;
using RestSharp;
using RestSharp.Deserializers;
using fastJSON;

namespace Grauhundreisen.Infrastructure
{
	public class EventStoreClient
	{
		public static EventStoreClient InitWith(EventStoreClientConfiguration configuration){

			if (configuration.IsComplete())
				return new EventStoreClient (configuration);

			throw new EventStoreClientException ("The configuration is not complete");
		}

		protected EventStoreClient (EventStoreClientConfiguration configuration)
		{
			ServerUri = configuration.ServerUri;
			AccountId = configuration.AccountId;

			SetupUris (configuration);

			InitEventStore ();
		}

		void SetupUris (EventStoreClientConfiguration configuration)
		{
			InitUri = ServerUri.Add (configuration.InitActionName).Add (configuration.AccountId);
			StoreUri = ServerUri.Add (configuration.StoreActionName).Add (configuration.AccountId);
			RetrieveUri = ServerUri.Add (configuration.RetrieveActionName).Add (configuration.AccountId);
			RemoveUri = ServerUri.Add (configuration.RemoveActionName).Add (configuration.AccountId);
		}

		void InitEventStore(){
			var restClient = new RestClient(InitUri.ToString());
			var request = new RestRequest (Method.POST);

			var response = restClient.Execute (request);

			if (response.StatusCode != System.Net.HttpStatusCode.OK) {

				if (response.Content.IsNullOrEmpty ()) {
					throw new EventStoreClientException (response.StatusCode.ToString());
				}

				throw new EventStoreClientException (response.Content);
			}
		}

		public Uri ServerUri {
			get;
			private set;
		}

		public Uri InitUri {
			get;
			private set;
		}

		public Uri StoreUri {
			get;
			private set;
		}

		public Uri RetrieveUri {
			get;
			private set;
		}

		public Uri RemoveUri {
			get;
			private set;
		}

		public String AccountId {
			get;
			private set;
		}

		public void Store (object @event)
		{
			var eventBagOfEvent = @event.ToEventBag ();
			var serializedEventBag = eventBagOfEvent.Serialize ();

			PostToServer (serializedEventBag);
		}

		public void Store (string entityId, object @event)
		{
			var eventBagOfEvent = @event.ToEventBag (entityId);
			var serializedEventBag = eventBagOfEvent.Serialize ();

			PostToServer (serializedEventBag);
		}

		public IEnumerable<object> RetrieveAll(){

			var allEvents = GetFromServer (RetrieveUri);

			return allEvents;
		}

		public IEnumerable<object> RetrieveFor(String entityId){

			var entityUrl = RetrieveUri.Add (entityId);

			var eventsOfEntity = GetFromServer (entityUrl);

			return eventsOfEntity;
		}

		public void RemoveEventStore(){

			var restClient = new RestClient(RemoveUri.ToString());
			var request = new RestRequest (Method.DELETE);

			request.RequestFormat = DataFormat.Json;

			var response = restClient.Execute (request);

			if (response.StatusCode != System.Net.HttpStatusCode.OK) {

				if (response.Content.IsNullOrEmpty ()) {
					throw new RemoveEventStoreException (response.StatusCode.ToString());
				}

				throw new RemoveEventStoreException (response.Content);
			}

		}

		void PostToServer(String sendData)
		{ 
			var restClient = new RestClient(StoreUri.ToString());
			var request = new RestRequest (Method.POST);

			request.AddParameter("application/json; charset=utf-8", sendData, ParameterType.RequestBody);
			request.RequestFormat = DataFormat.Json;

			var response = restClient.Execute (request);

			if (response.StatusCode != System.Net.HttpStatusCode.OK) {

				if (response.Content.IsNullOrEmpty ()) {
					throw new PostToServerException (response.StatusCode.ToString());
				}

				throw new PostToServerException (response.Content);
			}
		}

		IEnumerable<object> GetFromServer (Uri requestUrl)
		{
			var restClient = new RestClient(requestUrl.ToString());

			var request = new RestRequest (Method.GET);
			request.RequestFormat = DataFormat.Json;

			var response = restClient.Execute (request);

			if (response.StatusCode == System.Net.HttpStatusCode.OK) {
				var deserial = new JsonDeserializer ();
				var eventBags = deserial.Deserialize<List<EventBag>> (response);

				var events = eventBags
					.Where (eb => eb.EventType.IsNotNullOrEmpty ())
					.OrderBy(eb=>eb.TimeStamp)
					.Select (eb => {
						var eventType = ContractTypes.Resolve(eb.EventType);
						return JSON.ToObject (eb.EventData, eventType);
					})
					.ToList ();

				return events;
			}

			return new List<object> ();
		}
	}
}