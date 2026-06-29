import '../../core/utils/jwt_helper.dart';
import '../../core/utils/token_storage.dart';
import 'api_cliente.dart';

/// Espelha o retorno real do AuthController.Login (LoginResponseDTO):
/// Token, ExpiraEm, Nome, Tipo.
class LoginResult {
  final String token;
  final DateTime expiraEm;
  final String nome;
  final String tipo;

  LoginResult({
    required this.token,
    required this.expiraEm,
    required this.nome,
    required this.tipo,
  });

  factory LoginResult.fromJson(Map<String, dynamic> json) {
    return LoginResult(
      token: json['token'] as String,
      expiraEm: DateTime.parse(json['expiraEm'] as String),
      nome: json['nome'] as String,
      tipo: json['tipo'] as String,
    );
  }
}

/// Service responsável pela comunicação com o AuthController.
///
/// Implementa só o que realmente existe na API hoje: POST /auth/login.
/// O /login-admin não entra aqui porque o app não tem área administrativa.
///
/// Cadastro e alteração de senha moram no UsuarioService (UsuariosController).
/// Recuperação de senha ("esqueci minha senha") ainda não existe na API.
class AuthService {
  final ApiCliente _api = ApiCliente.instance;

  Future<LoginResult> login(String email, String senha) async {
    final response = await _api.post('/auth/login', data: {
      'email': email,
      'senha': senha,
    });

    final result = LoginResult.fromJson(response.data as Map<String, dynamic>);
    await TokenStorage.instance.saveToken(result.token);

    // O LoginResponseDTO não devolve o ID, mas o token tem a claim
    // customizada "usuarioId" - extraímos daqui pra usar nas rotas
    // protegidas que pedem o id na URL (ex: GET /api/usuarios/{id}).
    final usuarioId = JwtHelper.extrairUsuarioId(result.token);
    if (usuarioId != null) {
      await TokenStorage.instance.saveUserId(usuarioId);
    }

    return result;
  }

  Future<void> logout() async {
    await TokenStorage.instance.clear();
  }

  Future<bool> estaLogado() async {
    return TokenStorage.instance.hasToken();
  }
}
