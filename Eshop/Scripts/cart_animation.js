// Arrow Cart Animation
const arrow_svg = document.querySelector(".arrow-svg");
const child = document.querySelector(".cart-svg");
const cart_div = document.querySelector(".cart-div");
const cls3 = document.querySelector(".cls-3");
const cls2 = document.querySelector(".cls-2");
const cls4 = document.querySelector(".cls-4");

$(".cart-div").on("mouseover", function () {
    $(this).find(".cls-2").css({
        fill: "white",
        transform: "translateY(5%)",
    });
    $(this).find(".cls-4").css({
        fill: "white",
        transform: "translateY(5%)",
    });

    $(this).find(".cls-3").css({
        stroke: "white",
        transform: "translateY(5%)",
    });

    $(this).find(".arrow-svg").css({
        fill: "whie",
        stroke: "white",
        "z-index": "400",
        visibility: " visible",
        transition: "all linear 0.25s",
        transform: "translateY(100%)",
    });
});

$(".cart-div").on("mouseleave", function () {
    $(this).find(".cls-2").css({
        fill: "#959795",
        transform: "translateY(-5%)",
    });
    $(this).find(".cls-4").css({
        fill: "#959795",
        transform: "translateY(-5%)",
    });

    $(this).find(".cls-3").css({
        stroke: "#959795",
        transform: "translateY(-5%)",
    });

    $(this).find(".arrow-svg").css({
        fill: "white",
        stroke: "white",
        "z-index": "400",
        visibility: " hidden",
        transition: "all linear 0.2s",
        transform: "translateY(-100%)",
    });
});
