using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kodhier.ViewModels
{
    public class DeliveryViewModel
    {
		public Guid Id { get; set; }
		public int Quantity { get; set; }
		public int Size { get; set; }
		public string Comment { get; set; }

		public string Name { get; set; }
		public string ImagePath { get; set; }

		public string DeliveryAddress { get; set; }
	}
}
