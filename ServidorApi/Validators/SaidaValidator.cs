using FluentValidation;
using ServidorApi;
using ServidorApi.DTOs;
using ServidorApi.Models;

public class SaidaValidator : AbstractValidator<SaidaResponseDTO>
{
    public SaidaValidator()
    {
        /*RuleFor(x => x.Descricao).NotEmpty().WithMessage("Descrição necessaria.");*/
        RuleFor(x => x.ValorSaida).GreaterThan(0).WithMessage("Um valor é obrigatorio.");
    }
}
