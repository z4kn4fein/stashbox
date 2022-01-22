function renderHeader(hook) {
    const defaultGithubData = {
        mainEtag: "",
        releaseEtag: "",
        tagName: "",
        stars: "",
        forks: ""
    };

    const navBar = `
    <nav class="app-nav no-badge">
      <ul>
        <li>
          <div><div class="nav-root"><span class="material-icons">extension</span><span>Extensions</span></div></div>
          <ul>
            <li>
              <p><strong>.NET Core</strong></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection" target="_blank" rel="noopener" title="">ASP.NET Core</a></p>
            </li>
            <li>
              <p><strong>ASP.NET</strong></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-web-webapi" target="_blank" rel="noopener" title="">Web Api</a></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-web-mvc" target="_blank" rel="noopener" title="">MVC</a></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-signalr" target="_blank" rel="noopener" title="">SignalR</a></p>
            </li>
            <li>
              <p><strong>OWIN</strong></p>
            </li>
              <li><p><a href="https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-owin" target="_blank" rel="noopener" title="">OWIN</a></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-webapi-owin" target="_blank" rel="noopener" title="">Web Api</a></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-signalr-owin" target="_blank" rel="noopener" title="">SignalR</a></p>
            </li>
            <li>
              <p><strong>Other</strong></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-hangfire" target="_blank" rel="noopener" title="">Hangfire</a></p>
            </li>
            <li>
              <p><a href="https://github.com/z4kn4fein/stashbox-mocking" target="_blank" rel="noopener" title="">Mocking</a></p>
            </li>
          </ul>
        </li>
        <li>
          <div><div class="nav-root"><span class="material-icons">speed</span><span>Benchmarks</span></div></div>
          <ul>
            <li>
              <a href="https://danielpalme.github.io/IocPerformance" target="_blank" rel="noopener" title="">Performance</a>
            </li>
            <li>
              <a href="http://featuretests.apphb.com/DependencyInjection.html" target="_blank" rel="noopener" title="">Feature</a>
            </li>
          </ul>
        </li>
        <li>
          <div><div class="nav-root"><span class="material-icons">format_list_bulleted</span><span>Changelog</span></div></div>
          <ul>
            <li>
              <a href="#/changelog" title="">Release notes</a>
            </li>
            <li>
              <a href="https://github.com/z4kn4fein/stashbox/releases" target="_blank" rel="noopener" title="">GitHub releases</a>
            </li>
          </ul>
        </li>
      </ul>
    </nav>`

    const getGitHubArea = (data) => {
        return `<a href="https://github.com/z4kn4fein/stashbox" target="_blank" rel="noopener">
                <div class="github-section">
                  <div class="repo-logo"><i class="fa fa-github"></i></div>
                  <div class="repo-details">
                    <div class="repo-name">z4kn4fein/stashbox</div>
                    <div class="repo-stats">
                      <i class="fa fa-tag" aria-hidden="true"></i><span class="repo-stat">${data.tagName}</span>
                      <i class="fa fa-star" aria-hidden="true"></i><span class="repo-stat">${data.stars}</span>
                      <i class="fa fa-code-fork" aria-hidden="true"></i><span class="repo-stat">${data.forks}</span>
                    </div>
                  </div>
                </div>
              </a>`
    }

    const setGitHubArea = () => {
        if (window.githubData) {
            const githubArea = document.getElementById('github-area');
            githubArea.innerHTML = getGitHubArea(window.githubData);
        }
    }

    hook.init(async () => {
        try {
            let githubData = JSON.parse(localStorage.getItem('gh-data')) ?? defaultGithubData;

            let releasePromise = fetch('https://api.github.com/repos/z4kn4fein/stashbox/releases/latest', {
                headers: {
                    'If-None-Match': githubData.releaseEtag
                }
            })
                .then(async response => {
                    switch (response.status) {
                        case 200:
                            const release = await response.json();
                            return {
                                tagName: release['tag_name'],
                                releaseEtag: response.headers.get('ETag')
                            };
                        case 304:
                            return {
                                tagName: githubData.tagName,
                                releaseEtag: githubData.releaseEtag
                            };
                        default:
                            throw new Error();
                    }
                });

            let mainPromise = fetch('https://api.github.com/repos/z4kn4fein/stashbox', {
                headers: {
                    'If-None-Match': githubData.mainEtag
                }
            })
                .then(async response => {
                    switch (response.status) {
                        case 200:
                            const main = await response.json();
                            return {
                                mainEtag: response.headers.get('ETag'),
                                stars: main['stargazers_count'],
                                forks: main['forks_count']
                            };
                        case 304:
                            return {
                                mainEtag: githubData.mainEtag,
                                stars: githubData.stars,
                                forks: githubData.forks
                            };
                        default:
                            throw new Error();
                    }
                });

            let [release, main] = await Promise.all([releasePromise, mainPromise]);


            if (release.tagName && release.releaseEtag && main.mainEtag && main.stars && main.forks) {
                githubData.tagName = release.tagName;
                githubData.releaseEtag = release.releaseEtag;
                githubData.mainEtag = main.mainEtag;
                githubData.stars = main.stars;
                githubData.forks = main.forks;

                localStorage.setItem('gh-data', JSON.stringify(githubData));

                window.githubData = githubData;
            } else {
                throw new Error();
            }
        }
        catch (e) {
            console.log(e);
        }
    });

    hook.afterEach((content, next) => {
        const header = `
          <div class="header">
            <div>${navBar}</div>
            <div class="github-area" id="github-area"></div>
          </div>`;

        next(`<div>${header}<div class="markdown-content">${content}</div></div>`);
    });

    hook.doneEach(() => {
        setGitHubArea();
    });

    hook.ready(() => {
        setGitHubArea();
    });
}