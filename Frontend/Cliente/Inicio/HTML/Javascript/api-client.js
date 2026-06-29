// Configuração central de comunicação com a API VivaFinanças.
const API_BASE_URL = 'http://localhost:5079/api';

/**
 * Erro customizado que normaliza os 3 formatos de erro que a API devolve
 * (string pura, objeto { mensagem }, array de strings de validação) numa
 * única mensagem pronta pra mostrar na tela - mesma lógica do
 * ApiException que já fizemos no Flutter.
 */
class ApiError extends Error {
  constructor(status, data) {
    super(ApiError._extrairMensagem(status, data));
    this.status = status;
    this.data = data;
  }

  static _extrairMensagem(status, data) {
    if (typeof data === 'string' && data.trim()) return data;

    if (Array.isArray(data)) return data.join('\n');

    if (data && typeof data === 'object') {
      if (data.mensagem) return data.mensagem;
      if (data.message) return data.message;
      if (data.title) return data.title;
    }

    switch (status) {
      case 401:
        return 'Email ou senha inválidos.';
      case 403:
        return 'Você não tem permissão para essa ação.';
      case 404:
        return 'Recurso não encontrado.';
      case 429:
        return 'Muitas tentativas. Tente novamente mais tarde.';
      case 500:
        return 'Erro interno no servidor. Tente novamente mais tarde.';
      default:
        return 'Erro ao comunicar com o servidor.';
    }
  }
}

/**
 * Cliente HTTP baseado em fetch, com:
 * - injeção automática do token JWT (se existir) em toda requisição
 * - tratamento de erro consistente, igual ao ApiCliente do Flutter
 */
const ApiCliente = {
  async _montarHeaders() {
    const headers = { 'Content-Type': 'application/json' };
    const token = Auth.getToken();
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    return headers;
  },

  async _tratarResposta(response) {
    if (response.status === 204) return null;

    let data = null;
    const texto = await response.text();
    if (texto) {
      try {
        data = JSON.parse(texto);
      } catch {
        data = texto; // resposta veio como string pura
      }
    }

    if (!response.ok) {
      throw new ApiError(response.status, data);
    }

    return data;
  },

  async get(path) {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      method: 'GET',
      headers: await this._montarHeaders(),
    });
    return this._tratarResposta(response);
  },

  async post(path, body) {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      method: 'POST',
      headers: await this._montarHeaders(),
      body: JSON.stringify(body),
    });
    return this._tratarResposta(response);
  },

  async put(path, body) {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      method: 'PUT',
      headers: await this._montarHeaders(),
      body: JSON.stringify(body),
    });
    return this._tratarResposta(response);
  },

  async patch(path, body) {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      method: 'PATCH',
      headers: await this._montarHeaders(),
      body: JSON.stringify(body),
    });
    return this._tratarResposta(response);
  },

  async delete(path) {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      method: 'DELETE',
      headers: await this._montarHeaders(),
    });
    return this._tratarResposta(response);
  },
};