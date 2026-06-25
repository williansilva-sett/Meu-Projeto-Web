const elementos = document.querySelectorAll(
    '.hero, .card, .funcionalidade, .seguranca, .cta'
);

elementos.forEach((elemento, index) => {
    elemento.classList.add('animar');
    elemento.style.animationDelay = `${index * 0.15}s`;
});
