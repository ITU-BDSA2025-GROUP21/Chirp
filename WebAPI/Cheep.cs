namespace WebAPI
{
    public record Cheep
    {
        public Cheep(string Author, string Message, long Timestamp)
        {
            this.Message = Message;
            this.Author = Author;
            this.Timestamp = Timestamp;
        }

        public string Author { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
    }
}
