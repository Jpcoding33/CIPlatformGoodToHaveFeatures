//validation for comment 
$('#comment-text-area').keyup(function () {
let comment = $('#comment-text-area').val();
    if (comment === "") {
        $("#comment-text-req").removeClass('d-none');
    }
    else {
        $("#comment-text-req").addClass('d-none')
    }
});

//mission application
function mApplication(missionId) {
    $.ajax({
        type: 'POST',
        url: '/VolPage/MissionApplication',
        data: { "missId": missionId },
        success: function () {
            location.reload();
        },
        error: function () {
            console.log('error');
        },
    });
}

//change Rating by user
function changeRating(rating, missionId) {
    $.ajax({
        type: 'POST',
        url: '/VolPage/changeRatingByUser',
        data: { "rating": rating, "missionId": missionId },
        success: function () {
            location.reload();
        },
        error: function () {
            console.log('error');
        },
    });
}

//comment post by user
function postComment(missionId) {
    let commentText = $('#comment-text-area').val();
    if (commentText !== "") {
        $.ajax({
            type: 'POST',
            url: '/VolPage/addComment',
            data: { "missId": missionId, "commentText": commentText },
            success: function (data) {
                $('#comment-text-area').val('');
                var result = $($.parseHTML(data)).find("#volunteer-comments-section").html();
                $("#volunteer-comments-section").html(result);
            },
            error: function () {
                console.log('error');
            },
        });
        $("#comment-text-req").addClass('d-none');
    }
    else {
        $('#comment-text-area').focus();
        $("#comment-text-req").removeClass('d-none');
    }
}


$('textarea').keyup(function () {

    var characterCount = $(this).val().length,
        current = $('#current'),
        maximum = $('#maximum'),
        theCount = $('#the-count');

    current.text(characterCount);

    if (characterCount >= 600) {
        maximum.css('color', '#8f0001');
        current.css('color', '#8f0001');
        theCount.css('font-weight', 'bold');
    }
    else {
        maximum.css('color', '#666');
        theCount.css('font-weight', 'normal');
        current.css('color', '#666');
    }
});

