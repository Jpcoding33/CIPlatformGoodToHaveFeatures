let searchStr = "";
let sortBy = "";
let exploreBy = "";

$(document).ready(function () {
    loadMissions(pg = 1);
});

$('#searchtab').on("keyup", function (e) {
    searchStr = $('#searchtab').val().toLowerCase();
    loadMissions(pg = 1)
});

function loadMissions(pg, sortval, exploreVal) {
    var country = [];
    $('#Country').find("input:checked").each(function (i, obj) {
        country.push($(obj).val());
    });
        
    var city = [];
    $('#City li').find("input:checked").each(function (i, obj) {
        city.push($(obj).val());
    });

    var theme = [];
    $('#MissionTheme').find("input:checked").each(function (i, obj) {
        theme.push($(obj).val());
    });

    var skill = [];
    $('#Skill').find("input:checked").each(function (i, obj) {
        skill.push($(obj).val());
    });

    if (sortval != null) {
        sortBy = sortval;
    }

    if (exploreVal != null) {
        exploreBy = exploreVal;
    }

    $('#loader').attr('style', 'height:100vh');

    $.ajax({
        url: "/Home/CardData",
        method: "POST",
        dataType: "html",
        data: { "searchStr": searchStr, "city": city, "country": country, "theme": theme, "skill": skill, 'sortval': sortBy, 'exploreval': exploreBy, "pg": pg },
        success: function (data) {
            $('#loader').addClass('d-none');
            $('#mission-list').html("");
            $('#mission-list').html(data);
        },
        error: function (error) {
            console.log(error);
        }
    });
    //$.ajax({
    //    url: "/Home/GetCities",
    //    type: "POST",
    //    data: { "countries": country },
    //    success: function (data) {
    //        $('#City').html("");
    //        $.each(data, function (i, data) {
    //            var append_me =
    //                '<li class="form-check mb-2">'
    //                + '<input class="form-check-input checkbox" type="checkbox" value=' + data.name + '>'
    //                + '<label class="form-check-label" for="">' + data.name + '</label>'
    //                + '</li>';
    //            $('#City').append(append_me);
    //        });
    //    },
    //    error: function (error) {
    //        console.log(error);
    //    }
    //});
}


function sortByLowSeats() {
    
}