/*==================================================
            METAS FINANCEIRAS
==================================================*/

let metas = [

{
    id:1,
    titulo:"Comprar uma casa",
    categoria:"house",
    icone:"fa-house",
    atual:180000,
    objetivo:300000,
    prazo:"Dezembro/2028",
    status:"andamento"
},

{
    id:2,
    titulo:"Viagem para Europa",
    categoria:"travel",
    icone:"fa-plane",
    atual:18000,
    objetivo:40000,
    prazo:"Junho/2026",
    status:"andamento"
},

{
    id:3,
    titulo:"Curso de Pós-graduação",
    categoria:"study",
    icone:"fa-graduation-cap",
    atual:6000,
    objetivo:20000,
    prazo:"Janeiro/2026",
    status:"andamento"
},

{
    id:4,
    titulo:"Reserva de Emergência",
    categoria:"safe",
    icone:"fa-shield-heart",
    atual:40000,
    objetivo:50000,
    prazo:"Março/2025",
    status:"andamento"
},

{
    id:5,
    titulo:"Novo Notebook",
    categoria:"done",
    icone:"fa-laptop",
    atual:8500,
    objetivo:8500,
    prazo:"Concluída",
    status:"concluida"
}

];

/*==================================================
                INICIAR
==================================================*/

document.addEventListener("DOMContentLoaded",()=>{

    carregarMetas(metas);

    iniciarTabs();

    iniciarNovaMeta();

    criarGrafico();

});

/*==================================================
            RENDERIZAR METAS
==================================================*/

function carregarMetas(lista){

    const container=document.getElementById("goalsList");

    container.innerHTML="";

    lista.forEach(meta=>{

        const porcentagem=Math.min(
            Math.round(meta.atual/meta.objetivo*100),
            100
        );

        container.innerHTML+=`

<div class="goal">

<div class="goal-icon ${meta.categoria}">

<i class="fa-solid ${meta.icone}"></i>

</div>

<div class="goal-info">

<h4>${meta.titulo}</h4>

<span>Prazo: ${meta.prazo}</span>

<small>

R$ ${meta.atual.toLocaleString("pt-BR")}
de
R$ ${meta.objetivo.toLocaleString("pt-BR")}

</small>

</div>

<div class="progress ${corClasse(porcentagem)}">

<strong>${porcentagem}%</strong>

<div class="progress-bar">

<div
class="progress-fill"
style="
width:${porcentagem}%;
background:${corBarra(porcentagem)}
">

</div>

</div>

<p>

${meta.atual.toLocaleString("pt-BR")}

</p>

</div>

<div class="goal-value">

<strong>

R$
${(meta.objetivo-meta.atual).toLocaleString("pt-BR")}

</strong>

<span>Restante</span>

</div>

<div class="goal-actions">

<button
class="edit"
onclick="editarMeta(${meta.id})">

<i class="fa-solid fa-pen"></i>

</button>

<button
class="delete"
onclick="excluirMeta(${meta.id})">

<i class="fa-solid fa-trash"></i>

</button>

</div>

</div>

`;

    });

}

/*==================================================
            CORES
==================================================*/

function corClasse(valor){

    if(valor>=100) return "green";

    if(valor>=70) return "orange";

    if(valor>=40) return "blue";

    return "purple";

}

function corBarra(valor){

    if(valor>=100) return "#16a34a";

    if(valor>=70) return "#f59e0b";

    if(valor>=40) return "#2563eb";

    return "#7c3aed";

}

/*==================================================
            TABS
==================================================*/

function iniciarTabs(){

const tabs=document.querySelectorAll(".tab");

tabs.forEach(tab=>{

tab.addEventListener("click",()=>{

tabs.forEach(item=>item.classList.remove("active"));

tab.classList.add("active");

const texto=tab.innerText;

if(texto==="Todas as metas"){

carregarMetas(metas);

}

else if(texto==="Concluídas"){

carregarMetas(

metas.filter(

m=>m.status==="concluida"

)

);

}

else{

carregarMetas(

metas.filter(

m=>m.status==="andamento"

)

);

}

});

});

}

/*==================================================
            NOVA META
==================================================*/

function iniciarNovaMeta(){

const botao=document.querySelector(".btn-primary");

botao.addEventListener("click",()=>{

const nome=prompt("Nome da meta:");

if(!nome) return;

const objetivo=parseFloat(

prompt("Valor objetivo:")

);

if(isNaN(objetivo)) return;

metas.unshift({

id:Date.now(),

titulo:nome,

categoria:"safe",

icone:"fa-bullseye",

atual:0,

objetivo,

prazo:"Sem prazo",

status:"andamento"

});

carregarMetas(metas);

mostrarToast("Meta criada.");

});

}

/*==================================================
            EDITAR
==================================================*/

function editarMeta(id){

mostrarToast(

"Editar meta #"+id

);

}

/*==================================================
            EXCLUIR
==================================================*/

function excluirMeta(id){

if(confirm("Excluir esta meta?")){

metas=metas.filter(

meta=>meta.id!==id

);

carregarMetas(metas);

mostrarToast("Meta removida.");

}

}

/*==================================================
            TOAST
==================================================*/

function mostrarToast(texto){

const toast=document.createElement("div");

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

},300);

},2500);

}

/*==================================================
                CHART.JS
==================================================*/

function criarGrafico(){

    const canvas=document.getElementById("goalChart");

    if(!canvas) return;

    new Chart(canvas,{

        type:"doughnut",

        data:{

            labels:[
                "Moradia",
                "Viagem",
                "Educação",
                "Reserva",
                "Outros"
            ],

            datasets:[{

                data:[
                    45,
                    20,
                    12,
                    18,
                    5
                ],

                backgroundColor:[

                    "#16a34a",
                    "#2563eb",
                    "#7c3aed",
                    "#f59e0b",
                    "#94a3b8"

                ],

                borderWidth:0

            }]

        },

        options:{

            cutout:"72%",

            plugins:{

                legend:{

                    position:"bottom",

                    labels:{

                        padding:20

                    }

                }

            },

            responsive:true

        }

    });

}

/*==================================================
        CALCULADORA
==================================================*/

const calculadora=document.querySelector(".btn-outline");

if(calculadora){

calculadora.addEventListener("click",()=>{

const valor=parseFloat(

prompt("Valor da meta:")

);

if(isNaN(valor)) return;

const meses=parseInt(

prompt("Em quantos meses?")

);

if(isNaN(meses)||meses<=0) return;

const mensal=(valor/meses).toFixed(2);

mostrarToast(

`Você precisa guardar R$ ${mensal} por mês.`

);

});

}

/*==================================================
        LOCAL STORAGE
==================================================*/

function salvar(){

localStorage.setItem(

"metasFinanceiras",

JSON.stringify(metas)

);

}

function carregarStorage(){

const dados=localStorage.getItem(

"metasFinanceiras"

);

if(dados){

metas=JSON.parse(dados);

}

}

carregarStorage();

carregarMetas(metas);

/*==================================================
        OBSERVAR ALTERAÇÕES
==================================================*/

const originalCarregar=carregarMetas;

carregarMetas=function(lista){

originalCarregar(lista);

salvar();

atualizarCards();

};

/*==================================================
        CARDS
==================================================*/

function atualizarCards(){

const total=metas.length;

const investido=metas.reduce(

(s,m)=>s+m.atual,

0

);

const objetivo=metas.reduce(

(s,m)=>s+m.objetivo,

0

);

const progresso=((investido/objetivo)*100||0).toFixed(1);

const concluidas=metas.filter(

m=>m.status==="concluida"

).length;

const cards=document.querySelectorAll(".summary-card h2");

if(cards.length<4) return;

cards[0].textContent=total;

cards[1].textContent=

"R$ "+investido.toLocaleString("pt-BR");

cards[2].textContent=

progresso+"%";

cards[3].textContent=

concluidas;

}

/*==================================================
        MENU USUÁRIO
==================================================*/

const usuario=document.querySelector(".user");

if(usuario){

usuario.addEventListener("click",()=>{

mostrarToast("Perfil do usuário.");

});

}

/*==================================================
        NOTIFICAÇÃO
==================================================*/

const sino=document.querySelector(".notification");

if(sino){

sino.addEventListener("click",()=>{

mostrarToast("Você possui 2 notificações.");

});

}

/*==================================================
        MENU LATERAL
==================================================*/

document

.querySelectorAll(".sidebar li")

.forEach(item=>{

item.addEventListener("click",()=>{

document

.querySelectorAll(".sidebar li")

.forEach(li=>{

li.classList.remove("active");

});

item.classList.add("active");

});

});

/*==================================================
        TEMA ESCURO
==================================================*/

const tema=document.querySelector(".theme");

if(tema){

tema.addEventListener("click",()=>{

document.body.classList.toggle("dark");

localStorage.setItem(

"tema",

document.body.classList.contains("dark")

?"dark"

:"light"

);

});

}

if(localStorage.getItem("tema")==="dark"){

document.body.classList.add("dark");

}

/*==================================================
        ANIMAÇÕES
==================================================*/

const elementos=document.querySelectorAll(

".summary-card,.goal,.card"

);

elementos.forEach((el,index)=>{

el.style.opacity="0";

el.style.transform="translateY(20px)";

setTimeout(()=>{

el.style.transition=".45s";

el.style.opacity="1";

el.style.transform="translateY(0)";

},index*100);

});

/*==================================================
        TOOLTIPS
==================================================*/

document

.querySelectorAll("button")

.forEach(botao=>{

botao.title=botao.innerText;

});

/*==================================================
        INICIALIZAÇÃO
==================================================*/

atualizarCards();

console.log("Metas Financeiras carregadas.");