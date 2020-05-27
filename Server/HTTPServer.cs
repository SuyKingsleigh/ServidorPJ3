#nullable enable

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server {
    class HTTPServer {
        private static string _host = "http://127.0.0.1:42069/";

        /**
         * Adiciona uma mensagem ao banco de dados 
         */
        private void HandlePost(HttpListenerContext context) {
            string body = new StreamReader(context.Request.InputStream).ReadToEnd();
            Console.WriteLine("[POST] Recebeu: " + body);
            
            try {
                MedidasDB.AddMessage(Message.ToMessage(body));
                SendResponse(context.Response, "ACK");
            }
            catch(Exception e) {
                SendResponse(context.Response, "NACK", 400);
            }
        }

        /**
         * Solicita as mensagens de um determinado usuario (uid)
         * Envia ao cliente a lista de mensagens(medidas) recebidas
         */
        private void HandleGet(HttpListenerContext context) {
            // no get vem tudo na URL
            // Extrai o UID (http://server.com/UID) 
            var request = context.Request;
            var reqPath = request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            
            // Pega as medidas do usuario no DB
            int uid;
            dynamic resp = (Int32.TryParse(reqPath, out uid)) ? MedidasDB.GetMedidas(uid) : null;
            
            // Serializa em JSON 
            if(resp != null) {
                var list = new List<string>();
                foreach(Message message in resp) list.Add(message.ToJson());
                resp = JsonConvert.SerializeObject(list);
                this.SendResponse(context.Response, resp);
            }
            else
                this.SendResponse(context.Response, resp, 400);
        }
        
        /**
         * Cadastra um novo usuario
         */
        private void HandlePut(HttpListenerContext context) {
            string body = new StreamReader(context.Request.InputStream).ReadToEnd();
            Console.WriteLine("[PUT] Recebeu: " + body);
        }
        
        private void SendResponse(HttpListenerResponse response, string? resp, int status=200) {
            var buffer = System.Text.Encoding.UTF8.GetBytes(resp ?? $"Erro: {status}");
            response.StatusCode = status;
            response.ContentLength64 = buffer.Length;  // Define o tamanho da mensagem 
            System.IO.Stream output = response.OutputStream; // Abre um stream de saia 
            output.Write(buffer,0,buffer.Length);  
            output.Close();
        }

        private async Task ProccessRequestAsync(Task<HttpListenerContext> con ) {
            var context = await con; // espera o resultado do request e entao o processa 
            Console.WriteLine($"Recebeu um request: {context.Request.HttpMethod}");
            if(context.Request.HttpMethod == HttpMethod.Get.Method) this.HandleGet(context);
            else if(context.Request.HttpMethod == HttpMethod.Post.Method) this.HandlePost(context);
            else if(context.Request.HttpMethod == HttpMethod.Put.Method) this.HandlePut(context);
        }

        public async Task Run() { 
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(_host);
            httpListener.Start();
            
            // para cada request recebido, ira pegar os dados 
            // mas nao ficará bloqueado no mesmo 
            // assim consegue processar varios de uma vez. 
            while(httpListener.IsListening) 
                await this.ProccessRequestAsync(httpListener.GetContextAsync());
            httpListener.Close();
        }
        
        public static async Task Main(string[] args) {
            try {
                await new HTTPServer().Run();
            }
            catch(Exception e) {
                Console.WriteLine(e);
            }
        }
        
    }
}