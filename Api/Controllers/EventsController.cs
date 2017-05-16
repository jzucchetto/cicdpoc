using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RawRabbit;
using RawRabbit.Configuration.Exchange;
using RawRabbit.Extensions.TopologyUpdater;

namespace Api.Controllers
{
    [Route("[controller]")]
    public class EventsController : Controller
    {
        private readonly IBusClient _client;

        public EventsController(IBusClient client)
        {
            _client = client;
        }

        [HttpPost]
        [Route("{eventType}")]
        public void LogEvent(string eventType, [FromBody] JObject jsonPayload)
        {
            var @event = new EventModel
            {
                EventType = eventType,
                JsonPayload = jsonPayload.ToString()
            };

            // send event to RabbitMQ
            _client.PublishAsync(@event, configuration: cfg =>
            {
                cfg.WithExchange(exchangeCfg =>
                        exchangeCfg
                            .WithName("events")
                            .WithType(ExchangeType.Fanout))
                    .WithRoutingKey("events");
            });
        }
    }
}
