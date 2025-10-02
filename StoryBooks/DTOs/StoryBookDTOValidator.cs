using FluentValidation;
using StoryBooks.DTOs;
namespace StoryBooks.DTOs
{
    public class StoryBookDTOValidator : AbstractValidator<StoryBookDTO>
    {
        public StoryBookDTOValidator()
        {
            RuleFor(x => x.BookName)
                .NotEmpty().WithMessage("Book name is required.")
                .MaximumLength(200).WithMessage("Book name cannot exceed 200 characters.");
            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author name cannot exceed 100 characters.");
            RuleFor(x => x.Cover)
                .MaximumLength(500).WithMessage("Cover URL cannot exceed 500 characters.")
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("Cover must be a valid URL if provided.");
        }
    }
}
