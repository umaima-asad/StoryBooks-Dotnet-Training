using FluentValidation;
using StoryBooks.Application.DTOs;
namespace StoryBooks.Application.DTOs
{
    public class CreateStoryBookDTOValidator : AbstractValidator<CreateStoryBookDTO>
    {
        public CreateStoryBookDTOValidator()
        {
            RuleFor(x => x.BookName)
                .NotEmpty().WithMessage("Book name is required.")
                .MaximumLength(200).WithMessage("Book name cannot exceed 200 characters.");
            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author name cannot exceed 100 characters.");
        }
    }
}
