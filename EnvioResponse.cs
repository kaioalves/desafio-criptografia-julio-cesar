using Newtonsoft.Json;

namespace desafio_criptografia_julio_cesar
{
    public class EnvioResponse
    {
        [JsonProperty("score")]
        public int Score { get; set; }
    }
}
