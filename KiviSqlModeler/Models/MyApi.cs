using KiviSqlModeler.Models.DataModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models
{
    public static class MyApi
    {
        private static string uri = "http://localhost:8080";

        /// <summary>
        /// получение колекции таблицы
        /// </summary>
        /// <typeparam name="T"> Класс из Models</typeparam>
        /// <param name="table"> Название таблицы </param>
        /// <returns> колекция указанной таблицы </returns>
        public static ObservableCollection<T> GetTable<T>(string table)
        {
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($@"{uri}/{table}");
                dynamic response;
                try
                {
                    response = client.GetAsync(endpoint).Result;
                }
                catch (Exception err)
                {
                    throw err;
                }

                var json = response.Content.ReadAsStringAsync().Result;
                var values = JsonConvert.DeserializeObject<ObservableCollection<T>>(json);
                return values;
            }
        }

        /// <summary>
        /// Получение записи таблицы по её ID
        /// </summary>
        /// <param name="table"> название таблицы </param>
        /// <param name="id"> ID </param>
        /// <returns> словарь данных с записю указанной таблицы </returns>
        public static T GetRecord<T>(string table, int id)
        {
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($@"{uri}/{table}/{id}");
                dynamic response;
                try
                {
                    response = client.GetAsync(endpoint).Result;
                }
                catch (Exception err)
                {
                    throw err;
                }

                var json = response.Content.ReadAsStringAsync().Result;
                var values = JsonConvert.DeserializeObject<T>(json);
                return values;
            }
        }

        /// <summary>
        /// Получение записи таблицы по её параметру типа String
        /// </summary>
        /// <param name="table"> название таблицы </param>
        /// <param name="login"> параметр </param>
        /// <returns> словарь данных с записю указанной таблицы </returns>
        public static T GetRecord<T>(string table, string login)
        {
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($@"{uri}/{table}/Email/{login}");
                HttpResponseMessage response;
                try
                {
                    response = client.GetAsync(endpoint).Result;
                }
                catch (Exception err)
                {
                    throw err;
                }

                var json = response.Content.ReadAsStringAsync().Result;
                var values = JsonConvert.DeserializeObject<T>(json);
                return values;
            }
        }

        public static string CreateSHA256(string input)
        {
            using SHA256 hash = SHA256.Create();
            return Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(input)));
        }

        /// <summary>
        /// Добавление данных в таблицу
        /// </summary>
        /// <param name="table"> название таблицы </param>
        /// <param name="body"> экземпляр класса </param>
        /// <returns> Статус запроса </returns>
        public static string PostRecord(string table, MyTables body)
        {
            using (var client = new RestClient(uri + "/" + table))
            {
                try
                {
                    var request = new RestRequest();
                    request.AddJsonBody(body);
                    var response = client.Post(request);
                    return response.StatusCode.ToString();
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }

        /// <summary>
        /// Изменение записи в таблице
        /// </summary>
        /// <param name="table"> Название таблицы </param>
        /// <param name="body"> экземпляр класса </param>
        /// <returns> Статус запроса </returns>
        public static string PutRecord(string table, MyTables body)
        {
            using (var client = new RestClient(uri + "/" + table))
            {
                try
                {
                    var request = new RestRequest();
                    request.AddJsonBody(body);
                    var response = client.Put(request);
                    return response.StatusCode.ToString();
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }

        /// <summary>
        /// Удаление записи в таблице
        /// </summary>
        /// <param name="table"> название таблицы </param>
        /// <param name="id"> id </param>
        public static void DeleteRecord(string table, int id)
        {
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($@"{uri}/{table}/{id}");
                dynamic response;
                try
                {
                    response = client.DeleteAsync(endpoint).Result;
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }
    }
}
