const adminSidebar = document.querySelector('.admin-sidebar');
const test = document.querySelector('.test');
const adminSideBarBtn = document.querySelector('.admin-sidebar-toggle');
const arrowDown = document.querySelectorAll('.arrow-down');
const dropdown = document.querySelectorAll('.dropdown');
const mediaQuery = window.matchMedia('(max-width: 1200px)');

/* Admin Sidebar Toggle */
mediaQuery.addListener(function (e) {
    var executed = false;
    if (e.matches && !executed) {
        setTimeout(function () { location.reload(); }, 1000);
        executed = true;
    }
})


adminSidebar.addEventListener('click', function (e) {
    const clicked = e.target.closest('.arrow-down');
    if (!clicked) return;
    document.querySelector(`.dropdown--${clicked.dataset.tab}`)
        .classList.toggle('dropdown--active');
})

$('.admin-sidebar-toggle').on('click', () => {
    $('.admin-sidebar-logo span')
        .toggleClass('hide-logo');
    $('.admin-sidebar')
        .toggleClass('show-admin-sidebar');
})


// Modal

$('.btn--show-modal').on('click', (e) => {
    e.preventDefault();
    $('.modal').removeClass('hidden');
    var button = $(e.target).attr("data-productId");
    $('.admin-delete-btn').on('click', () => {
        $.ajax({
            url: "/Admin/Delete/" + button,
        }).done(() => {
            $('.modal').addClass('hidden');
            location.reload(true);
        }).fail(() => {
            
        })
    })

})

$('.btn--close-modal').on('click', () => {
    $('.modal').addClass('hidden');
})

$(document).bind('keydown', function (e) {
    if (event.keyCode == 27) {
        $('.modal').addClass('hidden');
    }
});

/* Actions Dropdown */

$('.actions-btn').on('click', function (e) {
    e.stopPropagation();
    $(this)
        .closest('.admin-btns-container')
        .find('.sub-menu-container')
        .toggle('show-sub-menu-container');
});


/* close menu when clicked outside of div */
$('body').on('click', function () {
    $('.sub-menu-container').hide();
    $('.admin-nav-dropdown-menu-container')
        .removeClass('show-admin-nav-dropdown-menu-container');
})

/* Admin Nav Dropdown menu  */

$('.account-icon').on('click', (e) => {
    e.stopPropagation();
    $('.admin-nav-dropdown-menu-container')
        .toggleClass('show-admin-nav-dropdown-menu-container');
})

/* Dark light mode */

const darkLightBtn = document.querySelector('.dark-light-mode-icon');
const body = document.querySelector('body');
const adminProductImage = document.querySelectorAll('.admin-product-img');

darkLightBtn.addEventListener('click', () => {
    if (body.classList.contains('dark-theme')) {
        darkLightBtn.src = "../Images/Icons/8665965_sun_icon.png"
        adminProductImage.forEach((e) => {
            e.style.opacity = '1';
        })
        body.classList.remove('dark-theme');
        localStorage.setItem("theme", "light-theme");
    } else {
        darkLightBtn.src = "../Images/Icons/dark_moon.png"
        body.classList.add('dark-theme');
        adminProductImage.forEach((e) => {
            e.style.opacity = '0.7';
        })
        localStorage.setItem('theme', 'dark-theme');
    }
})
if (localStorage.getItem("theme") === "dark-theme") {
    body.classList.add('dark-theme');
  
}
