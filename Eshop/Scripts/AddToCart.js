$(document).ready(function () {
    $('.add-to-cart-btn').click(function (e) {
        var cartbutton = $(e.target).attr("data-cart-id");
        $.post(`/Cart/AddToCartProduct/${cartbutton}`).done(function (response) {
            setTimeout(function () { location.reload(); }, 1000);
        }).fail(function () {
        })


    })

    $('.add-to-cart-btn').hover(function () {
        var userId = $('#userId').val();
        if (userId == "") {
            $('.cart-div').hover(function () {
                $(this).closest('.add-to-cart-btn').find(".unlogged-user").css({
                    "bottom": "50px",
                    "visibility": "visible",
                    "opacity": "1"
                })
            })
            $('.cart-div').mouseleave(function () {
                $(this).closest('.add-to-cart-btn').find(".unlogged-user").css({
                    "bottom": "0px",
                    "visibility": "hidden",
                    "opacity": "0"
                })
            })
        }
    })



})