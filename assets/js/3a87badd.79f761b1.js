"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[80],{3905:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>g});var a=n(7294);function o(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function r(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?r(Object(n),!0).forEach((function(t){o(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):r(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,a,o=function(e,t){if(null==e)return{};var n,a,o={},r=Object.keys(e);for(a=0;a<r.length;a++)n=r[a],t.indexOf(n)>=0||(o[n]=e[n]);return o}(e,t);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);for(a=0;a<r.length;a++)n=r[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(o[n]=e[n])}return o}var l=a.createContext({}),c=function(e){var t=a.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},p=function(e){var t=c(e.components);return a.createElement(l.Provider,{value:t},e.children)},u="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},m=a.forwardRef((function(e,t){var n=e.components,o=e.mdxType,r=e.originalType,l=e.parentName,p=s(e,["components","mdxType","originalType","parentName"]),u=c(n),m=o,g=u["".concat(l,".").concat(m)]||u[m]||d[m]||r;return n?a.createElement(g,i(i({ref:t},p),{},{components:n})):a.createElement(g,i({ref:t},p))}));function g(e,t){var n=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var r=n.length,i=new Array(r);i[0]=m;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s[u]="string"==typeof e?e:o,i[1]=s;for(var c=2;c<r;c++)i[c]=n[c];return a.createElement.apply(null,i)}return a.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>i});var a=n(7294),o=n(6010);const r="tabItem_Ymn6";function i(e){let{children:t,hidden:n,className:i}=e;return a.createElement("div",{role:"tabpanel",className:(0,o.Z)(r,i),hidden:n},t)}},4866:(e,t,n)=>{n.d(t,{Z:()=>N});var a=n(7462),o=n(7294),r=n(6010),i=n(2466),s=n(6550),l=n(1980),c=n(7392),p=n(12);function u(e){return function(e){return o.Children.map(e,(e=>{if(!e||(0,o.isValidElement)(e)&&function(e){const{props:t}=e;return!!t&&"object"==typeof t&&"value"in t}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}(e).map((e=>{let{props:{value:t,label:n,attributes:a,default:o}}=e;return{value:t,label:n,attributes:a,default:o}}))}function d(e){const{values:t,children:n}=e;return(0,o.useMemo)((()=>{const e=t??u(n);return function(e){const t=(0,c.l)(e,((e,t)=>e.value===t.value));if(t.length>0)throw new Error(`Docusaurus error: Duplicate values "${t.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[t,n])}function m(e){let{value:t,tabValues:n}=e;return n.some((e=>e.value===t))}function g(e){let{queryString:t=!1,groupId:n}=e;const a=(0,s.k6)(),r=function(e){let{queryString:t=!1,groupId:n}=e;if("string"==typeof t)return t;if(!1===t)return null;if(!0===t&&!n)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return n??null}({queryString:t,groupId:n});return[(0,l._X)(r),(0,o.useCallback)((e=>{if(!r)return;const t=new URLSearchParams(a.location.search);t.set(r,e),a.replace({...a.location,search:t.toString()})}),[r,a])]}function b(e){const{defaultValue:t,queryString:n=!1,groupId:a}=e,r=d(e),[i,s]=(0,o.useState)((()=>function(e){let{defaultValue:t,tabValues:n}=e;if(0===n.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(t){if(!m({value:t,tabValues:n}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${t}" but none of its children has the corresponding value. Available values are: ${n.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return t}const a=n.find((e=>e.default))??n[0];if(!a)throw new Error("Unexpected error: 0 tabValues");return a.value}({defaultValue:t,tabValues:r}))),[l,c]=g({queryString:n,groupId:a}),[u,b]=function(e){let{groupId:t}=e;const n=function(e){return e?`docusaurus.tab.${e}`:null}(t),[a,r]=(0,p.Nk)(n);return[a,(0,o.useCallback)((e=>{n&&r.set(e)}),[n,r])]}({groupId:a}),v=(()=>{const e=l??u;return m({value:e,tabValues:r})?e:null})();(0,o.useLayoutEffect)((()=>{v&&s(v)}),[v]);return{selectedValue:i,selectValue:(0,o.useCallback)((e=>{if(!m({value:e,tabValues:r}))throw new Error(`Can't select invalid tab value=${e}`);s(e),c(e),b(e)}),[c,b,r]),tabValues:r}}var v=n(2389);const h="tabList__CuJ",k="tabItem_LNqP";function y(e){let{className:t,block:n,selectedValue:s,selectValue:l,tabValues:c}=e;const p=[],{blockElementScrollPositionUntilNextRender:u}=(0,i.o5)(),d=e=>{const t=e.currentTarget,n=p.indexOf(t),a=c[n].value;a!==s&&(u(t),l(a))},m=e=>{let t=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const n=p.indexOf(e.currentTarget)+1;t=p[n]??p[0];break}case"ArrowLeft":{const n=p.indexOf(e.currentTarget)-1;t=p[n]??p[p.length-1];break}}t?.focus()};return o.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,r.Z)("tabs",{"tabs--block":n},t)},c.map((e=>{let{value:t,label:n,attributes:i}=e;return o.createElement("li",(0,a.Z)({role:"tab",tabIndex:s===t?0:-1,"aria-selected":s===t,key:t,ref:e=>p.push(e),onKeyDown:m,onClick:d},i,{className:(0,r.Z)("tabs__item",k,i?.className,{"tabs__item--active":s===t})}),n??t)})))}function f(e){let{lazy:t,children:n,selectedValue:a}=e;const r=(Array.isArray(n)?n:[n]).filter(Boolean);if(t){const e=r.find((e=>e.props.value===a));return e?(0,o.cloneElement)(e,{className:"margin-top--md"}):null}return o.createElement("div",{className:"margin-top--md"},r.map(((e,t)=>(0,o.cloneElement)(e,{key:t,hidden:e.props.value!==a}))))}function I(e){const t=b(e);return o.createElement("div",{className:(0,r.Z)("tabs-container",h)},o.createElement(y,(0,a.Z)({},e,t)),o.createElement(f,(0,a.Z)({},e,t)))}function N(e){const t=(0,v.Z)();return o.createElement(I,(0,a.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>s});var a=n(7294);const o="codeDescContainer_ie8f",r="desc_jyqI",i="example_eYlF";function s(e){let{children:t}=e,n=a.Children.toArray(t).filter((e=>e));return a.createElement("div",{className:o},a.createElement("div",{className:r},n[0]),a.createElement("div",{className:i},n[1]))}},9470:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>u,contentTitle:()=>c,default:()=>g,frontMatter:()=>l,metadata:()=>p,toc:()=>d});var a=n(7462),o=(n(7294),n(3905)),r=n(8846),i=n(4866),s=n(5162);const l={},c="Advanced registration",p={unversionedId:"guides/advanced-registration",id:"guides/advanced-registration",title:"Advanced registration",description:"This section is about Stashbox's further configuration options, including the registration configuration API, the registration of factory delegates, multiple implementations, batch registrations, the concept of the Composition Root, and many more.",source:"@site/docs/guides/advanced-registration.md",sourceDirName:"guides",slug:"/guides/advanced-registration",permalink:"/stashbox/docs/guides/advanced-registration",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/advanced-registration.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1690384637,formattedLastUpdatedAt:"Jul 26, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Basic usage",permalink:"/stashbox/docs/guides/basics"},next:{title:"Service resolution",permalink:"/stashbox/docs/guides/service-resolution"}},u={},d=[{value:"Factory registration",id:"factory-registration",level:2},{value:"Factories with parameter overrides",id:"factories-with-parameter-overrides",level:3},{value:"Consider this before using the resolver parameter inside a factory",id:"consider-this-before-using-the-resolver-parameter-inside-a-factory",level:3},{value:"Delegates with dependencies passed as parameters",id:"delegates-with-dependencies-passed-as-parameters",level:4},{value:"Accessing the currently resolving type in factories",id:"accessing-the-currently-resolving-type-in-factories",level:3},{value:"Multiple implementations",id:"multiple-implementations",level:2},{value:"Binding to multiple services",id:"binding-to-multiple-services",level:2},{value:"Batch registration",id:"batch-registration",level:2},{value:"Assembly registration",id:"assembly-registration",level:2},{value:"Composition root",id:"composition-root",level:2},{value:"Injection parameters",id:"injection-parameters",level:2},{value:"Initializer / finalizer",id:"initializer--finalizer",level:2}],m={toc:d};function g(e){let{components:t,...n}=e;return(0,o.kt)("wrapper",(0,a.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"advanced-registration"},"Advanced registration"),(0,o.kt)("p",null,"This section is about Stashbox's further configuration options, including the registration configuration API, the registration of factory delegates, multiple implementations, batch registrations, the concept of the ",(0,o.kt)("a",{parentName:"p",href:"https://blog.ploeh.dk/2011/07/28/CompositionRoot/"},"Composition Root"),", and many more."),(0,o.kt)("admonition",{type:"info"},(0,o.kt)("p",{parentName:"admonition"},"This section won't cover all the available options of the registrations API, but you can find them ",(0,o.kt)("a",{parentName:"p",href:"/docs/configuration/registration-configuration"},"here"),".")),(0,o.kt)("h2",{id:"factory-registration"},"Factory registration"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"You can bind a factory delegate to a registration that the container will invoke directly to instantiate your service. "),(0,o.kt)("p",null,"You can use parameter-less and custom parameterized delegates as a factory. ",(0,o.kt)("a",{parentName:"p",href:"/docs/configuration/registration-configuration#factory"},"Here")," is the list of all available options."),(0,o.kt)("p",null,"You can also get the current ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#dependency-resolver"},"dependency resolver")," as a delegate parameter to resolve any additional dependencies required for the service construction.")),(0,o.kt)("div",null,(0,o.kt)(i.Z,{mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Parameter-less",label:"Parameter-less",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<ILogger, ConsoleLogger>(options => options\n    .WithFactory(() => new ConsoleLogger());\n\n// the container uses the factory for instantiation.\nIJob job = container.Resolve<ILogger>();\n"))),(0,o.kt)(s.Z,{value:"Parameterized",label:"Parameterized",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithFactory<ILogger>(logger => new DbBackup(logger));\n\n// the container uses the factory for instantiation.\nIJob job = container.Resolve<IJob>();\n"))),(0,o.kt)(s.Z,{value:"Resolver parameter",label:"Resolver parameter",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithFactory(resolver => new DbBackup(resolver.Resolve<ILogger>()));\n    \n// the container uses the factory for instantiation.\nIJob job = container.Resolve<IJob>();\n")))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"Delegate factories are useful when your service's instantiation is not straight-forward for the container, like when it depends on something that is not available at resolution time. E.g., a connection string.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>(options => options\n    .WithFactory<ILogger>(logger => \n        new DbBackup(Configuration["DbConnectionString"], logger));\n')))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"factories-with-parameter-overrides"},"Factories with parameter overrides"),(0,o.kt)("p",null,"Stashbox can implicitly ",(0,o.kt)("a",{parentName:"p",href:"/docs/advanced/wrappers-resolvers#delegate"},"wrap")," your service in a ",(0,o.kt)("inlineCode",{parentName:"p"},"Delegate")," and lets you pass parameters that can override your service's dependencies. Moreover, you can register your own custom delegate that the container will resolve when you request your service wrapped in a ",(0,o.kt)("inlineCode",{parentName:"p"},"Delegate"),".")),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.RegisterFunc<string, IJob>((connectionString, resolver) => \n    new DbBackup(connectionString, resolver.Resolve<ILogger>()));\n\nFunc<string, IJob> backupFactory = container.Resolve<Func<string, IJob>>();\nIJob dbBackup = backupFactory(Configuration["ConnectionString"]);\n'))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.RegisterFunc<string, IJob>((connectionString, resolver) => \n    new DbBackup(connectionString, resolver.Resolve<ILogger>()));\n\nDelegate backupFactory = container.ResolveFactory(typeof(IJob), \n    parameterTypes: new[] { typeof(string) });\nIJob dbBackup = backupFactory.DynamicInvoke(Configuration["ConnectionString"]);\n')))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"If a service has multiple constructors, the container visits those first, that has matching parameters passed to the factory, with respecting the additional ",(0,o.kt)("a",{parentName:"p",href:"/docs/configuration/registration-configuration#constructor-selection"},"constructor selection rules"),".")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"class Service\n{\n    public Service(int number) { }\n    public Service(string text) { }\n}\n\ncontainer.Register<Service>();\n\n// create the factory with an int input parameter.\nvar func = constainer.Resolve<Func<int, Service>>();\n\n// the constructor with the int param \n// is used for instantiation.\nvar service = func(2);\n")))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"consider-this-before-using-the-resolver-parameter-inside-a-factory"},"Consider this before using the resolver parameter inside a factory"),(0,o.kt)("p",null,"Delegate factories are a black-box for the container. It doesn't have control over what's happening inside a delegate, which means when you resolve additional dependencies with the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#dependency-resolver"},"dependency resolver")," parameter, they could easily bypass the ",(0,o.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#lifetime-validation"},"lifetime")," and ",(0,o.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#circular-dependency"},"circular dependency")," validations. Fortunately, you have the option to keep them validated anyway with parameterized factory delegates."),(0,o.kt)("h4",{id:"delegates-with-dependencies-passed-as-parameters"},"Delegates with dependencies passed as parameters"),(0,o.kt)("p",null,"Rather than using the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#dependency-resolver"},"dependency resolver")," parameter inside the factory, let the container inject the dependencies into the delegate as parameters. This way, the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#resolution-tree"},"resolution tree's")," integrity remains stable because no service resolution happens inside the black-box, and each parameter is validated.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"interface IEventProcessor { }\n\nclass EventProcessor : IEventProcessor\n{\n    public EventProcessor(ILogger logger, IEventValidator validator)\n    { }\n}\n\ncontainer.Register<ILogger, ConsoleLogger>();\ncontainer.Register<IEventValidator, EventValidator>();\n\ncontainer.Register<IEventProcessor, EventProcessor>(options => options\n    // Ilogger and IEventValidator instances are injected\n    // by the container at resolution time, so they will be\n    // validated against circular and captive dependencies.\n    .WithFactory<ILogger, IEventValidator>((logger, validator) => \n        new EventProcessor(logger, validator));\n\n// the container resolves ILogger and IEventValidator first, then\n// it passes them to the factory as delegate parameters.\nIEventProcessor processor = container.Resolve<IEventProcessor>();\n")))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"accessing-the-currently-resolving-type-in-factories"},"Accessing the currently resolving type in factories"),(0,o.kt)("p",null,"To access the currently resolving type in factory delegates, you can set the ",(0,o.kt)("inlineCode",{parentName:"p"},"TypeInformation")," type as an input parameter of the factory.\nThe ",(0,o.kt)("inlineCode",{parentName:"p"},"TypeInformation")," holds every reflected context information about the currently resolving type. "),(0,o.kt)("p",null,"This can be useful when the resolution is, e.g., in an open generic context, and we want to know which closed generic variant is requested.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"interface IService<T> { }\n\nclass Service<T> : IService<T> { }\n\ncontainer.Register(typeof(IService<>), typeof(Service<>), options => \n    options.WithFactory<TypeInformation>(typeInfo => \n    {\n        // typeInfo.Type here holds the actual type like\n        // IService<int> based on the resolution request below.\n    }));\n    \ncontainer.Resolve<IService<int>>();\n")))),(0,o.kt)("h2",{id:"multiple-implementations"},"Multiple implementations"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"As we previously saw in the ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/basics#named-registration"},"Named registration")," topic, Stashbox allows you to have multiple implementations bound to a particular ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),". You can use names to distinguish them, but you can also access them by requesting a typed collection using the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),"."),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"The returned collection is in the same order as the services were registered.\nAlso, to request a collection, you can use any interface implemented by an array."))),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\ncontainer.Register<IJob, StorageCleanup>();\ncontainer.Register<IJob, ImageProcess>();\n")),(0,o.kt)(i.Z,{mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"ResolveAll",label:"ResolveAll",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// jobs contain all three services in registration order.\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n"))),(0,o.kt)(s.Z,{value:"Array",label:"Array",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// jobs contain all three services in registration order.\nIJob[] jobs = container.Resolve<IJob[]>();\n"))),(0,o.kt)(s.Z,{value:"IEnumerable",label:"IEnumerable",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// jobs contain all three services in registration order.\nIEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>();\n"))),(0,o.kt)(s.Z,{value:"IList",label:"IList",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// jobs contain all three services in registration order.\nIList<IJob> jobs = container.Resolve<IList<IJob>>();\n"))),(0,o.kt)(s.Z,{value:"ICollection",label:"ICollection",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// jobs contain all three services in registration order.\nICollection<IJob> jobs = container.Resolve<ICollection<IJob>>();\n")))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"When you have multiple implementations registered to a service, a request to the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," without a name will return the ",(0,o.kt)("strong",{parentName:"p"},"last registered implementation"),"."),(0,o.kt)("admonition",{type:"info"},(0,o.kt)("p",{parentName:"admonition"},"Not only names can be used to distinguish registrations, ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#conditional-resolution"},"conditions"),", ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/scopes#named-scopes"},"named scopes"),", and ",(0,o.kt)("a",{parentName:"p",href:"/docs/advanced/wrappers-resolvers#metadata--tuple"},"metadata")," can also influence the results."))),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\ncontainer.Register<IJob, StorageCleanup>();\ncontainer.Register<IJob, ImageProcess>();\n\n// job will be the ImageProcess.\nIJob job = container.Resolve<IJob>();\n")))),(0,o.kt)("h2",{id:"binding-to-multiple-services"},"Binding to multiple services"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"When you have a service that implements multiple interfaces, you have the option to bind its registration to all or some of those additional interfaces or base types."),(0,o.kt)("p",null,"Suppose we have the following class declaration:"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"class DbBackup : IJob, IScheduledJob\n{ \n    public DbBackup() { }\n}\n"))),(0,o.kt)("div",null,(0,o.kt)(i.Z,{mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"To another type",label:"To another type",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .AsServiceAlso<IScheduledJob>());\n\nIJob job = container.Resolve<IJob>(); // DbBackup\nIScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup\nDbBackup job = container.Resolve<DbBackup>(); // error, not found\n"))),(0,o.kt)(s.Z,{value:"To all implemented types",label:"To all implemented types",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<DbBackup>(options => options\n    .AsImplementedTypes());\n\nIJob job = container.Resolve<IJob>(); // DbBackup\nIScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup\nDbBackup job = container.Resolve<DbBackup>(); // DbBackup\n")))))),(0,o.kt)("h2",{id:"batch-registration"},"Batch registration"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"You have the option to register multiple services in a single registration operation. "),(0,o.kt)("p",null,(0,o.kt)("strong",{parentName:"p"},"Filters (optional):"),"\nFirst, the container will use the ",(0,o.kt)("em",{parentName:"p"},"implementation filter")," action to select only those types from the collection we want to register. When we have those, the container will execute the ",(0,o.kt)("em",{parentName:"p"},"service filter")," on their implemented interfaces and base classes to select which ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," they should be mapped to."),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"Framework types like ",(0,o.kt)("inlineCode",{parentName:"p"},"IDisposable")," are excluded from being considered as a ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," by default.")),(0,o.kt)("admonition",{type:"tip"},(0,o.kt)("p",{parentName:"admonition"},"You can use the registration configuration API to configure individual registrations."))),(0,o.kt)("div",null,(0,o.kt)(i.Z,{mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Default",label:"Default",mdxType:"TabItem"},(0,o.kt)("p",null,"This example will register three types to all their implemented interfaces, extended base classes, and to themselves (",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#self-registration"},"self registration"),") without any filter:"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterTypes(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup) \n    });\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nIJob job = container.Resolve<IJob>(); // StorageCleanup\nDbBackup backup = container.Resolve<DbBackup>(); // DbBackup\n"))),(0,o.kt)(s.Z,{value:"Filters",label:"Filters",mdxType:"TabItem"},(0,o.kt)("p",null,"In this example, we assume that ",(0,o.kt)("inlineCode",{parentName:"p"},"DbBackup")," and ",(0,o.kt)("inlineCode",{parentName:"p"},"StorageCleanup")," are implementing ",(0,o.kt)("inlineCode",{parentName:"p"},"IDisposable")," besides ",(0,o.kt)("inlineCode",{parentName:"p"},"IJob")," and also extending a ",(0,o.kt)("inlineCode",{parentName:"p"},"JobBase")," abstract class."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterTypes(new[] \n    { typeof(DbBackup), typeof(ConsoleLogger), typeof(StorageCleanup) },\n    // implementation filter, only those implementations that implements IDisposable\n    impl => typeof(IDisposable).IsAssignableFrom(impl),\n    // service filter, register them to base classes only\n    (impl, service) => service.IsAbstract && !service.IsInterface);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 0 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // error, not found\nDbBackup backup = container.Resolve<DbBackup>(); // DbBackup\n"))),(0,o.kt)(s.Z,{value:"Without self",label:"Without self",mdxType:"TabItem"},(0,o.kt)("p",null,"This example ignores the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#self-registration"},"self registrations")," completely:"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterTypes(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup)\n    },\n    registerSelf: false);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\nConsoleLogger logger = container.Resolve<ConsoleLogger>(); // error, not found\n"))),(0,o.kt)(s.Z,{value:"Registration options",label:"Registration options",mdxType:"TabItem"},(0,o.kt)("p",null,"This example will configure all registrations mapped to ",(0,o.kt)("inlineCode",{parentName:"p"},"ILogger")," as ",(0,o.kt)("inlineCode",{parentName:"p"},"Singleton"),":"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterTypes(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup)\n    },\n    configurator: options => \n    {\n        if (options.HasServiceType<ILogger>())\n            options.WithSingletonLifetime();\n    });\n\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nILogger newLogger = container.Resolve<ILogger>(); // the same ConsoleLogger\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\n")))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"Another type of service filter is the ",(0,o.kt)("inlineCode",{parentName:"p"},".RegisterTypesAs<T>()")," method, which registers only those types that implements the ",(0,o.kt)("inlineCode",{parentName:"p"},"T")," ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),"."),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"This method also accepts an implementation filter and a registration configurator action like ",(0,o.kt)("inlineCode",{parentName:"p"},".RegisterTypes()"),".")),(0,o.kt)("admonition",{type:"caution"},(0,o.kt)("p",{parentName:"admonition"},(0,o.kt)("inlineCode",{parentName:"p"},".RegisterTypesAs<T>()")," doesn't create ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#self-registration"},"self registrations")," as it only maps the implementations to the given ",(0,o.kt)("inlineCode",{parentName:"p"},"T")," ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),"."))),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterTypesAs<IJob>(new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup) \n    });\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // error, not found\nIJob job = container.Resolve<IJob>(); // StorageCleanup\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterTypesAs(typeof(IJob), new[] \n    { \n        typeof(DbBackup), \n        typeof(ConsoleLogger), \n        typeof(StorageCleanup) \n    });\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nILogger logger = container.Resolve<ILogger>(); // error, not found\nIJob job = container.Resolve<IJob>(); // StorageCleanup\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n")))))),(0,o.kt)("h2",{id:"assembly-registration"},"Assembly registration"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The batch registration API ",(0,o.kt)("em",{parentName:"p"},"(filters, registration configuration action, self-registration)")," is also usable for registering services from given assemblies."),(0,o.kt)("p",null,"In this example, we assume that the same three services we used in the ",(0,o.kt)("a",{parentName:"p",href:"#batch-registration"},"batch registration")," section are in the same assembly."),(0,o.kt)("admonition",{type:"info"},(0,o.kt)("p",{parentName:"admonition"},"The container also detects and registers open-generic definitions (when applicable) from the supplied type collection. You can read about ",(0,o.kt)("a",{parentName:"p",href:"/docs/advanced/generics#open-generics"},"open-generics here"),"."))),(0,o.kt)("div",null,(0,o.kt)(i.Z,{mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Single assembly",label:"Single assembly",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterAssembly(typeof(DbBackup).Assembly,\n    // service filter, register to interfaces only\n    serviceTypeSelector: (impl, service) => service.IsInterface,\n    registerSelf: false,\n    configurator: options => options.WithoutDisposalTracking()\n);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"))),(0,o.kt)(s.Z,{value:"Multiple assemblies",label:"Multiple assemblies",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterAssemblies(new[] \n    { \n        typeof(DbBackup).Assembly, \n        typeof(JobFromAnotherAssembly).Assembly \n    },\n    // service filter, register to interfaces only\n    serviceTypeSelector: (impl, service) => service.IsInterface,\n    registerSelf: false,\n    configurator: options => options.WithoutDisposalTracking()\n);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n"))),(0,o.kt)(s.Z,{value:"Containing type",label:"Containing type",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterAssemblyContaining<DbBackup>(\n    // service filter, register to interfaces only\n    serviceTypeSelector: (impl, service) => service.IsInterface,\n    registerSelf: false,\n    configurator: options => options.WithoutDisposalTracking()\n);\n\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items\nIEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items\nILogger logger = container.Resolve<ILogger>(); // ConsoleLogger\nDbBackup backup = container.Resolve<DbBackup>(); // error, not found\n")))))),(0,o.kt)("h2",{id:"composition-root"},"Composition root"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The ",(0,o.kt)("a",{parentName:"p",href:"https://blog.ploeh.dk/2011/07/28/CompositionRoot/"},"Composition Root")," is an entry point where all services required to make a component functional are wired together."),(0,o.kt)("p",null,"Stashbox provides an ",(0,o.kt)("inlineCode",{parentName:"p"},"ICompositionRoot")," interface that can be used to define an entry point for a given component or even for an entire assembly. "),(0,o.kt)("p",null,"You can wire up your ",(0,o.kt)("em",{parentName:"p"},"composition root")," implementation with ",(0,o.kt)("inlineCode",{parentName:"p"},"ComposeBy<TRoot>()"),", or you can let the container find and execute all available ",(0,o.kt)("em",{parentName:"p"},"composition root")," implementations within an assembly."),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"Your ",(0,o.kt)("inlineCode",{parentName:"p"},"ICompositionRoot")," implementation also can have dependencies that the container will resolve."))),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"class ExampleRoot : ICompositionRoot\n{\n    public ExampleRoot(IDependency rootDependency)\n    { }\n\n    public void Compose(IStashboxContainer container)\n    {\n       container.Register<IServiceA, ServiceA>();\n       container.Register<IServiceB, ServiceB>();\n    }\n}\n")),(0,o.kt)(i.Z,{mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Single",label:"Single",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// compose a single root.\ncontainer.ComposeBy<ExampleRoot>();\n"))),(0,o.kt)(s.Z,{value:"Assembly",label:"Assembly",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// compose every root in the given assembly.\ncontainer.ComposeAssembly(typeof(IServiceA).Assembly);\n"))),(0,o.kt)(s.Z,{value:"Override",label:"Override",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"// compose a single root with dependency override.\ncontainer.ComposeBy<ExampleRoot>(new CustomRootDependency());\n")))))),(0,o.kt)("h2",{id:"injection-parameters"},"Injection parameters"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"If you have pre-evaluated dependencies you'd like to inject at resolution time, you can set them as injection parameters during registration. "),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"Injection parameter names are matched to constructor arguments or field/property names."))),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>(options => options\n    .WithInjectionParameter("logger", new ConsoleLogger())\n    .WithInjectionParameter("eventBroadcaster", new MessageBus());\n\n// the injection parameters will be passed to DbBackup\'s constructor.\nIJob backup = container.Resolve<IJob>();\n')))),(0,o.kt)("h2",{id:"initializer--finalizer"},"Initializer / finalizer"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The container provides specific extension points to let you react to lifetime events of an instantiated service. "),(0,o.kt)("p",null,"For this reason, you can specify ",(0,o.kt)("em",{parentName:"p"},"Initializer")," and ",(0,o.kt)("em",{parentName:"p"},"Finalizer")," delegates. The ",(0,o.kt)("em",{parentName:"p"},"finalizer")," is called upon the service's ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/scopes#disposal"},"disposal"),", and the ",(0,o.kt)("em",{parentName:"p"},"initializer")," is called upon the service's construction.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<ILogger, FileLogger>(options => options\n    // delegate that called right after instantiation.\n    .WithInitializer((logger, resolver) => logger.OpenFile())\n    // delegate that called right before the instance's disposal.\n    .WithFinalizer(logger => logger.CloseFile()));\n")))))}g.isMDXComponent=!0}}]);