using System;
using System.Net.Http;
using System.Threading;
using RestSharp;

namespace TrafficGenerator
{
    class Program
    {
        private static readonly string[] _eventTypes = { "asset_click", "asset_preview", "asset_save", "asset_delete", "asset_download" };
        private static readonly string[] _browsers = { "firefox", "safari", "chrome", "ie" };
        private static readonly string[] _os = { "windows", "macos", "linux", "android", "ios" };
        private static readonly string[] _broserVersions = { "10", "11", "25", "26", "27", "28", "32" };
        private static readonly string[] _users = { "john", "mark", "bob", "tom", "joe" };

        static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ENVIRONMENT");
            string url;

            if (env == "dev")
                url = "http://dev.api.fmc.com/events/{eventType}";
            else if (env == "test")
                url = "http://test.api.fmc.com/events/{eventType}";
            else if (env == "prod")
                url = "http://api.fmc.com/events/{eventType}";
            else
                url = "http://api/events/{eventType}";               

            var client = new RestClient(url);

            while (true)
            {
                var @event = GenerateEvent();
                var request = new RestRequest(Method.POST)
                    .AddUrlSegment("eventType", @event.EventType)
                    .AddJsonBody(@event.Payload);
                request.RequestFormat = DataFormat.Json;

                client.PostAsync(request, (response, handle) =>
                {
                    
                });

                Thread.Sleep(3000);
            }
        }

        private static Event GenerateEvent()
        {
            return new Event
            {
                EventType = _eventTypes[new Random().Next(_eventTypes.Length)],
                Payload = new
                {
                    Browser = _browsers[new Random().Next(_browsers.Length)],
                    BrowserVersion = _broserVersions[new Random().Next(_broserVersions.Length)],
                    Os = _os[new Random().Next(_os.Length)],
                    User = _users[new Random().Next(_users.Length)]
                }
            };
        }

        public class Event
        {
            public string EventType { get; set; }

            public object Payload { get; set; }
        }
    }
}