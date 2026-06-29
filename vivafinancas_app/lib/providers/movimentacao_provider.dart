import 'package:flutter/foundation.dart';
import '../data/models/movimentacoes.dart';
import '../data/services/api_exception.dart';
import '../data/services/movimentacoes.dart';

/// Gerencia o estado das movimentações (entradas + saídas) pra UI.
class MovimentacaoProvider extends ChangeNotifier {
  final MovimentacaoService _service = MovimentacaoService();

  List<Movimentacao> movimentacoes = [];
  bool carregando = false;
  String? erro;

  double get totalEntradas => movimentacoes
      .where((m) => m.tipo == TipoMovimentacao.entrada)
      .fold(0.0, (soma, m) => soma + m.valor);

  double get totalSaidas => movimentacoes
      .where((m) => m.tipo == TipoMovimentacao.saida)
      .fold(0.0, (soma, m) => soma + m.valor);

  double get saldo => totalEntradas - totalSaidas;

  Future<void> carregarMovimentacoes() async {
    carregando = true;
    erro = null;
    notifyListeners();
    try {
      movimentacoes = await _service.listarTodas();
    } on ApiException catch (e) {
      erro = e.message;
    } catch (_) {
      erro = 'Erro ao carregar movimentações.';
    } finally {
      carregando = false;
      notifyListeners();
    }
  }

  Future<bool> criarEntrada({
    required String descricao,
    required double valor,
    required int idCategoria,
  }) async {
    try {
      final nova = await _service.criarEntrada(
        descricao: descricao,
        valor: valor,
        idCategoria: idCategoria,
      );
      movimentacoes = [nova, ...movimentacoes];
      notifyListeners();
      return true;
    } on ApiException catch (e) {
      erro = e.message;
      notifyListeners();
      return false;
    } catch (_) {
      erro = 'Erro ao criar entrada.';
      notifyListeners();
      return false;
    }
  }

  Future<bool> criarSaida({
    required double valor,
    required int idCategoria,
  }) async {
    try {
      final nova = await _service.criarSaida(
        valor: valor,
        idCategoria: idCategoria,
      );
      movimentacoes = [nova, ...movimentacoes];
      notifyListeners();
      return true;
    } on ApiException catch (e) {
      erro = e.message;
      notifyListeners();
      return false;
    } catch (_) {
      erro = 'Erro ao criar saída.';
      notifyListeners();
      return false;
    }
  }

  Future<bool> deletar(Movimentacao movimentacao) async {
    try {
      if (movimentacao.tipo == TipoMovimentacao.entrada) {
        await _service.deletarEntrada(movimentacao.id);
      } else {
        await _service.deletarSaida(movimentacao.id);
      }
      movimentacoes = movimentacoes
          .where((m) =>
              !(m.id == movimentacao.id && m.tipo == movimentacao.tipo))
          .toList();
      notifyListeners();
      return true;
    } catch (_) {
      erro = 'Erro ao deletar.';
      notifyListeners();
      return false;
    }
  }

  void limparErro() {
    erro = null;
    notifyListeners();
  }
}
