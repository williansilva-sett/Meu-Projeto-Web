import '../models/categoria.dart';
import 'api_cliente.dart';

/// Comunicação com o CategoriaController (rota: /api/Categoria).
///
/// Só leitura aqui de propósito - criar e deletar categoria são
/// restritos a Admin (Roles = "Admin"), e o app não tem área administrativa.
class CategoriaService {
  final ApiCliente _api = ApiCliente.instance;

  Future<List<Categoria>> listarTodas() async {
    final response = await _api.get('/Categoria');
    return (response.data as List)
        .map((j) => Categoria.fromJson(j as Map<String, dynamic>))
        .toList();
  }

  Future<Categoria> buscarPorId(int id) async {
    final response = await _api.get('/Categoria/$id');
    return Categoria.fromJson(response.data as Map<String, dynamic>);
  }

  /// POST /api/Categoria - liberado pra qualquer usuário autenticado.
  /// Os nomes de campo no JSON seguem o DTO real: "categoria" pro nome
  /// (não "nome") e "tipo".
  Future<Categoria> criar(String nome, TipoCategoria tipo) async {
    final response = await _api.post('/Categoria', data: {
      'categoria': nome,
      'tipo': tipo.valor,
    });
    return Categoria.fromJson(response.data as Map<String, dynamic>);
  }
}
