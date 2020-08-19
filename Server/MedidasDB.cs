using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Server {
    public class MedidasDB {
        private static string _CONNECTION = @"Database=ControleSementes; Data Source=localhost; User Id=root; Password=123";
        private static string _INSERT_MEDIDAS = @"INSERT INTO medidas (data, temp, humidity, usuarios_id) VALUES (@data, @temp, @humidity, @usuarios_id)";
        private static string _INSERT_USUARIO = @"INSERT INTO usuarios (nome) VALUES (@nome)";
        private static string _GET_MEDIDAS_UID = @"SELECT * FROM medidas WHERE usuarios_id=@usuarios_id";

        // nome das tabelas 
        private static string _USER = "usuarios_id";
        private static string _TEMP = "temp";
        private static string _DATA = "data";
        private static string _NOME = "nome";
        private static string _UMIDADE = "humidity";
        
        // parametros passados nas queries
        private static string _P_UID = "@usuarios_id";
        private static string _P_TEMP = "@temp";
        private static string _P_DATA = "@data";
        private static string _P_NOME = "@nome";
        private static string _P_UMIDADE = "@humidity";
        
        /**
         * Salva uma mensagem recebida na base de dados.
         */
        public static void AddMessage(Message message) {
            MySqlConnection sqlConnection = new MySqlConnection(_CONNECTION);
            sqlConnection.Open();
            using var cmd = new MySqlCommand(_INSERT_MEDIDAS, sqlConnection);
                
            cmd.Parameters.AddWithValue(_P_DATA, message.GetDate());
            cmd.Parameters.AddWithValue(_P_TEMP, message.temp);
            cmd.Parameters.AddWithValue(_P_UMIDADE, message.humidity);
            cmd.Parameters.AddWithValue(_P_UID, message.uid);

            cmd.Prepare();
            cmd.ExecuteNonQuery(); 
        }
        
        /**
         * Cadastra um usuario na base de dados. 
         */
        public static void AddUser(string nome) {
            MySqlConnection sqlConnection = new MySqlConnection(_CONNECTION);
            sqlConnection.Open();
            using var cmd = new MySqlCommand(_INSERT_USUARIO, sqlConnection);

            cmd.Parameters.AddWithValue(_P_NOME, nome);
                
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }
        
        /**
         * Retorna uma lista com as mensagens enviadas por um determinado usuario.
         */
        public static List<Message> GetMedidas(int uid) {
            MySqlConnection sqlConnection = new MySqlConnection(_CONNECTION);
            sqlConnection.Open();
            
            using var cmd = new MySqlCommand(_GET_MEDIDAS_UID, sqlConnection);
            cmd.Parameters.AddWithValue(_P_UID, uid);
            using MySqlDataReader dataReader = cmd.ExecuteReader();
            
            var list = new List<Message>();
            while(dataReader.Read()) list.Add(
                new Message(dataReader.GetDateTime(_DATA).ToString(), 
                    dataReader.GetFloat(_TEMP), dataReader.GetInt32(_UMIDADE), uid)
                );
            return list;
        }
    }
}