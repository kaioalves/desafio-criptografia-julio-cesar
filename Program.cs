using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace desafio_criptografia_julio_cesar
{
    class Program
    {
        static string _TOKEN = "43da7ef5f412d42430dde166a54fb552c7XXXXXX";
        static string _CARACTERES_IMUTAVEIS = " .1234567890";

        private static readonly HttpClient client = new HttpClient();

        public static AnswerResponse criarAnswerJSON(int casas, string token, string cifrado, string decifrado, string resumoCriptografado)
        {
            var answerResponse = new AnswerResponse
            {
                NumeroCasas = casas,
                Token = token,
                Cifrado = cifrado,
                Decifrado = decifrado,
                ResumoCriptografado = resumoCriptografado
            };

            return answerResponse;
        }

        public static async Task enviaArquivoAsync(string token, string path)
        {
            // enviar request multipart/form-data
            var multiForm = new MultipartFormDataContent();

            multiForm.Add(new StringContent(token), "token");

            // adicionar arquivo para ser enviado
            FileStream fs = File.OpenRead(path);
            multiForm.Add(new StreamContent(fs), "answer", Path.GetFileName(path));

            //url para envio POST
            var url = "https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=" + token;

            var response = await client.PostAsync(url, multiForm);

            // obter resposta da api
            var responseJson = await response.Content.ReadAsStringAsync();

            // converte JSON response para Object
            EnvioResponse envioResponse = JsonConvert.DeserializeObject<EnvioResponse>(responseJson);

            // meu score
            Console.WriteLine("MEU SCORE: " + envioResponse.Score + "%");
        }

            public static string decifrarFrase(string frase, int nCasas)
            {
                string[] aLetras = new string[]
                  {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};

                string sRetorno = "";

                int indice = 0;

                char[] charArr = frase.ToCharArray();

                foreach (char ch in charArr)
                {
                    if (_CARACTERES_IMUTAVEIS.Contains(ch.ToString().ToLower()))
                    {
                        sRetorno = sRetorno + ch.ToString().ToLower();
                    }
                    else
                    {
                        indice = Array.IndexOf(aLetras, ch.ToString().ToLower());
                        indice = indice - nCasas;

                        if (indice < 0)
                        {
                            indice = Math.Abs((indice));
                            indice = (aLetras.Length - indice);
                        }

                        sRetorno = sRetorno + aLetras[indice];
                    }
                }

                return sRetorno;
            }

        static async Task Main(string[] args)
        {
            try
            {
                //url para envio GET
                var url = "https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token=" + _TOKEN;

                var response = await client.GetAsync(url);

                // obter resposta da api
                var responseJson = await response.Content.ReadAsStringAsync();

                // converte JSON response para Object
                AnswerResponse answer = JsonConvert.DeserializeObject<AnswerResponse>(responseJson);

                //string mensagemDecifrada = decifrarFrase("gwc kiv vmdmz cvlmzmabquibm bpm abcxqlqbg wn bpm omvmzit xcjtqk. akwbb iliua", 8);
                string mensagemDecifrada = decifrarFrase(answer.Cifrado, answer.NumeroCasas);

                //gerar SH1
                UTF8Encoding enc = new UTF8Encoding();
                byte[] buffer = enc.GetBytes(mensagemDecifrada);
                SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();

                string resumoSH1 = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLower();

                //criar json
                var answerJSON = criarAnswerJSON(answer.NumeroCasas, answer.Token, answer.Cifrado, mensagemDecifrada, resumoSH1);

                var objJSON = JsonConvert.SerializeObject(answerJSON, Formatting.Indented);

                if (File.Exists("answer.json"))
                {
                    File.Delete("answer.json");
                }

                File.WriteAllText("answer.json", objJSON);

                //enviar answer.json
                enviaArquivoAsync(_TOKEN, "answer.json").Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao consumir API! Tente novamente. \n\n" + e);
            }
        }
    }
}
