document.getElementById('livesearchtags').addEventListener('keyup', function (e) {
    LiveSearch()
});

function LiveSearch() {

    if ($('#livesearchtags').val() == "") {
        $.ajax({
            type: "POST",
            url: "/Home/LiveTagSearch",
            data: { search: " " },
            datatype: "html",
            success: function (data) {
                $('#result').html(data);
                location.reload();
            }
        });

    }

    $.ajax({
        type: "POST",
        url: "/Home/LiveTagSearch",
        data: { search: $('#livesearchtags').val() },
        datatype: "html",
        success: function (data) {
            $('#result').html(data);
        },
        fail: function () {
            $('#result').html("Something went wrong, please try again");
        }
    });

}


document.getElementById('livesearchtags').addEventListener('keyup', function (e) {

    timeout = setTimeout(function () {
        LiveSearch()
    }, 800);
});