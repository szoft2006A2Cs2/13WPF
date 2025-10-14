using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Models
{
    [Table("author")]
    internal class Author : Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role {  get; set; }

        public Author() { }
    }
}
