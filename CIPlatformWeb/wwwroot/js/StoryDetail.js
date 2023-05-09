//recommend story to user
function sendStoryRec(storyId, views) {
    var recUsersList = [];
    $('.modal-body input[type="checkbox"]:checked').each(function () {
        recUsersList.push($(this).attr("id"));
    });

    $('#storyPageLoader').removeClass('d-none');
    $('#modal-content').addClass('d-none');
    if (recUsersList.length != 0) {
        $.ajax({
            type: 'POST',
            url: '/Story/RecStoryToUser',
            data: { "sId": storyId, "userIds": recUsersList, "totalViews" : views},
            success: function () {
                Alert('Story Recommended Successfully!');
                $('#storyPageLoader').addClass('d-none');
                $('#modal-content').removeClass('d-none');
                $('#rec-this-story').modal('toggle');
            },
            error: function () {
                console.log('error');
            },
        });
    }
    else {
        Alert('Please select atleast one user!');
        $('#storyPageLoader').addClass('d-none');
        $('#modal-content').removeClass('d-none');
    }
}

