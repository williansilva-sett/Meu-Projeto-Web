using FluentValidation;
using ServidorApi.DTOs;

public class UsuarioCreateValidator : AbstractValidator<UsuarioCreateDTO>
{
    public UsuarioCreateValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome é obrigatorio.");
        RuleFor(x => x.Sobrenome).NotEmpty().WithMessage("O sobrenome é obrigatorio.");
        RuleFor(x => x.Email).EmailAddress().WithMessage("E-mail invalido.");
        RuleFor(x => x.Idade).GreaterThanOrEqualTo(18).WithMessage("O cadastro só é permitido para maiores de 18 anos.");
        RuleFor(x => x.Telefone).NotEmpty().WithMessage("Telefone obrigatorio.");
        RuleFor(x => x.Telefone).MinimumLength(10).WithMessage("Telefone inválido.");
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(8).WithMessage("A senha deve ter no mínomo 8 caracteres.");
    }
}