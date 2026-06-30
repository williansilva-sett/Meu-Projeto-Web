/*=========================================
    REPORT.JS
=========================================*/

document.addEventListener("DOMContentLoaded", () => {

    /*=====================================
        CONTADOR DE CARACTERES
    =====================================*/

    const textarea = document.getElementById("description");
    const counter = document.getElementById("counter");

    if (textarea && counter) {

        textarea.addEventListener("input", () => {

            counter.textContent =
                `${textarea.value.length}/1000`;

        });

    }

    /*=====================================
        DRAG & DROP
    =====================================*/

    const dropArea = document.getElementById("dropArea");
    const fileInput = document.getElementById("fileInput");
    const preview = document.getElementById("previewFile");

    if (dropArea) {

        dropArea.addEventListener("click", () => {

            fileInput.click();

        });

        fileInput.addEventListener("change", () => {

            showFile(fileInput.files[0]);

        });

        ["dragenter","dragover"].forEach(event => {

            dropArea.addEventListener(event,(e)=>{

                e.preventDefault();

                dropArea.classList.add("dragover");

            });

        });

        ["dragleave","drop"].forEach(event=>{

            dropArea.addEventListener(event,(e)=>{

                e.preventDefault();

                dropArea.classList.remove("dragover");

            });

        });

        dropArea.addEventListener("drop",(e)=>{

            const file = e.dataTransfer.files[0];

            if(file){

                fileInput.files = e.dataTransfer.files;

                showFile(file);

            }

        });

    }

    function showFile(file){

        if(!file) return;

        preview.innerHTML =
        `
            <i class="fa-solid fa-file"></i>
            ${file.name}
        `;

    }

    /*=====================================
        VALIDAÇÃO
    =====================================*/

    const form = document.getElementById("reportForm");

    form.addEventListener("submit",(e)=>{

        e.preventDefault();

        const type =
            document.getElementById("reportType");

        const title =
            document.getElementById("title");

        const description =
            document.getElementById("description");

        if(
            type.value==="" ||
            title.value.trim()==="" ||
            description.value.trim()===""
        ){

            shake(type);
            shake(title);
            shake(description);

            alert("Preencha todos os campos obrigatórios.");

            return;

        }

        submitAnimation();

    });

    /*=====================================
        BOTÃO ENVIAR
    =====================================*/

    function submitAnimation(){

        const btn =
            document.querySelector(".submit-btn");

        btn.disabled=true;

        const original =
            btn.innerHTML;

        btn.innerHTML=
        `
        <i class="fa-solid fa-spinner fa-spin"></i>
        Enviando...
        `;

        setTimeout(()=>{

            btn.innerHTML=
            `
            <i class="fa-solid fa-check"></i>
            Relatório enviado
            `;

            btn.style.background="#0b944d";

        },1500);

        setTimeout(()=>{

            btn.innerHTML=original;

            btn.style.background="";

            btn.disabled=false;

            form.reset();

            counter.textContent="0/1000";

            preview.innerHTML="";

        },3500);

    }

    /*=====================================
        SHAKE
    =====================================*/

    function shake(element){

        element.animate([

            {transform:"translateX(0)"},

            {transform:"translateX(-6px)"},

            {transform:"translateX(6px)"},

            {transform:"translateX(-6px)"},

            {transform:"translateX(0)"}

        ],{

            duration:350

        });

    }

    /*=====================================
        ANIMAÇÃO DOS CARDS
    =====================================*/

    const cards =
        document.querySelectorAll(
            ".report-card,.info-card,.types-card,.security-card"
        );

    cards.forEach((card,index)=>{

        card.style.opacity="0";

        card.style.transform="translateY(25px)";

        setTimeout(()=>{

            card.style.transition=".6s ease";

            card.style.opacity="1";

            card.style.transform="translateY(0)";

        },index*180);

    });

    /*=====================================
        RIPPLE BUTTON
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

    const bell =
        document.querySelector(".notification");

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