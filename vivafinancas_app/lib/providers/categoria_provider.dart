import 'package:flutter/foundation.dart';
import '../data/models/categoria.dart';
import '../data/services/api_exception.dart';
import '../data/services/categoria.dart';

/// Mantém a lista de categorias em cache - elas mudam raramente (só Admin
/// cria/deleta), então carregamos uma vez e reusamos em qualquer tela que
/// precise: dropdown de criar entrada/saída, exibir o nome da categoria
/// na lista de movimentações, etc.
class CategoriaProvider extends ChangeNotifier {
  final CategoriaService _service = CategoriaService();

  List<Categoria> categorias = [];
  bool carregando = false;
  String? erro;

  List<Categoria> get categoriasEntrada =>
      categorias.where((c) => c.tipo == TipoCategoria.entrada).toList();

  List<Categoria> get categoriasSaida =>
      categorias.where((c) => c.tipo == TipoCategoria.saida).toList();

  /// Carrega só na primeira vez - chamadas seguintes usam o cache.
  /// Use [recarregar] se precisar forçar um refresh.
  Future<void> carregarCategorias() async {
    if (categorias.isNotEmpty) return;
    await _buscar();
  }

  Future<void> recarregar() async {
    categorias = [];
    await _buscar();
  }

  Future<void> _buscar() async {
    carregando = true;
    erro = null;
    notifyListeners();
    try {
      categorias = await _service.listarTodas();
    } on ApiException catch (e) {
      erro = e.message;
    } catch (_) {
      erro = 'Erro ao carregar categorias.';
    } finally {
      carregando = false;
      notifyListeners();
    }
  }

  /// Útil pra exibir o nome da categoria a partir só do idCategoria
  /// guardado numa Entrada/Saída.
  Categoria? buscarPorId(int id) {
    try {
      return categorias.firstWhere((c) => c.idCategoria == id);
    } catch (_) {
      return null;
    }
  }

  /// Cria uma categoria nova (qualquer usuário autenticado pode).
  Future<Categoria> criarCategoria(String nome, TipoCategoria tipo) async {
    final nova = await _service.criar(nome, tipo);
    categorias = [...categorias, nova];
    notifyListeners();
    return nova;
  }

  /// Procura uma categoria existente com esse nome+tipo (sem diferenciar
  /// maiúscula/minúscula) e reaproveita; se não encontrar, cria uma nova.
  /// Use isso na tela de criar Entrada/Saída em vez de chamar
  /// [criarCategoria] direto, pra evitar categorias duplicadas.
  Future<Categoria> obterOuCriar(String nome, TipoCategoria tipo) async {
    await carregarCategorias();

    final nomeBuscado = nome.trim().toLowerCase();
    for (final c in categorias) {
      if (c.nome.trim().toLowerCase() == nomeBuscado && c.tipo == tipo) {
        return c;
      }
    }

    return criarCategoria(nome.trim(), tipo);
  }

  void limparErro() {
    erro = null;
    notifyListeners();
  }
}
