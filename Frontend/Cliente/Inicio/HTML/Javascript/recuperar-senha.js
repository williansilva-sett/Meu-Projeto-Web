document.querySelector('#form-recuperar').addEventListener('submit', async (event) => {
    event.preventDefault();
  
    const email = document.querySelector('#email').value.trim();
    const botao = document.querySelector('.submit-button');
    const textoOriginal = botao.textContent;
  
    botao.disabled = true;
    botao.textContent = 'Enviando...';
  
    try {
      // A API sempre devolve sucesso aqui, exista ou não o email - é
      // proposital (evita revelar quais emails estão cadastrados na base).
      await ApiCliente.post('/auth/recuperar-senha', { email });
  
      document.querySelector('#form-recuperar').style.display = 'none';
      document.querySelector('#mensagem-sucesso').style.display = 'block';
    } catch (erro) {
      alert(erro.message);
      botao.disabled = false;
      botao.textContent = textoOriginal;
    }
  });