/* =========================
   GRÁFICO DE LINHA
========================= */

const linha = document
    .getElementById("linha")
    .getContext("2d");

const gradienteReceita =
    linha.createLinearGradient(
        0,
        0,
        0,
        350
    );

gradienteReceita.addColorStop(
    0,
    "rgba(0,146,69,0.35)"
);

gradienteReceita.addColorStop(
    1,
    "rgba(0,146,69,0)"
);

const gradienteDespesa =
    linha.createLinearGradient(
        0,
        0,
        0,
        350
    );

gradienteDespesa.addColorStop(
    0,
    "rgba(230,57,70,0.30)"
);

gradienteDespesa.addColorStop(
    1,
    "rgba(230,57,70,0)"
);

new Chart(linha, {
    type: "line",

    data: {
        labels: [
            "01 Mai",
            "05 Mai",
            "08 Mai",
            "12 Mai",
            "15 Mai",
            "20 Mai",
            "25 Mai",
            "31 Mai"
        ],

        datasets: [
            {
                label: "Receitas",

                data: [
                    4000,
                    5500,
                    7800,
                    6000,
                    4200,
                    7300,
                    5600,
                    9000
                ],

                borderColor: "#009245",
                backgroundColor:
                    gradienteReceita,

                fill: true,
                tension: .4,
                borderWidth: 3,

                pointRadius: 5,
                pointHoverRadius: 8,

                pointBackgroundColor:
                    "#009245"
            },

            {
                label: "Despesas",

                data: [
                    2300,
                    1500,
                    2900,
                    1600,
                    2400,
                    1800,
                    1600,
                    2700
                ],

                borderColor: "#e63946",
                backgroundColor:
                    gradienteDespesa,

                fill: true,
                tension: .4,
                borderWidth: 3,

                pointRadius: 5,
                pointHoverRadius: 8,

                pointBackgroundColor:
                    "#e63946"
            }
        ]
    },

    options: {
        responsive: true,

        animation: {
            duration: 2000,
            easing: "easeOutQuart"
        },

        plugins: {
            legend: {
                position: "top",

                labels: {
                    usePointStyle: true,
                    padding: 25
                }
            }
        },

        scales: {
            y: {
                beginAtZero: true,

                grid: {
                    color: "#ececec"
                },

                ticks: {
                    callback: function (
                        value
                    ) {
                        return (
                            "R$ " +
                            value / 1000 +
                            "k"
                        );
                    }
                }
            },

            x: {
                grid: {
                    display: false
                }
            }
        }
    }
});

/* =========================
   GRÁFICO DE ROSCA
========================= */

const pizza =
    document
        .getElementById("pizza")
        .getContext("2d");

const azul =
    pizza.createLinearGradient(
        0,
        0,
        250,
        250
    );

azul.addColorStop(
    0,
    "#60a5fa"
);

azul.addColorStop(
    1,
    "#2563eb"
);

const verde =
    pizza.createLinearGradient(
        0,
        0,
        250,
        250
    );

verde.addColorStop(
    0,
    "#4ade80"
);

verde.addColorStop(
    1,
    "#16a34a"
);

const amarelo =
    pizza.createLinearGradient(
        0,
        0,
        250,
        250
    );

amarelo.addColorStop(
    0,
    "#fde047"
);

amarelo.addColorStop(
    1,
    "#eab308"
);

const roxo =
    pizza.createLinearGradient(
        0,
        0,
        250,
        250
    );

roxo.addColorStop(
    0,
    "#c084fc"
);

roxo.addColorStop(
    1,
    "#9333ea"
);

const cinza =
    pizza.createLinearGradient(
        0,
        0,
        250,
        250
    );

cinza.addColorStop(
    0,
    "#d1d5db"
);

cinza.addColorStop(
    1,
    "#6b7280"
);

new Chart(pizza, {
    type: "doughnut",

    data: {
        labels: [
            "Moradia",
            "Alimentação",
            "Transporte",
            "Lazer",
            "Outros"
        ],

        datasets: [
            {
                data: [
                    40,
                    25,
                    15,
                    10,
                    10
                ],

                backgroundColor: [
                    azul,
                    verde,
                    amarelo,
                    roxo,
                    cinza
                ],

                borderWidth: 0
            }
        ]
    },

    options: {
        responsive: true,

        cutout: "65%",

        animation: {
            animateRotate: true,
            animateScale: true,
            duration: 2000,
            easing: "easeOutQuart"
        },

        plugins: {
            legend: {
                display: false
            },

            tooltip: {
                callbacks: {
                    label: function (
                        context
                    ) {
                        return (
                            context.label +
                            ": " +
                            context.raw +
                            "%"
                        );
                    }
                }
            }
        }
    }
});