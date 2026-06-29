import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Responsável por persistir o token JWT de forma segura (criptografado),
/// usando Keychain no iOS e EncryptedSharedPreferences no Android.
///
/// Nunca guardamos o token em SharedPreferences puro - lá fica em texto
/// plano e qualquer app com acesso ao storage do dispositivo conseguiria ler.
class TokenStorage {
  TokenStorage._();
  static final TokenStorage instance = TokenStorage._();

  final _storage = const FlutterSecureStorage(
    aOptions: AndroidOptions(encryptedSharedPreferences: true),
  );

  static const _tokenKey = 'viva_financas_jwt_token';
  static const _userIdKey = 'viva_financas_user_id';

  Future<void> saveToken(String token) async {
    await _storage.write(key: _tokenKey, value: token);
  }

  Future<String?> getToken() async {
    return _storage.read(key: _tokenKey);
  }

  Future<void> saveUserId(int idUsuario) async {
    await _storage.write(key: _userIdKey, value: idUsuario.toString());
  }

  Future<int?> getUserId() async {
    final value = await _storage.read(key: _userIdKey);
    return value != null ? int.tryParse(value) : null;
  }

  Future<bool> hasToken() async {
    final token = await getToken();
    return token != null && token.isNotEmpty;
  }

  /// Limpa tudo - usado no logout ou quando o token expira (401).
  Future<void> clear() async {
    await _storage.delete(key: _tokenKey);
    await _storage.delete(key: _userIdKey);
  }
}
