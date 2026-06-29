/// Espelha o status real da Meta (string no backend: "EmAndamento",
/// "Concluida", "Cancelada").
enum StatusMeta { emAndamento, concluida, cancelada }

extension StatusMetaJson on StatusMeta {
  String get valor => switch (this) {
        StatusMeta.emAndamento => 'EmAndamento',
        StatusMeta.concluida => 'Concluida',
        StatusMeta.cancelada => 'Cancelada',
      };

  static StatusMeta fromString(String valor) => switch (valor) {
        'Concluida' => StatusMeta.concluida,
        'Cancelada' => StatusMeta.cancelada,
        _ => StatusMeta.emAndamento,
      };
}

/// Espelha o MetaResponseDTO real (GET /api/meta e GET /api/meta/{id}).
class Meta {
  final int id;
  final String nome;
  final String? descricao;
  final double valorAlvo;
  final double valorAtual;
  final DateTime dataInicio;
  final DateTime? dataLimite;
  final StatusMeta status;
  final int idUsuario;

  /// Percentual de progresso - já vem calculado pela API (0 a 100).
  final double progresso;

  Meta({
    required this.id,
    required this.nome,
    this.descricao,
    required this.valorAlvo,
    required this.valorAtual,
    required this.dataInicio,
    this.dataLimite,
    required this.status,
    required this.idUsuario,
    required this.progresso,
  });

  factory Meta.fromJson(Map<String, dynamic> json) {
    return Meta(
      id: json['id'] as int,
      nome: json['nome'] as String,
      descricao: json['descricao'] as String?,
      valorAlvo: (json['valorAlvo'] as num).toDouble(),
      valorAtual: (json['valorAtual'] as num).toDouble(),
      dataInicio: DateTime.parse(json['dataInicio'] as String),
      dataLimite: json['dataLimite'] != null
          ? DateTime.parse(json['dataLimite'] as String)
          : null,
      status: StatusMetaJson.fromString(json['status'] as String),
      idUsuario: json['idUsuario'] as int,
      progresso: (json['progresso'] as num).toDouble(),
    );
  }
}

/// POST /api/meta (MetaCreateDTO).
///
/// Não precisa enviar idUsuario - o MetaController sobrescreve esse
/// campo com o valor do token automaticamente, então nem incluímos aqui.
class MetaCreateRequest {
  final String nome;
  final String? descricao;
  final double valorAlvo;
  final double valorAtual;
  final DateTime? dataInicio;
  final DateTime? dataLimite;

  MetaCreateRequest({
    required this.nome,
    this.descricao,
    required this.valorAlvo,
    this.valorAtual = 0,
    this.dataInicio,
    this.dataLimite,
  });

  Map<String, dynamic> toJson() => {
        'nome': nome,
        'descricao': descricao,
        'valorAlvo': valorAlvo,
        'valorAtual': valorAtual,
        if (dataInicio != null) 'dataInicio': dataInicio!.toIso8601String(),
        if (dataLimite != null) 'dataLimite': dataLimite!.toIso8601String(),
      };
}

/// PUT /api/meta/{id} (MetaUpdateDTO).
/// Não inclui valorAtual nem status - esses têm endpoints PATCH próprios.
class MetaUpdateRequest {
  final String nome;
  final String? descricao;
  final double valorAlvo;
  final DateTime? dataLimite;

  MetaUpdateRequest({
    required this.nome,
    this.descricao,
    required this.valorAlvo,
    this.dataLimite,
  });

  Map<String, dynamic> toJson() => {
        'nome': nome,
        'descricao': descricao,
        'valorAlvo': valorAlvo,
        if (dataLimite != null) 'dataLimite': dataLimite!.toIso8601String(),
      };
}
