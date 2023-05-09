tinymce.init({
    selector: 'textarea#myStory',
    plugins: 'preview importcss autosave save directionality fullscreen pagebreak nonbreaking anchor lists wordcount help emoticons',
    menubar: false,
    statusbar: false,
    toolbar: 'undo redo | bold italic strikethrough | alignleft aligncenter alignright alignjustify | superscript subscript removeformat',
    autosave_ask_before_unload: true,
    autosave_interval: "30s",
    autosave_prefix: "{path}{query}-{id}-",
    autosave_restore_when_empty: false,
    autosave_retention: "2m",
    content_css: '//www.tiny.cloud/css/codepen.min.css',
    importcss_append: true,
    height: 300,
    toolbar_mode: 'sliding',
    setup: function (ed) {
        ed.on('keyUp', function (e) {
            var max = 40000;
            var count = CountCharacters();
            if (count >= max) {
                if (e.keyCode != 8 && e.keyCode != 46)
                    tinymce.dom.Event.cancel(e);
                document.getElementById("character_count").innerHTML = "Maximun allowed character is: 40000";

            } else {
                document.getElementById("character_count").innerHTML = count;
            }
        });

    }
});
function CountCharacters() {
    var body = tinymce.get("myStory").getBody();
    var content = tinymce.trim(body.innerText || body.textContent);
    return content.length;
};

//let files = [],
//    dragArea = document.querySelector('#dragDrop'),
//    imagePreview = document.querySelector(".previewImages"),
//    browseImages = document.querySelector('.browseImages'),
//    fileInput = document.querySelector('#dragDrop input');

//// input change event
//browseImages.addEventListener('click', () => fileInput.click());
//fileInput.addEventListener('change', () => {
//    let file = fileInput.files;

//    for (let i = 0; i < file.length; i++) {
//        if (files.every(e => e.name != file[i].name)) {
//            files.push(file[i]);
//        }
//    }

//    //dragArea.reset();
//    showImages();
//});

//const showImages = () => {
//    let images = '';
//    files.forEach((e, i) => {
//        images += `<div class="image">
//                            <img src = "${URL.createObjectURL(e)}" />
//                            <span onclick="delImage(${i})" >&times; </span>
//                       </div>`
//    });

//    imagePreview.innerHTML = images;
//}

//const delImage = (index, e) => {
//    files.splice(index, 1);
//    showImages();
//}

//dragArea.addEventListener('dragover', e => {
//    e.preventDefault();
//});

////dragArea.addEventListener('dragleave', e => {
////    e.preventDefault();
////});

//dragArea.addEventListener('drop', e => {
//    e.preventDefault();
//    fileInput.files = e.dataTransfer.files;
//    let file = e.dataTransfer.files;
//    for (let i = 0; i < file.length; i++) {
//        if (files.every(e => e.name != file[i].name)) {
//            files.push(file[i]);
//        }
//    }
//    showImages();
//});

//check if the selected mission is draft 

function getDraftStory() {
    var missionId = $('#missionDropDownList').val();
    $.ajax({
        type: 'POST',
        url: '/Story/GetDraftStoryData',
        data: { "missId": missionId},
        success: function (data) {
            if (data == 0) {
                $('#storyTitle').val(null);
                $('#dateCreated').val(null);
                $('#videoUrl').val(null);
                tinyMCE.activeEditor.setContent("");
                $('.preview-image').html("");
                $(".preview-image").imageUploader({});
                $('.submit-story').prop("disabled", true);
            }
            else {
                $('#storyTitle').val(data[0].title);
                var dt = data[0].createdAt;
                dt = dt.split('T');
                $('#dateCreated').val(dt[0]);
                tinyMCE.activeEditor.setContent(data[0].description);
                $('.preview-image').html("");
                var i = 1;
                let preloaded = [];
                if (data[0].path != null) {
                    for (let x in data) {
                        if (data[x].type != 'video') {
                            let imgPath = data[x].path;
                            var content = {
                                id: i, src: "/StoryMedia/" + imgPath
                            };
                            i++;
                            preloaded.push(content);
                        }
                        else {
                            $("#videoUrl").val(data[x].path);
                        }
                    }
                }

                $('.preview-image').imageUploader({
                    preloaded: preloaded
                });
                $('.submit-story').prop("disabled", false);
                }
        },
        error: function () {
            console.log('error');
        },
    });
}

/*! Image Uploader - v1.2.3 - 26/11/2019
Copyright (c) 2019 Christian Bayer; Licensed MIT */
!(function (e) {
    e.fn.imageUploader = function (t) {
        let n,
            i = {
                preloaded: [],
                imagesInputName: "ImageFiles",
                preloadedInputName: "preloaded",
                label: "Drag & Drop files here or click to browse",
                extensions: [".jpg", ".jpeg", ".png"],
                mimes: ["image/jpeg", "image/png", "image/jpeg"],
                maxSize: 4 * 1024 * 1024,
                maxFiles: 20,
            },
            a = this,
            s = new DataTransfer();
        (a.settings = {}),
            (a.init = function () {
                (a.settings = e.extend(a.settings, i, t)),
                    a.each(function (t, n) {
                        let i = o();
                        if (
                            (e(n).append(i),
                                i.on("dragover", r.bind(i)),
                                i.on("dragleave", r.bind(i)),
                                i.on("drop", p.bind(i)),
                                a.settings.preloaded.length)
                        ) {
                            i.addClass("has-files");
                            let e = i.find(".uploaded");
                            for (let t = 0; t < a.settings.preloaded.length; t++)
                                e.append(
                                    l(a.settings.preloaded[t].src, a.settings.preloaded[t].id, !0)
                                );
                        }
                    });
            });
        let o = function () {
            let t = e("<div>", { class: "image-uploader" });
            n = e("<input>", {
                type: "file",
                id: a.settings.imagesInputName + "-" + h(),
                name: a.settings.imagesInputName,
                accept: a.settings.extensions.join(","),
                multiple: "",
            }).appendTo(t);

            let i = e("<div>", { class: "upload-text" }).appendTo(t);
            e("<i>", { class: "bi bi-plus-lg" }).appendTo(i);
            e("<span>", { text: a.settings.label }).appendTo(i);
            e("<div>", { class: "uploaded" }).appendTo(t);
            return (
                i.on("click", function (e) {
                    d(e), n.trigger("click");
                }),
                n.on("click", function (e) {
                    e.stopPropagation();
                }),
                n.on("change", p.bind(t)),
                t
            );
        },
            d = function (e) {
                e.preventDefault(), e.stopPropagation();
            },
            l = function (t, i, o) {
                let l = e("<div>", { class: "uploaded-image" }),
                    r =
                        (e("<img>", { src: t }).appendTo(l),
                            e("<button>", { class: "delete-image" }).appendTo(l));
                e("<i>", { class: "bi bi-x" }).appendTo(r);
                if (o) {
                    l.attr("data-preloaded", !0);
                    e("<input>", {
                        type: "hidden",
                        name: a.settings.preloadedInputName + "[]",
                        value: t,
                    }).appendTo(l);
                } else l.attr("data-index", i);
                return (
                    l.on("click", function (e) {
                        d(e);
                    }),
                    r.on("click", function (t) {
                        d(t);
                        let o = l.parent();
                        if (!0 === l.data("preloaded"))
                            a.settings.preloaded = a.settings.preloaded.filter(function (e) {
                                return e.id !== i;
                            });
                        else {
                            let t = parseInt(l.data("index"));
                            o.find(".uploaded-image[data-index]").each(function (n, i) {
                                n > t && e(i).attr("data-index", n - 1);
                            }),
                                s.items.remove(t),
                                n.prop("files", s.files);
                        }
                        l.remove(),
                            o.children().length || o.parent().removeClass("has-files");
                    }),
                    l
                );
            },
            r = function (t) {
                d(t),
                    "dragover" === t.type
                        ? e(this).addClass("drag-over")
                        : e(this).removeClass("drag-over");
            },
            p = function (t) {
                d(t);
                let i = e(this),
                    o = Array.from(t.target.files || t.originalEvent.dataTransfer.files),
                    l = [];
                e(o).each(function (e, t) {
                    (a.settings.extensions && !g(t)) ||
                        (a.settings.mimes && !c(t)) ||
                        (a.settings.maxSize && !f(t)) ||
                        (a.settings.maxFiles && !m(l.length, t)) ||
                        l.push(t);
                }),
                    l.length
                        ? (i.removeClass("drag-over"), u(i, l))
                        : n.prop("files", s.files);
            },
            g = function (e) {
                return (
                    !(
                        a.settings.extensions.indexOf(
                            e.name.replace(new RegExp("^.*\\."), ".")
                        ) < 0
                    ) ||
                    (sweetAlertError(
                        `The file "${e.name
                        }" does not match with the accepted file extensions: "${a.settings.extensions.join(
                            '", "'
                        )}"`
                    ),
                        !1)
                );
            },
            c = function (e) {
                return (
                    !(a.settings.mimes.indexOf(e.type) < 0) ||
                    (sweetAlertError(
                        `The file "${e.name
                        }" does not match with the accepted mime types: "${a.settings.mimes.join(
                            '", "'
                        )}"`
                    ),
                        !1)
                );
            },
            f = function (e) {
                return (
                    !(e.size > a.settings.maxSize) ||
                    (sweetAlertError(
                        `The file "${e.name}" exceeds the maximum size of ${a.settings.maxSize / 1024 / 1024
                        }Mb`
                    ),
                        !1)
                );
            },
            m = function (e, t) {
                return (
                    !(
                        e + s.items.length + a.settings.preloaded.length >=
                        a.settings.maxFiles
                    ) ||
                    (sweetAlertError(
                        `The file "${t.name}" could not be added because the limit of ${a.settings.maxFiles} files was reached`
                    ),
                        !1)
                );
            },
            u = function (t, n) {
                t.addClass("has-files");
                let i = t.find(".uploaded"),
                    a = t.find('input[type="file"]');
                e(n).each(function (e, t) {
                    s.items.add(t),
                        i.append(l(URL.createObjectURL(t), s.items.length - 1), !1);
                }),
                    a.prop("files", s.files);
            },
            h = function () {
                return Date.now() + Math.floor(100 * Math.random() + 1);
            };
        return this.init(), this;
    };
})(jQuery);