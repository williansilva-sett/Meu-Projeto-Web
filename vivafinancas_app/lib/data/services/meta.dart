import '../models/metas.dart';
import 'api_cliente.dart';

/// Comunicação com o MetaController.
/// Rota: /api/meta (singular - confirmado no [Route] do controller).
/// Todas as rotas exigem token válido; o backend já filtra por usuário
/// usando a claim "usuarioId" do token, então não precisamos enviar id
/// nenhum manualmente aqui.
class MetaService {
  final ApiCliente _api = ApiCliente.instance;

  /// GET /api/meta - já vem filtrado pelo usuário logado, no servidor.
  Future<List<Meta>> listarTodas() async {
    final response = await _api.get('/meta');
    return (response.data as List)
        .map((j) => Meta.fromJson(j as Map<String, dynamic>))
        .toList();
  }

  Future<Meta> buscarPorId(int id) async {
    final response = await _api.get('/meta/$id');
    return Meta.fromJson(response.data as Map<String, dynamic>);
  }

  Future<Meta> criar(MetaCreateRequest dados) async {
    final response = await _api.post('/meta', data: dados.toJson());
    return Meta.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> atualizar(int id, MetaUpdateRequest dados) async {
    await _api.put('/meta/$id', data: dados.toJson());
  }

  /// PATCH /api/meta/{id}/progresso - registra um novo valor atual (aporte).
  Future<void> atualizarProgresso(int id, double valorAtual) async {
    await _api.patch('/meta/$id/progresso', data: {'valorAtual': valorAtual});
  }

  /// PATCH /api/meta/{id}/status
  Future<void> atualizarStatus(int id, StatusMeta status) async {
    await _api.patch('/meta/$id/status', data: {'status': status.valor});
  }

  Future<void> deletar(int id) async {
    await _api.delete('/meta/$id');
  }
}
