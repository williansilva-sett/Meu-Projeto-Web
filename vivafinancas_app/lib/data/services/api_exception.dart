import 'package:dio/dio.dart';

class ApiException implements Exception {
  final String message;
  final int? statusCode;
  final bool isAuthError;

  ApiException(this.message, {this.statusCode, this.isAuthError = false});

  factory ApiException.fromDioException(DioException e) {
    switch (e.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        return ApiException(
            'Tempo de conexão esgotado. Verifique sua internet.');
      case DioExceptionType.connectionError:
        return ApiException(
            'Não foi possível conectar ao servidor. Verifique se a API está rodando.');
      case DioExceptionType.badResponse:
        return _fromResponse(e);
      case DioExceptionType.cancel:
        return ApiException('Requisição cancelada.');
      default:
        return ApiException('Ocorreu um erro inesperado. Tente novamente.');
    }
  }

  static ApiException _fromResponse(DioException e) {
    final statusCode = e.response?.statusCode;
    final data = e.response?.data;


    String? serverMessage;
    if (data is Map<String, dynamic>) {
      serverMessage =
          data['message'] ?? data['mensagem'] ?? data['title'] ?? data['error'];
    } else if (data is String && data.trim().isNotEmpty) {
      serverMessage = data;
    } else if (data is List) {
      final mensagens = data.map((e) => e.toString()).where((s) => s.isNotEmpty);
      if (mensagens.isNotEmpty) serverMessage = mensagens.join('\n');
    }

    switch (statusCode) {
      case 400:
        return ApiException(
            serverMessage ?? 'Dados inválidos. Verifique os campos.',
            statusCode: 400);
      case 401:

        return ApiException(
            serverMessage ?? 'Sessão expirada. Faça login novamente.',
            statusCode: 401,
            isAuthError: true);
      case 403:
        return ApiException(
            serverMessage ?? 'Você não tem permissão para essa ação.',
            statusCode: 403,
            isAuthError: true);
      case 404:
        return ApiException(serverMessage ?? 'Recurso não encontrado.',
            statusCode: 404);
      case 429:

        return ApiException(
            serverMessage ?? 'Muitas tentativas. Tente novamente mais tarde.',
            statusCode: 429);
      case 500:
        return ApiException(
            'Erro interno no servidor. Tente novamente mais tarde.',
            statusCode: 500);
      default:
        return ApiException(
            serverMessage ?? 'Erro ao comunicar com o servidor.',
            statusCode: statusCode);
    }
  }

  @override
  String toString() => message;
}