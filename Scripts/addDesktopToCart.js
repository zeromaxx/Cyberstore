$(document).ready(function () {
    $('#AddDesktopToCart_').click(function (e) {
        var cartbutton = $(e.target).attr("data-cart-id");
        $.post(`/Cart/AddDesktopToCart`).done(function (response) {
            setTimeout(function () { location.reload(); }, 1000);
        }).fail(function () {
        })

    })
})