"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[353],{3905:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>g});var a=n(7294);function r(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){r(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,a,r=function(e,t){if(null==e)return{};var n,a,r={},o=Object.keys(e);for(a=0;a<o.length;a++)n=o[a],t.indexOf(n)>=0||(r[n]=e[n]);return r}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(a=0;a<o.length;a++)n=o[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(r[n]=e[n])}return r}var l=a.createContext({}),c=function(e){var t=a.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},p=function(e){var t=c(e.components);return a.createElement(l.Provider,{value:t},e.children)},u="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},m=a.forwardRef((function(e,t){var n=e.components,r=e.mdxType,o=e.originalType,l=e.parentName,p=s(e,["components","mdxType","originalType","parentName"]),u=c(n),m=r,g=u["".concat(l,".").concat(m)]||u[m]||d[m]||o;return n?a.createElement(g,i(i({ref:t},p),{},{components:n})):a.createElement(g,i({ref:t},p))}));function g(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var o=n.length,i=new Array(o);i[0]=m;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s[u]="string"==typeof e?e:r,i[1]=s;for(var c=2;c<o;c++)i[c]=n[c];return a.createElement.apply(null,i)}return a.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>i});var a=n(7294),r=n(6010);const o="tabItem_Ymn6";function i(e){let{children:t,hidden:n,className:i}=e;return a.createElement("div",{role:"tabpanel",className:(0,r.Z)(o,i),hidden:n},t)}},5488:(e,t,n)=>{n.d(t,{Z:()=>m});var a=n(7462),r=n(7294),o=n(6010),i=n(2389),s=n(7392),l=n(7094),c=n(2466);const p="tabList__CuJ",u="tabItem_LNqP";function d(e){const{lazy:t,block:n,defaultValue:i,values:d,groupId:m,className:g}=e,h=r.Children.map(e.children,(e=>{if((0,r.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),b=d??h.map((e=>{let{props:{value:t,label:n,attributes:a}}=e;return{value:t,label:n,attributes:a}})),v=(0,s.l)(b,((e,t)=>e.value===t.value));if(v.length>0)throw new Error(`Docusaurus error: Duplicate values "${v.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const k=null===i?i:i??h.find((e=>e.props.default))?.props.value??h[0].props.value;if(null!==k&&!b.some((e=>e.value===k)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${k}" but none of its children has the corresponding value. Available values are: ${b.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:f,setTabGroupChoices:y}=(0,l.U)(),[N,w]=(0,r.useState)(k),x=[],{blockElementScrollPositionUntilNextRender:C}=(0,c.o5)();if(null!=m){const e=f[m];null!=e&&e!==N&&b.some((t=>t.value===e))&&w(e)}const I=e=>{const t=e.currentTarget,n=x.indexOf(t),a=b[n].value;a!==N&&(C(t),w(a),null!=m&&y(m,String(a)))},T=e=>{let t=null;switch(e.key){case"Enter":I(e);break;case"ArrowRight":{const n=x.indexOf(e.currentTarget)+1;t=x[n]??x[0];break}case"ArrowLeft":{const n=x.indexOf(e.currentTarget)-1;t=x[n]??x[x.length-1];break}}t?.focus()};return r.createElement("div",{className:(0,o.Z)("tabs-container",p)},r.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":n},g)},b.map((e=>{let{value:t,label:n,attributes:i}=e;return r.createElement("li",(0,a.Z)({role:"tab",tabIndex:N===t?0:-1,"aria-selected":N===t,key:t,ref:e=>x.push(e),onKeyDown:T,onClick:I},i,{className:(0,o.Z)("tabs__item",u,i?.className,{"tabs__item--active":N===t})}),n??t)}))),t?(0,r.cloneElement)(h.filter((e=>e.props.value===N))[0],{className:"margin-top--md"}):r.createElement("div",{className:"margin-top--md"},h.map(((e,t)=>(0,r.cloneElement)(e,{key:t,hidden:e.props.value!==N})))))}function m(e){const t=(0,i.Z)();return r.createElement(d,(0,a.Z)({key:String(t)},e))}},5007:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>p,contentTitle:()=>l,default:()=>m,frontMatter:()=>s,metadata:()=>c,toc:()=>u});var a=n(7462),r=(n(7294),n(3905)),o=n(5488),i=n(5162);const s={title:"Introduction"},l=void 0,c={unversionedId:"getting-started/introduction",id:"getting-started/introduction",title:"Introduction",description:"Stashbox and its extensions are distributed via NuGet packages.",source:"@site/docs/getting-started/introduction.md",sourceDirName:"getting-started",slug:"/getting-started/introduction",permalink:"/stashbox/docs/getting-started/introduction",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/getting-started/introduction.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1673344472,formattedLastUpdatedAt:"Jan 10, 2023",frontMatter:{title:"Introduction"},sidebar:"docs",previous:{title:"Overview",permalink:"/stashbox/docs/getting-started/overview"},next:{title:"Glossary",permalink:"/stashbox/docs/getting-started/glossary"}},p={},u=[{value:"Usage",id:"usage",level:2},{value:"How it works?",id:"how-it-works",level:2},{value:"Example",id:"example",level:2}],d={toc:u};function m(e){let{components:t,...n}=e;return(0,r.kt)("wrapper",(0,a.Z)({},d,n,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("p",null,"Stashbox and its extensions are distributed via ",(0,r.kt)("a",{parentName:"p",href:"https://www.nuget.org/packages?q=stashbox"},"NuGet")," packages."),(0,r.kt)(o.Z,{mdxType:"Tabs"},(0,r.kt)(i.Z,{value:"Package Manager",label:"Package Manager",mdxType:"TabItem"},(0,r.kt)("p",null,"You can install the package by typing the following into the Package Manager Console:"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-powershell"},"Install-Package Stashbox -Version 5.7.0\n"))),(0,r.kt)(i.Z,{value:"dotnet CLI",label:"dotnet CLI",mdxType:"TabItem"},(0,r.kt)("p",null,"You can install the package by using the dotnet cli:"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-bash"},"dotnet add package Stashbox --version 5.7.0\n"))),(0,r.kt)(i.Z,{value:"PackageReference",label:"PackageReference",mdxType:"TabItem"},(0,r.kt)("p",null,"You can add the package into the package references of your ",(0,r.kt)("inlineCode",{parentName:"p"},".csproj"),":"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-xml"},'<PackageReference Include="Stashbox" Version="5.7.0" />\n')))),(0,r.kt)("h2",{id:"usage"},"Usage"),(0,r.kt)("p",null,"The general idea behind using Stashbox is that you structure your code from loosely coupled components with the ",(0,r.kt)("a",{parentName:"p",href:"https://en.wikipedia.org/wiki/Dependency_inversion_principle"},"Dependency Inversion Principle"),", ",(0,r.kt)("a",{parentName:"p",href:"https://en.wikipedia.org/wiki/Inversion_of_control"},"Inversion Of Control")," and ",(0,r.kt)("a",{parentName:"p",href:"https://martinfowler.com/articles/injection.html"},"Dependency Injection")," in mind. "),(0,r.kt)("p",null,"Rather than letting the services instantiate their dependencies inside themselves, inject the dependencies on construction. Also, rather than creating the object hierarchy manually, you can use a Dependency Injection framework that does the work for you. That's why you are here, I suppose. \ud83d\ude42"),(0,r.kt)("p",null,"To achieve the most efficient usage of Stashbox, you should follow these steps:"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"At the startup of your application, instantiate a ",(0,r.kt)("inlineCode",{parentName:"li"},"StashboxContainer"),"."),(0,r.kt)("li",{parentName:"ul"},"Register your services into the container."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("a",{parentName:"li",href:"/docs/diagnostics/validation"},"Validate")," the state of the container and the registrations with the ",(0,r.kt)("inlineCode",{parentName:"li"},".Validate()")," method. ",(0,r.kt)("em",{parentName:"li"},"(Optional)")),(0,r.kt)("li",{parentName:"ul"},"During the lifetime of the application, use the container to resolve your services."),(0,r.kt)("li",{parentName:"ul"},"Create ",(0,r.kt)("a",{parentName:"li",href:"/docs/guides/scopes"},"scopes")," and use them to resolve your services. ",(0,r.kt)("em",{parentName:"li"},"(Optional)")),(0,r.kt)("li",{parentName:"ul"},"On application exit, call the container's ",(0,r.kt)("inlineCode",{parentName:"li"},".Dispose()")," or ",(0,r.kt)("inlineCode",{parentName:"li"},".DisposeAsync()")," method to clean up the resources. ",(0,r.kt)("em",{parentName:"li"},"(Optional)"))),(0,r.kt)("admonition",{type:"caution"},(0,r.kt)("p",{parentName:"admonition"},"You should create only a single instance from ",(0,r.kt)("inlineCode",{parentName:"p"},"StashboxContainer")," (plus child containers if you use them) per application domain. ",(0,r.kt)("inlineCode",{parentName:"p"},"StashboxContainer")," instances are thread-safe. Do not create new container instances continuously, such action will bypass the container's internal delegate cache and could lead to performance degradation. ")),(0,r.kt)("h2",{id:"how-it-works"},"How it works?"),(0,r.kt)("p",null,"Stashbox builds and maintains a collection of ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-registration--registered-service"},"registered services"),". When a service is requested, Stashbox starts looking for a matching registration with the same ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),". If it finds one, the container initiates a scan on the ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type's")," available constructors and selects the one with the most arguments it knows how to resolve by matching argument types to other registrations."),(0,r.kt)("p",null,"When every constructor argument has a companion registration, Stashbox jumps to the first one and continues the same scanning operation. "),(0,r.kt)("p",null,"This process is repeated until every ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#injectable-dependency"},"injectable dependency")," will have a match in the ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#resolution-tree"},"resolution tree"),". At the end of the process, Stashbox has each dependency node built-up in a hierarchical object structure to instantiate the initially requested service object."),(0,r.kt)("h2",{id:"example"},"Example"),(0,r.kt)("p",null,"Let's see a quick example. We have three services ",(0,r.kt)("inlineCode",{parentName:"p"},"DbBackup"),", ",(0,r.kt)("inlineCode",{parentName:"p"},"MessageBus")," and ",(0,r.kt)("inlineCode",{parentName:"p"},"ConsoleLogger"),". ",(0,r.kt)("inlineCode",{parentName:"p"},"DbBackup")," has a dependency on ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventBroadcaster")," (implemented by ",(0,r.kt)("inlineCode",{parentName:"p"},"MessageBus"),") and ",(0,r.kt)("inlineCode",{parentName:"p"},"ILogger")," (implemented by ",(0,r.kt)("inlineCode",{parentName:"p"},"ConsoleLogger"),"), ",(0,r.kt)("inlineCode",{parentName:"p"},"MessageBus")," also depending on an ",(0,r.kt)("inlineCode",{parentName:"p"},"ILogger"),":"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'public interface IJob { void DoTheJob(); }\n\npublic interface ILogger { void Log(string message); }\n\npublic interface IEventBroadcaster { void Broadcast(IEvent @event); }\n\n\npublic class ConsoleLogger : ILogger\n{\n    public void Log(string message) => Console.WriteLine(message);\n}\n\npublic class MessageBus : IEventBroadcaster\n{\n    private readonly ILogger logger;\n\n    public MessageBus(ILogger logger)\n    {\n        this.logger = logger;\n    }\n\n    void Broadcast(IEvent @event) \n    {\n        this.logger.Log($"Sending event to bus: {@event.Name}");\n        // Do the actual event broadcast.\n    }\n}\n\npublic class DbBackup : IJob\n{\n    private readonly ILogger logger;\n    private readonly IEventBroadcaster eventBroadcaster;\n\n    public DbBackup(ILogger logger, IEventBroadcaster eventBroadcaster)\n    {\n        this.logger = logger;\n        this.eventBroadcaster = eventBroadcaster;\n    }\n\n    public void DoTheJob() \n    {\n        this.logger.Log("Backing up!");\n        // Do the actual backup.\n        this.eventBroadcaster.Broadcast(new DbBackupCompleted());\n    } \n}\n')),(0,r.kt)("admonition",{type:"info"},(0,r.kt)("p",{parentName:"admonition"},"By depending only on interfaces, you decouple your services from concrete implementations. This gives you the flexibility of a more comfortable implementation replacement and isolates your components from each other. For example, unit testing benefits a lot from the possibility of replacing a real implementation with mock objects.")),(0,r.kt)("p",null,"The example services above used with Stashbox in a Console Application:"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"using Stashbox;\nusing System;\n\nnamespace Example\n{\n    public class Program\n    {\n        private static readonly IStashboxContainer container;\n\n        static Program()\n        {\n            // 1. Create container\n            container = new StashboxContainer();\n\n            // 2. Register your services\n            container.RegisterSingleton<ILogger, ConsoleLogger>();\n            container.Register<IEventBroadcaster, MessageBus>();\n            container.Register<IJob, DbBackup>();\n\n            // 3. Validate the configuration.\n            container.Validate();\n        }\n\n        static void Main(string[] args)\n        {\n            // 4. Resolve and use your service\n            var job = container.Resolve<IJob>();\n            job.DoTheJob();\n        }\n    }\n}\n")))}m.isMDXComponent=!0}}]);