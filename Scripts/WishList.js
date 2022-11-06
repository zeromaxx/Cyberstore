$(document).ready(function () {
    $(".favourite-button").click(function (e) {
        var button = $(e.target);
        if (button.hasClass("grey-heart")) {
            $.post("/Favourites/AddFavouriteproduct", { ProductId: button.attr("data-product-id") }).
                done(function () {                   
                    button.removeClass("grey-heart");
                    button.addClass("blue-heart");
                })
                .fail(function () {
                });

        } else {
            $.ajax({
                data: { ProductId: button.attr("data-product-id") },
                url: "/Favourites/DeleteFavouriteProduct",
                method: "DELETE"
            }).done(function () {
                button
                    .removeClass("blue-heart")
                    .addClass("grey-heart");
            }).fail(function () {
            });
        }

    });

    $('.remove-wishlist-button').click(function (e) {
        var button = $(e.target);
        $.ajax({
            data: { ProductId: button.attr("data-product-id") },
            url: "/Favourites/DeleteFavouriteProduct",
            method: "DELETE"
        }).done(function () {
            button.parents('tr').fadeOut(function () {
                $(this).remove();
            })
            window.setTimeout(function () {
                location.reload();
            }, 1500)

        }).fail(function () {
        });
    })

  
})