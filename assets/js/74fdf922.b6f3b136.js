"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[783],{3905:(e,n,t)=>{t.d(n,{Zo:()=>p,kt:()=>b});var a=t(7294);function s(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function o(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);n&&(a=a.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,a)}return t}function i(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?o(Object(t),!0).forEach((function(n){s(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):o(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function r(e,n){if(null==e)return{};var t,a,s=function(e,n){if(null==e)return{};var t,a,s={},o=Object.keys(e);for(a=0;a<o.length;a++)t=o[a],n.indexOf(t)>=0||(s[t]=e[t]);return s}(e,n);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(a=0;a<o.length;a++)t=o[a],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(s[t]=e[t])}return s}var l=a.createContext({}),c=function(e){var n=a.useContext(l),t=n;return e&&(t="function"==typeof e?e(n):i(i({},n),e)),t},p=function(e){var n=c(e.components);return a.createElement(l.Provider,{value:n},e.children)},d="mdxType",u={inlineCode:"code",wrapper:function(e){var n=e.children;return a.createElement(a.Fragment,{},n)}},m=a.forwardRef((function(e,n){var t=e.components,s=e.mdxType,o=e.originalType,l=e.parentName,p=r(e,["components","mdxType","originalType","parentName"]),d=c(t),m=s,b=d["".concat(l,".").concat(m)]||d[m]||u[m]||o;return t?a.createElement(b,i(i({ref:n},p),{},{components:t})):a.createElement(b,i({ref:n},p))}));function b(e,n){var t=arguments,s=n&&n.mdxType;if("string"==typeof e||s){var o=t.length,i=new Array(o);i[0]=m;var r={};for(var l in n)hasOwnProperty.call(n,l)&&(r[l]=n[l]);r.originalType=e,r[d]="string"==typeof e?e:s,i[1]=r;for(var c=2;c<o;c++)i[c]=t[c];return a.createElement.apply(null,i)}return a.createElement.apply(null,t)}m.displayName="MDXCreateElement"},5162:(e,n,t)=>{t.d(n,{Z:()=>i});var a=t(7294),s=t(6010);const o="tabItem_Ymn6";function i(e){let{children:n,hidden:t,className:i}=e;return a.createElement("div",{role:"tabpanel",className:(0,s.Z)(o,i),hidden:t},n)}},5488:(e,n,t)=>{t.d(n,{Z:()=>m});var a=t(7462),s=t(7294),o=t(6010),i=t(2389),r=t(7392),l=t(7094),c=t(2466);const p="tabList__CuJ",d="tabItem_LNqP";function u(e){const{lazy:n,block:t,defaultValue:i,values:u,groupId:m,className:b}=e,g=s.Children.map(e.children,(e=>{if((0,s.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),h=u??g.map((e=>{let{props:{value:n,label:t,attributes:a}}=e;return{value:n,label:t,attributes:a}})),k=(0,r.l)(h,((e,n)=>e.value===n.value));if(k.length>0)throw new Error(`Docusaurus error: Duplicate values "${k.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const v=null===i?i:i??g.find((e=>e.props.default))?.props.value??g[0].props.value;if(null!==v&&!h.some((e=>e.value===v)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${v}" but none of its children has the corresponding value. Available values are: ${h.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:f,setTabGroupChoices:y}=(0,l.U)(),[w,D]=(0,s.useState)(v),N=[],{blockElementScrollPositionUntilNextRender:I}=(0,c.o5)();if(null!=m){const e=f[m];null!=e&&e!==w&&h.some((n=>n.value===e))&&D(e)}const S=e=>{const n=e.currentTarget,t=N.indexOf(n),a=h[t].value;a!==w&&(I(n),D(a),null!=m&&y(m,String(a)))},T=e=>{let n=null;switch(e.key){case"Enter":S(e);break;case"ArrowRight":{const t=N.indexOf(e.currentTarget)+1;n=N[t]??N[0];break}case"ArrowLeft":{const t=N.indexOf(e.currentTarget)-1;n=N[t]??N[N.length-1];break}}n?.focus()};return s.createElement("div",{className:(0,o.Z)("tabs-container",p)},s.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":t},b)},h.map((e=>{let{value:n,label:t,attributes:i}=e;return s.createElement("li",(0,a.Z)({role:"tab",tabIndex:w===n?0:-1,"aria-selected":w===n,key:n,ref:e=>N.push(e),onKeyDown:T,onClick:S},i,{className:(0,o.Z)("tabs__item",d,i?.className,{"tabs__item--active":w===n})}),t??n)}))),n?(0,s.cloneElement)(g.filter((e=>e.props.value===w))[0],{className:"margin-top--md"}):s.createElement("div",{className:"margin-top--md"},g.map(((e,n)=>(0,s.cloneElement)(e,{key:n,hidden:e.props.value!==w})))))}function m(e){const n=(0,i.Z)();return s.createElement(u,(0,a.Z)({key:String(n)},e))}},8846:(e,n,t)=>{t.d(n,{Z:()=>r});var a=t(7294);const s="codeDescContainer_ie8f",o="desc_jyqI",i="example_eYlF";function r(e){let{children:n}=e,t=a.Children.toArray(n).filter((e=>e));return a.createElement("div",{className:s},a.createElement("div",{className:o},t[0]),a.createElement("div",{className:i},t[1]))}},3422:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>d,contentTitle:()=>c,default:()=>b,frontMatter:()=>l,metadata:()=>p,toc:()=>u});var a=t(7462),s=(t(7294),t(3905)),o=t(8846),i=t(5488),r=t(5162);const l={},c="Scopes",p={unversionedId:"guides/scopes",id:"guides/scopes",title:"Scopes",description:"A scope is Stashbox's implementation of the unit-of-work pattern; it encapsulates a given unit used to resolve and store instances required for a given work. When a scoped service is resolved or injected, the scope ensures that it gets instantiated only once within the scope's lifetime. When the work is finished, the scope cleans up the resources by disposing each tracked disposable instance.",source:"@site/docs/guides/scopes.md",sourceDirName:"guides",slug:"/guides/scopes",permalink:"/stashbox/docs/guides/scopes",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/guides/scopes.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1674223474,formattedLastUpdatedAt:"Jan 20, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Lifetimes",permalink:"/stashbox/docs/guides/lifetimes"},next:{title:"Registration configuration",permalink:"/stashbox/docs/configuration/registration-configuration"}},d={},u=[{value:"Creating a scope",id:"creating-a-scope",level:2},{value:"Named scopes",id:"named-scopes",level:2},{value:"Service as scope",id:"service-as-scope",level:2},{value:"Put instance to a scope",id:"put-instance-to-a-scope",level:2},{value:"Disposal",id:"disposal",level:2},{value:"Async disposal",id:"async-disposal",level:3},{value:"Finalizer delegate",id:"finalizer-delegate",level:3}],m={toc:u};function b(e){let{components:n,...t}=e;return(0,s.kt)("wrapper",(0,a.Z)({},m,t,{components:n,mdxType:"MDXLayout"}),(0,s.kt)("h1",{id:"scopes"},"Scopes"),(0,s.kt)("p",null,"A scope is Stashbox's implementation of the unit-of-work pattern; it encapsulates a given unit used to resolve and store instances required for a given work. When a scoped service is resolved or injected, the scope ensures that it gets instantiated only once within the scope's lifetime. When the work is finished, the scope cleans up the resources by disposing each tracked disposable instance."),(0,s.kt)("p",null,"A web application is a fair usage example for scopes as it has a well-defined execution unit that can be bound to a scope - the HTTP request. Every request could have its unique scope attached to the request's lifetime. When a request ends, the scope gets closed, and all the scoped instances will be disposed."),(0,s.kt)("h2",{id:"creating-a-scope"},"Creating a scope"),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"You can create a scope from the container by calling its ",(0,s.kt)("inlineCode",{parentName:"p"},".BeginScope()")," method."),(0,s.kt)("p",null,"Scopes can be ",(0,s.kt)("strong",{parentName:"p"},"nested"),", which means you can create sub-scopes from existing ones with their ",(0,s.kt)("inlineCode",{parentName:"p"},".BeginScope()")," method. "),(0,s.kt)("p",null,"Scoped service instances are not shared across parent and sub-scope relations."),(0,s.kt)("p",null,"Nested scopes can be ",(0,s.kt)("strong",{parentName:"p"},"attached to their parent's lifetime"),", which means when a parent gets disposed all child scopes attached to it will be disposed."),(0,s.kt)("p",null,"Scopes are ",(0,s.kt)("inlineCode",{parentName:"p"},"IDisposable"),"; they track all ",(0,s.kt)("inlineCode",{parentName:"p"},"IDisposable")," instances they resolved. Calling their ",(0,s.kt)("inlineCode",{parentName:"p"},"Dispose()")," method or wrapping them in ",(0,s.kt)("inlineCode",{parentName:"p"},"using")," statements is a crucial part of their service's lifetime management.")),(0,s.kt)("div",null,(0,s.kt)(i.Z,{mdxType:"Tabs"},(0,s.kt)(r.Z,{value:"Create",label:"Create",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterScoped<IJob, DbBackup>();\n\n// create the scope with using so it'll be auto disposed.\nusing (var scope = container.BeginScope())\n{\n    IJob job = scope.Resolve<IJob>();\n    IJob jobAgain = scope.Resolve<IJob>();\n    // job and jobAgain are created in the \n    // same scope, so they are the same instance.\n}\n"))),(0,s.kt)(r.Z,{value:"Nested",label:"Nested",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterScoped<IJob, DbBackup>();\n\nusing (var parent = container.BeginScope())\n{\n    IJob job = parent.Resolve<IJob>();\n    IJob jobAgain = parent.Resolve<IJob>();\n    // job and jobAgain are created in the \n    // same scope, so they are the same instance.\n\n    // create a sub-scope.\n    using var sub = parent.BeginScope();\n\n    IJob subJob = sub.Resolve<IJob>();\n    // subJob is a new instance created in the sub-scope, \n    // differs from either job and jobAgain.\n}\n"))),(0,s.kt)(r.Z,{value:"Nested attached",label:"Nested attached",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterScoped<IJob, DbBackup>();\n\nvar parent = container.BeginScope();\nvar sub = parent.BeginScope(attachToParent: true);\n\n// sub will also be disposed with the scope.\nscope.Dispose(); \n")))))),(0,s.kt)("h2",{id:"named-scopes"},"Named scopes"),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"There might be cases where you don't want to use a service globally across every scope, only in specific ones. "),(0,s.kt)("p",null,"For this reason, you can differentiate specific scope groups from other scopes with a ",(0,s.kt)("strong",{parentName:"p"},"name"),"."),(0,s.kt)("p",null,"You can set a service's lifetime to ",(0,s.kt)("a",{parentName:"p",href:"/docs/guides/lifetimes#named-scope-lifetime"},"named scope lifetime")," initialized with the ",(0,s.kt)("strong",{parentName:"p"},"scope's name")," to mark it usable only for that named scope."),(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>(options => \n    options.InNamedScope("DbScope"));\n\ncontainer.Register<IJob, DbCleanup>(options => \n    options.InNamedScope("DbScope"));\n\ncontainer.Register<IJob, DbIndexRebuild>(options => \n    options.InNamedScope("DbSubScope"));\n\ncontainer.Register<IJob, StorageCleanup>(options => \n    options.InNamedScope("StorageScope"));\n')),(0,s.kt)("admonition",{type:"note"},(0,s.kt)("p",{parentName:"admonition"},"Services with named scope lifetime are ",(0,s.kt)("strong",{parentName:"p"},"shared across parent and sub-scope relations"),".")),(0,s.kt)("p",null,"If you request a name-scoped service from an un-named scope, you'll get an error or no result (depending on the configuration) because those services are selectable only by named scopes with a matching name.")),(0,s.kt)("div",null,(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},'using (var dbScope = container.BeginScope("DbScope"))\n{ \n    // DbBackup and DbCleanup will be returned.\n    IEnumerable<IJob> jobs = dbScope.ResolveAll<IJob>();\n\n    // create a sub-scope of dbScope.\n    using var sub = dbScope.BeginScope();\n\n    // DbBackup and DbCleanup will be returned from the named parent scope.\n    IEnumerable<IJob> jobs = sub.ResolveAll<IJob>();\n\n    // create a named sub-scope.\n    using var namedSub = dbScope.BeginScope("DbSubScope");\n    // DbIndexRebuild will be returned from the named sub-scope.\n    IEnumerable<IJob> jobs = namedSub.ResolveAll<IJob>();\n}\n\nusing (var storageScope = container.BeginScope("StorageScope"))\n{\n    // StorageCleanup will be returned.\n    IJob job = storageScope.Resolve<IJob>();\n}\n\n// create a common scope without a name.\nusing (var unNamed = container.BeginScope())\n{\n    // empty result as there\'s no service registered without named scope.\n    IEnumerable<IJob> jobs = unNamed.ResolveAll<IJob>();\n\n    // throws an exception because there\'s no unnamed service registered.\n    IJob job = unNamed.Resolve<IJob>();\n}\n')))),(0,s.kt)("h2",{id:"service-as-scope"},"Service as scope"),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"You can configure a service to behave like a nested named scope. At the resolution of this kind of service, a new dedicated named scope is created implicitly for managing the service's dependencies. "),(0,s.kt)("p",null,"With this feature, you can organize your dependencies around logical groups (named scopes) instead of individual services."),(0,s.kt)("p",null,"Using ",(0,s.kt)("inlineCode",{parentName:"p"},"InScopeDefinedBy()"),", you can bind services to a defined scope without giving it a name. In this case, the defining service's ",(0,s.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"implementation type")," is used for naming the scope."),(0,s.kt)("admonition",{type:"note"},(0,s.kt)("p",{parentName:"admonition"},"The lifetime of the defined scope is attached to the current scope that was used to create the service."))),(0,s.kt)("div",null,(0,s.kt)(i.Z,{mdxType:"Tabs"},(0,s.kt)(r.Z,{value:"Define named",label:"Define named",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>(options => options\n    .DefinesScope("DbBackupScope"));\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    .InNamedScope("DbBackupScope"));\ncontainer.Register<ILogger, FileLogger>();\n\nvar scope = container.BeginScope();\n\n// DbBackup will create a named scope with the name "DbBackupScope".\n// the named scope will select ConsoleLogger as it\'s \n// bound to the named scope\'s identifier.\nIJob job = scope.Resolve<IJob>();\n\n// this will dispose the implicitly created named scope by DbBackup.\nscope.Dispose(); \n'))),(0,s.kt)(r.Z,{value:"Define typed",label:"Define typed",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => options\n    .DefinesScope());\ncontainer.Register<ILogger, ConsoleLogger>(options => options\n    .InScopeDefinedBy<DbBackup>());\ncontainer.Register<ILogger, FileLogger>();\n\nvar scope = container.BeginScope();\n\n// DbBackup will create a named scope with the name typeof(DbBackup).\n// the named scope will select ConsoleLogger as it's \n// bound to the named scope's identifier.\nIJob job = scope.Resolve<IJob>();\n\n// this will dispose the implicitly created named scope by DbBackup.\nscope.Dispose(); \n")))))),(0,s.kt)("h2",{id:"put-instance-to-a-scope"},"Put instance to a scope"),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"You can add an already instantiated service to a scope. The instance's lifetime will be tracked by the given scope.")),(0,s.kt)("div",null,(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"using var scope = container.BeginScope();\nscope.PutInstanceInScope<IJob>(new DbBackup());\n")))),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"You can disable the tracking by passing ",(0,s.kt)("inlineCode",{parentName:"p"},"true")," for the ",(0,s.kt)("inlineCode",{parentName:"p"},"withoutDisposalTracking")," parameter. In this case, only the strong reference to the instance is dropped when the scope is disposed.")),(0,s.kt)("div",null,(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"using var scope = container.BeginScope();\nscope.PutInstanceInScope<IJob>(new DbBackup(), withoutDisposalTracking: true);\n")))),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"You can also give your instance a name to use it like a ",(0,s.kt)("a",{parentName:"p",href:"/docs/guides/basics#named-registration"},"named registration"),":")),(0,s.kt)("div",null,(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},'using var scope = container.BeginScope();\nscope.PutInstanceInScope<IDrow>(new DbBackup(), false, name: "DbBackup");\n')))),(0,s.kt)("admonition",{type:"note"},(0,s.kt)("p",{parentName:"admonition"},"Instances put to a scope will take precedence over existing registrations with the same ",(0,s.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),".")),(0,s.kt)("h2",{id:"disposal"},"Disposal"),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"The currently resolving scope tracks services that implement either ",(0,s.kt)("inlineCode",{parentName:"p"},"IDisposable")," or ",(0,s.kt)("inlineCode",{parentName:"p"},"IAsyncDisposable"),". This means that when the scope is disposed, all the tracked disposable instances will be disposed with it."),(0,s.kt)("admonition",{type:"note"},(0,s.kt)("p",{parentName:"admonition"},"Disposing the container will dispose all the singleton instances and their dependencies."))),(0,s.kt)("div",null,(0,s.kt)(i.Z,{groupId:"lifetime-dispose",mdxType:"Tabs"},(0,s.kt)(r.Z,{value:"Using",label:"Using",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"using (var scope = container.BeginScope())\n{\n    var disposable = scope.Resolve<DisposableService>();\n} // 'disposable' will be disposed when \n  // the using statement ends.\n"))),(0,s.kt)(r.Z,{value:"Dispose",label:"Dispose",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"var scope = container.BeginScope();\nvar disposable = scope.Resolve<DisposableService>();\n\n// 'disposable' will be disposed with the scope.\nscope.Dispose();\n")))))),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("p",null,"You can disable the disposal tracking on a ",(0,s.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-registration--registered-service"},"service registration")," with the ",(0,s.kt)("inlineCode",{parentName:"p"},".WithoutDisposalTracking()")," option.")),(0,s.kt)("div",null,(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => \n    options.WithoutDisposalTracking());\n")))),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("h3",{id:"async-disposal"},"Async disposal"),(0,s.kt)("p",null,"As the container and its scopes implement the ",(0,s.kt)("inlineCode",{parentName:"p"},"IAsyncDisposable")," interface, you can dispose them asynchronously when they are used in an ",(0,s.kt)("inlineCode",{parentName:"p"},"async")," context."),(0,s.kt)("p",null,"Calling ",(0,s.kt)("inlineCode",{parentName:"p"},"DisposeAsync")," disposes both ",(0,s.kt)("inlineCode",{parentName:"p"},"IDisposable")," and ",(0,s.kt)("inlineCode",{parentName:"p"},"IAsyncDisposable")," instances; however, calling ",(0,s.kt)("inlineCode",{parentName:"p"},"Dispose")," only disposes ",(0,s.kt)("inlineCode",{parentName:"p"},"IDisposable")," instances.")),(0,s.kt)("div",null,(0,s.kt)(i.Z,{groupId:"lifetime-dispose",mdxType:"Tabs"},(0,s.kt)(r.Z,{value:"Using",label:"Using",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"await using (var scope = container.BeginScope())\n{\n    var disposable = scope.Resolve<DisposableService>();\n} // 'disposable' will be disposed asynchronously \n  // when the using statement ends.\n"))),(0,s.kt)(r.Z,{value:"Dispose",label:"Dispose",mdxType:"TabItem"},(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"var scope = container.BeginScope();\nvar disposable = scope.Resolve<DisposableService>();\n\n// 'disposable' will be disposed asynchronously with the scope.\nawait scope.DisposeAsync();\n")))))),(0,s.kt)(o.Z,{mdxType:"CodeDescPanel"},(0,s.kt)("div",null,(0,s.kt)("h3",{id:"finalizer-delegate"},"Finalizer delegate"),(0,s.kt)("p",null,"During ",(0,s.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-registration--registered-service"},"service registration"),", you can set a custom finalizer delegate that will be invoked at the service's disposal.")),(0,s.kt)("div",null,(0,s.kt)("pre",null,(0,s.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>(options => \n    options.WithFinalizer(backup => \n        backup.CloseDbConnection()));\n")))))}b.isMDXComponent=!0}}]);