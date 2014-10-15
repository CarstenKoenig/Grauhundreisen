using System;
using Grauhundreisen.Infrastructure;

namespace GrauhundReisen.Domain
{
    class DomainSettings
    {
        public static EventStoreClientConfiguration DefaultEventStoreCleintConfiguration
        {
            get
            {
                 var esConfig = new EventStoreClientConfiguration
                    {
                        AccountId = "MyTestAccount",
                        InitActionName = "init",
                        RemoveActionName = "remove",
                        RetrieveActionName = "events",
                        StoreActionName = "store",
                        ServerUri = new Uri("http://openspace2014.azurewebsites.net")
                    };

                return esConfig;
            }
        }
    }
}
