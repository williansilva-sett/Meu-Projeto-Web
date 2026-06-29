/**
 * Comunicação com o AuthController + gerenciamento do token no browser.
 *
 * ATENÇÃO: guardamos o token em localStorage. É simples e funciona, mas
 * fica exposto a ataques XSS (qualquer script que rode na página
 * consegue ler). Pra um MVP tudo bem; se for pra produção, o ideal é a
 * API devolver o token num cookie httpOnly em vez de no corpo da
 * resposta - mas isso exige mudar o backend também.
 */
const Auth = {
    TOKEN_KEY: 'viva_financas_token',
    USUARIO_ID_KEY: 'viva_financas_usuario_id',
  
    getToken() {
      return localStorage.getItem(this.TOKEN_KEY);
    },
  
    estaLogado() {
      return !!this.getToken();
    },
  
    getUsuarioId() {
      const valor = localStorage.getItem(this.USUARIO_ID_KEY);
      return valor ? parseInt(valor, 10) : null;
    },
  
    async login(email, senha) {
      const resultado = await ApiCliente.post('/auth/login', { email, senha });
  
      localStorage.setItem(this.TOKEN_KEY, resultado.token);
  
      const usuarioId = this._extrairUsuarioId(resultado.token);
      if (usuarioId) {
        localStorage.setItem(this.USUARIO_ID_KEY, usuarioId);
      }
  
      return resultado;
    },
  
    logout() {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.USUARIO_ID_KEY);
    },
  
    // Decodifica o payload do JWT (sem validar assinatura - isso a API já
    // faz a cada request) pra extrair a claim customizada "usuarioId" -
    // mesma lógica do JwtHelper que já fizemos no Flutter.
    _extrairUsuarioId(token) {
      try {
        const payloadBase64 = token.split('.')[1];
        const normalizado = payloadBase64.replace(/-/g, '+').replace(/_/g, '/');
        const resto = normalizado.length % 4;
        const comPadding = resto ? normalizado + '='.repeat(4 - resto) : normalizado;
        const payload = JSON.parse(atob(comPadding));
        return payload.usuarioId ?? null;
      } catch {
        return null;
      }
    },
  };