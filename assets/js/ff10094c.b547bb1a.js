"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[544],{3905:(e,t,n)=>{n.d(t,{Zo:()=>u,kt:()=>f});var i=n(7294);function a(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function r(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);t&&(i=i.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,i)}return n}function o(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?r(Object(n),!0).forEach((function(t){a(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):r(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,i,a=function(e,t){if(null==e)return{};var n,i,a={},r=Object.keys(e);for(i=0;i<r.length;i++)n=r[i],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);for(i=0;i<r.length;i++)n=r[i],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var l=i.createContext({}),c=function(e){var t=i.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):o(o({},t),e)),n},u=function(e){var t=c(e.components);return i.createElement(l.Provider,{value:t},e.children)},p="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return i.createElement(i.Fragment,{},t)}},m=i.forwardRef((function(e,t){var n=e.components,a=e.mdxType,r=e.originalType,l=e.parentName,u=s(e,["components","mdxType","originalType","parentName"]),p=c(n),m=a,f=p["".concat(l,".").concat(m)]||p[m]||d[m]||r;return n?i.createElement(f,o(o({ref:t},u),{},{components:n})):i.createElement(f,o({ref:t},u))}));function f(e,t){var n=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var r=n.length,o=new Array(r);o[0]=m;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s[p]="string"==typeof e?e:a,o[1]=s;for(var c=2;c<r;c++)o[c]=n[c];return i.createElement.apply(null,o)}return i.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>o});var i=n(7294),a=n(6010);const r="tabItem_Ymn6";function o(e){let{children:t,hidden:n,className:o}=e;return i.createElement("div",{role:"tabpanel",className:(0,a.Z)(r,o),hidden:n},t)}},4866:(e,t,n)=>{n.d(t,{Z:()=>N});var i=n(7462),a=n(7294),r=n(6010),o=n(2466),s=n(6550),l=n(1980),c=n(7392),u=n(12);function p(e){return function(e){return a.Children.map(e,(e=>{if(!e||(0,a.isValidElement)(e)&&function(e){const{props:t}=e;return!!t&&"object"==typeof t&&"value"in t}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}(e).map((e=>{let{props:{value:t,label:n,attributes:i,default:a}}=e;return{value:t,label:n,attributes:i,default:a}}))}function d(e){const{values:t,children:n}=e;return(0,a.useMemo)((()=>{const e=t??p(n);return function(e){const t=(0,c.l)(e,((e,t)=>e.value===t.value));if(t.length>0)throw new Error(`Docusaurus error: Duplicate values "${t.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[t,n])}function m(e){let{value:t,tabValues:n}=e;return n.some((e=>e.value===t))}function f(e){let{queryString:t=!1,groupId:n}=e;const i=(0,s.k6)(),r=function(e){let{queryString:t=!1,groupId:n}=e;if("string"==typeof t)return t;if(!1===t)return null;if(!0===t&&!n)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return n??null}({queryString:t,groupId:n});return[(0,l._X)(r),(0,a.useCallback)((e=>{if(!r)return;const t=new URLSearchParams(i.location.search);t.set(r,e),i.replace({...i.location,search:t.toString()})}),[r,i])]}function b(e){const{defaultValue:t,queryString:n=!1,groupId:i}=e,r=d(e),[o,s]=(0,a.useState)((()=>function(e){let{defaultValue:t,tabValues:n}=e;if(0===n.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(t){if(!m({value:t,tabValues:n}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${t}" but none of its children has the corresponding value. Available values are: ${n.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return t}const i=n.find((e=>e.default))??n[0];if(!i)throw new Error("Unexpected error: 0 tabValues");return i.value}({defaultValue:t,tabValues:r}))),[l,c]=f({queryString:n,groupId:i}),[p,b]=function(e){let{groupId:t}=e;const n=function(e){return e?`docusaurus.tab.${e}`:null}(t),[i,r]=(0,u.Nk)(n);return[i,(0,a.useCallback)((e=>{n&&r.set(e)}),[n,r])]}({groupId:i}),h=(()=>{const e=l??p;return m({value:e,tabValues:r})?e:null})();(0,a.useLayoutEffect)((()=>{h&&s(h)}),[h]);return{selectedValue:o,selectValue:(0,a.useCallback)((e=>{if(!m({value:e,tabValues:r}))throw new Error(`Can't select invalid tab value=${e}`);s(e),c(e),b(e)}),[c,b,r]),tabValues:r}}var h=n(2389);const g="tabList__CuJ",k="tabItem_LNqP";function v(e){let{className:t,block:n,selectedValue:s,selectValue:l,tabValues:c}=e;const u=[],{blockElementScrollPositionUntilNextRender:p}=(0,o.o5)(),d=e=>{const t=e.currentTarget,n=u.indexOf(t),i=c[n].value;i!==s&&(p(t),l(i))},m=e=>{let t=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const n=u.indexOf(e.currentTarget)+1;t=u[n]??u[0];break}case"ArrowLeft":{const n=u.indexOf(e.currentTarget)-1;t=u[n]??u[u.length-1];break}}t?.focus()};return a.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,r.Z)("tabs",{"tabs--block":n},t)},c.map((e=>{let{value:t,label:n,attributes:o}=e;return a.createElement("li",(0,i.Z)({role:"tab",tabIndex:s===t?0:-1,"aria-selected":s===t,key:t,ref:e=>u.push(e),onKeyDown:m,onClick:d},o,{className:(0,r.Z)("tabs__item",k,o?.className,{"tabs__item--active":s===t})}),n??t)})))}function y(e){let{lazy:t,children:n,selectedValue:i}=e;const r=(Array.isArray(n)?n:[n]).filter(Boolean);if(t){const e=r.find((e=>e.props.value===i));return e?(0,a.cloneElement)(e,{className:"margin-top--md"}):null}return a.createElement("div",{className:"margin-top--md"},r.map(((e,t)=>(0,a.cloneElement)(e,{key:t,hidden:e.props.value!==i}))))}function w(e){const t=b(e);return a.createElement("div",{className:(0,r.Z)("tabs-container",g)},a.createElement(v,(0,i.Z)({},e,t)),a.createElement(y,(0,i.Z)({},e,t)))}function N(e){const t=(0,h.Z)();return a.createElement(w,(0,i.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>s});var i=n(7294);const a="codeDescContainer_ie8f",r="desc_jyqI",o="example_eYlF";function s(e){let{children:t}=e,n=i.Children.toArray(t).filter((e=>e));return i.createElement("div",{className:a},i.createElement("div",{className:r},n[0]),i.createElement("div",{className:o},n[1]))}},4037:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>p,contentTitle:()=>c,default:()=>f,frontMatter:()=>l,metadata:()=>u,toc:()=>d});var i=n(7462),a=(n(7294),n(3905)),r=n(8846),o=n(4866),s=n(5162);const l={},c="Lifetimes",u={unversionedId:"guides/lifetimes",id:"guides/lifetimes",title:"Lifetimes",description:"Lifetime management controls how long a service's instances will live (from instantiation to disposal) and how they will be reused between resolution requests.",source:"@site/docs/guides/lifetimes.md",sourceDirName:"guides",slug:"/guides/lifetimes",permalink:"/stashbox/docs/guides/lifetimes",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/lifetimes.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1699538162,formattedLastUpdatedAt:"Nov 9, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Service resolution",permalink:"/stashbox/docs/guides/service-resolution"},next:{title:"Scopes",permalink:"/stashbox/docs/guides/scopes"}},p={},d=[{value:"Default lifetime",id:"default-lifetime",level:2},{value:"Transient lifetime",id:"transient-lifetime",level:2},{value:"Singleton lifetime",id:"singleton-lifetime",level:2},{value:"Scoped lifetime",id:"scoped-lifetime",level:2},{value:"Named scope lifetime",id:"named-scope-lifetime",level:2},{value:"Per-request lifetime",id:"per-request-lifetime",level:2},{value:"Per-scoped request lifetime",id:"per-scoped-request-lifetime",level:2},{value:"Custom lifetime",id:"custom-lifetime",level:2}],m={toc:d};function f(e){let{components:t,...n}=e;return(0,a.kt)("wrapper",(0,i.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"lifetimes"},"Lifetimes"),(0,a.kt)("p",null,"Lifetime management controls how long a service's instances will live (from instantiation to ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes#disposal"},"disposal"),") and how they will be reused between resolution requests."),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"Choosing the right lifetime helps you avoid ",(0,a.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#lifetime-validation"},"captive dependencies"),".")),(0,a.kt)("h2",{id:"default-lifetime"},"Default lifetime"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"When you are not specifying a lifetime during registration, Stashbox will use the default lifetime. By default, it's set to ",(0,a.kt)("a",{parentName:"p",href:"#transient-lifetime"},"Transient"),", but you can override it with the ",(0,a.kt)("inlineCode",{parentName:"p"},".WithDefaultLifetime()")," ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#default-lifetime"},"container configuration option"),". "),(0,a.kt)("p",null,"You can choose either from the pre-defined lifetimes defined on the ",(0,a.kt)("inlineCode",{parentName:"p"},"Lifetimes")," static class or use a ",(0,a.kt)("a",{parentName:"p",href:"#custom-lifetime"},"custom lifetime"),".")),(0,a.kt)("div",null,(0,a.kt)(o.Z,{mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Transient (default)",label:"Transient (default)",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Transient));\n"))),(0,a.kt)(s.Z,{value:"Singleton",label:"Singleton",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Singleton));\n"))),(0,a.kt)(s.Z,{value:"Scoped",label:"Scoped",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Scoped));\n")))))),(0,a.kt)("h2",{id:"transient-lifetime"},"Transient lifetime"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"A new instance is created for each resolution request. If a transient is referred by multiple consumers in the same ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#resolution-tree"},"resolution tree"),", each will get a new instance.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Transient));\n")))),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"Transient services are not tracked for disposal by default, but this feature can be turned on with the ",(0,a.kt)("inlineCode",{parentName:"p"},".WithDisposableTransientTracking()")," ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#tracking-disposable-transients"},"container configuration option"),". When it's enabled, the current ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope")," on which the resolution request was initiated takes the responsibility to track and dispose transient services.")),(0,a.kt)("h2",{id:"singleton-lifetime"},"Singleton lifetime"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"A single instance is created and reused for each resolution request and injected into each consumer."),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"Singleton services are disposed when the container (",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#root-scope"},"root scope"),") is being disposed."))),(0,a.kt)("div",null,(0,a.kt)(o.Z,{groupId:"lifetime-forms",mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Longer form",label:"Longer form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Singleton));\n"))),(0,a.kt)(s.Z,{value:"Shorter form",label:"Shorter form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterSingleton<IJob, DbBackup>();\n")))))),(0,a.kt)("h2",{id:"scoped-lifetime"},"Scoped lifetime"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"A new instance is created for each ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope"),", which will be returned for every resolution request initiated on the given scope. It's like a singleton lifetime within a scope. "),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"Scoped services are disposed when their scope is being disposed."))),(0,a.kt)("div",null,(0,a.kt)(o.Z,{groupId:"lifetime-forms",mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Longer form",label:"Longer form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Scoped));\n\nusing var scope = container.BeginScope();\nIJob job = scope.Resolve<IJob>();\n"))),(0,a.kt)(s.Z,{value:"Shorter form",label:"Shorter form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterScoped<IJob, DbBackup>();\n\nusing var scope = container.BeginScope();\nIJob job = scope.Resolve<IJob>();\n")))))),(0,a.kt)("h2",{id:"named-scope-lifetime"},"Named scope lifetime"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"It is the same as scoped lifetime, except the given service will be selected only when a scope with the same name initiates the resolution request."),(0,a.kt)("p",null,"You can also let a service ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes#service-as-scope"},"define")," its own named scope. During registration, this scope can be referred to by its name upon using a named scope lifetime."),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"Services with named scope lifetime are disposed when the related named scope is being disposed."))),(0,a.kt)("div",null,(0,a.kt)(o.Z,{mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Named",label:"Named",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>(options => options\n    .InNamedScope("DbScope"));\n\nusing var scope = container.BeginScope("DbScope");\nIJob job = scope.Resolve<IJob>();\n'))),(0,a.kt)(s.Z,{value:"Defined",label:"Defined",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<DbJobExecutor>(options => options\n    .DefinesScope());\n\nontainer.Register<IJob, DbBackup>(options => options\n    .InScopeDefinedBy<DbJobExecutor>());\n\n// the executor will begin a new scope within itself\n// when it gets resolved and DbBackup will be selected\n// and attached to that scope instead.\nusing var scope = container.BeginScope();\nDbJobExecutor executor = scope.Resolve<DbJobExecutor>();\n"))),(0,a.kt)(s.Z,{value:"Defined with name",label:"Defined with name",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<DbJobExecutor>(options => options\n    .DefinesScope("DbScope"));\n\nontainer.Register<IJob, DbBackup>(options => options\n    .InNamedScope("DbScope"));\n\n// the executor will begin a new scope within itself\n// when it gets resolved and DbBackup will be selected\n// and attached to that scope instead.\nusing var scope = container.BeginScope();\nDbJobExecutor executor = scope.Resolve<DbJobExecutor>();\n')))))),(0,a.kt)("h2",{id:"per-request-lifetime"},"Per-request lifetime"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"The requested service will be reused within the whole resolution request. A new instance is created for each individual request .")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithPerRequestLifetime());\n")))),(0,a.kt)("h2",{id:"per-scoped-request-lifetime"},"Per-scoped request lifetime"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"The requested service will behave like a singleton, but only within a scoped dependency request. This means every scoped service will get a new exclusive instance that will be used by its sub-dependencies as well.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithPerScopedRequestLifetime());\n")))),(0,a.kt)("h2",{id:"custom-lifetime"},"Custom lifetime"),(0,a.kt)("p",null,"If you'd like to use a custom lifetime, you can create your implementation by inheriting either from ",(0,a.kt)("inlineCode",{parentName:"p"},"FactoryLifetimeDescriptor")," or from ",(0,a.kt)("inlineCode",{parentName:"p"},"ExpressionLifetimeDescriptor"),", depending on how do you want to manage the service instances."),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"ExpressionLifetimeDescriptor"),": With this, you can build your lifetime with the expression form of the service instantiation."),(0,a.kt)("pre",{parentName:"li"},(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class CustomLifetime : ExpressionLifetimeDescriptor\n{\n    protected override Expression ApplyLifetime(\n        Expression expression, // The expression which describes the service creation\n        ServiceRegistration serviceRegistration, \n        ResolutionContext resolutionContext, \n        Type requestedType)\n    {\n        // Lifetime managing functionality\n    }\n}\n"))),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"FactoryLifetimeDescriptor"),": With this, you can build your lifetime based on a pre-compiled factory delegate used for service instantiation."),(0,a.kt)("pre",{parentName:"li"},(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class CustomLifetime : FactoryLifetimeDescriptor\n{\n    protected override Expression ApplyLifetime(\n        Func<IResolutionScope, object> factory, // The factory used for service creation\n        ServiceRegistration serviceRegistration, \n        ResolutionContext resolutionContext, \n        Type requestedType)\n    {\n        // Lifetime managing functionality\n    }\n}\n")))),(0,a.kt)("p",null,"Then you can use your lifetime like this:"),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options.WithLifetime(new CustomLifetime()));\n")))}f.isMDXComponent=!0}}]);