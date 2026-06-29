import 'dart:convert';

/// Decodifica o payload do JWT no cliente, sem validar assinatura
/// (a validação de verdade é feita pela API a cada requisição).
/// Usado só pra ler dados que já vieram autenticados, como o usuarioId.
class JwtHelper {
  JwtHelper._();

  static Map<String, dynamic> decodePayload(String token) {
    final parts = token.split('.');
    if (parts.length != 3) {
      throw const FormatException('Token JWT inválido.');
    }
    final normalized = base64Url.normalize(parts[1]);
    final payloadJson = utf8.decode(base64Url.decode(normalized));
    return jsonDecode(payloadJson) as Map<String, dynamic>;
  }

  /// Extrai a claim customizada "usuarioId" (conta.UsuarioID no backend) -
  /// é esse o ID usado em GET/PUT /api/usuarios/{id}, não o NameIdentifier
  /// (que é o ID da Conta de sistema, um valor diferente).
  static int? extrairUsuarioId(String token) {
    try {
      final payload = decodePayload(token);
      final value = payload['usuarioId'];
      if (value == null) return null;
      return int.tryParse(value.toString());
    } catch (_) {
      return null;
    }
  }
}
