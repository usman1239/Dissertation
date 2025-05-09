window.scrollToElement = (element) => {
    if (element) {
        element.scrollIntoView({ behavior: "smooth", block: "center" });
    }
};