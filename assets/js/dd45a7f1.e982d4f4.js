"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[652],{3905:(e,t,n)=>{n.d(t,{Zo:()=>c,kt:()=>h});var r=n(7294);function a(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function i(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function s(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?i(Object(n),!0).forEach((function(t){a(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):i(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function o(e,t){if(null==e)return{};var n,r,a=function(e,t){if(null==e)return{};var n,r,a={},i=Object.keys(e);for(r=0;r<i.length;r++)n=i[r],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(r=0;r<i.length;r++)n=i[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var l=r.createContext({}),p=function(e){var t=r.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):s(s({},t),e)),n},c=function(e){var t=p(e.components);return r.createElement(l.Provider,{value:t},e.children)},d="mdxType",m={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},u=r.forwardRef((function(e,t){var n=e.components,a=e.mdxType,i=e.originalType,l=e.parentName,c=o(e,["components","mdxType","originalType","parentName"]),d=p(n),u=a,h=d["".concat(l,".").concat(u)]||d[u]||m[u]||i;return n?r.createElement(h,s(s({ref:t},c),{},{components:n})):r.createElement(h,s({ref:t},c))}));function h(e,t){var n=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var i=n.length,s=new Array(i);s[0]=u;var o={};for(var l in t)hasOwnProperty.call(t,l)&&(o[l]=t[l]);o.originalType=e,o[d]="string"==typeof e?e:a,s[1]=o;for(var p=2;p<i;p++)s[p]=n[p];return r.createElement.apply(null,s)}return r.createElement.apply(null,n)}u.displayName="MDXCreateElement"},8846:(e,t,n)=>{n.d(t,{Z:()=>o});var r=n(7294);const a="codeDescContainer_ie8f",i="desc_jyqI",s="example_eYlF";function o(e){let{children:t}=e,n=r.Children.toArray(t).filter((e=>e));return r.createElement("div",{className:a},r.createElement("div",{className:i},n[0]),r.createElement("div",{className:s},n[1]))}},969:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>p,contentTitle:()=>o,default:()=>m,frontMatter:()=>s,metadata:()=>l,toc:()=>c});var r=n(7462),a=(n(7294),n(3905)),i=n(8846);const s={},o="Glossary",l={unversionedId:"getting-started/glossary",id:"getting-started/glossary",title:"Glossary",description:"The following terms and definitions are used in this documentation.",source:"@site/docs/getting-started/glossary.md",sourceDirName:"getting-started",slug:"/getting-started/glossary",permalink:"/stashbox/docs/getting-started/glossary",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/getting-started/glossary.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1675972034,formattedLastUpdatedAt:"Feb 9, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Introduction",permalink:"/stashbox/docs/getting-started/introduction"},next:{title:"Basic usage",permalink:"/stashbox/docs/guides/basics"}},p={},c=[{value:"Service type | Implementation type",id:"service-type--implementation-type",level:2},{value:"Service registration | Registered service",id:"service-registration--registered-service",level:2},{value:"Injectable dependency",id:"injectable-dependency",level:2},{value:"Resolution tree",id:"resolution-tree",level:2},{value:"Dependency resolver",id:"dependency-resolver",level:2},{value:"Root scope",id:"root-scope",level:2},{value:"Named resolution",id:"named-resolution",level:2},{value:"Self registration",id:"self-registration",level:2}],d={toc:c};function m(e){let{components:t,...n}=e;return(0,a.kt)("wrapper",(0,r.Z)({},d,n,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"glossary"},"Glossary"),(0,a.kt)("p",null,"The following terms and definitions are used in this documentation."),(0,a.kt)("h2",{id:"service-type--implementation-type"},"Service type | Implementation type"),(0,a.kt)("p",null,"The ",(0,a.kt)("em",{parentName:"p"},"Service type")," is usually an interface or an abstract class type used for service resolution or dependency injection. The ",(0,a.kt)("em",{parentName:"p"},"Implementation type")," is the actual type registered to the ",(0,a.kt)("em",{parentName:"p"},"Service type"),". A registration maps the ",(0,a.kt)("em",{parentName:"p"},"Service type")," to an ",(0,a.kt)("em",{parentName:"p"},"Implementation type"),". The ",(0,a.kt)("em",{parentName:"p"},"Implementation type")," must implement or extend the ",(0,a.kt)("em",{parentName:"p"},"Service type"),". "),(0,a.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"Example where a ",(0,a.kt)("em",{parentName:"p"},"Service type")," is mapped to an ",(0,a.kt)("em",{parentName:"p"},"Implementation type"),":")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IService, Implementation>();\n")))),(0,a.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"The ",(0,a.kt)("em",{parentName:"p"},"Service type")," used for requesting a service from the container:")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Resolve<IService>(); // returns Implementation\n")))),(0,a.kt)("h2",{id:"service-registration--registered-service"},"Service registration | Registered service"),(0,a.kt)("p",null,"It's an entity created by Stashbox when a service is registered. The service registration stores required information about how to instantiate the service, e.g., reflected type information, name, lifetime, conditions, and more."),(0,a.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"In this example, we are registering a named service. The container will create a service registration entity to store the type mapping and the name. During resolution, the container will find the registration by checking for the ",(0,a.kt)("em",{parentName:"p"},"Service type")," and the ",(0,a.kt)("em",{parentName:"p"},"name"),".")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'// the registration entity will look like: \n// IService => Implementation, name: Example\ncontainer.Register<IService, Implementation>("Example");\nvar service = container.Resolve<IService>("Example");\n')))),(0,a.kt)("h2",{id:"injectable-dependency"},"Injectable dependency"),(0,a.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"It's a constructor/method argument or a property/field of a registered ",(0,a.kt)("em",{parentName:"p"},"Implementation type")," that gets evaluated (",(0,a.kt)("em",{parentName:"p"},"injected"),") by Stashbox during the service's construction."),(0,a.kt)("p",null,"In this example, ",(0,a.kt)("inlineCode",{parentName:"p"},"Implementation")," has an ",(0,a.kt)("inlineCode",{parentName:"p"},"IDependency")," ",(0,a.kt)("em",{parentName:"p"},"injectable dependency")," in its constructor.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class Implementation : IService\n{\n    public Implementation(IDependency dependency) \n    { }\n}\n")))),(0,a.kt)("h2",{id:"resolution-tree"},"Resolution tree"),(0,a.kt)("p",null,"It's the structural representation of a service's resolution process. It describes the instantiation order of the dependencies required to resolve the desired type."),(0,a.kt)("p",null,"Let's see through an example:"),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class A\n{\n    public A(B b, C c) { }\n}\n\nclass B\n{\n    public B(C c, D d) { }\n}\n\nclass C { }\nclass D { }\n")),(0,a.kt)("p",null,"When we request the service ",(0,a.kt)("inlineCode",{parentName:"p"},"A"),", the container constructs the following resolution tree based on the dependencies and sub-dependencies."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre"},"        A\n      /   \\\n     B     C\n   /   \\\n  C     D\n")),(0,a.kt)("p",null,"The container instantiates those services first that don't have any dependencies. ",(0,a.kt)("inlineCode",{parentName:"p"},"C")," and ",(0,a.kt)("inlineCode",{parentName:"p"},"D")," will be injected into ",(0,a.kt)("inlineCode",{parentName:"p"},"B"),". Then, a new ",(0,a.kt)("inlineCode",{parentName:"p"},"C")," is instantiated (if it's ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/lifetimes#transient-lifetime"},"transient"),") and injected into ",(0,a.kt)("inlineCode",{parentName:"p"},"A")," along with the previously created ",(0,a.kt)("inlineCode",{parentName:"p"},"B"),"."),(0,a.kt)("h2",{id:"dependency-resolver"},"Dependency resolver"),(0,a.kt)("p",null,"It's the container itself or the ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"current scope"),", depending on which was asked to resolve a particular service. They are both implementing Stashbox's ",(0,a.kt)("inlineCode",{parentName:"p"},"IDependencyResolver")," and the .NET framework's ",(0,a.kt)("inlineCode",{parentName:"p"},"IServiceProvider")," interface and can be used for service resolution."),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"Stashbox implicitly injects the ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"current scope")," wherever ",(0,a.kt)("inlineCode",{parentName:"p"},"IDependencyResolver")," or ",(0,a.kt)("inlineCode",{parentName:"p"},"IServiceProvider")," is requested.")),(0,a.kt)("h2",{id:"root-scope"},"Root scope"),(0,a.kt)("p",null,"It's the ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"main scope")," created inside every container instance. It stores and handles the lifetime of all singletons. It's the base of each subsequent scope created by the container with the ",(0,a.kt)("inlineCode",{parentName:"p"},".BeginScope()")," method."),(0,a.kt)("admonition",{type:"caution"},(0,a.kt)("p",{parentName:"admonition"},(0,a.kt)("a",{parentName:"p",href:"/docs/guides/lifetimes#scoped-lifetime"},"Scoped services")," requested from the container (and not from a ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope"),") are managed by the root scope. This can lead to issues because their lifetime will effectively switch to singleton. Always be sure that you don't resolve scoped services directly from the container, only from a ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope"),". This case is monitored by the ",(0,a.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#lifetime-validation"},"lifetime")," validation rule when it's ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#lifetime-validation"},"enabled"),". ")),(0,a.kt)("h2",{id:"named-resolution"},"Named resolution"),(0,a.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"It's a resolution request for a named service. The same applies, when the container sees a dependency in the resolution tree with a name (set by ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#attributes"},"attributes")," or ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#dependency-binding"},"bindings"),"); it will search for a matching ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/basics#named-registration"},"Named registration")," to inject.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IService, Implementation>("Example");\n// the named request.\nvar service = container.Resolve<IService>("Example");\n')))),(0,a.kt)("h2",{id:"self-registration"},"Self registration"),(0,a.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"It's a service registration that's mapped to itself. This means its service and implementation type is the same.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"// equivalent to container.Register<Implementation, Implementation>();\ncontainer.Register<Implementation>();\n")))))}m.isMDXComponent=!0}}]);