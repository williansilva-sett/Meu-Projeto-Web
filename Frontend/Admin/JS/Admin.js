// Cliente/Admin/JS/admin.js
// Utilitários compartilhados por todas as páginas admin

const API_BASE   = 'http://localhost:5079';
const ADMIN_BASE = '/Frontend/Admin/HTML';

// ── TOKEN ──────────────────────────────────────────────────────────────────

function salvarToken(token, nome, tipo) {
    sessionStorage.setItem('vf_token', token);
    sessionStorage.setItem('vf_nome', nome);
    sessionStorage.setItem('vf_tipo', tipo);
}

function obterToken() {
    return sessionStorage.getItem('vf_token');
}

function obterNome() {
    return sessionStorage.getItem('vf_nome') || 'Admin';
}

function logout() {
    sessionStorage.clear();
    window.location.href = `${ADMIN_BASE}/LoginAdmin.html`;
}

function requerAdmin() {
    const token = obterToken();
    const tipo  = sessionStorage.getItem('vf_tipo');

    if (!token || tipo !== 'Admin') {
        window.location.href = `${ADMIN_BASE}/LoginAdmin.html`;
        return false;
    }
    return true;
}

// ── FETCH COM TOKEN E TIMEOUT ────────────────────────────────────────────────
// CORREÇÃO: fetch sem timeout pode ficar pendurado indefinidamente se o
// servidor não responder (não cair, só não responder) — isso era uma das
// causas do spinner preso. AbortController força uma falha após 10s.

async function apiFetch(path, options = {}) {
    const token = obterToken();
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 10000); // 10s

    try {
        const res = await fetch(API_BASE + path, {
            ...options,
            signal: controller.signal,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': token ? `Bearer ${token}` : '',
                ...(options.headers || {})
            }
        });

        clearTimeout(timeoutId);

        if (res.status === 401) {
            logout();
            return null;
        }

        return res;
    } catch (err) {
        clearTimeout(timeoutId);
        // Erro de rede, timeout (AbortError) ou CORS — devolve null,
        // quem chamou (apiGet etc.) decide o que fazer com isso.
        return null;
    }
}

// CORREÇÃO: apiGet agora sempre retorna um objeto com {ok, data, status},
// nunca apenas "null" de forma ambígua. Isso permite que cada página saiba
// DISTINGUIR "sem dados" de "erro de conexão" e tratar os dois casos —
// antes, se res fosse null por qualquer motivo, telas como o card de
// resumo financeiro do dashboard ficavam esperando pra sempre porque só
// o bloco de métricas tratava o caso de erro.
async function apiGet(path) {
    try {
        const res = await apiFetch(path);

        if (!res) {
            // Falha de rede/timeout — já tratado (token expirado já fez logout)
            return { ok: false, data: null, status: 0 };
        }

        if (!res.ok) {
            const erroBody = await res.json().catch(() => null);
            return { ok: false, data: erroBody, status: res.status };
        }

        const data = await res.json().catch(() => null);
        return { ok: true, data, status: res.status };
    } catch {
        return { ok: false, data: null, status: 0 };
    }
}

async function apiPost(path, body) {
    try {
        const res = await apiFetch(path, { method: 'POST', body: JSON.stringify(body) });
        if (!res) return { ok: false, data: null, status: 0 };
        const data = res.status !== 204 ? await res.json().catch(() => null) : null;
        return { ok: res.ok, data, status: res.status };
    } catch {
        return { ok: false, data: null, status: 0 };
    }
}

async function apiPut(path, body) {
    try {
        const res = await apiFetch(path, { method: 'PUT', body: JSON.stringify(body) });
        if (!res) return { ok: false, status: 0 };
        return { ok: res.ok, status: res.status };
    } catch {
        return { ok: false, status: 0 };
    }
}

async function apiPatch(path, body) {
    try {
        const res = await apiFetch(path, { method: 'PATCH', body: JSON.stringify(body) });
        if (!res) return { ok: false, status: 0 };
        return { ok: res.ok, status: res.status };
    } catch {
        return { ok: false, status: 0 };
    }
}

async function apiDelete(path) {
    try {
        const res = await apiFetch(path, { method: 'DELETE' });
        if (!res) return { ok: false, status: 0 };
        return { ok: res.ok, status: res.status };
    } catch {
        return { ok: false, status: 0 };
    }
}

// ── LOGIN ADMIN (sem token ainda — usa fetch direto, fora do apiFetch) ──────
// CORREÇÃO: padroniza a leitura da resposta 429, lendo COM SEGURANÇA tanto
// { mensagem: "..." } quanto { message: "..." } ou um body vazio/HTML de
// erro genérico do servidor — qualquer formato que a API já devolva hoje.
async function loginAdmin(email, senha) {
    const controller = new AbortController();
    const timeoutId  = setTimeout(() => controller.abort(), 10000);

    try {
        const res = await fetch(API_BASE + '/api/auth/login-admin', {
            method: 'POST',
            signal: controller.signal,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, senha })
        });

        clearTimeout(timeoutId);

        // Tenta ler JSON; se a API mandar texto puro ou nada, não quebra
        const body = await res.json().catch(() => null);

        if (res.status === 429) {
            const msg = body?.mensagem || body?.message
                || 'Muitas tentativas de login. Tente novamente em alguns minutos.';
            return { ok: false, status: 429, mensagem: msg };
        }

        if (res.status === 403) {
            return {
                ok: false,
                status: 403,
                mensagem: body?.mensagem || 'Acesso negado. Esta conta não tem permissão de administrador.'
            };
        }

        if (!res.ok) {
            return {
                ok: false,
                status: res.status,
                mensagem: body?.mensagem || 'E-mail ou senha incorretos.'
            };
        }

        return { ok: true, status: res.status, data: body };

    } catch (err) {
        clearTimeout(timeoutId);
        const timedOut = err.name === 'AbortError';
        return {
            ok: false,
            status: 0,
            mensagem: timedOut
                ? 'O servidor demorou demais para responder. Tente novamente.'
                : 'Não foi possível conectar ao servidor.'
        };
    }
}

// ── TOAST ──────────────────────────────────────────────────────────────────

function toast(mensagem, tipo = 'sucesso') {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    const el = document.createElement('div');
    el.className = `toast toast-${tipo}`;
    el.textContent = mensagem;
    container.appendChild(el);

    setTimeout(() => el.remove(), 3000);
}

// ── SEGURANÇA — ESCAPE DE HTML ──────────────────────────────────────────────

function escapeHtml(valor) {
    if (valor === null || valor === undefined) return '';
    return String(valor)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#39;');
}

function escapeJsAttr(valor) {
    if (valor === null || valor === undefined) return '';
    return String(valor)
        .replaceAll('\\', '\\\\')
        .replaceAll("'", "\\'")
        .replaceAll('"', '&quot;')
        .replaceAll('\n', ' ')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;');
}

// ── FORMATAÇÃO ─────────────────────────────────────────────────────────────

function formatarMoeda(valor) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(valor || 0);
}

function formatarData(dataStr) {
    if (!dataStr) return '—';
    return new Date(dataStr).toLocaleDateString('pt-BR');
}

function formatarDataHora(dataStr) {
    if (!dataStr) return '—';
    return new Date(dataStr).toLocaleString('pt-BR', { dateStyle: 'short', timeStyle: 'short' });
}

function iniciais(nome) {
    return (nome || '?').split(' ').slice(0, 2).map(p => p[0]).join('').toUpperCase();
}

// ── SIDEBAR / LAYOUT ─────────────────────────────────────────────────────────

function inicializarLayout() {
    const nomeEl   = document.getElementById('admin-nome');
    const avatarEl = document.getElementById('admin-avatar');
    const nome     = obterNome();

    if (nomeEl)   nomeEl.textContent   = nome;
    if (avatarEl) avatarEl.textContent = iniciais(nome);

    const btnSair = document.getElementById('btn-sair');
    if (btnSair) btnSair.addEventListener('click', logout);

    const paginaAtual = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sidebar-link').forEach(link => {
        const href = (link.getAttribute('href') || '').toLowerCase();
        if (href && paginaAtual === href) {
            link.classList.add('ativo');
        }
    });
}

// ── MODAL ──────────────────────────────────────────────────────────────────

function abrirModal(id) {
    document.getElementById(id)?.classList.remove('hidden');
}

function fecharModal(id) {
    document.getElementById(id)?.classList.add('hidden');
}

// ── LOADING ────────────────────────────────────────────────────────────────

function mostrarLoading(containerId) {
    const el = document.getElementById(containerId);
    if (el) el.innerHTML = `
        <div class="loading">
            <div class="spinner"></div>
            Carregando...
        </div>`;
}

// CORREÇÃO: nova função para exibir erro no lugar do spinner —
// usada por todas as páginas quando apiGet() retorna ok:false,
// garantindo que nenhum container fique girando para sempre.
function mostrarErro(containerId, mensagem = 'Não foi possível carregar os dados.') {
    const el = document.getElementById(containerId);
    if (el) el.innerHTML = `
        <div class="loading" style="color:var(--vermelho);">
            ${escapeHtml(mensagem)}
        </div>`;
}