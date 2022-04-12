
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
            $(".modulo" + idCurremtModulo).children("a").addClass("active");
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
        '<button type="button " onclick="openModalNewModulo(' + title + ', ' + lastId + ')">Agregar modulo</button>'
    );
}

function addButtonsToSubMenu() {
    $("#sortableMenu").find("ul").each(function () {
        let idElement = $(this).attr('id');
        let submodulo = $(this).attr('title');
        let title = "Agregar nuevo Submodulo a " + submodulo;
        title = "'" + title + "'";
        let lastId = "modulo" + lastIdModulo;
        lastId = "'" + lastId + "'";
        let padre = "'" + idElement + "'";
        $("#" + idElement).append(
            '<button type="button " onclick="openModalNewModulo(' + title + ', ' + lastId + ', ' + padre + ')">Agregar submódulo</button>'
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
    "Padre": null
}

function openModalNewModulo(title, lastId, padre) {
    console.log(title);
    console.log(lastId);
    console.log(padre);
    jsonNewModulo = {
        "Id": 0,
        "Titulo": "",
        "AccesoDirecto": 1,
        "Orden": 1,
        "Desplegable": 0,
        "IdModulo": "",
        "Padre": null
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
    parseInt(desplegable);
    jsonNewModulo.Titulo = title;
    jsonNewModulo.Desplegable = desplegable;
    $.ajax({
        type: "POST",
        url: "/Modulos/SaveMenuModulo",
        headers: { "RequestVerificationToken": "@GetAntiXsrfRequestToken()" },
        data: jsonNewModulo,
        success: function (response) {
            console.log('saved');

            let id = response.id;
            let urlModulo = window.location.origin + '/Modulos/Details?id=' + id;
            window.location.assign(urlModulo);
        },
        error: function (error) {
            Swal.fire({
                title: 'Error!',
                text: 'Ha ocurrido un error al actualizar la información. Inténtelo de nuevo más tarde y si el problema persiste contacte con soporte!',
                imageUrl: '/img/señalroja.png',
                imageHeight: 212,
                timer: 2000,
            })
        }
    });
    $("#modalNewModulo").modal("hide");
}