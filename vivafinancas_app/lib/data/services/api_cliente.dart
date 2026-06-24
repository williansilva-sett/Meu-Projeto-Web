import 'package:dio/dio.dart';
import '../../core/utils/token_storage.dart';
import 'api_exception.dart';


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

  Future<Response> delete(String path) async {
    try {
      return await _dio.delete(path);
    } on DioException catch (e) {
      throw ApiException.fromDioException(e);
    }
  }
}