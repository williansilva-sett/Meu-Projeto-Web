enum TipoMovimentacao { entrada, saida }

/// Modelo unificado pra Entrada e Saída - na API são dois controllers e
/// dois DTOs diferentes (EntradasResponseDTO / SaidaResponseDTO), mas na
/// UI tratamos como uma lista só de "transações".
class Movimentacao {
  final int id; // idEntrada ou idSaida, dependendo do tipo
  final TipoMovimentacao tipo;
  final String? descricao; // Saída não tem campo de descrição na API
  final double valor;
  final DateTime data;
  final int idUsuario;
  final int idCategoria;

  Movimentacao({
    required this.id,
    required this.tipo,
    this.descricao,
    required this.valor,
    required this.data,
    required this.idUsuario,
    required this.idCategoria,
  });

  factory Movimentacao.fromEntradaJson(Map<String, dynamic> json) {
    return Movimentacao(
      id: json['idEntrada'] as int,
      tipo: TipoMovimentacao.entrada,
      descricao: json['descricao'] as String?,
      valor: (json['valorEntrada'] as num).toDouble(),
      data: DateTime.parse(json['data'] as String),
      idUsuario: json['idUsuario'] as int,
      idCategoria: json['idCategoria'] as int,
    );
  }

  factory Movimentacao.fromSaidaJson(Map<String, dynamic> json) {
    return Movimentacao(
      id: json['idSaida'] as int,
      tipo: TipoMovimentacao.saida,
      descricao: null,
      valor: (json['valorSaida'] as num).toDouble(),
      data: DateTime.parse(json['dataSaida'] as String),
      idUsuario: json['idUsuario'] as int,
      idCategoria: json['idCategoria'] as int,
    );
  }
}
