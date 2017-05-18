namespace Api.Models
{
    public class EventModel : BaseModel
    {
        public string EventType { get; set; }

        public dynamic Payload { get; set; }
    }
}
