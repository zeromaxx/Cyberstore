// Navbar Toggle
const toggleBtn = document.querySelector(".sidebar-toggle");
const sideBar = document.querySelector(".sidebar");

toggleBtn.addEventListener("click", function () {
    sideBar.classList.toggle("show-sidebar");
});

