// @ts-check
// @ts-ignore
const prismLightTheme = require('prism-react-renderer/themes/github');

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'Stashbox',
  tagline: 'A lightweight, fast, and portable .NET DI framework.',
  url: 'https://z4kn4fein.github.io',
  baseUrl: '/stashbox',
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/favicon.ico',

  organizationName: 'z4kn4fein', 
  projectName: 'stashbox',
  trailingSlash: false,

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve('./sidebars.js'),
          showLastUpdateAuthor: true,
          showLastUpdateTime: true,
          sidebarCollapsible: true,
          editUrl:
            'https://github.com/z4kn4fein/stashbox/edit/master/docs',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.scss'),
        },
        gtag: {
          trackingID: 'G-HLNT9WV1HH'
        }
      }),
    ],
  ],

  plugins: [
    'docusaurus-plugin-sass',
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      navbar: {
        hideOnScroll: true,
        title: 'Stashbox',
        logo: {
          alt: 'Stashbox logo',
          src: 'img/icon.png',
        },
        items: [
          {
            type: 'dropdown',
            position: 'left',
            label: 'Extensions',
            items: [
              {
                type: 'html',
                className: 'navbar-title',
                value: '<b>.NET Core</b>',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#aspnet-core',
                label: 'ASP.NET Core',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#multitenant',
                label: 'ASP.NET Core Multitenant',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#testing',
                label: 'ASP.NET Core Testing',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#net-generic-host',
                label: '.NET Generic Host',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#servicecollection-based-applications',
                label: 'ServiceCollection Extensions',
              },
              {
                type: 'html',
                value: '<hr class="dropdown-separator">',
              },
              {
                type: 'html',
                className: 'navbar-title',
                value: '<b>ASP.NET</b>',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-web-webapi',
                label: 'Web API',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-web-mvc',
                label: 'MVC',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-signalr',
                label: 'SignalR',
              },
              {
                type: 'html',
                value: '<hr class="dropdown-separator">',
              },
              {
                type: 'html',
                className: 'navbar-title',
                value: '<b>OWIN</b>',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-owin',
                label: 'OWIN',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-webapi-owin',
                label: 'Web API',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-signalr-owin',
                label: 'SignalR',
              },
              {
                type: 'html',
                value: '<hr class="dropdown-separator">',
              },
              {
                type: 'html',
                className: 'navbar-title',
                value: '<b>Other</b>',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-hangfire',
                label: 'Hangfire',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox-mocking',
                label: 'Mocking',
              },
            ]
          },
          {
            type: 'dropdown',
            position: 'left',
            label: 'Benchmarks',
            items: [
              {
                href: 'https://danielpalme.github.io/IocPerformance',
                label: 'Performance',
              },
            ]
          },
          {
            type: 'dropdown',
            position: 'left',
            label: 'Changelog',
            items: [
              {
                href: 'https://github.com/z4kn4fein/stashbox/blob/master/CHANGELOG.md',
                label: 'Release notes',
              },
              {
                href: 'https://github.com/z4kn4fein/stashbox/releases',
                label: 'GitHub releases',
              },
            ]
          },
          {
            type: 'custom-icon',
            icon: 'nuget',
            position: 'right',
            className: 'nav-icon',
            href: 'https://www.nuget.org/packages/Stashbox'
          },
          {
            type: 'custom-icon',
            icon: 'github',
            position: 'right',
            className: 'nav-icon',
            href: 'https://github.com/z4kn4fein/stashbox'
          },
          {
            type: 'custom-separator',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'GUIDES',
            items: [
              {label: 'Basic usage', to: 'docs/guides/basics'},
              {label: 'Advanced registration', to: 'docs/guides/advanced-registration'},
              {label: 'Service resolution', to: 'docs/guides/service-resolution'},
              {label: 'Lifetimes', to: 'docs/guides/lifetimes'},
              {label: 'Scopes', to: 'docs/guides/scopes'},
            ],
          },
          {
            title: 'ADVANCED',
            items: [
              {label: 'Generics', to: 'docs/advanced/generics'},
              {label: 'Decorators', to: 'docs/advanced/decorators'},
              {label: 'Wrappers & resolvers', to: 'docs/advanced/wrappers-resolvers'},
              {label: 'Child containers', to: 'docs/advanced/child-containers'},
              {label: 'Special resolution cases', to: 'docs/advanced/special-resolution-cases'},
            ],
          }
        ],
        copyright: `Copyright © ${new Date().getFullYear()} Peter Csajtai. Built with Docusaurus.`,
      },
      docs: {
        sidebar: {
          hideable: true,
        },
      },
      prism: {
        theme: prismLightTheme,
        additionalLanguages: [
          'csharp',
          'powershell',
          'bash',
        ],
      },
      colorMode: {
        defaultMode: 'light',
        disableSwitch: false,
        respectPrefersColorScheme: true,
      },
      algolia: {
        appId: 'CYYLE77D6F',
        apiKey: '70fdb3ec7ec00e65922f35e5a5e35562',
        indexName: 'stashbox'
      },
    }),
};

async function createConfig() {
  const darkTheme = (await import('./src/utils/prismDark.mjs')).default;
  // @ts-expect-error: it exists
  config.themeConfig.prism.darkTheme = darkTheme;
  return config;
}

module.exports = createConfig;
