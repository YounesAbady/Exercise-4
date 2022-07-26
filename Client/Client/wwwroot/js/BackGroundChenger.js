function randomImage() {
    var images = [
        '../bg.jpg',
        '../Food.jpg',
        '../Food1.jpg',
        '../Food2.jpg',
        '../Food3.jpg',
        '../Food4.jpg'];
    var size = images.length;
    var x = Math.floor(size * Math.random());
    console.log(x);
    var element = document.getElementsByClassName('bg');
    console.log(element);
    element[0].style["background-image"] = "url(" + images[x] + ")";
}
document.addEventListener("DOMContentLoaded", randomImage);