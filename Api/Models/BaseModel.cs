using System;

namespace Api.Models
{
    public abstract class BaseModel
    {
        protected BaseModel()
        {
            Timestamp = DateTime.UtcNow.ToString("o");
        }

        public string Timestamp { get; }
    }
}