/*==================================================
            VIVA FINANÇAS
            RELATÓRIOS
==================================================*/

document.addEventListener("DOMContentLoaded", () => {

    iniciarGraficos();

    iniciarEventos();

});

/*==================================================
            DADOS
==================================================*/

const dadosFinanceiros = {

    receitas: [
        3200,
        3800,
        4100,
        5200,
        6100,
        8500
    ],

    despesas: [
        2100,
        2400,
        2500,
        2700,
        3100,
        3220
    ],

    meses: [

        "Jan",

        "Fev",

        "Mar",

        "Abr",

        "Mai",

        "Jun"

    ]

};

let graficoLinha;

let graficoDonut;

/*==================================================
            INICIAR
==================================================*/

function iniciarGraficos(){

    criarGraficoLinha();

    criarGraficoDonut();

}

/*==================================================
            EVENTOS
==================================================*/

function iniciarEventos(){

    const botao=document.querySelector(".export-btn");

    if(botao){

        botao.addEventListener("click",exportarRelatorio);

    }

    const periodo=document.getElementById("chartPeriod");

    if(periodo){

        periodo.addEventListener("change",(e)=>{

            alterarPeriodo(

                e.target.value

            );

        });

    }

}

/*==================================================
            EXPORTAR
==================================================*/

function exportarRelatorio(){

    alert(

        "Em breve você poderá exportar em PDF e Excel."

    );

}

/*==================================================
            ALTERAR PERÍODO
==================================================*/

function alterarPeriodo(periodo){

    console.log(

        "Período:",

        periodo

    );

}

/*==================================================
            GRÁFICO LINHA
==================================================*/

function criarGraficoLinha(){

    const canvas=document.getElementById("lineChart");

    if(!canvas) return;

    graficoLinha=new Chart(canvas,{

        type:"line",

        data:{

            labels:dadosFinanceiros.meses,

            datasets:[

                {

                    label:"Receitas",

                    data:dadosFinanceiros.receitas,

                    borderColor:"#22c55e",

                    backgroundColor:"rgba(34,197,94,.15)",

                    fill:true,

                    tension:.4,

                    borderWidth:3,

                    pointRadius:5,

                    pointHoverRadius:7

                },

                {

                    label:"Despesas",

                    data:dadosFinanceiros.despesas,

                    borderColor:"#ef4444",

                    backgroundColor:"rgba(239,68,68,.12)",

                    fill:true,

                    tension:.4,

                    borderWidth:3,

                    pointRadius:5,

                    pointHoverRadius:7

                }

            ]

        },

        options:{

            responsive:true,

            maintainAspectRatio:false,

            interaction:{

                mode:"index",

                intersect:false

            },

            plugins:{

                legend:{

                    position:"top"

                }

            },

            scales:{

                y:{

                    beginAtZero:true

                }

            }

        }

    });

}

/*==================================================
            GRÁFICO DONUT
==================================================*/

function criarGraficoDonut(){

    const canvas=document.getElementById("donutChart");

    if(!canvas) return;

    graficoDonut=new Chart(canvas,{

        type:"doughnut",

        data:{

            labels:[

                "Moradia",

                "Alimentação",

                "Transporte",

                "Lazer",

                "Outros"

            ],

            datasets:[{

                data:[40,25,15,10,10],

                backgroundColor:[

                    "#3b82f6",

                    "#22c55e",

                    "#facc15",

                    "#8b5cf6",

                    "#94a3b8"

                ],

                borderWidth:0,

                hoverOffset:15

            }]

        },

        options:{

            responsive:true,

            maintainAspectRatio:false,

            cutout:"70%",

            plugins:{

                legend:{

                    display:false

                }

            }

        }

    });

}

/*==================================================
            ATUALIZAR CARDS
==================================================*/

function atualizarCards(

    receitas,

    despesas

){

    const saldo=receitas-despesas;

    console.log(

        saldo

    );

}