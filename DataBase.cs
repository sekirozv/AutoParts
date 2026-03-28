using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GardensRu
{
    internal class DataBase
    {
        //public static string FIO, Category, Check;
        public static SqlConnection Connect()
        {
            return new SqlConnection(@"Data source = localhost; Initial catalog = AutoParts; Integrated Security = true");
        }
        public static void IntoDB(string stroke)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(stroke, Connect());
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
        }
        public static DataTable FromIntoDB(string stroke)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(stroke, Connect());
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }

        public static void ExecuteCommand(string sql)
        {
            using (var connection = Connect())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Хеширует пароль по SHA256 (в виде строки из 64 hex-символов).
        /// Используется при сохранении пароля в БД и при проверке при авторизации.
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
