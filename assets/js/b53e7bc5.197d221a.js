"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[343],{4192:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>u,contentTitle:()=>l,default:()=>p,frontMatter:()=>s,metadata:()=>c,toc:()=>d});var a=t(5893),r=t(1151),i=t(4866),o=t(5162);const s={},l="Special resolution cases",c={id:"advanced/special-resolution-cases",title:"Special resolution cases",description:"Unknown type resolution",source:"@site/docs/advanced/special-resolution-cases.md",sourceDirName:"advanced",slug:"/advanced/special-resolution-cases",permalink:"/stashbox/docs/advanced/special-resolution-cases",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/special-resolution-cases.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1712573937,formattedLastUpdatedAt:"Apr 8, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Child containers",permalink:"/stashbox/docs/advanced/child-containers"},next:{title:"Validation",permalink:"/stashbox/docs/diagnostics/validation"}},u={},d=[{value:"Unknown type resolution",id:"unknown-type-resolution",level:2},{value:"Default value injection",id:"default-value-injection",level:2},{value:"Optional value injection",id:"optional-value-injection",level:2}];function h(e){const n={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",p:"p",pre:"pre",...(0,r.a)(),...e.components};return(0,a.jsxs)(a.Fragment,{children:[(0,a.jsx)(n.h1,{id:"special-resolution-cases",children:"Special resolution cases"}),"\n",(0,a.jsx)(n.h2,{id:"unknown-type-resolution",children:"Unknown type resolution"}),"\n",(0,a.jsxs)(n.p,{children:["When this ",(0,a.jsx)(n.a,{href:"/docs/configuration/container-configuration#unknown-type-resolution",children:"feature"})," is enabled, the container will try to resolve unregistered types by registering them using a pre-defined configuration delegate."]}),"\n",(0,a.jsxs)(i.Z,{children:[(0,a.jsxs)(o.Z,{value:"Default",label:"Default",children:[(0,a.jsxs)(n.p,{children:["Without a registration configuration, the container can resolve only non-interface and non-abstract unknown types. In the following example,\nthe container creates an implicit registration for ",(0,a.jsx)(n.code,{children:"Dependency"})," and injects its instance into ",(0,a.jsx)(n.code,{children:"Service"}),"."]}),(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-cs",children:"class Dependency { }\n\nclass Service \n{\n    public Service(Dependency dependency)\n    { }     \n}\n\nvar container = new StashboxContainer(config => config\n    .WithUnknownTypeResolution());\n\ncontainer.Register<Service>();\n\nvar service = container.Resolve<Service>();\n"})})]}),(0,a.jsxs)(o.Z,{value:"With registration configuration",label:"With registration configuration",children:[(0,a.jsxs)(n.p,{children:["With a registration configuration, you can control how an unknown type's individual registration should behave. You can also react to a service resolution request. In the following example, we tell the container that if it finds an unregistered ",(0,a.jsx)(n.code,{children:"IDependency"})," for the first time, that should be mapped to ",(0,a.jsx)(n.code,{children:"Dependency"})," and have a singleton lifetime. Next time, when the container comes across this service, it will use the registration created at the first request."]}),(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-cs",children:"interface IDependency { }\n\nclass Dependency : IDependency { }\n\nclass Service \n{\n    public Service(IDependency dependency)\n    { }     \n}\n\nvar container = new StashboxContainer(config => config\n    .WithUnknownTypeResolution(options => \n    {\n        if(options.ServiceType == typeof(IDependency))\n        {\n            options.SetImplementationType(typeof(Dependency))\n                .WithLifetime(Lifetimes.Singleton);\n        }\n    }));\n\ncontainer.Register<Service>();\n\nvar service = container.Resolve<Service>();\n"})})]})]}),"\n",(0,a.jsx)(n.h2,{id:"default-value-injection",children:"Default value injection"}),"\n",(0,a.jsxs)(n.p,{children:["When this ",(0,a.jsx)(n.a,{href:"/docs/configuration/container-configuration#default-value-injection",children:"feature"})," is enabled, the container will resolve unknown primitive dependencies with their default value."]}),"\n",(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-cs",children:"class Person \n{\n    public Person(string name, int age) { }\n}\n\nvar container = new StashboxContainer(config => config\n    .WithDefaultValueInjection());\n// the name parameter will be null and the age will be 0.\nvar person = container.Resolve<Person>();\n"})}),"\n",(0,a.jsx)(n.admonition,{type:"note",children:(0,a.jsxs)(n.p,{children:["Unknown reference types are resolved to ",(0,a.jsx)(n.code,{children:"null"})," only in properties and fields."]})}),"\n",(0,a.jsx)(n.h2,{id:"optional-value-injection",children:"Optional value injection"}),"\n",(0,a.jsx)(n.p,{children:"Stashbox respects the optional value of each constructor and method argument."}),"\n",(0,a.jsx)(n.pre,{children:(0,a.jsx)(n.code,{className:"language-cs",children:"class Person \n{\n    public Person(string name = null, int age = 54, IContact contact = null) { }\n}\n\n// the name will be null \n// the age will be 54.\n// the contact will be null.\nvar person = container.Resolve<Person>();\n"})})]})}function p(e={}){const{wrapper:n}={...(0,r.a)(),...e.components};return n?(0,a.jsx)(n,{...e,children:(0,a.jsx)(h,{...e})}):h(e)}},5162:(e,n,t)=>{t.d(n,{Z:()=>o});t(7294);var a=t(4334);const r={tabItem:"tabItem_Ymn6"};var i=t(5893);function o(e){let{children:n,hidden:t,className:o}=e;return(0,i.jsx)("div",{role:"tabpanel",className:(0,a.Z)(r.tabItem,o),hidden:t,children:n})}},4866:(e,n,t)=>{t.d(n,{Z:()=>w});var a=t(7294),r=t(4334),i=t(2466),o=t(6550),s=t(469),l=t(1980),c=t(7392),u=t(12);function d(e){return a.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,a.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function h(e){const{values:n,children:t}=e;return(0,a.useMemo)((()=>{const e=n??function(e){return d(e).map((e=>{let{props:{value:n,label:t,attributes:a,default:r}}=e;return{value:n,label:t,attributes:a,default:r}}))}(t);return function(e){const n=(0,c.l)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,t])}function p(e){let{value:n,tabValues:t}=e;return t.some((e=>e.value===n))}function f(e){let{queryString:n=!1,groupId:t}=e;const r=(0,o.k6)(),i=function(e){let{queryString:n=!1,groupId:t}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!t)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return t??null}({queryString:n,groupId:t});return[(0,l._X)(i),(0,a.useCallback)((e=>{if(!i)return;const n=new URLSearchParams(r.location.search);n.set(i,e),r.replace({...r.location,search:n.toString()})}),[i,r])]}function v(e){const{defaultValue:n,queryString:t=!1,groupId:r}=e,i=h(e),[o,l]=(0,a.useState)((()=>function(e){let{defaultValue:n,tabValues:t}=e;if(0===t.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!p({value:n,tabValues:t}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${t.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const a=t.find((e=>e.default))??t[0];if(!a)throw new Error("Unexpected error: 0 tabValues");return a.value}({defaultValue:n,tabValues:i}))),[c,d]=f({queryString:t,groupId:r}),[v,m]=function(e){let{groupId:n}=e;const t=function(e){return e?`docusaurus.tab.${e}`:null}(n),[r,i]=(0,u.Nk)(t);return[r,(0,a.useCallback)((e=>{t&&i.set(e)}),[t,i])]}({groupId:r}),b=(()=>{const e=c??v;return p({value:e,tabValues:i})?e:null})();(0,s.Z)((()=>{b&&l(b)}),[b]);return{selectedValue:o,selectValue:(0,a.useCallback)((e=>{if(!p({value:e,tabValues:i}))throw new Error(`Can't select invalid tab value=${e}`);l(e),d(e),m(e)}),[d,m,i]),tabValues:i}}var m=t(2389);const b={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var g=t(5893);function x(e){let{className:n,block:t,selectedValue:a,selectValue:o,tabValues:s}=e;const l=[],{blockElementScrollPositionUntilNextRender:c}=(0,i.o5)(),u=e=>{const n=e.currentTarget,t=l.indexOf(n),r=s[t].value;r!==a&&(c(n),o(r))},d=e=>{let n=null;switch(e.key){case"Enter":u(e);break;case"ArrowRight":{const t=l.indexOf(e.currentTarget)+1;n=l[t]??l[0];break}case"ArrowLeft":{const t=l.indexOf(e.currentTarget)-1;n=l[t]??l[l.length-1];break}}n?.focus()};return(0,g.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,r.Z)("tabs",{"tabs--block":t},n),children:s.map((e=>{let{value:n,label:t,attributes:i}=e;return(0,g.jsx)("li",{role:"tab",tabIndex:a===n?0:-1,"aria-selected":a===n,ref:e=>l.push(e),onKeyDown:d,onClick:u,...i,className:(0,r.Z)("tabs__item",b.tabItem,i?.className,{"tabs__item--active":a===n}),children:t??n},n)}))})}function y(e){let{lazy:n,children:t,selectedValue:r}=e;const i=(Array.isArray(t)?t:[t]).filter(Boolean);if(n){const e=i.find((e=>e.props.value===r));return e?(0,a.cloneElement)(e,{className:"margin-top--md"}):null}return(0,g.jsx)("div",{className:"margin-top--md",children:i.map(((e,n)=>(0,a.cloneElement)(e,{key:n,hidden:e.props.value!==r})))})}function j(e){const n=v(e);return(0,g.jsxs)("div",{className:(0,r.Z)("tabs-container",b.tabList),children:[(0,g.jsx)(x,{...e,...n}),(0,g.jsx)(y,{...e,...n})]})}function w(e){const n=(0,m.Z)();return(0,g.jsx)(j,{...e,children:d(e.children)},String(n))}},1151:(e,n,t)=>{t.d(n,{Z:()=>s,a:()=>o});var a=t(7294);const r={},i=a.createContext(r);function o(e){const n=a.useContext(i);return a.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function s(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(r):e.components||r:o(e.components),a.createElement(i.Provider,{value:n},e.children)}}}]);