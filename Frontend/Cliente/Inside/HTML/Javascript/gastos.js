
let gastos = [

{
    data:"31/05/2024",
    descricao:"Supermercado",
    categoria:"Alimentação",
    metodo:"Pix",
    valor:420.90
},

{
    data:"29/05/2024",
    descricao:"Combustível",
    categoria:"Transporte",
    metodo:"Cartão",
    valor:250.00
},

{
    data:"27/05/2024",
    descricao:"Academia",
    categoria:"Saúde",
    metodo:"Pix",
    valor:89.90
},

{
    data:"24/05/2024",
    descricao:"Cinema",
    categoria:"Entretenimento",
    metodo:"Cartão",
    valor:62.00
},

{
    data:"22/05/2024",
    descricao:"Aluguel",
    categoria:"Moradia",
    metodo:"Pix",
    valor:1600.00
},

{
    data:"19/05/2024",
    descricao:"Farmácia",
    categoria:"Saúde",
    metodo:"Pix",
    valor:95.20
},

{
    data:"16/05/2024",
    descricao:"Uber",
    categoria:"Transporte",
    metodo:"Cartão",
    valor:34.80
},

{
    data:"12/05/2024",
    descricao:"Restaurante",
    categoria:"Alimentação",
    metodo:"Pix",
    valor:168.00

}

];


/*=========================================
            INICIALIZAÇÃO
==========================================*/

document.addEventListener("DOMContentLoaded",()=>{

    carregarTabela(gastos);

    iniciarBusca();

    iniciarBotoes();

    criarGraficos();

});


/*=========================================
            TABELA
==========================================*/

function carregarTabela(lista){

    const tbody=document.getElementById("expenseTable");

    tbody.innerHTML="";

    lista.forEach((gasto,index)=>{

        tbody.innerHTML+=`

        <tr>

            <td>${gasto.data}</td>

            <td>${gasto.descricao}</td>

            <td>${badgeCategoria(gasto.categoria)}</td>

            <td>${metodoPagamento(gasto.metodo)}</td>

            <td class="value">

                R$ ${gasto.valor.toFixed(2)}

            </td>

            <td>

                <div class="actions">

                    <button
                    class="btn-icon edit"
                    onclick="editar(${index})">

                    <i class="fa-solid fa-pen"></i>

                    </button>

                    <button
                    class="btn-icon delete"
                    onclick="excluir(${index})">

                    <i class="fa-solid fa-trash"></i>

                    </button>

                </div>

            </td>

        </tr>

        `;

    });

}

/*=========================================
            BADGES
==========================================*/

function badgeCategoria(cat){

    switch(cat){

        case "Alimentação":

        return `<span class="badge food">${cat}</span>`;

        case "Transporte":

        return `<span class="badge transport">${cat}</span>`;

        case "Saúde":

        return `<span class="badge health">${cat}</span>`;

        case "Moradia":

        return `<span class="badge house">${cat}</span>`;

        default:

        return `<span class="badge entertainment">${cat}</span>`;

    }

}


/*=========================================
        MÉTODO PAGAMENTO
==========================================*/

function metodoPagamento(tipo){

    if(tipo==="Pix"){

        return `

        <div class="method">

        <i class="fa-brands fa-pix pix"></i>

        Pix

        </div>

        `;

    }

    return `

    <div class="method">

        <i class="fa-regular fa-credit-card credit"></i>

        Cartão

    </div>

    `;

}


/*=========================================
            BUSCA
==========================================*/

function iniciarBusca(){

    const campo=document.querySelector(".search input");

    campo.addEventListener("keyup",()=>{

        let texto=campo.value.toLowerCase();

        let resultado=gastos.filter(item=>{

            return item.descricao
            .toLowerCase()
            .includes(texto);

        });

        carregarTabela(resultado);

    });

}

/*=========================================
            EDITAR
==========================================*/

function editar(id){

    mostrarToast("Editar gasto #"+(id+1));

}

/*=========================================
            EXCLUIR
==========================================*/

function excluir(id){

    if(confirm("Deseja excluir este gasto?")){

        gastos.splice(id,1);

        carregarTabela(gastos);

        mostrarToast("Gasto removido.");

    }

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

function iniciarBotoes(){

    document.querySelectorAll("button").forEach(btn=>{

        btn.addEventListener("mouseenter",()=>{

            btn.style.transform="translateY(-2px)";

        });

        btn.addEventListener("mouseleave",()=>{

            btn.style.transform="translateY(0)";

        });

    });

}

/*=========================================
        GRÁFICOS 
==========================================*/

function criarGraficos(){

    /*--------------------------
            Pizza
    ---------------------------*/

    const pie = document.getElementById("pieChart");

    if(pie){

        new Chart(pie,{

            type:"doughnut",

            data:{

                labels:[
                    "Alimentação",
                    "Transporte",
                    "Moradia",
                    "Saúde",
                    "Entretenimento"
                ],

                datasets:[{

                    data:[
                        650,
                        310,
                        1600,
                        180,
                        120
                    ],

                    backgroundColor:[
                        "#EF4444",
                        "#3B82F6",
                        "#F59E0B",
                        "#22C55E",
                        "#8B5CF6"
                    ],

                    borderWidth:0

                }]

            },

            options:{

                responsive:true,

                plugins:{

                    legend:{

                        position:"bottom"

                    }

                }

            }

        });

    }

    /*--------------------------
            Barras
    ---------------------------*/

    const bar=document.getElementById("barChart");

    if(bar){

        new Chart(bar,{

            type:"bar",

            data:{

                labels:[
                    "Jan",
                    "Fev",
                    "Mar",
                    "Abr",
                    "Mai",
                    "Jun"
                ],

                datasets:[{

                    label:"Gastos",

                    data:[
                        2800,
                        3100,
                        2950,
                        3400,
                        3220,
                        2900
                    ],

                    backgroundColor:"#0B6D3B",

                    borderRadius:8

                }]

            },

            options:{

                responsive:true,

                scales:{

                    y:{

                        beginAtZero:true

                    }

                }

            }

        });

    }

}

/*=========================================
        NOVO GASTO
==========================================*/

const novo=document.querySelector(".btn-primary");

if(novo){

    novo.addEventListener("click",()=>{

        const descricao=prompt("Descrição do gasto:");

        if(!descricao) return;

        const valor=parseFloat(prompt("Valor:"));

        if(isNaN(valor)) return;

        gastos.unshift({

            data:new Date().toLocaleDateString("pt-BR"),

            descricao:descricao,

            categoria:"Alimentação",

            metodo:"Pix",

            valor:valor

        });

        carregarTabela(gastos);

        mostrarToast("Novo gasto cadastrado.");

        notifications.gasto(

            "Novo Gasto",

            descrição+"cadastrado"
        );

    });

}

/*=========================================
            PAGINAÇÃO
==========================================*/

document.querySelectorAll(".pagination button")

.forEach(botao=>{

    botao.addEventListener("click",()=>{

        document

        .querySelectorAll(".pagination button")

        .forEach(btn=>btn.classList.remove("active"));

        botao.classList.add("active");

        mostrarToast("Página alterada.");

    });

});

/*=========================================
            FILTROS
==========================================*/

document

.querySelectorAll(".filters select")

.forEach(select=>{

    select.addEventListener("change",()=>{

        mostrarToast("Filtro aplicado.");

    });

});


/*==================================================
            INSTÂNCIA GLOBAL
==================================================*/

const notifications = new NotificationManager();



/*=========================================
            USUÁRIO
==========================================*/

const usuario=document.querySelector(".user");

if(usuario){

    usuario.addEventListener("click",()=>{

        mostrarToast("Menu do usuário.");

    });

}

/*=========================================
            TEMA ESCURO
==========================================*/

const tema=document.querySelector(".theme");

if(tema){

    tema.addEventListener("click",()=>{

        document.body.classList.toggle("dark");

        if(document.body.classList.contains("dark")){

            mostrarToast("Tema escuro ativado.");

        }else{

            mostrarToast("Tema claro ativado.");

        }

    });

}

/*=========================================
        ANIMAÇÃO DOS CARDS
==========================================*/

const cards=document.querySelectorAll(".summary-card,.card");

cards.forEach((card,index)=>{

    card.style.opacity="0";

    card.style.transform="translateY(25px)";

    setTimeout(()=>{

        card.style.transition=".45s";

        card.style.opacity="1";

        card.style.transform="translateY(0)";

    },index*120);

});

/*=========================================
        MENU LATERAL
==========================================*/

document

.querySelectorAll(".sidebar li")

.forEach(item=>{

    item.addEventListener("click",()=>{

        document

        .querySelectorAll(".sidebar li")

        .forEach(li=>li.classList.remove("active"));

        item.classList.add("active");

    });

});

