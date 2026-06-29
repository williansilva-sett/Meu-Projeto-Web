import 'package:flutter/foundation.dart';
import '../core/utils/token_storage.dart';
import '../data/models/usuario.dart';
import '../data/services/api_cliente.dart';
import '../data/services/api_exception.dart';
import '../data/services/auth.dart';
import '../data/services/usuario.dart';

enum AuthStatus { desconhecido, carregando, autenticado, naoAutenticado }

/// Gerencia o estado de autenticação da aplicação.
///
/// Responsabilidades:
/// - Login/logout
/// - Manter o Usuario atual em memória (evita buscar de novo em toda tela)
/// - Verificar se já existe uma sessão salva quando o app abre
/// - Reagir automaticamente quando o token expira (callback do ApiCliente)
class AuthProvider extends ChangeNotifier {
  final AuthService _authService = AuthService();
  final UsuarioService _usuarioService = UsuarioService();

  AuthProvider() {
    // Sempre que qualquer requisição (de qualquer service) receber 401,
    // o ApiCliente chama isso aqui pra derrubar a sessão na UI também.
    ApiCliente.instance.onUnauthorized = _aoExpirarSessao;
  }

  AuthStatus status = AuthStatus.desconhecido;
  Usuario? usuarioAtual;
  String? erro;

  bool get estaAutenticado => status == AuthStatus.autenticado;
  bool get carregando => status == AuthStatus.carregando;

  /// Chamado uma vez na inicialização do app (splash) pra saber se já
  /// existe um token salvo de uma sessão anterior.
  Future<void> verificarSessaoSalva() async {
    status = AuthStatus.carregando;
    notifyListeners();

    final logado = await _authService.estaLogado();
    if (!logado) {
      status = AuthStatus.naoAutenticado;
      notifyListeners();
      return;
    }

    // Não validamos a expiração localmente aqui de propósito: se o token
    // estiver vencido, a primeira chamada autenticada vai receber 401 e
    // o _aoExpirarSessao cuida de derrubar a sessão automaticamente.
    await _carregarUsuarioAtual();
    status = AuthStatus.autenticado;
    notifyListeners();
  }

  Future<bool> login(String email, String senha) async {
    status = AuthStatus.carregando;
    erro = null;
    notifyListeners();

    try {
      await _authService.login(email, senha);
      await _carregarUsuarioAtual();

      status = AuthStatus.autenticado;
      notifyListeners();
      return true;
    } on ApiException catch (e) {
      erro = e.message;
      status = AuthStatus.naoAutenticado;
      notifyListeners();
      return false;
    } catch (_) {
      erro = 'Erro inesperado. Tente novamente.';
      status = AuthStatus.naoAutenticado;
      notifyListeners();
      return false;
    }
  }

  Future<void> logout() async {
    await _authService.logout();
    usuarioAtual = null;
    status = AuthStatus.naoAutenticado;
    notifyListeners();
  }

  void limparErro() {
    erro = null;
    notifyListeners();
  }

  /// Busca os dados completos do usuário logado usando o usuarioId
  /// salvo (extraído do token no momento do login).
  ///
  /// Se essa busca falhar (ex: instabilidade de rede), não desfazemos
  /// o login - o usuário já foi autenticado com sucesso pela API. As
  /// telas que dependerem de usuarioAtual podem tentar de novo.
  Future<void> _carregarUsuarioAtual() async {
    final usuarioId = await TokenStorage.instance.getUserId();
    if (usuarioId == null) return;

    try {
      usuarioAtual = await _usuarioService.buscarPorId(usuarioId);
    } catch (_) {
      // Ignorado de propósito - ver comentário acima.
    }
  }

  void _aoExpirarSessao() {
    usuarioAtual = null;
    status = AuthStatus.naoAutenticado;
    erro = 'Sessão expirada. Faça login novamente.';
    notifyListeners();
  }
}
