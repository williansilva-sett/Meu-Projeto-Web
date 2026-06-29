// ATENÇÃO: o campo no HTML ainda tem id="nome" e label "NOME" (igual o
// mockup original), mas a API exige email pra login - mesma adaptação
// que fizemos no Flutter. Funciona como está, mas o ideal é trocar o
// label/placeholder no login.html pra "EMAIL" e "email@exemplo.com",
// pra não confundir quem for usar.

document.querySelector('form').addEventListener('submit', async (event) => {
    event.preventDefault();
  
    const email = document.querySelector('#nome').value.trim();
    const senha = document.querySelector('#senha').value;
  
    const botao = document.querySelector('.btn-login');
    const textoOriginal = botao.textContent;
    botao.disabled = true;
    botao.textContent = 'Entrando...';
  
    try {
      await Auth.login(email, senha);
      window.location.href = '/Inicio/HTML/dashboard.html';
    } catch (erro) {
      alert(erro.message);
    } finally {
      botao.disabled = false;
      botao.textContent = textoOriginal;
    }
  });