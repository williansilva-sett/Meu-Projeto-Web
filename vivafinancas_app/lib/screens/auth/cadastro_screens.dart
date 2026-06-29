import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../data/models/usuario.dart';
import '../../data/services/api_exception.dart';
import '../../data/services/usuario.dart';
import '../../providers/auth_provider.dart';
import '../../widgets/custom_text_field.dart';

/// Tela de Cadastro - visual replicado do cadastro.html original.
///
/// ATENÇÃO: duas diferenças em relação ao mockup, necessárias pra bater
/// com o UsuarioCreateDTO real:
/// 1. O mockup tinha um campo único "NOME" - separei em Nome + Sobrenome,
///    porque a API guarda os dois separados.
/// 2. O mockup pedia "Data de Nascimento", mas a API guarda Idade (um
///    número), não a data. Mantive o seletor de data (UX melhor) e
///    calculo a idade a partir dela antes de enviar.
class CadastroScreen extends StatefulWidget {
  const CadastroScreen({super.key});

  @override
  State<CadastroScreen> createState() => _CadastroScreenState();
}

class _CadastroScreenState extends State<CadastroScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nomeController = TextEditingController();
  final _sobrenomeController = TextEditingController();
  final _emailController = TextEditingController();
  final _telefoneController = TextEditingController();
  final _senhaController = TextEditingController();
  final _dataNascimentoController = TextEditingController();

  DateTime? _dataNascimento;
  bool _senhaVisivel = false;
  bool _carregando = false;

  @override
  void dispose() {
    _nomeController.dispose();
    _sobrenomeController.dispose();
    _emailController.dispose();
    _telefoneController.dispose();
    _senhaController.dispose();
    _dataNascimentoController.dispose();
    super.dispose();
  }

  int _calcularIdade(DateTime nascimento) {
    final hoje = DateTime.now();
    int idade = hoje.year - nascimento.year;
    if (hoje.month < nascimento.month ||
        (hoje.month == nascimento.month && hoje.day < nascimento.day)) {
      idade--;
    }
    return idade;
  }

  Future<void> _selecionarData() async {
    final selecionada = await showDatePicker(
      context: context,
      initialDate: DateTime(2000),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
    );

    if (selecionada != null) {
      setState(() {
        _dataNascimento = selecionada;
        _dataNascimentoController.text =
            '${selecionada.day.toString().padLeft(2, '0')}/${selecionada.month.toString().padLeft(2, '0')}/${selecionada.year}';
      });
    }
  }

  Future<void> _cadastrar() async {
    if (!_formKey.currentState!.validate()) return;

    if (_dataNascimento == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Selecione sua data de nascimento.')),
      );
      return;
    }

    final idade = _calcularIdade(_dataNascimento!);
    if (idade < 18) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('É necessário ter 18 anos ou mais para se cadastrar.'),
        ),
      );
      return;
    }

    setState(() => _carregando = true);

    try {
      await UsuarioService().cadastrar(UsuarioCreateRequest(
        nome: _nomeController.text.trim(),
        sobrenome: _sobrenomeController.text.trim(),
        idade: idade,
        telefone: _telefoneController.text.trim(),
        email: _emailController.text.trim(),
        senha: _senhaController.text,
      ));

      if (!mounted) return;

      // Cadastro feito - loga automaticamente com as mesmas credenciais,
      // pra não pedir pro usuário digitar tudo de novo. O AuthWrapper
      // (main.dart) troca de tela sozinho quando o status virar autenticado.
      await context.read<AuthProvider>().login(
            _emailController.text.trim(),
            _senhaController.text,
          );

      if (mounted) Navigator.of(context).pop();
    } on ApiException catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context)
            .showSnackBar(SnackBar(content: Text(e.message)));
      }
    } catch (_) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Erro ao cadastrar. Tente novamente.')),
        );
      }
    } finally {
      if (mounted) setState(() => _carregando = false);
    }
  }

  @override
  Widget build(BuildContext context) {
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
                            'Criar sua Conta',
                            style: TextStyle(
                              fontSize: 28,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF333333),
                            ),
                          ),
                          const SizedBox(height: 5),
                          Row(
                            children: [
                              const Text(
                                'Já tem uma conta? ',
                                style: TextStyle(
                                    fontSize: 13, color: Color(0xFF888888)),
                              ),
                              GestureDetector(
                                onTap: () => Navigator.of(context).pop(),
                                child: const Text(
                                  'Faça Login',
                                  style: TextStyle(
                                    fontSize: 13,
                                    color: Color(0xFF888888),
                                    decoration: TextDecoration.underline,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 25),

                          CustomTextField(
                            label: 'NOME',
                            controller: _nomeController,
                            hint: 'Jiara',
                            validator: (v) => (v == null || v.trim().isEmpty)
                                ? 'Informe seu nome'
                                : null,
                          ),
                          const SizedBox(height: 16),

                          CustomTextField(
                            label: 'SOBRENOME',
                            controller: _sobrenomeController,
                            hint: 'Martins',
                            validator: (v) => (v == null || v.trim().isEmpty)
                                ? 'Informe seu sobrenome'
                                : null,
                          ),
                          const SizedBox(height: 16),

                          CustomTextField(
                            label: 'EMAIL',
                            controller: _emailController,
                            hint: 'email@exemplo.com',
                            keyboardType: TextInputType.emailAddress,
                            validator: (v) {
                              if (v == null || v.trim().isEmpty) {
                                return 'Informe seu email';
                              }
                              if (!v.contains('@')) return 'Email inválido';
                              return null;
                            },
                          ),
                          const SizedBox(height: 16),

                          CustomTextField(
                            label: 'TELEFONE',
                            controller: _telefoneController,
                            hint: '(+55) xx xxxxx-xxxx',
                            keyboardType: TextInputType.phone,
                            validator: (v) => (v == null || v.trim().isEmpty)
                                ? 'Informe seu telefone'
                                : null,
                          ),
                          const SizedBox(height: 16),

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
                            validator: (v) {
                              if (v == null || v.isEmpty) return 'Crie uma senha';
                              if (v.length < 6) return 'Mínimo de 6 caracteres';
                              return null;
                            },
                          ),
                          const SizedBox(height: 16),

                          CustomTextField(
                            label: 'DATA DE NASCIMENTO',
                            controller: _dataNascimentoController,
                            hint: 'Selecionar data',
                            readOnly: true,
                            onTap: _selecionarData,
                            suffixIcon: const Icon(
                              Icons.calendar_today,
                              color: Color(0xFF999999),
                              size: 18,
                            ),
                          ),
                          const SizedBox(height: 25),

                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              onPressed: _carregando ? null : _cadastrar,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFF113F29),
                                foregroundColor: Colors.white,
                                padding: const EdgeInsets.symmetric(vertical: 15),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                elevation: 0,
                              ),
                              child: _carregando
                                  ? const SizedBox(
                                      width: 20,
                                      height: 20,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2,
                                        color: Colors.white,
                                      ),
                                    )
                                  : const Text(
                                      'Cadastrar-se',
                                      style: TextStyle(
                                        fontSize: 16,
                                        fontWeight: FontWeight.w600,
                                      ),
                                    ),
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