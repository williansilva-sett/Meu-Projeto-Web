using FluentValidation;
using ServidorApi.DTOs;

public class ContaValidator : AbstractValidator<ContaResponseDTO>
{
    public ContaValidator()
    {
        RuleFor(x => x.NomeConta).NotEmpty().WithMessage("O nome é Obrigatorio.");
    }
}