using FluentValidation;
using ServidorApi.DTOs;

public class CategoriaValidator : AbstractValidator<CategoriaResponseDTO>
{
    public CategoriaValidator()
    {
        RuleFor(x => x.Tipo).NotEmpty().WithMessage("O Tipo de Categoria é Obrigatorio.");
    }
}