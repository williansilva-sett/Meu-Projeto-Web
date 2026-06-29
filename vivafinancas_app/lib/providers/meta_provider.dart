import 'package:flutter/foundation.dart';
import '../data/models/metas.dart';
import '../data/services/api_exception.dart';
import '../data/services/meta.dart';

/// Gerencia o estado das metas financeiras pra UI.
class MetaProvider extends ChangeNotifier {
  final MetaService _metaService = MetaService();

  List<Meta> metas = [];
  bool carregando = false;
  String? erro;

  Future<void> carregarMetas() async {
    carregando = true;
    erro = null;
    notifyListeners();
    try {
      metas = await _metaService.listarTodas();
    } on ApiException catch (e) {
      erro = e.message;
    } catch (_) {
      erro = 'Erro ao carregar metas.';
    } finally {
      carregando = false;
      notifyListeners();
    }
  }

  Future<bool> criarMeta(MetaCreateRequest dados) async {
    try {
      final nova = await _metaService.criar(dados);
      metas = [nova, ...metas];
      notifyListeners();
      return true;
    } on ApiException catch (e) {
      erro = e.message;
      notifyListeners();
      return false;
    } catch (_) {
      erro = 'Erro ao criar meta.';
      notifyListeners();
      return false;
    }
  }

  Future<bool> atualizarMeta(int id, MetaUpdateRequest dados) async {
    try {
      await _metaService.atualizar(id, dados);
      await _recarregarUma(id);
      return true;
    } on ApiException catch (e) {
      erro = e.message;
      notifyListeners();
      return false;
    } catch (_) {
      erro = 'Erro ao atualizar meta.';
      notifyListeners();
      return false;
    }
  }

  /// Registra um aporte (novo valor atual) na meta.
  Future<bool> registrarAporte(int id, double novoValorAtual) async {
    try {
      await _metaService.atualizarProgresso(id, novoValorAtual);
      await _recarregarUma(id);
      return true;
    } on ApiException catch (e) {
      erro = e.message;
      notifyListeners();
      return false;
    } catch (_) {
      erro = 'Erro ao registrar aporte.';
      notifyListeners();
      return false;
    }
  }

  Future<bool> atualizarStatus(int id, StatusMeta status) async {
    try {
      await _metaService.atualizarStatus(id, status);
      await _recarregarUma(id);
      return true;
    } on ApiException catch (e) {
      erro = e.message;
      notifyListeners();
      return false;
    } catch (_) {
      erro = 'Erro ao atualizar status.';
      notifyListeners();
      return false;
    }
  }

  Future<bool> deletarMeta(int id) async {
    try {
      await _metaService.deletar(id);
      metas = metas.where((m) => m.id != id).toList();
      notifyListeners();
      return true;
    } catch (_) {
      erro = 'Erro ao deletar meta.';
      notifyListeners();
      return false;
    }
  }

  Future<void> _recarregarUma(int id) async {
    final atualizada = await _metaService.buscarPorId(id);
    final index = metas.indexWhere((m) => m.id == id);
    if (index != -1) {
      metas[index] = atualizada;
    }
    notifyListeners();
  }

  void limparErro() {
    erro = null;
    notifyListeners();
  }
}
