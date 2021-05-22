using System;
using System.ComponentModel.DataAnnotations;

namespace Lab4.Models
{
    public class Comment
    {
      public int Id { get; set; }

      public string Text { get; set; }

      public bool Important { get; set; }

      public Movie Movie { get; set; }

      public int MovieId { get; set; }
  }
}
