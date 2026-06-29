import '../models/movimentacoes.dart';
import 'api_cliente.dart';

/// Comunicação com EntradasController (/api/Entradas) e SaidaController
/// (/api/Saida) - unificados aqui num service só, já que na UI tratamos
/// os dois como "Movimentações".
///
/// Pressupõe a versão atualizada dos controllers, onde GetAll já filtra
/// por usuário no servidor e Create força o IDUsuario pelo token (mesmo
/// padrão do /meta) - por isso não precisamos mais filtrar no cliente
/// nem enviar idUsuario manualmente.
class MovimentacaoService {
  final ApiCliente _api = ApiCliente.instance;

  Future<List<Movimentacao>> listarTodas() async {
    final entradas = await _listarEntradas();
    final saidas = await _listarSaidas();

    final todas = [...entradas, ...saidas]
      ..sort((a, b) => b.data.compareTo(a.data));

    return todas;
  }

  Future<List<Movimentacao>> _listarEntradas() async {
    final response = await _api.get('/Entradas');
    return (response.data as List)
        .map((j) => Movimentacao.fromEntradaJson(j as Map<String, dynamic>))
        .toList();
  }

  Future<List<Movimentacao>> _listarSaidas() async {
    final response = await _api.get('/Saida');
    return (response.data as List)
        .map((j) => Movimentacao.fromSaidaJson(j as Map<String, dynamic>))
        .toList();
  }

  Future<Movimentacao> criarEntrada({
    required String descricao,
    required double valor,
    required int idCategoria,
    DateTime? data,
  }) async {
    final response = await _api.post('/Entradas', data: {
      'descricao': descricao,
      'valorEntrada': valor,
      'data': (data ?? DateTime.now()).toIso8601String(),
      'idCategoria': idCategoria,
    });
    return Movimentacao.fromEntradaJson(response.data as Map<String, dynamic>);
  }

  Future<Movimentacao> criarSaida({
    required double valor,
    required int idCategoria,
    DateTime? data,
  }) async {
    final response = await _api.post('/Saida', data: {
      'valorSaida': valor,
      'dataSaida': (data ?? DateTime.now()).toIso8601String(),
      'idCategoria': idCategoria,
    });
    return Movimentacao.fromSaidaJson(response.data as Map<String, dynamic>);
  }

  Future<void> atualizarEntrada(
    int id, {
    required String descricao,
    required double valor,
  }) async {
    await _api.put('/Entradas/$id', data: {
      'descricao': descricao,
      'valorEntrada': valor,
    });
  }

  Future<void> atualizarSaida(int id, {required double valor}) async {
    await _api.put('/Saida/$id', data: {'valorSaida': valor});
  }

  Future<void> deletarEntrada(int id) async {
    await _api.delete('/Entradas/$id');
  }

  Future<void> deletarSaida(int id) async {
    await _api.delete('/Saida/$id');
  }
}
