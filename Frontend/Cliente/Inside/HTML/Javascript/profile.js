/*==================================================
    PROFILE.JS
==================================================*/

document.addEventListener("DOMContentLoaded", () => {

    /*=========================================
        MOSTRAR / OCULTAR SENHAS
    =========================================*/

    const toggleButtons = document.querySelectorAll(".toggle-password");

    toggleButtons.forEach(icon => {

        icon.addEventListener("click", () => {

            const input = icon.previousElementSibling;

            if (input.type === "password") {

                input.type = "text";

                icon.classList.remove("fa-eye");
                icon.classList.add("fa-eye-slash");

            } else {

                input.type = "password";

                icon.classList.remove("fa-eye-slash");
                icon.classList.add("fa-eye");

            }

        });

    });



    /*=========================================
        ALTERAR FOTO
    =========================================*/

    const avatar = document.querySelector(".avatar-area img");
    const topAvatar = document.querySelector(".user-menu img");

    const photoBtn = document.querySelector(".edit-photo");
    const cameraBtn = document.querySelector(".camera-btn");

    const fileInput = document.createElement("input");

    fileInput.type = "file";
    fileInput.accept = "image/*";
    fileInput.style.display = "none";

    document.body.appendChild(fileInput);

    function escolherImagem() {
        fileInput.click();
    }

    if (photoBtn)
        photoBtn.addEventListener("click", escolherImagem);

    if (cameraBtn)
        cameraBtn.addEventListener("click", escolherImagem);

    fileInput.addEventListener("change", function () {

        const file = this.files[0];

        if (!file) return;

        const reader = new FileReader();

        reader.onload = function (e) {

            avatar.src = e.target.result;
            topAvatar.src = e.target.result;

            avatar.style.transform = "scale(0.92)";

            setTimeout(() => {

                avatar.style.transform = "scale(1)";

            }, 200);

        };

        reader.readAsDataURL(file);

    });



    /*=========================================
        BOTÃO SALVAR
    =========================================*/

    const saveBtn = document.querySelector(".save-btn");

    if (saveBtn) {

        saveBtn.addEventListener("click", function (e) {

            e.preventDefault();

            saveBtn.disabled = true;

            const originalHTML = saveBtn.innerHTML;

            saveBtn.innerHTML =
                '<i class="fa-solid fa-spinner fa-spin"></i> Salvando...';

            setTimeout(() => {

                saveBtn.innerHTML =
                    '<i class="fa-solid fa-check"></i> Alterações salvas';

                saveBtn.style.background = "#0e9f4b";

            }, 1200);

            setTimeout(() => {

                saveBtn.innerHTML = originalHTML;

                saveBtn.style.background = "";

                saveBtn.disabled = false;

            }, 3000);

        });

    }



    /*=========================================
        ANIMAÇÃO DOS CARDS
    =========================================*/

    const cards = document.querySelectorAll(".card, .profile-card");

    cards.forEach((card, index) => {

        card.style.opacity = "0";
        card.style.transform = "translateY(25px)";

        setTimeout(() => {

            card.style.transition = ".6s ease";

            card.style.opacity = "1";

            card.style.transform = "translateY(0)";

        }, 150 * index);

    });



    /*=========================================
        EFEITO NOS INPUTS
    =========================================*/

    const inputs = document.querySelectorAll("input");

    inputs.forEach(input => {

        input.addEventListener("focus", () => {

            input.parentElement.classList.add("focus");

        });

        input.addEventListener("blur", () => {

            input.parentElement.classList.remove("focus");

        });

    });



    /*=========================================
        HOVER SUAVE NOS BOTÕES
    =========================================*/

    const buttons = document.querySelectorAll("button");

    buttons.forEach(btn => {

        btn.addEventListener("mouseenter", () => {

            btn.style.transition = ".3s";

        });

    });



    /*=========================================
        ANIMAÇÃO SIDEBAR
    =========================================*/

    const links = document.querySelectorAll(".sidebar li");

    links.forEach(item => {

        item.addEventListener("mouseenter", () => {

            item.style.transform = "translateX(4px)";

        });

        item.addEventListener("mouseleave", () => {

            item.style.transform = "translateX(0)";

        });

    });



    /*=========================================
        BADGE NOTIFICAÇÃO
    =========================================*/

    const notification = document.querySelector(".notification");

    if (notification) {

        setInterval(() => {

            notification.animate([
                { transform: "rotate(0deg)" },
                { transform: "rotate(-12deg)" },
                { transform: "rotate(12deg)" },
                { transform: "rotate(0deg)" }
            ], {
                duration: 700,
                easing: "ease"
            });

        }, 7000);

    }



    /*=========================================
        EFEITO RIPPLE
    =========================================*/

    buttons.forEach(button => {

        button.addEventListener("click", function (e) {

            const ripple = document.createElement("span");

            const rect = button.getBoundingClientRect();

            const size = Math.max(rect.width, rect.height);

            ripple.style.width = ripple.style.height = size + "px";

            ripple.style.left = (e.clientX - rect.left - size / 2) + "px";

            ripple.style.top = (e.clientY - rect.top - size / 2) + "px";

            ripple.className = "ripple";

            button.appendChild(ripple);

            setTimeout(() => {

                ripple.remove();

            }, 600);

        });

    });

});