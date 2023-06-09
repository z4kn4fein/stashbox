"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[302],{3905:(e,n,t)=>{t.d(n,{Zo:()=>c,kt:()=>m});var a=t(7294);function r(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function l(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);n&&(a=a.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,a)}return t}function i(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?l(Object(t),!0).forEach((function(n){r(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):l(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function o(e,n){if(null==e)return{};var t,a,r=function(e,n){if(null==e)return{};var t,a,r={},l=Object.keys(e);for(a=0;a<l.length;a++)t=l[a],n.indexOf(t)>=0||(r[t]=e[t]);return r}(e,n);if(Object.getOwnPropertySymbols){var l=Object.getOwnPropertySymbols(e);for(a=0;a<l.length;a++)t=l[a],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(r[t]=e[t])}return r}var s=a.createContext({}),d=function(e){var n=a.useContext(s),t=n;return e&&(t="function"==typeof e?e(n):i(i({},n),e)),t},c=function(e){var n=d(e.components);return a.createElement(s.Provider,{value:n},e.children)},p="mdxType",u={inlineCode:"code",wrapper:function(e){var n=e.children;return a.createElement(a.Fragment,{},n)}},v=a.forwardRef((function(e,n){var t=e.components,r=e.mdxType,l=e.originalType,s=e.parentName,c=o(e,["components","mdxType","originalType","parentName"]),p=d(t),v=r,m=p["".concat(s,".").concat(v)]||p[v]||u[v]||l;return t?a.createElement(m,i(i({ref:n},c),{},{components:t})):a.createElement(m,i({ref:n},c))}));function m(e,n){var t=arguments,r=n&&n.mdxType;if("string"==typeof e||r){var l=t.length,i=new Array(l);i[0]=v;var o={};for(var s in n)hasOwnProperty.call(n,s)&&(o[s]=n[s]);o.originalType=e,o[p]="string"==typeof e?e:r,i[1]=o;for(var d=2;d<l;d++)i[d]=t[d];return a.createElement.apply(null,i)}return a.createElement.apply(null,t)}v.displayName="MDXCreateElement"},5162:(e,n,t)=>{t.d(n,{Z:()=>i});var a=t(7294),r=t(6010);const l="tabItem_Ymn6";function i(e){let{children:n,hidden:t,className:i}=e;return a.createElement("div",{role:"tabpanel",className:(0,r.Z)(l,i),hidden:t},n)}},4866:(e,n,t)=>{t.d(n,{Z:()=>I});var a=t(7462),r=t(7294),l=t(6010),i=t(2466),o=t(6550),s=t(1980),d=t(7392),c=t(12);function p(e){return function(e){return r.Children.map(e,(e=>{if(!e||(0,r.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}(e).map((e=>{let{props:{value:n,label:t,attributes:a,default:r}}=e;return{value:n,label:t,attributes:a,default:r}}))}function u(e){const{values:n,children:t}=e;return(0,r.useMemo)((()=>{const e=n??p(t);return function(e){const n=(0,d.l)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,t])}function v(e){let{value:n,tabValues:t}=e;return t.some((e=>e.value===n))}function m(e){let{queryString:n=!1,groupId:t}=e;const a=(0,o.k6)(),l=function(e){let{queryString:n=!1,groupId:t}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!t)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return t??null}({queryString:n,groupId:t});return[(0,s._X)(l),(0,r.useCallback)((e=>{if(!l)return;const n=new URLSearchParams(a.location.search);n.set(l,e),a.replace({...a.location,search:n.toString()})}),[l,a])]}function f(e){const{defaultValue:n,queryString:t=!1,groupId:a}=e,l=u(e),[i,o]=(0,r.useState)((()=>function(e){let{defaultValue:n,tabValues:t}=e;if(0===t.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!v({value:n,tabValues:t}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${t.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const a=t.find((e=>e.default))??t[0];if(!a)throw new Error("Unexpected error: 0 tabValues");return a.value}({defaultValue:n,tabValues:l}))),[s,d]=m({queryString:t,groupId:a}),[p,f]=function(e){let{groupId:n}=e;const t=function(e){return e?`docusaurus.tab.${e}`:null}(n),[a,l]=(0,c.Nk)(t);return[a,(0,r.useCallback)((e=>{t&&l.set(e)}),[t,l])]}({groupId:a}),g=(()=>{const e=s??p;return v({value:e,tabValues:l})?e:null})();(0,r.useLayoutEffect)((()=>{g&&o(g)}),[g]);return{selectedValue:i,selectValue:(0,r.useCallback)((e=>{if(!v({value:e,tabValues:l}))throw new Error(`Can't select invalid tab value=${e}`);o(e),d(e),f(e)}),[d,f,l]),tabValues:l}}var g=t(2389);const h="tabList__CuJ",E="tabItem_LNqP";function y(e){let{className:n,block:t,selectedValue:o,selectValue:s,tabValues:d}=e;const c=[],{blockElementScrollPositionUntilNextRender:p}=(0,i.o5)(),u=e=>{const n=e.currentTarget,t=c.indexOf(n),a=d[t].value;a!==o&&(p(n),s(a))},v=e=>{let n=null;switch(e.key){case"Enter":u(e);break;case"ArrowRight":{const t=c.indexOf(e.currentTarget)+1;n=c[t]??c[0];break}case"ArrowLeft":{const t=c.indexOf(e.currentTarget)-1;n=c[t]??c[c.length-1];break}}n?.focus()};return r.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,l.Z)("tabs",{"tabs--block":t},n)},d.map((e=>{let{value:n,label:t,attributes:i}=e;return r.createElement("li",(0,a.Z)({role:"tab",tabIndex:o===n?0:-1,"aria-selected":o===n,key:n,ref:e=>c.push(e),onKeyDown:v,onClick:u},i,{className:(0,l.Z)("tabs__item",E,i?.className,{"tabs__item--active":o===n})}),t??n)})))}function b(e){let{lazy:n,children:t,selectedValue:a}=e;const l=(Array.isArray(t)?t:[t]).filter(Boolean);if(n){const e=l.find((e=>e.props.value===a));return e?(0,r.cloneElement)(e,{className:"margin-top--md"}):null}return r.createElement("div",{className:"margin-top--md"},l.map(((e,n)=>(0,r.cloneElement)(e,{key:n,hidden:e.props.value!==a}))))}function k(e){const n=f(e);return r.createElement("div",{className:(0,l.Z)("tabs-container",h)},r.createElement(y,(0,a.Z)({},e,n)),r.createElement(b,(0,a.Z)({},e,n)))}function I(e){const n=(0,g.Z)();return r.createElement(k,(0,a.Z)({key:String(n)},e))}},8846:(e,n,t)=>{t.d(n,{Z:()=>o});var a=t(7294);const r="codeDescContainer_ie8f",l="desc_jyqI",i="example_eYlF";function o(e){let{children:n}=e,t=a.Children.toArray(n).filter((e=>e));return a.createElement("div",{className:r},a.createElement("div",{className:l},t[0]),a.createElement("div",{className:i},t[1]))}},7956:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>p,contentTitle:()=>d,default:()=>m,frontMatter:()=>s,metadata:()=>c,toc:()=>u});var a=t(7462),r=(t(7294),t(3905)),l=t(8846),i=t(4866),o=t(5162);const s={},d="Generics",c={unversionedId:"advanced/generics",id:"advanced/generics",title:"Generics",description:"This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, generic decorators, conditions based on generic constraints, and variance.",source:"@site/docs/advanced/generics.md",sourceDirName:"advanced",slug:"/advanced/generics",permalink:"/stashbox/docs/advanced/generics",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/generics.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1686304983,formattedLastUpdatedAt:"Jun 9, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Container configuration",permalink:"/stashbox/docs/configuration/container-configuration"},next:{title:"Decorators",permalink:"/stashbox/docs/advanced/decorators"}},p={},u=[{value:"Closed-generics",id:"closed-generics",level:2},{value:"Open-generics",id:"open-generics",level:2},{value:"Generic constraints",id:"generic-constraints",level:2},{value:"Variance",id:"variance",level:2}],v={toc:u};function m(e){let{components:n,...t}=e;return(0,r.kt)("wrapper",(0,a.Z)({},v,t,{components:n,mdxType:"MDXLayout"}),(0,r.kt)("h1",{id:"generics"},"Generics"),(0,r.kt)("p",null,"This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, ",(0,r.kt)("a",{parentName:"p",href:"/docs/advanced/decorators#generic-decorators"},"generic decorators"),", conditions based on generic constraints, and variance."),(0,r.kt)("h2",{id:"closed-generics"},"Closed-generics"),(0,r.kt)(l.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"The registration of a closed-generic type does not differ from registering a simple non-generic service."),(0,r.kt)("p",null,"You have all options available that you saw at the ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/basics"},"basic")," and ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/advanced-registration"},"advanced registration")," flows.")),(0,r.kt)("div",null,(0,r.kt)(i.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IValidator<User>, UserValidator>();\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n"))),(0,r.kt)(o.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IValidator<User>), typeof(UserValidator));\nobject validator = container.Resolve(typeof(IValidator<User>));\n")))))),(0,r.kt)("h2",{id:"open-generics"},"Open-generics"),(0,r.kt)("p",null,"The registration of an open-generic type differs from registering a closed-generic one as C# doesn't allow the usage of open-generic types in generic method parameters. We have to get a runtime type from the open-generic type first with ",(0,r.kt)("inlineCode",{parentName:"p"},"typeof()"),"."),(0,r.kt)(l.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"Open-generic types could help in such scenarios where you have generic interface-implementation pairs with numerous generic parameter variations. The registration of those different versions would look like this: ")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IValidator<User>, Validator<User>>();\ncontainer.Register<IValidator<Role>, Validator<Role>>();\ncontainer.Register<IValidator<Company>, Validator<Company>>();\n// and so on...\n")))),(0,r.kt)(l.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"Rather than doing that, you can register your type's generic definition and let Stashbox bind the type parameters for you. When a matching closed ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," is requested, the container will construct an equivalent closed-generic implementation.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IValidator<>), typeof(Validator<>));\n// Validator<User> will be returned.\nIValidator<User> userValidator = container.Resolve<IValidator<User>>();\n// Validator<Role> will be returned.\nIValidator<Role> roleValidator = container.Resolve<IValidator<Role>>();\n")))),(0,r.kt)(l.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"A registered closed-generic type always has priority over an open-generic type at service selection.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IValidator<User>, UserValidator>();\ncontainer.Register(typeof(IValidator<>), typeof(Validator<>));\n// UserValidator will be returned.\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n")))),(0,r.kt)("h2",{id:"generic-constraints"},"Generic constraints"),(0,r.kt)("p",null,"In the following examples, you can see how the container handles generic constraints during service resolution. Constraints can be used for ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#conditional-resolution"},"conditional resolution")," including collection filters. "),(0,r.kt)(i.Z,{mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Conditional resolution",label:"Conditional resolution",mdxType:"TabItem"},(0,r.kt)("p",null,"The container chooses ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler")," because it is the only one that has a constraint satisfied by the requested ",(0,r.kt)("inlineCode",{parentName:"p"},"UserUpdatedEvent")," generic parameter as it's implementing ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent"),"."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandler will be UpdatedEventHandler<ConstraintArgument>\nIEventHandler<UserUpdatedEvent> eventHandler = container.Resolve<IEventHandler<UserUpdatedEvent>>();\n"))),(0,r.kt)(o.Z,{value:"Collection filter",label:"Collection filter",mdxType:"TabItem"},(0,r.kt)("p",null,"This example shows how the container is filtering out those services from the returned collection that does not satisfy the given generic constraint needed to create the closed generic type."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandlers will contain only UpdatedEventHandler<ConstraintArgument>\nIEnumerable<IEventHandler<UserUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<UserUpdatedEvent>>();\n")))),(0,r.kt)("h2",{id:"variance"},"Variance"),(0,r.kt)("p",null,"Since .NET Framework 4.0, C# supports ",(0,r.kt)("a",{parentName:"p",href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/"},"covariance and contravariance")," in generic interfaces and delegates and allows implicit conversion of generic type parameters. In this section, we'll focus on variance in generic interfaces. "),(0,r.kt)("p",null,(0,r.kt)("a",{parentName:"p",href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces"},"Here")," you can read more about how to create variant generic interfaces, and the following example will show how you can use them with Stashbox."),(0,r.kt)(i.Z,{mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Contravariance",label:"Contravariance",mdxType:"TabItem"},(0,r.kt)("p",null,(0,r.kt)("strong",{parentName:"p"},"Contravariance")," only allows argument types that are less derived than that defined by the generic parameters. You can declare a generic type parameter contravariant by using the ",(0,r.kt)("inlineCode",{parentName:"p"},"in")," keyword."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"// contravariant generic event handler interface\ninterface IEventHandler<in TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<IUpdatedEvent>>();\n")),(0,r.kt)("p",null,"Despite the fact that only ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," implementations were requested, the result contains both ",(0,r.kt)("inlineCode",{parentName:"p"},"GeneralEventHandler")," and ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler"),". As ",(0,r.kt)("inlineCode",{parentName:"p"},"TEvent")," is declared ",(0,r.kt)("strong",{parentName:"p"},"contravariant")," with the ",(0,r.kt)("inlineCode",{parentName:"p"},"in")," keyword, and ",(0,r.kt)("inlineCode",{parentName:"p"},"IGeneralEvent")," is less derived than ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent"),", ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," implementations can be part of ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," collections."),(0,r.kt)("p",null,"If we request ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>"),", only ",(0,r.kt)("inlineCode",{parentName:"p"},"GeneralEventHandler")," would be returned, because ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent")," is more derived, so ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," implementations are not fit into ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," collections. ")),(0,r.kt)(o.Z,{value:"Covariance",label:"Covariance",mdxType:"TabItem"},(0,r.kt)("p",null,(0,r.kt)("strong",{parentName:"p"},"Covariance")," only allows argument types that are more derived than that defined by the generic parameters. You can declare a generic type parameter covariant by using the ",(0,r.kt)("inlineCode",{parentName:"p"},"out")," keyword."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"// covariant generic event handler interface\ninterface IEventHandler<out TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IGeneralEvent>> eventHandlers = container.ResolveAll<IEventHandler<IGeneralEvent>>();\n")),(0,r.kt)("p",null,"Despite the fact that only ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," implementations were requested, the result contains both ",(0,r.kt)("inlineCode",{parentName:"p"},"GeneralEventHandler")," and ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler"),". As ",(0,r.kt)("inlineCode",{parentName:"p"},"TEvent")," is declared ",(0,r.kt)("strong",{parentName:"p"},"covariant")," with the ",(0,r.kt)("inlineCode",{parentName:"p"},"out")," keyword, and ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent")," is more derived than ",(0,r.kt)("inlineCode",{parentName:"p"},"IGeneralEvent"),", ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," implementations can be part of ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," collections."),(0,r.kt)("p",null,"If we request ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>"),", only ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler")," would be returned, because ",(0,r.kt)("inlineCode",{parentName:"p"},"IGeneralEvent")," is less derived, so ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," implementations are not fit into ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," collections."))))}m.isMDXComponent=!0}}]);