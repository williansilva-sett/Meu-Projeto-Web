import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

/// Controla o tema (claro/escuro/sistema) do app e lembra a escolha do
/// usuário entre uma sessão e outra usando shared_preferences.
///
/// ATENÇÃO: usa o pacote `shared_preferences`, que ainda não está no
/// pubspec.yaml. Adicione: shared_preferences: ^2.2.0
class ThemeProvider extends ChangeNotifier {
  static const _prefKey = 'viva_financas_theme_mode';

  ThemeMode _themeMode = ThemeMode.system;
  ThemeMode get themeMode => _themeMode;

  bool get isDarkMode => _themeMode == ThemeMode.dark;

  ThemeProvider() {
    _carregarPreferencia();
  }

  Future<void> _carregarPreferencia() async {
    final prefs = await SharedPreferences.getInstance();
    final salvo = prefs.getString(_prefKey);

    switch (salvo) {
      case 'light':
        _themeMode = ThemeMode.light;
        break;
      case 'dark':
        _themeMode = ThemeMode.dark;
        break;
      default:
        _themeMode = ThemeMode.system;
    }
    notifyListeners();
  }

  /// Alterna entre claro e escuro (usado no botão de toggle simples).
  Future<void> alternarTema() async {
    _themeMode =
        _themeMode == ThemeMode.dark ? ThemeMode.light : ThemeMode.dark;
    notifyListeners();
    await _salvarPreferencia();
  }

  /// Define um modo específico (usado se a tela de Configurações tiver
  /// as 3 opções: claro / escuro / seguir o sistema).
  Future<void> definirTema(ThemeMode modo) async {
    _themeMode = modo;
    notifyListeners();
    await _salvarPreferencia();
  }

  Future<void> _salvarPreferencia() async {
    final prefs = await SharedPreferences.getInstance();
    final valor = switch (_themeMode) {
      ThemeMode.light => 'light',
      ThemeMode.dark => 'dark',
      ThemeMode.system => 'system',
    };
    await prefs.setString(_prefKey, valor);
  }
}