#nullable enable

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

namespace Server {
    class HTTPServer {
        private static string _host0 = "http://127.0.0.1:42069/";
        private static string _host1 = "http://localhost:42069/";
        private int _tid;
        
        public HTTPServer() {
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(_host0);
            httpListener.Prefixes.Add(_host1);
            httpListener.Start();

            while(httpListener.IsListening) {
                var thread = new Thread(ProccessRequest);
                thread.Name = (++_tid).ToString();
                thread.Start(httpListener.GetContext()); 
                Console.WriteLine($"Iniciou a Thread:  {thread.Name}");
            }
            
            httpListener.Close();
        }

        /**
         * Adiciona uma mensagem ao banco de dados 
         */
        private void HandlePost(HttpListenerContext context) {
            string body = new StreamReader(context.Request.InputStream).ReadToEnd();
            Console.WriteLine("[POST] Recebeu: " + body);
            try {
                DataBase.AddMessage(Message.ToMessage(body));
                SendResponse(context.Response, "ACK");
            }
            catch(Exception e) {
                SendResponse(context.Response, "NACK", 400);
            }
        }

        /**
         * Solicita as mensagens de um determinado usuario (uid)
         * Envia ao cliente a lista de mensanges recebidas
         */
        private void HandleGet(HttpListenerContext context) {
            // no get vem tudo na URL
            // Extrai o UID (http://server.com/UID) 
            var request = context.Request;
            var reqPath = request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            
            // tenta converter reqPath, caso consiga retorna true e altera o valor de uid
            int uid;
            dynamic resp = (Int32.TryParse(reqPath, out uid)) ? DataBase.GetMedidas(uid) : null;
            
            // Converte as mensagens para Json e as coloca numa lista
            if(resp != null) {
                List<string> list = new List<string>();
                foreach(Message message in resp) list.Add(message.ToJson());
                resp = JsonConvert.SerializeObject(list);
                this.SendResponse(context.Response, resp);
            }
            else
                this.SendResponse(context.Response, resp, 400);

        }

        private void SendResponse(HttpListenerResponse response, string? resp, int status=200) {
            var buffer = System.Text.Encoding.UTF8.GetBytes(resp ?? "UID deve ser int");
            response.StatusCode = status;
            response.ContentLength64 = buffer.Length;   // Define o tamanho da mensagem 
            System.IO.Stream output = response.OutputStream; // Abre um stream de saia 
            output.Write(buffer,0,buffer.Length);  
            output.Close();
        }

        private void ProccessRequest(object? o) {
            var context = (HttpListenerContext) o;
            Console.WriteLine($"Thread: {Thread.CurrentThread.Name} " +
                              $"Recebeu um request: {context.Request.HttpMethod}");
            
            if(context.Request.HttpMethod == HttpMethod.Get.Method) this.HandleGet(context);
            else if(context.Request.HttpMethod == HttpMethod.Post.Method) this.HandlePost(context);
            
            Console.WriteLine($"Terminou a thread {Thread.CurrentThread.Name}");
            _tid--;
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