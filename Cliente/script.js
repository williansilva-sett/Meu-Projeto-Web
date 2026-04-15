fetch('http://localhost:5079/viva-financas') 
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('Erro:', error));