using System.Net;

namespace LibraryManagerTests.Models
{
    public class Response<T>
    {
        public T Payload { get; set; }
        public Error Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
