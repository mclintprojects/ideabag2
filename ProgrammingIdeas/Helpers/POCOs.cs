using System.Collections.Generic;

namespace ProgrammingIdeas
{
    public class Category
    {
        public string CategoryLbl { get; set; }
        public int CategoryCount { get; set; }
        public string Description { get; set; }
        public List<CategoryItem> Items { get; set; }
    }

    public class CategoryItem
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string Difficulty { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public Note Note { get; set; }
    }

    public class Note
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
    }
}