window.setInterval(() => {
    const elem = document.getElementById("autoScroll");
    elem.scrollTop = elem.scrollHeight;
}, 5000);