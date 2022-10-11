using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using LibraryManagerTests.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LibraryManagerTests
{
    public class LibraryManagerClient
    {
        private HttpClient _client;
        private static IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        private static string Uri => Configuration.GetConnectionString("LibraryManager.Url");

        public LibraryManagerClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Uri)
            };
        }

        public Response<Book> AddBook(Book book)
        {
            var response = _client.PostAsync("/api/books", GetRequest(book)).GetAwaiter().GetResult();
            return GetResponse<Book>(response);
        }

        public Response<Book> UpdateBook(Book book, int id)
        {
            var response = _client.PutAsync($"/api/books/{id}", GetRequest(book)).GetAwaiter().GetResult();
            return GetResponse<Book>(response);
        }

        public Response<Book> DeleteBook(int id)
        {
            var response = _client.DeleteAsync($"/api/books/{id}").GetAwaiter().GetResult();
            return GetResponse<Book>(response);
        }

        public Response<Book> GetBook(int id)
        {
            var response = _client.GetAsync($"/api/books/{id}").GetAwaiter().GetResult();
            return GetResponse<Book>(response);
        }

        public Response<List<Book>> GetBooks(string title = null)
        {
            var response = _client.GetAsync($"/api/books?title={title}").GetAwaiter().GetResult();
            return GetResponse<List<Book>>(response);
        }

        private StringContent GetRequest<T>(T body)
        {
            return new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        }

        private Response<T> GetResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            var response = new Response<T>
            {
                StatusCode = httpResponseMessage.StatusCode
            };

            var httpPayload = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                response.Payload = JsonConvert.DeserializeObject<T>(httpPayload);
            }
            else
            {
                response.Error = JsonConvert.DeserializeObject<Error>(httpPayload);
            }

            return response;
        }
    }
}
