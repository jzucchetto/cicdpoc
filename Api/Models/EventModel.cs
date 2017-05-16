namespace Api.Models
{
    public class EventModel : BaseModel
    {
        public string EventType { get; set; }

        public string JsonPayload { get; set; }
    }
}
