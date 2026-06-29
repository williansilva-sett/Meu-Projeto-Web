import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../widgets/custom_text_field.dart';
import 'cadastro_screens.dart';

/// Tela de Login - visual replicado do login.html original (fundo preto,
/// card branco bem arredondado, botão verde-escuro quase preto, inputs
/// cinza claro).
///
/// ATENÇÃO: o mockup original tinha um campo "NOME" pro login, mas a API
/// exige Email (LoginRequestDTO.Email) - troquei o label/placeholder pra
/// EMAIL pra bater com o contrato real, mantendo o resto do visual igual.
class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _senhaController = TextEditingController();
  bool _senhaVisivel = false;

  @override
  void dispose() {
    _emailController.dispose();
    _senhaController.dispose();
    super.dispose();
  }

  Future<void> _entrar() async {
    if (!_formKey.currentState!.validate()) return;

    final authProvider = context.read<AuthProvider>();
    final sucesso = await authProvider.login(
      _emailController.text.trim(),
      _senhaController.text,
    );

    if (!sucesso && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(authProvider.erro ?? 'Erro ao fazer login.')),
      );
    }
    // Se der certo, o AuthWrapper (main.dart) já troca de tela sozinho
    // ao perceber authProvider.status == AuthStatus.autenticado.
  }

  @override
  Widget build(BuildContext context) {
    final authProvider = context.watch<AuthProvider>();

    return Scaffold(
      backgroundColor: Colors.black,
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 32),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 400),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  // Logo - adicione o arquivo real em assets/images/logo.png
                  // e declare no pubspec.yaml. Enquanto isso, cai num ícone.
                  Image.asset(
                    'assets/images/logo.png',
                    width: 250,
                    errorBuilder: (_, _, _) => const Icon(
                      Icons.account_balance_wallet,
                      color: Colors.white,
                      size: 64,
                    ),
                  ),
                  const SizedBox(height: 30),

                  Container(
                    width: double.infinity,
                    padding:
                        const EdgeInsets.symmetric(horizontal: 30, vertical: 40),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(40),
                    ),
                    child: Form(
                      key: _formKey,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'Login',
                            style: TextStyle(
                              fontSize: 32,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF333333),
                            ),
                          ),
                          const SizedBox(height: 5),
                          const Text(
                            'Faça login para continuar.',
                            style:
                                TextStyle(fontSize: 14, color: Color(0xFF888888)),
                          ),
                          const SizedBox(height: 30),

                          CustomTextField(
                            label: 'EMAIL',
                            controller: _emailController,
                            hint: 'email@exemplo.com',
                            keyboardType: TextInputType.emailAddress,
                            validator: (valor) {
                              if (valor == null || valor.trim().isEmpty) {
                                return 'Informe seu email';
                              }
                              if (!valor.contains('@')) {
                                return 'Email inválido';
                              }
                              return null;
                            },
                          ),
                          const SizedBox(height: 20),

                          CustomTextField(
                            label: 'SENHA',
                            controller: _senhaController,
                            hint: '******',
                            obscureText: !_senhaVisivel,
                            suffixIcon: IconButton(
                              icon: Icon(
                                _senhaVisivel
                                    ? Icons.visibility_off
                                    : Icons.visibility,
                                color: const Color(0xFF999999),
                              ),
                              onPressed: () =>
                                  setState(() => _senhaVisivel = !_senhaVisivel),
                            ),
                            validator: (valor) {
                              if (valor == null || valor.isEmpty) {
                                return 'Informe sua senha';
                              }
                              return null;
                            },
                          ),
                          const SizedBox(height: 10),

                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              onPressed: authProvider.carregando ? null : _entrar,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFF0B1F13),
                                foregroundColor: Colors.white,
                                padding: const EdgeInsets.symmetric(vertical: 15),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(10),
                                ),
                                elevation: 0,
                              ),
                              child: authProvider.carregando
                                  ? const SizedBox(
                                      width: 20,
                                      height: 20,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2,
                                        color: Colors.white,
                                      ),
                                    )
                                  : const Text(
                                      'Entrar',
                                      style: TextStyle(
                                        fontSize: 16,
                                        fontWeight: FontWeight.w600,
                                      ),
                                    ),
                            ),
                          ),
                          const SizedBox(height: 25),

                          Center(
                            child: Column(
                              children: [
                                TextButton(
                                  // TODO: navegar pra RecuperacaoSenhaScreen
                                  // quando ela existir.
                                  onPressed: () {},
                                  child: const Text(
                                    'Esqueceu a senha?',
                                    style: TextStyle(
                                      color: Color(0xFF333333),
                                      fontSize: 14,
                                    ),
                                  ),
                                ),
                                TextButton(
                                  onPressed: () {
                                    Navigator.of(context).push(
                                      MaterialPageRoute(
                                        builder: (_) => const CadastroScreen(),
                                      ),
                                    );
                                  },
                                  child: const Text(
                                    'Cadastre-se!',
                                    style: TextStyle(
                                      color: Color(0xFF333333),
                                      fontSize: 14,
                                      fontWeight: FontWeight.w600,
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

}