﻿using System;
using System.Threading;
using Newtonsoft.Json;
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
            var url = "http://api/events/{eventType}";

            Console.Out.WriteLine($"Traffic Generator started for {url}");


            var client = new RestClient(url);
            var rnd = new Random();


            var intervalMin = Environment.GetEnvironmentVariable("INTERVAL_MIN");
            var intervalMax = Environment.GetEnvironmentVariable("INTERVAL_MAX");
            var iMin = 1000;
            var iMax = 5000;
            if (intervalMin != null)
                iMin = Convert.ToInt32(intervalMin);

            if (intervalMax != null)
                iMax = Convert.ToInt32(intervalMax);

            while (true)
            {
                var @event = GenerateEvent(rnd);
                var request = new RestRequest(Method.POST)
                    .AddUrlSegment("eventType", @event.EventType)
                    .AddJsonBody(@event.Payload);
                request.RequestFormat = DataFormat.Json;

                Console.Out.WriteLine($"Posting to {url} with payload {JsonConvert.SerializeObject(@event.Payload)}");

                client.PostAsync(request, (response, handle) =>
                {
                    Console.Out.WriteLine($"Reveived {response.StatusCode} response");
                });


                Thread.Sleep(rnd.Next(iMin, iMax > iMin ? iMax : iMin));
            }
        }

        private static Event GenerateEvent(Random rnd)
        {
            return new Event
            {
                EventType = _eventTypes[rnd.Next(_eventTypes.Length)],
                Payload = new
                {
                    Browser = _browsers[rnd.Next(_browsers.Length)],
                    BrowserVersion = _broserVersions[rnd.Next(_broserVersions.Length)],
                    Os = _os[rnd.Next(_os.Length)],
                    User = _users[rnd.Next(_users.Length)]
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