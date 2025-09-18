using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChirpProject.MainApp.CheepClass
{
    public record Cheep
    {
        public Cheep() { }

        public Cheep(string message)
        {
            Message = message;
            Author = Environment.UserName;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public Cheep(string message, DateTimeOffset time)
        {
            Message = message;
            Author = Environment.UserName;
            Timestamp = time.ToUnixTimeSeconds();
        }

        public string Author { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
    }
}
