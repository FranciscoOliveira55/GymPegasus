﻿namespace FrontEndWeb.Models
{
    public class ProductBase:EntityBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
