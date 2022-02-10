function appendScript(id) {
    const script = document.createElement('script');
    script.async = true;
    script.src = 'https://www.googletagmanager.com/gtag/js?id=' + id;
    document.body.appendChild(script);
}

function init(id) {
    appendScript(id);

    window.dataLayer = window.dataLayer || [];
    window.gtag =
        window.gtag || 
        function () {
            window.dataLayer.push(arguments);
        };

    window.gtag('js', new Date());
    window.gtag('config', id);
}

function collect() {
    if (!window.gtag) {
        init($docsify.gtag);
    }

    window.gtag('event', 'page_view', {
        page_title: document.title,
        page_location: location.href,
        page_path: location.pathname,
    });
}

function useGtag(hook) {
    if (!$docsify.gtag) {
        console.error('GTAG is required.');
        return;
    }

    hook.beforeEach(collect);
};