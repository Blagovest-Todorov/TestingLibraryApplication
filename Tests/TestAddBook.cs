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
            var book = new Book(CurrentId, Author, Title, Description);
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            //Bug: AuthorProperty value in the response is null, but shoud be Ivan Vazov
            //var book = new Book(currentId, "Ivan Vazov", "Pod Igoto", "Hard time for BG People");
            //We compare obj book with the response.The response Author is null, whick is bug.

            AssertSuccessfulResponse(book, createdBookResponse);
        }

        [Test]
        public void AddTwoBooks_AllFields_ShouldPass()
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var bookSecond = new Book(CurrentId + 1, Author, Title, Description);
            var createdSecondBookResponse = LibraryManagerClient.AddBook(bookSecond);

            AssertSuccessfulResponse(bookSecond, createdSecondBookResponse);

            // in bookSecond the property Author is "Ivan Vazov" , in object createdSecondBookResponse property Author is null.
        }

        [Test]
        public void AddBook_SameIdTwice_ShouldFail()
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);            
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(createdBookResponse, String.Format(ExistingIdError, CurrentId), HttpStatusCode.BadRequest);            
        }        

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        public void AddABook_InvalidId_ShouldFail(int invalidId)
        {      
            var book = new Book(invalidId, Author, Title, Description);
            var createdBookResponse = LibraryManagerClient.AddBook(book);            
            
            AssertFailingResponse(createdBookResponse, InvalidIdError, HttpStatusCode.BadRequest);
        }  

        [TestCase("", AuthorRequiredError) ]
        [TestCase(" ", AuthorRequiredError) ]
        [TestCase(null, AuthorRequiredError)]
        [TestCase(InvalidAuthor31CharsLength, TooLongAuthorError)]
        public void AddABook_InvalidAuthor_ShouldFail(string invalidAuthor, string errorMessage)   
        {
            var book = new Book(CurrentId, invalidAuthor, Title, Description);
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(createdBookResponse, errorMessage, HttpStatusCode.BadRequest);
            //AuthorProperty is it Required ?
            //When we pass AuthorProperty the value "", null, White space
            //The response Error is:
            // Message:
            // String lengths are both 61.Strings differ at index 50.
            // Expected: "Book.Author is a required field.\r\nParameter name: Book.Author"
            // But was: "Book.Author is a required field.\r\nParameter name: book.Author"
            // book.Author must me with Capital "B"
            //When AuthorProperty is AthorProperty is 31Symbols test passes, becasue validation works.
        }

        [TestCase(ValidAuthor30CharsLength)]
        [TestCase(ValidAuthor1CharsLength)]
        public void AddBook_ValidAuthor_ShouldPass(string validAuthor) 
        {
            var book = new Book(CurrentId, validAuthor, Title, Description);
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            AssertSuccessfulResponse(book, createdBookResponse);

            //Bug: Entering book with Author 30 Symbols length should be allowed according to the requirements.
            //In practice the responseObject has Error, ErrorMessage ->Book.Author should not exceed 30 characters!, the payload is Empty, the Response HttpStatuscode is BadRequest, object book is not present inot the response, which is all wrong.
            //It should be Error is Empty, payload is full, and HttpStatusCode is "OK".book should be present in Response.
            // Entering a book with  Author Length 1 Symbol. Into the Response the Author is with nullValue, it should be Author with Value "I" ;
        }

        [TestCase("", TitleRequiredError)]
        [TestCase(" ", TitleRequiredError)]
        [TestCase(null, TitleRequiredError)]
        [TestCase(InvalidTitle101CharsLength, TooLongTitleError)]
        public void AddABook_InvalidTitleCharsCount_ShouldFail (string invalidTitle, string expectedError)
        {
            var book = new Book(CurrentId, Author, invalidTitle, Description);
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(createdBookResponse, expectedError, HttpStatusCode.BadRequest);
        }

        [TestCase(ValidTitle100CharsLength)]
        [TestCase(ValidTitle1CharsLength)]
        public void AddBook_ValidTitle_ShouldPass(string validTitle)
        {
            var book = new Book(CurrentId, Author, validTitle, Description);
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            AssertSuccessfulResponse(book, createdBookResponse);

            //Adding book with Title length 100 chars is possible accroding to the requirements.
            //The Response with Title length is 100 Chars is not added, which is not correct.
            //There is Error, Error Message(Adding book failed, Book.Title should not exceed 100 characters!),
            //HttpStatusCode is "BadRequest" which not correct.
            //Book with Title Lenght 100 should be possible present/ added into the Response, HttpStatuscode should be "OK".
            //Error must be null.
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void AddABook_NoDescriptionOrWhiteSpace_ShouldPass(string emptyDescrtiption) 
        {
            var book = new Book(CurrentId, Author, Title, emptyDescrtiption);
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            AssertSuccessfulResponse(book, createdBookResponse );
            //In the Document there is no limitation of the Description fieled length.
            //Would not it better we to have some field length limitation here ?
        }

        [Test]  // how to write such a test ?
        public void AddBook_AllFieldsInvalid_ShouldFail()
        {
            var book = new Book(int.MinValue, InvalidAuthor31CharsLength, InvalidTitle101CharsLength, Description);
            var createdBookResponse = LibraryManagerClient.AddBook(book);

            AssertFailingResponse(createdBookResponse, InvalidTitle101CharsLength, HttpStatusCode.BadRequest);
            // From the 3 Book Properies it throws Error only on invalid Title Length -> InvalidTitle101CharsLength, 
            // would it be better to throw error for the invalid Id, and Invalid Author ?
            // for Descripion we dont have limitations but we should ?

           // AssertFailingResponse(createdBookResponse, InvalidAuthor31CharsLength, HttpStatusCode.BadRequest);

        }
    }
}
