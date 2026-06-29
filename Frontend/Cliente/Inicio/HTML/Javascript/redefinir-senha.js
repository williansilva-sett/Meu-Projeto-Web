function obterTokenDaUrl() {
  const params = new URLSearchParams(window.location.search);
  return params.get('token');
}

const token = obterTokenDaUrl();

if (!token) {
  document.querySelector('#form-redefinir').style.display = 'none';
  document.querySelector('#mensagem-erro-token').style.display = 'block';
}

document.querySelector('#form-redefinir').addEventListener('submit', async (event) => {
  event.preventDefault();

  const senha = document.querySelector('#senha').value;
  const confirmarSenha = document.querySelector('#confirmar-senha').value;

  if (senha !== confirmarSenha) {
    alert('As senhas não coincidem.');
    return;
  }

  const botao = document.querySelector('.submit-button');
  const textoOriginal = botao.textContent;
  botao.disabled = true;
  botao.textContent = 'Redefinindo...';

  try {
    await ApiCliente.post('/auth/redefinir-senha', {
      token,
      novaSenha: senha,
    });

    alert('Senha redefinida com sucesso! Faça login com sua nova senha.');
    window.location.href = '/Inicio/HTML/login.html';
  } catch (erro) {
    if (erro.status === 400) {
      document.querySelector('#form-redefinir').style.display = 'none';
      document.querySelector('#mensagem-erro-token').style.display = 'block';
    } else {
      alert(erro.message);
    }
  } finally {
    botao.disabled = false;
    botao.textContent = textoOriginal;
  }
});