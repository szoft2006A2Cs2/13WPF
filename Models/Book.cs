using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Models
{
    [Table("book")]
    public class Book
    {
        public Book()
        {
            this.BookAuthors = new HashSet<BookAuthor>();
            this.BookGenres = new HashSet<BookGenre>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public float Rating { get; set; }
        public string Edition { get; set; }
        public string Language { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
        public virtual ICollection<BookGenre> BookGenres { get; set; }
    }
}
