const currentLocation = location.pathname;
const navbar = document.querySelector('.navbar');
const navLinks = document.querySelectorAll('.nav-link');
const navLogo = document.querySelector('.nav-logo');
const navSearch = document.querySelector('.navbar-search-container');
const whiteSection = document.querySelector('.white-section');
const navbarBrand = document.querySelector('.navbar-brand');

if (currentLocation == "/" || currentLocation == "/Home/Index") {
    navbar.style.backgroundColor = "black";
    navSearch.style.backgroundColor = "#f5f5f5";
} else {
    navbar.style.backgroundColor = "white";
    navbar.style.borderBottom = "0.5px solid #e5e7f2";
    whiteSection.style.backgroundColor = "white";
    navLogo.style.width = "255px";
    navbarBrand.style.height = "42px";
    navLogo.src = "/Images/Icons/logo-default.png";
    navLinks.forEach((e) => {
        e.style.color = "#494949";
        e.addEventListener('mouseover', () => {
            e.style.transition = "all linear 0.3s"
            e.style.color = "#5677fc";
            e.style.fontWeight = "bold";
        })
        e.addEventListener('mouseout', () => {
            e.style.color = "#494949";
        })
    })

}