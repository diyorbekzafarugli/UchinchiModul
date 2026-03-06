using FluentValidation;
using StudentCoursePlatform.Application.DTOs.Users;

namespace StudentCoursePlatform.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<UserRegisterDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ism bo'sh bo'lishi mumkin emas")
            .MaximumLength(50).WithMessage("Ism 50ta harfdan oshmasligi kerak");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email bo'sh bo'lishi mumkin emas")
            .EmailAddress().WithMessage("To'g'ri email formatida kiriting");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Parol kamida 8 ta belgidan iborat bo'lishi kerak")
            .Must(p => !p.Contains(" ")).WithMessage("Parolda bo'sh joy bo'lishi mumkin emas")
            .Matches(@"[A-Z]").WithMessage("Parolda kamida 1 ta katta harf ishtirok etishi kerak")
            .Matches(@"[a-z]").WithMessage("Parolda kamida bitta kichik harf ishtirok etishi kerak")
            .Matches(@"[0-9]").WithMessage("Parolda kamida 1 ta raqam ishtirok etishi kerak")
            .Matches(@"[\W_]").WithMessage("Parolda kamida 1 ta maxsus belgi ishtirok etishi kerak");
    }
}
