"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[946],{3905:(e,n,t)=>{t.d(n,{Zo:()=>d,kt:()=>m});var i=t(7294);function a(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function r(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);n&&(i=i.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,i)}return t}function o(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?r(Object(t),!0).forEach((function(n){a(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):r(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function l(e,n){if(null==e)return{};var t,i,a=function(e,n){if(null==e)return{};var t,i,a={},r=Object.keys(e);for(i=0;i<r.length;i++)t=r[i],n.indexOf(t)>=0||(a[t]=e[t]);return a}(e,n);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);for(i=0;i<r.length;i++)t=r[i],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(a[t]=e[t])}return a}var s=i.createContext({}),c=function(e){var n=i.useContext(s),t=n;return e&&(t="function"==typeof e?e(n):o(o({},n),e)),t},d=function(e){var n=c(e.components);return i.createElement(s.Provider,{value:n},e.children)},h="mdxType",p={inlineCode:"code",wrapper:function(e){var n=e.children;return i.createElement(i.Fragment,{},n)}},u=i.forwardRef((function(e,n){var t=e.components,a=e.mdxType,r=e.originalType,s=e.parentName,d=l(e,["components","mdxType","originalType","parentName"]),h=c(t),u=a,m=h["".concat(s,".").concat(u)]||h[u]||p[u]||r;return t?i.createElement(m,o(o({ref:n},d),{},{components:t})):i.createElement(m,o({ref:n},d))}));function m(e,n){var t=arguments,a=n&&n.mdxType;if("string"==typeof e||a){var r=t.length,o=new Array(r);o[0]=u;var l={};for(var s in n)hasOwnProperty.call(n,s)&&(l[s]=n[s]);l.originalType=e,l[h]="string"==typeof e?e:a,o[1]=l;for(var c=2;c<r;c++)o[c]=t[c];return i.createElement.apply(null,o)}return i.createElement.apply(null,t)}u.displayName="MDXCreateElement"},8846:(e,n,t)=>{t.d(n,{Z:()=>l});var i=t(7294);const a="codeDescContainer_ie8f",r="desc_jyqI",o="example_eYlF";function l(e){let{children:n}=e,t=i.Children.toArray(n).filter((e=>e));return i.createElement("div",{className:a},i.createElement("div",{className:r},t[0]),i.createElement("div",{className:o},t[1]))}},9674:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>c,contentTitle:()=>l,default:()=>p,frontMatter:()=>o,metadata:()=>s,toc:()=>d});var i=t(7462),a=(t(7294),t(3905)),r=t(8846);const o={},l="Child containers",s={unversionedId:"advanced/child-containers",id:"advanced/child-containers",title:"Child containers",description:"With child containers, you can build up parent-child relationships between containers. This means you can have a different subset of services present in a child than in the parent container.",source:"@site/docs/advanced/child-containers.md",sourceDirName:"advanced",slug:"/advanced/child-containers",permalink:"/stashbox/docs/advanced/child-containers",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/child-containers.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1693906449,formattedLastUpdatedAt:"Sep 5, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Wrappers & resolvers",permalink:"/stashbox/docs/advanced/wrappers-resolvers"},next:{title:"Special resolution cases",permalink:"/stashbox/docs/advanced/special-resolution-cases"}},c={},d=[{value:"Example",id:"example",level:2},{value:"Accessing child containers",id:"accessing-child-containers",level:2},{value:"Resolution behavior",id:"resolution-behavior",level:2},{value:"Re-building singletons",id:"re-building-singletons",level:2},{value:"Nested child containers",id:"nested-child-containers",level:2},{value:"Dispose",id:"dispose",level:2}],h={toc:d};function p(e){let{components:n,...t}=e;return(0,a.kt)("wrapper",(0,i.Z)({},h,t,{components:n,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"child-containers"},"Child containers"),(0,a.kt)("p",null,"With child containers, you can build up parent-child relationships between containers. This means you can have a different subset of services present in a child than in the parent container. "),(0,a.kt)("p",null,"When a dependency is missing from the child container during a resolution request, the parent will be asked to resolve the missing service. If it's found there, the parent will return only the service's registration, and the resolution request will jump back to the child. Also, child registrations with the same ",(0,a.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type")," will override the parent's services."),(0,a.kt)("p",null,"Resolving ",(0,a.kt)("inlineCode",{parentName:"p"},"IEnumerable<T>")," and ",(0,a.kt)("a",{parentName:"p",href:"/docs/advanced/decorators"},"decorators")," also considers parent containers by default. However, this behavior can be controlled with the ",(0,a.kt)("a",{parentName:"p",href:"#resolution-behavior"},(0,a.kt)("inlineCode",{parentName:"a"},"ResolutionBehavior"))," parameter. "),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"Child containers are the foundation of the ",(0,a.kt)("a",{parentName:"p",href:"https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#multitenant"},"ASP.NET Core multi-tenant extension"),".")),(0,a.kt)("h2",{id:"example"},"Example"),(0,a.kt)("p",null,"Here is an example case:"),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"interface IDependency {}\n\nclass B : IDependency {}\nclass C : IDependency {}\n\nclass A \n{\n    public A(IDependency dependency)\n    { }\n}\n\nusing (var container = new StashboxContainer())\n{\n    // register 'A' into the parent container.\n    container.Register<A>();\n\n    // register 'B' as a dependency into the parent container.\n    container.Register<IDependency, B>();\n\n    var child = container.CreateChildContainer()\n    \n    // register 'C' as a dependency into the child container.\n    child.Register<IDependency, C>();\n    \n    // 'A' is resolved from the parent and gets\n    // 'C' as IDependency because the resolution\n    // request was initiated on the child.\n    A fromChild = child.Resolve<A>();\n\n    // 'A' gets 'B' as IDependency because the \n    // resolution request was initiated on the parent.\n    A fromParent = container.Resolve<A>();\n} // using will dispose the parent along with the child.\n")),(0,a.kt)("p",null,"Let's see what's happening when we request ",(0,a.kt)("inlineCode",{parentName:"p"},"A")," from the ",(0,a.kt)("em",{parentName:"p"},"child"),":"),(0,a.kt)("ol",null,(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("inlineCode",{parentName:"li"},"A")," not found in the ",(0,a.kt)("em",{parentName:"li"},"child"),", go up to the ",(0,a.kt)("em",{parentName:"li"},"parent")," and check there."),(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("inlineCode",{parentName:"li"},"A")," found in the ",(0,a.kt)("em",{parentName:"li"},"parent"),", resolve."),(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("inlineCode",{parentName:"li"},"A")," depends on ",(0,a.kt)("inlineCode",{parentName:"li"},"IDependency"),", go back to the ",(0,a.kt)("em",{parentName:"li"},"child")," and search ",(0,a.kt)("inlineCode",{parentName:"li"},"IDependency")," implementations."),(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("inlineCode",{parentName:"li"},"C")," found in the ",(0,a.kt)("em",{parentName:"li"},"child"),", it does not have any dependencies, instantiate."),(0,a.kt)("li",{parentName:"ol"},"Inject the new ",(0,a.kt)("inlineCode",{parentName:"li"},"C")," instance into ",(0,a.kt)("inlineCode",{parentName:"li"},"A"),"."),(0,a.kt)("li",{parentName:"ol"},"All dependencies are resolved; return ",(0,a.kt)("inlineCode",{parentName:"li"},"A"),".")),(0,a.kt)("p",null,"When we make the same request on the parent, everything will go as usual because we have all dependencies in place. ",(0,a.kt)("inlineCode",{parentName:"p"},"B")," will be injected into ",(0,a.kt)("inlineCode",{parentName:"p"},"A"),"."),(0,a.kt)("admonition",{type:"info"},(0,a.kt)("p",{parentName:"admonition"},"You can ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration"},"re-configure")," child containers with the ",(0,a.kt)("inlineCode",{parentName:"p"},".Configure()")," method. It doesn't affect the parent container's configuration.")),(0,a.kt)("h2",{id:"accessing-child-containers"},"Accessing child containers"),(0,a.kt)("p",null,"You can identify child containers with the ",(0,a.kt)("inlineCode",{parentName:"p"},"identifier")," parameter of ",(0,a.kt)("inlineCode",{parentName:"p"},"CreateChildContainer()"),". Later, you can retrieve the given child container by passing its ID to ",(0,a.kt)("inlineCode",{parentName:"p"},"GetChildContainer()"),"."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'using var container = new StashboxContainer();\ncontainer.CreateChildContainer("child");\n// ...\n\nvar child = container.GetChildContainer("child");\n')),(0,a.kt)("p",null,"Also, each child container created by a container is available through the ",(0,a.kt)("inlineCode",{parentName:"p"},"IStashboxContainer.ChildContainers")," propert."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},'using var container = new StashboxContainer();\ncontainer.CreateChildContainer("child1");\ncontainer.CreateChildContainer("child2");\n// ...\n\nforeach (var child in container.ChildContainers)\n{\n    var id = child.Key;\n    var childContainer = child.Value;\n}\n')),(0,a.kt)("h2",{id:"resolution-behavior"},"Resolution behavior"),(0,a.kt)("p",null,"You can control which level of the container hierarchy can participate in the service resolution with the ",(0,a.kt)("inlineCode",{parentName:"p"},"ResolutionBehavior")," parameter. "),(0,a.kt)("p",null,"Possible values:"),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("inlineCode",{parentName:"li"},"Default"),": The default behavior, it's used when the parameter is not specified. Its value is ",(0,a.kt)("inlineCode",{parentName:"li"},"Parent | Current"),", so the parents and the current container (which initiated the resolution request) can participate in the resolution request's service selection."),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("inlineCode",{parentName:"li"},"Parent"),": Indicates that parent containers (including indirect all ancestors) can participate in the resolution request's service selection."),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("inlineCode",{parentName:"li"},"Current"),": Indicates that the current container (which initiated the resolution request) can participate in the service selection."),(0,a.kt)("li",{parentName:"ul"},(0,a.kt)("inlineCode",{parentName:"li"},"ParentDependency"),": Indicates that parent containers (including indirect all ancestors) can only provide dependencies for services that are already selected for resolution.")),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-csharp"},"interface IService {}\n\nclass A : IService {}\nclass B : IService {}\n\nusing (var container = new StashboxContainer())\n{\n    // register 'A' into the parent container.\n    container.Register<IService, A>();\n\n    var child = container.CreateChildContainer()\n    \n    // register 'B' into the child container.\n    child.Register<IService, B>();\n    \n    // 'A' is resolved because only parent\n    // can participate in the resolution request.\n    IService withParent = child.Resolve<IService>(ResolutionBehavior.Parent);\n\n    // Only 'B' is in the collection because\n    // only the caller container can take part\n    // in the resolution request.\n    IEnumerable<IService> allWithCurrent = child.Resolve<IEnumerable<IService>>(ResolutionBehavior.Current);\n    \n    // Both 'A' and 'B' is in the collection\n    // because both the parent and the caller container\n    // participates in the resolution request.\n    IEnumerable<IService> all = child.Resolve<IEnumerable<IService>>(ResolutionBehavior.Current | ResolutionBehavior.Parent);\n} // using will dispose the parent along with the child.\n")),(0,a.kt)("h2",{id:"re-building-singletons"},"Re-building singletons"),(0,a.kt)("p",null,"By default, singletons are instantiated and stored only in those containers that registered them. However, you can enable the re-instantiation of singletons in child containers with the ",(0,a.kt)("inlineCode",{parentName:"p"},".WithReBuildSingletonsInChildContainer()")," ",(0,a.kt)("a",{parentName:"p",href:"/docs/configuration/container-configuration#re-build-singletons-in-child-containers"},"container configuration option"),". "),(0,a.kt)("p",null,"If it's enabled, all singletons will be re-created in those containers that initiated the resolution request. By this, re-built singletons can use overridden dependencies from child containers. "),(0,a.kt)("p",null,"Re-building in child containers does not affect the singletons instantiated in the parent container."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"interface IDependency {}\n\nclass B : IDependency {}\nclass C : IDependency {}\n\nclass A \n{\n    public A(IDependency dependency)\n    { }\n}\n\nusing (var container = new StashboxContainer(options => options.WithReBuildSingletonsInChildContainer()))\n{\n    // register 'A' as a singleton into the parent container.\n    container.RegisterSingleton<A>();\n\n    // register 'B' as a dependency into the parent container.\n    container.Register<IDependency, B>();\n\n    // 'A' gets 'B' as IDependency and will be stored\n    // in the parent container as a singleton.\n    A fromParent = container.Resolve<A>();\n\n    var child = container.CreateChildContainer();\n    \n    // register 'C' as a dependency into the child container.\n    child.Register<IDependency, C>();\n\n    // a new 'A' singleton will be created in\n    // the child container with 'C' as IDependency.\n    A fromChild = child.Resolve<A>();\n} // using will dispose the parent along with the child.\n")),(0,a.kt)("h2",{id:"nested-child-containers"},"Nested child containers"),(0,a.kt)(r.Z,{mdxType:"CodeDescPanel"},(0,a.kt)("div",null,(0,a.kt)("p",null,"You can build up a hierarchical tree structure from containers by creating more child containers with the ",(0,a.kt)("inlineCode",{parentName:"p"},".CreateChildContainer()")," method.")),(0,a.kt)("div",null,(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"using var container = new StashboxContainer();\n\nvar child1 = container.CreateChildContainer();\nvar child2 = child1.CreateChildContainer();\n")))),(0,a.kt)("h2",{id:"dispose"},"Dispose"),(0,a.kt)("p",null,"By default, the parent container's disposal also disposes its child containers. You can control this behavior with the ",(0,a.kt)("inlineCode",{parentName:"p"},"CreateChildContainer()")," method's ",(0,a.kt)("inlineCode",{parentName:"p"},"attachToParent")," boolean parameter."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"using (var container = new StashboxContainer())\n{\n    using (var child1 = container.CreateChildContainer(attachToParent: false))\n    {\n    } // child1 will be disposed only once here.\n    \n    var child2 = container.CreateChildContainer();\n    var child3 = container.CreateChildContainer();\n} // using will dispose the parent along with child2 and child3.\n")),(0,a.kt)("p",null,"You can safely dispose a child even if it's attached to its parent, in this case the parent's disposal will not dispose the already disposed child."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-cs"},"using (var container = new StashboxContainer())\n{\n    using (var child1 = container.CreateChildContainer())\n    {\n    } // child1 will be disposed only once here.\n    \n    var child2 = container.CreateChildContainer();\n} // using will dispose only the parent and child2.\n")))}p.isMDXComponent=!0}}]);