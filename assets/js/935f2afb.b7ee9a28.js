"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[53],{1109:e=>{e.exports=JSON.parse('{"pluginId":"default","version":"current","label":"Next","banner":null,"badge":false,"noIndex":false,"className":"docs-version-current","isLast":true,"docsSidebars":{"docs":[{"type":"category","label":"Get started","items":[{"type":"link","label":"Overview","href":"/stashbox/docs/getting-started/overview","docId":"getting-started/overview"},{"type":"link","label":"Introduction","href":"/stashbox/docs/getting-started/introduction","docId":"getting-started/introduction"},{"type":"link","label":"Glossary","href":"/stashbox/docs/getting-started/glossary","docId":"getting-started/glossary"}],"collapsed":true,"collapsible":true},{"type":"category","label":"Guides","items":[{"type":"link","label":"Basic usage","href":"/stashbox/docs/guides/basics","docId":"guides/basics"},{"type":"link","label":"Advanced registration","href":"/stashbox/docs/guides/advanced-registration","docId":"guides/advanced-registration"},{"type":"link","label":"Service resolution","href":"/stashbox/docs/guides/service-resolution","docId":"guides/service-resolution"},{"type":"link","label":"Lifetimes","href":"/stashbox/docs/guides/lifetimes","docId":"guides/lifetimes"},{"type":"link","label":"Scopes","href":"/stashbox/docs/guides/scopes","docId":"guides/scopes"}],"collapsed":true,"collapsible":true},{"type":"category","label":"Configuration","items":[{"type":"link","label":"Registration configuration","href":"/stashbox/docs/configuration/registration-configuration","docId":"configuration/registration-configuration"},{"type":"link","label":"Container configuration","href":"/stashbox/docs/configuration/container-configuration","docId":"configuration/container-configuration"}],"collapsed":true,"collapsible":true},{"type":"category","label":"Advanced","items":[{"type":"link","label":"Generics","href":"/stashbox/docs/advanced/generics","docId":"advanced/generics"},{"type":"link","label":"Decorators","href":"/stashbox/docs/advanced/decorators","docId":"advanced/decorators"},{"type":"link","label":"Wrappers & resolvers","href":"/stashbox/docs/advanced/wrappers-resolvers","docId":"advanced/wrappers-resolvers"},{"type":"link","label":"Child containers","href":"/stashbox/docs/advanced/child-containers","docId":"advanced/child-containers"},{"type":"link","label":"Special resolution cases","href":"/stashbox/docs/advanced/special-resolution-cases","docId":"advanced/special-resolution-cases"}],"collapsed":true,"collapsible":true},{"type":"category","label":"Diagnostics","items":[{"type":"link","label":"Validation","href":"/stashbox/docs/diagnostics/validation","docId":"diagnostics/validation"},{"type":"link","label":"Utilities","href":"/stashbox/docs/diagnostics/utilities","docId":"diagnostics/utilities"}],"collapsed":true,"collapsible":true}]},"docs":{"advanced/child-containers":{"id":"advanced/child-containers","title":"Child containers","description":"With child containers, you can build up parent-child relationships between containers. It means you can have a different subset of services present in a child than the parent container. When a dependency is missing from the child container during a resolution request, the parent will be asked to resolve the missing service. If it\'s found there, the parent will return only the service\'s registration, and the resolution request will continue within the child. Also, child registrations with the same service type will override the parent\'s services.","sidebar":"docs"},"advanced/decorators":{"id":"advanced/decorators","title":"Decorators","description":"Stashbox supports decorator service registration to take advantage of the Decorator pattern. This pattern is used to extend the functionality of a class without changing its implementation. This is also what the Open\u2013closed principle stands for; services should be open for extension but closed for modification.","sidebar":"docs"},"advanced/generics":{"id":"advanced/generics","title":"Generics","description":"This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, generic decorators, conditions based on generic constraints, and variance.","sidebar":"docs"},"advanced/special-resolution-cases":{"id":"advanced/special-resolution-cases","title":"Special resolution cases","description":"Unknown type resolution","sidebar":"docs"},"advanced/wrappers-resolvers":{"id":"advanced/wrappers-resolvers","title":"Wrappers & resolvers","description":"Stashbox uses so-called Wrapper and Resolver implementations to handle those special resolution requests that none of the service registrations can fulfill. Functionalities like wrapper and unknown type resolution, cross-container requests, optional and default value injection are all built with resolvers.","sidebar":"docs"},"configuration/container-configuration":{"id":"configuration/container-configuration","title":"Container configuration","description":"The container\'s constructor has an Action parameter used to configure its behavior.","sidebar":"docs"},"configuration/registration-configuration":{"id":"configuration/registration-configuration","title":"Registration configuration","description":"Most of the registration methods have an Action parameter, enabling several customization options on the given registration.","sidebar":"docs"},"diagnostics/utilities":{"id":"diagnostics/utilities","title":"Utilities","description":"Is registered?","sidebar":"docs"},"diagnostics/validation":{"id":"diagnostics/validation","title":"Validation","description":"Stashbox has validation routines that help you detect and solve common misconfiguration issues. You can verify the container\'s actual state with its .Validate() method. This method walks through the whole resolution tree and collects all the issues into an AggregateException.","sidebar":"docs"},"getting-started/glossary":{"id":"getting-started/glossary","title":"Glossary","description":"The following terms and definitions are used in this documentation.","sidebar":"docs"},"getting-started/introduction":{"id":"getting-started/introduction","title":"Introduction","description":"Stashbox and its extensions are distributed via NuGet packages.","sidebar":"docs"},"getting-started/overview":{"id":"getting-started/overview","title":"Overview","description":"Appveyor Build Status","sidebar":"docs"},"guides/advanced-registration":{"id":"guides/advanced-registration","title":"Advanced registration","description":"This section is about Stashbox\'s further configuration options, including the registration configuration API, the registration of factory delegates, multiple implementations, batch registration, the concept of the Composition Root and many more.","sidebar":"docs"},"guides/basics":{"id":"guides/basics","title":"Basic usage","description":"This section is about the basics of Stashbox\'s API. It will give you a good starting point for more advanced topics described in the following sections.","sidebar":"docs"},"guides/lifetimes":{"id":"guides/lifetimes","title":"Lifetimes","description":"Lifetime management is the concept of controlling how long a service\'s instances will live (from instantiation to disposal) and how they will be reused between resolution requests.","sidebar":"docs"},"guides/scopes":{"id":"guides/scopes","title":"Scopes","description":"A scope is Stashbox\'s implementation of the unit-of-work pattern; it encapsulates a given unit used to resolve and store instances required for a given work. When a scoped service is resolved or injected, the scope ensures that it gets instantiated only once within the scope\'s lifetime. When the work is finished, the scope cleans up the resources by disposing every tracked disposable instance.","sidebar":"docs"},"guides/service-resolution":{"id":"guides/service-resolution","title":"Service resolution","description":"When you have all your components registered and configured adequately, you can resolve them from the container or a scope by requesting their service type.","sidebar":"docs"}}}')}}]);