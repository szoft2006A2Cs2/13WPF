using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Models
{
    [Table("book_author")]
    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        public virtual Book Book { get; set; }
        public virtual Author Author { get; set; }
    }
}
