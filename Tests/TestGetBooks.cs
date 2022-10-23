using NUnit.Framework;
using System.Linq;

namespace LibraryManagerTests.Tests
{
    [TestFixture]
    internal class TestGetBooks : BaseTest
    {
        [Test]
        public void GetBooks_When_SearchingByExistingFullTitle_ShouldPass()
        {
            var createdBooks = CreateListOfBooks();
            var expectedBooks = createdBooks.Where(bk => bk.Title == FullTitle).ToList();

            AddBooksToDBLibrary(createdBooks);
            var response = LibraryManagerClient.GetBooks(FullTitle);

            AssertSuccessfulResponse(response);
            AssertGetBooksPayload(response.Payload, expectedBooks);
        }
        
        [TestCase("2")]       
        [TestCase("Title2")]
        public void GetBooks_When_SearchingByExistingPartialTitle_ShouldPass(string parcialTitle)
        {
            var creactedBooks = CreateListOfBooks();
            var expectedBooks = creactedBooks.Where(bk => bk.Title.Contains(parcialTitle)).ToList();
            AddBooksToDBLibrary(expectedBooks);
            var response = LibraryManagerClient.GetBooks(parcialTitle);

            AssertSuccessfulResponse(response);
            AssertGetBooksPayload(response.Payload, expectedBooks);
        }       
       
        [TestCase("")]       
        [TestCase(null)]
        [TestCase(" ")] 
        public void GetBooks_When_BooksAdded_SearchingByEmptyOrWhiteSpaceTitle_ShouldPass(string nonExistingTitle)
        {
            var expectedBooks = CreateListOfBooks();
            AddBooksToDBLibrary(expectedBooks);
            var response = LibraryManagerClient.GetBooks(nonExistingTitle);

            AssertSuccessfulResponse(response);
            AssertGetBooksPayload(response.Payload, expectedBooks);
        }                

        [TestCase(TitleCapitalCase)]
        [TestCase(TitleLowerCase)]
        [TestCase(PartialTitleCapitalCase)]
        [TestCase(PartialTitleLowerCase)]
        public void GetBooks_IsCaseSensitive_ShouldPass(string title)
        {
            var createdBooks = CreateListOfBooks();
            var expectedBooks = createdBooks.Where(bk => bk.Title.Contains(title)).ToList(); 
            AddBooksToDBLibrary(expectedBooks);
            var response = LibraryManagerClient.GetBooks(title);

            AssertSuccessfulResponse(response);
            AssertGetBooksPayload(response.Payload, expectedBooks);
        }  

        [Test]
        public void GetBooks_When_SearchingByTitleContainingNumbers_ShouldPass()
        {
            var createdBooks = CreateListOfBooks();
            var expectedBooks = createdBooks.Where(bk => bk.Title.Contains(TitleContiningNumbers)).ToList();
            AddBooksToDBLibrary(expectedBooks);
            var response = LibraryManagerClient.GetBooks(TitleContiningNumbers);

            AssertSuccessfulResponse(response);
            AssertGetBooksPayload(response.Payload, expectedBooks);
        }

        [TestCase("ComeHere")]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void GetBooks_When_NoBooksAdded_SearchingByNonExistingOrEmptyOrWhiteSpaceTitle_ShouldFail(string nonExistingTitle)
        {
            var response = LibraryManagerClient.GetBooks(nonExistingTitle);
            Assert.Zero(response.Payload.Count);
            AssertSuccessfulResponse(response);

            //What should the Api returns if Search string is empty, null, or whiteSpace When Aplication Contains no Entered books -> will it return No Books available as an answer ?  Why is HttpStatus "NoContent"  not returned when the books are returned?
            // not returned and it is returned HttpStatus Ok?
        }

        [TestCase("ComeHere")]
        public void GetBooks_When_BooksAdded_SearchingByNonExistingTitle_ShouldFail(string nonExistingTitle)
        {
            var createdctedBooks = CreateListOfBooks();
            AddBooksToDBLibrary(createdctedBooks);
            var response = LibraryManagerClient.GetBooks(nonExistingTitle);
            Assert.Zero(response.Payload.Count);
            AssertSuccessfulResponse(response);
        }
    }
}
