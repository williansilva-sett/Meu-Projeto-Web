import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'providers/auth_provider.dart';
//import 'providers/categoria_provider.dart';
import 'providers/meta_provider.dart';
import 'providers/movimentacao_provider.dart';
import 'providers/theme_provider.dart';
import 'screens/auth/login_screens.dart';
import 'screens/dashboard/dashboard_screens.dart';

void main() {
  runApp(const VivaFinancasApp());
}

class VivaFinancasApp extends StatelessWidget {
  const VivaFinancasApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => MetaProvider()),
        ChangeNotifierProvider(create: (_) => MovimentacaoProvider()),
        ChangeNotifierProvider(create: (_) => ThemeProvider()),
      ],
      child: Consumer<ThemeProvider>(
        builder: (context, themeProvider, _) {
          return MaterialApp(
            title: 'VivaFinanças',
            debugShowCheckedModeBanner: false,
            themeMode: themeProvider.themeMode,
            // camada for construída - por enquanto, um tema básico só
            // pra não travar o desenvolvimento das telas.
            theme: ThemeData(
              useMaterial3: true,
              colorScheme: ColorScheme.fromSeed(seedColor: Colors.green),
            ),
            darkTheme: ThemeData(
              useMaterial3: true,
              colorScheme: ColorScheme.fromSeed(
                seedColor: Colors.green,
                brightness: Brightness.dark,
              ),
            ),
            home: const AuthWrapper(),
          );
        },
      ),
    );
  }
}

/// Decide qual tela mostrar com base no status de autenticação.
///
/// Verifica uma única vez, na abertura do app, se já existe uma sessão
/// salva (token). Como o AuthProvider reage automaticamente a 401 (ver
/// onUnauthorized no ApiCliente), essa tela também troca pra "não
/// autenticado" sozinha se o token expirar durante o uso do app.
class AuthWrapper extends StatefulWidget {
  const AuthWrapper({super.key});

  @override
  State<AuthWrapper> createState() => _AuthWrapperState();
}

class _AuthWrapperState extends State<AuthWrapper> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<AuthProvider>().verificarSessaoSalva();
    });
  }

  @override
  Widget build(BuildContext context) {
    final authProvider = context.watch<AuthProvider>();

    switch (authProvider.status) {
      case AuthStatus.desconhecido:
      case AuthStatus.carregando:
        return const Scaffold(
          body: Center(child: CircularProgressIndicator()),
        );

      case AuthStatus.autenticado:
        return const DashboardScreen();

      case AuthStatus.naoAutenticado:
        return const LoginScreen();
    }
  }
}
