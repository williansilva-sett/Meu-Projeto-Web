import 'package:dio/dio.dart';
import '../../core/utils/token_storage.dart';
import 'api_exception.dart';

/// Cliente HTTP central da aplicação.
/// Toda comunicação com a API do VivaFinanças passa por aqui.
///
/// Responsabilidades:
/// - Centralizar a URL base e os timeouts
/// - Injetar o token JWT automaticamente em toda requisição (igual ao
///   [Authorize] do lado da API)
/// - Reagir globalmente quando o token expira (401)
/// - Converter erros do Dio em ApiException, já tratados pra UI
class ApiCliente {
  ApiCliente._internal() {
    _dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: const Duration(seconds: 15),
        receiveTimeout: const Duration(seconds: 15),
        headers: {'Content-Type': 'application/json'},
      ),
    );

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final token = await TokenStorage.instance.getToken();
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onError: (DioException error, handler) async {
          if (error.response?.statusCode == 401) {
            // Token expirado/inválido: limpa a sessão local.
            // O AuthProvider deve escutar isso (via onUnauthorized) e
            // redirecionar para a tela de Login.
            await TokenStorage.instance.clear();
            onUnauthorized?.call();
          }
          handler.next(error);
        },
      ),
    );
  }

  static final ApiCliente instance = ApiCliente._internal();

  late final Dio _dio;
  Dio get dio => _dio;

  /// Configurado pelo AuthProvider para reagir globalmente quando a
  /// sessão expira (ex: navegar pra tela de Login).
  void Function()? onUnauthorized;

  // -------------------------------------------------------------------
  // ATENÇÃO: ajuste a baseUrl conforme onde a API está rodando.
  //
  // Emulador Android   -> http://10.0.2.2:PORTA
  // Simulador iOS      -> http://localhost:PORTA
  // Dispositivo físico -> http://SEU_IP_NA_REDE:PORTA (ex: 192.168.1.10:5000)
  //
  // A porta é a que aparece no launchSettings.json do seu projeto .NET.
  // -------------------------------------------------------------------
  static const String baseUrl = 'http://10.0.2.2:5000/api';

  Future<Response> get(String path,
      {Map<String, dynamic>? queryParameters}) async {
    try {
      return await _dio.get(path, queryParameters: queryParameters);
    } on DioException catch (e) {
      throw ApiException.fromDioException(e);
    }
  }

  Future<Response> post(String path, {dynamic data}) async {
    try {
      return await _dio.post(path, data: data);
    } on DioException catch (e) {
      throw ApiException.fromDioException(e);
    }
  }

  Future<Response> put(String path, {dynamic data}) async {
    try {
      return await _dio.put(path, data: data);
    } on DioException catch (e) {
      throw ApiException.fromDioException(e);
    }
  }

  Future<Response> patch(String path, {dynamic data}) async {
    try {
      return await _dio.patch(path, data: data);
    } on DioException catch (e) {
      throw ApiException.fromDioException(e);
    }
  }

  Future<Response> delete(String path) async {
    try {
      return await _dio.delete(path);
    } on DioException catch (e) {
      throw ApiException.fromDioException(e);
    }
  }
}
