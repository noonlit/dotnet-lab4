using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.ViewModels
{
	public class FavouritesForUserViewModel
	{
		public ApplicationUserViewModel User { get; set; }
		public List<MovieViewModel> Movies { get; set; }
		public int Year { get; set; }
	}
}
