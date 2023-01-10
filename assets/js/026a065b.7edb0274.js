"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[340],{3905:(e,n,t)=>{t.d(n,{Zo:()=>p,kt:()=>b});var o=t(7294);function a(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function r(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);n&&(o=o.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,o)}return t}function i(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?r(Object(t),!0).forEach((function(n){a(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):r(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function l(e,n){if(null==e)return{};var t,o,a=function(e,n){if(null==e)return{};var t,o,a={},r=Object.keys(e);for(o=0;o<r.length;o++)t=r[o],n.indexOf(t)>=0||(a[t]=e[t]);return a}(e,n);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);for(o=0;o<r.length;o++)t=r[o],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(a[t]=e[t])}return a}var s=o.createContext({}),c=function(e){var n=o.useContext(s),t=n;return e&&(t="function"==typeof e?e(n):i(i({},n),e)),t},p=function(e){var n=c(e.components);return o.createElement(s.Provider,{value:n},e.children)},u="mdxType",d={inlineCode:"code",wrapper:function(e){var n=e.children;return o.createElement(o.Fragment,{},n)}},g=o.forwardRef((function(e,n){var t=e.components,a=e.mdxType,r=e.originalType,s=e.parentName,p=l(e,["components","mdxType","originalType","parentName"]),u=c(t),g=a,b=u["".concat(s,".").concat(g)]||u[g]||d[g]||r;return t?o.createElement(b,i(i({ref:n},p),{},{components:t})):o.createElement(b,i({ref:n},p))}));function b(e,n){var t=arguments,a=n&&n.mdxType;if("string"==typeof e||a){var r=t.length,i=new Array(r);i[0]=g;var l={};for(var s in n)hasOwnProperty.call(n,s)&&(l[s]=n[s]);l.originalType=e,l[u]="string"==typeof e?e:a,i[1]=l;for(var c=2;c<r;c++)i[c]=t[c];return o.createElement.apply(null,i)}return o.createElement.apply(null,t)}g.displayName="MDXCreateElement"},5162:(e,n,t)=>{t.d(n,{Z:()=>i});var o=t(7294),a=t(6010);const r="tabItem_Ymn6";function i(e){let{children:n,hidden:t,className:i}=e;return o.createElement("div",{role:"tabpanel",className:(0,a.Z)(r,i),hidden:t},n)}},5488:(e,n,t)=>{t.d(n,{Z:()=>g});var o=t(7462),a=t(7294),r=t(6010),i=t(2389),l=t(7392),s=t(7094),c=t(2466);const p="tabList__CuJ",u="tabItem_LNqP";function d(e){const{lazy:n,block:t,defaultValue:i,values:d,groupId:g,className:b}=e,m=a.Children.map(e.children,(e=>{if((0,a.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),k=d??m.map((e=>{let{props:{value:n,label:t,attributes:o}}=e;return{value:n,label:t,attributes:o}})),h=(0,l.l)(k,((e,n)=>e.value===n.value));if(h.length>0)throw new Error(`Docusaurus error: Duplicate values "${h.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const y=null===i?i:i??m.find((e=>e.props.default))?.props.value??m[0].props.value;if(null!==y&&!k.some((e=>e.value===y)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${y}" but none of its children has the corresponding value. Available values are: ${k.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:v,setTabGroupChoices:f}=(0,s.U)(),[I,N]=(0,a.useState)(y),D=[],{blockElementScrollPositionUntilNextRender:w}=(0,c.o5)();if(null!=g){const e=v[g];null!=e&&e!==I&&k.some((n=>n.value===e))&&N(e)}const C=e=>{const n=e.currentTarget,t=D.indexOf(n),o=k[t].value;o!==I&&(w(n),N(o),null!=g&&f(g,String(o)))},B=e=>{let n=null;switch(e.key){case"Enter":C(e);break;case"ArrowRight":{const t=D.indexOf(e.currentTarget)+1;n=D[t]??D[0];break}case"ArrowLeft":{const t=D.indexOf(e.currentTarget)-1;n=D[t]??D[D.length-1];break}}n?.focus()};return a.createElement("div",{className:(0,r.Z)("tabs-container",p)},a.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,r.Z)("tabs",{"tabs--block":t},b)},k.map((e=>{let{value:n,label:t,attributes:i}=e;return a.createElement("li",(0,o.Z)({role:"tab",tabIndex:I===n?0:-1,"aria-selected":I===n,key:n,ref:e=>D.push(e),onKeyDown:B,onClick:C},i,{className:(0,r.Z)("tabs__item",u,i?.className,{"tabs__item--active":I===n})}),t??n)}))),n?(0,a.cloneElement)(m.filter((e=>e.props.value===I))[0],{className:"margin-top--md"}):a.createElement("div",{className:"margin-top--md"},m.map(((e,n)=>(0,a.cloneElement)(e,{key:n,hidden:e.props.value!==I})))))}function g(e){const n=(0,i.Z)();return a.createElement(d,(0,o.Z)({key:String(n)},e))}},8846:(e,n,t)=>{t.d(n,{Z:()=>l});var o=t(7294);const a="codeDescContainer_ie8f",r="desc_jyqI",i="example_eYlF";function l(e){let{children:n}=e,t=o.Children.toArray(n).filter((e=>e));return o.createElement("div",{className:a},o.createElement("div",{className:r},t[0]),o.createElement("div",{className:i},t[1]))}},385:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>u,contentTitle:()=>c,default:()=>b,frontMatter:()=>s,metadata:()=>p,toc:()=>d});var o=t(7462),a=(t(7294),t(3905)),r=t(8846),i=t(5488),l=t(5162);const s={},c="Service resolution",p={unversionedId:"guides/service-resolution",id:"guides/service-resolution",title:"Service resolution",description:"When you have all your components registered and configured adequately, you can resolve them from the container or a scope by requesting their service type.",source:"@site/docs/guides/service-resolution.md",sourceDirName:"guides",slug:"/guides/service-resolution",permalink:"/stashbox/docs/guides/service-resolution",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/service-resolution.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1673344472,formattedLastUpdatedAt:"Jan 10, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Advanced registration",permalink:"/stashbox/docs/guides/advanced-registration"},next:{title:"Lifetimes",permalink:"/stashbox/docs/guides/lifetimes"}},u={},d=[{value:"Injection patterns",id:"injection-patterns",level:2},{value:"Attributes",id:"attributes",level:2},{value:"Dependency binding",id:"dependency-binding",level:2},{value:"Conventional resolution",id:"conventional-resolution",level:2},{value:"Conditional resolution",id:"conditional-resolution",level:2},{value:"Optional resolution",id:"optional-resolution",level:2},{value:"Dependency overrides",id:"dependency-overrides",level:2},{value:"Activation",id:"activation",level:2},{value:"Build-up",id:"build-up",level:3}],g={toc:d};function b(e){let{components:n,...t}=e;return(0,a.kt)("wrapper",(0,o.Z)({},g,t,{components:n,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"service-resolution"},"Service resolution"),(0,a.kt)("p",null,"When you have all your components registered and configured adequately, you can resolve them from the container or a ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope")," by requesting their ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),"."),(0,a.kt)("p",null,"During a service's resolution, the container walks through the entire ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#resolution-tree"},"resolution tree")," and instantiates all dependencies required for the service construction.\nWhen the container encounters any violations of ",(0,a.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#resolution-validation"},"these rules")," ",(0,a.kt)("em",{parentName:"p"},"(circular dependencies, missing required services, lifetime misconfigurations)")," during the walkthrough, it lets you know that something is wrong by throwing the appropriate exception."),(0,a.kt)("h2",{id:"injection-patterns"},"Injection patterns"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Constructor injection")," is the ",(0,a.kt)("em",{parentName:"p"},"primary dependency injection pattern"),". It encourages the organization of the dependencies to a single place - the constructor."),(0,a.kt)("p",null,"Stashbox, by default, uses the constructor that has the most parameters it knows how to resolve. This behavior is configurable through ",(0,a.kt)("em",{parentName:"p"},(0,a.kt)("a",{parentName:"em",href:"/docs/configuration/registration-configuration#constructor-selection"},"constructor selection")),"."),(0,a.kt)("p",null,(0,a.kt)("em",{parentName:"p"},(0,a.kt)("a",{parentName:"em",href:"/docs/configuration/registration-configuration#property-field-injection"},"Property / field injection"))," is also supported in cases where constructor injection is not applicable."),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#constructor-selection"},"Constructor selection")," and ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#auto-member-injection"},"property / field injection")," is also configurable container-wide."))),(0,a.kt)("div",null,(0,a.kt)(i.Z,{mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Constructor injection",label:"Constructor injection",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob\n{\n    private readonly ILogger logger;\n    private readonly IEventBroadcaster eventBroadcaster;\n\n    public DbBackup(ILogger logger, IEventBroadcaster eventBroadcaster)\n    {\n        this.logger = logger;\n        this.eventBroadcaster = eventBroadcaster;\n    }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>();\ncontainer.Register<IEventBroadcaster, MessageBus>();\n\ncontainer.Register<IJob, DbBackup>();\n\n// resolution using the available constructor.\nIJob job = container.Resolve<IJob>();\n"))),(0,a.kt)(l.Z,{value:"Property / field injection",label:"Property / field injection",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob\n{\n    public ILogger Logger { get; set; }\n    public IEventBroadcaster EventBroadcaster { get; set; }\n\n    public DbBackup() \n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>();\ncontainer.Register<IEventBroadcaster, MessageBus>();\n\n// registration of service with auto member injection.\ncontainer.Register<IJob, DbBackup>(options => \n    options.WithAutoMemberInjection());\n\n// resolution will inject the properties.\nIJob job = container.Resolve<IJob>();\n")))))),(0,a.kt)("admonition",{type:"caution"},(0,a.kt)("p",{parentName:"admonition"},"It's a common mistake to use the ",(0,a.kt)("em",{parentName:"p"},"property / field injection")," only to disencumber the constructor from having too many parameters. That's a code smell and also a violation of the ",(0,a.kt)("a",{parentName:"p",href:"https://en.wikipedia.org/wiki/Single-responsibility_principle"},"Single-responsibility principle"),". If you recognize these conditions, you might consider not adding that extra property-injected dependency into your class but instead split it into multiple smaller units. ")),(0,a.kt)("h2",{id:"attributes"},"Attributes"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"Attributes give you control over how Stashbox selects dependencies for a service's resolution."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Dependency attribute"),": "),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"On a constructor / method parameter"),": used with the ",(0,a.kt)("em",{parentName:"p"},"name")," property, it works as a marker for ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#named-resolution"},"named resolution"),".")),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"On a property / field"),": first, it enables the ",(0,a.kt)("em",{parentName:"p"},"auto-injection")," of the marked property / field (even if it wasn't configured at registration), and just as with the method parameter, it allows ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#named-resolution"},"named resolution"),"."))),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"InjectionMethod attribute"),": marks a method to be called when the requested service is being instantiated.")),(0,a.kt)("div",null,(0,a.kt)(i.Z,{mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Constructor",label:"Constructor",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'class DbBackup : IJob\n{\n    private readonly ILogger logger;\n\n    public DbBackup([Dependency("Console")]ILogger logger)\n    {\n        this.logger = logger;\n    }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'))),(0,a.kt)(l.Z,{value:"Property / field",label:"Property / field",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'class DbBackup : IJob\n{\n    [Dependency("Console")]\n    public ILogger Logger { get; set; }\n\n    public DbBackup() \n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'))),(0,a.kt)(l.Z,{value:"Method",label:"Method",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'class DbBackup : IJob\n{\n    [InjectionMethod]\n    public void Initialize([Dependency("Console")]ILogger logger)\n    {\n        this.logger.Log("Initializing.");\n    }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will call DbBackup\'s Initialize method.\nIJob job = container.Resolve<IJob>();\n')))))),(0,a.kt)("admonition",{type:"caution"},(0,a.kt)("p",{parentName:"admonition"},"Attributes provide a more straightforward configuration, but using them also tightens the bond between your application and Stashbox. If that's an issue for you, the same functionality is available on the ",(0,a.kt)("em",{parentName:"p"},"registration API")," as ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#dependency-binding"},"dependency binding"),".")),(0,a.kt)("h2",{id:"dependency-binding"},"Dependency binding"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"The same dependency configuration as attributes have is available using the registration configuration API."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Bind to parameter"),": it has the same functionality as the ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#attributes"},"Dependency attribute")," on a constructor or method parameter, enables the ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#named-resolution"},"named resolution"),"."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Bind to property / field"),": it has the same functionality as the ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#attributes"},"Dependency attribute"),"; it enables the injection of the given property / field."),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"There are more dependency binding options ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/registration-configuration#dependency-configuration"},"available"),"."))),(0,a.kt)("div",null,(0,a.kt)(i.Z,{mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Bind to parameter",label:"Bind to parameter",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\n// registration of service with the dependency binding.\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithDependencyBinding("logger", "Console"));\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'))),(0,a.kt)(l.Z,{value:"Bind to property / field",label:"Bind to property / field",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'class DbBackup : IJob\n{\n    public ILogger Logger { get; set; }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("Console");\ncontainer.Register<ILogger, FileLogger>("File");\n\n// registration of service with the member injection.\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithDependencyBinding("Logger", "Console"));\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n')))))),(0,a.kt)("h2",{id:"conventional-resolution"},"Conventional resolution"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"When you enable the conventional resolution, the container treats the member and method parameter names as their dependency identifier. "),(0,a.kt)("p",null,"It's like an implicit dependency binding on every class member."),(0,a.kt)("p",null,"First, you have to enable the conventional resolution through the configuration of the container:  "),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .TreatParameterAndMemberNameAsDependencyName());\n")),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"The container will attempt ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#named-resolution"},"named resolution")," on every dependency based on parameter or property / field name."))),(0,a.kt)("div",null,(0,a.kt)(i.Z,{mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Parameters",label:"Parameters",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'class DbBackup : IJob\n{\n    public DbBackup(\n        // the parameter name identifies the dependency.\n        ILogger consoleLogger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("consoleLogger");\ncontainer.Register<ILogger, FileLogger>("fileLogger");\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n'))),(0,a.kt)(l.Z,{value:"Properties / fields",label:"Properties / fields",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'class DbBackup : IJob\n{\n    // the property name identifies the dependency.\n    public ILogger ConsoleLogger { get; set; }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>("ConsoleLogger");\ncontainer.Register<ILogger, FileLogger>("FileLogger");\n\n// registration of service with auto member injection.\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithAutoMemberInjection());\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n')))))),(0,a.kt)("h2",{id:"conditional-resolution"},"Conditional resolution"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"Stashbox can resolve a particular dependency based on its context. This context is typically the reflected type information of the dependency, its usage, and the type it gets injected into."),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"Attribute"),": you can filter on constructor, method, property, or field attributes to select the desired dependency for your service. In contrast to the ",(0,a.kt)("inlineCode",{parentName:"p"},"Dependency")," attribute, this configuration method doesn't tie your application to Stashbox because you can use your attributes.")),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"Parent type"),": you can filter on what type the given service is injected into.")),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"Resolution path"),": similar to the parent type and attribute condition but extended with inheritance. You can set that the given service is only usable in a type's resolution path. This means that each direct and sub-dependency of the selected type must use the given service as dependency.")),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"Custom"),": with this, you can build your own selection logic based on the passed contextual type information.")))),(0,a.kt)("div",null,(0,a.kt)(i.Z,{mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Attribute",label:"Attribute",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class ConsoleAttribute : Attribute { }\n\nclass DbBackup : IJob\n{\n    public DbBackup([Console]ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // resolve only when the injected parameter, \n    // property or field has the 'Console' attribute\n    .WhenHas<ConsoleAttribute>());\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n"))),(0,a.kt)(l.Z,{value:"Parent",label:"Parent",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // inject only when we are \n    // currently resolving DbBackup OR StorageCleanup.\n    .WhenDependantIs<DbBackup>()\n    .WhenDependantIs<StorageCleanup>());\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n"))),(0,a.kt)(l.Z,{value:"Path",label:"Path",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob\n{\n    public DbBackup(IStorage storage)\n    { }\n}\n\nclass FileStorage : IStorage\n{\n    public FileStorage(ILogger logger) \n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // inject only when we are in the\n    // resolution path of DbBackup\n    .WhenInResolutionPathOf<DbBackup>());\n\ncontainer.Register<IStorage, FileStorage>();\ncontainer.Register<IJob, DbBackup>();\n\n// the container will select ConsoleLogger for FileStorage\n// because they are injected into DbBackup.\nIJob job = container.Resolve<IJob>();\n"))),(0,a.kt)(l.Z,{value:"Custom",label:"Custom",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    // inject only when we are \n    // currently resolving DbBackup.\n    .When(typeInfo => typeInfo.ParentType.Equals(typeof(DbBackup))));\n\ncontainer.Register<IJob, DbBackup>();\n\n// the container will resolve DbBackup with ConsoleLogger.\nIJob job = container.Resolve<IJob>();\n"))),(0,a.kt)(l.Z,{value:"Collection",label:"Collection",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbJobsExecutor : IJobsExecutor\n{\n    public DbBackup(IEnumerable<IJob> jobs)\n    { }\n}\n\ncontainer.Register<IJob, DbBackup>(options => options\n    .WhenDependantIs<DbJobsExecutor>());\ncontainer.Register<IJob, DbCleanup>(options => options\n    .WhenDependantIs<DbJobsExecutor>());\nontainer.Register<IJob, StorageCleanup>();\n\ncontainer.Register<IJobsExecutor, DbJobsExecutor>();\n\n// jobsExecutor will get DbBackup and DbCleanup within a collection.\nIJobsExecutor jobsExecutor = container.Resolve<IJobsExecutor>();\n")))))),(0,a.kt)("p",null,"The specified conditions are behaving like filters when a ",(0,a.kt)("strong",{parentName:"p"},"collection")," is requested."),(0,a.kt)("p",null,"When you use the same conditional option multiple times, the container will evaluate them ",(0,a.kt)("strong",{parentName:"p"},"combined with OR")," logical operator."),(0,a.kt)("admonition",{type:"tip"},(0,a.kt)("p",{parentName:"admonition"},(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/registration-configuration#conditions"},"Here")," you can find each condition related registration option.")),(0,a.kt)("h2",{id:"optional-resolution"},"Optional resolution"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"In cases where it's not guaranteed that a service is resolvable, either because it's not registered or any of its dependencies are missing, you can attempt an optional resolution by using the ",(0,a.kt)("inlineCode",{parentName:"p"},"ResolveOrDefault()")," method. "),(0,a.kt)("p",null,"In this case, the resolution request will return with ",(0,a.kt)("inlineCode",{parentName:"p"},"null")," (or ",(0,a.kt)("inlineCode",{parentName:"p"},"default")," in case of type values) when the attempt fails.")),(0,a.kt)("div",null,(0,a.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"// returns null when the resolution fails.\nIJob job = container.ResolveOrDefault<IJob>();\n\n// throws ResolutionFailedException when the resolution fails.\nIJob job = container.Resolve<IJob>();\n"))),(0,a.kt)(l.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"// returns null when the resolution fails.\nobject job = container.ResolveOrDefault(typeof(IJob));\n\n// throws ResolutionFailedException when the resolution fails.\nobject job = container.Resolve(typeof(IJob));\n")))))),(0,a.kt)("h2",{id:"dependency-overrides"},"Dependency overrides"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"At resolution time, you have the option to override the dependencies of your resolved service by passing them as an ",(0,a.kt)("inlineCode",{parentName:"p"},"object[]")," to the ",(0,a.kt)("inlineCode",{parentName:"p"},"Resolve()")," method."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob\n{\n    public DbBackup(ILogger logger)\n    { }\n}\n"))),(0,a.kt)("div",null,(0,a.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"DbBackup backup = container.Resolve<DbBackup>( \n    dependencyOverrides: new object[] \n    { \n        new ConsoleLogger() \n    });\n"))),(0,a.kt)(l.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"object backup = container.Resolve(typeof(DbBackup),\n    dependencyOverrides: new object[] \n    { \n        new ConsoleLogger() \n    });\n")))))),(0,a.kt)("h2",{id:"activation"},"Activation"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"When you only want to build up an instance from a type on the fly without a registration, you can use the container's ",(0,a.kt)("inlineCode",{parentName:"p"},".Activate()")," method. "),(0,a.kt)("p",null,"It also allows dependency overriding with ",(0,a.kt)("inlineCode",{parentName:"p"},"object")," arguments and performs member injection on the created instance (when configured)."),(0,a.kt)("p",null,"It works like ",(0,a.kt)("inlineCode",{parentName:"p"},"Activator.CreateInstance()")," except that Stashbox supplies the dependencies.")),(0,a.kt)("div",null,(0,a.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,a.kt)(l.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"// use dependency injected by container.\nDbBackup backup = container.Activate<DbBackup>();\n\n// override the injected dependency.\nDbBackup backup = container.Activate<DbBackup>(new ConsoleLogger());\n"))),(0,a.kt)(l.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"// use dependency injected by container.\nobject backup = container.Activate(typeof(DbBackup));\n\n// override the injected dependency.\nobject backup = container.Activate(typeof(DbBackup), new ConsoleLogger());\n")))))),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"build-up"},"Build-up"),(0,a.kt)("p",null,"You can also do the same ",(0,a.kt)("em",{parentName:"p"},"on the fly")," activation post-processing (member/method injection) on already constructed instances with the ",(0,a.kt)("inlineCode",{parentName:"p"},".BuildUp()")," method. "),(0,a.kt)("admonition",{type:"caution"},(0,a.kt)("p",{parentName:"admonition"},(0,a.kt)("inlineCode",{parentName:"p"},".BuildUp()")," won't register the given instance into the container."))),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob\n{\n    public ILogger Logger { get; set; }\n}\n\nDbBackup backup = new DbBackup();\n// the container fills the Logger property.\ncontainer.BuildUp(backup); \n")))))}b.isMDXComponent=!0}}]);