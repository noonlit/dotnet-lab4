using System;
using System.Collections.Generic;

namespace Lab4.ViewModels
{
	public class MovieWithCommentsViewModel
	{
		public enum GenreType
		{
			Action, Comedy, Horror, Thriller
		}

		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public GenreType Genre { get; set; }

		public float DurationMinutes { get; set; }

		public int ReleaseYear { get; set; }

		public string Director { get; set; }

		public DateTime AddedAt { get; set; }

		public int? Rating { get; set; }

		public bool Watched { get; set; } = false;

		public IEnumerable<CommentViewModel> Comments { get; set; }
  }
}