"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[995],{3905:(e,t,n)=>{n.d(t,{Zo:()=>c,kt:()=>b});var a=n(7294);function o(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function r(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?r(Object(n),!0).forEach((function(t){o(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):r(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,a,o=function(e,t){if(null==e)return{};var n,a,o={},r=Object.keys(e);for(a=0;a<r.length;a++)n=r[a],t.indexOf(n)>=0||(o[n]=e[n]);return o}(e,t);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);for(a=0;a<r.length;a++)n=r[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(o[n]=e[n])}return o}var l=a.createContext({}),p=function(e){var t=a.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},c=function(e){var t=p(e.components);return a.createElement(l.Provider,{value:t},e.children)},u="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},m=a.forwardRef((function(e,t){var n=e.components,o=e.mdxType,r=e.originalType,l=e.parentName,c=s(e,["components","mdxType","originalType","parentName"]),u=p(n),m=o,b=u["".concat(l,".").concat(m)]||u[m]||d[m]||r;return n?a.createElement(b,i(i({ref:t},c),{},{components:n})):a.createElement(b,i({ref:t},c))}));function b(e,t){var n=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var r=n.length,i=new Array(r);i[0]=m;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s[u]="string"==typeof e?e:o,i[1]=s;for(var p=2;p<r;p++)i[p]=n[p];return a.createElement.apply(null,i)}return a.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>i});var a=n(7294),o=n(6010);const r="tabItem_Ymn6";function i(e){let{children:t,hidden:n,className:i}=e;return a.createElement("div",{role:"tabpanel",className:(0,o.Z)(r,i),hidden:n},t)}},5488:(e,t,n)=>{n.d(t,{Z:()=>m});var a=n(7462),o=n(7294),r=n(6010),i=n(2389),s=n(7392),l=n(7094),p=n(2466);const c="tabList__CuJ",u="tabItem_LNqP";function d(e){const{lazy:t,block:n,defaultValue:i,values:d,groupId:m,className:b}=e,g=o.Children.map(e.children,(e=>{if((0,o.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),k=d??g.map((e=>{let{props:{value:t,label:n,attributes:a}}=e;return{value:t,label:n,attributes:a}})),h=(0,s.l)(k,((e,t)=>e.value===t.value));if(h.length>0)throw new Error(`Docusaurus error: Duplicate values "${h.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const v=null===i?i:i??g.find((e=>e.props.default))?.props.value??g[0].props.value;if(null!==v&&!k.some((e=>e.value===v)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${v}" but none of its children has the corresponding value. Available values are: ${k.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:y,setTabGroupChoices:f}=(0,l.U)(),[I,N]=(0,o.useState)(v),T=[],{blockElementScrollPositionUntilNextRender:w}=(0,p.o5)();if(null!=m){const e=y[m];null!=e&&e!==I&&k.some((t=>t.value===e))&&N(e)}const J=e=>{const t=e.currentTarget,n=T.indexOf(t),a=k[n].value;a!==I&&(w(t),N(a),null!=m&&f(m,String(a)))},x=e=>{let t=null;switch(e.key){case"Enter":J(e);break;case"ArrowRight":{const n=T.indexOf(e.currentTarget)+1;t=T[n]??T[0];break}case"ArrowLeft":{const n=T.indexOf(e.currentTarget)-1;t=T[n]??T[T.length-1];break}}t?.focus()};return o.createElement("div",{className:(0,r.Z)("tabs-container",c)},o.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,r.Z)("tabs",{"tabs--block":n},b)},k.map((e=>{let{value:t,label:n,attributes:i}=e;return o.createElement("li",(0,a.Z)({role:"tab",tabIndex:I===t?0:-1,"aria-selected":I===t,key:t,ref:e=>T.push(e),onKeyDown:x,onClick:J},i,{className:(0,r.Z)("tabs__item",u,i?.className,{"tabs__item--active":I===t})}),n??t)}))),t?(0,o.cloneElement)(g.filter((e=>e.props.value===I))[0],{className:"margin-top--md"}):o.createElement("div",{className:"margin-top--md"},g.map(((e,t)=>(0,o.cloneElement)(e,{key:t,hidden:e.props.value!==I})))))}function m(e){const t=(0,i.Z)();return o.createElement(d,(0,a.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>s});var a=n(7294);const o="codeDescContainer_ie8f",r="desc_jyqI",i="example_eYlF";function s(e){let{children:t}=e,n=a.Children.toArray(t).filter((e=>e));return a.createElement("div",{className:o},a.createElement("div",{className:r},n[0]),a.createElement("div",{className:i},n[1]))}},9144:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>u,contentTitle:()=>p,default:()=>b,frontMatter:()=>l,metadata:()=>c,toc:()=>d});var a=n(7462),o=(n(7294),n(3905)),r=n(8846),i=n(5488),s=n(5162);const l={},p="Basic usage",c={unversionedId:"guides/basics",id:"guides/basics",title:"Basic usage",description:"This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections.",source:"@site/docs/guides/basics.md",sourceDirName:"guides",slug:"/guides/basics",permalink:"/stashbox/docs/guides/basics",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/basics.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1672673299,formattedLastUpdatedAt:"Jan 2, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Glossary",permalink:"/stashbox/docs/getting-started/glossary"},next:{title:"Advanced registration",permalink:"/stashbox/docs/guides/advanced-registration"}},u={},d=[{value:"Default registration",id:"default-registration",level:2},{value:"Named registration",id:"named-registration",level:2},{value:"Instance registration",id:"instance-registration",level:2},{value:"Re-mapping",id:"re-mapping",level:2},{value:"Wiring up",id:"wiring-up",level:2},{value:"Lifetime shortcuts",id:"lifetime-shortcuts",level:2}],m={toc:d};function b(e){let{components:t,...n}=e;return(0,o.kt)("wrapper",(0,a.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"basic-usage"},"Basic usage"),(0,o.kt)("p",null,"This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections.\nStashbox provides several methods that enable registering services, and we'll go through the most common scenarios with code examples."),(0,o.kt)("h2",{id:"default-registration"},"Default registration"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"Stashbox allows registration operations via the ",(0,o.kt)("inlineCode",{parentName:"p"},"Register()")," methods. "),(0,o.kt)("p",null,"During registration, the container checks whether the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," is assignable from the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type")," and if not, the container throws an ",(0,o.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#registration-validation"},"exception"),". "),(0,o.kt)("p",null,"Also, when the implementation is not resolvable, the container throws the same ",(0,o.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#registration-validation"},"exception"),"."),(0,o.kt)("p",null,"The example registers ",(0,o.kt)("inlineCode",{parentName:"p"},"DbBackup")," to be returned when ",(0,o.kt)("inlineCode",{parentName:"p"},"IJob")," is requested.")),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n// throws an exception because ConsoleLogger doesn't implement IJob.\ncontainer.Register<IJob, ConsoleLogger>();\n// throws an exception because IJob is not a valid implementation.\ncontainer.Register<IJob, IJob>();\n"))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IJob), typeof(DbBackup));\nobject job = container.Resolve(typeof(IJob));\n// throws an exception because ConsoleLogger doesn't implement IJob.\ncontainer.Register(typeof(IJob), typeof(ConsoleLogger));\n// throws an exception because IJob is not a valid implementation.\ncontainer.Register(typeof(IJob), typeof(IJob));\n")))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"You can register a service to itself without specifying a ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),", only the implementation (",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#self-registration"},"self registration"),"). "),(0,o.kt)("p",null,"In this case, the given implementation is considered as the ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," and must be used for requesting the service (",(0,o.kt)("inlineCode",{parentName:"p"},"DbBackup")," in the example).")),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<DbBackup>();\nDbBackup backup = container.Resolve<DbBackup>();\n"))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(DbBackup));\nobject backup = container.Resolve(typeof(DbBackup));\n")))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The container's API is fluent, which means you can chain the calls on its methods after each other.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"var job = container.Register<IJob, DbBackup>()\n    .Register<ILogger, ConsoleLogger>()\n    .Resolve<IJob>();\n")))),(0,o.kt)("h2",{id:"named-registration"},"Named registration"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The example shows how you can bind more implementations to a ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," using names for identification. "),(0,o.kt)("p",null,"The same name must be used to resolve the named service."),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"The name is an ",(0,o.kt)("inlineCode",{parentName:"p"},"object")," type."))),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>("DbBackup");\ncontainer.Register<IJob, StorageCleanup>("StorageCleanup");\nIJob cleanup = container.Resolve<IJob>("StorageCleanup");\n'))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register(typeof(IJob), typeof(DbBackup), "DbBackup");\ncontainer.Register(typeof(IJob), typeof(StorageCleanup), "StorageCleanup");\nobject cleanup = container.Resolve(typeof(IJob), "StorageCleanup");\n')))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"You can also get each service that share the same name by requesting an ",(0,o.kt)("inlineCode",{parentName:"p"},"IEnumerable<>")," or using the ",(0,o.kt)("inlineCode",{parentName:"p"},"ResolveAll()")," method with the ",(0,o.kt)("inlineCode",{parentName:"p"},"name")," parameter.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>("StorageJobs");\ncontainer.Register<IJob, StorageCleanup>("StorageJobs");\ncontainer.Register<IJob, AnotherJob>();\n// jobs will be [DbBackup, StorageCleanup].\nIEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>("StorageJobs");\n')))),(0,o.kt)("h2",{id:"instance-registration"},"Instance registration"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With instance registration, you can provide an already created external instance to use when the given ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," is requested."),(0,o.kt)("p",null,"Stashbox automatically handles the ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/scopes#disposal"},"disposal")," of the registered instances, but you can turn this feature off with the ",(0,o.kt)("inlineCode",{parentName:"p"},"withoutDisposalTracking")," parameter."),(0,o.kt)("p",null,"When an ",(0,o.kt)("inlineCode",{parentName:"p"},"IJob")," is requested, the container will always return the external instance.")),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job);\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>();\n"))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"var job = new DbBackup();\ncontainer.RegisterInstance(job, typeof(IJob));\n\n// resolvedJob and job are the same.\nobject resolvedJob = container.Resolve(typeof(IJob));\n"))),(0,o.kt)(s.Z,{value:"Named",label:"Named",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job, "DbBackup");\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>("DbBackup");\n'))),(0,o.kt)(s.Z,{value:"No dispose",label:"No dispose",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job, withoutDisposalTracking: true);\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>();\n")))))),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The instance registration API allows the batched registration of different instances.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterInstances<IJob>(new DbBackup(), new StorageCleanup());\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n")))),(0,o.kt)("h2",{id:"re-mapping"},"Re-mapping"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With re-map, you can bind new implementations to a ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," and delete its old registrations in one action. "),(0,o.kt)("admonition",{type:"caution"},(0,o.kt)("p",{parentName:"admonition"},"When there are multiple registrations mapped to a ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),", ",(0,o.kt)("inlineCode",{parentName:"p"},".ReMap()")," will replace all of them with the given ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type"),". If you want to replace only one specified service, use the ",(0,o.kt)("inlineCode",{parentName:"p"},".ReplaceExisting()")," ",(0,o.kt)("a",{parentName:"p",href:"/docs/configuration/registration-configuration#replace"},"configuration option"),"."))),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\ncontainer.ReMap<IJob, StorageCleanup>();\n// jobs contain all two jobs\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n\ncontainer.ReMap<IJob, SlackMessageSender>();\n// jobs contains only the SlackMessageSender\njobs = container.ResolveAll<IJob>();\n"))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IJob), typeof(DbBackup));\ncontainer.Register(typeof(IJob), typeof(StorageCleanup));\n// jobs contain all two jobs\nIEnumerable<object> jobs = container.ResolveAll(typeof(IJob));\n\ncontainer.ReMap(typeof(IJob), typeof(SlackMessageSender));\n// jobs contains only the SlackMessageSender\njobs = container.ResolveAll(typeof(IJob));\n")))))),(0,o.kt)("h2",{id:"wiring-up"},"Wiring up"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"Wiring up is similar to the ",(0,o.kt)("a",{parentName:"p",href:"#instance-registration"},"Instance registration")," except that the container will perform property / field injection (if configured so and applicable) on the registered instance during resolution.")),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.WireUp<IJob>(new DbBackup());\nIJob job = container.Resolve<IJob>();\n"))),(0,o.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.WireUp(new DbBackup(), typeof(IJob));\nobject job = container.Resolve(typeof(IJob));\n")))))),(0,o.kt)("h2",{id:"lifetime-shortcuts"},"Lifetime shortcuts"),(0,o.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,"The service's lifetime indicates how long the service's instance will live and the re-using policy applied when it gets injected.",(0,o.kt)("p",null,"This example shows how you can use the registration API's shortcuts for lifetimes. These are just sugars, and there are more ways explained in the ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/lifetimes"},"lifetimes")," section."),(0,o.kt)("admonition",{type:"info"},(0,o.kt)("p",{parentName:"admonition"},"The ",(0,o.kt)("inlineCode",{parentName:"p"},"DefaultLifetime")," is ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/lifetimes#default-lifetime"},"configurable"),"."))),(0,o.kt)("div",null,(0,o.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,o.kt)(s.Z,{value:"Default",label:"Default",mdxType:"TabItem"},(0,o.kt)("p",null,"When no lifetime is specified, the service will use the container's ",(0,o.kt)("inlineCode",{parentName:"p"},"DefaultLifetime"),", which is ",(0,o.kt)("inlineCode",{parentName:"p"},"Transient")," by default."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n"))),(0,o.kt)(s.Z,{value:"Singleton",label:"Singleton",mdxType:"TabItem"},(0,o.kt)("p",null,"A service with ",(0,o.kt)("inlineCode",{parentName:"p"},"Singleton")," lifetime will be instantiated once and reused during the container's lifetime."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterSingleton<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n"))),(0,o.kt)(s.Z,{value:"Scoped",label:"Scoped",mdxType:"TabItem"},(0,o.kt)("p",null,"The ",(0,o.kt)("inlineCode",{parentName:"p"},"Scoped")," lifetime behaves like a ",(0,o.kt)("inlineCode",{parentName:"p"},"Singleton")," within a ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope"),".\nThe scoped service is instantiated once and reused during the scope's lifetime."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterScoped<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n")))))))}b.isMDXComponent=!0}}]);