using LibraryManagerTests.Models;
using NUnit.Framework;
using System;
using System.Net;

namespace LibraryManagerTests.Tests
{
    [TestFixture]
    internal class TestDeleteBook : BaseTest
    {
        [Test]
        public void DeleteBook_ShouldPass()
        {
            var bookId = 1;
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);
            var response = LibraryManagerClient.DeleteBook(bookId);

            AssertSucessfulDelitionResponse(response);

            var getBookResponse = LibraryManagerClient.GetBook(bookId);
            AssertFailingResponse(getBookResponse, String.Format(NonExistingIdError, bookId), HttpStatusCode.NotFound);
        }        
        
        [Test]
        public void DeleteBook_When_BookAddedAndUpdated_ShouldPass()
        {
            var bookId = 1000;
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            LibraryManagerClient.UpdateBook(updatedBook, bookId);
            var response = LibraryManagerClient.DeleteBook(bookId);

            AssertSucessfulDelitionResponse(response);
        }

        [Test]
        public void DeleteBook_When_TwoBooksAdded_ShouldPass()
        {
            var bookId1 = 1;
            var book1 = new Book(bookId1, Author, Title, Description);
            LibraryManagerClient.AddBook(book1);            

            var bookId2 = 100;
            var book2 = new Book(bookId2, Author, Title, Description);
            LibraryManagerClient.AddBook(book2);

            var responseDeletedBook1 = LibraryManagerClient.DeleteBook(bookId1);
            AssertSucessfulDelitionResponse(responseDeletedBook1);

            var responseDeletedBook2 = LibraryManagerClient.DeleteBook(bookId2);
            AssertSucessfulDelitionResponse(responseDeletedBook2);
        } 

        [Test] 
        public void DeleteBook_When_TwoBooksAreAddedUpdated_ShouldPass()
        { 
            var bookId1 = 1;
            var book1 = new Book(bookId1, Author, Title, Description);
            LibraryManagerClient.AddBook(book1);

            var bookId2 = 100;
            var book2 = new Book(bookId2, Author, Title, Description);
            LibraryManagerClient.AddBook(book2);

            var updatedBook1 = new Book(bookId1, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            var updatedBook2 = new Book(bookId2, UpdatedAuthor, UpdatedTitle, UpdatedDescription);

            LibraryManagerClient.UpdateBook(updatedBook1, bookId1);
            LibraryManagerClient.UpdateBook(updatedBook2, bookId2);

            var responseDeletedBook1 = LibraryManagerClient.DeleteBook(bookId1);
            AssertSucessfulDelitionResponse(responseDeletedBook1);  
           
            var responseDeletedBook2 = LibraryManagerClient.DeleteBook(bookId2);
            AssertSucessfulDelitionResponse(responseDeletedBook2);
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(10)]
        public void DeleteBook_InvalidOrNonExistingId_ShouldFail(int invalidOrNonExistingId)
        {
            var response = LibraryManagerClient.DeleteBook(invalidOrNonExistingId);

            AssertFailingResponse(response, String.Format(NonExistingIdError, invalidOrNonExistingId), HttpStatusCode.NotFound);
        }
    }
}
