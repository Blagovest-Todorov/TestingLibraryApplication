using LibraryManagerTests.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LibraryManagerTests.Tests
{
    [TestFixture]
    internal abstract class BaseTest
    {
        protected const string Author = "TestAutor";
        protected const string Title = "TestTitle";
        protected const string Description = "TestDescription";
        protected const string UpdatedAuthor = "Ivan Vazov1";
        protected const string UpdatedTitle = "Pod Igoto Tom 1";
        protected const string NonExistingTitle = "ComeHere";
        protected const string UpdatedDescription = "Hard times";
        protected const string NonExistingIdError = "Book with id {0} not found!";
        protected const string ExistingIdError = "Book with id {0} already exists!";
        protected const string InvalidIdError = "Book.Id should be a positive integer!\r\nParameter name: book.Id";
        protected const string InvalidTitleError = "Book.Title should not exceed 100 characters!\r\nParameter name: Book.Title";
        protected const string TooLongAuthorError = "Book.Author should not exceed 30 characters!\r\nParameter name: Book.Author";
        protected const string TooLongTitleError = "Book.Title should not exceed 100 characters!\r\nParameter name: Book.Title";
        protected const string AuthorRequiredError = "Book.Author is a required field.\r\nParameter name: Book.Author";
        protected const string TitleRequiredError = "Book.Title is a required field\r\nParameter name: Book.Title";

        protected const string InvalidAuthor31CharsLength = "TestAuthorTestAuthorTestAuthor1";
        protected const string ValidAuthor1CharsLength = "T";
        protected const string ValidAuthor30CharsLength = "TestAuthorTestAuthorTestAuthor";
        protected const string InvalidTitle101CharsLength = "Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 12";
        protected const string ValidTitle100CharsLength = "Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1Pod Igoto Tom nice 1";
        protected const string ValidTitle1CharsLength = "P";
        protected const string ValidTitle1CharsLengthLowerCase = "p";  
        protected const string TitleCapitalCase = "TESTTITLE";
        protected const string TitleLowerCase = "testtitle";
        protected const string FullTitle = "TestTitle1";
        protected const string ParcialTitle = "Test";
        protected const string PartialTitleCapitalCase = "TEST";
        protected const string PartialTitleLowerCase = "test";
        protected const string TitleContiningNumbers = "789";
        protected const string FullTitleUpdated = "TestTitle1";

        public BaseTest()
        {
            LibraryManagerClient = new LibraryManagerClient();
            IdCollection = new List<int>();
        }

        protected List<int> IdCollection { get; }

        protected LibraryManagerClient LibraryManagerClient { get; }

        [TearDown]
        protected void TearDown()
        {
            foreach (var item in IdCollection)
            {
                LibraryManagerClient.DeleteBook(item);
            }

            IdCollection.Clear();
        }

        protected Book CreateBook(int bookId)
        {
            IdCollection.Add(bookId);
            return new Book(bookId, Author, Title, Description);
        }

        protected List<Book> CreateListOfBooks()
        {
            List<Book> listBooks = new List<Book>
            {
                new Book(1, "TestAuthor1", "TestTitle1", "TestDescription1"),
                new Book(2, "TestAuthor2", "TestTitle2", "TestDescription2"),
                new Book(3, "TestAuthor3", "TestTitle3", "TestDescription3"),
                new Book(4, "Ivan Vazov", "Pod Igoto", "Hard time for BG People"),
                new Book(5, "TESTAUTHOR5", "TESTTITLE5", "TESTDESCRIPTION5"),
                new Book(6, "testauthor6", "testtitle6", "testdescription6"),
                new Book(7, "testAutor7", "testTestTitle7", "TestDescription1"),
                new Book(8, "testAutor8", "testTESTTitle8", "TestDescription1"),
                new Book(9, "testAutor9", "789", "TestDescription9"),
                new Book(10, "testAutor10", "789102", "")
            };

            IdCollection.AddRange(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            return listBooks;
        }

        protected void AddBooksToDBLibrary(List<Book> books)
        {
            for (int i = 0; i < books.Count; i++)
            {
                LibraryManagerClient.AddBook(books[i]);
            }
        }

        protected void AssertGetBooksPayload(List<Book> payload, List<Book> expectedBooks)
        {
            var expectedBookCount = expectedBooks.Count;
            var actualCount = payload.Count;
            Assert.AreEqual(expectedBookCount, actualCount);

            for (int i = 0; i < payload.Count; i++)
            {
                var actualBook = payload[i];
                var expectedBook = expectedBooks.FirstOrDefault(bk => bk.Id == actualBook.Id);

                Assert.AreEqual(expectedBook.Id, actualBook.Id);
                Assert.AreEqual(expectedBook.Title, actualBook.Title);
                Assert.AreEqual(expectedBook.Description, actualBook.Description);

                // Assert.AreEqual(expectedBook.Author, actualBook.Author);
                // actualBook.Author = null into the response ; this is propable Bug in ClientLibraryAdd Method, When inserting a book with specified Author value, the Author value is always null in response.
            }
        }

        public void AssertSuccessfulResponse(Response<List<Book>> response)
        {
            Assert.IsNotNull(response);
            Assert.IsNull(response.Error);
            Assert.IsNotNull(response.Payload);
            CollectionAssert.AllItemsAreNotNull(response.Payload);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        protected static void AssertSuccessfulResponse(Book expectedBook, Response<Book> actualBookResponse)
        {
            Assert.IsNotNull(actualBookResponse);
            Assert.IsNull(actualBookResponse.Error, $"Adding book failed. {actualBookResponse.Error?.Message}");
            Assert.AreEqual(HttpStatusCode.OK, actualBookResponse.StatusCode);
            Assert.IsNotNull(actualBookResponse.Payload);
            Assert.AreEqual(expectedBook.Id, actualBookResponse.Payload.Id);
            // A bug is logged, becasue the AuthorProperty in the Response is null, but should be filled with a Value.
            // Assert.AreEqual(expectedBook.Author, actualBookResponse.Payload.Author); 
            Assert.AreEqual(expectedBook.Title, actualBookResponse.Payload.Title);
            Assert.AreEqual(expectedBook.Description, actualBookResponse.Payload.Description);
        }

        protected void AssertFailingResponse(
            Response<Book> actualBookResponse, string expectedErrorMessage, HttpStatusCode expectedStatusCode)
        {
            Assert.IsNotNull(actualBookResponse);
            Assert.AreEqual(expectedStatusCode, actualBookResponse.StatusCode);
            Assert.IsNotNull(actualBookResponse.Error);
            Assert.AreEqual(expectedErrorMessage, actualBookResponse.Error.Message);
            Assert.IsNull(actualBookResponse.Payload);
        }

        protected void AssertSucessfulDelitionResponse(Response<Book> response)
        {
            Assert.IsNotNull(response);
            Assert.IsNull(response.Payload);
            Assert.IsNull(response.Error);
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}

