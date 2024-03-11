﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopQuanAo.Models
{
    [Serializable]
    public class Cart_item
    {
        public Mproduct product { get; set; }
        public int quantity { get; set; }

        public int countCart { get; set; }

        public string meThod { get; set; }

        public long priceTotal { get; set; }

        public long priceSaleee { get; set; }
        public bool f { get; set; }
    }
}