//window.scrollToTop = () => {
//    window.scrollTo({ top: 0, behavior: 'smooth' });
//};

window.scrollToTopInstant = () => {
    window.scrollTo({ top: 0, behavior: 'auto' }); // Instant scroll
};

const scrollListener = () => {
    let contentElement = document.querySelector(".content");
    let scrollButton = document.getElementById("scrollToTopButton");

    let rect = contentElement.getBoundingClientRect();
    let isAtTop = rect.top >= -10; // Checks if the element is at the top

    if (scrollButton === null) return;

    if (isAtTop) {
            scrollButton.classList.remove("show"); // Smoothly fades out
    } else {
        scrollButton.classList.add("show"); // Smoothly fades in
    }
};

window.initializeScrollButton = function () {
    let mainScrollElement = document.querySelector("main");
    let contentElement = document.querySelector(".content");
    let scrollButton = document.getElementById("scrollToTopButton");

    //console.log("Content element:", contentElement);
    //console.log("Scroll button:", scrollButton);

    if (!contentElement || !scrollButton) {
        console.warn("Missing elements! Scroll button will not work.");
        return;
    }


    mainScrollElement.addEventListener("scroll", scrollListener);
};

window.removeMainScrollListener = function() {
    document.querySelector("main").removeEventListener("scroll", scrollListener);
};

window.scrollToTop = function () {
    let contentElement = document.querySelector(".content"); // Select the scrollable container
    if (contentElement) {
        contentElement.scrollIntoView({ block: 'start', behavior: 'smooth' });
    }
};