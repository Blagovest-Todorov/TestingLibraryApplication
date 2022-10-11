using LibraryManagerTests.Models;
using NUnit.Framework;
using System;
using System.Net;

namespace LibraryManagerTests.Tests
{
    internal class TestDeleteBook : BaseTest
    {
        [Test]
        public void DeleteBook_ShouldPass() 
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);           
            // Is IT Correct structured and written ?
            var response = LibraryManagerClient.DeleteBook(CurrentId);

            Assert.IsNotNull(response);
            Assert.IsNull(response.Payload);
            Assert.IsNull(response.Error);
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.IsNull(response.Error?.Message);

            var expectedBooks = 0;
            var actualAfterDelitionBooksCount = LibraryManagerClient.GetBooks().Payload.Count;
            Assert.AreEqual(expectedBooks, actualAfterDelitionBooksCount);            
        }

        [Test]
        public void Delete_NonExistingBook_ShouldFail()
        {      
            int expectedCountOfBooks = 0;
            int actualCountOfBooks = LibraryManagerClient.GetBooks().Payload.Count;            
            var response = LibraryManagerClient.DeleteBook(CurrentId);           

            Assert.AreEqual(expectedCountOfBooks, actualCountOfBooks);
            AssertFailingResponse(response, String.Format(NonExistingIdError, CurrentId), HttpStatusCode.NotFound);
        }

        [TestCase(int.MinValue)]
        [TestCase(0)]
        public void DeleteBook_InvalidId_ShouldFail(int invalidId) 
        {
            var response = LibraryManagerClient.DeleteBook(invalidId);            

            AssertFailingResponse(response, String.Format(NonExistingIdError, invalidId), HttpStatusCode.NotFound);
        }
    }
}
