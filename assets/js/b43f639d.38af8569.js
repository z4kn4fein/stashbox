"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[965],{3905:(e,t,n)=>{n.d(t,{Zo:()=>u,kt:()=>h});var i=n(7294);function o(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function a(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);t&&(i=i.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,i)}return n}function r(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?a(Object(n),!0).forEach((function(t){o(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):a(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function l(e,t){if(null==e)return{};var n,i,o=function(e,t){if(null==e)return{};var n,i,o={},a=Object.keys(e);for(i=0;i<a.length;i++)n=a[i],t.indexOf(n)>=0||(o[n]=e[n]);return o}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(i=0;i<a.length;i++)n=a[i],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(o[n]=e[n])}return o}var s=i.createContext({}),c=function(e){var t=i.useContext(s),n=t;return e&&(n="function"==typeof e?e(t):r(r({},t),e)),n},u=function(e){var t=c(e.components);return i.createElement(s.Provider,{value:t},e.children)},p="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return i.createElement(i.Fragment,{},t)}},m=i.forwardRef((function(e,t){var n=e.components,o=e.mdxType,a=e.originalType,s=e.parentName,u=l(e,["components","mdxType","originalType","parentName"]),p=c(n),m=o,h=p["".concat(s,".").concat(m)]||p[m]||d[m]||a;return n?i.createElement(h,r(r({ref:t},u),{},{components:n})):i.createElement(h,r({ref:t},u))}));function h(e,t){var n=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var a=n.length,r=new Array(a);r[0]=m;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l[p]="string"==typeof e?e:o,r[1]=l;for(var c=2;c<a;c++)r[c]=n[c];return i.createElement.apply(null,r)}return i.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>r});var i=n(7294),o=n(6010);const a="tabItem_Ymn6";function r(e){let{children:t,hidden:n,className:r}=e;return i.createElement("div",{role:"tabpanel",className:(0,o.Z)(a,r),hidden:n},t)}},5488:(e,t,n)=>{n.d(t,{Z:()=>m});var i=n(7462),o=n(7294),a=n(6010),r=n(2389),l=n(7392),s=n(7094),c=n(2466);const u="tabList__CuJ",p="tabItem_LNqP";function d(e){const{lazy:t,block:n,defaultValue:r,values:d,groupId:m,className:h}=e,k=o.Children.map(e.children,(e=>{if((0,o.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),v=d??k.map((e=>{let{props:{value:t,label:n,attributes:i}}=e;return{value:t,label:n,attributes:i}})),f=(0,l.l)(v,((e,t)=>e.value===t.value));if(f.length>0)throw new Error(`Docusaurus error: Duplicate values "${f.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const g=null===r?r:r??k.find((e=>e.props.default))?.props.value??k[0].props.value;if(null!==g&&!v.some((e=>e.value===g)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${g}" but none of its children has the corresponding value. Available values are: ${v.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:b,setTabGroupChoices:y}=(0,s.U)(),[w,C]=(0,o.useState)(g),N=[],{blockElementScrollPositionUntilNextRender:x}=(0,c.o5)();if(null!=m){const e=b[m];null!=e&&e!==w&&v.some((t=>t.value===e))&&C(e)}const P=e=>{const t=e.currentTarget,n=N.indexOf(t),i=v[n].value;i!==w&&(x(t),C(i),null!=m&&y(m,String(i)))},T=e=>{let t=null;switch(e.key){case"Enter":P(e);break;case"ArrowRight":{const n=N.indexOf(e.currentTarget)+1;t=N[n]??N[0];break}case"ArrowLeft":{const n=N.indexOf(e.currentTarget)-1;t=N[n]??N[N.length-1];break}}t?.focus()};return o.createElement("div",{className:(0,a.Z)("tabs-container",u)},o.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,a.Z)("tabs",{"tabs--block":n},h)},v.map((e=>{let{value:t,label:n,attributes:r}=e;return o.createElement("li",(0,i.Z)({role:"tab",tabIndex:w===t?0:-1,"aria-selected":w===t,key:t,ref:e=>N.push(e),onKeyDown:T,onClick:P},r,{className:(0,a.Z)("tabs__item",p,r?.className,{"tabs__item--active":w===t})}),n??t)}))),t?(0,o.cloneElement)(k.filter((e=>e.props.value===w))[0],{className:"margin-top--md"}):o.createElement("div",{className:"margin-top--md"},k.map(((e,t)=>(0,o.cloneElement)(e,{key:t,hidden:e.props.value!==w})))))}function m(e){const t=(0,r.Z)();return o.createElement(d,(0,i.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>l});var i=n(7294);const o="codeDescContainer_ie8f",a="desc_jyqI",r="example_eYlF";function l(e){let{children:t}=e,n=i.Children.toArray(t).filter((e=>e));return i.createElement("div",{className:o},i.createElement("div",{className:a},n[0]),i.createElement("div",{className:r},n[1]))}},6765:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>c,contentTitle:()=>l,default:()=>d,frontMatter:()=>r,metadata:()=>s,toc:()=>u});var i=n(7462),o=(n(7294),n(3905)),a=n(8846);n(5488),n(5162);const r={},l="Container configuration",s={unversionedId:"configuration/container-configuration",id:"configuration/container-configuration",title:"Container configuration",description:"The container's constructor has an Action parameter used to configure its behavior.",source:"@site/docs/configuration/container-configuration.md",sourceDirName:"configuration",slug:"/configuration/container-configuration",permalink:"/stashbox/docs/configuration/container-configuration",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/configuration/container-configuration.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1673344472,formattedLastUpdatedAt:"Jan 10, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Registration configuration",permalink:"/stashbox/docs/configuration/registration-configuration"},next:{title:"Generics",permalink:"/stashbox/docs/advanced/generics"}},c={},u=[{value:"Default configuration",id:"default-configuration",level:2},{value:"Tracking disposable transients",id:"tracking-disposable-transients",level:2},{value:"Auto member-injection",id:"auto-member-injection",level:2},{value:"<code>PropertiesWithPublicSetter</code>",id:"propertieswithpublicsetter",level:3},{value:"<code>PropertiesWithLimitedAccess</code>",id:"propertieswithlimitedaccess",level:3},{value:"<code>PrivateFields</code>",id:"privatefields",level:3},{value:"Combined rules",id:"combined-rules",level:3},{value:"Constructor selection",id:"constructor-selection",level:2},{value:"<code>PreferMostParameters</code>",id:"prefermostparameters",level:3},{value:"<code>PreferLeastParameters</code>",id:"preferleastparameters",level:3},{value:"Registration behavior",id:"registration-behavior",level:2},{value:"<code>SkipDuplications</code>",id:"skipduplications",level:3},{value:"<code>ThrowException</code>",id:"throwexception",level:3},{value:"<code>ReplaceExisting</code>",id:"replaceexisting",level:3},{value:"<code>PreserveDuplications</code>",id:"preserveduplications",level:3},{value:"Default lifetime",id:"default-lifetime",level:2},{value:"Lifetime validation",id:"lifetime-validation",level:2},{value:"Conventional resolution",id:"conventional-resolution",level:2},{value:"Using named service for un-named requests",id:"using-named-service-for-un-named-requests",level:2},{value:"Default value injection",id:"default-value-injection",level:2},{value:"Unknown type resolution",id:"unknown-type-resolution",level:2},{value:"Custom compiler",id:"custom-compiler",level:2},{value:"Re-build singletons in child containers",id:"re-build-singletons-in-child-containers",level:2}],p={toc:u};function d(e){let{components:t,...n}=e;return(0,o.kt)("wrapper",(0,i.Z)({},p,n,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"container-configuration"},"Container configuration"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The container's constructor has an ",(0,o.kt)("inlineCode",{parentName:"p"},"Action<T>")," parameter used to configure its behavior."),(0,o.kt)("p",null,"The configuration API is fluent, which means you can chain the configuration methods after each other.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDisposableTransientTracking()\n    .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)\n    .WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"The ",(0,o.kt)("strong",{parentName:"p"},"re-configuration")," of the container is also supported by calling its ",(0,o.kt)("inlineCode",{parentName:"p"},".Configure()")," method.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer();\ncontainer.Configure(options => options.WithDisposableTransientTracking());\n")))),(0,o.kt)("h2",{id:"default-configuration"},"Default configuration"),(0,o.kt)("p",null,"These features are set or enabled by default:"),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("a",{parentName:"li",href:"/docs/configuration/container-configuration#constructor-selection"},"Constructor selection"),": ",(0,o.kt)("inlineCode",{parentName:"li"},"Rules.ConstructorSelection.PreferMostParameters")),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("a",{parentName:"li",href:"/docs/configuration/container-configuration#registration-behavior"},"Registration behavior"),": ",(0,o.kt)("inlineCode",{parentName:"li"},"Rules.RegistrationBehavior.SkipDuplications")),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("a",{parentName:"li",href:"/docs/configuration/container-configuration#default-lifetime"},"Default lifetime"),": ",(0,o.kt)("inlineCode",{parentName:"li"},"Lifetimes.Transient"))),(0,o.kt)("h2",{id:"tracking-disposable-transients"},"Tracking disposable transients"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can enable or disable the tracking of disposable transient objects.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithDisposableTransientTracking());\n")))),(0,o.kt)("h2",{id:"auto-member-injection"},"Auto member-injection"),(0,o.kt)("p",null,"With this option, you can enable or disable the auto member-injection without ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#attributes"},"attributes"),"."),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"propertieswithpublicsetter"},(0,o.kt)("inlineCode",{parentName:"h3"},"PropertiesWithPublicSetter")),(0,o.kt)("p",null,"With this flag, the container will perform auto-injection on properties with public setters.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(\n        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"propertieswithlimitedaccess"},(0,o.kt)("inlineCode",{parentName:"h3"},"PropertiesWithLimitedAccess")),(0,o.kt)("p",null,"With this flag, the container will perform auto-injection on properties even when they don't have a public setter.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(\n        Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"privatefields"},(0,o.kt)("inlineCode",{parentName:"h3"},"PrivateFields")),(0,o.kt)("p",null,"With this flag, the container will perform auto-injection on private fields too.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(\n        Rules.AutoMemberInjectionRules.PrivateFields));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"combined-rules"},"Combined rules"),(0,o.kt)("p",null,"You can also combine these flags with bitwise logical operators to get a merged ruleset.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields | \n        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));\n")))),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"Member selection filter: ",(0,o.kt)("inlineCode",{parentName:"p"},"config.WithAutoMemberInjection(filter: member => member.Type != typeof(IJob))"))),(0,o.kt)("h2",{id:"constructor-selection"},"Constructor selection"),(0,o.kt)("p",null,"With this option, you can set the constructor selection rule used to determine which constructor the container should use for instantiation."),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"prefermostparameters"},(0,o.kt)("inlineCode",{parentName:"h3"},"PreferMostParameters")),(0,o.kt)("p",null,"It prefers the constructor which has the most extended parameter list.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithConstructorSelectionRule(\n        Rules.ConstructorSelection.PreferMostParameters));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"preferleastparameters"},(0,o.kt)("inlineCode",{parentName:"h3"},"PreferLeastParameters")),(0,o.kt)("p",null,"It prefers the constructor which has the shortest parameter list.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithConstructorSelectionRule(\n        Rules.ConstructorSelection.PreferLeastParameters));\n")))),(0,o.kt)("h2",{id:"registration-behavior"},"Registration behavior"),(0,o.kt)("p",null,"With this option, you can set the actual behavior used when a new service is registered into the container. These options do not affect named registrations."),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"skipduplications"},(0,o.kt)("inlineCode",{parentName:"h3"},"SkipDuplications")),(0,o.kt)("p",null,"The container will skip new registrations when the given ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type")," is already registered.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.SkipDuplications));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"throwexception"},(0,o.kt)("inlineCode",{parentName:"h3"},"ThrowException")),(0,o.kt)("p",null,"The container throws an ",(0,o.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#servicealreadyregisteredexception"},"exception")," when the given ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type")," is already registered.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.ThrowException));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"replaceexisting"},(0,o.kt)("inlineCode",{parentName:"h3"},"ReplaceExisting")),(0,o.kt)("p",null,"The container will replace the already ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-registration--registered-service"},"registered service")," with the given one when they have the same ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type"),".")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.ReplaceExisting));\n")))),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("h3",{id:"preserveduplications"},(0,o.kt)("inlineCode",{parentName:"h3"},"PreserveDuplications")),(0,o.kt)("p",null,"The container will keep registering the new services with the same ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type"),".")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.PreserveDuplications));\n")))),(0,o.kt)("h2",{id:"default-lifetime"},"Default lifetime"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can set the default lifetime used when a service doesn't have a configured one.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Scoped));\n")))),(0,o.kt)("h2",{id:"lifetime-validation"},"Lifetime validation"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can enable or disable the life-span and ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#root-scope"},"root scope")," resolution ",(0,o.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#lifetime-validation"},"validation")," on the dependency tree.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithLifetimeValidation());\n")))),(0,o.kt)("h2",{id:"conventional-resolution"},"Conventional resolution"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can enable or disable conventional resolution, which means the container treats the constructor/method parameter or member names as dependency names used by ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#named-resolution"},"named resolution"),".")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .TreatParameterAndMemberNameAsDependencyName());\n")))),(0,o.kt)("h2",{id:"using-named-service-for-un-named-requests"},"Using named service for un-named requests"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can enable or disable the selection of named registrations when the resolution request is un-named but with the same type.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithNamedDependencyResolutionForUnNamedRequests());\n")))),(0,o.kt)("h2",{id:"default-value-injection"},"Default value injection"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can enable or disable the default value injection.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithDefaultValueInjection());\n")))),(0,o.kt)("h2",{id:"unknown-type-resolution"},"Unknown type resolution"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can enable or disable the resolution of unregistered types. You can also use a configurator delegate to configure the registrations the container will create from the unknown types.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithUnknownTypeResolution(config => config.AsImplementedTypes()));\n")))),(0,o.kt)("h2",{id:"custom-compiler"},"Custom compiler"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can set an external expression tree compiler. It can be useful on platforms where the IL generator modules are not available; therefore, the expression compiler in Stashbox couldn't work.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithExpressionCompiler(\n        Rules.ExpressionCompilers.MicrosoftExpressionCompiler));\n")))),(0,o.kt)("h2",{id:"re-build-singletons-in-child-containers"},"Re-build singletons in child containers"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"With this option, you can enable or disable the re-building of singletons in child containers. It allows the child containers to override singleton dependencies in the parent.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithReBuildSingletonsInChildContainer());\n")))),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"This feature is not affecting the already built singleton instances in the parent.")))}d.isMDXComponent=!0}}]);