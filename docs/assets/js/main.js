window.$docsify = {
  loadSidebar: true,

  maxLevel   : 2,
  subMaxLevel: 2,
  name: 'Stashbox',
  nameLink: '/stashbox',
  logo: 'assets/images/icon.png',

  auto2top: true,

  coverpage: true,
  homepage: 'getting-started/overview.md',

  pagination: {
    crossChapter: true,
    crossChapterText: true,
  },

  copyCode: {
    buttonText : '<img class="noselect" src="assets/images/copy.svg" width=18>',
  },

  search: {
    depth: 3,
    noData: 'No results! 😬',
    placeholder: 'Search...'
  },

  tabs: {
    theme: 'material'
  },

  alias: {
    '/.*/_sidebar.md': '/_sidebar.md',
    '/.*/_navbar.md': '/_navbar.md',
    '.*?/changelog': 'https://raw.githubusercontent.com/z4kn4fein/stashbox/master/CHANGELOG.md',
  },

  onlyCover: true,

  plugins: [
    renderHeader,
    editOnGithubPlugin,
    glossaryPlugin,  
  ]
}