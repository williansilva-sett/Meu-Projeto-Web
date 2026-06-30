/*=========================================
    DASHBOARD.JS
=========================================*/

document.addEventListener("DOMContentLoaded", () => {

    /*=====================================
            SAUDAÇÃO
    =====================================*/

    const titulo = document.querySelector(".welcome h1");

    if (titulo) {

        const hora = new Date().getHours();

        let saudacao = "Bem-vinda de volta";

        if (hora >= 5 && hora < 12) {
            saudacao = "Bom dia";
        } else if (hora >= 12 && hora < 18) {
            saudacao = "Boa tarde";
        } else {
            saudacao = "Boa noite";
        }

        titulo.innerHTML = `${saudacao}, Jiara! 👋`;
    }

    /*=====================================
            CONTADOR DOS CARDS
    =====================================*/

    const numeros = document.querySelectorAll(".card h2");

    numeros.forEach(card => {

        const texto = card.innerText;

        if (!texto.includes("R$")) return;

        const valor = Number(
            texto
                .replace("R$", "")
                .replace(/\./g, "")
                .replace(",", ".")
                .trim()
        );

        let atual = 0;

        const incremento = valor / 70;

        function animar() {

            atual += incremento;

            if (atual >= valor) {

                atual = valor;

            }

            card.innerHTML =
                "R$ " +
                atual.toLocaleString("pt-BR", {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });

            if (atual < valor) {

                requestAnimationFrame(animar);

            }

        }

        animar();

    });

    /*=====================================
            HOVER DOS BOTÕES
    =====================================*/

    const botoes = document.querySelectorAll(".action-card");

    botoes.forEach(botao => {

        botao.addEventListener("mouseenter", () => {

            botao.style.transform = "translateY(-10px) scale(1.03)";

        });

        botao.addEventListener("mouseleave", () => {

            botao.style.transform = "";

        });

    });

    /*=====================================
        HOVER DAS TRANSAÇÕES
    =====================================*/

    const transacoes = document.querySelectorAll(".transaction");

    transacoes.forEach(item => {

        item.addEventListener("mouseenter", () => {

            item.style.background = "#F8FAFC";
            item.style.borderRadius = "12px";
            item.style.paddingLeft = "10px";
            item.style.paddingRight = "10px";

        });

        item.addEventListener("mouseleave", () => {

            item.style.background = "";
            item.style.paddingLeft = "";
            item.style.paddingRight = "";

        });

    });

    /*=====================================
            NOTIFICAÇÃO
    =====================================*/

    const sino = document.querySelector(".notification");

    if (sino) {

        sino.addEventListener("click", () => {

            sino.animate([
                { transform: "rotate(0deg)" },
                { transform: "rotate(18deg)" },
                { transform: "rotate(-18deg)" },
                { transform: "rotate(18deg)" },
                { transform: "rotate(0deg)" }
            ], {
                duration: 500
            });

        });

    }

    /*=====================================
            ANIMAÇÃO DOS CARDS
    =====================================*/

    const cards = document.querySelectorAll(".card");

    cards.forEach((card, index) => {

        card.style.opacity = "0";
        card.style.transform = "translateY(30px)";

        setTimeout(() => {

            card.style.transition = ".6s";

            card.style.opacity = "1";
            card.style.transform = "translateY(0px)";

        }, index * 150);

    });

    /*=====================================
            CHART.JS
    =====================================*/

    const canvas = document.getElementById("financeChart");

    if (canvas) {

        new Chart(canvas, {

            type: "line",

            data: {

                labels: [
                    "Jan",
                    "Fev",
                    "Mar",
                    "Abr",
                    "Mai",
                    "Jun"
                ],

                datasets: [

                    {

                        label: "Receitas",

                        data: [
                            2200,
                            3800,
                            4200,
                            5200,
                            6100,
                            8500
                        ],

                        borderColor: "#16A34A",

                        backgroundColor: "rgba(22,163,74,.12)",

                        borderWidth: 4,

                        fill: true,

                        tension: .4,

                        pointRadius: 5,

                        pointHoverRadius: 7

                    },

                    {

                        label: "Despesas",

                        data: [
                            1800,
                            2100,
                            2600,
                            2500,
                            3000,
                            3220
                        ],

                        borderColor: "#DC2626",

                        backgroundColor: "rgba(220,38,38,.08)",

                        borderWidth: 4,

                        fill: true,

                        tension: .4,

                        pointRadius: 5,

                        pointHoverRadius: 7

                    }

                ]

            },

            options: {

                responsive: true,

                maintainAspectRatio: false,

                plugins: {

                    legend: {

                        position: "top",

                        labels: {

                            usePointStyle: true,

                            padding: 20,

                            font: {

                                family: "Poppins",

                                size: 14

                            }

                        }

                    }

                },

                scales: {

                    x: {

                        grid: {

                            display: false

                        }

                    },

                    y: {

                        beginAtZero: true,

                        ticks: {

                            callback: function(value){

                                return "R$ " + value;

                            }

                        }

                    }

                }

            }

        });

    }

});