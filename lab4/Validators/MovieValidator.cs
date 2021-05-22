using FluentValidation;
using Lab4.Data;
using Lab4.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Validators
{
	public class MovieValidator: AbstractValidator<MovieViewModel>
	{
        private readonly ApplicationDbContext _context;

        public MovieValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(m => m.Title).MinimumLength(1);
            RuleFor(m => m.Description).MinimumLength(10);
            RuleFor(m => m.ReleaseYear).Must(BeAValidReleaseYear).WithMessage("The release year cannot be in the future.");
            RuleFor(m => m.DurationMinutes).GreaterThan(0);
            RuleFor(m => m.Rating).Null().When(m => m.Watched == false).WithMessage("You cannot rate a movie you haven't watched.");
            RuleFor(m => m.Rating).InclusiveBetween(1, 10).When(m => m.Watched == true);
            RuleFor(m => m.Director).MinimumLength(1);
            RuleFor(m => m.Genre).NotNull();
        }

        private bool BeAValidReleaseYear(int releaseYear)
        {
            int currentYear = DateTime.Now.Year;
            return releaseYear <= currentYear;
        }
    }
}
