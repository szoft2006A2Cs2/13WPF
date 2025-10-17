using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Models
{
    [Table("genre")]
    public class Genre
    {
        public Genre()
        {
            this.BookGenres = new HashSet<BookGenre>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<BookGenre> BookGenres { get; set; }
    }
}
