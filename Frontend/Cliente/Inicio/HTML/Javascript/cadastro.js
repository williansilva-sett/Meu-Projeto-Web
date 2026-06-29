function calcularIdade(dataNascimentoStr) {
    const nascimento = new Date(dataNascimentoStr);
    const hoje = new Date();
    let idade = hoje.getFullYear() - nascimento.getFullYear();
    const aindaNaoFezAniversarioEsseAno =
      hoje.getMonth() < nascimento.getMonth() ||
      (hoje.getMonth() === nascimento.getMonth() && hoje.getDate() < nascimento.getDate());
    if (aindaNaoFezAniversarioEsseAno) idade--;
    return idade;
  }
  
  document.querySelector('form').addEventListener('submit', async (event) => {
    event.preventDefault();
  
    const nome = document.querySelector('#nome').value.trim();
    const sobrenome = document.querySelector('#sobrenome').value.trim();
    const email = document.querySelector('#email').value.trim();
    const telefone = document.querySelector('#telefone').value.trim();
    const senha = document.querySelector('#senha').value;
    const dataNascimento = document.querySelector('#data-nascimento').value;
  
    if (!dataNascimento) {
      alert('Selecione sua data de nascimento.');
      return;
    }
  
    const idade = calcularIdade(dataNascimento);
    if (idade < 18) {
      alert('É necessário ter 18 anos ou mais para se cadastrar.');
      return;
    }
  
    const botao = document.querySelector('.submit-button');
    const textoOriginal = botao.textContent;
    botao.disabled = true;
    botao.textContent = 'Cadastrando...';
  
    try {
      await UsuarioApi.cadastrar({ nome, sobrenome, idade, telefone, email, senha });
  
      // Cadastro feito - loga automaticamente com as mesmas credenciais,
      // pra não pedir pro usuário digitar tudo de novo.
      await Auth.login(email, senha);
      window.location.href = '/Inicio/HTML/dashboard.html';
    } catch (erro) {
      alert(erro.message);
    } finally {
      botao.disabled = false;
      botao.textContent = textoOriginal;
    }
  });