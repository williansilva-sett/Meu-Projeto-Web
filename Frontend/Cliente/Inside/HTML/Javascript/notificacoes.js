/*==================================================
        VIVA FINANÇAS
        CENTRAL DE NOTIFICAÇÕES
==================================================*/

class NotificationManager {

    constructor() {

        /*==========================
            ELEMENTOS
        ==========================*/

        this.button =
            document.getElementById("notificationButton");

        this.panel =
            document.getElementById("notificationPanel");

        this.list =
            document.getElementById("notificationList");

        this.counter =
            document.getElementById("notificationCount");

        this.searchInput =
            document.getElementById("notificationSearch");

        this.closeButton =
            document.getElementById("closeNotificationPanel");

        this.markReadButton =
            document.getElementById("markAllRead");

        this.clearButton =
            document.getElementById("clearNotifications");

        this.filters =
            document.querySelectorAll(".filter");

        /*==========================
            DADOS
        ==========================*/

        this.notifications = [];

        this.currentFilter = "all";

        this.search = "";

        this.storage = "vf_notifications";

        /*==========================
            INICIAR
        ==========================*/

        this.init();

    }

    /*==================================================
                    INICIALIZAÇÃO
    ==================================================*/

    init(){

        this.load();

        this.bindEvents();

        this.render();

        this.updateCounter();

    }

    /*==================================================
                    EVENTOS
    ==================================================*/

    registerEvents(){

}

    /*==================================================
                    PAINEL
    ==================================================*/

    open(){

        if(!this.panel) return;

        this.panel.classList.add("show");

    }

    close(){

        if(!this.panel) return;

        this.panel.classList.remove("show");

    }

    toggle(){

        if(!this.panel) return;

        this.panel.classList.toggle("show");

    }

    /*==================================================
                    LOCAL STORAGE
    ==================================================*/

    save(){

        localStorage.setItem(

            this.storage,

            JSON.stringify(

                this.notifications

            )

        );

    }

    load(){

        const data = localStorage.getItem(

            this.storage

        );

        if(data){

            this.notifications = JSON.parse(data);

        }

    }

    /*==================================================
                CRIAR NOTIFICAÇÃO
    ==================================================*/

    add({

        title,

        message,

        category="sistema",

        icon="fa-circle-info",

        type="info"

    }){

        const notification={

            id:Date.now(),

            title,

            message,

            category,

            icon,

            type,

            read:false,

            createdAt:new Date()

        };

        this.notifications.unshift(

            notification

        );

        this.save();

        this.render();

        this.updateCounter();

        this.toast(notification);

    }

    /*==================================================
                MÉTODOS PRONTOS
    ==================================================*/

    success(title,message,category="sistema"){

        this.add({

            title,

            message,

            category,

            icon:"fa-circle-check",

            type:"success"

        });

    }

    info(title,message,category="sistema"){

        this.add({

            title,

            message,

            category,

            icon:"fa-circle-info",

            type:"info"

        });

    }

    warning(title,message,category="sistema"){

        this.add({

            title,

            message,

            category,

            icon:"fa-triangle-exclamation",

            type:"warning"

        });

    }

    error(title,message,category="sistema"){

        this.add({

            title,

            message,

            category,

            icon:"fa-circle-xmark",

            type:"error"

        });

    }

    /*==================================================
            RENDERIZAÇÃO
==================================================*/

render(){

    let lista = [...this.notifications];

    /* Pesquisa */

    if(this.search.trim() !== ""){

        lista = lista.filter(item=>{

            return (

                item.title
                .toLowerCase()
                .includes(this.search.toLowerCase())

                ||

                item.message
                .toLowerCase()
                .includes(this.search.toLowerCase())

            );

        });

    }

    /* Filtro */

    if(this.currentFilter !== "all"){

        lista = lista.filter(item=>{

            return item.category === this.currentFilter;

        });

    }

    /* Lista vazia */

    if(lista.length===0){

        this.list.innerHTML=`

            <div class="notification-empty">

                <i class="fa-regular fa-bell-slash"></i>

                <h3>Nenhuma notificação</h3>

                <p>

                    Não existem notificações.

                </p>

            </div>

        `;

        return;

    }

    this.list.innerHTML="";

    lista.forEach(item=>{

        this.list.appendChild(

            this.createItem(item)

        );

    });

}

/*==================================================
            ITEM
==================================================*/

createItem(notification){

    const div=document.createElement("div");

    div.className="notification-item";

    if(!notification.read){

        div.classList.add("unread");

    }

    div.dataset.id=notification.id;

    div.innerHTML=`

        <div class="notification-icon ${notification.category}">

            <i class="fa-solid ${notification.icon}"></i>

        </div>

        <div class="notification-content">

            <div class="notification-title">

                ${notification.title}

            </div>

            <div class="notification-message">

                ${notification.message}

            </div>

            <div class="notification-time">

                ${this.relativeDate(notification.createdAt)}

            </div>

        </div>

    `;

    div.addEventListener("click",()=>{

        this.markAsRead(notification.id);

    });

    return div;

}

/*==================================================
            CONTADOR
==================================================*/

updateCounter(){

    const unread=this.notifications.filter(item=>{

        return !item.read;

    }).length;

    this.counter.textContent=unread;

    if(unread===0){

        this.counter.classList.add("hide");

    }else{

        this.counter.classList.remove("hide");

    }

}

/*==================================================
            DATA RELATIVA
==================================================*/

relativeDate(data){

    const agora=new Date();

    const criada=new Date(data);

    const diferenca=agora-criada;

    const segundos=Math.floor(diferenca/1000);

    const minutos=Math.floor(segundos/60);

    const horas=Math.floor(minutos/60);

    const dias=Math.floor(horas/24);

    if(segundos<60){

        return "Agora";

    }

    if(minutos<60){

        return `Há ${minutos} min`;

    }

    if(horas<24){

        return `Há ${horas} h`;

    }

    if(dias===1){

        return "Ontem";

    }

    if(dias<7){

        return `Há ${dias} dias`;

    }

    return criada.toLocaleDateString("pt-BR");

}

/*==================================================
            TOAST
==================================================*/

toast(notification){

    const container=document.getElementById("toastContainer");

    if(!container) return;

    const toast=document.createElement("div");

    toast.className=`toast ${notification.type}`;

    toast.innerHTML=`

        <i class="fa-solid ${notification.icon}"></i>

        <div>

            <strong>

                ${notification.title}

            </strong>

            <p>

                ${notification.message}

            </p>

        </div>

    `;

    container.appendChild(toast);

    setTimeout(()=>{

        toast.style.opacity="0";

        toast.style.transform="translateX(100%)";

        setTimeout(()=>{

            toast.remove();

        },300);

    },3500);

}

/*==================================================
            EVENTOS
==================================================*/

bindEvents(){

    /*=========================
            ABRIR
    =========================*/

    if(this.button){

        this.button.addEventListener("click",()=>{

            this.toggle();

        });

    }

    /*=========================
            FECHAR
    =========================*/

    if(this.closeButton){

        this.closeButton.addEventListener("click",()=>{

            this.close();

        });

    }

    /*=========================
            PESQUISA
    =========================*/

    if(this.searchInput){

        this.searchInput.addEventListener("input",(e)=>{

            this.search=e.target.value;

            this.render();

        });

    }

    /*=========================
            FILTROS
    =========================*/

    this.filters.forEach(botao=>{

        botao.addEventListener("click",()=>{

            this.filters.forEach(item=>{

                item.classList.remove("active");

            });

            botao.classList.add("active");

            this.currentFilter=botao.dataset.filter;

            this.render();

        });

    });

    /*=========================
            MARCAR TODAS
    =========================*/

    if(this.markReadButton){

        this.markReadButton.addEventListener("click",()=>{

            this.markAllRead();

        });

    }

    /*=========================
            LIMPAR
    =========================*/

    if(this.clearButton){

        this.clearButton.addEventListener("click",()=>{

            this.clear();

        });

    }

}

/*==================================================
            MARCAR LIDA
==================================================*/

markAsRead(id){

    const item=this.notifications.find(n=>n.id===id);

    if(!item) return;

    item.read=true;

    this.save();

    this.updateCounter();

    this.render();

}

/*==================================================
            MARCAR TODAS
==================================================*/

markAllRead(){

    this.notifications.forEach(item=>{

        item.read=true;

    });

    this.save();

    this.updateCounter();

    this.render();

}

/*==================================================
            REMOVER
==================================================*/

remove(id){

    this.notifications=this.notifications.filter(item=>{

        return item.id!==id;

    });

    this.save();

    this.updateCounter();

    this.render();

}

/*==================================================
            LIMPAR
==================================================*/

clear(){

    if(!confirm("Deseja remover todas as notificações?")){

        return;

    }

    this.notifications=[];

    this.save();

    this.updateCounter();

    this.render();

}

/*==================================================
            LIMITE DE NOTIFICAÇÕES
==================================================*/

limit(max = 100){

    if(this.notifications.length > max){

        this.notifications = this.notifications.slice(0,max);

        this.save();

    }

}

/*==================================================
            ORDENAÇÃO
==================================================*/

sort(){

    this.notifications.sort((a,b)=>{

        return new Date(b.createdAt) - new Date(a.createdAt);

    });

}

/*==================================================
            IMPORTAR
==================================================*/

import(data){

    if(!Array.isArray(data)) return;

    this.notifications = data;

    this.sort();

    this.limit();

    this.save();

    this.render();

    this.updateCounter();

}

/*==================================================
            EXPORTAR
==================================================*/

export(){

    return [...this.notifications];

}

/*==================================================
            QUANTIDADE
==================================================*/

count(){

    return this.notifications.length;

}

/*==================================================
            NÃO LIDAS
==================================================*/

unread(){

    return this.notifications.filter(item=>!item.read);

}

/*==================================================
            APAGAR LIDAS
==================================================*/

clearRead(){

    this.notifications=this.notifications.filter(item=>{

        return !item.read;

    });

    this.save();

    this.render();

    this.updateCounter();

}

/*==================================================
            CATEGORIAS
==================================================*/

gasto(titulo,mensagem){

    this.success(

        titulo,

        mensagem,

        "gasto"

    );

}

renda(titulo,mensagem){

    this.success(

        titulo,

        mensagem,

        "renda"

    );

}

meta(titulo,mensagem){

    this.info(

        titulo,

        mensagem,

        "meta"

    );

}

sistema(titulo,mensagem){

    this.info(

        titulo,

        mensagem,

        "sistema"

    );

}

/*==================================================
            ALERTAS
==================================================*/

erro(titulo,mensagem){

    this.error(

        titulo,

        mensagem,

        "sistema"

    );

}

alerta(titulo,mensagem){

    this.warning(

        titulo,

        mensagem,

        "sistema"

    );

}

/*==================================================
            RESET
==================================================*/

reset(){

    this.notifications=[];

    localStorage.removeItem(this.storage);

    this.render();

    this.updateCounter();

}

}

/*==================================================
            INSTÂNCIA GLOBAL
==================================================*/

const notifications = new NotificationManager();

/*==================================================
            API GLOBAL
==================================================*/

window.notifications = notifications;