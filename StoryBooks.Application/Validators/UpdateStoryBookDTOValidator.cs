using FluentValidation;
using StoryBooks.Application.DTOs;

namespace StoryBooks.Application.Validators
{
    public class UpdateStoryBookDTOValidator : AbstractValidator<UpdateStoryBookDTO>
    {
        public UpdateStoryBookDTOValidator()
        {
            RuleFor(x => x.BookName)
                .MaximumLength(200)
                .WithMessage("Book name must not exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.BookName));

            RuleFor(x => x.Author)
                .MaximumLength(100)
                .WithMessage("Author name must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Author));

        }
    }
}
