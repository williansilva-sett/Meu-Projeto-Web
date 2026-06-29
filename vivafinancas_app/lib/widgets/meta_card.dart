import 'package:flutter/material.dart';
import '../data/models/metas.dart';

/// Card que mostra uma meta com nome, valores e barra de progresso -
/// mesmo conceito visual do .meta-item no dashboard web.
class MetaCard extends StatelessWidget {
  final Meta meta;
  final VoidCallback? onTap;

  const MetaCard({super.key, required this.meta, this.onTap});

  String _formatarMoeda(double valor) {
    return 'R\$ ${valor.toStringAsFixed(2).replaceAll('.', ',')}';
  }

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 10),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    meta.nome,
                    style: const TextStyle(
                      fontWeight: FontWeight.w600,
                      fontSize: 15,
                      color: Color(0xFF07124B),
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
                Text(
                  '${_formatarMoeda(meta.valorAtual)} / ${_formatarMoeda(meta.valorAlvo)}',
                  style: TextStyle(color: Colors.grey[600], fontSize: 13),
                ),
              ],
            ),
            const SizedBox(height: 8),
            ClipRRect(
              borderRadius: BorderRadius.circular(10),
              child: LinearProgressIndicator(
                value: (meta.progresso / 100).clamp(0.0, 1.0),
                minHeight: 10,
                backgroundColor: const Color(0xFFEEF2EF),
                valueColor: const AlwaysStoppedAnimation(Color(0xFF16A34A)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
