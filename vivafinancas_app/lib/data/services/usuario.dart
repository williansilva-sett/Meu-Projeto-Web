import '../models/usuario.dart';
import 'api_cliente.dart';

/// Espelha o UsuarioUpDateDTO real - só permite atualizar Telefone e Email.
class UsuarioUpdateRequest {
  final String telefone;
  final String email;

  UsuarioUpdateRequest({
    required this.telefone,
    required this.email,
  });

  Map<String, dynamic> toJson() => {
        'telefone': telefone,
        'email': email,
      };
}

/// Espelha o UsuarioAlterarSenhaDTO real.
class AlterarSenhaRequest {
  final String senhaAtual;
  final String novaSenha;

  AlterarSenhaRequest({required this.senhaAtual, required this.novaSenha});

  Map<String, dynamic> toJson() => {
        'senhaAtual': senhaAtual,
        'novaSenha': novaSenha,
      };
}

/// Service responsável pela comunicação com o UsuariosController.
class UsuarioService {
  final ApiCliente _api = ApiCliente.instance;

  /// POST /api/usuarios - cadastro (AllowAnonymous, não precisa de token).
  /// Se a validação falhar (ex: idade < 18), o erro vem como lista de
  /// mensagens - já tratado pelo ApiException.
  Future<Usuario> cadastrar(UsuarioCreateRequest dados) async {
    final response = await _api.post('/usuarios', data: dados.toJson());
    return Usuario.fromJson(response.data as Map<String, dynamic>);
  }

  /// GET /api/usuarios/{id} - requer estar logado.
  Future<Usuario> buscarPorId(int id) async {
    final response = await _api.get('/usuarios/$id');
    return Usuario.fromJson(response.data as Map<String, dynamic>);
  }

  /// PUT /api/usuarios/{id}
  Future<void> atualizar(int id, UsuarioUpdateRequest dados) async {
    await _api.put('/usuarios/$id', data: dados.toJson());
  }

  /// PUT /api/usuarios/alterar-senha
  /// O ID do usuário é extraído do token no backend (claim NameIdentifier),
  /// não vai na URL - por isso esse método não pede id.
  Future<void> alterarSenha(AlterarSenhaRequest dados) async {
    await _api.put('/usuarios/alterar-senha', data: dados.toJson());
  }
}
