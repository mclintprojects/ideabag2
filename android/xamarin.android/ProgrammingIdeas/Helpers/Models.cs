using System.Collections.Generic;

namespace ProgrammingIdeas
{
    /// <summary>
    /// Represents and idea category
    /// </summary>
    public class Category
    {
        public string CategoryLbl { get; set; }
        public int CategoryCount { get; set; }
        public string Description { get; set; }
        public List<Idea> Items { get; set; }
    }

    /// <summary>
    /// Represents an idea
    /// </summary>
    public class Idea
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Difficulty { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public Note Note { get; set; }

        /// <summary>
        /// For use with sorting the ideas by Status [done, inprogress, completed]
        /// </summary>
        /// <returns></returns>
        public int GetStatusId()
        {
            switch (State)
            {
                case Status.Undecided:
                    return 1;

                case Status.InProgress:
                    return 2;

                case Status.Done:
                    return 3;

                default:
                    return 1;
            }
        }

        /// <summary>
        /// For use with sorting the ideas by Status [beginner, intermediate, expert]
        /// </summary>
        /// <returns></returns>
        public int GetDifficultyId()
        {
            switch (Difficulty)
            {
                case DifficultyConsts.Beginner:
                    return 1;

                case DifficultyConsts.Intermediate:
                    return 2;

                case DifficultyConsts.Expert:
                    return 3;

                default:
                    return 1;
            }
        }
    }

    internal struct Status
    {
        public const string InProgress = "inprogress";
        public const string Done = "done";
        public const string Undecided = "undecided";
    }

    internal struct DifficultyConsts
    {
        public const string Beginner = "Beginner";
        public const string Intermediate = "Intermediate";
        public const string Expert = "Expert";
    }

    /// <summary>
    /// Represents an idea's notes
    /// </summary>
    public class Note
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
    }
}