using System;

namespace Domain
{
    public class Tab
    {
        public int Id { get; set; }

        public string AuthorName { get; set; }

        //public int SongId { get; set; }
        //public Song Song { get; set; }

        public string Artist { get; set; }
        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; }
    }
}