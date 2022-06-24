
$(function () {
    $("#sortableMenu").sortable();
    getAllModulos();
});

var lastIdModulo = 0;
const urlParams = new URLSearchParams(window.location.search);
var idCurremtModulo = urlParams.get('id');
console.log(idCurremtModulo);

function getAllModulos() {
    $("#sortableMenu").empty();
    $.ajax({
        type: "GET",
        url: "/Modulos/GetAllModulos",
        success: function (response) {
            console.log(response);
            lastIdModulo = response.length + 1;
            let menuOpen = "menu-open";
            response.map(item => {
                addItemToMenu(item, menuOpen);
                menuOpen = "";
            });
            addButtonToMenu();
            addButtonsToSubMenu();
            $(".modulo" + idCurremtModulo).children("a").addClass("submenu");
        }
    });
}



function addItemToMenu(item, menuOpen) {
    let desplegable = item.desplegable;
    let titulo = item.titulo;
    let padre = item.padre;
    let idModulo = item.idModulo;
    let menuItems = "";
    let margin = (padre) ? "marginUl" : "";
    if (desplegable == 1) {
        menuItems += '<li class="nav-item ' + menuOpen + ' ' + margin + '" >';
        menuItems += '<a href="#" class="nav-link">';
        menuItems += '<p>' + titulo + '<i class="fas fa-angle-left right"></i></p>';
        menuItems += '</a>';
        menuItems += '<ul class="nav nav-treeview" id="' + idModulo + '" title="' + titulo + '">';
        menuItems += '</ul>';
        menuItems += '</li>';
    } else if (desplegable != 1 && padre !== null) {
        menuItems += '<li class="nav-item modulo' + item.id + '" id="' + idModulo + '">';
        menuItems += '<a href="/Modulos/Details?id=' + item.id + '" class="nav-link">';
        menuItems += '<p style="margin-left:20px">' + titulo + '</p>';
        menuItems += '</a>';
        menuItems += '</li>';
    } else {
        menuItems += '<li class="nav-item modulo' + item.id + '" id="' + idModulo + '">';
        menuItems += '<a href="/Modulos/Details?id=' + item.id + '" class="nav-link">';
        menuItems += '<p>' + titulo + '</p>';
        menuItems += '</a>';
        menuItems += '</li>';
    }
    if (padre !== null) {
        $("#" + padre).append(menuItems);
    } else {
        $("#sortableMenu").append(menuItems);
    }
}

function addButtonToMenu() {
    let title = "'Agregar nuevo Modulo'";
    let lastId = "modulo" + lastIdModulo;
    lastId = "'" + lastId + "'";
    $("#sortableMenu").append(
        '<button type="button " class="btn btn-block btnModulo mb-4" onclick="openModalNewModulo(' + title + ', ' + lastId + ')">+ Módulo</button>'
    );
}

function addButtonsToSubMenu() {
    $("#sortableMenu").find("ul").each(function () {
        let idElement = $(this).attr('id');
        let submodulo = $(this).attr('title');
        let title = "Agregar nuevo Submódulo a " + submodulo;
        title = "'" + title + "'";
        let lastId = "modulo" + lastIdModulo;
        lastId = "'" + lastId + "'";
        let padre = "'" + idElement + "'";
        $("#" + idElement).append(
            '<button type="button " class=" btn btnSubMenu" onclick="openModalNewModulo(' + title + ', ' + lastId + ', ' + padre + ')">+ Submódulo</button>'
        );
    });
}

var jsonNewModulo = {
    "Id": 0,
    "Titulo": "",
    "AccesoDirecto": 1,
    "Orden": 1,
    "Desplegable": 0,
    "IdModulo": "",
    "Padre": null,
    "Url": "",
    "Files": "",
    "Favorito": "",
    "TipoGuia": "",
    "UrlFondo": "",
    "BackgroundColor": ""
}

function openModalNewModulo(title, lastId, padre) {
    console.log(title);
    console.log(lastId);
    console.log(padre);
    $("#divTipo_guia").hide();
    jsonNewModulo = {
        "Id": 0,
        "Titulo": "",
        "AccesoDirecto": 1,
        "Orden": 1,
        "Desplegable": 0,
        "IdModulo": "",
        "Padre": null,
        "Url": "",
        "Files": "",
        "Favorito": false,
        "TipoGuia": "",
        "UrlFondo": "",
        "BackgroundColor": ""
    }
    if (padre) {
        let arrayLastId = lastId.split("-");
        let lastIndex = parseInt([arrayLastId.length - 1]);
        let index = lastId.lastIndexOf("-");
        let newModuloId = lastId.substring(0, index);
        newModuloId = newModuloId + '-' + lastIndex;
        jsonNewModulo.IdModulo = newModuloId;
        jsonNewModulo.Padre = padre;
    } else {
        delete jsonNewModulo.Padre;
    }
    jsonNewModulo.IdModulo = lastId;
    $("#modalTitle").text(title);
    $("#modalNewModulo").modal("show");
}

function saveNewModulo() {
    
    let title = $("#titleNewModulo").val();
    let desplegable = $('input[name="radioDesplegable"]:checked').val();
    //let value = $("#imagenIcono").prop("files");
    let value2 = $("#addFondoSubModulo").prop("files");
    let favorito = $("#guiasRapidas").prop("checked");
    let tipo_guia = $("#tipo_guia").val();
    let background = $("#addBackgroundColor").val();
    if (favorito == false) {
        tipo_guia = ""
    }
    console.log(tipo_guia);
    //if (value[0]) {
    //    jsonNewModulo.Url = window.location.origin + '/files/' + value[0].name;
    //    jsonNewModulo.Files = value[0];
    //}
    if (value2[0]) {
        /*jsonNewModulo.UrlFondo = window.location.origin + '/files/' + value2[0].name;*/
        var valDim = $("#addFondoSubModulo").attr("customValue");
        if (valDim != "") {
            jsonNewModulo.Files = value2[0];
        }
    }

    parseInt(desplegable);
    jsonNewModulo.Titulo = title;
    jsonNewModulo.Desplegable = desplegable;
    jsonNewModulo.Favorito = favorito;
    jsonNewModulo.TipoGuia = tipo_guia;
    jsonNewModulo.BackgroundColor = background;
    jsonNewModulo.UrlFondo = $("#addFondoSubModulo").attr("customValue");
    const formData = new FormData();
    formData.append("Id", jsonNewModulo.Id);
    formData.append("Titulo", jsonNewModulo.Titulo);
    formData.append("AccesoDirecto", jsonNewModulo.AccesoDirecto);
    formData.append("Orden", jsonNewModulo.Orden);
    formData.append("Desplegable", jsonNewModulo.Desplegable);
    formData.append("IdModulo", jsonNewModulo.IdModulo);
    formData.append("Padre", jsonNewModulo.Padre);
    formData.append("Url", jsonNewModulo.Url);
    formData.append("Files", jsonNewModulo.Files);
    formData.append("Favorito", jsonNewModulo.Favorito);
    formData.append("TipoGuia", jsonNewModulo.TipoGuia);
    formData.append("UrlFondo", jsonNewModulo.UrlFondo);
    formData.append("BackgroundColor", jsonNewModulo.BackgroundColor);
    console.log(jsonNewModulo);
    
    $.ajax({
        type: "POST",
        url: "/Modulos/SaveMenuModulo",
        headers: { "RequestVerificationToken": "@GetAntiXsrfRequestToken()" },
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log('saved');

            let id = response.id;
            let urlModulo = window.location.origin + '/Modulos/Details?id=' + id;
            
            Swal.fire({
                title: 'Bien!',
                text: 'La información del modulo se ha agregado con exito!',
                imageUrl: '/img/señalverde.png',
                type: 'success',
                showConfirmButton: false,
                timer: 3000,
            }).then((result) => {
                window.location.assign(urlModulo);
                location.reload();
            });

        },
        error: function (error) {
            Swal.fire({
                title: 'Error!',
                text: 'Ha ocurrido un error al actualizar la información. Inténtelo de nuevo más tarde y si el problema persiste contacte con soporte!',
                imageUrl: '/img/señalroja.png',
                imageHeight: 212,
                timer: 5000,
            }).then((result) => {
                location.reload();
            });
        }
    });
    $("#modalNewModulo").modal("hide");
}

$('#addFondoSubModulo').ready(function () {
    //$('#addFondoSubModulo').on('input', function () {
    $('#addFondoSubModulo').on('change', function () {
        console.log("On Change")
        let value2 = $("#addFondoSubModulo").prop("files");
        var urlImage = window.location.origin + '/files/' + value2[0].name;
        var image = new Image();
        image.src = urlImage;
        image.onload = function () {
            var width = this.width;
            console.log("width", width);
            if (width > 800) {
                Swal.fire({
                    title: 'Advertencia!',
                    text: 'La imagen excede los 800 pixeles de ancho, por favor seleccioné otra imagen que cumpla con las dimensiones!',
                    imageUrl: '/img/señalroja.png',
                    imageHeight: 212,
                    timer: 5000,
                })
                //$("#addFondoSubModulo").empty();
                //$("#divSubMFondo").empty();
                //$("#divSubMFondo").append(
                //    '<label>Fondo Submódulo</label>'
                //);
                //$("#divSubMFondo").append(
                //    '<input type="file" customValue="" validDimentions="0" id="addFondoSubModulo" accept=".png"/>'
                //);
                $("#addFondoSubModulo").attr("customValue", '');
            } else {
                $("#addFondoSubModulo").attr("customValue", urlImage);
            }
        }
    });
});

function addItemEstaticToMenu(item, menuOpen) {
    let desplegable = item.desplegable;
    let titulo = item.titulo;
    let padre = item.padre;
    let idModulo = item.idModulo;
    let menuItems = "";
    let margin = (padre) ? "marginUl" : "";
    if (desplegable == 1) {
        menuItems += '<li class="nav-item ' + menuOpen + ' ' + margin + '" >';
        menuItems += '<a href="#" class="nav-link">';
        menuItems += '<p>' + titulo + '<i class="fas fa-angle-left right"></i></p>';
        menuItems += '</a>';
        menuItems += '<ul class="nav nav-treeview" id="' + idModulo + '" title="' + titulo + '">';
        menuItems += '</ul>';
        menuItems += '</li>';
    } else {
        menuItems += '<li class="nav-item modulo' + item.id + '" id="' + idModulo + '">';
        menuItems += '<a href="/Modulos/DetailsEst?id=' + item.id + '" class="nav-link">';
        menuItems += '<p>' + titulo + '</p>';
        menuItems += '</a>';
        menuItems += '</li>';
    }
    if (padre !== null) {
        $("#" + padre).append(menuItems);
    } else {
        $("#EstaticMenu").append(menuItems);
    }
}

$("#radioPrimary2").click(function () {
    $("#iconoImg").hide();
    $("#guiaRapida").show();
    $("#imagenIcono").val("");
    console.log("desplegable: seleccinar icono si");
})
$("#radioPrimary1").click(function () {
    $("#iconoImg").show();
    $("#guiaRapida").hide();
    $("#guiasRapidas").prop("checked", false);
    $("#divTipo_guia").hide();
    $("#tipo_guia").val("");
    console.log("desplegable: seleccinar icono no");
})
$("#guiasRapidas").click(function () {
    let favorito = $("#guiasRapidas").prop("checked");
    console.log(favorito);
    if (favorito == true) {
        $("#divTipo_guia").show();
    } else {
        $("#divTipo_guia").hide();
    }
})
$("#guiaRapidaa").click(function () {
    let favorito = $("#guiaRapidaa").prop("checked");
    console.log(favorito);
    if (favorito == true) {
        $("#divTipo_guia2").show();
    } else {
        $("#divTipo_guia2").hide();
    }
})

