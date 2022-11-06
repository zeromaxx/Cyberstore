$('.buttonalex2').click(function (e) {
    var SelectedID = $(e.target).attr("data-product-id");
    console.log(SelectedID);
    $.post(`/Desktop/SendToDesktop/${SelectedID}`).done(function (response) {
        setTimeout(function () { location.reload(); }, 1000);
    }).fail(function () {
    })

})