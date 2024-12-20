"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[338],{8594:(e,i,n)=>{n.r(i),n.d(i,{assets:()=>u,contentTitle:()=>c,default:()=>f,frontMatter:()=>l,metadata:()=>d,toc:()=>h});var t=n(4848),s=n(8453),r=n(7470),o=n(1470),a=n(9365);const l={},c="Lifetimes",d={id:"guides/lifetimes",title:"Lifetimes",description:"Lifetime management controls how long a service's instances will live (from instantiation to disposal) and how they will be reused between resolution requests.",source:"@site/docs/guides/lifetimes.md",sourceDirName:"guides",slug:"/guides/lifetimes",permalink:"/stashbox/docs/guides/lifetimes",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/lifetimes.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1734711205,formattedLastUpdatedAt:"Dec 20, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Service resolution",permalink:"/stashbox/docs/guides/service-resolution"},next:{title:"Scopes",permalink:"/stashbox/docs/guides/scopes"}},u={},h=[{value:"Default lifetime",id:"default-lifetime",level:2},{value:"Transient lifetime",id:"transient-lifetime",level:2},{value:"Singleton lifetime",id:"singleton-lifetime",level:2},{value:"Scoped lifetime",id:"scoped-lifetime",level:2},{value:"Named scope lifetime",id:"named-scope-lifetime",level:2},{value:"Per-request lifetime",id:"per-request-lifetime",level:2},{value:"Per-scoped request lifetime",id:"per-scoped-request-lifetime",level:2},{value:"Auto lifetime",id:"auto-lifetime",level:2},{value:"Custom lifetime",id:"custom-lifetime",level:2}];function p(e){const i={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",li:"li",p:"p",pre:"pre",strong:"strong",ul:"ul",...(0,s.R)(),...e.components};return(0,t.jsxs)(t.Fragment,{children:[(0,t.jsx)(i.h1,{id:"lifetimes",children:"Lifetimes"}),"\n",(0,t.jsxs)(i.p,{children:["Lifetime management controls how long a service's instances will live (from instantiation to ",(0,t.jsx)(i.a,{href:"/docs/guides/scopes#disposal",children:"disposal"}),") and how they will be reused between resolution requests."]}),"\n",(0,t.jsx)(i.admonition,{type:"info",children:(0,t.jsxs)(i.p,{children:["Choosing the right lifetime helps you avoid ",(0,t.jsx)(i.a,{href:"/docs/diagnostics/validation#lifetime-validation",children:"captive dependencies"}),"."]})}),"\n",(0,t.jsx)(i.h2,{id:"default-lifetime",children:"Default lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(i.p,{children:["When you are not specifying a lifetime during registration, Stashbox will use the default lifetime. By default, it's set to ",(0,t.jsx)(i.a,{href:"#transient-lifetime",children:"Transient"}),", but you can override it with the ",(0,t.jsx)(i.code,{children:".WithDefaultLifetime()"})," ",(0,t.jsx)(i.a,{href:"/docs/configuration/container-configuration#default-lifetime",children:"container configuration option"}),"."]}),(0,t.jsxs)(i.p,{children:["You can choose either from the pre-defined lifetimes defined on the ",(0,t.jsx)(i.code,{children:"Lifetimes"})," static class or use a ",(0,t.jsx)(i.a,{href:"#custom-lifetime",children:"custom lifetime"}),"."]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.A,{children:[(0,t.jsx)(a.A,{value:"Transient (default)",label:"Transient (default)",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Transient));\n"})})}),(0,t.jsx)(a.A,{value:"Singleton",label:"Singleton",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Singleton));\n"})})}),(0,t.jsx)(a.A,{value:"Scoped",label:"Scoped",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"var container = new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Scoped));\n"})})})]})})]}),"\n",(0,t.jsx)(i.h2,{id:"transient-lifetime",children:"Transient lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsxs)(i.p,{children:["A new instance is created for each resolution request. If a transient is referred by multiple consumers in the same ",(0,t.jsx)(i.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree"}),", each will get a new instance."]})}),(0,t.jsx)("div",{children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Transient));\n"})})})]}),"\n",(0,t.jsx)(i.admonition,{type:"info",children:(0,t.jsxs)(i.p,{children:["Transient services are not tracked for disposal by default, but this feature can be turned on with the ",(0,t.jsx)(i.code,{children:".WithDisposableTransientTracking()"})," ",(0,t.jsx)(i.a,{href:"/docs/configuration/container-configuration#tracking-disposable-transients",children:"container configuration option"}),". When it's enabled, the current ",(0,t.jsx)(i.a,{href:"/docs/guides/scopes",children:"scope"})," on which the resolution request was initiated takes the responsibility to track and dispose transient services."]})}),"\n",(0,t.jsx)(i.h2,{id:"singleton-lifetime",children:"Singleton lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(i.p,{children:"A single instance is created and reused for each resolution request and injected into each consumer."}),(0,t.jsx)(i.admonition,{type:"note",children:(0,t.jsxs)(i.p,{children:["Singleton services are disposed when the container (",(0,t.jsx)(i.a,{href:"/docs/getting-started/glossary#root-scope",children:"root scope"}),") is being disposed."]})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.A,{groupId:"lifetime-forms",children:[(0,t.jsx)(a.A,{value:"Longer form",label:"Longer form",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Singleton));\n"})})}),(0,t.jsx)(a.A,{value:"Shorter form",label:"Shorter form",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.RegisterSingleton<IJob, DbBackup>();\n"})})})]})})]}),"\n",(0,t.jsx)(i.h2,{id:"scoped-lifetime",children:"Scoped lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(i.p,{children:["A new instance is created for each ",(0,t.jsx)(i.a,{href:"/docs/guides/scopes",children:"scope"}),", which will be returned for every resolution request initiated on the given scope. It's like a singleton lifetime within a scope."]}),(0,t.jsx)(i.admonition,{type:"note",children:(0,t.jsx)(i.p,{children:"Scoped services are disposed when their scope is being disposed."})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.A,{groupId:"lifetime-forms",children:[(0,t.jsx)(a.A,{value:"Longer form",label:"Longer form",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithLifetime(Lifetimes.Scoped));\n\nusing var scope = container.BeginScope();\nIJob job = scope.Resolve<IJob>();\n"})})}),(0,t.jsx)(a.A,{value:"Shorter form",label:"Shorter form",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.RegisterScoped<IJob, DbBackup>();\n\nusing var scope = container.BeginScope();\nIJob job = scope.Resolve<IJob>();\n"})})})]})})]}),"\n",(0,t.jsx)(i.h2,{id:"named-scope-lifetime",children:"Named scope lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(i.p,{children:"It is the same as scoped lifetime, except the given service will be selected only when a scope with the same name initiates the resolution request."}),(0,t.jsxs)(i.p,{children:["You can also let a service ",(0,t.jsx)(i.a,{href:"/docs/guides/scopes#service-as-scope",children:"define"})," its own named scope. During registration, this scope can be referred to by its name upon using a named scope lifetime."]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(o.A,{children:[(0,t.jsx)(a.A,{value:"Named",label:"Named",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>(options => options\n    .InNamedScope("DbScope"));\n\nusing var scope = container.BeginScope("DbScope");\nIJob job = scope.Resolve<IJob>();\n'})})}),(0,t.jsx)(a.A,{value:"Defined",label:"Defined",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<DbJobExecutor>(options => options\n    .DefinesScope());\n\nontainer.Register<IJob, DbBackup>(options => options\n    .InScopeDefinedBy<DbJobExecutor>());\n\n// the executor will begin a new scope within itself\n// when it gets resolved and DbBackup will be selected\n// and attached to that scope instead.\nusing var scope = container.BeginScope();\nDbJobExecutor executor = scope.Resolve<DbJobExecutor>();\n"})})}),(0,t.jsx)(a.A,{value:"Defined with name",label:"Defined with name",children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:'container.Register<DbJobExecutor>(options => options\n    .DefinesScope("DbScope"));\n\nontainer.Register<IJob, DbBackup>(options => options\n    .InNamedScope("DbScope"));\n\n// the executor will begin a new scope within itself\n// when it gets resolved and DbBackup will be selected\n// and attached to that scope instead.\nusing var scope = container.BeginScope();\nDbJobExecutor executor = scope.Resolve<DbJobExecutor>();\n'})})})]})})]}),"\n",(0,t.jsx)(i.admonition,{type:"note",children:(0,t.jsx)(i.p,{children:"Services with named scope lifetime are disposed when the related named scope is being disposed."})}),"\n",(0,t.jsx)(i.h2,{id:"per-request-lifetime",children:"Per-request lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(i.p,{children:"The requested service will be reused within the whole resolution request. A new instance is created for each individual request ."})}),(0,t.jsx)("div",{children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithPerRequestLifetime());\n"})})})]}),"\n",(0,t.jsx)(i.h2,{id:"per-scoped-request-lifetime",children:"Per-scoped request lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(i.p,{children:"The requested service will behave like a singleton, but only within a scoped dependency request. This means every scoped service will get a new exclusive instance that will be used by its sub-dependencies as well."})}),(0,t.jsx)("div",{children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithPerScopedRequestLifetime());\n"})})})]}),"\n",(0,t.jsx)(i.h2,{id:"auto-lifetime",children:"Auto lifetime"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(i.p,{children:"The requested service's lifetime will align to the lifetime of its dependencies. When the requested service has a dependency with a higher lifespan, this lifetime will inherit that lifespan up to a given boundary."})}),(0,t.jsx)("div",{children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .WithAutoLifetime(Lifetimes.Scoped /* boundary lifetime */));\n"})})})]}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(i.p,{children:"If the requested service has auto lifetime with a scoped boundary and it has only transient dependencies, it'll inherit their transient lifetime."})}),(0,t.jsx)("div",{children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<ILogger, Logger>();\n\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithAutoLifetime(Lifetimes.Scoped /* boundary lifetime */));\n\n// job has transient lifetime.\nvar job = container.Resolve<IJob>();\n"})})})]}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(i.p,{children:"When there's a dependency with higher lifespan than the given boundary, the requested service will get the boundary lifetime."})}),(0,t.jsx)("div",{children:(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.RegisterSingleton<ILogger, Logger>();\n\ncontainer.Register<IJob, DbBackup>(options => options\n    .WithAutoLifetime(Lifetimes.Scoped /* boundary lifetime */));\n\n// job has scoped lifetime.\nvar job = container.Resolve<IJob>();\n"})})})]}),"\n",(0,t.jsx)(i.h2,{id:"custom-lifetime",children:"Custom lifetime"}),"\n",(0,t.jsxs)(i.p,{children:["If you'd like to use a custom lifetime, you can create your implementation by inheriting either from ",(0,t.jsx)(i.code,{children:"FactoryLifetimeDescriptor"})," or from ",(0,t.jsx)(i.code,{children:"ExpressionLifetimeDescriptor"}),", depending on how do you want to manage the service instances."]}),"\n",(0,t.jsxs)(i.ul,{children:["\n",(0,t.jsxs)(i.li,{children:["\n",(0,t.jsxs)(i.p,{children:[(0,t.jsx)(i.strong,{children:"ExpressionLifetimeDescriptor"}),": With this, you can build your lifetime with the expression form of the service instantiation."]}),"\n",(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"class CustomLifetime : ExpressionLifetimeDescriptor\n{\n    protected override Expression ApplyLifetime(\n        Expression expression, // The expression which describes the service creation\n        ServiceRegistration serviceRegistration, \n        ResolutionContext resolutionContext, \n        Type requestedType)\n    {\n        // Lifetime managing functionality\n    }\n}\n"})}),"\n"]}),"\n",(0,t.jsxs)(i.li,{children:["\n",(0,t.jsxs)(i.p,{children:[(0,t.jsx)(i.strong,{children:"FactoryLifetimeDescriptor"}),": With this, you can build your lifetime based on a pre-compiled factory delegate used for service instantiation."]}),"\n",(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"class CustomLifetime : FactoryLifetimeDescriptor\n{\n    protected override Expression ApplyLifetime(\n        Func<IResolutionScope, object> factory, // The factory used for service creation\n        ServiceRegistration serviceRegistration, \n        ResolutionContext resolutionContext, \n        Type requestedType)\n    {\n        // Lifetime managing functionality\n    }\n}\n"})}),"\n"]}),"\n"]}),"\n",(0,t.jsx)(i.p,{children:"Then you can use your lifetime like this:"}),"\n",(0,t.jsx)(i.pre,{children:(0,t.jsx)(i.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options.WithLifetime(new CustomLifetime()));\n"})})]})}function f(e={}){const{wrapper:i}={...(0,s.R)(),...e.components};return i?(0,t.jsx)(i,{...e,children:(0,t.jsx)(p,{...e})}):p(e)}},9365:(e,i,n)=>{n.d(i,{A:()=>o});n(6540);var t=n(870);const s={tabItem:"tabItem_Ymn6"};var r=n(4848);function o(e){let{children:i,hidden:n,className:o}=e;return(0,r.jsx)("div",{role:"tabpanel",className:(0,t.A)(s.tabItem,o),hidden:n,children:i})}},1470:(e,i,n)=>{n.d(i,{A:()=>y});var t=n(6540),s=n(870),r=n(3104),o=n(6347),a=n(205),l=n(7485),c=n(1682),d=n(9466);function u(e){return t.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,t.isValidElement)(e)&&function(e){const{props:i}=e;return!!i&&"object"==typeof i&&"value"in i}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function h(e){const{values:i,children:n}=e;return(0,t.useMemo)((()=>{const e=i??function(e){return u(e).map((e=>{let{props:{value:i,label:n,attributes:t,default:s}}=e;return{value:i,label:n,attributes:t,default:s}}))}(n);return function(e){const i=(0,c.X)(e,((e,i)=>e.value===i.value));if(i.length>0)throw new Error(`Docusaurus error: Duplicate values "${i.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[i,n])}function p(e){let{value:i,tabValues:n}=e;return n.some((e=>e.value===i))}function f(e){let{queryString:i=!1,groupId:n}=e;const s=(0,o.W6)(),r=function(e){let{queryString:i=!1,groupId:n}=e;if("string"==typeof i)return i;if(!1===i)return null;if(!0===i&&!n)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return n??null}({queryString:i,groupId:n});return[(0,l.aZ)(r),(0,t.useCallback)((e=>{if(!r)return;const i=new URLSearchParams(s.location.search);i.set(r,e),s.replace({...s.location,search:i.toString()})}),[r,s])]}function m(e){const{defaultValue:i,queryString:n=!1,groupId:s}=e,r=h(e),[o,l]=(0,t.useState)((()=>function(e){let{defaultValue:i,tabValues:n}=e;if(0===n.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(i){if(!p({value:i,tabValues:n}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${i}" but none of its children has the corresponding value. Available values are: ${n.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return i}const t=n.find((e=>e.default))??n[0];if(!t)throw new Error("Unexpected error: 0 tabValues");return t.value}({defaultValue:i,tabValues:r}))),[c,u]=f({queryString:n,groupId:s}),[m,x]=function(e){let{groupId:i}=e;const n=function(e){return e?`docusaurus.tab.${e}`:null}(i),[s,r]=(0,d.Dv)(n);return[s,(0,t.useCallback)((e=>{n&&r.set(e)}),[n,r])]}({groupId:s}),g=(()=>{const e=c??m;return p({value:e,tabValues:r})?e:null})();(0,a.A)((()=>{g&&l(g)}),[g]);return{selectedValue:o,selectValue:(0,t.useCallback)((e=>{if(!p({value:e,tabValues:r}))throw new Error(`Can't select invalid tab value=${e}`);l(e),u(e),x(e)}),[u,x,r]),tabValues:r}}var x=n(2303);const g={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var b=n(4848);function j(e){let{className:i,block:n,selectedValue:t,selectValue:o,tabValues:a}=e;const l=[],{blockElementScrollPositionUntilNextRender:c}=(0,r.a_)(),d=e=>{const i=e.currentTarget,n=l.indexOf(i),s=a[n].value;s!==t&&(c(i),o(s))},u=e=>{let i=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const n=l.indexOf(e.currentTarget)+1;i=l[n]??l[0];break}case"ArrowLeft":{const n=l.indexOf(e.currentTarget)-1;i=l[n]??l[l.length-1];break}}i?.focus()};return(0,b.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,s.A)("tabs",{"tabs--block":n},i),children:a.map((e=>{let{value:i,label:n,attributes:r}=e;return(0,b.jsx)("li",{role:"tab",tabIndex:t===i?0:-1,"aria-selected":t===i,ref:e=>l.push(e),onKeyDown:u,onClick:d,...r,className:(0,s.A)("tabs__item",g.tabItem,r?.className,{"tabs__item--active":t===i}),children:n??i},i)}))})}function v(e){let{lazy:i,children:n,selectedValue:s}=e;const r=(Array.isArray(n)?n:[n]).filter(Boolean);if(i){const e=r.find((e=>e.props.value===s));return e?(0,t.cloneElement)(e,{className:"margin-top--md"}):null}return(0,b.jsx)("div",{className:"margin-top--md",children:r.map(((e,i)=>(0,t.cloneElement)(e,{key:i,hidden:e.props.value!==s})))})}function w(e){const i=m(e);return(0,b.jsxs)("div",{className:(0,s.A)("tabs-container",g.tabList),children:[(0,b.jsx)(j,{...e,...i}),(0,b.jsx)(v,{...e,...i})]})}function y(e){const i=(0,x.A)();return(0,b.jsx)(w,{...e,children:u(e.children)},String(i))}},7470:(e,i,n)=>{n.d(i,{A:()=>o});var t=n(6540);const s={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var r=n(4848);function o(e){let{children:i}=e,n=t.Children.toArray(i).filter((e=>e));return(0,r.jsxs)("div",{className:s.codeDescContainer,children:[(0,r.jsx)("div",{className:s.desc,children:n[0]}),(0,r.jsx)("div",{className:s.example,children:n[1]})]})}},8453:(e,i,n)=>{n.d(i,{R:()=>o,x:()=>a});var t=n(6540);const s={},r=t.createContext(s);function o(e){const i=t.useContext(r);return t.useMemo((function(){return"function"==typeof e?e(i):{...i,...e}}),[i,e])}function a(e){let i;return i=e.disableParentContext?"function"==typeof e.components?e.components(s):e.components||s:o(e.components),t.createElement(r.Provider,{value:i},e.children)}}}]);