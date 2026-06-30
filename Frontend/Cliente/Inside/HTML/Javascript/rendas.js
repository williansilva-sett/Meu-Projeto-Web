/*=========================================
    RENDAS.JS
=========================================*/

document.addEventListener("DOMContentLoaded", () => {

    /*=====================================
        FILTRO DAS ABAS
    =====================================*/

    const tabs = document.querySelectorAll(".tab");
    const rows = document.querySelectorAll("#incomeTable tr");

    tabs.forEach(tab => {

        tab.addEventListener("click", () => {

            tabs.forEach(t => t.classList.remove("active"));
            tab.classList.add("active");

            const filter = tab.dataset.filter;

            rows.forEach(row => {

                const status = row.dataset.status;

                if (filter === "all" || filter === status) {

                    row.style.display = "";

                } else {

                    row.style.display = "none";

                }

            });

        });

    });

    /*=====================================
        BOTÃO NOVA RENDA
    =====================================*/

    const newIncomeBtn = document.querySelector(".new-income-btn");

    newIncomeBtn.addEventListener("click", () => {

        const fonte = prompt("Nome da renda:");

        if (!fonte) return;

        const categoria = prompt("Categoria:");

        if (!categoria) return;

        const valor = prompt("Valor (ex: 1500):");

        if (!valor) return;

        const frequencia = prompt("Frequência:");

        if (!frequencia) return;

        const recebimento = prompt("Próximo recebimento:");

        if (!recebimento) return;

        adicionarLinha(
            fonte,
            categoria,
            parseFloat(valor),
            frequencia,
            recebimento
        );

        atualizarResumo();

    });

    /*=====================================
        ADICIONAR LINHA
    =====================================*/

    function adicionarLinha(nome, categoria, valor, frequencia, data) {

        const tbody = document.getElementById("incomeTable");

        const tr = document.createElement("tr");

        tr.dataset.status = "ativa";

        tr.innerHTML = `
            <td>
                <div class="income-name">
                    <div class="icon green">
                        <i class="fa-solid fa-wallet"></i>
                    </div>
                    ${nome}
                </div>
            </td>

            <td>${categoria}</td>

            <td>R$ ${valor.toLocaleString("pt-BR", {
                minimumFractionDigits:2
            })}</td>

            <td>${frequencia}</td>

            <td>${data}</td>

            <td>
                <span class="status active-status">
                    Ativa
                </span>
            </td>

            <td>

                <button class="action edit">

                    <i class="fa-solid fa-pen"></i>

                </button>

                <button class="action delete">

                    <i class="fa-solid fa-trash"></i>

                </button>

            </td>
        `;

        tbody.appendChild(tr);

        adicionarEventosLinha(tr);

    }

    /*=====================================
        EVENTOS DAS LINHAS
    =====================================*/

    function adicionarEventosLinha(row){

        row.querySelector(".delete")
        .addEventListener("click",()=>{

            if(confirm("Deseja excluir esta renda?")){

                row.remove();

                atualizarResumo();

            }

        });

        row.querySelector(".edit")
        .addEventListener("click",()=>{

            const nome =
                row.querySelector(".income-name").innerText.trim();

            const novoNome =
                prompt("Editar nome:",nome);

            if(novoNome){

                row.querySelector(".income-name").lastChild.textContent =
                " " + novoNome;

            }

        });

    }

    rows.forEach(adicionarEventosLinha);

    /*=====================================
        ATUALIZA RESUMO
    =====================================*/

    function atualizarResumo(){

        const linhas =
            document.querySelectorAll("#incomeTable tr");

        let total=0;

        let ativos=0;

        linhas.forEach(row=>{

            const valorTexto =
                row.children[2].innerText
                .replace("R$","")
                .replace(/\./g,"")
                .replace(",",".")
                .trim();

            const valor =
                parseFloat(valorTexto);

            if(!isNaN(valor))
                total += valor;

            if(row.dataset.status==="ativa")
                ativos++;

        });

        document.getElementById("totalIncome")
        .innerText =
        total.toLocaleString("pt-BR",{

            style:"currency",

            currency:"BRL"

        });

        document.getElementById("averageIncome")
        .innerText =
        total.toLocaleString("pt-BR",{

            style:"currency",

            currency:"BRL"

        });

        document.getElementById("activeCount")
        .innerText = ativos;

    }

    atualizarResumo();

    /*=====================================
        RIPPLE
    =====================================*/

    document.querySelectorAll("button").forEach(btn=>{

        btn.addEventListener("click",(e)=>{

            const ripple =
                document.createElement("span");

            const rect =
                btn.getBoundingClientRect();

            const size =
                Math.max(rect.width,rect.height);

            ripple.className="ripple";

            ripple.style.width=size+"px";
            ripple.style.height=size+"px";

            ripple.style.left=
            (e.clientX-rect.left-size/2)+"px";

            ripple.style.top=
            (e.clientY-rect.top-size/2)+"px";

            btn.appendChild(ripple);

            setTimeout(()=>{

                ripple.remove();

            },600);

        });

    });

    /*=====================================
        ANIMAÇÃO DOS CARDS
    =====================================*/

    const cards = document.querySelectorAll(".summary-card,.table-card");

    cards.forEach((card,index)=>{

        card.style.opacity="0";
        card.style.transform="translateY(25px)";

        setTimeout(()=>{

            card.style.transition=".6s";

            card.style.opacity="1";

            card.style.transform="translateY(0)";

        },index*180);

    });

    /*=====================================
        HOVER SIDEBAR
    =====================================*/

    document.querySelectorAll(".sidebar li").forEach(item=>{

        item.addEventListener("mouseenter",()=>{

            item.style.transform="translateX(5px)";

        });

        item.addEventListener("mouseleave",()=>{

            item.style.transform="translateX(0px)";

        });

    });

    /*=====================================
        ANIMAÇÃO DO SINO
    =====================================*/

    const bell = document.querySelector(".notification");

    if(bell){

        setInterval(()=>{

            bell.animate([

                {transform:"rotate(0deg)"},
                {transform:"rotate(-12deg)"},
                {transform:"rotate(12deg)"},
                {transform:"rotate(0deg)"}

            ],{

                duration:700,

                easing:"ease"

            });

        },7000);

    }

});