using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Models
{
	public class Favourites
	{
		public int Id { get; set; }
		public ApplicationUser User { get; set; }
		public IEnumerable<Movie> Movies { get; set; }
		public int Year { get; set; }
	}
}
