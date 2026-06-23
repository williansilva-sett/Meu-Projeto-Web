import 'dart:convert';


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

.
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