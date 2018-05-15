using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Pizza")]
        public Guid? PizzaId { get; set; }
        public virtual Pizza Pizza { get; set; }
    }
}
