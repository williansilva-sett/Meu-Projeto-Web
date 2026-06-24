import '../../core/utils/jwt_helper.dart';
import '../../core/utils/token_storage.dart';
import 'api_cliente.dart';
/// Espelha AuthController
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

class AuthService {
  final ApiCliente _api = ApiCliente.instance;

  Future<LoginResult> login(String email, String senha) async {
    final response = await _api.post('/auth/login', data: {
      'email': email,
      'senha': senha,
    });

    final result = LoginResult.fromJson(response.data as Map<String, dynamic>);
    await TokenStorage.instance.saveToken(result.token);

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