using System;
using System.Collections.Generic;

namespace PrintPalSearch.Models.Database
{
    public partial class Product
    {
        public int Id { get; set; }
        public string? ProductCategory { get; set; }
        public string? ProductTitle { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductSrcurl { get; set; }
        public string? ProductImage { get; set; }
    }
}
