/**
 * Comunicação com o UsuariosController.
 */
const UsuarioApi = {
    async cadastrar({ nome, sobrenome, idade, telefone, email, senha }) {
      return ApiCliente.post('/usuarios', {
        nome,
        sobrenome,
        idade,
        telefone,
        email,
        senha,
      });
    },
  
    async buscarPorId(id) {
      return ApiCliente.get(`/usuarios/${id}`);
    },
  };