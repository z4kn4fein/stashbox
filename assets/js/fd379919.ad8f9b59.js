"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[995],{3905:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>b});var a=n(7294);function r(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){r(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,a,r=function(e,t){if(null==e)return{};var n,a,r={},o=Object.keys(e);for(a=0;a<o.length;a++)n=o[a],t.indexOf(n)>=0||(r[n]=e[n]);return r}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(a=0;a<o.length;a++)n=o[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(r[n]=e[n])}return r}var l=a.createContext({}),c=function(e){var t=a.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},p=function(e){var t=c(e.components);return a.createElement(l.Provider,{value:t},e.children)},u="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},m=a.forwardRef((function(e,t){var n=e.components,r=e.mdxType,o=e.originalType,l=e.parentName,p=s(e,["components","mdxType","originalType","parentName"]),u=c(n),m=r,b=u["".concat(l,".").concat(m)]||u[m]||d[m]||o;return n?a.createElement(b,i(i({ref:t},p),{},{components:n})):a.createElement(b,i({ref:t},p))}));function b(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var o=n.length,i=new Array(o);i[0]=m;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s[u]="string"==typeof e?e:r,i[1]=s;for(var c=2;c<o;c++)i[c]=n[c];return a.createElement.apply(null,i)}return a.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>i});var a=n(7294),r=n(6010);const o="tabItem_Ymn6";function i(e){let{children:t,hidden:n,className:i}=e;return a.createElement("div",{role:"tabpanel",className:(0,r.Z)(o,i),hidden:n},t)}},4866:(e,t,n)=>{n.d(t,{Z:()=>N});var a=n(7462),r=n(7294),o=n(6010),i=n(2466),s=n(6550),l=n(1980),c=n(7392),p=n(12);function u(e){return function(e){return r.Children.map(e,(e=>{if(!e||(0,r.isValidElement)(e)&&function(e){const{props:t}=e;return!!t&&"object"==typeof t&&"value"in t}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}(e).map((e=>{let{props:{value:t,label:n,attributes:a,default:r}}=e;return{value:t,label:n,attributes:a,default:r}}))}function d(e){const{values:t,children:n}=e;return(0,r.useMemo)((()=>{const e=t??u(n);return function(e){const t=(0,c.l)(e,((e,t)=>e.value===t.value));if(t.length>0)throw new Error(`Docusaurus error: Duplicate values "${t.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[t,n])}function m(e){let{value:t,tabValues:n}=e;return n.some((e=>e.value===t))}function b(e){let{queryString:t=!1,groupId:n}=e;const a=(0,s.k6)(),o=function(e){let{queryString:t=!1,groupId:n}=e;if("string"==typeof t)return t;if(!1===t)return null;if(!0===t&&!n)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return n??null}({queryString:t,groupId:n});return[(0,l._X)(o),(0,r.useCallback)((e=>{if(!o)return;const t=new URLSearchParams(a.location.search);t.set(o,e),a.replace({...a.location,search:t.toString()})}),[o,a])]}function g(e){const{defaultValue:t,queryString:n=!1,groupId:a}=e,o=d(e),[i,s]=(0,r.useState)((()=>function(e){let{defaultValue:t,tabValues:n}=e;if(0===n.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(t){if(!m({value:t,tabValues:n}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${t}" but none of its children has the corresponding value. Available values are: ${n.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return t}const a=n.find((e=>e.default))??n[0];if(!a)throw new Error("Unexpected error: 0 tabValues");return a.value}({defaultValue:t,tabValues:o}))),[l,c]=b({queryString:n,groupId:a}),[u,g]=function(e){let{groupId:t}=e;const n=function(e){return e?`docusaurus.tab.${e}`:null}(t),[a,o]=(0,p.Nk)(n);return[a,(0,r.useCallback)((e=>{n&&o.set(e)}),[n,o])]}({groupId:a}),k=(()=>{const e=l??u;return m({value:e,tabValues:o})?e:null})();(0,r.useLayoutEffect)((()=>{k&&s(k)}),[k]);return{selectedValue:i,selectValue:(0,r.useCallback)((e=>{if(!m({value:e,tabValues:o}))throw new Error(`Can't select invalid tab value=${e}`);s(e),c(e),g(e)}),[c,g,o]),tabValues:o}}var k=n(2389);const h="tabList__CuJ",f="tabItem_LNqP";function v(e){let{className:t,block:n,selectedValue:s,selectValue:l,tabValues:c}=e;const p=[],{blockElementScrollPositionUntilNextRender:u}=(0,i.o5)(),d=e=>{const t=e.currentTarget,n=p.indexOf(t),a=c[n].value;a!==s&&(u(t),l(a))},m=e=>{let t=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const n=p.indexOf(e.currentTarget)+1;t=p[n]??p[0];break}case"ArrowLeft":{const n=p.indexOf(e.currentTarget)-1;t=p[n]??p[p.length-1];break}}t?.focus()};return r.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":n},t)},c.map((e=>{let{value:t,label:n,attributes:i}=e;return r.createElement("li",(0,a.Z)({role:"tab",tabIndex:s===t?0:-1,"aria-selected":s===t,key:t,ref:e=>p.push(e),onKeyDown:m,onClick:d},i,{className:(0,o.Z)("tabs__item",f,i?.className,{"tabs__item--active":s===t})}),n??t)})))}function y(e){let{lazy:t,children:n,selectedValue:a}=e;const o=(Array.isArray(n)?n:[n]).filter(Boolean);if(t){const e=o.find((e=>e.props.value===a));return e?(0,r.cloneElement)(e,{className:"margin-top--md"}):null}return r.createElement("div",{className:"margin-top--md"},o.map(((e,t)=>(0,r.cloneElement)(e,{key:t,hidden:e.props.value!==a}))))}function I(e){const t=g(e);return r.createElement("div",{className:(0,o.Z)("tabs-container",h)},r.createElement(v,(0,a.Z)({},e,t)),r.createElement(y,(0,a.Z)({},e,t)))}function N(e){const t=(0,k.Z)();return r.createElement(I,(0,a.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>s});var a=n(7294);const r="codeDescContainer_ie8f",o="desc_jyqI",i="example_eYlF";function s(e){let{children:t}=e,n=a.Children.toArray(t).filter((e=>e));return a.createElement("div",{className:r},a.createElement("div",{className:o},n[0]),a.createElement("div",{className:i},n[1]))}},9144:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>u,contentTitle:()=>c,default:()=>b,frontMatter:()=>l,metadata:()=>p,toc:()=>d});var a=n(7462),r=(n(7294),n(3905)),o=n(8846),i=n(4866),s=n(5162);const l={},c="Basic usage",p={unversionedId:"guides/basics",id:"guides/basics",title:"Basic usage",description:"This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections.",source:"@site/docs/guides/basics.md",sourceDirName:"guides",slug:"/guides/basics",permalink:"/stashbox/docs/guides/basics",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/basics.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1686681130,formattedLastUpdatedAt:"Jun 13, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Glossary",permalink:"/stashbox/docs/getting-started/glossary"},next:{title:"Advanced registration",permalink:"/stashbox/docs/guides/advanced-registration"}},u={},d=[{value:"Default registration",id:"default-registration",level:2},{value:"Named registration",id:"named-registration",level:2},{value:"Instance registration",id:"instance-registration",level:2},{value:"Re-mapping",id:"re-mapping",level:2},{value:"Wiring up",id:"wiring-up",level:2},{value:"Lifetime shortcuts",id:"lifetime-shortcuts",level:2}],m={toc:d};function b(e){let{components:t,...n}=e;return(0,r.kt)("wrapper",(0,a.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("h1",{id:"basic-usage"},"Basic usage"),(0,r.kt)("p",null,"This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections.\nStashbox provides several methods that enable registering services, and we'll go through the most common scenarios with code examples."),(0,r.kt)("h2",{id:"default-registration"},"Default registration"),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"Stashbox allows registration operations via the ",(0,r.kt)("inlineCode",{parentName:"p"},"Register()")," methods. "),(0,r.kt)("p",null,"During registration, the container checks whether the ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," is assignable from the ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type")," and if not, the container throws an ",(0,r.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#registration-validation"},"exception"),". "),(0,r.kt)("p",null,"Also, when the implementation is not resolvable, the container throws the same ",(0,r.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#registration-validation"},"exception"),"."),(0,r.kt)("p",null,"The example registers ",(0,r.kt)("inlineCode",{parentName:"p"},"DbBackup")," to be returned when ",(0,r.kt)("inlineCode",{parentName:"p"},"IJob")," is requested.")),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n// throws an exception because ConsoleLogger doesn't implement IJob.\ncontainer.Register<IJob, ConsoleLogger>();\n// throws an exception because IJob is not a valid implementation.\ncontainer.Register<IJob, IJob>();\n"))),(0,r.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IJob), typeof(DbBackup));\nobject job = container.Resolve(typeof(IJob));\n// throws an exception because ConsoleLogger doesn't implement IJob.\ncontainer.Register(typeof(IJob), typeof(ConsoleLogger));\n// throws an exception because IJob is not a valid implementation.\ncontainer.Register(typeof(IJob), typeof(IJob));\n")))))),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"You can register a service to itself without specifying a ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),", only the implementation (",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#self-registration"},"self registration"),"). "),(0,r.kt)("p",null,"In this case, the given implementation is considered the ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," and must be used to request the service (",(0,r.kt)("inlineCode",{parentName:"p"},"DbBackup")," in the example).")),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<DbBackup>();\nDbBackup backup = container.Resolve<DbBackup>();\n"))),(0,r.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(DbBackup));\nobject backup = container.Resolve(typeof(DbBackup));\n")))))),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"The container's API is fluent, which means you can chain the calls on its methods after each other.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"var job = container.Register<IJob, DbBackup>()\n    .Register<ILogger, ConsoleLogger>()\n    .Resolve<IJob>();\n")))),(0,r.kt)("h2",{id:"named-registration"},"Named registration"),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"The example shows how you can bind more implementations to a ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," using names for identification. "),(0,r.kt)("p",null,"The same name must be used to resolve the named service."),(0,r.kt)("admonition",{type:"note"},(0,r.kt)("p",{parentName:"admonition"},"The name is an ",(0,r.kt)("inlineCode",{parentName:"p"},"object")," type."))),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>("DbBackup");\ncontainer.Register<IJob, StorageCleanup>("StorageCleanup");\nIJob cleanup = container.Resolve<IJob>("StorageCleanup");\n'))),(0,r.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register(typeof(IJob), typeof(DbBackup), "DbBackup");\ncontainer.Register(typeof(IJob), typeof(StorageCleanup), "StorageCleanup");\nobject cleanup = container.Resolve(typeof(IJob), "StorageCleanup");\n')))))),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"You can also get each service that share the same name by requesting an ",(0,r.kt)("inlineCode",{parentName:"p"},"IEnumerable<>")," or using the ",(0,r.kt)("inlineCode",{parentName:"p"},"ResolveAll()")," method with the ",(0,r.kt)("inlineCode",{parentName:"p"},"name")," parameter.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>("StorageJobs");\ncontainer.Register<IJob, StorageCleanup>("StorageJobs");\ncontainer.Register<IJob, AnotherJob>();\n// jobs will be [DbBackup, StorageCleanup].\nIEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>("StorageJobs");\n')))),(0,r.kt)("h2",{id:"instance-registration"},"Instance registration"),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"With instance registration, you can provide an already created external instance to use when the given ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," is requested."),(0,r.kt)("p",null,"Stashbox automatically handles the ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/scopes#disposal"},"disposal")," of the registered instances, but you can turn this feature off with the ",(0,r.kt)("inlineCode",{parentName:"p"},"withoutDisposalTracking")," parameter."),(0,r.kt)("p",null,"When an ",(0,r.kt)("inlineCode",{parentName:"p"},"IJob")," is requested, the container will always return the external instance.")),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job);\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>();\n"))),(0,r.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"var job = new DbBackup();\ncontainer.RegisterInstance(job, typeof(IJob));\n\n// resolvedJob and job are the same.\nobject resolvedJob = container.Resolve(typeof(IJob));\n"))),(0,r.kt)(s.Z,{value:"Named",label:"Named",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job, "DbBackup");\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>("DbBackup");\n'))),(0,r.kt)(s.Z,{value:"No dispose",label:"No dispose",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"var job = new DbBackup();\ncontainer.RegisterInstance<IJob>(job, withoutDisposalTracking: true);\n\n// resolvedJob and job are the same.\nIJob resolvedJob = container.Resolve<IJob>();\n")))))),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"The instance registration API allows the batched registration of different instances.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterInstances<IJob>(new DbBackup(), new StorageCleanup());\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n")))),(0,r.kt)("h2",{id:"re-mapping"},"Re-mapping"),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"With re-map, you can bind new implementations to a ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," and delete old registrations in one action. "),(0,r.kt)("admonition",{type:"caution"},(0,r.kt)("p",{parentName:"admonition"},"When there are multiple registrations mapped to a ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),", ",(0,r.kt)("inlineCode",{parentName:"p"},".ReMap()")," will replace all of them with the given ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type"),". If you want to replace only one specific service, use the ",(0,r.kt)("inlineCode",{parentName:"p"},".ReplaceExisting()")," ",(0,r.kt)("a",{parentName:"p",href:"/docs/configuration/registration-configuration#replace"},"configuration option"),"."))),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\ncontainer.ReMap<IJob, StorageCleanup>();\n// jobs contain all two jobs\nIEnumerable<IJob> jobs = container.ResolveAll<IJob>();\n\ncontainer.ReMap<IJob, SlackMessageSender>();\n// jobs contains only the SlackMessageSender\njobs = container.ResolveAll<IJob>();\n"))),(0,r.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IJob), typeof(DbBackup));\ncontainer.Register(typeof(IJob), typeof(StorageCleanup));\n// jobs contain all two jobs\nIEnumerable<object> jobs = container.ResolveAll(typeof(IJob));\n\ncontainer.ReMap(typeof(IJob), typeof(SlackMessageSender));\n// jobs contains only the SlackMessageSender\njobs = container.ResolveAll(typeof(IJob));\n")))))),(0,r.kt)("h2",{id:"wiring-up"},"Wiring up"),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"Wiring up is similar to ",(0,r.kt)("a",{parentName:"p",href:"#instance-registration"},"Instance registration")," except that the container will perform property/field injection (if configured so and applicable) on the registered instance during resolution.")),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(s.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.WireUp<IJob>(new DbBackup());\nIJob job = container.Resolve<IJob>();\n"))),(0,r.kt)(s.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.WireUp(new DbBackup(), typeof(IJob));\nobject job = container.Resolve(typeof(IJob));\n")))))),(0,r.kt)("h2",{id:"lifetime-shortcuts"},"Lifetime shortcuts"),(0,r.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,"A service's lifetime indicates how long its instance will live and which re-using policy should be applied when it gets injected.",(0,r.kt)("p",null,"This example shows how you can use the registration API's shortcuts for lifetimes. These are just sugars, and there are more ways explained in the ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/lifetimes"},"lifetimes")," section."),(0,r.kt)("admonition",{type:"info"},(0,r.kt)("p",{parentName:"admonition"},"The ",(0,r.kt)("inlineCode",{parentName:"p"},"DefaultLifetime")," is ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/lifetimes#default-lifetime"},"configurable"),"."))),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(s.Z,{value:"Default",label:"Default",mdxType:"TabItem"},(0,r.kt)("p",null,"When no lifetime is specified, the service will use the container's ",(0,r.kt)("inlineCode",{parentName:"p"},"DefaultLifetime"),", which is ",(0,r.kt)("inlineCode",{parentName:"p"},"Transient")," by default."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n"))),(0,r.kt)(s.Z,{value:"Singleton",label:"Singleton",mdxType:"TabItem"},(0,r.kt)("p",null,"A service with ",(0,r.kt)("inlineCode",{parentName:"p"},"Singleton")," lifetime will be instantiated once and reused during the container's lifetime."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterSingleton<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n"))),(0,r.kt)(s.Z,{value:"Scoped",label:"Scoped",mdxType:"TabItem"},(0,r.kt)("p",null,"The ",(0,r.kt)("inlineCode",{parentName:"p"},"Scoped")," lifetime behaves like a ",(0,r.kt)("inlineCode",{parentName:"p"},"Singleton")," within a ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/scopes"},"scope"),".\nA scoped service is instantiated once and reused during the scope's whole lifetime."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterScoped<IJob, DbBackup>();\nIJob job = container.Resolve<IJob>();\n")))))))}b.isMDXComponent=!0}}]);