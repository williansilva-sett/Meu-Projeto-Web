import 'package:flutter/material.dart';
import '../data/models/movimentacoes.dart';

/// Linha que mostra uma movimentação (entrada ou saída) - mesmo conceito
/// visual do .movimentacao-item no dashboard web.
class MovimentacaoCard extends StatelessWidget {
  final Movimentacao movimentacao;
  final VoidCallback? onTap;
  final VoidCallback? onDelete;

  const MovimentacaoCard({
    super.key,
    required this.movimentacao,
    this.onTap,
    this.onDelete,
  });

  String _formatarMoeda(double valor) {
    return 'R\$ ${valor.toStringAsFixed(2).replaceAll('.', ',')}';
  }

  String _formatarData(DateTime data) {
    return '${data.day.toString().padLeft(2, '0')}/${data.month.toString().padLeft(2, '0')}/${data.year}';
  }

  @override
  Widget build(BuildContext context) {
    final eEntrada = movimentacao.tipo == TipoMovimentacao.entrada;
    final cor = eEntrada ? const Color(0xFF15803D) : const Color(0xFFEF4444);
    final corFundo = eEntrada ? const Color(0xFFE8F7EE) : const Color(0xFFFFE8E8);
    final icone = eEntrada ? Icons.arrow_upward : Icons.arrow_downward;

    return ListTile(
      onTap: onTap,
      contentPadding: EdgeInsets.zero,
      leading: Container(
        width: 40,
        height: 40,
        decoration: BoxDecoration(
          color: corFundo,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Icon(icone, color: cor, size: 18),
      ),
      title: Text(
        movimentacao.descricao ?? (eEntrada ? 'Entrada' : 'Saída'),
        style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 15),
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: Text(
        _formatarData(movimentacao.data),
        style: TextStyle(color: Colors.grey[600], fontSize: 13),
      ),
      trailing: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            '${eEntrada ? '+' : '-'} ${_formatarMoeda(movimentacao.valor)}',
            style: TextStyle(fontWeight: FontWeight.bold, color: cor, fontSize: 14),
          ),
          if (onDelete != null)
            IconButton(
              icon: const Icon(Icons.delete_outline, size: 20, color: Colors.grey),
              onPressed: onDelete,
            ),
        ],
      ),
    );
  }
}
