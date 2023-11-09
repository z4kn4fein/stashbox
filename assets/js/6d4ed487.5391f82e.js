"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[302],{1043:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>v,contentTitle:()=>d,default:()=>p,frontMatter:()=>o,metadata:()=>c,toc:()=>u});var r=t(5893),a=t(1151),s=t(8846),i=t(4866),l=t(5162);const o={},d="Generics",c={id:"advanced/generics",title:"Generics",description:"This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, generic decorators, conditions based on generic constraints, and variance.",source:"@site/docs/advanced/generics.md",sourceDirName:"advanced",slug:"/advanced/generics",permalink:"/stashbox/docs/advanced/generics",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/generics.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1699543594,formattedLastUpdatedAt:"Nov 9, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Container configuration",permalink:"/stashbox/docs/configuration/container-configuration"},next:{title:"Decorators",permalink:"/stashbox/docs/advanced/decorators"}},v={},u=[{value:"Closed-generics",id:"closed-generics",level:2},{value:"Open-generics",id:"open-generics",level:2},{value:"Generic constraints",id:"generic-constraints",level:2},{value:"Variance",id:"variance",level:2}];function h(e){const n={a:"a",code:"code",h1:"h1",h2:"h2",p:"p",pre:"pre",strong:"strong",...(0,a.a)(),...e.components};return(0,r.jsxs)(r.Fragment,{children:[(0,r.jsx)(n.h1,{id:"generics",children:"Generics"}),"\n",(0,r.jsxs)(n.p,{children:["This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, ",(0,r.jsx)(n.a,{href:"/docs/advanced/decorators#generic-decorators",children:"generic decorators"}),", conditions based on generic constraints, and variance."]}),"\n",(0,r.jsx)(n.h2,{id:"closed-generics",children:"Closed-generics"}),"\n",(0,r.jsxs)(s.Z,{children:[(0,r.jsxs)("div",{children:[(0,r.jsx)(n.p,{children:"The registration of a closed-generic type does not differ from registering a simple non-generic service."}),(0,r.jsxs)(n.p,{children:["You have all options available that you saw at the ",(0,r.jsx)(n.a,{href:"/docs/guides/basics",children:"basic"})," and ",(0,r.jsx)(n.a,{href:"/docs/guides/advanced-registration",children:"advanced registration"})," flows."]})]}),(0,r.jsx)("div",{children:(0,r.jsxs)(i.Z,{groupId:"generic-runtime-apis",children:[(0,r.jsx)(l.Z,{value:"Generic API",label:"Generic API",children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register<IValidator<User>, UserValidator>();\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n"})})}),(0,r.jsx)(l.Z,{value:"Runtime type API",label:"Runtime type API",children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register(typeof(IValidator<User>), typeof(UserValidator));\nobject validator = container.Resolve(typeof(IValidator<User>));\n"})})})]})})]}),"\n",(0,r.jsx)(n.h2,{id:"open-generics",children:"Open-generics"}),"\n",(0,r.jsxs)(n.p,{children:["The registration of an open-generic type differs from registering a closed-generic one as C# doesn't allow the usage of open-generic types in generic method parameters. We have to get a runtime type from the open-generic type first with ",(0,r.jsx)(n.code,{children:"typeof()"}),"."]}),"\n",(0,r.jsxs)(s.Z,{children:[(0,r.jsx)("div",{children:(0,r.jsx)(n.p,{children:"Open-generic types could help in such scenarios where you have generic interface-implementation pairs with numerous generic parameter variations. The registration of those different versions would look like this:"})}),(0,r.jsx)("div",{children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register<IValidator<User>, Validator<User>>();\ncontainer.Register<IValidator<Role>, Validator<Role>>();\ncontainer.Register<IValidator<Company>, Validator<Company>>();\n// and so on...\n"})})})]}),"\n",(0,r.jsxs)(s.Z,{children:[(0,r.jsx)("div",{children:(0,r.jsxs)(n.p,{children:["Rather than doing that, you can register your type's generic definition and let Stashbox bind the type parameters for you. When a matching closed ",(0,r.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," is requested, the container will construct an equivalent closed-generic implementation."]})}),(0,r.jsx)("div",{children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register(typeof(IValidator<>), typeof(Validator<>));\n// Validator<User> will be returned.\nIValidator<User> userValidator = container.Resolve<IValidator<User>>();\n// Validator<Role> will be returned.\nIValidator<Role> roleValidator = container.Resolve<IValidator<Role>>();\n"})})})]}),"\n",(0,r.jsxs)(s.Z,{children:[(0,r.jsx)("div",{children:(0,r.jsx)(n.p,{children:"A registered closed-generic type always has priority over an open-generic type at service selection."})}),(0,r.jsx)("div",{children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register<IValidator<User>, UserValidator>();\ncontainer.Register(typeof(IValidator<>), typeof(Validator<>));\n// UserValidator will be returned.\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n"})})})]}),"\n",(0,r.jsx)(n.h2,{id:"generic-constraints",children:"Generic constraints"}),"\n",(0,r.jsxs)(n.p,{children:["In the following examples, you can see how the container handles generic constraints during service resolution. Constraints can be used for ",(0,r.jsx)(n.a,{href:"/docs/guides/service-resolution#conditional-resolution",children:"conditional resolution"})," including collection filters."]}),"\n",(0,r.jsxs)(i.Z,{children:[(0,r.jsxs)(l.Z,{value:"Conditional resolution",label:"Conditional resolution",children:[(0,r.jsxs)(n.p,{children:["The container chooses ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"})," because it is the only one that has a constraint satisfied by the requested ",(0,r.jsx)(n.code,{children:"UserUpdatedEvent"})," generic parameter as it's implementing ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"}),"."]}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandler will be UpdatedEventHandler<ConstraintArgument>\nIEventHandler<UserUpdatedEvent> eventHandler = container.Resolve<IEventHandler<UserUpdatedEvent>>();\n"})})]}),(0,r.jsxs)(l.Z,{value:"Collection filter",label:"Collection filter",children:[(0,r.jsx)(n.p,{children:"This example shows how the container is filtering out those services from the returned collection that does not satisfy the given generic constraint needed to create the closed generic type."}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandlers will contain only UpdatedEventHandler<ConstraintArgument>\nIEnumerable<IEventHandler<UserUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<UserUpdatedEvent>>();\n"})})]})]}),"\n",(0,r.jsx)(n.h2,{id:"variance",children:"Variance"}),"\n",(0,r.jsxs)(n.p,{children:["Since .NET Framework 4.0, C# supports ",(0,r.jsx)(n.a,{href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/",children:"covariance and contravariance"})," in generic interfaces and delegates and allows implicit conversion of generic type parameters. In this section, we'll focus on variance in generic interfaces."]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.a,{href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces",children:"Here"})," you can read more about how to create variant generic interfaces, and the following example will show how you can use them with Stashbox."]}),"\n",(0,r.jsxs)(i.Z,{children:[(0,r.jsxs)(l.Z,{value:"Contravariance",label:"Contravariance",children:[(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Contravariance"})," only allows argument types that are less derived than that defined by the generic parameters. You can declare a generic type parameter contravariant by using the ",(0,r.jsx)(n.code,{children:"in"})," keyword."]}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"// contravariant generic event handler interface\ninterface IEventHandler<in TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<IUpdatedEvent>>();\n"})}),(0,r.jsxs)(n.p,{children:["Despite the fact that only ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," implementations were requested, the result contains both ",(0,r.jsx)(n.code,{children:"GeneralEventHandler"})," and ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"}),". As ",(0,r.jsx)(n.code,{children:"TEvent"})," is declared ",(0,r.jsx)(n.strong,{children:"contravariant"})," with the ",(0,r.jsx)(n.code,{children:"in"})," keyword, and ",(0,r.jsx)(n.code,{children:"IGeneralEvent"})," is less derived than ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"}),", ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," implementations can be part of ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," collections."]}),(0,r.jsxs)(n.p,{children:["If we request ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"}),", only ",(0,r.jsx)(n.code,{children:"GeneralEventHandler"})," would be returned, because ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"})," is more derived, so ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," implementations are not fit into ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," collections."]})]}),(0,r.jsxs)(l.Z,{value:"Covariance",label:"Covariance",children:[(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Covariance"})," only allows argument types that are more derived than that defined by the generic parameters. You can declare a generic type parameter covariant by using the ",(0,r.jsx)(n.code,{children:"out"})," keyword."]}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"// covariant generic event handler interface\ninterface IEventHandler<out TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IGeneralEvent>> eventHandlers = container.ResolveAll<IEventHandler<IGeneralEvent>>();\n"})}),(0,r.jsxs)(n.p,{children:["Despite the fact that only ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," implementations were requested, the result contains both ",(0,r.jsx)(n.code,{children:"GeneralEventHandler"})," and ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"}),". As ",(0,r.jsx)(n.code,{children:"TEvent"})," is declared ",(0,r.jsx)(n.strong,{children:"covariant"})," with the ",(0,r.jsx)(n.code,{children:"out"})," keyword, and ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"})," is more derived than ",(0,r.jsx)(n.code,{children:"IGeneralEvent"}),", ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," implementations can be part of ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," collections."]}),(0,r.jsxs)(n.p,{children:["If we request ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"}),", only ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"})," would be returned, because ",(0,r.jsx)(n.code,{children:"IGeneralEvent"})," is less derived, so ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," implementations are not fit into ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," collections."]})]})]})]})}function p(e={}){const{wrapper:n}={...(0,a.a)(),...e.components};return n?(0,r.jsx)(n,{...e,children:(0,r.jsx)(h,{...e})}):h(e)}},5162:(e,n,t)=>{t.d(n,{Z:()=>i});t(7294);var r=t(4334);const a={tabItem:"tabItem_Ymn6"};var s=t(5893);function i(e){let{children:n,hidden:t,className:i}=e;return(0,s.jsx)("div",{role:"tabpanel",className:(0,r.Z)(a.tabItem,i),hidden:t,children:n})}},4866:(e,n,t)=>{t.d(n,{Z:()=>b});var r=t(7294),a=t(4334),s=t(2466),i=t(6550),l=t(469),o=t(1980),d=t(7392),c=t(12);function v(e){return r.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,r.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function u(e){const{values:n,children:t}=e;return(0,r.useMemo)((()=>{const e=n??function(e){return v(e).map((e=>{let{props:{value:n,label:t,attributes:r,default:a}}=e;return{value:n,label:t,attributes:r,default:a}}))}(t);return function(e){const n=(0,d.l)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,t])}function h(e){let{value:n,tabValues:t}=e;return t.some((e=>e.value===n))}function p(e){let{queryString:n=!1,groupId:t}=e;const a=(0,i.k6)(),s=function(e){let{queryString:n=!1,groupId:t}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!t)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return t??null}({queryString:n,groupId:t});return[(0,o._X)(s),(0,r.useCallback)((e=>{if(!s)return;const n=new URLSearchParams(a.location.search);n.set(s,e),a.replace({...a.location,search:n.toString()})}),[s,a])]}function g(e){const{defaultValue:n,queryString:t=!1,groupId:a}=e,s=u(e),[i,o]=(0,r.useState)((()=>function(e){let{defaultValue:n,tabValues:t}=e;if(0===t.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!h({value:n,tabValues:t}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${t.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const r=t.find((e=>e.default))??t[0];if(!r)throw new Error("Unexpected error: 0 tabValues");return r.value}({defaultValue:n,tabValues:s}))),[d,v]=p({queryString:t,groupId:a}),[g,f]=function(e){let{groupId:n}=e;const t=function(e){return e?`docusaurus.tab.${e}`:null}(n),[a,s]=(0,c.Nk)(t);return[a,(0,r.useCallback)((e=>{t&&s.set(e)}),[t,s])]}({groupId:a}),E=(()=>{const e=d??g;return h({value:e,tabValues:s})?e:null})();(0,l.Z)((()=>{E&&o(E)}),[E]);return{selectedValue:i,selectValue:(0,r.useCallback)((e=>{if(!h({value:e,tabValues:s}))throw new Error(`Can't select invalid tab value=${e}`);o(e),v(e),f(e)}),[v,f,s]),tabValues:s}}var f=t(2389);const E={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var m=t(5893);function x(e){let{className:n,block:t,selectedValue:r,selectValue:i,tabValues:l}=e;const o=[],{blockElementScrollPositionUntilNextRender:d}=(0,s.o5)(),c=e=>{const n=e.currentTarget,t=o.indexOf(n),a=l[t].value;a!==r&&(d(n),i(a))},v=e=>{let n=null;switch(e.key){case"Enter":c(e);break;case"ArrowRight":{const t=o.indexOf(e.currentTarget)+1;n=o[t]??o[0];break}case"ArrowLeft":{const t=o.indexOf(e.currentTarget)-1;n=o[t]??o[o.length-1];break}}n?.focus()};return(0,m.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,a.Z)("tabs",{"tabs--block":t},n),children:l.map((e=>{let{value:n,label:t,attributes:s}=e;return(0,m.jsx)("li",{role:"tab",tabIndex:r===n?0:-1,"aria-selected":r===n,ref:e=>o.push(e),onKeyDown:v,onClick:c,...s,className:(0,a.Z)("tabs__item",E.tabItem,s?.className,{"tabs__item--active":r===n}),children:t??n},n)}))})}function I(e){let{lazy:n,children:t,selectedValue:a}=e;const s=(Array.isArray(t)?t:[t]).filter(Boolean);if(n){const e=s.find((e=>e.props.value===a));return e?(0,r.cloneElement)(e,{className:"margin-top--md"}):null}return(0,m.jsx)("div",{className:"margin-top--md",children:s.map(((e,n)=>(0,r.cloneElement)(e,{key:n,hidden:e.props.value!==a})))})}function j(e){const n=g(e);return(0,m.jsxs)("div",{className:(0,a.Z)("tabs-container",E.tabList),children:[(0,m.jsx)(x,{...e,...n}),(0,m.jsx)(I,{...e,...n})]})}function b(e){const n=(0,f.Z)();return(0,m.jsx)(j,{...e,children:v(e.children)},String(n))}},8846:(e,n,t)=>{t.d(n,{Z:()=>i});var r=t(7294);const a={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var s=t(5893);function i(e){let{children:n}=e,t=r.Children.toArray(n).filter((e=>e));return(0,s.jsxs)("div",{className:a.codeDescContainer,children:[(0,s.jsx)("div",{className:a.desc,children:t[0]}),(0,s.jsx)("div",{className:a.example,children:t[1]})]})}},1151:(e,n,t)=>{t.d(n,{Z:()=>l,a:()=>i});var r=t(7294);const a={},s=r.createContext(a);function i(e){const n=r.useContext(s);return r.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function l(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(a):e.components||a:i(e.components),r.createElement(s.Provider,{value:n},e.children)}}}]);