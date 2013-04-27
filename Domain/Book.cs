﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<int> TabIds { get; set; }
    }
}