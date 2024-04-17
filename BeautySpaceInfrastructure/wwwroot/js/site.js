let carouselEl = document.getElementById('bootstrap-gallery-carousel');

const carousel = new bootstrap.Carousel(carouselEl, {
    interval: 3000,
    wrap: true,
});

carouselEl.addEventListener('slide.bs.carousel', (event) => {
    const container = document.querySelector('.carousel-inner');
    window.lightGallery(container, {
        thumbnail: true,
        pager: false,
        plugins: [lgThumbnail],
        hash: false,
        zoomFromOrigin: false,
        preload: 3,
        selector: '.lg-item',
    });
});