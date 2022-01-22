function glossaryPlugin(hook) {
    hook.beforeEach((content, next) => {
        if (window.location.hash.match(/_glossary/g)) {
            next(content);
            return;
        }

        const addLinks = (content, next, terms) => {
            for (const term in terms) {
                const link = ` [$1](/_glossary?id=${terms[term]})`;
                const re = new RegExp(`\\ (${term}s?('s)?)`, 'ig');
                content = content.replace(re, link);
            }
            next(content);
        }

        if (!window.$docsify.terms) {
            fetch('_glossary.md').then(data => {
                data.text().then(text => {
                    window.$docsify.terms = {};
                    const lines = text.split('\n');
                    lines.forEach(line => {
                        if (line.match(/##/g)) {
                            const title = line.replace('##', '').trim();
                            const id = title.toLowerCase().replaceAll(' | ', '-').replaceAll(' ', '-');
                            title.split('|').forEach(term => {
                                window.$docsify.terms[term.trim()] = id;
                            });
                        }
                    });

                    addLinks(content, next, window.$docsify.terms);
                })
            })
        } else {
            addLinks(content, next, window.$docsify.terms);
        }
    });
}