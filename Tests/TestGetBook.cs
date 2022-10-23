using LibraryManagerTests.Models;
using NUnit.Framework;
using System;
using System.Net;

namespace LibraryManagerTests.Tests
{
    [TestFixture]
    internal class TestGetBook : BaseTest
    {
        [Test]
        public void GetBook_Existing_ShouldPass() 
        {
            var bookId = 10;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);
            var response = LibraryManagerClient.GetBook(bookId);

            // Bug -> AuthorProperty in the response is null, should it be "Ivan Vazov" 

            AssertSuccessfulResponse(book, response);
        }                

        [Test]
        public void GetBook_When_TwoBooksAreAdded_ShouldPass()
        {
            var bookId1 = 90000;
            IdCollection.Add(bookId1);
            var book1 = new Book(bookId1, Author, Title, Description);
            var responseBook1 = LibraryManagerClient.AddBook(book1);

            var bookId2 = 90;
            IdCollection.Add(bookId2);
            var book2 = new Book(bookId2, Author, Title, Description);
            var responseBook2 = LibraryManagerClient.AddBook(book2);

            AssertSuccessfulResponse(book1, responseBook1);
            AssertSuccessfulResponse(book2, responseBook2);
        }

        [Test]
        public void GetBook_When_BookIsAddedUpdated_ShouldPass()
        {
            var bookId = 90000;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            //Description is actualized = "Hard times" in the responseUpdatedBook, so it should be updated in the response.
            var responseUpdatedBook = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            //Bug11 When we get the book by Id, in the response is staying "TestDescription" but is should stay "Hard times" field description is not showing the updated value.
            var responseGetBook = LibraryManagerClient.GetBook(bookId);

            AssertSuccessfulResponse(updatedBook, responseGetBook);
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(100)]
        public void GetBook_InvalidOrNonExistingId_ShouldFail(int invalidOrNonExistingId)
        {
            var response = LibraryManagerClient.GetBook(invalidOrNonExistingId);
            AssertFailingResponse(response, String.Format(NonExistingIdError, invalidOrNonExistingId), HttpStatusCode.NotFound);
        }

        [Test]
        public void GetBook_When_BookIsAddedThenDeleted_ShouldFail() 
        {
            var bookId = 90000;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);
            LibraryManagerClient.DeleteBook(bookId);
            var response = LibraryManagerClient.GetBook(bookId);          

            AssertFailingResponse(response, String.Format(NonExistingIdError, bookId), HttpStatusCode.NotFound);  
        }       

        [Test]
        public void GetBook_When_BookIsAddedUpdatedDeleted_ShouldFail()
        {
            var bookId = 90000;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, UpdatedAuthor, UpdatedTitle, UpdatedDescription);            
            LibraryManagerClient.UpdateBook(updatedBook, bookId);
            LibraryManagerClient.DeleteBook(bookId);
            var responseGetBook = LibraryManagerClient.GetBook(bookId);

            AssertFailingResponse(responseGetBook, String.Format(NonExistingIdError, bookId), HttpStatusCode.NotFound);
        }        

        [Test]
        public void GetBook_When_TwoBooksAreAddedDeleted_ShouldFail()
        {
            int bookId1 = 10000;            
            var book = CreateBook(bookId1);
            LibraryManagerClient.AddBook(book);

            var bookId2 = 100001;            
            var book2 = CreateBook(bookId2);
            LibraryManagerClient.AddBook(book2);

            LibraryManagerClient.DeleteBook(bookId1);
            LibraryManagerClient.DeleteBook(bookId2);

            var responseBook1 = LibraryManagerClient.GetBook(bookId1);
            var responseBook2 = LibraryManagerClient.GetBook(bookId2);

            AssertFailingResponse(responseBook1, String.Format(NonExistingIdError, bookId1), HttpStatusCode.NotFound);
            AssertFailingResponse(responseBook2, String.Format(NonExistingIdError, bookId2), HttpStatusCode.NotFound);
        }        

        [Test]
        public void GetBook_When_TwoBooksAddedUpdatedDeleted_ShoulFail() 
        {
            var bookId1 = 90000;
            IdCollection.Add(bookId1);
            var book1 = CreateBook(bookId1);
            LibraryManagerClient.AddBook(book1);

            var bookId2 = 90;
            IdCollection.Add(bookId2);
            var book2 = CreateBook(bookId1);
            LibraryManagerClient.AddBook(book2);

            var firstBookUpdated = new Book(bookId1, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            LibraryManagerClient.UpdateBook(firstBookUpdated, bookId1);
            var secondBookUpdated = new Book(bookId2, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            LibraryManagerClient.UpdateBook(secondBookUpdated, bookId2);

            LibraryManagerClient.DeleteBook(bookId1);
            LibraryManagerClient.DeleteBook(bookId2);

            var responseGetFirstBook = LibraryManagerClient.GetBook(bookId1);
            var responseGetSecBook = LibraryManagerClient.GetBook(bookId2);

            AssertFailingResponse(responseGetFirstBook, String.Format(NonExistingIdError, bookId1), HttpStatusCode.NotFound);
            AssertFailingResponse(responseGetSecBook, String.Format(NonExistingIdError, bookId2), HttpStatusCode.NotFound);
        }        
    }
}
