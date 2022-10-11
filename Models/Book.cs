namespace LibraryManagerTests.Models
{
    public class Book
    {
        //ctor -> created a constructor / moved above the properties 
        // should Id prop be acessed and set by outside  ??? i think no
        public Book(int id, string author, string title, string description)
        {
            this.Id = id;
            this.Author = author;
            this.Title = title;
            this.Description = description;
        }

        public int Id { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }

    }
}
