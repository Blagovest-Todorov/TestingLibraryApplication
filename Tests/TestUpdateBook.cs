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
        public void Update_ExistingBook_AllFields_ShouldPass()
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            AssertSuccessfulResponse(updatedBook, updatedBookResponse);        
        }

        [TestCase(ValidAuthor1CharsLength)]
        [TestCase(ValidAuthor30CharsLength)]
        public void UpdateBook_ValidAuthor_ShouldPass(string updatedAuthor)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, updatedAuthor, Title, Description);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            //Bug9 In response an error Message for 30 chars long Author is thrown. By requirements Author validation should throw Error Message if passed Author length exceeds 30 symbols ie is more than 30 symbols.
            //expected when we update a book with Author with 30 chars the title should be updated.
            //actual error message is appearing and title is not updated, that is incorrect according to requirements

            AssertSuccessfulResponse(updatedBook, updatedBookResponse);
        }

        [TestCase(ValidTitle1CharsLength)]
        [TestCase(ValidTitle100CharsLength)]
        public void UpdateBook_ValidTitle_ShouldPass(string updatedTilte)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, Author, updatedTilte, Description);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            //Bug10 In response when the title is updated with Title containing 100 symbols , it throws an Error and ErrorMessage, but it should not according requirements. 
            //Expected: if we update a title to 100 chars, it should be updated and no Error message to appear.
            //If we pass Title length 101 Symbols then it should throw Error message and title not updated.

            AssertSuccessfulResponse(updatedBook, updatedBookResponse);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateBook_EmptyDescription_ShouldPass(string emptyString)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, Author, Title, emptyString);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            AssertSuccessfulResponse(updatedBook, updatedBookResponse);
        }

        [Test]
        public void Update_NonExistingBook_ShouldFail()
        {
            var bookId = 1;            
            var updatedBook = new Book(bookId, UpdatedAuthor, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            AssertFailingResponse(updatedBookResponse, String.Format(NonExistingIdError, bookId), HttpStatusCode.NotFound);
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        public void UpdateBook_InvalidId_ShouldFail(int invalidId)
        {
            var bookId = invalidId;
            IdCollection.Add(bookId);
            var updatedBook = new Book(invalidId, UpdatedDescription, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, invalidId);

            AssertFailingResponse(updatedBookResponse, InvalidIdError, HttpStatusCode.BadRequest);
        }

        [Test]
        public void UpdateBook_AllFieldsInvalid_ShouldFail()
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);            
           
            var updatedBook = new Book(int.MinValue, InvalidAuthor31CharsLength, InvalidTitle101CharsLength, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            //Bug7 The updatedAuthor31CharsLength contains 31chars, that is invalid, but no error is thrown for it ErrorMessage is thrown for property UpdatedTitle101CharsLength which is 101 (invalid) characters, and for property Id = int.MinValue (invalid), that is correct. But is it ok from 3 Properies with invalid values to throw an ErrorMessage only for two of them ? For invalid Author - there is no errormessage. Description field is correct, there is no litmitations for it.
            //Message:
            //Expected string length 72 but was 62.Strings differ at index 5.
            //Expected: "Book.Title should not exceed 100 characters!\r\nParameter name:..."
            //But was:  "Book.Id should be a positive integer!\r\nParameter name: book.Id"

            AssertFailingResponse(updatedBookResponse, InvalidTitleError, HttpStatusCode.BadRequest);
        }

        [Test]
        public void UpdateBook_ValidIdAndDescritpionAndInvalidAuthorAndTitle_ShouldFail()
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, InvalidAuthor31CharsLength, InvalidTitle101CharsLength, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);

            //Bug8 The updatedAuthor31CharsLength contains 31chars, that is invalid - but no error is thrown for it.ErrorMessage is thrown for property UpdatedTitle101CharsLength which is 101 (invalid) characters, that is correct. But is it ok from 2 Properies with invalid values to throw an ErrorMessage only for one of them. UpdatedTitle101CharsLength and for InvalidAuthor31CharsLength not ?
            // Id is correct, description field is correct, there is no litmitations for it.            

            AssertFailingResponse(updatedBookResponse, InvalidTitleError, HttpStatusCode.BadRequest);
        }

        [TestCase(null, AuthorRequiredError)]
        [TestCase("", AuthorRequiredError)]
        [TestCase(" ", AuthorRequiredError)]
        [TestCase(InvalidAuthor31CharsLength, TooLongAuthorError)]
        public void UpdateBook_InvalidAuthor_ShouldFail(string author, string expectedErrorMessage)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);            
           
            var updatedBook = new Book(bookId, author, UpdatedTitle, UpdatedDescription);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);
            //repeating Bug as Bug6 in previous cases ErrorMessage:
            //String lengths are both 61.Strings differ at index 50.
            //Expected: "Book.Author is a required field.\r\nParameter name: Book.Author"
            //But was:  "Book.Author is a required field.\r\nParameter name: book.Author" //book should be capital "B" and the  "." should it be a "!" for consistency with other error messages.

            AssertFailingResponse(updatedBookResponse, expectedErrorMessage, HttpStatusCode.BadRequest);
        }        

        [TestCase("", TitleRequiredError)]
        [TestCase(null, TitleRequiredError)]
        [TestCase(" ", TitleRequiredError)]
        [TestCase(InvalidTitle101CharsLength, TooLongTitleError)]
        public void UpdateBook_InvalidTitle_ShouldFail(string updatedTitle, string expectedTitleError)
        {
            var bookId = 1;
            IdCollection.Add(bookId);
            var book = new Book(bookId, Author, Title, Description);
            LibraryManagerClient.AddBook(book);

            var updatedBook = new Book(bookId, Author, updatedTitle, Description);
            var updatedBookResponse = LibraryManagerClient.UpdateBook(updatedBook, bookId);
            
            AssertFailingResponse(updatedBookResponse, expectedTitleError, HttpStatusCode.BadRequest);
        }        
    }
}
