enum TipoCategoria { entrada, saida }

extension TipoCategoriaJson on TipoCategoria {
  String get valor => switch (this) {
        TipoCategoria.entrada => 'Entrada',
        TipoCategoria.saida => 'Saida',
      };

  static TipoCategoria fromString(String valor) => switch (valor) {
        'Saida' => TipoCategoria.saida,
        _ => TipoCategoria.entrada,
      };
}

/// Espelha o CategoriaResponseDTO real.
///
/// ATENÇÃO: o campo de nome vem com a chave JSON "categoria" (minúsculo),
/// porque a propriedade C# também é "categoria" (não "Categoria") - não é
/// erro de digitação meu, é assim mesmo no DTO original.
class Categoria {
  final int idCategoria;
  final String nome;
  final TipoCategoria tipo;
  final int? idUsuario; // null = categoria global do sistema

  Categoria({
    required this.idCategoria,
    required this.nome,
    required this.tipo,
    this.idUsuario,
  });

  factory Categoria.fromJson(Map<String, dynamic> json) {
    return Categoria(
      idCategoria: json['idCategoria'] as int,
      nome: json['categoria'] as String,
      tipo: TipoCategoriaJson.fromString(json['tipo'] as String),
      idUsuario: json['idUsuario'] as int?,
    );
  }
}