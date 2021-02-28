function editOnGithubPlugin(hook, vm) {
    hook.beforeEach(function (content, next) {
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

        fetch('https://api.github.com/repos/z4kn4fein/stashbox/commits?per_page=1&path=docs/' + vm.route.file)
            .then((response) => {
                if (!response.ok) {
                    throw new Error();
                }
                return response.json();
            })
            .then((commits) => {
                let modified = commits[0]['commit']['committer']['date'].slice(0,10);
                let modifiedBy = commits[0]['commit']['committer']['name'];
                let modifiedDate = new Date(modified);
                let editSection = '[<img src="assets/images/github.svg" class="gh-prefix" alt="GitHub" width=18>Edit this page](' + url + ') - <span class="last-modified">*(last updated: <strong>'+ modifiedDate.toLocaleDateString() +'</strong> by <strong>'+ modifiedBy +'</strong>)*</span>\n\n';
                next(editSection + content);
            })
            .catch(_ => {
                let editSection = '[<img src="assets/images/github.svg" class="gh-prefix" alt="GitHub" width=18>Edit this page](' + url + ')\n\n';
                next(editSection + content);
            });
    })
}

function glossaryPlugin(hook, vm) {
    hook.beforeEach(function(content,next) {
        if(window.location.hash.match(/_glossary/g)){
          next(content);
          return;
        }

        let addLinks = function(content,next,terms){
          for (let term in terms){
            let link = ` [$1](/_glossary?id=${terms[term]})`;
            let re = new RegExp(`\\ (${term})`,'ig');
            content = content.replace(re,link);
          }
          next(content);
        }

        if(!window.$docsify.terms){
          fetch('_glossary.md').then(function(data){
            data.text().then(function(text){
              window.$docsify.terms = {};
              let lines = text.split('\n');
              lines.forEach(function(line){
                if(line.match(/##/g)){
                  let title = line.replace('##','').trim();
                  let id = title.toLowerCase().replaceAll(' | ','-').replaceAll(' ','-');
                  title.split('|').forEach(function(term) {
                    window.$docsify.terms[term.trim()] = id;
                  });                 
                }
              });

              addLinks(content,next,window.$docsify.terms);
            })
          })
        } else{
          addLinks(content,next,window.$docsify.terms);
        }
    })
}