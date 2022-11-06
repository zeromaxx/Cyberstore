const removeFromDesktopBtn = document.querySelectorAll('#RemoveFromDesktop');
const productData = document.querySelector('.product-data');

productData.addEventListener('click', function (e) {
    const clicked = e.target.closest('#RemoveFromDesktop').getAttribute('data-product-id');
    console.log(clicked);
    $.post(`/Desktop/RemoveFromDesktop/${clicked}`).done(function () {
        setTimeout(function () { location.reload(); }, 1000);
    })
})


