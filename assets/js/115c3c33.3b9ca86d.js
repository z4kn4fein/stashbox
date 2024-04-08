"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[196],{2490:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>d,contentTitle:()=>c,default:()=>p,frontMatter:()=>a,metadata:()=>l,toc:()=>u});var i=t(5893),s=t(1151),r=t(4866),o=t(5162);const a={},c="Validation",l={id:"diagnostics/validation",title:"Validation",description:"Stashbox validation routines help you detect and solve common misconfiguration issues. You can verify the container's actual state with its .Validate() method. It walks through the whole resolution tree and collects all issues into an AggregateException.",source:"@site/docs/diagnostics/validation.md",sourceDirName:"diagnostics",slug:"/diagnostics/validation",permalink:"/stashbox/docs/diagnostics/validation",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/diagnostics/validation.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1712620140,formattedLastUpdatedAt:"Apr 8, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Special resolution cases",permalink:"/stashbox/docs/advanced/special-resolution-cases"},next:{title:"Utilities",permalink:"/stashbox/docs/diagnostics/utilities"}},d={},u=[{value:"Registration validation",id:"registration-validation",level:2},{value:"InvalidRegistrationException",id:"invalidregistrationexception",level:3},{value:"ServiceAlreadyRegisteredException",id:"servicealreadyregisteredexception",level:3},{value:"Resolution validation",id:"resolution-validation",level:2},{value:"Lifetime validation",id:"lifetime-validation",level:2},{value:"Circular dependency",id:"circular-dependency",level:2},{value:"Other exceptions",id:"other-exceptions",level:2},{value:"CompositionRootNotFoundException",id:"compositionrootnotfoundexception",level:3},{value:"ConstructorNotFoundException",id:"constructornotfoundexception",level:3}];function h(e){const n={a:"a",br:"br",code:"code",h1:"h1",h2:"h2",h3:"h3",li:"li",ol:"ol",p:"p",pre:"pre",strong:"strong",ul:"ul",...(0,s.a)(),...e.components};return(0,i.jsxs)(i.Fragment,{children:[(0,i.jsx)(n.h1,{id:"validation",children:"Validation"}),"\n",(0,i.jsxs)(n.p,{children:["Stashbox validation routines help you detect and solve common misconfiguration issues. You can verify the container's actual state with its ",(0,i.jsx)(n.code,{children:".Validate()"})," method. It walks through the whole ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree"})," and collects all issues into an ",(0,i.jsx)(n.code,{children:"AggregateException"}),"."]}),"\n",(0,i.jsx)(n.h2,{id:"registration-validation",children:"Registration validation"}),"\n",(0,i.jsx)(n.p,{children:"During registration, the container validates the passed types and throws the following exceptions when the validation fails."}),"\n",(0,i.jsx)(n.h3,{id:"invalidregistrationexception",children:"InvalidRegistrationException"}),"\n",(0,i.jsxs)(n.ol,{children:["\n",(0,i.jsxs)(n.li,{children:[(0,i.jsxs)(n.strong,{children:["When the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"implementation type"})," is not resolvable."]})," (it's an interface or an abstract class registered like: ",(0,i.jsx)(n.code,{children:"Register<IService>()"}),"):"]}),"\n"]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"The type Namespace.IService could not be resolved. It's probably an interface, abstract class, or primitive type.\n"})}),"\n",(0,i.jsxs)(n.ol,{start:"2",children:["\n",(0,i.jsxs)(n.li,{children:[(0,i.jsxs)(n.strong,{children:["When the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"implementation type"})," does not implement the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"})]}),"."]}),"\n"]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"The type Namespace.MotorCycle does not implement the '[service type](/docs/getting-started/glossary#service-type--implementation-type)' Namespace.ICar.\n"})}),"\n",(0,i.jsx)(n.h3,{id:"servicealreadyregisteredexception",children:"ServiceAlreadyRegisteredException"}),"\n",(0,i.jsxs)(n.p,{children:[(0,i.jsxs)(n.strong,{children:["When the given ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"implementation type"})," is already registered"]})," and the ",(0,i.jsx)(n.code,{children:"RegistrationBehavior"})," ",(0,i.jsx)(n.a,{href:"/docs/configuration/container-configuration#registration-behavior",children:"container configuration option"})," is set to ",(0,i.jsx)(n.code,{children:"ThrowException"}),":"]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"The type Namespace.Service is already registered.\n"})}),"\n",(0,i.jsx)(n.h2,{id:"resolution-validation",children:"Resolution validation"}),"\n",(0,i.jsxs)(n.p,{children:["During the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree's"})," construction, the container continuously checks its actual state to ensure stability. When any of the following issues occur, the container throws a ",(0,i.jsx)(n.code,{children:"ResolutionFailedException"}),"."]}),"\n",(0,i.jsxs)(n.ol,{children:["\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsxs)(n.strong,{children:["When a dependency is missing from the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree"})]}),"."]}),"\n",(0,i.jsxs)(r.Z,{children:[(0,i.jsxs)(o.Z,{value:"Parameter",label:"Parameter",children:[(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class Service\n{\n    public Service(Dependency dep) { }\n\n    public Service(Dependency2 dep2) { }\n}\n\ncontainer.Register<Service>();\nvar service = container.Resolve<Service>();\n"})}),(0,i.jsx)(n.p,{children:"This will result in the following exception message:"}),(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"Could not resolve type Namespace.Service.\nConstructor Void .ctor(Dependency) found with unresolvable parameter: (Namespace.Dependency)dep.\nConstructor Void .ctor(Dependency2) found with unresolvable parameter: (Namespace.Dependency2)dep2.\n"})})]}),(0,i.jsxs)(o.Z,{value:"Property / field",label:"Property / field",children:[(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class Service\n{\n    public Dependency Dep { get; set; }\n}\n\ncontainer.Register<Service>(options => options.WithDependencyBinding(s => s.Dep));\nvar service = container.Resolve<Service>();\n"})}),(0,i.jsx)(n.p,{children:"This will show the following message:"}),(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"Could not resolve type Namespace.Service.\nUnresolvable property: (Namespace.Dependency)Dep.\n"})})]})]}),"\n"]}),"\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"When the requested type is unresolvable."})," E.g., it doesn't have a public constructor."]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"Could not resolve type Namespace.Service.\nService is not registered or unresolvable type requested.\n"})}),"\n"]}),"\n"]}),"\n",(0,i.jsx)(n.h2,{id:"lifetime-validation",children:"Lifetime validation"}),"\n",(0,i.jsxs)(n.p,{children:["This validation enforces the following rules. When they are violated, the container throws a ",(0,i.jsx)(n.code,{children:"LifetimeValidationFailedException"}),"."]}),"\n",(0,i.jsxs)(n.ol,{children:["\n",(0,i.jsxs)(n.li,{children:[(0,i.jsxs)(n.strong,{children:["When a scoped service is requested from the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#root-scope",children:"root scope"})]}),".",(0,i.jsx)(n.br,{}),"\n","As the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#root-scope",children:"root scope's"})," lifetime is bound to the container's lifetime, this action unintentionally promotes the scoped service's lifetime to singleton:"]}),"\n"]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"Resolution of Namespace.Service (ScopedLifetime) from the '[root scope](/docs/getting-started/glossary#root-scope)' is not allowed, \nthat would promote the service's lifetime to a singleton.\n"})}),"\n",(0,i.jsxs)(n.ol,{start:"2",children:["\n",(0,i.jsxs)(n.li,{children:["\n",(0,i.jsxs)(n.p,{children:[(0,i.jsx)(n.strong,{children:"When the life-span of a dependency is shorter than its parent's"}),".",(0,i.jsx)(n.br,{}),"\n","It's called ",(0,i.jsx)(n.a,{href:"https://blog.ploeh.dk/2014/06/02/captive-dependency/",children:"captive dependency"}),". Every lifetime has a ",(0,i.jsx)(n.code,{children:"LifeSpan"})," value, which determines how long the related service lives. The main rule is that services may not contain dependencies with shorter life spans. E.g., singletons should not depend on scoped services. The only exception is the life span value ",(0,i.jsx)(n.code,{children:"0"}),", which indicates that the related service is state-less and could be injected into any service."]}),"\n",(0,i.jsxs)(n.p,{children:["These are the ",(0,i.jsx)(n.code,{children:"LifeSpan"})," values of the pre-defined lifetimes:"]}),"\n",(0,i.jsxs)(n.ul,{children:["\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.strong,{children:"Singleton"}),": 20"]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.strong,{children:"Scoped"}),": 10"]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.strong,{children:"NamedScope"}),": 10"]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.strong,{children:"PerRequest"}),": 0"]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.strong,{children:"PerScopedRequest"}),": 0"]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.strong,{children:"Transient"}),": 0"]}),"\n"]}),"\n"]}),"\n"]}),"\n",(0,i.jsx)(n.p,{children:"In case of a failed validation the exception message would be:"}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"The life-span of Namespace.Service (ScopedLifetime|10) is shorter than \nits direct or indirect parent's Namespace.Dependency (Singleton|20). \nThis could lead to incidental lifetime promotions with longer life-span, \nit's recommended to double-check your lifetime configurations.\n"})}),"\n",(0,i.jsx)(n.h2,{id:"circular-dependency",children:"Circular dependency"}),"\n",(0,i.jsxs)(n.p,{children:["When the container encounters a circular dependency loop in the ",(0,i.jsx)(n.a,{href:"/docs/getting-started/glossary#resolution-tree",children:"resolution tree"}),", it throws a ",(0,i.jsx)(n.code,{children:"CircularDependencyException"}),"."]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class Service1\n{\n    public Service1(Service2 service2) { }\n}\n\nclass Service2\n{\n    public Service2(Service1 service1) { }\n}\n\ncontainer.Register<Service1>();\ncontainer.Register<Service2>();\nvar service = container.Resolve<Service1>();\n"})}),"\n",(0,i.jsx)(n.p,{children:"The exception message is:"}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"Circular dependency detected during the resolution of Namespace.Service1.\n"})}),"\n",(0,i.jsx)(n.h2,{id:"other-exceptions",children:"Other exceptions"}),"\n",(0,i.jsx)(n.h3,{id:"compositionrootnotfoundexception",children:"CompositionRootNotFoundException"}),"\n",(0,i.jsxs)(n.p,{children:["This exception pops up when we try to compose an assembly, but it doesn't contain an ",(0,i.jsx)(n.code,{children:"ICompositionRoot"})," implementation."]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"container.ComposeAssembly(typeof(Service).Assembly);\n"})}),"\n",(0,i.jsx)(n.p,{children:"The exception message is:"}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"No ICompositionRoot found in the given assembly: {your-assembly-name}\n"})}),"\n",(0,i.jsx)(n.h3,{id:"constructornotfoundexception",children:"ConstructorNotFoundException"}),"\n",(0,i.jsxs)(n.p,{children:["During the registration phase, when you are using the ",(0,i.jsx)(n.a,{href:"/docs/configuration/registration-configuration#withconstructorbyargumenttypes",children:(0,i.jsx)(n.code,{children:"WithConstructorByArgumentTypes()"})})," or ",(0,i.jsx)(n.a,{href:"/docs/configuration/registration-configuration#withconstructorbyarguments",children:(0,i.jsx)(n.code,{children:"WithConstructorByArguments()"})})," options, you can accidentally point to a non-existing constructor. In that case, the container throws a ",(0,i.jsx)(n.code,{children:"ConstructorNotFoundException"}),"."]}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{className:"language-cs",children:"class Service\n{\n    public Service(Dependency dep) { }\n}\n\ncontainer.Register<Service>(options => options.WithConstructorByArgumentTypes(typeof(string), typeof(int)));\n"})}),"\n",(0,i.jsx)(n.p,{children:"The exception message is:"}),"\n",(0,i.jsx)(n.pre,{children:(0,i.jsx)(n.code,{children:"Constructor not found for Namespace.Service with the given argument types: System.String, System.Int32.\n"})})]})}function p(e={}){const{wrapper:n}={...(0,s.a)(),...e.components};return n?(0,i.jsx)(n,{...e,children:(0,i.jsx)(h,{...e})}):h(e)}},5162:(e,n,t)=>{t.d(n,{Z:()=>o});t(7294);var i=t(4334);const s={tabItem:"tabItem_Ymn6"};var r=t(5893);function o(e){let{children:n,hidden:t,className:o}=e;return(0,r.jsx)("div",{role:"tabpanel",className:(0,i.Z)(s.tabItem,o),hidden:t,children:n})}},4866:(e,n,t)=>{t.d(n,{Z:()=>w});var i=t(7294),s=t(4334),r=t(2466),o=t(6550),a=t(469),c=t(1980),l=t(7392),d=t(12);function u(e){return i.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,i.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function h(e){const{values:n,children:t}=e;return(0,i.useMemo)((()=>{const e=n??function(e){return u(e).map((e=>{let{props:{value:n,label:t,attributes:i,default:s}}=e;return{value:n,label:t,attributes:i,default:s}}))}(t);return function(e){const n=(0,l.l)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,t])}function p(e){let{value:n,tabValues:t}=e;return t.some((e=>e.value===n))}function x(e){let{queryString:n=!1,groupId:t}=e;const s=(0,o.k6)(),r=function(e){let{queryString:n=!1,groupId:t}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!t)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return t??null}({queryString:n,groupId:t});return[(0,c._X)(r),(0,i.useCallback)((e=>{if(!r)return;const n=new URLSearchParams(s.location.search);n.set(r,e),s.replace({...s.location,search:n.toString()})}),[r,s])]}function g(e){const{defaultValue:n,queryString:t=!1,groupId:s}=e,r=h(e),[o,c]=(0,i.useState)((()=>function(e){let{defaultValue:n,tabValues:t}=e;if(0===t.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!p({value:n,tabValues:t}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${t.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const i=t.find((e=>e.default))??t[0];if(!i)throw new Error("Unexpected error: 0 tabValues");return i.value}({defaultValue:n,tabValues:r}))),[l,u]=x({queryString:t,groupId:s}),[g,v]=function(e){let{groupId:n}=e;const t=function(e){return e?`docusaurus.tab.${e}`:null}(n),[s,r]=(0,d.Nk)(t);return[s,(0,i.useCallback)((e=>{t&&r.set(e)}),[t,r])]}({groupId:s}),m=(()=>{const e=l??g;return p({value:e,tabValues:r})?e:null})();(0,a.Z)((()=>{m&&c(m)}),[m]);return{selectedValue:o,selectValue:(0,i.useCallback)((e=>{if(!p({value:e,tabValues:r}))throw new Error(`Can't select invalid tab value=${e}`);c(e),u(e),v(e)}),[u,v,r]),tabValues:r}}var v=t(2389);const m={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var f=t(5893);function j(e){let{className:n,block:t,selectedValue:i,selectValue:o,tabValues:a}=e;const c=[],{blockElementScrollPositionUntilNextRender:l}=(0,r.o5)(),d=e=>{const n=e.currentTarget,t=c.indexOf(n),s=a[t].value;s!==i&&(l(n),o(s))},u=e=>{let n=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const t=c.indexOf(e.currentTarget)+1;n=c[t]??c[0];break}case"ArrowLeft":{const t=c.indexOf(e.currentTarget)-1;n=c[t]??c[c.length-1];break}}n?.focus()};return(0,f.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,s.Z)("tabs",{"tabs--block":t},n),children:a.map((e=>{let{value:n,label:t,attributes:r}=e;return(0,f.jsx)("li",{role:"tab",tabIndex:i===n?0:-1,"aria-selected":i===n,ref:e=>c.push(e),onKeyDown:u,onClick:d,...r,className:(0,s.Z)("tabs__item",m.tabItem,r?.className,{"tabs__item--active":i===n}),children:t??n},n)}))})}function y(e){let{lazy:n,children:t,selectedValue:s}=e;const r=(Array.isArray(t)?t:[t]).filter(Boolean);if(n){const e=r.find((e=>e.props.value===s));return e?(0,i.cloneElement)(e,{className:"margin-top--md"}):null}return(0,f.jsx)("div",{className:"margin-top--md",children:r.map(((e,n)=>(0,i.cloneElement)(e,{key:n,hidden:e.props.value!==s})))})}function b(e){const n=g(e);return(0,f.jsxs)("div",{className:(0,s.Z)("tabs-container",m.tabList),children:[(0,f.jsx)(j,{...e,...n}),(0,f.jsx)(y,{...e,...n})]})}function w(e){const n=(0,v.Z)();return(0,f.jsx)(b,{...e,children:u(e.children)},String(n))}},1151:(e,n,t)=>{t.d(n,{Z:()=>a,a:()=>o});var i=t(7294);const s={},r=i.createContext(s);function o(e){const n=i.useContext(r);return i.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function a(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(s):e.components||s:o(e.components),i.createElement(r.Provider,{value:n},e.children)}}}]);