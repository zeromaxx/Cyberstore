const emailSubmitBtn = document.querySelector('#email-submit-btn');
$(document).ready(function () {

    $('#email-submit-btn').click(function (e) {
        e.preventDefault();
        if ($("#receiverEmail").val().length === 0) {
            $("#receiverEmail").addClass('emptyInput');
            $("#receiverEmail").attr('placeholder', 'The field is required.').val("").focus().blur();
           
        };

        $.ajax({
            type: "POST",
            url: "/Home/SendEmail",
            data: { receiver: $('#receiverEmail').val() },
            success: function (data) {
                $('#receiverEmail').val("");
                $("#receiverEmail").removeClass('emptyInput');
                $('#status').text(`${data.status}`);
                $('.toast-container').addClass('show-toast');
                setTimeout(function () {
                    $('.toast-container').removeClass('show-toast');
                }, 5000);
                $('#receiverEmail').attr('placeholder', 'Your Email Adress...').val("").focus().blur();
            },
            fail: function () {
                $('#emailResult').text(`${data.status}`);
            }
        });
    })
    
})