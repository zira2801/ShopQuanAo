namespace ShopQuanAo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("link")]
    public partial class link
    {
        [Key]
        public int ID { get; set; }

        public string slug { get; set; }

        public int tableId { get; set; }

        public string type { get; set; }

        public int parentId { get; set; }
    }
}
