"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[544],{3905:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>f});var i=n(7294);function a(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);t&&(i=i.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,i)}return n}function r(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){a(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,i,a=function(e,t){if(null==e)return{};var n,i,a={},o=Object.keys(e);for(i=0;i<o.length;i++)n=o[i],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(i=0;i<o.length;i++)n=o[i],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var l=i.createContext({}),c=function(e){var t=i.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):r(r({},t),e)),n},p=function(e){var t=c(e.components);return i.createElement(l.Provider,{value:t},e.children)},u="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return i.createElement(i.Fragment,{},t)}},m=i.forwardRef((function(e,t){var n=e.components,a=e.mdxType,o=e.originalType,l=e.parentName,p=s(e,["components","mdxType","originalType","parentName"]),u=c(n),m=a,f=u["".concat(l,".").concat(m)]||u[m]||d[m]||o;return n?i.createElement(f,r(r({ref:t},p),{},{components:n})):i.createElement(f,r({ref:t},p))}));function f(e,t){var n=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var o=n.length,r=new Array(o);r[0]=m;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s[u]="string"==typeof e?e:a,r[1]=s;for(var c=2;c<o;c++)r[c]=n[c];return i.createElement.apply(null,r)}return i.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>r});var i=n(7294),a=n(6010);const o="tabItem_Ymn6";function r(e){let{children:t,hidden:n,className:r}=e;return i.createElement("div",{role:"tabpanel",className:(0,a.Z)(o,r),hidden:n},t)}},5488:(e,t,n)=>{n.d(t,{Z:()=>m});var i=n(7462),a=n(7294),o=n(6010),r=n(2389),s=n(7392),l=n(7094),c=n(2466);const p="tabList__CuJ",u="tabItem_LNqP";function d(e){const{lazy:t,block:n,defaultValue:r,values:d,groupId:m,className:f}=e,b=a.Children.map(e.children,(e=>{if((0,a.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),h=d??b.map((e=>{let{props:{value:t,label:n,attributes:i}}=e;return{value:t,label:n,attributes:i}})),g=(0,s.l)(h,((e,t)=>e.value===t.value));if(g.length>0)throw new Error(`Docusaurus error: Duplicate values "${g.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const k=null===r?r:r??b.find((e=>e.props.default))?.props.value??b[0].props.value;if(null!==k&&!h.some((e=>e.value===k)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${k}" but none of its children has the corresponding value. Available values are: ${h.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:v,setTabGroupChoices:y}=(0,l.U)(),[w,N]=(0,a.useState)(k),T=[],{blockElementScrollPositionUntilNextRender:x}=(0,c.o5)();if(null!=m){const e=v[m];null!=e&&e!==w&&h.some((t=>t.value===e))&&N(e)}const D=e=>{const t=e.currentTarget,n=T.indexOf(t),i=h[n].value;i!==w&&(x(t),N(i),null!=m&&y(m,String(i)))},S=e=>{let t=null;switch(e.key){case"Enter":D(e);break;case"ArrowRight":{const n=T.indexOf(e.currentTarget)+1;t=T[n]??T[0];break}case"ArrowLeft":{const n=T.indexOf(e.currentTarget)-1;t=T[n]??T[T.length-1];break}}t?.focus()};return a.createElement("div",{className:(0,o.Z)("tabs-container",p)},a.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":n},f)},h.map((e=>{let{value:t,label:n,attributes:r}=e;return a.createElement("li",(0,i.Z)({role:"tab",tabIndex:w===t?0:-1,"aria-selected":w===t,key:t,ref:e=>T.push(e),onKeyDown:S,onClick:D},r,{className:(0,o.Z)("tabs__item",u,r?.className,{"tabs__item--active":w===t})}),n??t)}))),t?(0,a.cloneElement)(b.filter((e=>e.props.value===w))[0],{className:"margin-top--md"}):a.createElement("div",{className:"margin-top--md"},b.map(((e,t)=>(0,a.cloneElement)(e,{key:t,hidden:e.props.value!==w})))))}function m(e){const t=(0,r.Z)();return a.createElement(d,(0,i.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>s});var i=n(7294);const a="codeDescContainer_ie8f",o="desc_jyqI",r="example_eYlF";function s(e){let{children:t}=e,n=i.Children.toArray(t).filter((e=>e));return i.createElement("div",{className:a},i.createElement("div",{className:o},n[0]),i.createElement("div",{className:r},n[1]))}},4037:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>u,contentTitle:()=>c,default:()=>f,frontMatter:()=>l,metadata:()=>p,toc:()=>d});var i=n(7462),a=(n(7294),n(3905)),o=n(8846),r=n(5488),s=n(5162);const l={},c="Lifetimes",p={unversionedId:"guides/lifetimes",id:"guides/lifetimes",title:"Lifetimes",description:"Lifetime management is the concept of controlling how long a service's instances will live (from instantiation to disposal) and how they will be reused between resolution requests.",source:"@site/docs/guides/lifetimes.md",sourceDirName:"guides",slug:"/guides/lifetimes",permalink:"/stashbox/docs/guides/lifetimes",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/lifetimes.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1672673299,formattedLastUpdatedAt:"Jan 2, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Service resolution",permalink:"/stashbox/docs/guides/service-resolution"},next:{title:"Scopes",permalink:"/stashbox/docs/guides/scopes"}},u={},d=[{value:"Default lifetime",id:"default-lifetime",level:2},{value:"Transient lifetime",id:"transient-lifetime",level:2},{value:"Singleton lifetime",id:"singleton-lifetime",level:2},{value:"Scoped lifetime",id:"scoped-lifetime",level:2},{value:"Named scope lifetime",id:"named-scope-lifetime",level:2},{value:"Per-request lifetime",id:"per-request-lifetime",level:2},{value:"Per-scoped request lifetime",id:"per-scoped-request-lifetime",level:2},{value:"Custom lifetime",id:"custom-lifetime",level:2}],m={toc:d};function f(e){let{components:t,...n}=e;return(0,a.kt)("wrapper",(0,i.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"lifetimes"},"Lifetimes"),(0,a.kt)("p",null,"Lifetime management is the concept of controlling how long a service's instances will live (from instantiation to ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes#disposal"},"disposal"),") and how they will be reused between resolution requests."),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"Choosing the right lifetime helps you avoid ",(0,a.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#lifetime-validation"},"captive dependencies"),".")),(0,a.kt)("h2",{id:"default-lifetime"},"Default lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"When you are not specifying a lifetime during registration, Stashbox will use the default lifetime. By default, it's set to ",(0,a.kt)("strong",{parentName:"p"},"Transient"),", but you can override it with the ",(0,a.kt)("inlineCode",{parentName:"p"},".WithDefaultLifetime()")," ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#default-lifetime"},"container configuration option"),". "),(0,a.kt)("p",null,"You can choose either from the pre-defined lifetimes defined in the ",(0,a.kt)("inlineCode",{parentName:"p"},"Lifetimes")," static class or use a ",(0,a.kt)("a",{parentName:"p",href:"#custom-lifetime"},"custom one"),".")),(0,a.kt)("div",null,(0,a.kt)(r.Z,{mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Transient (default)",label:"Transient (default)",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Transient));\n"))),(0,a.kt)(s.Z,{value:"Singleton",label:"Singleton",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Singleton));\n"))),(0,a.kt)(s.Z,{value:"Scoped",label:"Scoped",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Scoped));\n")))))),(0,a.kt)("h2",{id:"transient-lifetime"},"Transient lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"A new instance will be created for every resolution request. If a transient is referred by multiple consumers in the same ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#resolution-tree"},"resolution tree"),", each of them will get a new instance.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Transient));\n")))),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"Transient services are not tracked for disposal by default, but this feature can be turned on with the ",(0,a.kt)("inlineCode",{parentName:"p"},".WithDisposableTransientTracking()")," ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#tracking-disposable-transients"},"container configuration option"),". When it's enabled, the current ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope")," on which the resolution request was initiated takes the responsibility to track and dispose the transient services.")),(0,a.kt)("h2",{id:"singleton-lifetime"},"Singleton lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"A single instance will be created and reused for every resolution request and injected into every consumer."),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"Singleton services are disposed when the container (",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#root-scope"},"root scope"),") is being disposed."))),(0,a.kt)("div",null,(0,a.kt)(r.Z,{groupId:"lifetime-forms",mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Longer form",label:"Longer form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Singleton));\n"))),(0,a.kt)(s.Z,{value:"Shorter form",label:"Shorter form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterSingleton<IJob, DbBackup>();\n")))))),(0,a.kt)("h2",{id:"scoped-lifetime"},"Scoped lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"A new instance is created for each ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope"),", and that instance will be returned for every resolution request initiated on the given scope. It's like a singleton lifetime within the scope. "),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"Scoped services are disposed when their scope is being disposed."))),(0,a.kt)("div",null,(0,a.kt)(r.Z,{groupId:"lifetime-forms",mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Longer form",label:"Longer form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Scoped));\n\nusing var scope = container.BeginScope();\nIJob job = scope.Resolve<IJob>();\n"))),(0,a.kt)(s.Z,{value:"Shorter form",label:"Shorter form",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterScoped<IJob, DbBackup>();\n\nusing var scope = container.BeginScope();\nIJob job = scope.Resolve<IJob>();\n")))))),(0,a.kt)("h2",{id:"named-scope-lifetime"},"Named scope lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"It is the same as the scoped lifetime, except that the given service will be selected only when the resolution request is initiated by a scope with the same name."),(0,a.kt)("p",null,"You can also let a service ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/scopes#service-as-scope"},"define")," its own named scope within itself. During registration, this scope can be referred to by its name upon using a named scope lifetime."),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"Services with named scope lifetime are disposed when the related named scope is being disposed."))),(0,a.kt)("div",null,(0,a.kt)(r.Z,{mdxType:"Tabs"},(0,a.kt)(s.Z,{value:"Named",label:"Named",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>(options => options\n    .InNamedScope("DbScope"));\n\nusing var scope = container.BeginScope("DbScope");\nIJob job = scope.Resolve<IJob>();\n'))),(0,a.kt)(s.Z,{value:"Defined",label:"Defined",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<DbJobExecutor>(options => options\n    .DefinesScope());\n\nontainer.Register<IJob, DbBackup>(options => options\n    .InScopeDefinedBy<DbJobExecutor>());\n\n// the executor will begin a new scope within itself\n// when it gets resolved and DbBackup will be selected\n// and attached to that scope instead.\nusing var scope = container.BeginScope();\nDbJobExecutor executor = scope.Resolve<DbJobExecutor>();\n"))),(0,a.kt)(s.Z,{value:"Defined with name",label:"Defined with name",mdxType:"TabItem"},(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<DbJobExecutor>(options => options\n    .DefinesScope("DbScope"));\n\nontainer.Register<IJob, DbBackup>(options => options\n    .InNamedScope("DbScope"));\n\n// the executor will begin a new scope within itself\n// when it gets resolved and DbBackup will be selected\n// and attached to that scope instead.\nusing var scope = container.BeginScope();\nDbJobExecutor executor = scope.Resolve<DbJobExecutor>();\n')))))),(0,a.kt)("h2",{id:"per-request-lifetime"},"Per-request lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"The requested service will be reused within the whole resolution request. For each individual request a new instance will be created.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithPerRequestLifetime());\n")))),(0,a.kt)("h2",{id:"per-scoped-request-lifetime"},"Per-scoped request lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"The requested service will behave like a singleton, but only within a scoped dependency request. This means every scoped service will get a new exclusive instance that will be used by its sub-dependencies as well.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .WithPerScopedRequestLifetime());\n")))),(0,a.kt)("h2",{id:"custom-lifetime"},"Custom lifetime"),(0,a.kt)("p",null,"Suppose you'd like to use a custom lifetime. In that case, you can create your implementation by inheriting either from ",(0,a.kt)("inlineCode",{parentName:"p"},"FactoryLifetimeDescriptor")," or from ",(0,a.kt)("inlineCode",{parentName:"p"},"ExpressionLifetimeDescriptor"),", depending on how do you want to manage the given service instances. Then you can pass it to the ",(0,a.kt)("inlineCode",{parentName:"p"},"WithLifetime()")," configuration method."),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"ExpressionLifetimeDescriptor"),": With this, you can build your lifetime with the expression form of the service instantiation."),(0,a.kt)("pre",{parentName:"li"},(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class CustomLifetime : ExpressionLifetimeDescriptor\n{\n    protected override Expression ApplyLifetime(\n        Expression expression, // The expression which describes the service creation\n        ServiceRegistration serviceRegistration, \n        ResolutionContext resolutionContext, \n        Type requestedType)\n    {\n        // Lifetime managing functionality\n    }\n}\n"))),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("p",{parentName:"li"},(0,a.kt)("strong",{parentName:"p"},"FactoryLifetimeDescriptor"),": With this, you can build your lifetime based on a pre-compiled factory delegate used for service instantiation."),(0,a.kt)("pre",{parentName:"li"},(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"class CustomLifetime : FactoryLifetimeDescriptor\n{\n    protected override Expression ApplyLifetime(\n        Func<IResolutionScope, object> factory, // The factory used for service creation\n        ServiceRegistration serviceRegistration, \n        ResolutionContext resolutionContext, \n        Type requestedType)\n    {\n        // Lifetime managing functionality\n    }\n}\n")))),(0,a.kt)("p",null,"Then you can use your lifetime like this:"),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options.WithLifetime(new CustomLifetime()));\n")))}f.isMDXComponent=!0}}]);