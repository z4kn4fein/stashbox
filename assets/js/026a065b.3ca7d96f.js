"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[340],{2858:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>u,contentTitle:()=>l,default:()=>p,frontMatter:()=>c,metadata:()=>d,toc:()=>h});var i=t(5893),o=t(1151),r=t(8846),s=t(4866),a=t(5162);const c={},l="Service resolution",d={id:"guides/service-resolution",title:"Service resolution",description:"When you have all your components registered and configured adequately, you can resolve them from the container or a scope by requesting their service type.",source:"@site/docs/guides/service-resolution.md",sourceDirName:"guides",slug:"/guides/service-resolution",permalink:"/stashbox/docs/guides/service-resolution",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/service-resolution.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1719792327,formattedLastUpdatedAt:"Jul 1, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Advanced registration",permalink:"/stashbox/docs/guides/advanced-registration"},next:{title:"Lifetimes",permalink:"/stashbox/docs/guides/lifetimes"}},u={},h=[{value:"Injection patterns",id:"injection-patterns",level:2},{value:"Attributes",id:"attributes",level:2},{value:"Using your own attributes",id:"using-your-own-attributes",level:3},{value:"Dependency binding",id:"dependency-binding",level:2},{value:"Conventional resolution",id:"conventional-resolution",level:2},{value:"Conditional resolution",id:"conditional-resolution",level:2},{value:"Optional resolution",id:"optional-resolution",level:2},{value:"Dependency overrides",id:"dependency-overrides",level:2},{value:"Activation",id:"activation",level:2},{value:"Build-up",id:"build-up",level:3}];function g(e){const n={a:"a",admonition:"admonition",code:"code",em:"em",h1:"h1",h2:"h2",h3:"h3",li:"li",mdxAdmonitionTitle:"mdxAdmonitionTitle",p:"p",pre:"pre",strong:"strong",ul:"ul",...(0,o.a)(),...e.components};return(0,i.jsxs)(i.Fragment,{children:[(0,i.jsx)(n.h1,{id:"service-resolution",children:"Service resolution"}),"\n",(0,i.jsxs)(n.p,{children:["When you have all your components registered and configured adequately, you can resolve them from the container or a ",(0,i.jsx)(n.a,{href:"/docs/guides/scopes",children:"scope"})," by requesting their ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),"."]}),"\n",(0,i.jsxs)(n.p,{children:["During a service's resolution, the container walks through the entire ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree"})," and instantiates all dependencies required for the service construction.\nWhen the container encounters any violations of ",(0,i.jsx)(n.a,{href:"/docs/diagnostics/validation#resolution-validation",children:"these rules"})," ",(0,i.jsx)(n.em,{children:"(circular dependencies, missing required services, lifetime misconfigurations)"})," during the walkthrough, it lets you know that something is wrong by throwing a specific exception."]}),"\n",(0,i.jsx)(n.h2,{id:"injection-patterns",children:"Injection patterns"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Constructor injection"})," is the ",(0,i.jsx)(n.em,{children:"primary dependency injection pattern"}),". It encourages the organization of dependencies to a single place - the constructor."]}),(0,i.jsxs)(n.p,{children:["Stashbox, by default, uses the constructor that has the most parameters it knows how to resolve. This behavior is configurable through ",(0,i.jsx)(n.a,{href:"/docs/configuration/registration-configuration#constructor-selection",children:"constructor selection"}),"."]}),(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.a,{href:"/docs/configuration/registration-configuration#property-field-injection",children:"Property/field injection"})," is also supported in cases where constructor injection is not applicable."]}),(0,i.jsxs)(n.p,{children:["Members defined with C# 11's ",(0,i.jsx)(n.a,{href:"https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required",children:(0,i.jsx)(n.code,{children:"required"})})," keyword are automatically injected by the container.\nThis behavior can be controlled with ",(0,i.jsx)(n.a,{href:"/docs/configuration/registration-configuration#required-member-injection",children:"registration"})," or ",(0,i.jsx)(n.a,{href:"/docs/configuration/container-configuration#required-member-injection",children:"container"})," configuration options"]}),(0,i.jsxs)(n.admonition,{type:"info",children:[(0,i.jsx)(n.mdxAdmonitionTitle,{}),(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.a,{href:"/docs/configuration/container-configuration#constructor-selection",children:"Constructor selection"})," and ",(0,i.jsx)(n.a,{href:"/docs/configuration/container-configuration#auto-member-injection",children:"property/field injection"})," is also configurable container-wide."]})]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{children:[(0,i.jsx)(a.Z,{value:"Constructor injection",label:"Constructor injection",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob\n{\n    private readonly ILogger logger;\n    private readonly IEventBroadcaster eventBroadcaster;\n\n    public DbBackup(ILogger logger, IEventBroadcaster eventBroadcaster)\n    {\n        this.logger = logger;\n        this.eventBroadcaster = eventBroadcaster;\n    }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>();\ncontainer.Register<IEventBroadcaster, MessageBus>();\n\ncontainer.Register<IJob, DbBackup>();\n\n// resolution using the available constructor.\nIJob job = container.Resolve<IJob>();\n"})})}),(0,i.jsx)(a.Z,{value:"Property/field injection",label:"Property/field injection",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob\n{\n    public ILogger Logger { get; set; }\n    public IEventBroadcaster EventBroadcaster { get; set; }\n\n    public DbBackup() \n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>();\ncontainer.Register<IEventBroadcaster, MessageBus>();\n\n// registration of service with auto member injection.\ncontainer.Register<IJob, DbBackup>(options => \n    options.WithAutoMemberInjection());\n\n// resolution will inject the properties.\nIJob job = container.Resolve<IJob>();\n"})})})]})})]}),"\n",(0,i.jsx)(n.admonition,{type:"caution",children:(0,i.jsxs)(n.p,{children:["It's a common mistake to use the ",(0,i.jsx)(n.em,{children:"property/field injection"})," only to disencumber the constructor from having too many parameters. That's a code smell and also violates the ",(0,i.jsx)(n.a,{href:"https://en.wikipedia.org/wiki/Single-responsibility_principle",children:"Single-responsibility principle"}),". If you recognize these conditions, you should consider splitting your class into multiple smaller units rather than adding an extra property-injected dependency."]})}),"\n",(0,i.jsx)(n.h2,{id:"attributes",children:"Attributes"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(n.p,{children:"Attributes can give you control over how Stashbox selects dependencies for a service's resolution."}),(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Dependency attribute"}),":"]}),(0,i.jsxs)(n.ul,{children:["\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"On a constructor/method parameter"}),": used with the ",(0,i.jsx)(n.em,{children:"name"})," property, it works as a marker for ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#named-resolution",children:"named resolution"}),"."]}),"\n"]}),"\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"On a property/field"}),": first, it enables ",(0,i.jsx)(n.em,{children:"auto-injection"})," on the marked property/field (even if it wasn't configured at registration explicitly), and just as with the method parameter, it allows ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#named-resolution",children:"named resolution"}),"."]}),"\n"]}),"\n"]}),(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"DependencyName attribute"}),": a parameter marked with this attribute will get the related service's dependency name."]}),(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"InjectionMethod attribute"}),": marks a method to be called when the requested service is instantiated."]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{children:[(0,i.jsx)(a.Z,{value:"Constructor",label:"Constructor",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    private readonly ILogger logger;\n\n    public DbBackup([Dependency("Console")]ILogger logger)\n    {\n        this.logger = logger;\n    }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'})})}),(0,i.jsx)(a.Z,{value:"Property/field",label:"Property/field",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    [Dependency("Console")]\n    public ILogger Logger { get; set; }\n\n    public DbBackup() \n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'})})}),(0,i.jsx)(a.Z,{value:"DependencyName",label:"DependencyName",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    public string Name { get; set; }\n\n    public DbBackup([DependencyName] string name) \n    { }\n}\n\ncontainer.Register<IJob, DbBackup>("Backup");\n\n\nIJob job = container.Resolve<IJob>();\n// name is "Backup".\nvar name = job.Name;\n'})})}),(0,i.jsx)(a.Z,{value:"Method",label:"Method",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    [InjectionMethod]\n    public void Initialize([Dependency("Console")]ILogger logger)\n    {\n        this.logger.Log("Initializing.");\n    }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will call DbBackup\'s Initialize method.\nIJob job = container.Resolve<IJob>();\n'})})})]})})]}),"\n",(0,i.jsx)(n.admonition,{type:"caution",children:(0,i.jsxs)(n.p,{children:["Attributes provide a more straightforward configuration, but using them also tightens the bond between your application and Stashbox. If you consider this an issue, you can use the ",(0,i.jsx)(n.a,{href:"/docs/guides/service-resolution#dependency-binding",children:"dependency binding"})," API or ",(0,i.jsx)(n.a,{href:"/docs/guides/service-resolution#using-your-own-attributes",children:"your own attributes"}),"."]})}),"\n",(0,i.jsx)(n.h3,{id:"using-your-own-attributes",children:"Using your own attributes"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(n.p,{children:"There's an option to extend the container's dependency finding mechanism with your own attributes."}),(0,i.jsxs)(n.ul,{children:["\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Additional Dependency attributes"}),": you can use the ",(0,i.jsx)(n.a,{href:"/docs/configuration/container-configuration#withadditionaldependencyattribute",children:(0,i.jsx)(n.code,{children:".WithAdditionalDependencyAttribute()"})})," container configuration option to let the container know that it should watch for additional attributes besides the built-in ",(0,i.jsx)(n.a,{href:"/docs/guides/service-resolution#attributes",children:(0,i.jsx)(n.code,{children:"Dependency"})})," attribute upon building up the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree"}),"."]}),"\n"]}),"\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Additional DependencyName attributes"}),": you can use the ",(0,i.jsx)(n.a,{href:"/docs/configuration/container-configuration#withadditionaldependencynameattribute",children:(0,i.jsx)(n.code,{children:".WithAdditionalDependencyNameAttribute()"})})," container configuration option to use additional dependency name indicator attributes besides the built-in ",(0,i.jsx)(n.a,{href:"/docs/guides/service-resolution#attributes",children:(0,i.jsx)(n.code,{children:"DependencyName"})})," attribute."]}),"\n"]}),"\n"]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{children:[(0,i.jsx)(a.Z,{value:"Dependency",label:"Dependency",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    [CustomDependency("Console")]\n    public ILogger Logger { get; set; }\n\n    public DbBackup() \n    { }\n}\n\nvar container = new StashboxContainer(options => options\n    .WithAdditionalDependencyAttribute<CustomDependencyAttribute>());\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'})})}),(0,i.jsx)(a.Z,{value:"DependencyName",label:"DependencyName",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    public string Name { get; set; }\n\n    public DbBackup([CustomName] string name) \n    { }\n}\n\nvar container = new StashboxContainer(options => options\n    .WithAdditionalDependencyNameAttribute<CustomNameAttribute>());\n\ncontainer.Register<IJob, DbBackup>("Backup");\n\nIJob job = container.Resolve<IJob>();\n// name is "Backup".\nvar name = job.Name;\n'})})})]})})]}),"\n",(0,i.jsx)(n.h2,{id:"dependency-binding",children:"Dependency binding"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(n.p,{children:"The same dependency configuration functionality as attributes, but without attributes."}),(0,i.jsxs)(n.ul,{children:["\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Binding to a parameter"}),": the same functionality as the ",(0,i.jsx)(n.a,{href:"/docs/guides/service-resolution#attributes",children:(0,i.jsx)(n.code,{children:"Dependency"})})," attribute on a constructor or method parameter, enabling ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#named-resolution",children:"named resolution"}),"."]}),"\n"]}),"\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Binding to a property/field"}),": the same functionality as the ",(0,i.jsx)(n.a,{href:"/docs/guides/service-resolution#attributes",children:(0,i.jsx)(n.code,{children:"Dependency"})})," attribute, enabling the injection of the given property/field."]}),"\n"]}),"\n"]}),(0,i.jsx)(n.admonition,{type:"info",children:(0,i.jsxs)(n.p,{children:["There are further dependency binding options ",(0,i.jsx)(n.a,{href:"/docs/configuration/registration-configuration#dependency-configuration",children:"available"})," on the registration configuration API."]})})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{children:[(0,i.jsx)(a.Z,{value:"Bind to parameter",label:"Bind to parameter",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\n// registration of service with the dependency binding.\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithDependencyBinding("logger", "Console"));\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'})})}),(0,i.jsx)(a.Z,{value:"Bind to property / field",label:"Bind to property / field",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    public ILogger Logger { get; set; }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\n// registration of service with the member injection.\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithDependencyBinding("Logger", "Console"));\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'})})})]})})]}),"\n",(0,i.jsx)(n.h2,{id:"conventional-resolution",children:"Conventional resolution"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(n.p,{children:"When you enable conventional resolution, the container treats member and method parameter names as their dependency identifier."}),(0,i.jsx)(n.p,{children:"It's like an implicit dependency binding on every class member."}),(0,i.jsx)(n.p,{children:"First, you have to enable conventional resolution through the configuration of the container:"}),(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"new StashboxContainer(options => options\n    .TreatParameterAndMemberNameAsDependencyName());\n"})}),(0,i.jsx)(n.admonition,{type:"note",children:(0,i.jsxs)(n.p,{children:["The container will attempt a ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#named-resolution",children:"named resolution"})," on each dependency based on their parameter or property/field name."]})})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{children:[(0,i.jsx)(a.Z,{value:"Parameters",label:"Parameters",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    public DbBackup(\n        // the parameter name identifies the dependency.\n        ILogger consoleLogger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("consoleLogger");\ncontainer.Register<ILogger, FileLogger>("fileLogger");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'})})}),(0,i.jsx)(a.Z,{value:"Properties / fields",label:"Properties / fields",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:'class DbBackup : IJob\n{\n    // the property name identifies the dependency.\n    public ILogger ConsoleLogger { get; set; }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("ConsoleLogger");\ncontainer.Register<ILogger, FileLogger>("FileLogger");\n\n// registration of service with auto member injection.\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithAutoMemberInjection());\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'})})})]})})]}),"\n",(0,i.jsx)(n.h2,{id:"conditional-resolution",children:"Conditional resolution"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(n.p,{children:"Stashbox can resolve a particular dependency based on its context. This context is typically the reflected type of dependency, its usage, and the type it gets injected into."}),(0,i.jsxs)(n.ul,{children:["\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Attribute"}),": you can filter on constructor, method, property, or field attributes to select the desired dependency for your service. In contrast to the ",(0,i.jsx)(n.code,{children:"Dependency"})," attribute, this configuration doesn't tie your application to Stashbox because you use your own attributes."]}),"\n"]}),"\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Parent type"}),": you can filter on what type the given service is injected into."]}),"\n"]}),"\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Resolution path"}),": similar to the parent type and attribute condition but extended with inheritance. You can set that the given service is only usable in a type's resolution path. This means that each direct and sub-dependency of the selected type must use the provided service as a dependency."]}),"\n"]}),"\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"Custom"}),": with this, you can build your own selection logic based on the given contextual type information."]}),"\n"]}),"\n"]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{children:[(0,i.jsx)(a.Z,{value:"Attribute",label:"Attribute",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class ConsoleAttribute : Attribute { }\n\nclass DbBackup : IJob\n{\n    public DbBackup([Console]ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // resolve only when the injected parameter, \n    // property or field has the 'Console' attribute\n    .WhenHas<ConsoleAttribute>());\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n"})})}),(0,i.jsx)(a.Z,{value:"Parent",label:"Parent",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // inject only when we are \n    // currently resolving DbBackup OR StorageCleanup.\n    .WhenDependantIs<DbBackup>()\n    .WhenDependantIs<StorageCleanup>());\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n"})})}),(0,i.jsx)(a.Z,{value:"Path",label:"Path",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob\n{\n    public DbBackup(IStorage storage)\n    { }\n}\n\nclass FileStorage : IStorage\n{\n    public FileStorage(ILogger logger) \n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // inject only when we are in the\n    // resolution path of DbBackup\n    .WhenInResolutionPathOf<DbBackup>());\n\ncontainer.Register<IStorage, FileStorage>();\ncontainer.Register<IJob, DbBackup>();\n\n// the container will select ConsoleLogger for FileStorage\n// because they are injected into DbBackup.\nIJob job = container.Resolve<IJob>();\n"})})}),(0,i.jsx)(a.Z,{value:"Custom",label:"Custom",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // inject only when we are \n    // currently resolving DbBackup.\n    .When(typeInfo => typeInfo.ParentType.Equals(typeof(DbBackup))));\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n"})})}),(0,i.jsx)(a.Z,{value:"Collection",label:"Collection",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbJobsExecutor : IJobsExecutor\n{\n    public DbBackup(IEnumerable<IJob> jobs)\n    { }\n}\n\ncontainer.Register<IJob, DbBackup>(options => options\n    .WhenDependantIs<DbJobsExecutor>());\ncontainer.Register<IJob, DbCleanup>(options => options\n    .WhenDependantIs<DbJobsExecutor>());\nontainer.Register<IJob, StorageCleanup>();\n\ncontainer.Register<IJobsExecutor, DbJobsExecutor>();\n\n// jobsExecutor will get DbBackup and DbCleanup within a collection.\nIJobsExecutor jobsExecutor = container.Resolve<IJobsExecutor>();\n"})})})]})})]}),"\n",(0,i.jsxs)(n.p,{children:["The specified conditions are behaving like filters when a ",(0,i.jsx)(n.strong,{children:"collection"})," is requested."]}),"\n",(0,i.jsxs)(n.p,{children:["When you use the same conditional option multiple times, the container will evaluate them ",(0,i.jsx)(n.strong,{children:"with OR"})," logical operator."]}),"\n",(0,i.jsx)(n.admonition,{type:"tip",children:(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.a,{href:"/docs/configuration/registration-configuration#conditions",children:"Here"})," you can find each condition related registration option."]})}),"\n",(0,i.jsx)(n.h2,{id:"optional-resolution",children:"Optional resolution"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsxs)(n.p,{children:["In cases where it's not guaranteed that a service is resolvable, either because it's not registered or any of its dependencies are missing, you can attempt an optional resolution using the ",(0,i.jsx)(n.code,{children:"ResolveOrDefault()"})," method."]}),(0,i.jsxs)(n.p,{children:["When the resolution attempt fails, it will return ",(0,i.jsx)(n.code,{children:"null"})," (or ",(0,i.jsx)(n.code,{children:"default"})," in case of value types)."]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{groupId:"generic-runtime-apis",children:[(0,i.jsx)(a.Z,{value:"Generic API",label:"Generic API",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"// returns null when the resolution fails.\nIJob job = container.ResolveOrDefault<IJob>();\n\n// throws ResolutionFailedException when the resolution fails.\nIJob job = container.Resolve<IJob>();\n"})})}),(0,i.jsx)(a.Z,{value:"Runtime type API",label:"Runtime type API",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"// returns null when the resolution fails.\nobject job = container.ResolveOrDefault(typeof(IJob));\n\n// throws ResolutionFailedException when the resolution fails.\nobject job = container.Resolve(typeof(IJob));\n"})})})]})})]}),"\n",(0,i.jsx)(n.h2,{id:"dependency-overrides",children:"Dependency overrides"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsxs)(n.p,{children:["At resolution time, you can override a service's dependencies by passing an ",(0,i.jsx)(n.code,{children:"object[]"})," to the ",(0,i.jsx)(n.code,{children:"Resolve()"})," method."]}),(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n"})})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{groupId:"generic-runtime-apis",children:[(0,i.jsx)(a.Z,{value:"Generic API",label:"Generic API",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"DbBackup backup = container.Resolve<DbBackup>( \n    dependencyOverrides: new object[] \n    { \n        new ConsoleLogger() \n    });\n"})})}),(0,i.jsx)(a.Z,{value:"Runtime type API",label:"Runtime type API",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"object backup = container.Resolve(typeof(DbBackup),\n    dependencyOverrides: new object[] \n    { \n        new ConsoleLogger() \n    });\n"})})})]})})]}),"\n",(0,i.jsx)(n.h2,{id:"activation",children:"Activation"}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsxs)(n.p,{children:["You can use the container's ",(0,i.jsx)(n.code,{children:".Activate()"})," method when you only want to build up an instance from a type on the fly without registration."]}),(0,i.jsxs)(n.p,{children:["It allows dependency overriding with ",(0,i.jsx)(n.code,{children:"object"})," arguments and performs property/field/method injection (when configured)."]}),(0,i.jsxs)(n.p,{children:["It works like ",(0,i.jsx)(n.code,{children:"Activator.CreateInstance()"})," except that Stashbox supplies the dependencies."]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(s.Z,{groupId:"generic-runtime-apis",children:[(0,i.jsx)(a.Z,{value:"Generic API",label:"Generic API",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"// use dependency injected by container.\nDbBackup backup = container.Activate<DbBackup>();\n\n// override the injected dependency.\nDbBackup backup = container.Activate<DbBackup>(new ConsoleLogger());\n"})})}),(0,i.jsx)(a.Z,{value:"Runtime type API",label:"Runtime type API",children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"// use dependency injected by container.\nobject backup = container.Activate(typeof(DbBackup));\n\n// override the injected dependency.\nobject backup = container.Activate(typeof(DbBackup), new ConsoleLogger());\n"})})})]})})]}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(n.h3,{id:"build-up",children:"Build-up"}),(0,i.jsxs)(n.p,{children:["With the ",(0,i.jsx)(n.code,{children:".BuildUp()"})," method, you can do the same ",(0,i.jsx)(n.em,{children:"on the fly"})," post-processing (property/field/method injection) on already constructed instances."]}),(0,i.jsx)(n.admonition,{type:"caution",children:(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.code,{children:".BuildUp()"})," won't register the given instance into the container."]})})]}),(0,i.jsx)("div",{children:(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class DbBackup : IJob\n{\n    public ILogger Logger { get; set; }\n}\n\nDbBackup backup = new DbBackup();\n// the container fills the Logger property.\ncontainer.BuildUp(backup); \n"})})})]})]})}function p(e={}){const{wrapper:n}={...(0,o.a)(),...e.components};return n?(0,i.jsx)(n,{...e,children:(0,i.jsx)(g,{...e})}):g(e)}},5162:(e,n,t)=>{t.d(n,{Z:()=>s});t(7294);var i=t(4334);const o={tabItem:"tabItem_Ymn6"};var r=t(5893);function s(e){let{children:n,hidden:t,className:s}=e;return(0,r.jsx)("div",{role:"tabpanel",className:(0,i.Z)(o.tabItem,s),hidden:t,children:n})}},4866:(e,n,t)=>{t.d(n,{Z:()=>I});var i=t(7294),o=t(4334),r=t(2466),s=t(6550),a=t(469),c=t(1980),l=t(7392),d=t(12);function u(e){return i.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,i.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function h(e){const{values:n,children:t}=e;return(0,i.useMemo)((()=>{const e=n??function(e){return u(e).map((e=>{let{props:{value:n,label:t,attributes:i,default:o}}=e;return{value:n,label:t,attributes:i,default:o}}))}(t);return function(e){const n=(0,l.l)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,t])}function g(e){let{value:n,tabValues:t}=e;return t.some((e=>e.value===n))}function p(e){let{queryString:n=!1,groupId:t}=e;const o=(0,s.k6)(),r=function(e){let{queryString:n=!1,groupId:t}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!t)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return t??null}({queryString:n,groupId:t});return[(0,c._X)(r),(0,i.useCallback)((e=>{if(!r)return;const n=new URLSearchParams(o.location.search);n.set(r,e),o.replace({...o.location,search:n.toString()})}),[r,o])]}function b(e){const{defaultValue:n,queryString:t=!1,groupId:o}=e,r=h(e),[s,c]=(0,i.useState)((()=>function(e){let{defaultValue:n,tabValues:t}=e;if(0===t.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!g({value:n,tabValues:t}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${t.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const i=t.find((e=>e.default))??t[0];if(!i)throw new Error("Unexpected error: 0 tabValues");return i.value}({defaultValue:n,tabValues:r}))),[l,u]=p({queryString:t,groupId:o}),[b,j]=function(e){let{groupId:n}=e;const t=function(e){return e?`docusaurus.tab.${e}`:null}(n),[o,r]=(0,d.Nk)(t);return[o,(0,i.useCallback)((e=>{t&&r.set(e)}),[t,r])]}({groupId:o}),x=(()=>{const e=l??b;return g({value:e,tabValues:r})?e:null})();(0,a.Z)((()=>{x&&c(x)}),[x]);return{selectedValue:s,selectValue:(0,i.useCallback)((e=>{if(!g({value:e,tabValues:r}))throw new Error(`Can't select invalid tab value=${e}`);c(e),u(e),j(e)}),[u,j,r]),tabValues:r}}var j=t(2389);const x={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var m=t(5893);function v(e){let{className:n,block:t,selectedValue:i,selectValue:s,tabValues:a}=e;const c=[],{blockElementScrollPositionUntilNextRender:l}=(0,r.o5)(),d=e=>{const n=e.currentTarget,t=c.indexOf(n),o=a[t].value;o!==i&&(l(n),s(o))},u=e=>{let n=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const t=c.indexOf(e.currentTarget)+1;n=c[t]??c[0];break}case"ArrowLeft":{const t=c.indexOf(e.currentTarget)-1;n=c[t]??c[c.length-1];break}}n?.focus()};return(0,m.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":t},n),children:a.map((e=>{let{value:n,label:t,attributes:r}=e;return(0,m.jsx)("li",{role:"tab",tabIndex:i===n?0:-1,"aria-selected":i===n,ref:e=>c.push(e),onKeyDown:u,onClick:d,...r,className:(0,o.Z)("tabs__item",x.tabItem,r?.className,{"tabs__item--active":i===n}),children:t??n},n)}))})}function y(e){let{lazy:n,children:t,selectedValue:o}=e;const r=(Array.isArray(t)?t:[t]).filter(Boolean);if(n){const e=r.find((e=>e.props.value===o));return e?(0,i.cloneElement)(e,{className:"margin-top--md"}):null}return(0,m.jsx)("div",{className:"margin-top--md",children:r.map(((e,n)=>(0,i.cloneElement)(e,{key:n,hidden:e.props.value!==o})))})}function f(e){const n=b(e);return(0,m.jsxs)("div",{className:(0,o.Z)("tabs-container",x.tabList),children:[(0,m.jsx)(v,{...e,...n}),(0,m.jsx)(y,{...e,...n})]})}function I(e){const n=(0,j.Z)();return(0,m.jsx)(f,{...e,children:u(e.children)},String(n))}},8846:(e,n,t)=>{t.d(n,{Z:()=>s});var i=t(7294);const o={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var r=t(5893);function s(e){let{children:n}=e,t=i.Children.toArray(n).filter((e=>e));return(0,r.jsxs)("div",{className:o.codeDescContainer,children:[(0,r.jsx)("div",{className:o.desc,children:t[0]}),(0,r.jsx)("div",{className:o.example,children:t[1]})]})}},1151:(e,n,t)=>{t.d(n,{Z:()=>a,a:()=>s});var i=t(7294);const o={},r=i.createContext(o);function s(e){const n=i.useContext(r);return i.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function a(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(o):e.components||o:s(e.components),i.createElement(r.Provider,{value:n},e.children)}}}]);