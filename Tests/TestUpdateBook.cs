using LibraryManagerTests.Models;
using NUnit.Framework;
using System;
using System.Net;

namespace LibraryManagerTests.Tests
{
    [TestFixture]
    internal class TestUpdateBook : BaseTest
    {     
        [Test]
        public void UpdateExistingBook_AllFields_ShouldPass()
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(CurrentId, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);

            AssertSuccessfulResponse(updatedBook, updatedBookResponse);
        }

        [Test]
        public void UpdateNonExistingBook_ShouldFail()
        {
            var updatedBook = new Book(CurrentId, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);

            AssertFailingResponse(updatedBookResponse, String.Format(NonExistingIdError, CurrentId), HttpStatusCode.NotFound);
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        public void UpdateBook_InvalidId_ShouldFail(int invalidId)
        {
            var updatedBook = new Book(invalidId, UpdatedDescription, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, invalidId);

            AssertFailingResponse(updatedBookResponse, InvalidIdError, HttpStatusCode.BadRequest);
        }

        [Test]
        public void UpdateBook_AllFieldsInvalid_ShouldFail()
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(++CurrentId, InvalidAuthor31CharsLength, InvalidTitle101CharsLength, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);
            
            //There is no Id with value = 2 and updatedAuthor31CharsLength contains 31chars that is invalid
            //there is not ErrorMesage thrown for Book with Id = 2 and Author with length 31 Chars.
            //There is only an ErrorMessage thrown for property UpdatedTitle101CharsLength which is 101 (invalid)     characters.
            //Is it ok from 3 Properies with invalid values to throw an ErrorMessage only for one of them
            //(UpdatedTitle101CharsLength) and for the other not?.
            //description field is correct -> there is no litmitations for it

            AssertFailingResponse(updatedBookResponse, InvalidTitleError, HttpStatusCode.BadRequest);
        }

        [TestCase(null, AuthorRequiredError)]
        [TestCase("", AuthorRequiredError)]
        [TestCase(" ", AuthorRequiredError)]
        [TestCase(InvalidAuthor31CharsLength, TooLongAuthorError)]
        public void UpdateBook_InvalidAuthorCountSymbols_ShouldFail(string updatedAuthor, string expectedErrorMessage)
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(CurrentId, updatedAuthor, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);
            
            // log as a Bug -> ErrorMessage:
            //String lengths are both 61.Strings differ at index 50.
            //Expected: "Book.Author is a required field.\r\nParameter name: Book.Author"
            //But was:  "Book.Author is a required field.\r\nParameter name: book.Author" //book should be capital "B"

            AssertFailingResponse(updatedBookResponse, expectedErrorMessage, HttpStatusCode.BadRequest);
        }

        [TestCase(ValidAuthor1CharsLength)]
        [TestCase(ValidAuthor30CharsLength)]  
        public void UpdateBook_ValidAuthorCountSymbols_ShouldPass(string updatedAuthor)
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(CurrentId, updatedAuthor, Title, Description);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);
           
            // when i pass 30 characters long Author, throws Error with ErrorMessage,
            // that Authour should not exeed 30 characters, which is not correct behaviour. It should not throw Error.
            // Validation for 30 chars is not correctly working here. By requirement Author validation should throw //Error if Author length exeeds 30 symbols ie is more than 30 symbols.

             AssertSuccessfulResponse(updatedBook, updatedBookResponse);
        }

        [TestCase("", TitleRequiredError)]
        [TestCase(null, TitleRequiredError)]
        [TestCase(" ", TitleRequiredError)]
        [TestCase(InvalidTitle101CharsLength, TooLongTitleError)]
        public void UpdateBook_InvalidTitle_ShouldFail(string updatedTitle, string expectedTitleError)
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(CurrentId, Author, updatedTitle, Description);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);
            
            AssertFailingResponse(updatedBookResponse, expectedTitleError, HttpStatusCode.BadRequest);
        }

        [TestCase(ValidTitle1CharsLength)]
        [TestCase(ValidTitle100CharsLength)]
        public void UpdateBook_ValidTitleCountSymbols_ShouldPass(string updatedTilte)
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(CurrentId, Author, updatedTilte, Description);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);
            
            //validation for Title for 100 symbols throws an Error and ErrorMessage, but it should not, becasue by
            //requirements Title is not longer than 100. If we pass Title length 101 Symbols it should throw Error.

            AssertSuccessfulResponse(updatedBook, updatedBookResponse);
        }        

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateBook_EmptyDescription_ShouldPass(string emptyString) 
        {
            var book = new Book(CurrentId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(CurrentId, UpdatedAuthor, UpdatedTitle, emptyString);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, CurrentId);

            AssertSuccessfulResponse(updatedBook, updatedBookResponse);
        }        
    }
}
