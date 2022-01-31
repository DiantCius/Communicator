using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Text { get; set; }
        public DateTime WhenSent { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public int ChatId { get; set; }
        [JsonIgnore]
        public Chat Chat { get; set; }
        [JsonIgnore]
        public int PersonId { get; set; }
        [JsonIgnore]
        public Person Person { get; set; }
    }
}
