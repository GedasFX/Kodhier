using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class News
    {
        public int Id { get; set; }
        public string TitleLt { get; set; }
        public string CaptionLt { get; set; }

        public string TitleEn { get; set; }
        public string CaptionEn { get; set; }

        public bool IsActive { get; set; }

        public string ImagePath { get; set; }

        [ForeignKey("Pizza")]
        public Guid? PizzaId { get; set; }
        public virtual Pizza Pizza { get; set; }
    }
}
