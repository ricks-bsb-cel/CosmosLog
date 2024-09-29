namespace CosmosLogCall
{
    public class LogModel
    {
        public string Source { get; set; } = "Unknow";
        public string Category { get; set; } = "Unknow";
        public string SubCategory { get; set; } = "Unknow";
        public string Level { get; set; } = "Info";
        public object? logPayload { get; set; }
        public long? Ttl { get; set; }
    }
}
