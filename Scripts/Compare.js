$('.compare-btn').click(function (e) {
    var SelectedID = $(e.target).attr("data-cart-id");
    console.log(SelectedID);
    $.post(`/Home/Compare/${SelectedID}`).done(function (response) {
        setTimeout(function () { location.reload(); }, 1000);
    }).fail(function () {
    })


    console.log("Neo Script JJJ")
})