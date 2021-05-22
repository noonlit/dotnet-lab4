using FluentValidation;
using Lab4.Data;
using Lab4.ViewModels;

namespace Lab4.Validators
{
	public class CommentValidator: AbstractValidator<CommentViewModel>
	{
        private readonly ApplicationDbContext _context;

        public CommentValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(c => c.Text).MinimumLength(10);
            RuleFor(c => c.MovieId).NotNull();
        }
    }
}
