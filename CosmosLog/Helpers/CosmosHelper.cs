namespace CosmosLog.Helpers
{
    public static class CosmosHelper
    {
        /// <summary>
        /// Retorna o TimeToLive de um registro do Cosmos
        /// </summary>
        /// <param name="tempo">Exemplo: 00:15 para 15 minutos</param>
        /// <returns></returns>
        public static long TtlMinutos(string tempo)
        {
            return TtlHoras(tempo);
        }

        /// <summary>
        /// Retorna o TimeToLive de um registro do Cosmos
        /// </summary>
        /// <param name="tempo">Exemplo: 02:15 para 2 Hora e 15 minutos</param>
        /// <returns></returns>
        public static long TtlHoras(string tempo)
        {
            TimeSpan timeSpan = TimeSpan.Parse(tempo);

            return (long)timeSpan.TotalSeconds;
        }

        public static long TtlDias(int dias)
        {
            return (long)(dias * 24 * 60 * 60);
        }

        public static long Ttl(int dias, int meses, int anos)
        {
            long segundosPorDia = 86400;
            long segundosPorMes = 30 * segundosPorDia;
            long segundosPorAno = 365 * segundosPorDia;

            return (dias * segundosPorDia) + (meses * segundosPorMes) + (anos * segundosPorAno);
        }

    }
}
