$(function () {
    $("#sortableMenu").sortable();
    $.ajax({
        type: "GET",
        url: "/Modulos/GetAllModulos",
        success: function (response) {
            console.log(response);
            response.map(item => {
                let idModulo = item.id;
                addItemToMenu(item);
                getSubmodulosForModulo(idModulo);
            });
        }
    });
});

function addItemToMenu(item) {
    $("#sortableMenu").append(
        '<li class="nav-item menu-open" id="modulo'+item.id+'">' +
            '<a href="#" class="nav-link">' +
                '<p>' + item.titulo +
                    '<i class="fas fa-angle-left right"></i>'+
                '</p>'+
            '</a>'+
        '</li>'
    );
}

function getSubmodulosForModulo(idModulo) {
    $.ajax({
        type: "POST",
        url: "/Modulos/GetAllSubModulos",
        data: { "idModulo": idModulo },
        success: function (response) {
            console.log(response);
            if (response.length > 0) {
                $("#modulo" + idModulo).append(
                    '<ul class="nav nav-treeview" id="submodulos' + idModulo + '">'
                    
                );
                response.map(item => {
                    $("#submodulos" + idModulo).append(
                        '<li class="nav-item">'+
                            '<a href="#" class= "nav-link">'+
                                '<i class="far fa-circle nav-icon"></i>'+
                                '<p>'+item.titulo+'</p>'+
                            '</a >'+
                        '</li >'
                    );
                });
                $("#modulo" + idModulo).append(
                   '</ul >'
                );
                    
            }
        }
    });
}