const staticNanasPatries = "nanas-pastries";
const assets = [
    "/",
    "/index.html",
    "/css/style.css",
    "/js/app.js",
    "/favicon.ico",  
    "https://i.pinimg.com/736x/ab/c1/0b/abc10b8f1a21a828f918bad1b235c57f.jpg",
];

self.addEventListener("install", installEvent => {
    installEvent.waitUntil(
        caches.open(staticNanasPatries).then(cache => {
            return cache.addAll(assets);
        })
    );
});

self.addEventListener("fetch", fetchEvent => {
    const url = new URL(fetchEvent.request.url);

    // Si la solicitud es a la API, no la cachees
    if (url.origin === location.origin && url.pathname.startsWith('/api/')) {
        fetchEvent.respondWith(
            fetch(fetchEvent.request).catch(() => {
                return caches.match(fetchEvent.request);
            })
        );
    } else {
        fetchEvent.respondWith(
            caches.match(fetchEvent.request).then(res => {
                return res || fetch(fetchEvent.request);
            })
        );
    }
});

