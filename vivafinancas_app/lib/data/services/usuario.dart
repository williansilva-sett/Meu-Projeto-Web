import '../models/usuario.dart';
import 'api_cliente.dart';

/// Espelha o UsuarioUpDateDTO real 
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

/// Espelha o UsuarioAlterarSenhaDTO 
class AlterarSenhaRequest {
  final String senhaAtual;
  final String novaSenha;

  AlterarSenhaRequest({required this.senhaAtual, required this.novaSenha});

  Map<String, dynamic> toJson() => {
        'senhaAtual': senhaAtual,
        'novaSenha': novaSenha,
      };
}

class UsuarioService {
  final ApiCliente _api = ApiCliente.instance;

  /// POST /api/usuarios - cadastro (AllowAnonymous, não precisa de token).
  Future<Usuario> cadastrar(UsuarioCreateRequest dados) async {
    final response = await _api.post('/usuarios', data: dados.toJson());
    return Usuario.fromJson(response.data as Map<String, dynamic>);
  }

  /// GET /api/usuarios/{id}
  Future<Usuario> buscarPorId(int id) async {
    final response = await _api.get('/usuarios/$id');
    return Usuario.fromJson(response.data as Map<String, dynamic>);
  }

  /// PUT /api/usuarios/{id}
  Future<void> atualizar(int id, UsuarioUpdateRequest dados) async {
    await _api.put('/usuarios/$id', data: dados.toJson());
  }

  /// PUT /api/usuarios/alterar-senha
  Future<void> alterarSenha(AlterarSenhaRequest dados) async {
    await _api.put('/usuarios/alterar-senha', data: dados.toJson());
  }
}