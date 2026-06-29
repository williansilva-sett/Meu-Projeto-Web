import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../providers/meta_provider.dart';
import '../../providers/movimentacao_provider.dart';
import '../../widgets/loading_indicator.dart';
import '../../widgets/meta_card.dart';
import '../../widgets/movimentacao_card.dart';

/// Tela inicial depois do login - resumo financeiro (saldo, entradas,
/// saídas), últimas movimentações e progresso das metas.
///
/// Mesmo conteúdo do dashboard.html da web, em widgets nativos.
class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _carregarDados());
  }

  Future<void> _carregarDados() async {
    final movProvider = context.read<MovimentacaoProvider>();
    final metaProvider = context.read<MetaProvider>();
    await Future.wait([
      movProvider.carregarMovimentacoes(),
      metaProvider.carregarMetas(),
    ]);
  }

  String _formatarMoeda(double valor) {
    return 'R\$ ${valor.toStringAsFixed(2).replaceAll('.', ',')}';
  }

  @override
  Widget build(BuildContext context) {
    final authProvider = context.watch<AuthProvider>();
    final movProvider = context.watch<MovimentacaoProvider>();
    final metaProvider = context.watch<MetaProvider>();

    final nome = authProvider.usuarioAtual?.nome ?? '';
    final carregandoInicial =
        (movProvider.carregando && movProvider.movimentacoes.isEmpty) ||
            (metaProvider.carregando && metaProvider.metas.isEmpty);

    return Scaffold(
      backgroundColor: const Color(0xFFF6F8F7),
      appBar: AppBar(
        backgroundColor: const Color(0xFF024B2C),
        foregroundColor: Colors.white,
        title: Text(nome.isNotEmpty ? 'Olá, $nome' : 'Viva Finanças'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Sair',
            onPressed: () => context.read<AuthProvider>().logout(),
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: _carregarDados,
        child: carregandoInicial
            ? const LoadingIndicator()
            : ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  _cardSaldo(movProvider.saldo),
                  const SizedBox(height: 16),
                  Row(
                    children: [
                      Expanded(
                        child: _cardResumo(
                          titulo: 'Entradas',
                          valor: movProvider.totalEntradas,
                          cor: const Color(0xFF15803D),
                          corFundo: const Color(0xFFE8F7EE),
                          icone: Icons.arrow_upward,
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: _cardResumo(
                          titulo: 'Saídas',
                          valor: movProvider.totalSaidas,
                          cor: const Color(0xFFEF4444),
                          corFundo: const Color(0xFFFFE8E8),
                          icone: Icons.arrow_downward,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),

                  _cabecalhoSecao('Últimas movimentações', onVerTodas: () {
                    // TODO: navegar pra MovimentacoesScreen quando existir
                  }),
                  const SizedBox(height: 8),
                  _painel(
                    child: movProvider.erro != null
                        ? Text(movProvider.erro!,
                            style: const TextStyle(color: Colors.red))
                        : movProvider.movimentacoes.isEmpty
                            ? const Padding(
                                padding: EdgeInsets.symmetric(vertical: 24),
                                child: Center(
                                  child: Text(
                                    'Nenhuma movimentação ainda.',
                                    style: TextStyle(color: Colors.grey),
                                  ),
                                ),
                              )
                            : Column(
                                children: movProvider.movimentacoes
                                    .take(5)
                                    .map((m) => MovimentacaoCard(movimentacao: m))
                                    .toList(),
                              ),
                  ),
                  const SizedBox(height: 24),

                  _cabecalhoSecao('Suas metas', onVerTodas: () {
                    // TODO: navegar pra MetasScreen quando existir
                  }),
                  const SizedBox(height: 8),
                  _painel(
                    child: metaProvider.erro != null
                        ? Text(metaProvider.erro!,
                            style: const TextStyle(color: Colors.red))
                        : metaProvider.metas.isEmpty
                            ? const Padding(
                                padding: EdgeInsets.symmetric(vertical: 24),
                                child: Center(
                                  child: Text(
                                    'Nenhuma meta criada ainda.',
                                    style: TextStyle(color: Colors.grey),
                                  ),
                                ),
                              )
                            : Column(
                                children: metaProvider.metas
                                    .take(4)
                                    .map((m) => MetaCard(meta: m))
                                    .toList(),
                              ),
                  ),
                  const SizedBox(height: 24),
                ],
              ),
      ),
    );
  }

  Widget _cardSaldo(double saldo) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 20,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 56,
            height: 56,
            decoration: BoxDecoration(
              color: const Color(0xFFF1ECFF),
              borderRadius: BorderRadius.circular(14),
            ),
            child: const Icon(Icons.account_balance_wallet, color: Color(0xFF8B5CF6)),
          ),
          const SizedBox(width: 16),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text('Saldo', style: TextStyle(color: Colors.grey, fontSize: 14)),
              const SizedBox(height: 4),
              Text(
                _formatarMoeda(saldo),
                style: TextStyle(
                  fontSize: 26,
                  fontWeight: FontWeight.bold,
                  color: saldo >= 0 ? const Color(0xFF13213A) : const Color(0xFFEF4444),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _cardResumo({
    required String titulo,
    required double valor,
    required Color cor,
    required Color corFundo,
    required IconData icone,
  }) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 16,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: corFundo,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(icone, color: cor, size: 20),
          ),
          const SizedBox(height: 12),
          Text(titulo, style: const TextStyle(color: Colors.grey, fontSize: 13)),
          const SizedBox(height: 4),
          Text(
            _formatarMoeda(valor),
            style: const TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
              color: Color(0xFF13213A),
            ),
          ),
        ],
      ),
    );
  }

  Widget _cabecalhoSecao(String titulo, {VoidCallback? onVerTodas}) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          titulo,
          style: const TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Color(0xFF07124B),
          ),
        ),
        TextButton(
          onPressed: onVerTodas,
          child: const Text(
            'Ver todas',
            style: TextStyle(color: Color(0xFF16A34A), fontWeight: FontWeight.w600),
          ),
        ),
      ],
    );
  }

  Widget _painel({required Widget child}) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 16,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: child,
    );
  }
}