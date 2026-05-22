using FluentValidation;
using ServidorApi;
using ServidorApi.DTOs;

public class EntradasValidator : AbstractValidator<EntradasResponseDTO>
{
    public EntradasValidator()
    {
        RuleFor(x => x.Descricao).NotEmpty().WithMessage("Descrição necessaria.");
        RuleFor(x => x.ValorEntrada).GreaterThan(0).WithMessage("Um valor é obrigatorio.");
    }
}
