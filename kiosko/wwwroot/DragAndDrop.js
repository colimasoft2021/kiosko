Dropzone.options.DropImagen = {
    paramName: "file", // The name that will be used to transfer the file
    maxFilesize: 2, // MB
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
    acceptedFiles: ".mp4",
    

    //accept: function (file, done) {
    //    if (file.name == "justinbieber.jpg") {
    //        done("Naha, you don't.");
    //    }
    //    else { done(); }
    //}
};