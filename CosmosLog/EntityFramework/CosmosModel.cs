using CosmosLog.Helpers;

namespace CosmosLog.EntityFramework
{
    public abstract class CosmosModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long ttl { get; set; } = CosmosHelper.Ttl(0, 0, 100);

        public DateTime DtInclusao { get; set; } = DateTimeHelper.Now();
    }
}
