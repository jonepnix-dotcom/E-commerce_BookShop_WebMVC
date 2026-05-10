const slidePage = document.querySelector(".slide-page");
const nextBtnFirst = document.querySelector(".firstNext");
const prevBtnSec = document.querySelector(".prev-1");
const nextBtnSec = document.querySelector(".next-1");
const prevBtnThird = document.querySelector(".prev-2");
const nextBtnThird = document.querySelector(".next-2");
const prevBtnFourth = document.querySelector(".prev-3");
const submitBtn = document.querySelector(".submit");
const loginBtn = document.querySelector(".login");
const progressText = document.querySelectorAll(".step p");
const progressCheck = document.querySelectorAll(".step .check");
const bullet = document.querySelectorAll(".step .bullet");
let current = 1;


function enableButton1() {
    var nextButton = document.getElementById('nextButton1');
    var input1 = document.getElementById('name');
    if (input1.value.trim() !== "")
        nextButton.disabled = false;
    else
        nextButton.disabled = true;
}
function enableButton2() {
    var nextButton = document.getElementById('nextButton2');
    var input1 = document.getElementById('email');
    var input2 = document.getElementById('phones');
    if (input1.value.trim() !== "" && input2.value.trim() !== "")
        nextButton.disabled = false;
    else
        nextButton.disabled = true;
}
function enableButton3() {
    var nextButton = document.getElementById('nextButton3');
    var input1 = document.getElementById('birthday');
    var input2 = document.getElementById('address');
    if (input1.value.trim() !== "" && input2.value.trim() !== "")
        nextButton.disabled = false;
    else
        nextButton.disabled = true;
}
function enableButton4() {
    var nextButton = document.getElementById('submitButton');
    var input1 = document.getElementById('username');
    var input2 = document.getElementById('password');
    if (input1.value.trim() !== "" && input2.value.trim() !== "")
        nextButton.disabled = false;
    else
        nextButton.disabled = true;
}
nextBtnFirst.addEventListener("click", function (event) {


    const span = document.getElementById("span1");
    if (span.innerText.trim() === "") {

        slidePage.style.marginLeft = "-25%";
        bullet[current - 1].classList.add("active");
        progressCheck[current - 1].classList.add("active");
        progressText[current - 1].classList.add("active");
        current += 1;
    }
});

nextBtnSec.addEventListener("click", function (event) {

    const span = document.getElementById("span2");
    const span1 = document.getElementById("span3");
    if (span.innerText.trim() === "" && span1.innerText.trim() === "") {

        slidePage.style.marginLeft = "-50%";
        bullet[current - 1].classList.add("active");
        progressCheck[current - 1].classList.add("active");
        progressText[current - 1].classList.add("active");
        current += 1;
    }
});
nextBtnThird.addEventListener("click", function (event) {

    const span = document.getElementById("span4");
    const span1 = document.getElementById("span5");
    if (span.innerText.trim() === "" && span1.innerText.trim() === "") {

        slidePage.style.marginLeft = "-75%";
        bullet[current - 1].classList.add("active");
        progressCheck[current - 1].classList.add("active");
        progressText[current - 1].classList.add("active");
        current += 1;
    }
});
submitBtn.addEventListener("click", function (event) {
    const span = document.getElementById("span6");
    const span1 = document.getElementById("span7");
    // Check if both fields are filled
    if (span.innerText.trim() === "" && span1.innerText.trim() === "") {

        bullet[current - 1].classList.add("active");
        progressCheck[current - 1].classList.add("active");
        progressText[current - 1].classList.add("active");
        current -= 1;
    }


});

prevBtnSec.addEventListener("click", function (event) {
    event.preventDefault();
    slidePage.style.marginLeft = "0%";
    bullet[current - 2].classList.remove("active");
    progressCheck[current - 2].classList.remove("active");
    progressText[current - 2].classList.remove("active");
    current -= 1;
});
prevBtnThird.addEventListener("click", function (event) {
    event.preventDefault();
    slidePage.style.marginLeft = "-25%";
    bullet[current - 2].classList.remove("active");
    progressCheck[current - 2].classList.remove("active");
    progressText[current - 2].classList.remove("active");
    current -= 1;
});
prevBtnFourth.addEventListener("click", function (event) {
    event.preventDefault();
    slidePage.style.marginLeft = "-50%";
    bullet[current - 2].classList.remove("active");
    progressCheck[current - 2].classList.remove("active");
    progressText[current - 2].classList.remove("active");
    current -= 1;
});
