// Protege a página - sem token, manda pro login.
if (!Auth.estaLogado()) {
    window.location.href = '/Inicio/HTML/login.html';
  }
  
  document.querySelector('#btn-logout').addEventListener('click', () => {
    Auth.logout();
    window.location.href = '/Inicio/HTML/login.html';
  });
  
  function formatarMoeda(valor) {
    return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }
  
  function formatarData(dataIso) {
    return new Date(dataIso).toLocaleDateString('pt-BR');
  }
  
  async function carregarSaudacao() {
    const id = Auth.getUsuarioId();
    if (!id) return;
    try {
      const usuario = await UsuarioApi.buscarPorId(id);
      document.querySelector('#saudacao').textContent = `Olá, ${usuario.nome}`;
    } catch {
      // Não é crítico - só não mostra a saudação se falhar.
    }
  }
  
  async function carregarMovimentacoes() {
    const lista = document.querySelector('#lista-movimentacoes');
    try {
      const [entradas, saidas] = await Promise.all([
        ApiCliente.get('/Entradas'),
        ApiCliente.get('/Saida'),
      ]);
  
      const todas = [
        ...entradas.map((e) => ({
          id: e.idEntrada,
          tipo: 'entrada',
          descricao: e.descricao,
          valor: e.valorEntrada,
          data: e.data,
        })),
        ...saidas.map((s) => ({
          id: s.idSaida,
          tipo: 'saida',
          descricao: 'Saída',
          valor: s.valorSaida,
          data: s.dataSaida,
        })),
      ].sort((a, b) => new Date(b.data) - new Date(a.data));
  
      const totalEntradas = entradas.reduce((soma, e) => soma + e.valorEntrada, 0);
      const totalSaidas = saidas.reduce((soma, s) => soma + s.valorSaida, 0);
  
      document.querySelector('#total-entradas').textContent = formatarMoeda(totalEntradas);
      document.querySelector('#total-saidas').textContent = formatarMoeda(totalSaidas);
      document.querySelector('#saldo').textContent = formatarMoeda(totalEntradas - totalSaidas);
  
      if (todas.length === 0) {
        lista.innerHTML = '<li class="vazio">Nenhuma movimentação ainda.</li>';
        return;
      }
  
      lista.innerHTML = todas
        .slice(0, 5)
        .map(
          (m) => `
          <li class="movimentacao-item ${m.tipo}">
            <div class="icone">
              <i class="fa-solid ${m.tipo === 'entrada' ? 'fa-arrow-up' : 'fa-arrow-down'}"></i>
            </div>
            <div class="info">
              <span class="descricao">${m.descricao}</span>
              <span class="data">${formatarData(m.data)}</span>
            </div>
            <span class="valor">${m.tipo === 'entrada' ? '+' : '-'} ${formatarMoeda(m.valor)}</span>
          </li>
        `
        )
        .join('');
    } catch (erro) {
      lista.innerHTML = `<li class="vazio">Erro ao carregar: ${erro.message}</li>`;
    }
  }
  
  async function carregarMetas() {
    const lista = document.querySelector('#lista-metas');
    try {
      const metas = await ApiCliente.get('/meta');
  
      if (metas.length === 0) {
        lista.innerHTML = '<li class="vazio">Nenhuma meta criada ainda.</li>';
        return;
      }
  
      lista.innerHTML = metas
        .slice(0, 4)
        .map(
          (m) => `
          <li class="meta-item">
            <div class="meta-info">
              <span class="nome">${m.nome}</span>
              <span class="valores">${formatarMoeda(m.valorAtual)} / ${formatarMoeda(m.valorAlvo)}</span>
            </div>
            <div class="barra-progresso">
              <div class="barra-preenchida" style="width: ${m.progresso}%"></div>
            </div>
          </li>
        `
        )
        .join('');
    } catch (erro) {
      lista.innerHTML = `<li class="vazio">Erro ao carregar: ${erro.message}</li>`;
    }
  }
  
  carregarSaudacao();
  carregarMovimentacoes();
  carregarMetas();