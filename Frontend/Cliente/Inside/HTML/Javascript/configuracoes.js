
document.addEventListener("DOMContentLoaded", () => {

    iniciarSwitches();
    iniciarTema();
    iniciarUsuario();
    iniciarExportacao();
    iniciarExcluirConta();
    iniciarAnimacoes();

});

/*=========================================
        SWITCHES
==========================================*/

function iniciarSwitches(){

    const switches = document.querySelectorAll(".switch input");

    switches.forEach((item)=>{

        item.addEventListener("change",()=>{

            if(item.checked){

                mostrarToast("Configuração ativada");

            }else{

                mostrarToast("Configuração desativada");

            }

        });

    });

}

/*=========================================
            TEMA
==========================================*/

function iniciarTema(){

    const tema = document.querySelector("select");

    if(!tema) return;

    tema.addEventListener("change",()=>{

        if(tema.value==="Escuro"){

            document.body.classList.add("dark");

            mostrarToast("Tema escuro ativado");

        }else{

            document.body.classList.remove("dark");

            mostrarToast("Tema claro ativado");

        }

    });

}

/*=========================================
        MENU USUÁRIO
==========================================*/

function iniciarUsuario(){

    const user = document.querySelector(".user");

    if(!user) return;

    user.addEventListener("click",()=>{

        mostrarToast("Menu do usuário");

    });

}

/*=========================================
        EXPORTAÇÃO
==========================================*/

function iniciarExportacao(){

    const botao = document.querySelector(".btn-export");

    if(!botao) return;

    botao.addEventListener("click",()=>{

        botao.disabled=true;

        botao.innerHTML=
        '<i class="fa-solid fa-spinner fa-spin"></i> Exportando...';

        setTimeout(()=>{

            botao.disabled=false;

            botao.innerHTML=
            '<i class="fa-solid fa-download"></i> Exportar';

            mostrarToast("Dados exportados com sucesso.");

        },2000);

    });

}

/*=========================================
        EXCLUIR CONTA
==========================================*/

function iniciarExcluirConta(){

    const botao=document.querySelector(".btn-danger");

    if(!botao) return;

    botao.addEventListener("click",()=>{

        const confirmar=confirm(
            "Tem certeza que deseja excluir sua conta?"
        );

        if(confirmar){

            mostrarToast("Conta removida.");

        }

    });

}

/*=========================================
        ANIMAÇÕES
==========================================*/

function iniciarAnimacoes(){

    const cards=document.querySelectorAll(".card");

    cards.forEach((card,index)=>{

        card.style.opacity="0";

        card.style.transform="translateY(25px)";

        setTimeout(()=>{

            card.style.transition=".45s";

            card.style.opacity="1";

            card.style.transform="translateY(0)";

        },index*120);

    });

}

/*=========================================
            TOAST
==========================================*/

function mostrarToast(texto){

    let toast=document.createElement("div");

    toast.className="toast";

    toast.innerHTML=`

        <i class="fa-solid fa-circle-check"></i>

        ${texto}

    `;

    document.body.appendChild(toast);

    setTimeout(()=>{

        toast.classList.add("show");

    },100);

    setTimeout(()=>{

        toast.classList.remove("show");

        setTimeout(()=>{

            toast.remove();

        },400);

    },2500);

}

/*=========================================
    BOTÕES
==========================================*/

document.querySelectorAll("button").forEach(botao=>{

    botao.addEventListener("mouseenter",()=>{

        botao.style.transform="translateY(-2px)";

    });

    botao.addEventListener("mouseleave",()=>{

        botao.style.transform="translateY(0px)";

    });

});

/*=========================================
        SELECT
==========================================*/

document.querySelectorAll("select").forEach(select=>{

    select.addEventListener("focus",()=>{

        select.style.borderColor="#0d7a46";

    });

    select.addEventListener("blur",()=>{

        select.style.borderColor="#d0d5dd";

    });

});

/*=========================================
        SIDEBAR
==========================================*/

document.querySelectorAll(".sidebar a").forEach(link=>{

    link.addEventListener("click",(e)=>{

        e.preventDefault();

        document.querySelectorAll(".sidebar li")
        .forEach(li=>li.classList.remove("active"));

        link.parentElement.classList.add("active");

    });

});

/*=========================================
        NOTIFICAÇÃO
==========================================*/

const sino=document.querySelector(".notification");

if(sino){

    sino.addEventListener("click",()=>{

        mostrarToast("Você possui 2 notificações.");

    });

}

/*=========================================
        BADGE
==========================================*/

const badge=document.querySelector(".badge");

if(badge){

    badge.addEventListener("click",()=>{

        mostrarToast("Backup automático ativo.");

    });

}