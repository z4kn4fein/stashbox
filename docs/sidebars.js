/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
  docs: [
    {
      type: 'category',
      label: 'Get started',
      collapsed: false,
      items: [
        'getting-started/overview',
        'getting-started/introduction',
        'getting-started/glossary',
      ],
    },
    {
      type: 'category',
      label: 'Guides',
      collapsed: false,
      items: [
        'guides/basics',
        'guides/advanced-registration',
        'guides/service-resolution',
        'guides/lifetimes',
        'guides/scopes',
      ],
    },
    {
      type: 'category',
      label: 'Configuration',
      collapsed: false,
      items: [
        'configuration/registration-configuration',
        'configuration/container-configuration',
      ],
    },
    {
      type: 'category',
      label: 'Advanced',
      collapsed: false,
      items: [
        'advanced/generics',
        'advanced/decorators',
        'advanced/wrappers-resolvers',
        'advanced/child-containers',
        'advanced/special-resolution-cases',
      ],
    },
    {
      type: 'category',
      label: 'Diagnostics',
      collapsed: false,
      items: [
        'diagnostics/validation',
        'diagnostics/utilities',
      ],
    },
  ]
};

module.exports = sidebars;
