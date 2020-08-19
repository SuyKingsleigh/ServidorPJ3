using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Newtonsoft.Json;

namespace Server {
	public class Message {
		public Message(string date, float temp, int humidity, int uid) {
			this.date = date;
			this.temp = temp;
			this.humidity = humidity;
			this.uid = uid;
		}


		private static string[] _format = {"HH:mm:ss d/M/yyyy"};


		public string date { get; set; }

		public float temp { get; set; }
		
		public int humidity { get; set; }

		public int uid { get; set; }


		/**
     * Converte uma string recebida para uma mensagem
     */
		public static Message ToMessage(string jsonString) 
			=> JsonConvert.DeserializeObject<Message>(jsonString);

		public override string ToString() 
			=> "Date: " + this.date + ", Temp: " + this.temp + ", UID: " + this.uid + ", humidity: " + this.humidity;

		public string ToJson() 
			=> JsonConvert.SerializeObject(this, Formatting.None);

		public DateTime? GetDate() {
			try {
				return DateTime.ParseExact(this.date, _format, new CultureInfo("pt-BR"));
			}
			catch(Exception e) {
				return null;
			}
		}
	}
}