"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[495],{1267:(e,n,s)=>{s.r(n),s.d(n,{assets:()=>u,contentTitle:()=>l,default:()=>b,frontMatter:()=>c,metadata:()=>d,toc:()=>h});var t=s(4848),i=s(8453),r=s(7470),a=s(1470),o=s(9365);const c={},l="Basic usage",d={id:"guides/basics",title:"Basic usage",description:"This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections.",source:"@site/docs/guides/basics.md",sourceDirName:"guides",slug:"/guides/basics",permalink:"/stashbox/docs/guides/basics",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/basics.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1729417558,formattedLastUpdatedAt:"Oct 20, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Glossary",permalink:"/stashbox/docs/getting-started/glossary"},next:{title:"Advanced registration",permalink:"/stashbox/docs/guides/advanced-registration"}},u={},h=[{value:"Default registration",id:"default-registration",level:2},{value:"Named registration",id:"named-registration",level:2},{value:"Instance registration",id:"instance-registration",level:2},{value:"Re-mapping",id:"re-mapping",level:2},{value:"Wiring up",id:"wiring-up",level:2},{value:"Lifetime shortcuts",id:"lifetime-shortcuts",level:2}];function p(e){const n={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",p:"p",pre:"pre",...(0,i.R)(),...e.components};return(0,t.jsxs)(t.Fragment,{children:[(0,t.jsx)(n.h1,{id:"basic-usage",children:"Basic usage"}),"\n",(0,t.jsx)(n.p,{children:"This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections.\nStashbox provides several methods that enable registering services, and we'll go through the most common scenarios with code examples."}),"\n",(0,t.jsx)(n.h2,{id:"default-registration",children:"Default registration"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["Stashbox allows registration operations via the ",(0,t.jsx)(n.code,{children:"Register()"})," methods."]}),(0,t.jsxs)(n.p,{children:["During registration, the container checks whether the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," is assignable from the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"implementation type"})," and if not, the container throws an ",(0,t.jsx)(n.a,{href:"/docs/diagnostics/validation#registration-validation",children:"exception"}),"."]}),(0,t.jsxs)(n.p,{children:["Also, when the implementation is not resolvable, the container throws the same ",(0,t.jsx)(n.a,{href:"/docs/diagnostics/validation#registration-validation",children:"exception"}),"."]}),(0,t.jsxs)(n.p,{children:["The example registers ",(0,t.jsx)(n.code,{children:"DbBackup"})," to be returned when ",(0,t.jsx)(n.code,{children:"IJob"})," is requested."]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(a.A,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(o.A,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n// throws an exception because ConsoleLogger doesn't implement IJob.\ncontainer.Register<IJob, ConsoleLogger>();\n// throws an exception because IJob is not a valid implementation.\ncontainer.Register<IJob, IJob>();\n"})})}),(0,t.jsx)(o.A,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register(typeof(IJob), typeof(DbBackup));\nobject job = container.Resolve(typeof(IJob));\n// throws an exception because ConsoleLogger doesn't implement IJob.\ncontainer.Register(typeof(IJob), typeof(ConsoleLogger));\n// throws an exception because IJob is not a valid implementation.\ncontainer.Register(typeof(IJob), typeof(IJob));\n"})})})]})})]}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["You can register a service to itself without specifying a ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),", only the implementation (",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#self-registration",children:"self registration"}),")."]}),(0,t.jsxs)(n.p,{children:["In this case, the given implementation is considered the ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," and must be used to request the service (",(0,t.jsx)(n.code,{children:"DbBackup"})," in the example)."]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(a.A,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(o.A,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<DbBackup>();\nDbBackup backup = container.Resolve<DbBackup>();\n"})})}),(0,t.jsx)(o.A,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register(typeof(DbBackup));\nobject backup = container.Resolve(typeof(DbBackup));\n"})})})]})})]}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(n.p,{children:"The container's API is fluent, which means you can chain the calls on its methods after each other."})}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"var job = container.Register<IJob, DbBackup>()\n    .Register<ILogger, ConsoleLogger>()\n    .Resolve<IJob>();\n"})})})]}),"\n",(0,t.jsx)(n.h2,{id:"named-registration",children:"Named registration"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["The example shows how you can bind more implementations to a ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," using names for identification."]}),(0,t.jsx)(n.p,{children:"The same name must be used to resolve the named service."}),(0,t.jsx)(n.admonition,{type:"note",children:(0,t.jsxs)(n.p,{children:["The name is an ",(0,t.jsx)(n.code,{children:"object"})," type."]})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(a.A,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(o.A,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>("DbBackup");\ncontainer.Register<IJob, StorageCleanup>("StorageCleanup");\nIJob cleanup = container.Resolve<IJob>("StorageCleanup");\n'})})}),(0,t.jsx)(o.A,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.Register(typeof(IJob), typeof(DbBackup), "DbBackup");\ncontainer.Register(typeof(IJob), typeof(StorageCleanup), "StorageCleanup");\nobject cleanup = container.Resolve(typeof(IJob), "StorageCleanup");\n'})})})]})})]}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsxs)(n.p,{children:["You can also get each service that share the same name by requesting an ",(0,t.jsx)(n.code,{children:"IEnumerable<>"})," or using the ",(0,t.jsx)(n.code,{children:"ResolveAll()"})," method with the ",(0,t.jsx)(n.code,{children:"name"})," parameter."]})}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>("StorageJobs");\ncontainer.Register<IJob, StorageCleanup>("StorageJobs");\ncontainer.Register<IJob, AnotherJob>();\n// jobs will be [DbBackup, StorageCleanup].\nIEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>("StorageJobs");\n'})})})]}),"\n",(0,t.jsx)(n.h2,{id:"instance-registration",children:"Instance registration"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["With instance registration, you can provide an already created external instance to use when the given ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," is requested."]}),(0,t.jsxs)(n.p,{children:["Stashbox automatically handles the ",(0,t.jsx)(n.a,{href:"/docs/guides/scopes#disposal",children:"disposal"})," of the registered instances, but you can turn this feature off with the ",(0,t.jsx)(n.code,{children:"withoutDisposalTracking"})," parameter."]}),(0,t.jsxs)(n.p,{children:["When an ",(0,t.jsx)(n.code,{children:"IJob"})," is requested, the container will always return the external instance."]})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(a.A,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(o.A,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job);\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>();\n"})})}),(0,t.jsx)(o.A,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"var job = new DbBackup();\ncontainer.RegisterInstance(job, typeof(IJob));\n\n// resolvedJob and job are the same.\nobject resolvedJob = container.Resolve(typeof(IJob));\n"})})}),(0,t.jsx)(o.A,{value:"Named",label:"Named",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job, "DbBackup");\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>("DbBackup");\n'})})}),(0,t.jsx)(o.A,{value:"No dispose",label:"No dispose",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job, withoutDisposalTracking: true);\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>();\n"})})})]})})]}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsx)(n.p,{children:"The instance registration API allows the batched registration of different instances."})}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterInstances<IJob>(new DbBackup(), new StorageCleanup());\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n"})})})]}),"\n",(0,t.jsx)(n.h2,{id:"re-mapping",children:"Re-mapping"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsxs)(n.p,{children:["With re-map, you can bind new implementations to a ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," and delete old registrations in one action."]}),(0,t.jsx)(n.admonition,{type:"caution",children:(0,t.jsxs)(n.p,{children:["When there are multiple registrations mapped to a ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),", ",(0,t.jsx)(n.code,{children:".ReMap()"})," will replace all of them with the given ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"implementation type"}),". If you want to replace only one specific service, use the ",(0,t.jsx)(n.code,{children:".ReplaceExisting()"})," ",(0,t.jsx)(n.a,{href:"/docs/configuration/registration-configuration#replace",children:"configuration option"}),"."]})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(a.A,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(o.A,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>();\ncontainer.ReMap<IJob, StorageCleanup>();\n// jobs contain all two jobs\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n\ncontainer.ReMap<IJob, SlackMessageSender>();\n// jobs contains only the SlackMessageSender\njobs = container.ResolveAll<IJob>();\n"})})}),(0,t.jsx)(o.A,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register(typeof(IJob), typeof(DbBackup));\ncontainer.Register(typeof(IJob), typeof(StorageCleanup));\n// jobs contain all two jobs\nIEnumerable<object> jobs = container.ResolveAll(typeof(IJob));\n\ncontainer.ReMap(typeof(IJob), typeof(SlackMessageSender));\n// jobs contains only the SlackMessageSender\njobs = container.ResolveAll(typeof(IJob));\n"})})})]})})]}),"\n",(0,t.jsx)(n.h2,{id:"wiring-up",children:"Wiring up"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsx)("div",{children:(0,t.jsxs)(n.p,{children:["Wiring up is similar to ",(0,t.jsx)(n.a,{href:"#instance-registration",children:"Instance registration"})," except that the container will perform property/field injection (if configured so and applicable) on the registered instance during resolution."]})}),(0,t.jsx)("div",{children:(0,t.jsxs)(a.A,{groupId:"generic-runtime-apis",children:[(0,t.jsx)(o.A,{value:"Generic API",label:"Generic API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.WireUp<IJob>(new DbBackup());\nIJob job = container.Resolve<IJob>();\n"})})}),(0,t.jsx)(o.A,{value:"Runtime type API",label:"Runtime type API",children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.WireUp(new DbBackup(), typeof(IJob));\nobject job = container.Resolve(typeof(IJob));\n"})})})]})})]}),"\n",(0,t.jsx)(n.h2,{id:"lifetime-shortcuts",children:"Lifetime shortcuts"}),"\n",(0,t.jsxs)(r.A,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"A service's lifetime indicates how long its instance will live and which re-using policy should be applied when it gets injected."}),(0,t.jsxs)(n.p,{children:["This example shows how you can use the registration API's shortcuts for lifetimes. These are just sugars, and there are more ways explained in the ",(0,t.jsx)(n.a,{href:"/docs/guides/lifetimes",children:"lifetimes"})," section."]}),(0,t.jsx)(n.admonition,{type:"info",children:(0,t.jsxs)(n.p,{children:["The ",(0,t.jsx)(n.code,{children:"DefaultLifetime"})," is ",(0,t.jsx)(n.a,{href:"/docs/guides/lifetimes#default-lifetime",children:"configurable"}),"."]})})]}),(0,t.jsx)("div",{children:(0,t.jsxs)(a.A,{groupId:"generic-runtime-apis",children:[(0,t.jsxs)(o.A,{value:"Default",label:"Default",children:[(0,t.jsxs)(n.p,{children:["When no lifetime is specified, the service will use the container's ",(0,t.jsx)(n.code,{children:"DefaultLifetime"}),", which is ",(0,t.jsx)(n.code,{children:"Transient"})," by default."]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n"})})]}),(0,t.jsxs)(o.A,{value:"Singleton",label:"Singleton",children:[(0,t.jsxs)(n.p,{children:["A service with ",(0,t.jsx)(n.code,{children:"Singleton"})," lifetime will be instantiated once and reused during the container's lifetime."]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterSingleton<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n"})})]}),(0,t.jsxs)(o.A,{value:"Scoped",label:"Scoped",children:[(0,t.jsxs)(n.p,{children:["The ",(0,t.jsx)(n.code,{children:"Scoped"})," lifetime behaves like a ",(0,t.jsx)(n.code,{children:"Singleton"})," within a ",(0,t.jsx)(n.a,{href:"/docs/guides/scopes",children:"scope"}),".\nA scoped service is instantiated once and reused during the scope's whole lifetime."]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.RegisterScoped<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n"})})]})]})})]})]})}function b(e={}){const{wrapper:n}={...(0,i.R)(),...e.components};return n?(0,t.jsx)(n,{...e,children:(0,t.jsx)(p,{...e})}):p(e)}},9365:(e,n,s)=>{s.d(n,{A:()=>a});s(6540);var t=s(870);const i={tabItem:"tabItem_Ymn6"};var r=s(4848);function a(e){let{children:n,hidden:s,className:a}=e;return(0,r.jsx)("div",{role:"tabpanel",className:(0,t.A)(i.tabItem,a),hidden:s,children:n})}},1470:(e,n,s)=>{s.d(n,{A:()=>y});var t=s(6540),i=s(870),r=s(3104),a=s(6347),o=s(205),c=s(7485),l=s(1682),d=s(9466);function u(e){return t.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,t.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function h(e){const{values:n,children:s}=e;return(0,t.useMemo)((()=>{const e=n??function(e){return u(e).map((e=>{let{props:{value:n,label:s,attributes:t,default:i}}=e;return{value:n,label:s,attributes:t,default:i}}))}(s);return function(e){const n=(0,l.X)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,s])}function p(e){let{value:n,tabValues:s}=e;return s.some((e=>e.value===n))}function b(e){let{queryString:n=!1,groupId:s}=e;const i=(0,a.W6)(),r=function(e){let{queryString:n=!1,groupId:s}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!s)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return s??null}({queryString:n,groupId:s});return[(0,c.aZ)(r),(0,t.useCallback)((e=>{if(!r)return;const n=new URLSearchParams(i.location.search);n.set(r,e),i.replace({...i.location,search:n.toString()})}),[r,i])]}function g(e){const{defaultValue:n,queryString:s=!1,groupId:i}=e,r=h(e),[a,c]=(0,t.useState)((()=>function(e){let{defaultValue:n,tabValues:s}=e;if(0===s.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!p({value:n,tabValues:s}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${s.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const t=s.find((e=>e.default))??s[0];if(!t)throw new Error("Unexpected error: 0 tabValues");return t.value}({defaultValue:n,tabValues:r}))),[l,u]=b({queryString:s,groupId:i}),[g,m]=function(e){let{groupId:n}=e;const s=function(e){return e?`docusaurus.tab.${e}`:null}(n),[i,r]=(0,d.Dv)(s);return[i,(0,t.useCallback)((e=>{s&&r.set(e)}),[s,r])]}({groupId:i}),j=(()=>{const e=l??g;return p({value:e,tabValues:r})?e:null})();(0,o.A)((()=>{j&&c(j)}),[j]);return{selectedValue:a,selectValue:(0,t.useCallback)((e=>{if(!p({value:e,tabValues:r}))throw new Error(`Can't select invalid tab value=${e}`);c(e),u(e),m(e)}),[u,m,r]),tabValues:r}}var m=s(2303);const j={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var x=s(4848);function v(e){let{className:n,block:s,selectedValue:t,selectValue:a,tabValues:o}=e;const c=[],{blockElementScrollPositionUntilNextRender:l}=(0,r.a_)(),d=e=>{const n=e.currentTarget,s=c.indexOf(n),i=o[s].value;i!==t&&(l(n),a(i))},u=e=>{let n=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const s=c.indexOf(e.currentTarget)+1;n=c[s]??c[0];break}case"ArrowLeft":{const s=c.indexOf(e.currentTarget)-1;n=c[s]??c[c.length-1];break}}n?.focus()};return(0,x.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,i.A)("tabs",{"tabs--block":s},n),children:o.map((e=>{let{value:n,label:s,attributes:r}=e;return(0,x.jsx)("li",{role:"tab",tabIndex:t===n?0:-1,"aria-selected":t===n,ref:e=>c.push(e),onKeyDown:u,onClick:d,...r,className:(0,i.A)("tabs__item",j.tabItem,r?.className,{"tabs__item--active":t===n}),children:s??n},n)}))})}function f(e){let{lazy:n,children:s,selectedValue:i}=e;const r=(Array.isArray(s)?s:[s]).filter(Boolean);if(n){const e=r.find((e=>e.props.value===i));return e?(0,t.cloneElement)(e,{className:"margin-top--md"}):null}return(0,x.jsx)("div",{className:"margin-top--md",children:r.map(((e,n)=>(0,t.cloneElement)(e,{key:n,hidden:e.props.value!==i})))})}function I(e){const n=g(e);return(0,x.jsxs)("div",{className:(0,i.A)("tabs-container",j.tabList),children:[(0,x.jsx)(v,{...e,...n}),(0,x.jsx)(f,{...e,...n})]})}function y(e){const n=(0,m.A)();return(0,x.jsx)(I,{...e,children:u(e.children)},String(n))}},7470:(e,n,s)=>{s.d(n,{A:()=>a});var t=s(6540);const i={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var r=s(4848);function a(e){let{children:n}=e,s=t.Children.toArray(n).filter((e=>e));return(0,r.jsxs)("div",{className:i.codeDescContainer,children:[(0,r.jsx)("div",{className:i.desc,children:s[0]}),(0,r.jsx)("div",{className:i.example,children:s[1]})]})}},8453:(e,n,s)=>{s.d(n,{R:()=>a,x:()=>o});var t=s(6540);const i={},r=t.createContext(i);function a(e){const n=t.useContext(r);return t.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function o(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(i):e.components||i:a(e.components),t.createElement(r.Provider,{value:n},e.children)}}}]);