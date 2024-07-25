"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[652],{7421:(e,n,i)=>{i.r(n),i.d(n,{assets:()=>a,contentTitle:()=>d,default:()=>p,frontMatter:()=>c,metadata:()=>o,toc:()=>l});var s=i(5893),t=i(1151),r=i(8846);const c={},d="Glossary",o={id:"getting-started/glossary",title:"Glossary",description:"The following terms and definitions are used in this documentation.",source:"@site/docs/getting-started/glossary.md",sourceDirName:"getting-started",slug:"/getting-started/glossary",permalink:"/stashbox/docs/getting-started/glossary",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/getting-started/glossary.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1721951279,formattedLastUpdatedAt:"Jul 25, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Introduction",permalink:"/stashbox/docs/getting-started/introduction"},next:{title:"Basic usage",permalink:"/stashbox/docs/guides/basics"}},a={},l=[{value:"Service type | Implementation type",id:"service-type--implementation-type",level:2},{value:"Service registration | Registered service",id:"service-registration--registered-service",level:2},{value:"Injectable dependency",id:"injectable-dependency",level:2},{value:"Resolution tree",id:"resolution-tree",level:2},{value:"Dependency resolver",id:"dependency-resolver",level:2},{value:"Root scope",id:"root-scope",level:2},{value:"Named resolution",id:"named-resolution",level:2},{value:"Self registration",id:"self-registration",level:2}];function h(e){const n={a:"a",admonition:"admonition",code:"code",em:"em",h1:"h1",h2:"h2",p:"p",pre:"pre",...(0,t.a)(),...e.components};return(0,s.jsxs)(s.Fragment,{children:[(0,s.jsx)(n.h1,{id:"glossary",children:"Glossary"}),"\n",(0,s.jsx)(n.p,{children:"The following terms and definitions are used in this documentation."}),"\n",(0,s.jsx)(n.h2,{id:"service-type--implementation-type",children:"Service type | Implementation type"}),"\n",(0,s.jsxs)(n.p,{children:["The ",(0,s.jsx)(n.em,{children:"Service type"})," is usually an interface or an abstract class type used for service resolution or dependency injection. The ",(0,s.jsx)(n.em,{children:"Implementation type"})," is the actual type registered to the ",(0,s.jsx)(n.em,{children:"Service type"}),". A registration maps the ",(0,s.jsx)(n.em,{children:"Service type"})," to an ",(0,s.jsx)(n.em,{children:"Implementation type"}),". The ",(0,s.jsx)(n.em,{children:"Implementation type"})," must implement or extend the ",(0,s.jsx)(n.em,{children:"Service type"}),"."]}),"\n",(0,s.jsxs)(r.Z,{children:[(0,s.jsx)("div",{children:(0,s.jsxs)(n.p,{children:["Example where a ",(0,s.jsx)(n.em,{children:"Service type"})," is mapped to an ",(0,s.jsx)(n.em,{children:"Implementation type"}),":"]})}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"container.Register<IService, Implementation>();\n"})})})]}),"\n",(0,s.jsxs)(r.Z,{children:[(0,s.jsx)("div",{children:(0,s.jsxs)(n.p,{children:["The ",(0,s.jsx)(n.em,{children:"Service type"})," used for requesting a service from the container:"]})}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"container.Resolve<IService>(); // returns Implementation\n"})})})]}),"\n",(0,s.jsx)(n.h2,{id:"service-registration--registered-service",children:"Service registration | Registered service"}),"\n",(0,s.jsx)(n.p,{children:"It's an entity created by Stashbox when a service is registered. The service registration stores required information about how to instantiate the service, e.g., reflected type information, name, lifetime, conditions, and more."}),"\n",(0,s.jsxs)(r.Z,{children:[(0,s.jsx)("div",{children:(0,s.jsxs)(n.p,{children:["In this example, we are registering a named service. The container will create a service registration entity to store the type mapping and the name. During resolution, the container will find the registration by checking for the ",(0,s.jsx)(n.em,{children:"Service type"})," and the ",(0,s.jsx)(n.em,{children:"name"}),"."]})}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:'// the registration entity will look like: \n// IService => Implementation, name: Example\ncontainer.Register<IService, Implementation>("Example");\nvar service = container.Resolve<IService>("Example");\n'})})})]}),"\n",(0,s.jsx)(n.h2,{id:"injectable-dependency",children:"Injectable dependency"}),"\n",(0,s.jsxs)(r.Z,{children:[(0,s.jsxs)("div",{children:[(0,s.jsxs)(n.p,{children:["It's a constructor/method argument or a property/field of a registered ",(0,s.jsx)(n.em,{children:"Implementation type"})," that gets evaluated (",(0,s.jsx)(n.em,{children:"injected"}),") by Stashbox during the service's construction."]}),(0,s.jsxs)(n.p,{children:["In this example, ",(0,s.jsx)(n.code,{children:"Implementation"})," has an ",(0,s.jsx)(n.code,{children:"IDependency"})," ",(0,s.jsx)(n.em,{children:"injectable dependency"})," in its constructor."]})]}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"class Implementation : IService\n{\n    public Implementation(IDependency dependency) \n    { }\n}\n"})})})]}),"\n",(0,s.jsx)(n.h2,{id:"resolution-tree",children:"Resolution tree"}),"\n",(0,s.jsx)(n.p,{children:"It's the structural representation of a service's resolution process. It describes the instantiation order of the dependencies required to resolve the desired type."}),"\n",(0,s.jsx)(n.p,{children:"Let's see through an example:"}),"\n",(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"class A\n{\n    public A(B b, C c) { }\n}\n\nclass B\n{\n    public B(C c, D d) { }\n}\n\nclass C { }\nclass D { }\n"})}),"\n",(0,s.jsxs)(n.p,{children:["When we request the service ",(0,s.jsx)(n.code,{children:"A"}),", the container constructs the following resolution tree based on the dependencies and sub-dependencies."]}),"\n",(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{children:"        A\n      /   \\\n     B     C\n   /   \\\n  C     D\n"})}),"\n",(0,s.jsxs)(n.p,{children:["The container instantiates those services first that don't have any dependencies. ",(0,s.jsx)(n.code,{children:"C"})," and ",(0,s.jsx)(n.code,{children:"D"})," will be injected into ",(0,s.jsx)(n.code,{children:"B"}),". Then, a new ",(0,s.jsx)(n.code,{children:"C"})," is instantiated (if it's ",(0,s.jsx)(n.a,{href:"/docs/guides/lifetimes#transient-lifetime",children:"transient"}),") and injected into ",(0,s.jsx)(n.code,{children:"A"})," along with the previously created ",(0,s.jsx)(n.code,{children:"B"}),"."]}),"\n",(0,s.jsx)(n.h2,{id:"dependency-resolver",children:"Dependency resolver"}),"\n",(0,s.jsxs)(n.p,{children:["It's the container itself or the ",(0,s.jsx)(n.a,{href:"/docs/guides/scopes",children:"current scope"}),", depending on which was asked to resolve a particular service. They are both implementing Stashbox's ",(0,s.jsx)(n.code,{children:"IDependencyResolver"})," and the .NET framework's ",(0,s.jsx)(n.code,{children:"IServiceProvider"})," interface and can be used for service resolution."]}),"\n",(0,s.jsx)(n.admonition,{type:"info",children:(0,s.jsxs)(n.p,{children:["Stashbox implicitly injects the ",(0,s.jsx)(n.a,{href:"/docs/guides/scopes",children:"current scope"})," wherever ",(0,s.jsx)(n.code,{children:"IDependencyResolver"})," or ",(0,s.jsx)(n.code,{children:"IServiceProvider"})," is requested."]})}),"\n",(0,s.jsx)(n.h2,{id:"root-scope",children:"Root scope"}),"\n",(0,s.jsxs)(n.p,{children:["It's the ",(0,s.jsx)(n.a,{href:"/docs/guides/scopes",children:"main scope"})," created inside every container instance. It stores and handles the lifetime of all singletons. It's the base of each subsequent scope created by the container with the ",(0,s.jsx)(n.code,{children:".BeginScope()"})," method."]}),"\n",(0,s.jsx)(n.admonition,{type:"caution",children:(0,s.jsxs)(n.p,{children:[(0,s.jsx)(n.a,{href:"/docs/guides/lifetimes#scoped-lifetime",children:"Scoped services"})," requested from the container (and not from a ",(0,s.jsx)(n.a,{href:"/docs/guides/scopes",children:"scope"}),") are managed by the root scope. This can lead to issues because their lifetime will effectively switch to singleton. Always be sure that you don't resolve scoped services directly from the container, only from a ",(0,s.jsx)(n.a,{href:"/docs/guides/scopes",children:"scope"}),". This case is monitored by the ",(0,s.jsx)(n.a,{href:"/docs/diagnostics/validation#lifetime-validation",children:"lifetime"})," validation rule when it's ",(0,s.jsx)(n.a,{href:"/docs/configuration/container-configuration#lifetime-validation",children:"enabled"}),"."]})}),"\n",(0,s.jsx)(n.h2,{id:"named-resolution",children:"Named resolution"}),"\n",(0,s.jsxs)(r.Z,{children:[(0,s.jsx)("div",{children:(0,s.jsxs)(n.p,{children:["It's a resolution request for a named service. The same applies, when the container sees a dependency in the resolution tree with a name (set by ",(0,s.jsx)(n.a,{href:"/docs/guides/service-resolution#attributes",children:"attributes"})," or ",(0,s.jsx)(n.a,{href:"/docs/guides/service-resolution#dependency-binding",children:"bindings"}),"); it will search for a matching ",(0,s.jsx)(n.a,{href:"/docs/guides/basics#named-registration",children:"Named registration"})," to inject."]})}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:'container.Register<IService, Implementation>("Example");\n// the named request.\nvar service = container.Resolve<IService>("Example");\n'})})})]}),"\n",(0,s.jsx)(n.h2,{id:"self-registration",children:"Self registration"}),"\n",(0,s.jsxs)(r.Z,{children:[(0,s.jsx)("div",{children:(0,s.jsx)(n.p,{children:"It's a service registration that's mapped to itself. This means its service and implementation type is the same."})}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"// equivalent to container.Register<Implementation, Implementation>();\ncontainer.Register<Implementation>();\n"})})})]})]})}function p(e={}){const{wrapper:n}={...(0,t.a)(),...e.components};return n?(0,s.jsx)(n,{...e,children:(0,s.jsx)(h,{...e})}):h(e)}},8846:(e,n,i)=>{i.d(n,{Z:()=>c});var s=i(7294);const t={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var r=i(5893);function c(e){let{children:n}=e,i=s.Children.toArray(n).filter((e=>e));return(0,r.jsxs)("div",{className:t.codeDescContainer,children:[(0,r.jsx)("div",{className:t.desc,children:i[0]}),(0,r.jsx)("div",{className:t.example,children:i[1]})]})}},1151:(e,n,i)=>{i.d(n,{Z:()=>d,a:()=>c});var s=i(7294);const t={},r=s.createContext(t);function c(e){const n=s.useContext(r);return s.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function d(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(t):e.components||t:c(e.components),s.createElement(r.Provider,{value:n},e.children)}}}]);