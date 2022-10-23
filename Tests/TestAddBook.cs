using LibraryManagerTests.Models;
using NUnit.Framework;
using System;
using System.Net;

namespace LibraryManagerTests.Tests
{
    [TestFixture]
    internal class TestAddBook : BaseTest
    {
        [Test]
        public void AddBook_AllFields_ShouldPass()
        {
            var bookId = 1;
            IdCollection.Add(bookId);            
            var book = new Book(bookId, Author, Title, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            //probable Bug1: AuthorProperty value in the response is null, but should be TestAuthor
            
            AssertSuccessfulResponse(book, addedBookResponse);

            //Author property is commented "//" inside AssertSuccessfulResponse method to be able to test other Assertions
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void AddBook_NoDescriptionOrWhiteSpace_ShouldPass(string missingDescrtiption)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, missingDescrtiption);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertSuccessfulResponse(book, addedBookResponse);

            //Probable Bug2 - In the Requirements there is no limitation of the Description field length.
            //Would it be better we have some field length limitation here ?
        }

        [TestCase(ValidAuthor30CharsLength)]
        [TestCase(ValidAuthor1CharsLength)]
        public void AddBook_ValidAuthor_ShouldPass(string validAuthor)
        {
            var bookId = 500;
            IdCollection.Add(bookId);
            var book = new Book(bookId, validAuthor, Title, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertSuccessfulResponse(book, addedBookResponse);

            //probable Bug3: Entering book with Author 30 Symbols length should be allowed according to the requirements, but it is not. As a result the book with Author in length 30 chars is not present into the response, but it should be present.           
            // Entering a book with  Author Length 1 Symbol. Into the Response the Author is with null Value, it should be Author with Value "I" (one symbol length);
        }

        [Test]
        public void AddTwoBooks_AllFields_ShouldPass()
        {
            var bookId1 = 100;
            IdCollection.Add(bookId1);
            var book1 = new Book(bookId1, Author, Title, Description);
            LibraryManagerClient.AddBook(book1);

            var bookId2 = 2;
            IdCollection.Add(bookId2);
            var bookSecond = new Book(bookId2, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            var addedBookResponse = LibraryManagerClient.AddBook(bookSecond);

            AssertSuccessfulResponse(bookSecond, addedBookResponse);
        }

        [Test]
        public void AddBook_SameAuthorTitleDescriptionTwice_ShouldPass()
        {
            var firstBookId = 1;
            IdCollection.Add(firstBookId);
            var firstBook = new Book(firstBookId, Author, Title, Description);
            LibraryManagerClient.AddBook(firstBook);

            var secondBookId = 100;
            IdCollection.Add(secondBookId);
            var secondBook = new Book(secondBookId, Author, Title, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(secondBook);

            AssertSuccessfulResponse(secondBook, addedBookResponse);

            // repeating bug as before here : Propery Author in response is null, should be Author = "TestAutor".
            // One Assertion is commented into AssertSuccessfulResponse ();
        }

        [Test]
        public void AddBook_SameIdTwice_ShouldFail()
        {
            var bookId = 1000;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(addedBookResponse, String.Format(ExistingIdError, bookId), HttpStatusCode.BadRequest);
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        public void AddBook_InvalidId_ShouldFail(int invalidId)
        {
            var bookId = invalidId;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(addedBookResponse, InvalidIdError, HttpStatusCode.BadRequest);
        }

        [TestCase("", AuthorRequiredError)]
        [TestCase(" ", AuthorRequiredError)]
        [TestCase(null, AuthorRequiredError)]
        [TestCase(InvalidAuthor31CharsLength, TooLongAuthorError)]
        public void AddBook_InvalidAuthor_ShouldFail(string invalidAuthor, string errorMessage)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, invalidAuthor, Title, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(addedBookResponse, errorMessage, HttpStatusCode.BadRequest);

            //Bug4      AuthorProperty is it Required ?
            //When we pass AuthorProperty the value "", null, White space
            //The response Error message is:            
            // String lengths are both 61.Strings differ at index 50.
            // Expected: "Book.Author is a required field.\r\nParameter name: Book.Author"
            // But was: "Book.Author is a required field.\r\nParameter name: book.Author"
            // book.Author must me with Capital "B" probably there should be "!" instead of "." symbol
            // after field for consistency with the other errror messages.
        }

        [TestCase("", TitleRequiredError)]
        [TestCase(" ", TitleRequiredError)]
        [TestCase(null, TitleRequiredError)]
        [TestCase(InvalidTitle101CharsLength, TooLongTitleError)]
        public void AddABook_InvalidTitle_ShouldFail(string invalidTitle, string expectedError)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, invalidTitle, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(addedBookResponse, expectedError, HttpStatusCode.BadRequest);
        }

        [TestCase(ValidTitle100CharsLength)]
        [TestCase(ValidTitle1CharsLength)]
        public void AddBook_ValidTitle_ShouldPass(string validTitle)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, validTitle, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertSuccessfulResponse(book, addedBookResponse);

            //Bug 5 AddingBookTitleWithLength100CharsIsNotPossible. When we enter a book with title with length 100chars appears to be an invalid operation.The title is not added/present and the response  and the response gives an Error Message(Adding book failed, Book.Title should not exceed 100 characters!) which not correct.Payload is null, Bad request. According to the requirements a Book with Title length 100 chars should be added and present into the Response, HttpStatuscode should be "OK" and Error must be null (No error message should appear in response).
        }       

        [Test] 
        public void AddBook_AllFieldsInvalid_ShouldFail()
        {
            var book = new Book(int.MinValue, InvalidAuthor31CharsLength, InvalidTitle101CharsLength, Description);
            var addedBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(addedBookResponse, InvalidIdError, HttpStatusCode.BadRequest);
            // Bug6: From the 3 invalid Book Properies it throws Error only on invalid Id  ->  
            // (would it be better to throw error for the invalid Title, and Invalid Author too ? )          
            // into the error Message "Book.Id should be a positive integer!\r\nParameter name: book.Id" ,
            // book.Id should be corrected to Book.Id for consistency to be one format everywhere.         
        }     
    }
}
