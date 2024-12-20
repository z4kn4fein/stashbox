"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[289],{8225:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>d,contentTitle:()=>c,default:()=>p,frontMatter:()=>i,metadata:()=>l,toc:()=>u});var a=t(4848),r=t(8453),s=t(1470),o=t(9365);const i={title:"Introduction"},c=void 0,l={id:"getting-started/introduction",title:"Introduction",description:"Stashbox and its extensions are distributed via NuGet packages.",source:"@site/docs/getting-started/introduction.md",sourceDirName:"getting-started",slug:"/getting-started/introduction",permalink:"/stashbox/docs/getting-started/introduction",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/getting-started/introduction.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1734704927,formattedLastUpdatedAt:"Dec 20, 2024",frontMatter:{title:"Introduction"},sidebar:"docs",previous:{title:"Overview",permalink:"/stashbox/docs/getting-started/overview"},next:{title:"Glossary",permalink:"/stashbox/docs/getting-started/glossary"}},d={},u=[{value:"Usage",id:"usage",level:2},{value:"How it works?",id:"how-it-works",level:2},{value:"Example",id:"example",level:2}];function h(e){const n={a:"a",admonition:"admonition",code:"code",em:"em",h2:"h2",li:"li",p:"p",pre:"pre",ul:"ul",...(0,r.R)(),...e.components};return(0,a.jsxs)(a.Fragment,{children:[(0,a.jsxs)(n.p,{children:["Stashbox and its extensions are distributed via ",(0,a.jsx)(n.a,{href:"https://www.nuget.org/packages?q=stashbox",children:"NuGet"})," packages."]}),"\n",(0,a.jsxs)(s.A,{children:[(0,a.jsxs)(o.A,{value:"Package Manager",label:"Package Manager",children:[(0,a.jsx)(n.p,{children:"You can install the package by typing the following into the Package Manager Console:"}),(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-powershell",children:"Install-Package Stashbox -Version 5.17.0\n"})})]}),(0,a.jsxs)(o.A,{value:"dotnet CLI",label:"dotnet CLI",children:[(0,a.jsx)(n.p,{children:"You can install the package by using the dotnet cli:"}),(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-bash",children:"dotnet add package Stashbox --version 5.17.0\n"})})]}),(0,a.jsxs)(o.A,{value:"PackageReference",label:"PackageReference",children:[(0,a.jsxs)(n.p,{children:["You can add the package into the package references of your ",(0,a.jsx)(n.code,{children:".csproj"}),":"]}),(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-xml",children:'<PackageReference Include="Stashbox" Version="5.17.0" />\n'})})]})]}),"\n",(0,a.jsx)(n.h2,{id:"usage",children:"Usage"}),"\n",(0,a.jsxs)(n.p,{children:["The general idea behind using Stashbox is that you structure your code with loosely coupled components with the ",(0,a.jsx)(n.a,{href:"https://en.wikipedia.org/wiki/Dependency_inversion_principle",children:"Dependency Inversion Principle"}),", ",(0,a.jsx)(n.a,{href:"https://en.wikipedia.org/wiki/Inversion_of_control",children:"Inversion Of Control"})," and ",(0,a.jsx)(n.a,{href:"https://martinfowler.com/articles/injection.html",children:"Dependency Injection"})," in mind."]}),"\n",(0,a.jsx)(n.p,{children:"Rather than letting the services instantiate their dependencies inside themselves, inject the dependencies on construction. Also, rather than creating the object hierarchy manually, you can use a Dependency Injection framework that does the work for you. That's why you are here, I suppose. \ud83d\ude42"}),"\n",(0,a.jsx)(n.p,{children:"To achieve the most efficient usage of Stashbox, you should follow these steps:"}),"\n",(0,a.jsxs)(n.ul,{children:["\n",(0,a.jsxs)(n.li,{children:["At the startup of your application, instantiate a ",(0,a.jsx)(n.code,{children:"StashboxContainer"}),"."]}),"\n",(0,a.jsx)(n.li,{children:"Register your services into the container."}),"\n",(0,a.jsxs)(n.li,{children:[(0,a.jsx)(n.a,{href:"/docs/diagnostics/validation",children:"Validate"})," the state of the container and the registrations with the ",(0,a.jsx)(n.code,{children:".Validate()"})," method. ",(0,a.jsx)(n.em,{children:"(Optional)"})]}),"\n",(0,a.jsx)(n.li,{children:"During the lifetime of the application, use the container to resolve your services."}),"\n",(0,a.jsxs)(n.li,{children:["Create ",(0,a.jsx)(n.a,{href:"/docs/guides/scopes",children:"scopes"})," and use them to resolve your services. ",(0,a.jsx)(n.em,{children:"(Optional)"})]}),"\n",(0,a.jsxs)(n.li,{children:["On application exit, call the container's ",(0,a.jsx)(n.code,{children:".Dispose()"})," or ",(0,a.jsx)(n.code,{children:".DisposeAsync()"})," method to clean up the resources. ",(0,a.jsx)(n.em,{children:"(Optional)"})]}),"\n"]}),"\n",(0,a.jsx)(n.admonition,{type:"caution",children:(0,a.jsxs)(n.p,{children:["You should create only a single instance from ",(0,a.jsx)(n.code,{children:"StashboxContainer"})," (plus child containers if you use them) per application domain. ",(0,a.jsx)(n.code,{children:"StashboxContainer"})," instances are thread-safe. Do not create new container instances continuously, such action will bypass the container's internal delegate cache and could lead to performance degradation."]})}),"\n",(0,a.jsx)(n.h2,{id:"how-it-works",children:"How it works?"}),"\n",(0,a.jsxs)(n.p,{children:["Stashbox builds and maintains a collection of ",(0,a.jsx)(n.a,{href:"/docs/getting-started/glossary#service-registration--registered-service",children:"registered services"}),". When a service is requested for resolution, Stashbox starts looking for a matching registration that has the same ",(0,a.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," as the type that was requested. If it finds one, the container initiates a scan on the ",(0,a.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"implementation type's"})," available constructors and selects the one with the most arguments it knows how to resolve by matching argument types to other registrations."]}),"\n",(0,a.jsx)(n.p,{children:"When every constructor argument has a companion registration, Stashbox jumps to the first one and continues the same scanning operation."}),"\n",(0,a.jsxs)(n.p,{children:["This process is repeated until every ",(0,a.jsx)(n.a,{href:"/docs/getting-started/glossary#injectable-dependency",children:"injectable dependency"})," has a matching registration in the ",(0,a.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree"}),". At the end of the process, Stashbox will have each dependency node built-up in a hierarchical object structure to instantiate the initially requested service object."]}),"\n",(0,a.jsx)(n.h2,{id:"example",children:"Example"}),"\n",(0,a.jsxs)(n.p,{children:["Let's see a quick example. We have three services ",(0,a.jsx)(n.code,{children:"DbBackup"}),", ",(0,a.jsx)(n.code,{children:"MessageBus"})," and ",(0,a.jsx)(n.code,{children:"ConsoleLogger"}),". ",(0,a.jsx)(n.code,{children:"DbBackup"})," has a dependency on ",(0,a.jsx)(n.code,{children:"IEventBroadcaster"})," (implemented by ",(0,a.jsx)(n.code,{children:"MessageBus"}),") and ",(0,a.jsx)(n.code,{children:"ILogger"})," (implemented by ",(0,a.jsx)(n.code,{children:"ConsoleLogger"}),"), ",(0,a.jsx)(n.code,{children:"MessageBus"})," also depending on an ",(0,a.jsx)(n.code,{children:"ILogger"}),":"]}),"\n",(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-cs",children:'public interface IJob { void DoTheJob(); }\n\npublic interface ILogger { void Log(string message); }\n\npublic interface IEventBroadcaster { void Broadcast(IEvent @event); }\n\n\npublic class ConsoleLogger : ILogger\n{\n    public void Log(string message) => Console.WriteLine(message);\n}\n\npublic class MessageBus : IEventBroadcaster\n{\n    private readonly ILogger logger;\n\n    public MessageBus(ILogger logger)\n    {\n        this.logger = logger;\n    }\n\n    void Broadcast(IEvent @event) \n    {\n        this.logger.Log($"Sending event to bus: {@event.Name}");\n        // Do the actual event broadcast.\n    }\n}\n\npublic class DbBackup : IJob\n{\n    private readonly ILogger logger;\n    private readonly IEventBroadcaster eventBroadcaster;\n\n    public DbBackup(ILogger logger, IEventBroadcaster eventBroadcaster)\n    {\n        this.logger = logger;\n        this.eventBroadcaster = eventBroadcaster;\n    }\n\n    public void DoTheJob() \n    {\n        this.logger.Log("Backing up!");\n        // Do the actual backup.\n        this.eventBroadcaster.Broadcast(new DbBackupCompleted());\n    } \n}\n'})}),"\n",(0,a.jsx)(n.admonition,{type:"info",children:(0,a.jsx)(n.p,{children:"By depending only on interfaces, you decouple your services from concrete implementations. This gives you the flexibility of a more comfortable implementation replacement and also isolates your components from each other. For example, unit testing benefits a lot from the possibility of replacing real implementations with mocks."})}),"\n",(0,a.jsx)(n.p,{children:"The example above configured with Stashbox in a Console Application:"}),"\n",(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-cs",children:"using Stashbox;\nusing System;\n\nnamespace Example\n{\n    public class Program\n    {\n        private static readonly IStashboxContainer container;\n\n        static Program()\n        {\n            // 1. Create container\n            container = new StashboxContainer();\n\n            // 2. Register your services\n            container.RegisterSingleton<ILogger, ConsoleLogger>();\n            container.Register<IEventBroadcaster, MessageBus>();\n            container.Register<IJob, DbBackup>();\n\n            // 3. Validate the configuration.\n            container.Validate();\n        }\n\n        static void Main(string[] args)\n        {\n            // 4. Resolve and use your service\n            var job = container.Resolve<IJob>();\n            job.DoTheJob();\n        }\n    }\n}\n"})})]})}function p(e={}){const{wrapper:n}={...(0,r.R)(),...e.components};return n?(0,a.jsx)(n,{...e,children:(0,a.jsx)(h,{...e})}):h(e)}},9365:(e,n,t)=>{t.d(n,{A:()=>o});t(6540);var a=t(870);const r={tabItem:"tabItem_Ymn6"};var s=t(4848);function o(e){let{children:n,hidden:t,className:o}=e;return(0,s.jsx)("div",{role:"tabpanel",className:(0,a.A)(r.tabItem,o),hidden:t,children:n})}},1470:(e,n,t)=>{t.d(n,{A:()=>w});var a=t(6540),r=t(870),s=t(3104),o=t(6347),i=t(205),c=t(7485),l=t(1682),d=t(9466);function u(e){return a.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,a.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function h(e){const{values:n,children:t}=e;return(0,a.useMemo)((()=>{const e=n??function(e){return u(e).map((e=>{let{props:{value:n,label:t,attributes:a,default:r}}=e;return{value:n,label:t,attributes:a,default:r}}))}(t);return function(e){const n=(0,l.X)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,t])}function p(e){let{value:n,tabValues:t}=e;return t.some((e=>e.value===n))}function g(e){let{queryString:n=!1,groupId:t}=e;const r=(0,o.W6)(),s=function(e){let{queryString:n=!1,groupId:t}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!t)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return t??null}({queryString:n,groupId:t});return[(0,c.aZ)(s),(0,a.useCallback)((e=>{if(!s)return;const n=new URLSearchParams(r.location.search);n.set(s,e),r.replace({...r.location,search:n.toString()})}),[s,r])]}function m(e){const{defaultValue:n,queryString:t=!1,groupId:r}=e,s=h(e),[o,c]=(0,a.useState)((()=>function(e){let{defaultValue:n,tabValues:t}=e;if(0===t.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!p({value:n,tabValues:t}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${t.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const a=t.find((e=>e.default))??t[0];if(!a)throw new Error("Unexpected error: 0 tabValues");return a.value}({defaultValue:n,tabValues:s}))),[l,u]=g({queryString:t,groupId:r}),[m,b]=function(e){let{groupId:n}=e;const t=function(e){return e?`docusaurus.tab.${e}`:null}(n),[r,s]=(0,d.Dv)(t);return[r,(0,a.useCallback)((e=>{t&&s.set(e)}),[t,s])]}({groupId:r}),f=(()=>{const e=l??m;return p({value:e,tabValues:s})?e:null})();(0,i.A)((()=>{f&&c(f)}),[f]);return{selectedValue:o,selectValue:(0,a.useCallback)((e=>{if(!p({value:e,tabValues:s}))throw new Error(`Can't select invalid tab value=${e}`);c(e),u(e),b(e)}),[u,b,s]),tabValues:s}}var b=t(2303);const f={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var v=t(4848);function x(e){let{className:n,block:t,selectedValue:a,selectValue:o,tabValues:i}=e;const c=[],{blockElementScrollPositionUntilNextRender:l}=(0,s.a_)(),d=e=>{const n=e.currentTarget,t=c.indexOf(n),r=i[t].value;r!==a&&(l(n),o(r))},u=e=>{let n=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const t=c.indexOf(e.currentTarget)+1;n=c[t]??c[0];break}case"ArrowLeft":{const t=c.indexOf(e.currentTarget)-1;n=c[t]??c[c.length-1];break}}n?.focus()};return(0,v.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,r.A)("tabs",{"tabs--block":t},n),children:i.map((e=>{let{value:n,label:t,attributes:s}=e;return(0,v.jsx)("li",{role:"tab",tabIndex:a===n?0:-1,"aria-selected":a===n,ref:e=>c.push(e),onKeyDown:u,onClick:d,...s,className:(0,r.A)("tabs__item",f.tabItem,s?.className,{"tabs__item--active":a===n}),children:t??n},n)}))})}function j(e){let{lazy:n,children:t,selectedValue:r}=e;const s=(Array.isArray(t)?t:[t]).filter(Boolean);if(n){const e=s.find((e=>e.props.value===r));return e?(0,a.cloneElement)(e,{className:"margin-top--md"}):null}return(0,v.jsx)("div",{className:"margin-top--md",children:s.map(((e,n)=>(0,a.cloneElement)(e,{key:n,hidden:e.props.value!==r})))})}function y(e){const n=m(e);return(0,v.jsxs)("div",{className:(0,r.A)("tabs-container",f.tabList),children:[(0,v.jsx)(x,{...e,...n}),(0,v.jsx)(j,{...e,...n})]})}function w(e){const n=(0,b.A)();return(0,v.jsx)(y,{...e,children:u(e.children)},String(n))}},8453:(e,n,t)=>{t.d(n,{R:()=>o,x:()=>i});var a=t(6540);const r={},s=a.createContext(r);function o(e){const n=a.useContext(s);return a.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function i(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(r):e.components||r:o(e.components),a.createElement(s.Provider,{value:n},e.children)}}}]);