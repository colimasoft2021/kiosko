Dropzone.options.DropImagen = {
    paramName: "file", // The name that will be used to transfer the file
    maxFilesize: 2, // MB
    addRemoveLinks: true,
    maxFiles: 1,
    acceptedFiles: ".png",

    //accept: function (file, done) {
    //    if (file.name == "justinbieber.jpg") {
    //        done("Naha, you don't.");
    //    }
    //    else { done(); }
    //}
};

Dropzone.options.DropVideo = {
    paramName: "file", // The name that will be used to transfer the file
    maxFilesize: 400, // MB
    addRemoveLinks: true,
    acceptedFiles: ".mp4",
    

    //accept: function (file, done) {
    //    if (file.name == "justinbieber.jpg") {
    //        done("Naha, you don't.");
    //    }
    //    else { done(); }
    //}
//};

    function archivo(evt) {
    var files = evt.target.files; // FileList object

    //Obtenemos la imagen del campo "file". 
    for (var i = 0, f; f = files[i]; i++) {
        //Solo admitimos imágenes.
        if (!f.type.match('image.*')) {
            continue;
        }

        var reader = new FileReader();

        reader.onload = (function (theFile) {
            return function (e) {
                // Creamos la imagen.
                document.getElementById("list").innerHTML = ['<img class="thumb" src="', e.target.result, '" title="', escape(theFile.name), '"/>'].join('');
            };
        })(f);

        reader.readAsDataURL(f);
    }
}

document.getElementById('files').addEventListener('change', archivo, false);
