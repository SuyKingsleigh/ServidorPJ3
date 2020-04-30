using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Server {
	public class Message {
        
		private static string[] _format =  {"HH:mm:ss d/M/yyyy"};

		public Message(string date, float temp, int uid) {
			this.date = date;
			this.temp = temp;
			this.uid = uid;
		}

		public string date { get; set; }

		public float temp { get; set; }
        
		public int uid { get; set; }
        
		/**
         * Converte uma string recebida para uma mensagem
         */
		public static Message ToMessage(string jsonString) => JsonConvert.DeserializeObject<Message>(jsonString);

		public override string ToString() => "Date: " + this.date + ", Temp: " + this.temp + ", UID: " + this.uid;
        
		public DateTime? GetDate() {
			try {
				return DateTime.ParseExact(this.date, _format, new CultureInfo("pt-BR"));
			}
			catch(Exception e) {
				return null;
			}
		}

		// public static void Main(string[] args) {
		// var jsonString = @"{""date"" : ""19:21:17 28/3/2020"", ""temp"" : 5, ""uid"": 3}";
		// Message message = Message.ToMessage(jsonString); // converte json pra Message 
		//     Console.WriteLine(message);
		//     // Console.WriteLine((ValueType) message.GetDate() ?? -5); // se a data nao for null entao imprime-a
		// }
	}
}