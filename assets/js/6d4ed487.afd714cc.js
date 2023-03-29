"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[302],{3905:(e,n,t)=>{t.d(n,{Zo:()=>c,kt:()=>m});var a=t(7294);function r(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function i(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);n&&(a=a.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,a)}return t}function l(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?i(Object(t),!0).forEach((function(n){r(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):i(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function o(e,n){if(null==e)return{};var t,a,r=function(e,n){if(null==e)return{};var t,a,r={},i=Object.keys(e);for(a=0;a<i.length;a++)t=i[a],n.indexOf(t)>=0||(r[t]=e[t]);return r}(e,n);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(a=0;a<i.length;a++)t=i[a],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(r[t]=e[t])}return r}var s=a.createContext({}),d=function(e){var n=a.useContext(s),t=n;return e&&(t="function"==typeof e?e(n):l(l({},n),e)),t},c=function(e){var n=d(e.components);return a.createElement(s.Provider,{value:n},e.children)},p="mdxType",v={inlineCode:"code",wrapper:function(e){var n=e.children;return a.createElement(a.Fragment,{},n)}},u=a.forwardRef((function(e,n){var t=e.components,r=e.mdxType,i=e.originalType,s=e.parentName,c=o(e,["components","mdxType","originalType","parentName"]),p=d(t),u=r,m=p["".concat(s,".").concat(u)]||p[u]||v[u]||i;return t?a.createElement(m,l(l({ref:n},c),{},{components:t})):a.createElement(m,l({ref:n},c))}));function m(e,n){var t=arguments,r=n&&n.mdxType;if("string"==typeof e||r){var i=t.length,l=new Array(i);l[0]=u;var o={};for(var s in n)hasOwnProperty.call(n,s)&&(o[s]=n[s]);o.originalType=e,o[p]="string"==typeof e?e:r,l[1]=o;for(var d=2;d<i;d++)l[d]=t[d];return a.createElement.apply(null,l)}return a.createElement.apply(null,t)}u.displayName="MDXCreateElement"},5162:(e,n,t)=>{t.d(n,{Z:()=>l});var a=t(7294),r=t(6010);const i="tabItem_Ymn6";function l(e){let{children:n,hidden:t,className:l}=e;return a.createElement("div",{role:"tabpanel",className:(0,r.Z)(i,l),hidden:t},n)}},5488:(e,n,t)=>{t.d(n,{Z:()=>u});var a=t(7462),r=t(7294),i=t(6010),l=t(2389),o=t(7392),s=t(7094),d=t(2466);const c="tabList__CuJ",p="tabItem_LNqP";function v(e){const{lazy:n,block:t,defaultValue:l,values:v,groupId:u,className:m}=e,g=r.Children.map(e.children,(e=>{if((0,r.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),E=v??g.map((e=>{let{props:{value:n,label:t,attributes:a}}=e;return{value:n,label:t,attributes:a}})),h=(0,o.l)(E,((e,n)=>e.value===n.value));if(h.length>0)throw new Error(`Docusaurus error: Duplicate values "${h.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const f=null===l?l:l??g.find((e=>e.props.default))?.props.value??g[0].props.value;if(null!==f&&!E.some((e=>e.value===f)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${f}" but none of its children has the corresponding value. Available values are: ${E.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:k,setTabGroupChoices:y}=(0,s.U)(),[I,b]=(0,r.useState)(f),N=[],{blockElementScrollPositionUntilNextRender:C}=(0,d.o5)();if(null!=u){const e=k[u];null!=e&&e!==I&&E.some((n=>n.value===e))&&b(e)}const w=e=>{const n=e.currentTarget,t=N.indexOf(n),a=E[t].value;a!==I&&(C(n),b(a),null!=u&&y(u,String(a)))},U=e=>{let n=null;switch(e.key){case"Enter":w(e);break;case"ArrowRight":{const t=N.indexOf(e.currentTarget)+1;n=N[t]??N[0];break}case"ArrowLeft":{const t=N.indexOf(e.currentTarget)-1;n=N[t]??N[N.length-1];break}}n?.focus()};return r.createElement("div",{className:(0,i.Z)("tabs-container",c)},r.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,i.Z)("tabs",{"tabs--block":t},m)},E.map((e=>{let{value:n,label:t,attributes:l}=e;return r.createElement("li",(0,a.Z)({role:"tab",tabIndex:I===n?0:-1,"aria-selected":I===n,key:n,ref:e=>N.push(e),onKeyDown:U,onClick:w},l,{className:(0,i.Z)("tabs__item",p,l?.className,{"tabs__item--active":I===n})}),t??n)}))),n?(0,r.cloneElement)(g.filter((e=>e.props.value===I))[0],{className:"margin-top--md"}):r.createElement("div",{className:"margin-top--md"},g.map(((e,n)=>(0,r.cloneElement)(e,{key:n,hidden:e.props.value!==I})))))}function u(e){const n=(0,l.Z)();return r.createElement(v,(0,a.Z)({key:String(n)},e))}},8846:(e,n,t)=>{t.d(n,{Z:()=>o});var a=t(7294);const r="codeDescContainer_ie8f",i="desc_jyqI",l="example_eYlF";function o(e){let{children:n}=e,t=a.Children.toArray(n).filter((e=>e));return a.createElement("div",{className:r},a.createElement("div",{className:i},t[0]),a.createElement("div",{className:l},t[1]))}},7956:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>p,contentTitle:()=>d,default:()=>m,frontMatter:()=>s,metadata:()=>c,toc:()=>v});var a=t(7462),r=(t(7294),t(3905)),i=t(8846),l=t(5488),o=t(5162);const s={},d="Generics",c={unversionedId:"advanced/generics",id:"advanced/generics",title:"Generics",description:"This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, generic decorators, conditions based on generic constraints, and variance.",source:"@site/docs/advanced/generics.md",sourceDirName:"advanced",slug:"/advanced/generics",permalink:"/stashbox/docs/advanced/generics",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/generics.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1680047976,formattedLastUpdatedAt:"Mar 28, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Container configuration",permalink:"/stashbox/docs/configuration/container-configuration"},next:{title:"Decorators",permalink:"/stashbox/docs/advanced/decorators"}},p={},v=[{value:"Closed-generics",id:"closed-generics",level:2},{value:"Open-generics",id:"open-generics",level:2},{value:"Generic constraints",id:"generic-constraints",level:2},{value:"Variance",id:"variance",level:2}],u={toc:v};function m(e){let{components:n,...t}=e;return(0,r.kt)("wrapper",(0,a.Z)({},u,t,{components:n,mdxType:"MDXLayout"}),(0,r.kt)("h1",{id:"generics"},"Generics"),(0,r.kt)("p",null,"This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, ",(0,r.kt)("a",{parentName:"p",href:"/docs/advanced/decorators#generic-decorators"},"generic decorators"),", conditions based on generic constraints, and variance."),(0,r.kt)("h2",{id:"closed-generics"},"Closed-generics"),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"The registration of a closed-generic type does not differ from registering a simple non-generic service."),(0,r.kt)("p",null,"You have all options available that you saw at the ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/basics"},"basic")," and ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/advanced-registration"},"advanced registration")," flows.")),(0,r.kt)("div",null,(0,r.kt)(l.Z,{groupId:"generic-runtime-apis",mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Generic API",label:"Generic API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IValidator<User>, UserValidator>();\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n"))),(0,r.kt)(o.Z,{value:"Runtime type API",label:"Runtime type API",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IValidator<User>), typeof(UserValidator));\nobject validator = container.Resolve(typeof(IValidator<User>));\n")))))),(0,r.kt)("h2",{id:"open-generics"},"Open-generics"),(0,r.kt)("p",null,"The registration of an open-generic type differs from registering a closed-generic one as C# doesn't allow the usage of open-generic types in generic method parameters. We have to get a runtime type from the open-generic type first with ",(0,r.kt)("inlineCode",{parentName:"p"},"typeof()"),"."),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"Open-generic types could help in such scenarios where you have generic interface-implementation pairs with numerous generic parameter variations. The registration of those different versions would look like this: ")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IValidator<User>, Validator<User>>();\ncontainer.Register<IValidator<Role>, Validator<Role>>();\ncontainer.Register<IValidator<Company>, Validator<Company>>();\n// and so on...\n")))),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"Rather than doing that, you can register your type's generic definition and let Stashbox bind the type parameters for you. When a matching closed ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," is requested, the container will construct an equivalent closed-generic implementation.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register(typeof(IValidator<>), typeof(Validator<>));\n// Validator<User> will be returned.\nIValidator<User> userValidator = container.Resolve<IValidator<User>>();\n// Validator<Role> will be returned.\nIValidator<Role> roleValidator = container.Resolve<IValidator<Role>>();\n")))),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("p",null,"A registered closed-generic type always has priority over an open-generic type at service selection.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IValidator<User>, UserValidator>();\ncontainer.Register(typeof(IValidator<>), typeof(Validator<>));\n// UserValidator will be returned.\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n")))),(0,r.kt)("h2",{id:"generic-constraints"},"Generic constraints"),(0,r.kt)("p",null,"In the following examples, you can see how the container handles generic constraints during service resolution. Constraints can be used for ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#conditional-resolution"},"conditional resolution")," including collection filters. "),(0,r.kt)(l.Z,{mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Conditional resolution",label:"Conditional resolution",mdxType:"TabItem"},(0,r.kt)("p",null,"The container chooses ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler")," because it is the only one that has a constraint satisfied by the requested ",(0,r.kt)("inlineCode",{parentName:"p"},"UserUpdatedEvent")," generic parameter as it's implementing ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent"),"."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandler will be UpdatedEventHandler<ConstraintArgument>\nIEventHandler<UserUpdatedEvent> eventHandler = container.Resolve<IEventHandler<UserUpdatedEvent>>();\n"))),(0,r.kt)(o.Z,{value:"Collection filter",label:"Collection filter",mdxType:"TabItem"},(0,r.kt)("p",null,"This example shows how the container is filtering out those services from the returned collection that does not satisfy the given generic constraint needed to create the closed generic type."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandlers will contain only UpdatedEventHandler<ConstraintArgument>\nIEnumerable<IEventHandler<UserUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<UserUpdatedEvent>>();\n")))),(0,r.kt)("h2",{id:"variance"},"Variance"),(0,r.kt)("p",null,"Since .NET Framework 4.0, C# supports ",(0,r.kt)("a",{parentName:"p",href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/"},"covariance and contravariance")," in generic interfaces and delegates and allows implicit conversion of generic type parameters. In this section, we'll focus on variance in generic interfaces. "),(0,r.kt)("p",null,(0,r.kt)("a",{parentName:"p",href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces"},"Here")," you can read more about how to create variant generic interfaces, and the following example will show how you can use them with Stashbox."),(0,r.kt)(l.Z,{mdxType:"Tabs"},(0,r.kt)(o.Z,{value:"Contravariance",label:"Contravariance",mdxType:"TabItem"},(0,r.kt)("p",null,(0,r.kt)("strong",{parentName:"p"},"Contravariance")," only allows argument types that are less derived than that defined by the generic parameters. You can declare a generic type parameter contravariant by using the ",(0,r.kt)("inlineCode",{parentName:"p"},"in")," keyword."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"// contravariant generic event handler interface\ninterface IEventHandler<in TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<IUpdatedEvent>>();\n")),(0,r.kt)("p",null,"Despite the fact that only ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," implementations were requested, the result contains both ",(0,r.kt)("inlineCode",{parentName:"p"},"GeneralEventHandler")," and ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler"),". As ",(0,r.kt)("inlineCode",{parentName:"p"},"TEvent")," is declared ",(0,r.kt)("strong",{parentName:"p"},"contravariant")," with the ",(0,r.kt)("inlineCode",{parentName:"p"},"in")," keyword, and ",(0,r.kt)("inlineCode",{parentName:"p"},"IGeneralEvent")," is less derived than ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent"),", ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," implementations can be part of ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," collections."),(0,r.kt)("p",null,"If we request ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>"),", only ",(0,r.kt)("inlineCode",{parentName:"p"},"GeneralEventHandler")," would be returned, because ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent")," is more derived, so ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," implementations are not fit into ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," collections. ")),(0,r.kt)(o.Z,{value:"Covariance",label:"Covariance",mdxType:"TabItem"},(0,r.kt)("p",null,(0,r.kt)("strong",{parentName:"p"},"Covariance")," only allows argument types that are more derived than that defined by the generic parameters. You can declare a generic type parameter covariant by using the ",(0,r.kt)("inlineCode",{parentName:"p"},"out")," keyword."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"// covariant generic event handler interface\ninterface IEventHandler<out TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IGeneralEvent>> eventHandlers = container.ResolveAll<IEventHandler<IGeneralEvent>>();\n")),(0,r.kt)("p",null,"Despite the fact that only ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," implementations were requested, the result contains both ",(0,r.kt)("inlineCode",{parentName:"p"},"GeneralEventHandler")," and ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler"),". As ",(0,r.kt)("inlineCode",{parentName:"p"},"TEvent")," is declared ",(0,r.kt)("strong",{parentName:"p"},"covariant")," with the ",(0,r.kt)("inlineCode",{parentName:"p"},"out")," keyword, and ",(0,r.kt)("inlineCode",{parentName:"p"},"IUpdatedEvent")," is more derived than ",(0,r.kt)("inlineCode",{parentName:"p"},"IGeneralEvent"),", ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," implementations can be part of ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," collections."),(0,r.kt)("p",null,"If we request ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>"),", only ",(0,r.kt)("inlineCode",{parentName:"p"},"UpdatedEventHandler")," would be returned, because ",(0,r.kt)("inlineCode",{parentName:"p"},"IGeneralEvent")," is less derived, so ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IGeneralEvent>")," implementations are not fit into ",(0,r.kt)("inlineCode",{parentName:"p"},"IEventHandler<IUpdatedEvent>")," collections."))))}m.isMDXComponent=!0}}]);