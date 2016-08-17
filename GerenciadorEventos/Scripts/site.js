function mensagem(caminho, tipo, titulo, mensagem, eventoOk) {
    $.ajax({
        url: caminho + "?tipo=" + tipo + "&titulo=" + titulo + "&mensagem=" + encodeURIComponent(mensagem),
        type: "GET",
        contentType: 'charset=utf-8',
        success: function (data, textStatus, jqXHR) {
            $('#divMensagem').html(data);
            $('#dvModalMensagem').modal();
            $('#btnOkMsg').on("click", eventoOk);
        }
    })
}

function mensagemDecisao(caminho, tipo, titulo, mensagem, eventoSim) {
    $.ajax({
        url: caminho + "?tipo=" + tipo + "&titulo=" + titulo + "&mensagem=" + encodeURIComponent(mensagem),
        type: "GET",
        contentType: 'charset=utf-8',
        success: function (data, textStatus, jqXHR) {
            $('#divMensagem').html(data);
            $('#dvModalMensagemDecisao').modal();
            $('#btnSim').on("click", eventoSim);
        }
    })
}

//Mostrar loading
function AjaxBegin() {
    //Mostra o loading
    $('#ModalLoading').show();
    $("#ModalLoading").modal({
        keyboard: false
    });
}
//fechar loading
function AjaxComplete() {
    $("#ModalLoading").modal('hide');
}

/*Remover class do body que move a tela para o lado*/
$(document).ready(function () {
    $('.modal').on('show.bs.modal', function () {
        if ($(document).height() > $(window).height()) {
            // no-scroll
            $('body').addClass("modal-open-noscroll");
        }
        else {
            $('body').removeClass("modal-open-noscroll");
        }
    })
    $('.modal').on('hide.bs.modal', function () {
        $('body').removeClass("modal-open-noscroll");
    })
})

$(document).ready(function () {
    $("body").tooltip({ selector: '[data-toggle=tooltip]' });
});

//Verifica se uma data é verdadeira
function verificaData(Data) {
    Data = Data.substring(0, 10);
    var dma = -1;
    var data = Array(3);
    var ch = Data.charAt(0);
    for (i = 0; i < Data.length && ((ch >= '0' && ch <= '9') || (ch == '/' && i != 0)) ;) {
        data[++dma] = '';
        if (ch != '/' && i != 0)
            return false;
        if (i != 0) ch = Data.charAt(++i);
        if (ch == '0') ch = Data.charAt(++i);
        while (ch >= '0' && ch <= '9') {
            data[dma] += ch; ch = Data.charAt(++i);
        }
    }
    if (ch != '') return false;
    if (data[0] == '' || isNaN(data[0]) || parseInt(data[0]) < 1)
        return false;
    if (data[1] == '' || isNaN(data[1]) || parseInt(data[1]) < 1 || parseInt(data[1]) > 12)
        return false;
    if (data[2] == '' || isNaN(data[2]) || ((parseInt(data[2]) < 0 || parseInt(data[2]) > 99) && (parseInt(data[2]) < 1900 || parseInt(data[2]) > 9999))) return false;
    if (data[2] < 50) data[2] = parseInt(data[2]) + 2000;
    else if (data[2] < 100) data[2] = parseInt(data[2]) + 1900;
    switch (parseInt(data[1])) {
        case 2: {
            if (((parseInt(data[2]) % 4 != 0 || (parseInt(data[2]) % 100 == 0 && parseInt(data[2]) % 400 != 0)) && parseInt(data[0]) > 28) || parseInt(data[0]) > 29) return false; break;
        } case 4: case 6: case 9: case 11: {
            if (parseInt(data[0]) > 30) return false; break;
        } default: {
            if (parseInt(data[0]) > 31) return false;
        }
    } return true;
};

//Formato 24 horas
function FormatoHora(hora) {
    if (!/^\d{2}:\d{2}$/.test(hora)) return false;
    var parts = hora.split(':');
    if (parts[0] > 23 || parts[1] > 59) return false;
    return true;
}

//Verifica se é numero
function verificaNumero(e) {
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        return false;
    }
}

//Verifica se é numero e/ou traço
function verificaNumeroTraco(e) {
    if (e.which != 8 && e.which != 0 && e.which != 45 && (e.which < 48 || e.which > 57)) {
        return false;
    }
}