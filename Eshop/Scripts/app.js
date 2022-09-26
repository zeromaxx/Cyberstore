// Slideshow
let thumbnails = document.getElementsByClassName("thumbnail");
let slider = document.getElementById("slider");
let buttonRight = document.getElementById("slide-right");
let buttonLeft = document.getElementById("slide-left");

buttonLeft.addEventListener("click", function () {
    slider.scrollLeft -= 100;
});

buttonRight.addEventListener("click", function () {
    slider.scrollLeft += 100;
});

const maxScrollLeft = slider.scrollWidth - slider.clientWidth;


function autoPlay() {
    if (slider.scrollLeft > maxScrollLeft - 1) {
        slider.scrollLeft -= maxScrollLeft;
    } else {
        slider.scrollLeft += 1;
    }
}
let play = setInterval(autoPlay, 50);


for (var i = 0; i < thumbnails.length; i++) {
    thumbnails[i].addEventListener("mouseover", function () {
        clearInterval(play);
    });

    thumbnails[i].addEventListener("mouseout", function () {
        return (play = setInterval(autoPlay, 50));
    });
}

const slides = document.querySelectorAll('.mySlides');
const leftBtn = document.getElementById('left');
const rightBtn = document.getElementById('right');

let activeSlide = 0;

function setActiveSlide() {
    slides.forEach((e) => {
        e.classList.remove('active');

        slides[activeSlide].classList.add('active');
    })
}

rightBtn.addEventListener('click', () => {
    activeSlide++;
    if (activeSlide > slides.length - 1) {
        activeSlide = 0;
    }

    setActiveSlide();
})

leftBtn.addEventListener('click', () => {
    activeSlide--;
    if (activeSlide < 0) {
        activeSlide = slides.length - 1;
    }

    setActiveSlide();
})

// Navbar Toggle
const toggleBtn = document.querySelector(".sidebar-toggle");
const sideBar = document.querySelector(".sidebar");

toggleBtn.addEventListener("click", function () {
    sideBar.classList.toggle("show-sidebar");

});

