"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[747],{5151:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>v,contentTitle:()=>d,default:()=>p,frontMatter:()=>o,metadata:()=>c,toc:()=>u});var r=t(4848),a=t(8453),i=t(7470),s=t(1470),l=t(9365);const o={},d="Generics",c={id:"advanced/generics",title:"Generics",description:"This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, generic decorators, conditions based on generic constraints, and variance.",source:"@site/docs/advanced/generics.md",sourceDirName:"advanced",slug:"/advanced/generics",permalink:"/stashbox/docs/advanced/generics",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/generics.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1734704927,formattedLastUpdatedAt:"Dec 20, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Container configuration",permalink:"/stashbox/docs/configuration/container-configuration"},next:{title:"Decorators",permalink:"/stashbox/docs/advanced/decorators"}},v={},u=[{value:"Closed-generics",id:"closed-generics",level:2},{value:"Open-generics",id:"open-generics",level:2},{value:"Generic constraints",id:"generic-constraints",level:2},{value:"Variance",id:"variance",level:2}];function h(e){const n={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",p:"p",pre:"pre",strong:"strong",...(0,a.R)(),...e.components};return(0,r.jsxs)(r.Fragment,{children:[(0,r.jsx)(n.h1,{id:"generics",children:"Generics"}),"\n",(0,r.jsxs)(n.p,{children:["This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, ",(0,r.jsx)(n.a,{href:"/docs/advanced/decorators#generic-decorators",children:"generic decorators"}),", conditions based on generic constraints, and variance."]}),"\n",(0,r.jsx)(n.h2,{id:"closed-generics",children:"Closed-generics"}),"\n",(0,r.jsxs)(i.A,{children:[(0,r.jsxs)("div",{children:[(0,r.jsx)(n.p,{children:"The registration of a closed-generic type does not differ from registering a simple non-generic service."}),(0,r.jsxs)(n.p,{children:["You have all options available that you saw at the ",(0,r.jsx)(n.a,{href:"/docs/guides/basics",children:"basic"})," and ",(0,r.jsx)(n.a,{href:"/docs/guides/advanced-registration",children:"advanced registration"})," flows."]})]}),(0,r.jsx)("div",{children:(0,r.jsxs)(s.A,{groupId:"generic-runtime-apis",children:[(0,r.jsx)(l.A,{value:"Generic API",label:"Generic API",children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register<IValidator<User>, UserValidator>();\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n"})})}),(0,r.jsx)(l.A,{value:"Runtime type API",label:"Runtime type API",children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register(typeof(IValidator<User>), typeof(UserValidator));\nobject validator = container.Resolve(typeof(IValidator<User>));\n"})})})]})})]}),"\n",(0,r.jsx)(n.h2,{id:"open-generics",children:"Open-generics"}),"\n",(0,r.jsxs)(n.p,{children:["The registration of an open-generic type differs from registering a closed-generic one as C# doesn't allow the usage of open-generic types in generic method parameters. We have to get a runtime type from the open-generic type first with ",(0,r.jsx)(n.code,{children:"typeof()"}),"."]}),"\n",(0,r.jsxs)(i.A,{children:[(0,r.jsx)("div",{children:(0,r.jsx)(n.p,{children:"Open-generic types could help in such scenarios where you have generic interface-implementation pairs with numerous generic parameter variations. The registration of those different versions would look like this:"})}),(0,r.jsx)("div",{children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register<IValidator<User>, Validator<User>>();\ncontainer.Register<IValidator<Role>, Validator<Role>>();\ncontainer.Register<IValidator<Company>, Validator<Company>>();\n// and so on...\n"})})})]}),"\n",(0,r.jsxs)(i.A,{children:[(0,r.jsx)("div",{children:(0,r.jsxs)(n.p,{children:["Rather than doing that, you can register your type's generic definition and let Stashbox bind the type parameters for you. When a matching closed ",(0,r.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})," is requested, the container will construct an equivalent closed-generic implementation."]})}),(0,r.jsx)("div",{children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register(typeof(IValidator<>), typeof(Validator<>));\n// Validator<User> will be returned.\nIValidator<User> userValidator = container.Resolve<IValidator<User>>();\n// Validator<Role> will be returned.\nIValidator<Role> roleValidator = container.Resolve<IValidator<Role>>();\n"})})})]}),"\n",(0,r.jsxs)(i.A,{children:[(0,r.jsx)("div",{children:(0,r.jsx)(n.p,{children:"A registered closed-generic type always has priority over an open-generic type at service selection."})}),(0,r.jsx)("div",{children:(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"container.Register<IValidator<User>, UserValidator>();\ncontainer.Register(typeof(IValidator<>), typeof(Validator<>));\n// UserValidator will be returned.\nIValidator<User> validator = container.Resolve<IValidator<User>>();\n"})})})]}),"\n",(0,r.jsx)(n.h2,{id:"generic-constraints",children:"Generic constraints"}),"\n",(0,r.jsxs)(n.p,{children:["In the following examples, you can see how the container handles generic constraints during service resolution. Constraints can be used for ",(0,r.jsx)(n.a,{href:"/docs/guides/service-resolution#conditional-resolution",children:"conditional resolution"})," including collection filters."]}),"\n",(0,r.jsxs)(s.A,{children:[(0,r.jsxs)(l.A,{value:"Conditional resolution",label:"Conditional resolution",children:[(0,r.jsxs)(n.p,{children:["The container chooses ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"})," because it is the only one that has a constraint satisfied by the requested ",(0,r.jsx)(n.code,{children:"UserUpdatedEvent"})," generic parameter as it's implementing ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"}),"."]}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandler will be UpdatedEventHandler<ConstraintArgument>\nIEventHandler<UserUpdatedEvent> eventHandler = container.Resolve<IEventHandler<UserUpdatedEvent>>();\n"})})]}),(0,r.jsxs)(l.A,{value:"Collection filter",label:"Collection filter",children:[(0,r.jsx)(n.p,{children:"This example shows how the container is filtering out those services from the returned collection that does not satisfy the given generic constraint needed to create the closed generic type."}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"interface IEventHandler<TEvent> { }\n\n// event interfaces\ninterface IUpdatedEvent { }\ninterface ICreatedEvent { }\n\n// event handlers\nclass UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }\nclass CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }\n\n// event implementation\nclass UserUpdatedEvent : IUpdatedEvent { }\n\nusing var container = new StashboxContainer();\n\ncontainer.RegisterTypesAs(typeof(IEventHandler<>), new[] \n    { \n        typeof(UpdateEventHandler<>), \n        typeof(CreateEventHandler<>) \n    });\n\n// eventHandlers will contain only UpdatedEventHandler<ConstraintArgument>\nIEnumerable<IEventHandler<UserUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<UserUpdatedEvent>>();\n"})})]})]}),"\n",(0,r.jsx)(n.h2,{id:"variance",children:"Variance"}),"\n",(0,r.jsxs)(n.p,{children:["Since .NET Framework 4.0, C# supports ",(0,r.jsx)(n.a,{href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/",children:"covariance and contravariance"})," in generic interfaces and delegates and allows implicit conversion of generic type parameters. In this section, we'll focus on variance in generic interfaces."]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.a,{href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces",children:"Here"})," you can read more about how to create variant generic interfaces, and the following example will show how you can use them with Stashbox."]}),"\n",(0,r.jsxs)(s.A,{children:[(0,r.jsxs)(l.A,{value:"Contravariance",label:"Contravariance",children:[(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Contravariance"})," only allows argument types that are less derived than that defined by the generic parameters. You can declare a generic type parameter contravariant by using the ",(0,r.jsx)(n.code,{children:"in"})," keyword."]}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"// contravariant generic event handler interface\ninterface IEventHandler<in TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<IUpdatedEvent>>();\n"})}),(0,r.jsxs)(n.p,{children:["Despite the fact that only ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," implementations were requested, the result contains both ",(0,r.jsx)(n.code,{children:"GeneralEventHandler"})," and ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"}),". As ",(0,r.jsx)(n.code,{children:"TEvent"})," is declared ",(0,r.jsx)(n.strong,{children:"contravariant"})," with the ",(0,r.jsx)(n.code,{children:"in"})," keyword, and ",(0,r.jsx)(n.code,{children:"IGeneralEvent"})," is less derived than ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"}),", ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," implementations can be part of ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," collections."]}),(0,r.jsxs)(n.p,{children:["If we request ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"}),", only ",(0,r.jsx)(n.code,{children:"GeneralEventHandler"})," would be returned, because ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"})," is more derived, so ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," implementations are not fit into ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," collections."]})]}),(0,r.jsxs)(l.A,{value:"Covariance",label:"Covariance",children:[(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Covariance"})," only allows argument types that are more derived than that defined by the generic parameters. You can declare a generic type parameter covariant by using the ",(0,r.jsx)(n.code,{children:"out"})," keyword."]}),(0,r.jsx)(n.pre,{children:(0,r.jsx)(n.code,{className:"language-cs",children:"// covariant generic event handler interface\ninterface IEventHandler<out TEvent> { } \n\n// event interfaces\ninterface IGeneralEvent { }\ninterface IUpdatedEvent : IGeneralEvent { }\n\n// event handlers\nclass GeneralEventHandler : IEventHandler<IGeneralEvent> { }\nclass UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }\n\ncontainer.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();\ncontainer.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();\n\n// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler\nIEnumerable<IEventHandler<IGeneralEvent>> eventHandlers = container.ResolveAll<IEventHandler<IGeneralEvent>>();\n"})}),(0,r.jsxs)(n.p,{children:["Despite the fact that only ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," implementations were requested, the result contains both ",(0,r.jsx)(n.code,{children:"GeneralEventHandler"})," and ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"}),". As ",(0,r.jsx)(n.code,{children:"TEvent"})," is declared ",(0,r.jsx)(n.strong,{children:"covariant"})," with the ",(0,r.jsx)(n.code,{children:"out"})," keyword, and ",(0,r.jsx)(n.code,{children:"IUpdatedEvent"})," is more derived than ",(0,r.jsx)(n.code,{children:"IGeneralEvent"}),", ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," implementations can be part of ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," collections."]}),(0,r.jsxs)(n.p,{children:["If we request ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"}),", only ",(0,r.jsx)(n.code,{children:"UpdatedEventHandler"})," would be returned, because ",(0,r.jsx)(n.code,{children:"IGeneralEvent"})," is less derived, so ",(0,r.jsx)(n.code,{children:"IEventHandler<IGeneralEvent>"})," implementations are not fit into ",(0,r.jsx)(n.code,{children:"IEventHandler<IUpdatedEvent>"})," collections."]})]})]}),"\n",(0,r.jsx)(n.admonition,{type:"info",children:(0,r.jsxs)(n.p,{children:["The check for variant generic types is enabled by default, but it can be turned off via a ",(0,r.jsx)(n.a,{href:"/docs/configuration/container-configuration#generic-variance",children:"container configuration option"}),"."]})})]})}function p(e={}){const{wrapper:n}={...(0,a.R)(),...e.components};return n?(0,r.jsx)(n,{...e,children:(0,r.jsx)(h,{...e})}):h(e)}},9365:(e,n,t)=>{t.d(n,{A:()=>s});t(6540);var r=t(870);const a={tabItem:"tabItem_Ymn6"};var i=t(4848);function s(e){let{children:n,hidden:t,className:s}=e;return(0,i.jsx)("div",{role:"tabpanel",className:(0,r.A)(a.tabItem,s),hidden:t,children:n})}},1470:(e,n,t)=>{t.d(n,{A:()=>b});var r=t(6540),a=t(870),i=t(3104),s=t(6347),l=t(205),o=t(7485),d=t(1682),c=t(9466);function v(e){return r.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,r.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function u(e){const{values:n,children:t}=e;return(0,r.useMemo)((()=>{const e=n??function(e){return v(e).map((e=>{let{props:{value:n,label:t,attributes:r,default:a}}=e;return{value:n,label:t,attributes:r,default:a}}))}(t);return function(e){const n=(0,d.X)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,t])}function h(e){let{value:n,tabValues:t}=e;return t.some((e=>e.value===n))}function p(e){let{queryString:n=!1,groupId:t}=e;const a=(0,s.W6)(),i=function(e){let{queryString:n=!1,groupId:t}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!t)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return t??null}({queryString:n,groupId:t});return[(0,o.aZ)(i),(0,r.useCallback)((e=>{if(!i)return;const n=new URLSearchParams(a.location.search);n.set(i,e),a.replace({...a.location,search:n.toString()})}),[i,a])]}function g(e){const{defaultValue:n,queryString:t=!1,groupId:a}=e,i=u(e),[s,o]=(0,r.useState)((()=>function(e){let{defaultValue:n,tabValues:t}=e;if(0===t.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!h({value:n,tabValues:t}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${t.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const r=t.find((e=>e.default))??t[0];if(!r)throw new Error("Unexpected error: 0 tabValues");return r.value}({defaultValue:n,tabValues:i}))),[d,v]=p({queryString:t,groupId:a}),[g,f]=function(e){let{groupId:n}=e;const t=function(e){return e?`docusaurus.tab.${e}`:null}(n),[a,i]=(0,c.Dv)(t);return[a,(0,r.useCallback)((e=>{t&&i.set(e)}),[t,i])]}({groupId:a}),E=(()=>{const e=d??g;return h({value:e,tabValues:i})?e:null})();(0,l.A)((()=>{E&&o(E)}),[E]);return{selectedValue:s,selectValue:(0,r.useCallback)((e=>{if(!h({value:e,tabValues:i}))throw new Error(`Can't select invalid tab value=${e}`);o(e),v(e),f(e)}),[v,f,i]),tabValues:i}}var f=t(2303);const E={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var m=t(4848);function x(e){let{className:n,block:t,selectedValue:r,selectValue:s,tabValues:l}=e;const o=[],{blockElementScrollPositionUntilNextRender:d}=(0,i.a_)(),c=e=>{const n=e.currentTarget,t=o.indexOf(n),a=l[t].value;a!==r&&(d(n),s(a))},v=e=>{let n=null;switch(e.key){case"Enter":c(e);break;case"ArrowRight":{const t=o.indexOf(e.currentTarget)+1;n=o[t]??o[0];break}case"ArrowLeft":{const t=o.indexOf(e.currentTarget)-1;n=o[t]??o[o.length-1];break}}n?.focus()};return(0,m.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,a.A)("tabs",{"tabs--block":t},n),children:l.map((e=>{let{value:n,label:t,attributes:i}=e;return(0,m.jsx)("li",{role:"tab",tabIndex:r===n?0:-1,"aria-selected":r===n,ref:e=>o.push(e),onKeyDown:v,onClick:c,...i,className:(0,a.A)("tabs__item",E.tabItem,i?.className,{"tabs__item--active":r===n}),children:t??n},n)}))})}function I(e){let{lazy:n,children:t,selectedValue:a}=e;const i=(Array.isArray(t)?t:[t]).filter(Boolean);if(n){const e=i.find((e=>e.props.value===a));return e?(0,r.cloneElement)(e,{className:"margin-top--md"}):null}return(0,m.jsx)("div",{className:"margin-top--md",children:i.map(((e,n)=>(0,r.cloneElement)(e,{key:n,hidden:e.props.value!==a})))})}function j(e){const n=g(e);return(0,m.jsxs)("div",{className:(0,a.A)("tabs-container",E.tabList),children:[(0,m.jsx)(x,{...e,...n}),(0,m.jsx)(I,{...e,...n})]})}function b(e){const n=(0,f.A)();return(0,m.jsx)(j,{...e,children:v(e.children)},String(n))}},7470:(e,n,t)=>{t.d(n,{A:()=>s});var r=t(6540);const a={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var i=t(4848);function s(e){let{children:n}=e,t=r.Children.toArray(n).filter((e=>e));return(0,i.jsxs)("div",{className:a.codeDescContainer,children:[(0,i.jsx)("div",{className:a.desc,children:t[0]}),(0,i.jsx)("div",{className:a.example,children:t[1]})]})}},8453:(e,n,t)=>{t.d(n,{R:()=>s,x:()=>l});var r=t(6540);const a={},i=r.createContext(a);function s(e){const n=r.useContext(i);return r.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function l(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(a):e.components||a:s(e.components),r.createElement(i.Provider,{value:n},e.children)}}}]);