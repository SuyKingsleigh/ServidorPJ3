#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Server {
    class HTTPServer {
        private static string _host = "http://localhost:42069/";

        public HTTPServer() {
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(_host);
            httpListener.Start();

            while(httpListener.IsListening) ProccessRequest(httpListener.GetContext());
            httpListener.Close();
        }
        
        /**
         * Solicita as mensagens de um determinado usuario (uid)
         * Envia ao cliente a lista de mensanges recebidas
         */
        private async void HandleGet(HttpListenerContext context) {
            // no get vem tudo na URL
            // Extrai o UID (http://server.com/UID) 
            var request = context.Request;
            var reqPath = request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            
            // Pega as medidas do usuario no DB
            int uid;
            dynamic resp = (Int32.TryParse(reqPath, out uid)) ? DataBase.GetMedidas(uid) : null;
            
            // Serializa em JSON 
            if(resp != null) {
                List<string> list = new List<string>();
                foreach(Message message in resp) list.Add(message.ToJson());
                resp = JsonConvert.SerializeObject(list);
            }
            
            this.SendResponse(context.Response, resp);
        }

        private void SendResponse(HttpListenerResponse response, string? resp) {
            var buffer = System.Text.Encoding.UTF8.GetBytes(
                "<HTML><BODY> " + (resp ?? "UID deve ser int") + "</BODY></HTML>");
            
            response.ContentLength64 = buffer.Length;   // Define o tamanho da mensagem 
            System.IO.Stream output = response.OutputStream; // Abre um stream de saia 
            output.Write(buffer,0,buffer.Length);  
            output.Close();
        }
        
        /**
         * Adiciona uma mensagem ao banco de dados 
         */
        private async void HadlePost(HttpListenerContext context) {
        }
        
        private void ProccessRequest(HttpListenerContext context) {
            if(context.Request.HttpMethod == HttpMethod.Get.Method) this.HandleGet(context);
            if(context.Request.HttpMethod == HttpMethod.Post.Method) this.HadlePost(context);
        }
        
        public static void Main(string[] args) {
            try {
                HTTPServer server = new HTTPServer();
            }
            catch(Exception e) {
                Console.WriteLine(e);
            }
        }
        
        
    }
}