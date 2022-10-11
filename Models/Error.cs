namespace LibraryManagerTests.Models
{
    public class Error
    {
        // done by me ctor
        public Error(string someMEssage)
        {
            this.Message = someMEssage;
        }

        public string Message { get; set; }
    }
}
