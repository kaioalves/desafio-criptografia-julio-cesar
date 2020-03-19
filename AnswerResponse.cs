using Newtonsoft.Json;

namespace desafio_criptografia_julio_cesar
{
    public class AnswerResponse
    {
        [JsonProperty("numero_casas")]
        public int NumeroCasas { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("cifrado")]
        public string Cifrado { get; set; }

        [JsonProperty("decifrado")]
        public string Decifrado { get; set; }

        [JsonProperty("resumo_criptografico")]
        public string ResumoCriptografado { get; set; }
    }
}