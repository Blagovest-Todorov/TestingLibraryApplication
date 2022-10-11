using LibraryManagerTests.Models;
using NUnit.Framework;
using System.Net;

namespace LibraryManagerTests.Tests
{
    internal abstract class BaseTest
    {      
        protected const string Author = "Ivan Vazov";
        protected const string Title = "Pod Igoto";
        protected const string Description = "Hard time for BG People";

        protected const string UpdatedAuthor = "Ivan Vazov1";
        protected const string UpdatedTitle = "Pod Igoto Tom 1";
        protected const string UpdatedDescription = "Hard times";
        protected const string NonExistingIdError = "Book with id {0} not found!";
        protected const string ExistingIdError = "Book with id {0} already exists!";
        protected const string InvalidIdError = "Book.Id should be a positive integer!\r\nParameter name: book.Id";
        protected const string InvalidTitleError = "Book.Title should not exceed 100 characters!\r\nParameter name: Book.Title";
        protected const string TooLongAuthorError = "Book.Author should not exceed 30 characters!\r\nParameter name: Book.Author";
        protected const string TooLongTitleError = "Book.Title should not exceed 100 characters!\r\nParameter name: Book.Title";
        protected const string AuthorRequiredError = "Book.Author is a required field.\r\nParameter name: Book.Author";
        protected const string TitleRequiredError = "Book.Title is a required field\r\nParameter name: Book.Title";

        protected const string InvalidAuthor31CharsLength = "Ivan VazovIvan VazovIvan Vazov1";
        protected const string ValidAuthor1CharsLength = "I";
        protected const string ValidAuthor30CharsLength = "Ivan VazovIvan VazovIvan Vazov";
        protected const string InvalidTitle101CharsLength = "Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 12";
        protected const string ValidTitle100CharsLength = "Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1";
        protected const string ValidTitle1CharsLength = "P";
        protected const string ValidDeleteError = "Book with id {1} not found!";

        public BaseTest()
        {
            LibraryManagerClient = new LibraryManagerClient();
            CurrentId = 1;
        }

        protected int CurrentId { get; set; } 

        protected LibraryManagerClient LibraryManagerClient  { get; }

        [TearDown]
        protected void TearDown()
        {
            LibraryManagerClient.DeleteBook(CurrentId);
            LibraryManagerClient.DeleteBook(2);
        }

        protected static void AssertSuccessfulResponse(Book expectedBook, Response<Book> actualBookResponse)
        {
            Assert.IsNotNull(actualBookResponse);
            Assert.IsNull(actualBookResponse.Error, $"Adding book failed. {actualBookResponse.Error?.Message}");
            Assert.AreEqual(HttpStatusCode.OK, actualBookResponse.StatusCode);
            Assert.IsNotNull(actualBookResponse.Payload);
            Assert.AreEqual(expectedBook.Id, actualBookResponse.Payload.Id);
            Assert.AreEqual(expectedBook.Author, actualBookResponse.Payload.Author);
            Assert.AreEqual(expectedBook.Title, actualBookResponse.Payload.Title);
            Assert.AreEqual(expectedBook.Description, actualBookResponse.Payload.Description);
        }

        protected void AssertFailingResponse(Response<Book> actualBookResponse, string expectedErrorMessage, HttpStatusCode expectedStatusCode)
        {
            Assert.IsNotNull(actualBookResponse);
            Assert.AreEqual(expectedStatusCode, actualBookResponse.StatusCode);
            Assert.IsNotNull(actualBookResponse.Error);
            Assert.AreEqual(expectedErrorMessage, actualBookResponse.Error.Message);
            Assert.IsNull(actualBookResponse.Payload);
        }
    }
}
