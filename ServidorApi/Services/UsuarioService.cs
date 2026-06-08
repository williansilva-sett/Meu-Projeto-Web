using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;
 
namespace ServidorApi.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ISenhaHasher _senhaHasher;
 
        public UsuarioService(DataContext context, IMapper mapper, ISenhaHasher senhaHasher)
        {
            _context = context;
            _mapper = mapper;
            _senhaHasher = senhaHasher;
        }
 
        public async Task<IEnumerable<UsuarioResponseDTO>> ListarTodos()
        {
            // Include(u => u.Conta) carrega a conta de sistema junto
            // necessário para retornar o email no DTO de resposta
            var usuarios = await _context.Usuarios
                .Include(u => u.Conta)
                .ToListAsync();
 
            return _mapper.Map<IEnumerable<UsuarioResponseDTO>>(usuarios);
        }
 
        public async Task<UsuarioResponseDTO?> BuscarPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Conta)
                .FirstOrDefaultAsync(u => u.ID == id);
 
            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }
 
        public async Task<UsuarioResponseDTO> Criar(UsuarioCreateDTO dto)
        {
            // Verifica se já existe uma conta com esse email antes de criar
            var emailNormalizado = dto.Email.ToLower().Trim();
            var emailExiste = await _context.Contas
                .AnyAsync(c => c.Email == emailNormalizado);
 
            if (emailExiste)
                throw new InvalidOperationException("Email já cadastrado.");
 
            // Cria o Usuario com apenas dados pessoais
            var usuario = new Usuario
            {
                Nome      = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Idade     = dto.Idade,
                Telefone  = dto.Telefone
            };
 
            // Salva o Usuario primeiro para gerar o ID
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
 
            // Cria a Conta de sistema vinculada ao Usuario recém-criado
            // A senha é hasheada aqui — nunca armazenada em texto puro
            var conta = new Conta
            {
                Email      = emailNormalizado,
                Senha      = _senhaHasher.Hash(dto.Senha),
                Tipo       = TipoUsuario.Usuario, // Todo novo usuário nasce como Usuario
                Ativo      = true,
                DataCriacao = DateTime.Now,
                UsuarioID  = usuario.ID           // FK para o Usuario criado acima
            };
 
            _context.Contas.Add(conta);
            await _context.SaveChangesAsync();
 
            // Carrega a conta para o mapper ter acesso ao email no DTO de resposta
            usuario.Conta = conta;
 
            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }
 
        public async Task Atualizar(int id, UsuarioUpDateDTO usuarioDto)
        {
            // Atualiza apenas dados pessoais (telefone)
            // Email só pode ser alterado via AlterarEmail (futuro) ou pelo admin
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _mapper.Map(usuarioDto, usuario);
                await _context.SaveChangesAsync();
            }
        }
 
        public async Task Deletar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                // Cascade no DataContext garante que a Conta é deletada junto
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
 
        public async Task<bool> AlterarSenha(int contaId, UsuarioAlterarSenhaDTO dto)
        {
            // Busca a Conta de sistema pelo ID que vem do token JWT (claim NameIdentifier)
            // O token agora guarda o ID da Conta, não do Usuario
            var conta = await _context.Contas.FindAsync(contaId);
            if (conta is null) return false;
 
            // Valida que a senha atual bate com o hash armazenado na Conta
            if (!_senhaHasher.Verify(dto.SenhaAtual, conta.Senha))
                return false;
 
            // Aplica o hash na nova senha e salva
            conta.Senha = _senhaHasher.Hash(dto.NovaSenha);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}