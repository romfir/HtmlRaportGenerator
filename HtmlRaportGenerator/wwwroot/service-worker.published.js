// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js', './js/decode.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = await Promise.all(self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(async asset => {

            const response = await fetch(asset.url + '.br', { cache: 'no-cache' });
            if (!response.ok) {
                console.log(response);
                throw new Error(response.statusText);
            }
            //console.log(defaultUri);
            const originalResponseBuffer = await response.arrayBuffer();
            const originalResponseArray = new Int8Array(originalResponseBuffer);
            const decompressedResponseArray = BrotliDecode(originalResponseArray);
            const contentType = response.type ===
                'dotnetwasm'
                ? 'application/wasm'
                : 'application/octet-stream';
            return new Response(decompressedResponseArray,
                { headers: { 'content-type': contentType, integrity: asset.hash } });

            //new Request(asset.url, { integrity: asset.hash })

            //ypeError: Failed to execute 'Cache' on 'addAll': Request failed
        }));

    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate';

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }

    if (cachedResponse) {
        return cachedResponse;
    }

    try {
        //if (type !== 'dotnetjs') {

        const response = await fetch(defaultUri + '.br', { cache: 'no-cache' });
        if (!response.ok) {
            console.log(response);
            throw new Error(response.statusText);
        }
        console.log(defaultUri);
        const originalResponseBuffer = await response.arrayBuffer();
        const originalResponseArray = new Int8Array(originalResponseBuffer);
        const decompressedResponseArray = BrotliDecode(originalResponseArray);
        const contentType = response.type ===
            'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
        return new Response(decompressedResponseArray,
            { headers: { 'content-type': contentType } });
        // }
    } catch (e) {
        console.log(e);
    }


    return fetch(event.request);
}
