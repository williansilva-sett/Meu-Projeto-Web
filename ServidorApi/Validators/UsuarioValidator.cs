using FluentValidation;
using ServidorApi.DTOs;

public class UsuarioValidator : AbstractValidator<UsuarioResponseDTO>
{
    public UsuarioValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Sobrenome).NotEmpty().WithMessage("O sobrenome é obrigatorio.");
        RuleFor(x => x.Email).EmailAddress().WithMessage("E-mail inválido.");
        RuleFor(x => x.Idade).GreaterThanOrEqualTo(18).WithMessage("O cadastro só é permitido para maiores de 18 anos.");
        RuleFor(x => x.Telefone).NotEmpty().WithMessage("Telefone obrigatório.");
        RuleFor(x => x.Telefone).MinimumLength(10).WithMessage("Telefone inválido.");
    }
}