using System;
using System.Collections.Generic;
using Kodhier.Models;

namespace Kodhier.ViewModels
{
    public class CookingViewModel
    {

        public IEnumerable<Order> Queue { get; set; }
        public IEnumerable<Order> InProgress { get; set; }
    }
}