using Newtonsoft.Json.Linq;

namespace CosmosLog.EntityFramework.Model
{
    public class CosmosLogModel : CosmosModel
    {
        public string Source { get; set; } = "Unknow";
        public string Category { get; set; } = "Unknow";
        public string SubCategory { get; set; } = "Unknow";
        public string Level { get; set; } = "Info";

        public JObject? logPayload { get; set; }
    }
}
