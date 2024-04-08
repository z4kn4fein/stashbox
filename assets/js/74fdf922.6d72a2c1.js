"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[783],{9034:(e,s,n)=>{n.r(s),n.d(s,{assets:()=>p,contentTitle:()=>l,default:()=>b,frontMatter:()=>r,metadata:()=>d,toc:()=>u});var i=n(5893),o=n(1151),a=n(8846),t=n(4866),c=n(5162);const r={},l="Scopes",d={id:"guides/scopes",title:"Scopes",description:"A scope is Stashbox's implementation of the unit-of-work pattern; it encapsulates a given unit used to resolve and store instances required for a given work. When a scoped service is resolved or injected, the scope ensures that it gets instantiated only once within the scope's lifetime. When the work is finished, the scope cleans up the resources by disposing each tracked disposable instance.",source:"@site/docs/guides/scopes.md",sourceDirName:"guides",slug:"/guides/scopes",permalink:"/stashbox/docs/guides/scopes",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/scopes.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1712620140,formattedLastUpdatedAt:"Apr 8, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Lifetimes",permalink:"/stashbox/docs/guides/lifetimes"},next:{title:"Registration configuration",permalink:"/stashbox/docs/configuration/registration-configuration"}},p={},u=[{value:"Creating a scope",id:"creating-a-scope",level:2},{value:"Named scopes",id:"named-scopes",level:2},{value:"Service as scope",id:"service-as-scope",level:2},{value:"Put instance to a scope",id:"put-instance-to-a-scope",level:2},{value:"Disposal",id:"disposal",level:2},{value:"Async disposal",id:"async-disposal",level:3},{value:"Finalizer delegate",id:"finalizer-delegate",level:3}];function h(e){const s={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",h3:"h3",p:"p",pre:"pre",strong:"strong",...(0,o.a)(),...e.components};return(0,i.jsxs)(i.Fragment,{children:[(0,i.jsx)(s.h1,{id:"scopes",children:"Scopes"}),"\n",(0,i.jsx)(s.p,{children:"A scope is Stashbox's implementation of the unit-of-work pattern; it encapsulates a given unit used to resolve and store instances required for a given work. When a scoped service is resolved or injected, the scope ensures that it gets instantiated only once within the scope's lifetime. When the work is finished, the scope cleans up the resources by disposing each tracked disposable instance."}),"\n",(0,i.jsx)(s.p,{children:"A web application is a fair usage example for scopes as it has a well-defined execution unit that can be bound to a scope - the HTTP request. Every request could have its unique scope attached to the request's lifetime. When a request ends, the scope gets closed, and all the scoped instances will be disposed."}),"\n",(0,i.jsx)(s.h2,{id:"creating-a-scope",children:"Creating a scope"}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsxs)(s.p,{children:["You can create a scope from the container by calling its ",(0,i.jsx)(s.code,{children:".BeginScope()"})," method."]}),(0,i.jsxs)(s.p,{children:["Scopes can be ",(0,i.jsx)(s.strong,{children:"nested"}),", which means you can create sub-scopes from existing ones with their ",(0,i.jsx)(s.code,{children:".BeginScope()"})," method."]}),(0,i.jsx)(s.p,{children:"Scoped service instances are not shared across parent and sub-scope relations."}),(0,i.jsxs)(s.p,{children:["Nested scopes can be ",(0,i.jsx)(s.strong,{children:"attached to their parent's lifetime"}),", which means when a parent gets disposed all child scopes attached to it will be disposed."]}),(0,i.jsxs)(s.p,{children:["Scopes are ",(0,i.jsx)(s.code,{children:"IDisposable"}),"; they track all ",(0,i.jsx)(s.code,{children:"IDisposable"})," instances they resolved. Calling their ",(0,i.jsx)(s.code,{children:"Dispose()"})," method or wrapping them in ",(0,i.jsx)(s.code,{children:"using"})," statements is a crucial part of their service's lifetime management."]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(t.Z,{children:[(0,i.jsx)(c.Z,{value:"Create",label:"Create",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"container.RegisterScoped<IJob, DbBackup>();\n\n// create the scope with using so it'll be auto disposed.\nusing (var scope = container.BeginScope())\n{\n    IJob job = scope.Resolve<IJob>();\n    IJob jobAgain = scope.Resolve<IJob>();\n    // job and jobAgain are created in the \n    // same scope, so they are the same instance.\n}\n"})})}),(0,i.jsx)(c.Z,{value:"Nested",label:"Nested",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"container.RegisterScoped<IJob, DbBackup>();\n\nusing (var parent = container.BeginScope())\n{\n    IJob job = parent.Resolve<IJob>();\n    IJob jobAgain = parent.Resolve<IJob>();\n    // job and jobAgain are created in the \n    // same scope, so they are the same instance.\n\n    // create a sub-scope.\n    using var sub = parent.BeginScope();\n\n    IJob subJob = sub.Resolve<IJob>();\n    // subJob is a new instance created in the sub-scope, \n    // differs from either job and jobAgain.\n}\n"})})}),(0,i.jsx)(c.Z,{value:"Nested attached",label:"Nested attached",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"container.RegisterScoped<IJob, DbBackup>();\n\nvar parent = container.BeginScope();\nvar sub = parent.BeginScope(attachToParent: true);\n\n// sub will also be disposed with the scope.\nscope.Dispose(); \n"})})})]})})]}),"\n",(0,i.jsx)(s.h2,{id:"named-scopes",children:"Named scopes"}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(s.p,{children:"There might be cases where you don't want to use a service globally across every scope, only in specific ones."}),(0,i.jsxs)(s.p,{children:["For this reason, you can differentiate specific scope groups from other scopes with a ",(0,i.jsx)(s.strong,{children:"name"}),"."]}),(0,i.jsxs)(s.p,{children:["You can set a service's lifetime to ",(0,i.jsx)(s.a,{href:"/docs/guides/lifetimes#named-scope-lifetime",children:"named scope lifetime"})," initialized with the ",(0,i.jsx)(s.strong,{children:"scope's name"})," to mark it usable only for that named scope."]}),(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>(options => \n    options.InNamedScope("DbScope"));\n\ncontainer.Register<IJob, DbCleanup>(options => \n    options.InNamedScope("DbScope"));\n\ncontainer.Register<IJob, DbIndexRebuild>(options => \n    options.InNamedScope("DbSubScope"));\n\ncontainer.Register<IJob, StorageCleanup>(options => \n    options.InNamedScope("StorageScope"));\n'})}),(0,i.jsx)(s.admonition,{type:"note",children:(0,i.jsxs)(s.p,{children:["Services with named scope lifetime are ",(0,i.jsx)(s.strong,{children:"shared across parent and sub-scope relations"}),"."]})}),(0,i.jsx)(s.p,{children:"If you request a name-scoped service from an un-named scope, you'll get an error or no result (depending on the configuration) because those services are selectable only by named scopes with a matching name."})]}),(0,i.jsx)("div",{children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:'using (var dbScope = container.BeginScope("DbScope"))\n{ \n    // DbBackup and DbCleanup will be returned.\n    IEnumerable<IJob> jobs = dbScope.ResolveAll<IJob>();\n\n    // create a sub-scope of dbScope.\n    using var sub = dbScope.BeginScope();\n\n    // DbBackup and DbCleanup will be returned from the named parent scope.\n    IEnumerable<IJob> jobs = sub.ResolveAll<IJob>();\n\n    // create a named sub-scope.\n    using var namedSub = dbScope.BeginScope("DbSubScope");\n    // DbIndexRebuild will be returned from the named sub-scope.\n    IEnumerable<IJob> jobs = namedSub.ResolveAll<IJob>();\n}\n\nusing (var storageScope = container.BeginScope("StorageScope"))\n{\n    // StorageCleanup will be returned.\n    IJob job = storageScope.Resolve<IJob>();\n}\n\n// create a common scope without a name.\nusing (var unNamed = container.BeginScope())\n{\n    // empty result as there\'s no service registered without named scope.\n    IEnumerable<IJob> jobs = unNamed.ResolveAll<IJob>();\n\n    // throws an exception because there\'s no unnamed service registered.\n    IJob job = unNamed.Resolve<IJob>();\n}\n'})})})]}),"\n",(0,i.jsx)(s.h2,{id:"service-as-scope",children:"Service as scope"}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(s.p,{children:"You can configure a service to behave like a nested named scope. At the resolution of this kind of service, a new dedicated named scope is created implicitly for managing the service's dependencies."}),(0,i.jsx)(s.p,{children:"With this feature, you can organize your dependencies around logical groups (named scopes) instead of individual services."}),(0,i.jsxs)(s.p,{children:["Using ",(0,i.jsx)(s.code,{children:"InScopeDefinedBy()"}),", you can bind services to a defined scope without giving it a name. In this case, the defining service's ",(0,i.jsx)(s.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"implementation type"})," is used for naming the scope."]}),(0,i.jsx)(s.admonition,{type:"note",children:(0,i.jsx)(s.p,{children:"The lifetime of the defined scope is attached to the current scope that was used to create the service."})})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(t.Z,{children:[(0,i.jsx)(c.Z,{value:"Define named",label:"Define named",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>(options => options\n    .DefinesScope("DbBackupScope"));\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    .InNamedScope("DbBackupScope"));\ncontainer.Register<ILogger, FileLogger>();\n\nvar scope = container.BeginScope();\n\n// DbBackup will create a named scope with the name "DbBackupScope".\n// the named scope will select ConsoleLogger as it\'s \n// bound to the named scope\'s identifier.\nIJob job = scope.Resolve<IJob>();\n\n// this will dispose the implicitly created named scope by DbBackup.\nscope.Dispose(); \n'})})}),(0,i.jsx)(c.Z,{value:"Define typed",label:"Define typed",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => options\n    .DefinesScope());\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    .InScopeDefinedBy<DbBackup>());\ncontainer.Register<ILogger, FileLogger>();\n\nvar scope = container.BeginScope();\n\n// DbBackup will create a named scope with the name typeof(DbBackup).\n// the named scope will select ConsoleLogger as it's \n// bound to the named scope's identifier.\nIJob job = scope.Resolve<IJob>();\n\n// this will dispose the implicitly created named scope by DbBackup.\nscope.Dispose(); \n"})})})]})})]}),"\n",(0,i.jsx)(s.h2,{id:"put-instance-to-a-scope",children:"Put instance to a scope"}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsx)("div",{children:(0,i.jsx)(s.p,{children:"You can add an already instantiated service to a scope. The instance's lifetime will be tracked by the given scope."})}),(0,i.jsx)("div",{children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"using var scope = container.BeginScope();\nscope.PutInstanceInScope<IJob>(new DbBackup());\n"})})})]}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsx)("div",{children:(0,i.jsxs)(s.p,{children:["You can disable the tracking by passing ",(0,i.jsx)(s.code,{children:"true"})," for the ",(0,i.jsx)(s.code,{children:"withoutDisposalTracking"})," parameter. In this case, only the strong reference to the instance is dropped when the scope is disposed."]})}),(0,i.jsx)("div",{children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"using var scope = container.BeginScope();\nscope.PutInstanceInScope<IJob>(new DbBackup(), withoutDisposalTracking: true);\n"})})})]}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsx)("div",{children:(0,i.jsxs)(s.p,{children:["You can also give your instance a name to use it like a ",(0,i.jsx)(s.a,{href:"/docs/guides/basics#named-registration",children:"named registration"}),":"]})}),(0,i.jsx)("div",{children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:'using var scope = container.BeginScope();\nscope.PutInstanceInScope<IDrow>(new DbBackup(), false, name: "DbBackup");\n'})})})]}),"\n",(0,i.jsx)(s.admonition,{type:"note",children:(0,i.jsxs)(s.p,{children:["Instances put to a scope will take precedence over existing registrations with the same ",(0,i.jsx)(s.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),"."]})}),"\n",(0,i.jsx)(s.h2,{id:"disposal",children:"Disposal"}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsxs)(s.p,{children:["The currently resolving scope tracks services that implement either ",(0,i.jsx)(s.code,{children:"IDisposable"})," or ",(0,i.jsx)(s.code,{children:"IAsyncDisposable"}),". This means that when the scope is disposed, all the tracked disposable instances will be disposed with it."]}),(0,i.jsx)(s.admonition,{type:"note",children:(0,i.jsx)(s.p,{children:"Disposing the container will dispose all the singleton instances and their dependencies."})})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(t.Z,{groupId:"lifetime-dispose",children:[(0,i.jsx)(c.Z,{value:"Using",label:"Using",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"using (var scope = container.BeginScope())\n{\n    var disposable = scope.Resolve<DisposableService>();\n} // 'disposable' will be disposed when \n  // the using statement ends.\n"})})}),(0,i.jsx)(c.Z,{value:"Dispose",label:"Dispose",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"var scope = container.BeginScope();\nvar disposable = scope.Resolve<DisposableService>();\n\n// 'disposable' will be disposed with the scope.\nscope.Dispose();\n"})})})]})})]}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsx)("div",{children:(0,i.jsxs)(s.p,{children:["You can disable the disposal tracking on a ",(0,i.jsx)(s.a,{href:"/docs/getting-started/glossary#service-registration--registered-service",children:"service registration"})," with the ",(0,i.jsx)(s.code,{children:".WithoutDisposalTracking()"})," option."]})}),(0,i.jsx)("div",{children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => \n    options.WithoutDisposalTracking());\n"})})})]}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(s.h3,{id:"async-disposal",children:"Async disposal"}),(0,i.jsxs)(s.p,{children:["As the container and its scopes implement the ",(0,i.jsx)(s.code,{children:"IAsyncDisposable"})," interface, you can dispose them asynchronously when they are used in an ",(0,i.jsx)(s.code,{children:"async"})," context."]}),(0,i.jsxs)(s.p,{children:["Calling ",(0,i.jsx)(s.code,{children:"DisposeAsync"})," disposes both ",(0,i.jsx)(s.code,{children:"IDisposable"})," and ",(0,i.jsx)(s.code,{children:"IAsyncDisposable"})," instances; however, calling ",(0,i.jsx)(s.code,{children:"Dispose"})," only disposes ",(0,i.jsx)(s.code,{children:"IDisposable"})," instances."]})]}),(0,i.jsx)("div",{children:(0,i.jsxs)(t.Z,{groupId:"lifetime-dispose",children:[(0,i.jsx)(c.Z,{value:"Using",label:"Using",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"await using (var scope = container.BeginScope())\n{\n    var disposable = scope.Resolve<DisposableService>();\n} // 'disposable' will be disposed asynchronously \n  // when the using statement ends.\n"})})}),(0,i.jsx)(c.Z,{value:"Dispose",label:"Dispose",children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"var scope = container.BeginScope();\nvar disposable = scope.Resolve<DisposableService>();\n\n// 'disposable' will be disposed asynchronously with the scope.\nawait scope.DisposeAsync();\n"})})})]})})]}),"\n",(0,i.jsxs)(a.Z,{children:[(0,i.jsxs)("div",{children:[(0,i.jsx)(s.h3,{id:"finalizer-delegate",children:"Finalizer delegate"}),(0,i.jsxs)(s.p,{children:["During ",(0,i.jsx)(s.a,{href:"/docs/getting-started/glossary#service-registration--registered-service",children:"service registration"}),", you can set a custom finalizer delegate that will be invoked at the service's disposal."]})]}),(0,i.jsx)("div",{children:(0,i.jsx)(s.pre,{children:(0,i.jsx)(s.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>(options => \n    options.WithFinalizer(backup => \n        backup.CloseDbConnection()));\n"})})})]})]})}function b(e={}){const{wrapper:s}={...(0,o.a)(),...e.components};return s?(0,i.jsx)(s,{...e,children:(0,i.jsx)(h,{...e})}):h(e)}},5162:(e,s,n)=>{n.d(s,{Z:()=>t});n(7294);var i=n(4334);const o={tabItem:"tabItem_Ymn6"};var a=n(5893);function t(e){let{children:s,hidden:n,className:t}=e;return(0,a.jsx)("div",{role:"tabpanel",className:(0,i.Z)(o.tabItem,t),hidden:n,children:s})}},4866:(e,s,n)=>{n.d(s,{Z:()=>w});var i=n(7294),o=n(4334),a=n(2466),t=n(6550),c=n(469),r=n(1980),l=n(7392),d=n(12);function p(e){return i.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,i.isValidElement)(e)&&function(e){const{props:s}=e;return!!s&&"object"==typeof s&&"value"in s}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function u(e){const{values:s,children:n}=e;return(0,i.useMemo)((()=>{const e=s??function(e){return p(e).map((e=>{let{props:{value:s,label:n,attributes:i,default:o}}=e;return{value:s,label:n,attributes:i,default:o}}))}(n);return function(e){const s=(0,l.l)(e,((e,s)=>e.value===s.value));if(s.length>0)throw new Error(`Docusaurus error: Duplicate values "${s.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[s,n])}function h(e){let{value:s,tabValues:n}=e;return n.some((e=>e.value===s))}function b(e){let{queryString:s=!1,groupId:n}=e;const o=(0,t.k6)(),a=function(e){let{queryString:s=!1,groupId:n}=e;if("string"==typeof s)return s;if(!1===s)return null;if(!0===s&&!n)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return n??null}({queryString:s,groupId:n});return[(0,r._X)(a),(0,i.useCallback)((e=>{if(!a)return;const s=new URLSearchParams(o.location.search);s.set(a,e),o.replace({...o.location,search:s.toString()})}),[a,o])]}function g(e){const{defaultValue:s,queryString:n=!1,groupId:o}=e,a=u(e),[t,r]=(0,i.useState)((()=>function(e){let{defaultValue:s,tabValues:n}=e;if(0===n.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(s){if(!h({value:s,tabValues:n}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${s}" but none of its children has the corresponding value. Available values are: ${n.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return s}const i=n.find((e=>e.default))??n[0];if(!i)throw new Error("Unexpected error: 0 tabValues");return i.value}({defaultValue:s,tabValues:a}))),[l,p]=b({queryString:n,groupId:o}),[g,m]=function(e){let{groupId:s}=e;const n=function(e){return e?`docusaurus.tab.${e}`:null}(s),[o,a]=(0,d.Nk)(n);return[o,(0,i.useCallback)((e=>{n&&a.set(e)}),[n,a])]}({groupId:o}),v=(()=>{const e=l??g;return h({value:e,tabValues:a})?e:null})();(0,c.Z)((()=>{v&&r(v)}),[v]);return{selectedValue:t,selectValue:(0,i.useCallback)((e=>{if(!h({value:e,tabValues:a}))throw new Error(`Can't select invalid tab value=${e}`);r(e),p(e),m(e)}),[p,m,a]),tabValues:a}}var m=n(2389);const v={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var x=n(5893);function j(e){let{className:s,block:n,selectedValue:i,selectValue:t,tabValues:c}=e;const r=[],{blockElementScrollPositionUntilNextRender:l}=(0,a.o5)(),d=e=>{const s=e.currentTarget,n=r.indexOf(s),o=c[n].value;o!==i&&(l(s),t(o))},p=e=>{let s=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const n=r.indexOf(e.currentTarget)+1;s=r[n]??r[0];break}case"ArrowLeft":{const n=r.indexOf(e.currentTarget)-1;s=r[n]??r[r.length-1];break}}s?.focus()};return(0,x.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":n},s),children:c.map((e=>{let{value:s,label:n,attributes:a}=e;return(0,x.jsx)("li",{role:"tab",tabIndex:i===s?0:-1,"aria-selected":i===s,ref:e=>r.push(e),onKeyDown:p,onClick:d,...a,className:(0,o.Z)("tabs__item",v.tabItem,a?.className,{"tabs__item--active":i===s}),children:n??s},s)}))})}function f(e){let{lazy:s,children:n,selectedValue:o}=e;const a=(Array.isArray(n)?n:[n]).filter(Boolean);if(s){const e=a.find((e=>e.props.value===o));return e?(0,i.cloneElement)(e,{className:"margin-top--md"}):null}return(0,x.jsx)("div",{className:"margin-top--md",children:a.map(((e,s)=>(0,i.cloneElement)(e,{key:s,hidden:e.props.value!==o})))})}function y(e){const s=g(e);return(0,x.jsxs)("div",{className:(0,o.Z)("tabs-container",v.tabList),children:[(0,x.jsx)(j,{...e,...s}),(0,x.jsx)(f,{...e,...s})]})}function w(e){const s=(0,m.Z)();return(0,x.jsx)(y,{...e,children:p(e.children)},String(s))}},8846:(e,s,n)=>{n.d(s,{Z:()=>t});var i=n(7294);const o={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var a=n(5893);function t(e){let{children:s}=e,n=i.Children.toArray(s).filter((e=>e));return(0,a.jsxs)("div",{className:o.codeDescContainer,children:[(0,a.jsx)("div",{className:o.desc,children:n[0]}),(0,a.jsx)("div",{className:o.example,children:n[1]})]})}},1151:(e,s,n)=>{n.d(s,{Z:()=>c,a:()=>t});var i=n(7294);const o={},a=i.createContext(o);function t(e){const s=i.useContext(a);return i.useMemo((function(){return"function"==typeof e?e(s):{...s,...e}}),[s,e])}function c(e){let s;return s=e.disableParentContext?"function"==typeof e.components?e.components(o):e.components||o:t(e.components),i.createElement(a.Provider,{value:s},e.children)}}}]);