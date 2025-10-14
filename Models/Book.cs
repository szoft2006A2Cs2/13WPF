using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Models
{
    [Table("book")]
    internal class Book : Model
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public float Rating { get; set; }
        public string Edition { get; set; }
        public string Language { get; set; }
        public Book() { }

        public override string ToString()
        {
            return $"{this.Id}, {this.Title}, {this.Rating}, {this.Edition}, {this.Language}";
        }
    }
}
