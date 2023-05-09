// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//add to fav
function addToFav(missionId, pg, flag) {
    $.ajax({
        type: 'POST',
        url: '/Common/addToFav',
        data: { "missId": missionId },
        success: function () {
            if (flag == 1) {
                loadMissions(pg);
            }
            else {
                location.reload();
            }
        },
        error: function () {
            console.log('error');
        },
    });
}

//recommend mission to user
function sendMail(missionId) {
    var recUsersList = [];
    $('.modal-body input[type="checkbox"]:checked').each(function () {
        recUsersList.push($(this).attr("id"));
    });

    $('#volPageLoader').removeClass('d-none');
    $('#modal-content').addClass('d-none');
    if (recUsersList.length != 0) {
        $.ajax({
            type: 'POST',
            url: '/Common/recToUser',
            data: { "missionId": missionId, "userIds": recUsersList },
            success: function () {
                Alert('Mission Recommended Successfully!');
                $('#volPageLoader').addClass('d-none');
                $('#modal-content').removeClass('d-none');
                $('#share-to-frnd').modal('toggle');
                $('#rec-this-miss').modal('toggle');
                $('#rec-rel-miss').modal('toggle');
            },
            error: function () {
                console.log('error');
            },
        });
    }
    else {
        Alert('Please select atleast one user!');
        $('#volPageLoader').addClass('d-none');
        $('#modal-content').removeClass('d-none');
    }
}

//alert function
function Alert(message) {
    let timerInterval
    Swal.fire({
        title: message,
        timer: 1500,
        timerProgressBar: true,
        didOpen: () => {
            Swal.showLoading()
        },
        willClose: () => {
            clearInterval(timerInterval)
        }
    }).then((result) => {
        if (result.dismiss === Swal.DismissReason.timer) {
            console.log("I was closed");
        }
    })
}

function missionClosedAlert(message) {
    Swal.fire({
        title: message,
        imageUrl: '/images/open-closed.gif',
        imageWidth: 400,
        imageHeight: 200,
        imageAlt: 'Custom image',
    })
}


function sweetAlertAnimation(message) {
    Swal.fire({
        title: message,
        showClass: {
            popup: 'animate__animated animate__fadeInDown'
        },
        hideClass: {
            popup: 'animate__animated animate__fadeOutUp'
        }
    })
}

function sweetAlertError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
    })
}