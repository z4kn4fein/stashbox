"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[80],{6503:(e,n,s)=>{s.r(n),s.d(n,{assets:()=>h,contentTitle:()=>c,default:()=>g,frontMatter:()=>l,metadata:()=>d,toc:()=>p});var t=s(5893),r=s(1151),i=s(8846),o=s(4866),a=s(5162);const l={},c="Advanced registration",d={id:"guides/advanced-registration",title:"Advanced registration",description:"This section is about Stashbox's further configuration options, including the registration configuration API, the registration of factory delegates, multiple implementations, batch registrations, the concept of the Composition Root, and many more.",source:"@site/docs/guides/advanced-registration.md",sourceDirName:"guides",slug:"/guides/advanced-registration",permalink:"/stashbox/docs/guides/advanced-registration",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/advanced-registration.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1700351127,formattedLastUpdatedAt:"Nov 18, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Basic usage",permalink:"/stashbox/docs/guides/basics"},next:{title:"Service resolution",permalink:"/stashbox/docs/guides/service-resolution"}},h={},p=[{value:"Factory registration",id:"factory-registration",level:2},{value:"Factories with parameter overrides",id:"factories-with-parameter-overrides",level:3},{value:"Consider this before using the resolver parameter inside a factory",id:"consider-this-before-using-the-resolver-parameter-inside-a-factory",level:3},{value:"Delegates with dependencies passed as parameters",id:"delegates-with-dependencies-passed-as-parameters",level:4},{value:"Accessing the currently resolving type in factories",id:"accessing-the-currently-resolving-type-in-factories",level:3},{value:"Multiple implementations",id:"multiple-implementations",level:2},{value:"Binding to multiple services",id:"binding-to-multiple-services",level:2},{value:"Batch registration",id:"batch-registration",level:2},{value:"Assembly registration",id:"assembly-registration",level:2},{value:"Composition root",id:"composition-root",level:2},{value:"Injection parameters",id:"injection-parameters",level:2},{value:"Initializer / finalizer",id:"initializer--finalizer",level:2}];function u(e){const n={a:"a",admonition:"admonition",code:"code",em:"em",h1:"h1",h2:"h2",h3:"h3",h4:"h4",mdxAdmonitionTitle:"mdxAdmonitionTitle",p:"p",pre:"pre",strong:"strong",...(0,r.a)(),...e.components};return(0,t.jsxs)(t.Fragment,{children:[(0,t.jsx)(n.h1,{id:"advanced-registration",children:"Advanced registration"}),"\n",(0,t.jsxs)(n.p,{children:["This section is about Stashbox's further configuration options, including the registration configuration API, the registration of factory delegates, multiple implementations, batch registrations, the concept of the ",(0,t.jsx)(n.a,{href:"https://blog.ploeh.dk/2011/07/28/CompositionRoot/",children:"Composition Root"}),", and many more."]}),"\n",(0,t.jsx)(n.admonition,{type:"info",children:(0,t.jsxs)(n.p,{children:["This section won't cover all the available options of the registrations API, but you can find them ",(0,t.jsx)(n.a,{href:"/docs/configuration/registration-configuration",children:"here"}),"."]})}),"\n",(0,t.jsx)(n.h2,{id:"factory-registration",children:"Factory registration"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"You can bind a factory delegate to a registration that the container will invoke directly to instantiate your service."}),(0,t.jsxs)(n.p,{children:["You can use parameter-less and custom parameterized delegates as a factory. ",(0,t.jsx)(n.a,{href:"/docs/configuration/registration-configuration#factory",children:"Here"})," is the list of all available options."]}),(0,t.jsxs)(n.p,{children:["You can also get the current ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#dependency-resolver",children:"dependency resolver"})," as a delegate parameter to resolve any additional dependencies required for the service construction."]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.Z,{children:[(0,t.jsx)(a.Z,{value:"Parameter-less",label:"Parameter-less",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<ILogger, ConsoleLogger>(options => options\n    .WithFactory(() => new ConsoleLogger());\n\n// the container uses the factory for instantiation.\nIJob job = container.Resolve<ILogger>();\n"})})}),(0,t.jsx)(a.Z,{value:"Parameterized",label:"Parameterized",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithFactory<ILogger>(logger => new DbBackup(logger));\n\n// the container uses the factory for instantiation.\nIJob job = container.Resolve<IJob>();\n"})})}),(0,t.jsx)(a.Z,{value:"Resolver parameter",label:"Resolver parameter",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithFactory(resolver => new DbBackup(resolver.Resolve<ILogger>()));\n    \n// the container uses the factory for instantiation.\nIJob job = container.Resolve<IJob>();\n"})})})]})})]}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(n.p,{children:"Delegate factories are useful when your service's instantiation is not straight-forward for the container, like when it depends on something that is not available at resolution time. E.g., a connection string."})}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>(options => options\n    .WithFactory<ILogger>(logger => \n        new DbBackup(Configuration["DbConnectionString"], logger));\n'})})})]}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.h3,{id:"factories-with-parameter-overrides",children:"Factories with parameter overrides"}),(0,t.jsxs)(n.p,{children:["Stashbox can implicitly ",(0,t.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#delegate",children:"wrap"})," your service in a ",(0,t.jsx)(n.code,{children:"Delegate"})," and lets you pass parameters that can override your service's dependencies. Moreover, you can register your own custom delegate that the container will resolve when you request your service wrapped in a ",(0,t.jsx)(n.code,{children:"Delegate"}),"."]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.Z,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(a.Z,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.RegisterFunc<string, IJob>((connectionString, resolver) => \n    new DbBackup(connectionString, resolver.Resolve<ILogger>()));\n\nFunc<string, IJob> backupFactory = container.Resolve<Func<string, IJob>>();\nIJob dbBackup = backupFactory(Configuration["ConnectionString"]);\n'})})}),(0,t.jsx)(a.Z,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.RegisterFunc<string, IJob>((connectionString, resolver) => \n    new DbBackup(connectionString, resolver.Resolve<ILogger>()));\n\nDelegate backupFactory = container.ResolveFactory(typeof(IJob), \n    parameterTypes: new[] { typeof(string) });\nIJob dbBackup = backupFactory.DynamicInvoke(Configuration["ConnectionString"]);\n'})})})]})})]}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsx)("div",{children:(0,t.jsxs)(n.p,{children:["If a service has multiple constructors, the container visits those first, that has matching parameters passed to the factory, with respecting the additional ",(0,t.jsx)(n.a,{href:"/docs/configuration/registration-configuration#constructor-selection",children:"constructor selection rules"}),"."]})}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"class Service\n{\n    public Service(int number) { }\n    public Service(string text) { }\n}\n\ncontainer.Register<Service>();\n\n// create the factory with an int input parameter.\nvar func = constainer.Resolve<Func<int, Service>>();\n\n// the constructor with the int param \n// is used for instantiation.\nvar service = func(2);\n"})})})]}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.h3,{id:"consider-this-before-using-the-resolver-parameter-inside-a-factory",children:"Consider this before using the resolver parameter inside a factory"}),(0,t.jsxs)(n.p,{children:["Delegate factories are a black-box for the container. It doesn't have control over what's happening inside a delegate, which means when you resolve additional dependencies with the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#dependency-resolver",children:"dependency resolver"})," parameter, they could easily bypass the ",(0,t.jsx)(n.a,{href:"/docs/diagnostics/validation#lifetime-validation",children:"lifetime"})," and ",(0,t.jsx)(n.a,{href:"/docs/diagnostics/validation#circular-dependency",children:"circular dependency"})," validations. Fortunately, you have the option to keep them validated anyway with parameterized factory delegates."]}),(0,t.jsx)(n.h4,{id:"delegates-with-dependencies-passed-as-parameters",children:"Delegates with dependencies passed as parameters"}),(0,t.jsxs)(n.p,{children:["Rather than using the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#dependency-resolver",children:"dependency resolver"})," parameter inside the factory, let the container inject the dependencies into the delegate as parameters. This way, the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree's"})," integrity remains stable because no service resolution happens inside the black-box, and each parameter is validated."]})]}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"interface IEventProcessor { }\n\nclass EventProcessor : IEventProcessor\n{\n    public EventProcessor(ILogger logger, IEventValidator validator)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>();\ncontainer.Register<IEventValidator, EventValidator>();\n\ncontainer.Register<IEventProcessor, EventProcessor>(options => options\n    // Ilogger and IEventValidator instances are injected\n    // by the container at resolution time, so they will be\n    // validated against circular and captive dependencies.\n    .WithFactory<ILogger, IEventValidator>((logger, validator) => \n        new EventProcessor(logger, validator));\n\n// the container resolves ILogger and IEventValidator first, then\n// it passes them to the factory as delegate parameters.\nIEventProcessor processor = container.Resolve<IEventProcessor>();\n"})})})]}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.h3,{id:"accessing-the-currently-resolving-type-in-factories",children:"Accessing the currently resolving type in factories"}),(0,t.jsxs)(n.p,{children:["To access the currently resolving type in factory delegates, you can set the ",(0,t.jsx)(n.code,{children:"TypeInformation"})," type as an input parameter of the factory.\nThe ",(0,t.jsx)(n.code,{children:"TypeInformation"})," holds every reflected context information about the currently resolving type."]}),(0,t.jsx)(n.p,{children:"This can be useful when the resolution is, e.g., in an open generic context, and we want to know which closed generic variant is requested."})]}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"interface IService<T> { }\n\nclass Service<T> : IService<T> { }\n\ncontainer.Register(typeof(IService<>), typeof(Service<>), options => \n    options.WithFactory<TypeInformation>(typeInfo => \n    {\n        // typeInfo.Type here holds the actual type like\n        // IService<int> based on the resolution request below.\n    }));\n    \ncontainer.Resolve<IService<int>>();\n"})})})]}),"\n",(0,t.jsx)(n.h2,{id:"multiple-implementations",children:"Multiple implementations"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["As we previously saw in the ",(0,t.jsx)(n.a,{href:"/docs/guides/basics#named-registration",children:"Named registration"})," topic, Stashbox allows you to have multiple implementations bound to a particular ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),". You can use names to distinguish them, but you can also access them by requesting a typed collection using the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),"."]}),(0,t.jsxs)(n.admonition,{type:"note",children:[(0,t.jsx)(n.mdxAdmonitionTitle,{}),(0,t.jsx)(n.p,{children:"The returned collection is in the same order as the services were registered.\nAlso, to request a collection, you can use any interface implemented by an array."})]})]}),(0,t.jsxs)("div",{children:[(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>();\ncontainer.Register<IJob, StorageCleanup>();\ncontainer.Register<IJob, ImageProcess>();\n"})}),(0,t.jsxs)(o.Z,{children:[(0,t.jsx)(a.Z,{value:"ResolveAll",label:"ResolveAll",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// jobs contain all three services in registration order.\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n"})})}),(0,t.jsx)(a.Z,{value:"Array",label:"Array",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// jobs contain all three services in registration order.\nIJob[] jobs = container.Resolve<IJob[]>();\n"})})}),(0,t.jsx)(a.Z,{value:"IEnumerable",label:"IEnumerable",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// jobs contain all three services in registration order.\nIEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>();\n"})})}),(0,t.jsx)(a.Z,{value:"IList",label:"IList",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// jobs contain all three services in registration order.\nIList<IJob> jobs = container.Resolve<IList<IJob>>();\n"})})}),(0,t.jsx)(a.Z,{value:"ICollection",label:"ICollection",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// jobs contain all three services in registration order.\nICollection<IJob> jobs = container.Resolve<ICollection<IJob>>();\n"})})})]})]})]}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["When you have multiple implementations registered to a service, a request to the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," without a name will return the ",(0,t.jsx)(n.strong,{children:"last registered implementation"}),"."]}),(0,t.jsx)(n.admonition,{type:"info",children:(0,t.jsxs)(n.p,{children:["Not only names can be used to distinguish registrations, ",(0,t.jsx)(n.a,{href:"/docs/guides/service-resolution#conditional-resolution",children:"conditions"}),", ",(0,t.jsx)(n.a,{href:"/docs/guides/scopes#named-scopes",children:"named scopes"}),", and ",(0,t.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#metadata--tuple",children:"metadata"})," can also influence the results."]})})]}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>();\ncontainer.Register<IJob, StorageCleanup>();\ncontainer.Register<IJob, ImageProcess>();\n\n// job will be the ImageProcess.\nIJob job = container.Resolve<IJob>();\n"})})})]}),"\n",(0,t.jsx)(n.h2,{id:"binding-to-multiple-services",children:"Binding to multiple services"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"When you have a service that implements multiple interfaces, you have the option to bind its registration to all or some of those additional interfaces or base types."}),(0,t.jsx)(n.p,{children:"Suppose we have the following class declaration:"}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob, IScheduledJob\n{ \n    public DbBackup() { }\n}\n"})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.Z,{children:[(0,t.jsx)(a.Z,{value:"To another type",label:"To another type",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .AsServiceAlso<IScheduledJob>());\n\nIJob job = container.Resolve<IJob>(); // DbBackup\nIScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup\nDbBackup job = container.Resolve<DbBackup>(); // error, not found\n"})})}),(0,t.jsx)(a.Z,{value:"To all implemented types",label:"To all implemented types",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<DbBackup>(options => options\n    .AsImplementedTypes());\n\nIJob job = container.Resolve<IJob>(); // DbBackup\nIScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup\nDbBackup job = container.Resolve<DbBackup>(); // DbBackup\n"})})})]})})]}),"\n",(0,t.jsx)(n.h2,{id:"batch-registration",children:"Batch registration"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"You have the option to register multiple services in a single registration operation."}),(0,t.jsxs)(n.p,{children:[(0,t.jsx)(n.strong,{children:"Filters (optional):"}),"\nFirst, the container will use the ",(0,t.jsx)(n.em,{children:"implementation filter"})," action to select only those types from the collection we want to register. When we have those, the container will execute the ",(0,t.jsx)(n.em,{children:"service filter"})," on their implemented interfaces and base classes to select which ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," they should be mapped to."]}),(0,t.jsx)(n.admonition,{type:"note",children:(0,t.jsxs)(n.p,{children:["Framework types like ",(0,t.jsx)(n.code,{children:"IDisposable"})," are excluded from being considered as a ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," by default."]})}),(0,t.jsxs)(n.admonition,{type:"tip",children:[(0,t.jsx)(n.mdxAdmonitionTitle,{}),(0,t.jsx)(n.p,{children:"You can use the registration configuration API to configure individual registrations."})]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.Z,{children:[(0,t.jsxs)(a.Z,{value:"Default",label:"Default",children:[(0,t.jsxs)(n.p,{children:["This example will register three types to all their implemented interfaces, extended base classes, and to themselves (",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#self-registration",children:"self registration"}),") without any filter:"]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterTypes(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup) \n    });\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nIJob job = container.Resolve<IJob>(); // StorageCleanup\nDbBackup backup = container.Resolve<DbBackup>(); // DbBackup\n"})})]}),(0,t.jsxs)(a.Z,{value:"Filters",label:"Filters",children:[(0,t.jsxs)(n.p,{children:["In this example, we assume that ",(0,t.jsx)(n.code,{children:"DbBackup"})," and ",(0,t.jsx)(n.code,{children:"StorageCleanup"})," are implementing ",(0,t.jsx)(n.code,{children:"IDisposable"})," besides ",(0,t.jsx)(n.code,{children:"IJob"})," and also extending a ",(0,t.jsx)(n.code,{children:"JobBase"})," abstract class."]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterTypes(new[] \n    { typeof(DbBackup), typeof(ConsoleLogger), typeof(StorageCleanup) },\n    // implementation filter, only those implementations that implements IDisposable\n    impl => typeof(IDisposable).IsAssignableFrom(impl),\n    // service filter, register them to base classes only\n    (impl, service) => service.IsAbstract && !service.IsInterface);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 0 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // error, not found\nDbBackup backup = container.Resolve<DbBackup>(); // DbBackup\n"})})]}),(0,t.jsxs)(a.Z,{value:"Without self",label:"Without self",children:[(0,t.jsxs)(n.p,{children:["This example ignores the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#self-registration",children:"self registrations"})," completely:"]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterTypes(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup)\n    },\n    registerSelf: false);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\nConsoleLogger logger = container.Resolve<ConsoleLogger>(); // error, not found\n"})})]}),(0,t.jsxs)(a.Z,{value:"Registration options",label:"Registration options",children:[(0,t.jsxs)(n.p,{children:["This example will configure all registrations mapped to ",(0,t.jsx)(n.code,{children:"ILogger"})," as ",(0,t.jsx)(n.code,{children:"Singleton"}),":"]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterTypes(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup)\n    },\n    configurator: options => \n    {\n        if (options.HasServiceType<ILogger>())\n            options.WithSingletonLifetime();\n    });\n\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nILogger newLogger = container.Resolve<ILogger>(); // the same ConsoleLogger\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\n"})})]})]})})]}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["Another type of service filter is the ",(0,t.jsx)(n.code,{children:".RegisterTypesAs<T>()"})," method, which registers only those types that implements the ",(0,t.jsx)(n.code,{children:"T"})," ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),"."]}),(0,t.jsxs)(n.admonition,{type:"note",children:[(0,t.jsx)(n.mdxAdmonitionTitle,{}),(0,t.jsxs)(n.p,{children:["This method also accepts an implementation filter and a registration configurator action like ",(0,t.jsx)(n.code,{children:".RegisterTypes()"}),"."]})]}),(0,t.jsx)(n.admonition,{type:"caution",children:(0,t.jsxs)(n.p,{children:[(0,t.jsx)(n.code,{children:".RegisterTypesAs<T>()"})," doesn't create ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#self-registration",children:"self registrations"})," as it only maps the implementations to the given ",(0,t.jsx)(n.code,{children:"T"})," ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),"."]})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.Z,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(a.Z,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterTypesAs<IJob>(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup) \n    });\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // error, not found\nIJob job = container.Resolve<IJob>(); // StorageCleanup\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"})})}),(0,t.jsx)(a.Z,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterTypesAs(typeof(IJob), new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup) \n    });\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // error, not found\nIJob job = container.Resolve<IJob>(); // StorageCleanup\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"})})})]})})]}),"\n",(0,t.jsx)(n.h2,{id:"assembly-registration",children:"Assembly registration"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["The batch registration API ",(0,t.jsx)(n.em,{children:"(filters, registration configuration action, self-registration)"})," is also usable for registering services from given assemblies."]}),(0,t.jsxs)(n.p,{children:["In this example, we assume that the same three services we used in the ",(0,t.jsx)(n.a,{href:"#batch-registration",children:"batch registration"})," section are in the same assembly."]}),(0,t.jsx)(n.admonition,{type:"info",children:(0,t.jsxs)(n.p,{children:["The container also detects and registers open-generic definitions (when applicable) from the supplied type collection. You can read about ",(0,t.jsx)(n.a,{href:"/docs/advanced/generics#open-generics",children:"open-generics here"}),"."]})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.Z,{children:[(0,t.jsx)(a.Z,{value:"Single assembly",label:"Single assembly",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterAssembly(typeof(DbBackup).Assembly,\n    // service filter, register to interfaces only\n    serviceTypeSelector: (impl, service) => service.IsInterface,\n    registerSelf: false,\n    configurator: options => options.WithoutDisposalTracking()\n);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"})})}),(0,t.jsx)(a.Z,{value:"Multiple assemblies",label:"Multiple assemblies",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterAssemblies(new[] \n    { \n        typeof(DbBackup).Assembly, \n        typeof(JobFromAnotherAssembly).Assembly \n    },\n    // service filter, register to interfaces only\n    serviceTypeSelector: (impl, service) => service.IsInterface,\n    registerSelf: false,\n    configurator: options => options.WithoutDisposalTracking()\n);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"})})}),(0,t.jsx)(a.Z,{value:"Containing type",label:"Containing type",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterAssemblyContaining<DbBackup>(\n    // service filter, register to interfaces only\n    serviceTypeSelector: (impl, service) => service.IsInterface,\n    registerSelf: false,\n    configurator: options => options.WithoutDisposalTracking()\n);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"})})})]})})]}),"\n",(0,t.jsx)(n.h2,{id:"composition-root",children:"Composition root"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["The ",(0,t.jsx)(n.a,{href:"https://blog.ploeh.dk/2011/07/28/CompositionRoot/",children:"Composition Root"})," is an entry point where all services required to make a component functional are wired together."]}),(0,t.jsxs)(n.p,{children:["Stashbox provides an ",(0,t.jsx)(n.code,{children:"ICompositionRoot"})," interface that can be used to define an entry point for a given component or even for an entire assembly."]}),(0,t.jsxs)(n.p,{children:["You can wire up your ",(0,t.jsx)(n.em,{children:"composition root"})," implementation with ",(0,t.jsx)(n.code,{children:"ComposeBy<TRoot>()"}),", or you can let the container find and execute all available ",(0,t.jsx)(n.em,{children:"composition root"})," implementations within an assembly."]}),(0,t.jsx)(n.admonition,{type:"note",children:(0,t.jsxs)(n.p,{children:["Your ",(0,t.jsx)(n.code,{children:"ICompositionRoot"})," implementation also can have dependencies that the container will resolve."]})})]}),(0,t.jsxs)("div",{children:[(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"class ExampleRoot : ICompositionRoot\n{\n    public ExampleRoot(IDependency rootDependency)\n    { }\n\n    public void Compose(IStashboxContainer container)\n    {\n       container.Register<IServiceA, ServiceA>();\n       container.Register<IServiceB, ServiceB>();\n    }\n}\n"})}),(0,t.jsxs)(o.Z,{children:[(0,t.jsx)(a.Z,{value:"Single",label:"Single",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// compose a single root.\ncontainer.ComposeBy<ExampleRoot>();\n"})})}),(0,t.jsx)(a.Z,{value:"Assembly",label:"Assembly",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// compose every root in the given assembly.\ncontainer.ComposeAssembly(typeof(IServiceA).Assembly);\n"})})}),(0,t.jsx)(a.Z,{value:"Override",label:"Override",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"// compose a single root with dependency override.\ncontainer.ComposeBy<ExampleRoot>(new CustomRootDependency());\n"})})})]})]})]}),"\n",(0,t.jsx)(n.h2,{id:"injection-parameters",children:"Injection parameters"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"If you have pre-evaluated dependencies you'd like to inject at resolution time, you can set them as injection parameters during registration."}),(0,t.jsx)(n.admonition,{type:"note",children:(0,t.jsx)(n.p,{children:"Injection parameter names are matched to constructor arguments or field/property names."})})]}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>(options => options\n    .WithInjectionParameter("logger", new ConsoleLogger())\n    .WithInjectionParameter("eventBroadcaster", new MessageBus());\n\n// the injection parameters will be passed to DbBackup\'s constructor.\nIJob backup = container.Resolve<IJob>();\n'})})})]}),"\n",(0,t.jsx)(n.h2,{id:"initializer--finalizer",children:"Initializer / finalizer"}),"\n",(0,t.jsxs)(i.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"The container provides specific extension points to let you react to lifetime events of an instantiated service."}),(0,t.jsxs)(n.p,{children:["For this reason, you can specify ",(0,t.jsx)(n.em,{children:"Initializer"})," and ",(0,t.jsx)(n.em,{children:"Finalizer"})," delegates. The ",(0,t.jsx)(n.em,{children:"finalizer"})," is called upon the service's ",(0,t.jsx)(n.a,{href:"/docs/guides/scopes#disposal",children:"disposal"}),", and the ",(0,t.jsx)(n.em,{children:"initializer"})," is called upon the service's construction."]})]}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<ILogger, FileLogger>(options => options\n    // delegate that called right after instantiation.\n    .WithInitializer((logger, resolver) => logger.OpenFile())\n    // delegate that called right before the instance's disposal.\n    .WithFinalizer(logger => logger.CloseFile()));\n"})})})]})]})}function g(e={}){const{wrapper:n}={...(0,r.a)(),...e.components};return n?(0,t.jsx)(n,{...e,children:(0,t.jsx)(u,{...e})}):u(e)}},5162:(e,n,s)=>{s.d(n,{Z:()=>o});s(7294);var t=s(4334);const r={tabItem:"tabItem_Ymn6"};var i=s(5893);function o(e){let{children:n,hidden:s,className:o}=e;return(0,i.jsx)("div",{role:"tabpanel",className:(0,t.Z)(r.tabItem,o),hidden:s,children:n})}},4866:(e,n,s)=>{s.d(n,{Z:()=>I});var t=s(7294),r=s(4334),i=s(2466),o=s(6550),a=s(469),l=s(1980),c=s(7392),d=s(12);function h(e){return t.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,t.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function p(e){const{values:n,children:s}=e;return(0,t.useMemo)((()=>{const e=n??function(e){return h(e).map((e=>{let{props:{value:n,label:s,attributes:t,default:r}}=e;return{value:n,label:s,attributes:t,default:r}}))}(s);return function(e){const n=(0,c.l)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,s])}function u(e){let{value:n,tabValues:s}=e;return s.some((e=>e.value===n))}function g(e){let{queryString:n=!1,groupId:s}=e;const r=(0,o.k6)(),i=function(e){let{queryString:n=!1,groupId:s}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!s)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return s??null}({queryString:n,groupId:s});return[(0,l._X)(i),(0,t.useCallback)((e=>{if(!i)return;const n=new URLSearchParams(r.location.search);n.set(i,e),r.replace({...r.location,search:n.toString()})}),[i,r])]}function m(e){const{defaultValue:n,queryString:s=!1,groupId:r}=e,i=p(e),[o,l]=(0,t.useState)((()=>function(e){let{defaultValue:n,tabValues:s}=e;if(0===s.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!u({value:n,tabValues:s}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${s.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const t=s.find((e=>e.default))??s[0];if(!t)throw new Error("Unexpected error: 0 tabValues");return t.value}({defaultValue:n,tabValues:i}))),[c,h]=g({queryString:s,groupId:r}),[m,b]=function(e){let{groupId:n}=e;const s=function(e){return e?`docusaurus.tab.${e}`:null}(n),[r,i]=(0,d.Nk)(s);return[r,(0,t.useCallback)((e=>{s&&i.set(e)}),[s,i])]}({groupId:r}),v=(()=>{const e=c??m;return u({value:e,tabValues:i})?e:null})();(0,a.Z)((()=>{v&&l(v)}),[v]);return{selectedValue:o,selectValue:(0,t.useCallback)((e=>{if(!u({value:e,tabValues:i}))throw new Error(`Can't select invalid tab value=${e}`);l(e),h(e),b(e)}),[h,b,i]),tabValues:i}}var b=s(2389);const v={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var j=s(5893);function x(e){let{className:n,block:s,selectedValue:t,selectValue:o,tabValues:a}=e;const l=[],{blockElementScrollPositionUntilNextRender:c}=(0,i.o5)(),d=e=>{const n=e.currentTarget,s=l.indexOf(n),r=a[s].value;r!==t&&(c(n),o(r))},h=e=>{let n=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const s=l.indexOf(e.currentTarget)+1;n=l[s]??l[0];break}case"ArrowLeft":{const s=l.indexOf(e.currentTarget)-1;n=l[s]??l[l.length-1];break}}n?.focus()};return(0,j.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,r.Z)("tabs",{"tabs--block":s},n),children:a.map((e=>{let{value:n,label:s,attributes:i}=e;return(0,j.jsx)("li",{role:"tab",tabIndex:t===n?0:-1,"aria-selected":t===n,ref:e=>l.push(e),onKeyDown:h,onClick:d,...i,className:(0,r.Z)("tabs__item",v.tabItem,i?.className,{"tabs__item--active":t===n}),children:s??n},n)}))})}function f(e){let{lazy:n,children:s,selectedValue:r}=e;const i=(Array.isArray(s)?s:[s]).filter(Boolean);if(n){const e=i.find((e=>e.props.value===r));return e?(0,t.cloneElement)(e,{className:"margin-top--md"}):null}return(0,j.jsx)("div",{className:"margin-top--md",children:i.map(((e,n)=>(0,t.cloneElement)(e,{key:n,hidden:e.props.value!==r})))})}function y(e){const n=m(e);return(0,j.jsxs)("div",{className:(0,r.Z)("tabs-container",v.tabList),children:[(0,j.jsx)(x,{...e,...n}),(0,j.jsx)(f,{...e,...n})]})}function I(e){const n=(0,b.Z)();return(0,j.jsx)(y,{...e,children:h(e.children)},String(n))}},8846:(e,n,s)=>{s.d(n,{Z:()=>o});var t=s(7294);const r={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var i=s(5893);function o(e){let{children:n}=e,s=t.Children.toArray(n).filter((e=>e));return(0,i.jsxs)("div",{className:r.codeDescContainer,children:[(0,i.jsx)("div",{className:r.desc,children:s[0]}),(0,i.jsx)("div",{className:r.example,children:s[1]})]})}},1151:(e,n,s)=>{s.d(n,{Z:()=>a,a:()=>o});var t=s(7294);const r={},i=t.createContext(r);function o(e){const n=t.useContext(i);return t.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function a(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(r):e.components||r:o(e.components),t.createElement(i.Provider,{value:n},e.children)}}}]);