"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[524],{3905:(e,t,n)=>{n.d(t,{Zo:()=>u,kt:()=>g});var a=n(7294);function r(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function i(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function l(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?i(Object(n),!0).forEach((function(t){r(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):i(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function o(e,t){if(null==e)return{};var n,a,r=function(e,t){if(null==e)return{};var n,a,r={},i=Object.keys(e);for(a=0;a<i.length;a++)n=i[a],t.indexOf(n)>=0||(r[n]=e[n]);return r}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(a=0;a<i.length;a++)n=i[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(r[n]=e[n])}return r}var s=a.createContext({}),c=function(e){var t=a.useContext(s),n=t;return e&&(n="function"==typeof e?e(t):l(l({},t),e)),n},u=function(e){var t=c(e.components);return a.createElement(s.Provider,{value:t},e.children)},p="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},m=a.forwardRef((function(e,t){var n=e.components,r=e.mdxType,i=e.originalType,s=e.parentName,u=o(e,["components","mdxType","originalType","parentName"]),p=c(n),m=r,g=p["".concat(s,".").concat(m)]||p[m]||d[m]||i;return n?a.createElement(g,l(l({ref:t},u),{},{components:n})):a.createElement(g,l({ref:t},u))}));function g(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var i=n.length,l=new Array(i);l[0]=m;var o={};for(var s in t)hasOwnProperty.call(t,s)&&(o[s]=t[s]);o.originalType=e,o[p]="string"==typeof e?e:r,l[1]=o;for(var c=2;c<i;c++)l[c]=n[c];return a.createElement.apply(null,l)}return a.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>l});var a=n(7294),r=n(6010);const i="tabItem_Ymn6";function l(e){let{children:t,hidden:n,className:l}=e;return a.createElement("div",{role:"tabpanel",className:(0,r.Z)(i,l),hidden:n},t)}},5488:(e,t,n)=>{n.d(t,{Z:()=>m});var a=n(7462),r=n(7294),i=n(6010),l=n(2389),o=n(7392),s=n(7094),c=n(2466);const u="tabList__CuJ",p="tabItem_LNqP";function d(e){const{lazy:t,block:n,defaultValue:l,values:d,groupId:m,className:g}=e,b=r.Children.map(e.children,(e=>{if((0,r.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),v=d??b.map((e=>{let{props:{value:t,label:n,attributes:a}}=e;return{value:t,label:n,attributes:a}})),h=(0,o.l)(v,((e,t)=>e.value===t.value));if(h.length>0)throw new Error(`Docusaurus error: Duplicate values "${h.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const k=null===l?l:l??b.find((e=>e.props.default))?.props.value??b[0].props.value;if(null!==k&&!v.some((e=>e.value===k)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${k}" but none of its children has the corresponding value. Available values are: ${v.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:y,setTabGroupChoices:f}=(0,s.U)(),[N,I]=(0,r.useState)(k),T=[],{blockElementScrollPositionUntilNextRender:C}=(0,c.o5)();if(null!=m){const e=y[m];null!=e&&e!==N&&v.some((t=>t.value===e))&&I(e)}const w=e=>{const t=e.currentTarget,n=T.indexOf(t),a=v[n].value;a!==N&&(C(t),I(a),null!=m&&f(m,String(a)))},E=e=>{let t=null;switch(e.key){case"Enter":w(e);break;case"ArrowRight":{const n=T.indexOf(e.currentTarget)+1;t=T[n]??T[0];break}case"ArrowLeft":{const n=T.indexOf(e.currentTarget)-1;t=T[n]??T[T.length-1];break}}t?.focus()};return r.createElement("div",{className:(0,i.Z)("tabs-container",u)},r.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,i.Z)("tabs",{"tabs--block":n},g)},v.map((e=>{let{value:t,label:n,attributes:l}=e;return r.createElement("li",(0,a.Z)({role:"tab",tabIndex:N===t?0:-1,"aria-selected":N===t,key:t,ref:e=>T.push(e),onKeyDown:E,onClick:w},l,{className:(0,i.Z)("tabs__item",p,l?.className,{"tabs__item--active":N===t})}),n??t)}))),t?(0,r.cloneElement)(b.filter((e=>e.props.value===N))[0],{className:"margin-top--md"}):r.createElement("div",{className:"margin-top--md"},b.map(((e,t)=>(0,r.cloneElement)(e,{key:t,hidden:e.props.value!==N})))))}function m(e){const t=(0,l.Z)();return r.createElement(d,(0,a.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>o});var a=n(7294);const r="codeDescContainer_ie8f",i="desc_jyqI",l="example_eYlF";function o(e){let{children:t}=e,n=a.Children.toArray(t).filter((e=>e));return a.createElement("div",{className:r},a.createElement("div",{className:i},n[0]),a.createElement("div",{className:l},n[1]))}},9395:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>p,contentTitle:()=>c,default:()=>g,frontMatter:()=>s,metadata:()=>u,toc:()=>d});var a=n(7462),r=(n(7294),n(3905)),i=n(8846),l=n(5488),o=n(5162);const s={},c="Utilities",u={unversionedId:"diagnostics/utilities",id:"diagnostics/utilities",title:"Utilities",description:"Is registered?",source:"@site/docs/diagnostics/utilities.md",sourceDirName:"diagnostics",slug:"/diagnostics/utilities",permalink:"/stashbox/docs/diagnostics/utilities",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/diagnostics/utilities.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1672673299,formattedLastUpdatedAt:"Jan 2, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Validation",permalink:"/stashbox/docs/diagnostics/validation"}},p={},d=[{value:"Is registered?",id:"is-registered",level:2},{value:"<strong>Named</strong>",id:"named",level:4},{value:"Can resolve?",id:"can-resolve",level:2},{value:"Get all mappings",id:"get-all-mappings",level:2},{value:"Registration diagnostics",id:"registration-diagnostics",level:2}],m={toc:d};function g(e){let{components:t,...n}=e;return(0,r.kt)("wrapper",(0,a.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("h1",{id:"utilities"},"Utilities"),(0,r.kt)("h2",{id:"is-registered"},"Is registered?"),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"With the ",(0,r.kt)("inlineCode",{parentName:"p"},"IsRegistered()")," function, you can find out whether a service is registered into the container or not."),(0,r.kt)("p",null,"It returns ",(0,r.kt)("inlineCode",{parentName:"p"},"true")," only when the container has a registration with the given type (and name). It only checks the actual container's registrations. For every other cases, you should use the ",(0,r.kt)("inlineCode",{parentName:"p"},"CanResolve()")," method.")),(0,r.kt)("div",null,(0,r.kt)(l.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"bool isIJobRegistered = container.IsRegistered<IJob>();\n"))),(0,r.kt)(o.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"bool isIJobRegistered = container.IsRegistered(typeof(IJob));\n"))),(0,r.kt)(o.Z,{value:"Named",label:"Named",mdxType:"TabItem"},(0,r.kt)("h4",{id:"named"},(0,r.kt)("strong",{parentName:"h4"},"Named")),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'bool isIJobRegistered = container.IsRegistered<IJob>("DbBackup");\n')))))),(0,r.kt)("h2",{id:"can-resolve"},"Can resolve?"),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"There might be cases when rather than finding out that a service is registered, you are more interested in whether it's resolvable from the container's actual state or not."),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"CanResolve()")," returns ",(0,r.kt)("inlineCode",{parentName:"p"},"true")," only when at least one of the following is true:"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"The requested type is registered in the current or one of the parent containers."),(0,r.kt)("li",{parentName:"ul"},"The requested type is a closed generic type and its open generic definition is registered."),(0,r.kt)("li",{parentName:"ul"},"The requested type is a wrapper (",(0,r.kt)("inlineCode",{parentName:"li"},"IEnumerable<>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"Lazy<>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"Func<>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"KeyValuePair<,>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"ReadOnlyKeyValue<,>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"Metadata<,>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"ValueTuple<,>"),", or ",(0,r.kt)("inlineCode",{parentName:"li"},"Tuple<,>"),") and the underlying type is registered."),(0,r.kt)("li",{parentName:"ul"},"The requested type is not registered but it's resolvable and the ",(0,r.kt)("a",{parentName:"li",href:"/docs/configuration/container-configuration#unknown-type-resolution"},"unknown type resolution")," is enabled."))),(0,r.kt)("div",null,(0,r.kt)(l.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"bool isIJobResolvable = container.CanResolve<IJob>();\n"))),(0,r.kt)(o.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"bool isIJobResolvable = container.CanResolve(typeof(IJob));\n"))),(0,r.kt)(o.Z,{value:"Named",label:"Named",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'bool isIJobResolvable = container.CanResolve<IJob>("DbBackup");\n')))))),(0,r.kt)("h2",{id:"get-all-mappings"},"Get all mappings"),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"You can get all registrations in a key-value pair collection (where the key is the ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," and the value is the actual registration) by calling the ",(0,r.kt)("inlineCode",{parentName:"p"},".GetRegistrationMappings()")," method.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"IEnumerable<KeyValuePair<Type, ServiceRegistration>> mappings = \n    container.GetRegistrationMappings();\n")))),(0,r.kt)("h2",{id:"registration-diagnostics"},"Registration diagnostics"),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"You can get a much more readable version of the registration mappings by calling the ",(0,r.kt)("inlineCode",{parentName:"p"},".GetRegistrationDiagnostics()")," method."),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"RegistrationDiagnosticsInfo")," has an overridden ",(0,r.kt)("inlineCode",{parentName:"p"},".ToString()")," method that returns the mapping details formatted in a human-readable form.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>("DbBackupJob");\ncontainer.Register(typeof(IEventHandler<>), typeof(EventHandler<>));\n\nIEnumerable<RegistrationDiagnosticsInfo> diagnostics = \n    container.GetRegistrationDiagnostics();\n\ndiagnostics.ForEach(Console.WriteLine);\n// output:\n// IJob => DbBackup, name: DbBackupJob\n// IEventHandler<> => EventHandler<>, name: null\n')))))}g.isMDXComponent=!0}}]);