"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[965],{3905:(e,t,n)=>{n.d(t,{Zo:()=>c,kt:()=>h});var i=n(7294);function a(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);t&&(i=i.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,i)}return n}function r(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){a(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function l(e,t){if(null==e)return{};var n,i,a=function(e,t){if(null==e)return{};var n,i,a={},o=Object.keys(e);for(i=0;i<o.length;i++)n=o[i],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(i=0;i<o.length;i++)n=o[i],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var s=i.createContext({}),u=function(e){var t=i.useContext(s),n=t;return e&&(n="function"==typeof e?e(t):r(r({},t),e)),n},c=function(e){var t=u(e.components);return i.createElement(s.Provider,{value:t},e.children)},p="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return i.createElement(i.Fragment,{},t)}},m=i.forwardRef((function(e,t){var n=e.components,a=e.mdxType,o=e.originalType,s=e.parentName,c=l(e,["components","mdxType","originalType","parentName"]),p=u(n),m=a,h=p["".concat(s,".").concat(m)]||p[m]||d[m]||o;return n?i.createElement(h,r(r({ref:t},c),{},{components:n})):i.createElement(h,r({ref:t},c))}));function h(e,t){var n=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var o=n.length,r=new Array(o);r[0]=m;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l[p]="string"==typeof e?e:a,r[1]=l;for(var u=2;u<o;u++)r[u]=n[u];return i.createElement.apply(null,r)}return i.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>r});var i=n(7294),a=n(6010);const o="tabItem_Ymn6";function r(e){let{children:t,hidden:n,className:r}=e;return i.createElement("div",{role:"tabpanel",className:(0,a.Z)(o,r),hidden:n},t)}},4866:(e,t,n)=>{n.d(t,{Z:()=>C});var i=n(7462),a=n(7294),o=n(6010),r=n(2466),l=n(6550),s=n(1980),u=n(7392),c=n(12);function p(e){return function(e){return a.Children.map(e,(e=>{if(!e||(0,a.isValidElement)(e)&&function(e){const{props:t}=e;return!!t&&"object"==typeof t&&"value"in t}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}(e).map((e=>{let{props:{value:t,label:n,attributes:i,default:a}}=e;return{value:t,label:n,attributes:i,default:a}}))}function d(e){const{values:t,children:n}=e;return(0,a.useMemo)((()=>{const e=t??p(n);return function(e){const t=(0,u.l)(e,((e,t)=>e.value===t.value));if(t.length>0)throw new Error(`Docusaurus error: Duplicate values "${t.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[t,n])}function m(e){let{value:t,tabValues:n}=e;return n.some((e=>e.value===t))}function h(e){let{queryString:t=!1,groupId:n}=e;const i=(0,l.k6)(),o=function(e){let{queryString:t=!1,groupId:n}=e;if("string"==typeof t)return t;if(!1===t)return null;if(!0===t&&!n)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return n??null}({queryString:t,groupId:n});return[(0,s._X)(o),(0,a.useCallback)((e=>{if(!o)return;const t=new URLSearchParams(i.location.search);t.set(o,e),i.replace({...i.location,search:t.toString()})}),[o,i])]}function f(e){const{defaultValue:t,queryString:n=!1,groupId:i}=e,o=d(e),[r,l]=(0,a.useState)((()=>function(e){let{defaultValue:t,tabValues:n}=e;if(0===n.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(t){if(!m({value:t,tabValues:n}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${t}" but none of its children has the corresponding value. Available values are: ${n.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return t}const i=n.find((e=>e.default))??n[0];if(!i)throw new Error("Unexpected error: 0 tabValues");return i.value}({defaultValue:t,tabValues:o}))),[s,u]=h({queryString:n,groupId:i}),[p,f]=function(e){let{groupId:t}=e;const n=function(e){return e?`docusaurus.tab.${e}`:null}(t),[i,o]=(0,c.Nk)(n);return[i,(0,a.useCallback)((e=>{n&&o.set(e)}),[n,o])]}({groupId:i}),k=(()=>{const e=s??p;return m({value:e,tabValues:o})?e:null})();(0,a.useLayoutEffect)((()=>{k&&l(k)}),[k]);return{selectedValue:r,selectValue:(0,a.useCallback)((e=>{if(!m({value:e,tabValues:o}))throw new Error(`Can't select invalid tab value=${e}`);l(e),u(e),f(e)}),[u,f,o]),tabValues:o}}var k=n(2389);const g="tabList__CuJ",v="tabItem_LNqP";function b(e){let{className:t,block:n,selectedValue:l,selectValue:s,tabValues:u}=e;const c=[],{blockElementScrollPositionUntilNextRender:p}=(0,r.o5)(),d=e=>{const t=e.currentTarget,n=c.indexOf(t),i=u[n].value;i!==l&&(p(t),s(i))},m=e=>{let t=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const n=c.indexOf(e.currentTarget)+1;t=c[n]??c[0];break}case"ArrowLeft":{const n=c.indexOf(e.currentTarget)-1;t=c[n]??c[c.length-1];break}}t?.focus()};return a.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":n},t)},u.map((e=>{let{value:t,label:n,attributes:r}=e;return a.createElement("li",(0,i.Z)({role:"tab",tabIndex:l===t?0:-1,"aria-selected":l===t,key:t,ref:e=>c.push(e),onKeyDown:m,onClick:d},r,{className:(0,o.Z)("tabs__item",v,r?.className,{"tabs__item--active":l===t})}),n??t)})))}function y(e){let{lazy:t,children:n,selectedValue:i}=e;const o=(Array.isArray(n)?n:[n]).filter(Boolean);if(t){const e=o.find((e=>e.props.value===i));return e?(0,a.cloneElement)(e,{className:"margin-top--md"}):null}return a.createElement("div",{className:"margin-top--md"},o.map(((e,t)=>(0,a.cloneElement)(e,{key:t,hidden:e.props.value!==i}))))}function w(e){const t=f(e);return a.createElement("div",{className:(0,o.Z)("tabs-container",g)},a.createElement(b,(0,i.Z)({},e,t)),a.createElement(y,(0,i.Z)({},e,t)))}function C(e){const t=(0,k.Z)();return a.createElement(w,(0,i.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>l});var i=n(7294);const a="codeDescContainer_ie8f",o="desc_jyqI",r="example_eYlF";function l(e){let{children:t}=e,n=i.Children.toArray(t).filter((e=>e));return i.createElement("div",{className:a},i.createElement("div",{className:o},n[0]),i.createElement("div",{className:r},n[1]))}},1336:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>u,contentTitle:()=>l,default:()=>d,frontMatter:()=>r,metadata:()=>s,toc:()=>c});var i=n(7462),a=(n(7294),n(3905)),o=n(8846);n(4866),n(5162);const r={},l="Container configuration",s={unversionedId:"configuration/container-configuration",id:"configuration/container-configuration",title:"Container configuration",description:"The container's constructor has an Action parameter used to configure its behavior.",source:"@site/docs/configuration/container-configuration.md",sourceDirName:"configuration",slug:"/configuration/container-configuration",permalink:"/stashbox/docs/configuration/container-configuration",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/configuration/container-configuration.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1693906449,formattedLastUpdatedAt:"Sep 5, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Registration configuration",permalink:"/stashbox/docs/configuration/registration-configuration"},next:{title:"Generics",permalink:"/stashbox/docs/advanced/generics"}},u={},c=[{value:"Default configuration",id:"default-configuration",level:2},{value:"Tracking disposable transients",id:"tracking-disposable-transients",level:2},{value:"Auto member-injection",id:"auto-member-injection",level:2},{value:"<code>PropertiesWithPublicSetter</code>",id:"propertieswithpublicsetter",level:3},{value:"<code>PropertiesWithLimitedAccess</code>",id:"propertieswithlimitedaccess",level:3},{value:"<code>PrivateFields</code>",id:"privatefields",level:3},{value:"Combined rules",id:"combined-rules",level:3},{value:"Constructor selection",id:"constructor-selection",level:2},{value:"<code>PreferMostParameters</code>",id:"prefermostparameters",level:3},{value:"<code>PreferLeastParameters</code>",id:"preferleastparameters",level:3},{value:"Registration behavior",id:"registration-behavior",level:2},{value:"<code>SkipDuplications</code>",id:"skipduplications",level:3},{value:"<code>ThrowException</code>",id:"throwexception",level:3},{value:"<code>ReplaceExisting</code>",id:"replaceexisting",level:3},{value:"<code>PreserveDuplications</code>",id:"preserveduplications",level:3},{value:"Default lifetime",id:"default-lifetime",level:2},{value:"Lifetime validation",id:"lifetime-validation",level:2},{value:"Conventional resolution",id:"conventional-resolution",level:2},{value:"Using named service for un-named requests",id:"using-named-service-for-un-named-requests",level:2},{value:"Default value injection",id:"default-value-injection",level:2},{value:"Unknown type resolution",id:"unknown-type-resolution",level:2},{value:"Custom compiler",id:"custom-compiler",level:2},{value:"Re-build singletons in child containers",id:"re-build-singletons-in-child-containers",level:2}],p={toc:c};function d(e){let{components:t,...n}=e;return(0,a.kt)("wrapper",(0,i.Z)({},p,n,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"container-configuration"},"Container configuration"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"The container's constructor has an ",(0,a.kt)("inlineCode",{parentName:"p"},"Action<T>")," parameter used to configure its behavior."),(0,a.kt)("p",null,"The configuration API is fluent, which means you can chain the configuration methods after each other.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer(options => options\n    .WithDisposableTransientTracking()\n    .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)\n    .WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Re-configuration")," of the container is also supported by calling its ",(0,a.kt)("inlineCode",{parentName:"p"},".Configure()")," method.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"var container = new StashboxContainer();\ncontainer.Configure(options => options.WithDisposableTransientTracking());\n")))),(0,a.kt)("h2",{id:"default-configuration"},"Default configuration"),(0,a.kt)("p",null,"These features are set by default:"),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("a",{parentName:"li",href:"/docs/configuration/container-configuration#constructor-selection"},"Constructor selection"),": ",(0,a.kt)("inlineCode",{parentName:"li"},"Rules.ConstructorSelection.PreferMostParameters")),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("a",{parentName:"li",href:"/docs/configuration/container-configuration#registration-behavior"},"Registration behavior"),": ",(0,a.kt)("inlineCode",{parentName:"li"},"Rules.RegistrationBehavior.SkipDuplications")),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("a",{parentName:"li",href:"/docs/configuration/container-configuration#default-lifetime"},"Default lifetime"),": ",(0,a.kt)("inlineCode",{parentName:"li"},"Lifetimes.Transient"))),(0,a.kt)("h2",{id:"tracking-disposable-transients"},"Tracking disposable transients"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can enable or disable the tracking of disposable transient objects.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithDisposableTransientTracking());\n")))),(0,a.kt)("h2",{id:"auto-member-injection"},"Auto member-injection"),(0,a.kt)("p",null,"With this option, you can enable or disable the auto member-injection without ",(0,a.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#attributes"},"attributes"),"."),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"propertieswithpublicsetter"},(0,a.kt)("inlineCode",{parentName:"h3"},"PropertiesWithPublicSetter")),(0,a.kt)("p",null,"With this flag, the container will perform auto-injection on properties with public setters.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(\n        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"propertieswithlimitedaccess"},(0,a.kt)("inlineCode",{parentName:"h3"},"PropertiesWithLimitedAccess")),(0,a.kt)("p",null,"With this flag, the container will perform auto-injection on properties even when they don't have a public setter.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(\n        Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"privatefields"},(0,a.kt)("inlineCode",{parentName:"h3"},"PrivateFields")),(0,a.kt)("p",null,"With this flag, the container will perform auto-injection on private fields too.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(\n        Rules.AutoMemberInjectionRules.PrivateFields));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"combined-rules"},"Combined rules"),(0,a.kt)("p",null,"You can also combine these flags with bitwise logical operators to get a merged ruleset.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields | \n        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));\n")))),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"Member selection filter: ",(0,a.kt)("inlineCode",{parentName:"p"},"config.WithAutoMemberInjection(filter: member => member.Type != typeof(IJob))"))),(0,a.kt)("h2",{id:"constructor-selection"},"Constructor selection"),(0,a.kt)("p",null,"With this option, you can set the constructor selection rule used to determine which constructor the container should use for instantiation."),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"prefermostparameters"},(0,a.kt)("inlineCode",{parentName:"h3"},"PreferMostParameters")),(0,a.kt)("p",null,"It prefers the constructor which has the most extended parameter list.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithConstructorSelectionRule(\n        Rules.ConstructorSelection.PreferMostParameters));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"preferleastparameters"},(0,a.kt)("inlineCode",{parentName:"h3"},"PreferLeastParameters")),(0,a.kt)("p",null,"It prefers the constructor which has the shortest parameter list.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithConstructorSelectionRule(\n        Rules.ConstructorSelection.PreferLeastParameters));\n")))),(0,a.kt)("h2",{id:"registration-behavior"},"Registration behavior"),(0,a.kt)("p",null,"With this option, you can set the actual behavior used when a new service is registered into the container. These options do not affect named registrations."),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"skipduplications"},(0,a.kt)("inlineCode",{parentName:"h3"},"SkipDuplications")),(0,a.kt)("p",null,"The container will skip new registrations when the given ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type")," is already registered.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.SkipDuplications));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"throwexception"},(0,a.kt)("inlineCode",{parentName:"h3"},"ThrowException")),(0,a.kt)("p",null,"The container throws an ",(0,a.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#servicealreadyregisteredexception"},"exception")," when the given ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type")," is already registered.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.ThrowException));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"replaceexisting"},(0,a.kt)("inlineCode",{parentName:"h3"},"ReplaceExisting")),(0,a.kt)("p",null,"The container will replace the already ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-registration--registered-service"},"registered service")," with the given one when they have the same ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type"),".")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.ReplaceExisting));\n")))),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("h3",{id:"preserveduplications"},(0,a.kt)("inlineCode",{parentName:"h3"},"PreserveDuplications")),(0,a.kt)("p",null,"The container will keep registering the new services with the same ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type"),".")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithRegistrationBehavior(\n        Rules.RegistrationBehavior.PreserveDuplications));\n")))),(0,a.kt)("h2",{id:"default-lifetime"},"Default lifetime"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can set the default lifetime used when a service doesn't have a configured one.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithDefaultLifetime(Lifetimes.Scoped));\n")))),(0,a.kt)("h2",{id:"lifetime-validation"},"Lifetime validation"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can enable or disable the life-span and ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#root-scope"},"root scope")," resolution ",(0,a.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#lifetime-validation"},"validation")," on the dependency tree.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithLifetimeValidation());\n")))),(0,a.kt)("h2",{id:"conventional-resolution"},"Conventional resolution"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can enable or disable conventional resolution, which means the container treats the constructor/method parameter or member names as dependency names used by ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#named-resolution"},"named resolution"),".")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .TreatParameterAndMemberNameAsDependencyName());\n")))),(0,a.kt)("h2",{id:"using-named-service-for-un-named-requests"},"Using named service for un-named requests"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can enable or disable the selection of named registrations when the resolution request is un-named but with the same type.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithNamedDependencyResolutionForUnNamedRequests());\n")))),(0,a.kt)("h2",{id:"default-value-injection"},"Default value injection"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can enable or disable the default value injection.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithDefaultValueInjection());\n")))),(0,a.kt)("h2",{id:"unknown-type-resolution"},"Unknown type resolution"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can enable or disable the resolution of unregistered types. You can also use a configurator delegate to configure the registrations the container will create from the unknown types.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithUnknownTypeResolution(config => config.AsImplementedTypes()));\n")))),(0,a.kt)("h2",{id:"custom-compiler"},"Custom compiler"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can set an external expression tree compiler. It can be useful on platforms where the IL generator modules are not available; therefore, the expression compiler in Stashbox couldn't work.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithExpressionCompiler(\n        Rules.ExpressionCompilers.MicrosoftExpressionCompiler));\n")))),(0,a.kt)("h2",{id:"re-build-singletons-in-child-containers"},"Re-build singletons in child containers"),(0,a.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"With this option, you can enable or disable the re-building of singletons in child containers. It allows the child containers to override singleton dependencies in the parent.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"new StashboxContainer(options => options\n    .WithReBuildSingletonsInChildContainer());\n")))),(0,a.kt)("admonition",{type:"note"},(0,a.kt)("p",{parentName:"admonition"},"This feature is not affecting the already built singleton instances in the parent.")))}d.isMDXComponent=!0}}]);