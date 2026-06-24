/// Espelha o UsuarioResponseDTO real da API.
class Usuario {
  final int id;
  final String nome;
  final String sobrenome;
  final int idade;
  final String telefone;
  final String email;
  final DateTime dataCriacao;

  Usuario({
    required this.id,
    required this.nome,
    required this.sobrenome,
    required this.idade,
    required this.telefone,
    required this.email,
    required this.dataCriacao,
  });

  String get nomeCompleto => '$nome $sobrenome';

  factory Usuario.fromJson(Map<String, dynamic> json) {
    return Usuario(
      id: json['id'] as int,
      nome: json['nome'] as String,
      sobrenome: json['sobrenome'] as String,
      idade: json['idade'] as int,
      telefone: json['telefone'] as String,
      email: json['email'] as String,
      dataCriacao: DateTime.parse(json['dataCriacao'] as String),
    );
  }

  Map<String, dynamic> toJson() => {
        'id': id,
        'nome': nome,
        'sobrenome': sobrenome,
        'idade': idade,
        'telefone': telefone,
        'email': email,
        'dataCriacao': dataCriacao.toIso8601String(),
      };
}

/// Espelha o UsuarioCreateDTO - payload enviado no cadastro.

class UsuarioCreateRequest {
  final String nome;
  final String sobrenome;
  final int idade;
  final String telefone;
  final String email;
  final String senha;

  UsuarioCreateRequest({
    required this.nome,
    required this.sobrenome,
    required this.idade,
    required this.telefone,
    required this.email,
    required this.senha,
  });

  Map<String, dynamic> toJson() => {
        'nome': nome,
        'sobrenome': sobrenome,
        'idade': idade,
        'telefone': telefone,
        'email': email,
        'senha': senha,
      };
}