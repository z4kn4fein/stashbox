function editOnGithubPlugin(hook, vm) {
    hook.beforeEach((content, next) => {
        if (/CHANGELOG\.md/.test(vm.route.file)) {
            next(content);
            return;
        } else if (/githubusercontent\.com/.test(vm.route.file)) {
            url = vm.route.file
                .replace('raw.githubusercontent.com', 'github.com')
                .replace(/\/master/, '/blob/master')
        } else {
            url = 'https://github.com/z4kn4fein/stashbox/edit/master/docs/' + vm.route.file
        }

        let ghEditData = JSON.parse(localStorage.getItem('gh-edit')) ?? {};
        let dataForPage = ghEditData[vm.route.file];

        fetch('https://api.github.com/repos/z4kn4fein/stashbox/commits?per_page=1&path=docs/' + vm.route.file, {
            headers: {
                'If-None-Match': dataForPage?.etag
            }
        })
            .then(async response => {
                switch (response.status) {
                    case 200:
                        const commits = await response.json();
                        const modified = commits[0]['commit']['committer']['date'].slice(0, 10);
                        const modifiedBy = commits[0]['commit']['committer']['name'];
                        const modifiedDate = new Date(modified);

                        const newData = {
                            etag: response.headers.get('ETag'),
                            modifiedBy: modifiedBy,
                            modifiedDate: modifiedDate
                        };

                        ghEditData[vm.route.file] = newData;
                        localStorage.setItem('gh-edit', JSON.stringify(ghEditData));
                        return newData;
                    case 304:
                        return dataForPage;
                    default:
                        throw new Error();
                }
            })
            .then(ghData => {
                if (!(ghData.modifiedDate instanceof Date)) {
                    ghData.modifiedDate = new Date(ghData.modifiedDate);
                }
                const editSection = '[<img src="assets/images/github.svg" class="gh-prefix" alt="GitHub" width=18>Edit this page](' + url + ') - <span class="last-modified">*(last updated on <strong>' + ghData.modifiedDate.toLocaleDateString() + '</strong> by <strong>' + ghData.modifiedBy + '</strong>)*</span>\n\n';
                next(editSection + content);
            })
            .catch(_ => {
                const editSection = '[<img src="assets/images/github.svg" class="gh-prefix" alt="GitHub" width=18>Edit this page](' + url + ')\n\n';
                next(editSection + content);
            });
    });
}