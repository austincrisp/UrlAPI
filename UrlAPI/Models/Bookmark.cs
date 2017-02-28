using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UrlAPI.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public bool MakePublic { get; set; }

        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        protected virtual ApplicationUser Owner { get; set; }
    }
}