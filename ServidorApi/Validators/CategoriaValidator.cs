using FluentValidation;
using ServidorApi.DTOs;
using ServidorApi.Models;

public class CategoriaValidator : AbstractValidator<CategoriaResponseDTO>
{
    public CategoriaValidator()
    {
        RuleFor(x => x.categoria)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(80).WithMessage("O nome da categoria deve ter no máximo 80 caracteres.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O Tipo de Categoria é Obrigatorio.")
            .IsEnumName(typeof(TipoCategoria), caseSensitive: true)
            .WithMessage("Tipo deve ser 'Entrada' ou 'Saida'.");

    }
}