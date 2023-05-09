//profile pic on hover css

$('#edit-pic-icon').hover(function () {
    $('#profileDiv').css('background-color', 'rgba(0,0,0,0.4)');
}, function () {
    $('#profileDiv').css('background-color', 'rgba(0,0,0,0)');
});

//initializing the arrays
var skillList = [];
var skillId = [];
var actives = '';

//preloaded skills
var preloadedSkills = [];
$('#user-skills li').each(function () {
    preloadedSkills.push($(this).val());
});
for (var i = 0; i < preloadedSkills.length; i++) {
    $('#available li').each(function () {
        if (($(this).val()) == preloadedSkills[i]) {
            $(this).addClass('active-skill');
            return false;
        }
    });
}
actives = $('.list-left ul li.active-skill');
actives.clone().appendTo('.list-right ul');

actives.remove();
if ($('.list-right ul li').hasClass('active-skill')) {
    $('.list-right ul li').removeClass('active-skill');
}

getSkillListAndIds();
fetchingSkillList();


//add skills

$(function () {
    $('body').on('click', '.list-group .list-group-item', function () {
        $(this).toggleClass('active-skill');
    });
    $('.list-arrows a').click(function () {
        var $a = $(this);
        if ($a.hasClass('move-left')) {
            actives = $('.list-right ul li.active-skill');
            actives.clone().appendTo('.list-left ul');
            actives.remove();
            if ($('.list-left ul li').hasClass('active-skill')) {
                $('.list-left ul li').removeClass('active-skill');
            }
        } else if ($a.hasClass('move-right')) {
            actives = $('.list-left ul li.active-skill');
            actives.clone().appendTo('.list-right ul');

            actives.remove();
            if ($('.list-right ul li').hasClass('active-skill')) {
                $('.list-right ul li').removeClass('active-skill');
            }
        }
        getSkillListAndIds();
    });

    $('#save-skills').on('click', function () {
        $('#add-skills').modal('toggle');
        $('#selected-skills').html('');
        fetchingSkillList();
    });

    $('[name="SearchDualList"]').keyup(function (e) {
        var code = e.keyCode || e.which;
        if (code == '9') return;
        if (code == '27') $(this).val(null);
        var $rows = $(this).closest('.dual-list').find('.list-group li');
        var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();
        $rows.show().filter(function () {
            var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
            return !~text.indexOf(val);
        }).hide();
    });
    $(window).on('resize', function () {
        var win = $(this);
        if (win.width() < 991) {
            $('.list-arrows .move-right i').removeClass('bi-caret-right-fill').addClass('bi-caret-down-fill');
            $('.list-arrows .move-left i').removeClass('bi-caret-left-fill').addClass('bi-caret-up-fill');
        } else {
            $('.list-arrows .move-right i').addClass('bi-caret-right-fill').removeClass('bi-caret-down-fill');
            $('.list-arrows .move-left i').addClass('bi-caret-left-fill').removeClass('bi-caret-up-fill');
        }
    });
});

function fetchingSkillList() {
    if (skillList.length > 0) {
        for (var i = 0; i < skillList.length; i++) {
            $('#selected-skills').append('<small class="mb-2">' + skillList[i] + '</small>');
        }
    }
    else {
        $('#selected-skills').append('<small class="mb-2 text-danger">No Skills Selected</small>');
    }
    if (skillId.length > 0) {
        for (var i = 0; i < skillId.length; i++) {
            let inputElement = $('<input>', {
                type: 'hidden',
                value: skillId[i],
                name: 'finalSkillList'
            });
            $('#selected-skills').append(inputElement);
        }
    }
}

function getSkillListAndIds() {
    skillList = [];
    $('.list-right ul li').map(function () {
        skillList.push($(this).text());
    });
    skillId = [];
    $('.list-right ul li').map(function () {
        skillId.push($(this).val());
    });
}

//change password

$(document).ready(function () {
    $.validator.unobtrusive.parse($("#changePassForm"));
});

function changePassword() {
    var oldPass = $('#old-password').val();
    var newPass = $('#new-password').val();
    if (oldPass != '' && newPass != null && oldPass == newPass) {
        Alert("OldPassword and New password must be different!");
    }
    else {
        if ($("#changePassForm").valid()) {
            var formData = $("#changePassForm").serialize();
            //console.log(formData);
            $.ajax({
                type: 'POST',
                url: '/User/ChangePassword',
                data: formData,
                success: function (data) {
                    if (data == 1) {
                        Alert("Password Changed Successfully!");
                        $('#change-password').modal('hide');
                        $('#change-password').on('hidden', function () {
                            $(this).removeData('modal');
                        });
                    }
                    if (data == 0) {
                        Alert('Enter valid Old Password!');
                    }
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    }
}

//country city cascade

$(document).ready(function () {
    var cId = $('#userCountry').val();
    cascadeCity(cId);
});

$('#userCountry').change(function () {
    var cId = $('#userCountry').val();
    cascadeCity(cId);
});

function cascadeCity(cId) {
    $.ajax({
        type: 'POST',
        url: '/User/CascadeCity',
        data: { "countryId": cId },
        success: function (data) {
            $('#userCity').empty();
            $.each(data.cities, function (i, city) {
                $('#userCity').append('<option value=' + city.cityId + '>' + city.name + '</option>');
            });
            //console.log(data.cityId);
            var IsExists = false;
            $('#userCity option').each(function () {
                if (this.value == data.cityId) {
                    IsExists = true;
                    return 1;
                }
            });
            if (IsExists == true) {
                $("#userCity").val(data.cityId).change();
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}



