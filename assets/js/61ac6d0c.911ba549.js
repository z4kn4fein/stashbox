"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[834],{3905:(e,t,n)=>{n.d(t,{Zo:()=>c,kt:()=>v});var a=n(7294);function r(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function i(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function o(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?i(Object(n),!0).forEach((function(t){r(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):i(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function l(e,t){if(null==e)return{};var n,a,r=function(e,t){if(null==e)return{};var n,a,r={},i=Object.keys(e);for(a=0;a<i.length;a++)n=i[a],t.indexOf(n)>=0||(r[n]=e[n]);return r}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(a=0;a<i.length;a++)n=i[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(r[n]=e[n])}return r}var s=a.createContext({}),p=function(e){var t=a.useContext(s),n=t;return e&&(n="function"==typeof e?e(t):o(o({},t),e)),n},c=function(e){var t=p(e.components);return a.createElement(s.Provider,{value:t},e.children)},d="mdxType",u={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},m=a.forwardRef((function(e,t){var n=e.components,r=e.mdxType,i=e.originalType,s=e.parentName,c=l(e,["components","mdxType","originalType","parentName"]),d=p(n),m=r,v=d["".concat(s,".").concat(m)]||d[m]||u[m]||i;return n?a.createElement(v,o(o({ref:t},c),{},{components:n})):a.createElement(v,o({ref:t},c))}));function v(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var i=n.length,o=new Array(i);o[0]=m;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l[d]="string"==typeof e?e:r,o[1]=l;for(var p=2;p<i;p++)o[p]=n[p];return a.createElement.apply(null,o)}return a.createElement.apply(null,n)}m.displayName="MDXCreateElement"},5162:(e,t,n)=>{n.d(t,{Z:()=>o});var a=n(7294),r=n(6010);const i="tabItem_Ymn6";function o(e){let{children:t,hidden:n,className:o}=e;return a.createElement("div",{role:"tabpanel",className:(0,r.Z)(i,o),hidden:n},t)}},5488:(e,t,n)=>{n.d(t,{Z:()=>m});var a=n(7462),r=n(7294),i=n(6010),o=n(2389),l=n(7392),s=n(7094),p=n(2466);const c="tabList__CuJ",d="tabItem_LNqP";function u(e){const{lazy:t,block:n,defaultValue:o,values:u,groupId:m,className:v}=e,k=r.Children.map(e.children,(e=>{if((0,r.isValidElement)(e)&&"value"in e.props)return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)})),y=u??k.map((e=>{let{props:{value:t,label:n,attributes:a}}=e;return{value:t,label:n,attributes:a}})),h=(0,l.l)(y,((e,t)=>e.value===t.value));if(h.length>0)throw new Error(`Docusaurus error: Duplicate values "${h.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`);const b=null===o?o:o??k.find((e=>e.props.default))?.props.value??k[0].props.value;if(null!==b&&!y.some((e=>e.value===b)))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${b}" but none of its children has the corresponding value. Available values are: ${y.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);const{tabGroupChoices:g,setTabGroupChoices:f}=(0,s.U)(),[N,C]=(0,r.useState)(b),I=[],{blockElementScrollPositionUntilNextRender:w}=(0,p.o5)();if(null!=m){const e=g[m];null!=e&&e!==N&&y.some((t=>t.value===e))&&C(e)}const S=e=>{const t=e.currentTarget,n=I.indexOf(t),a=y[n].value;a!==N&&(w(t),C(a),null!=m&&f(m,String(a)))},T=e=>{let t=null;switch(e.key){case"Enter":S(e);break;case"ArrowRight":{const n=I.indexOf(e.currentTarget)+1;t=I[n]??I[0];break}case"ArrowLeft":{const n=I.indexOf(e.currentTarget)-1;t=I[n]??I[I.length-1];break}}t?.focus()};return r.createElement("div",{className:(0,i.Z)("tabs-container",c)},r.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,i.Z)("tabs",{"tabs--block":n},v)},y.map((e=>{let{value:t,label:n,attributes:o}=e;return r.createElement("li",(0,a.Z)({role:"tab",tabIndex:N===t?0:-1,"aria-selected":N===t,key:t,ref:e=>I.push(e),onKeyDown:T,onClick:S},o,{className:(0,i.Z)("tabs__item",d,o?.className,{"tabs__item--active":N===t})}),n??t)}))),t?(0,r.cloneElement)(k.filter((e=>e.props.value===N))[0],{className:"margin-top--md"}):r.createElement("div",{className:"margin-top--md"},k.map(((e,t)=>(0,r.cloneElement)(e,{key:t,hidden:e.props.value!==N})))))}function m(e){const t=(0,o.Z)();return r.createElement(u,(0,a.Z)({key:String(t)},e))}},8846:(e,t,n)=>{n.d(t,{Z:()=>l});var a=n(7294);const r="codeDescContainer_ie8f",i="desc_jyqI",o="example_eYlF";function l(e){let{children:t}=e,n=a.Children.toArray(t).filter((e=>e));return a.createElement("div",{className:r},a.createElement("div",{className:i},n[0]),a.createElement("div",{className:o},n[1]))}},2328:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>d,contentTitle:()=>p,default:()=>v,frontMatter:()=>s,metadata:()=>c,toc:()=>u});var a=n(7462),r=(n(7294),n(3905)),i=n(8846),o=n(5488),l=n(5162);const s={},p="Wrappers & resolvers",c={unversionedId:"advanced/wrappers-resolvers",id:"advanced/wrappers-resolvers",title:"Wrappers & resolvers",description:"Stashbox uses so-called Wrapper and Resolver implementations to handle special resolution requests that none of the service registrations can fulfill. Functionalities like wrapper and unknown type resolution, cross-container requests, optional and default value injection are all built with resolvers.",source:"@site/docs/advanced/wrappers-resolvers.md",sourceDirName:"advanced",slug:"/advanced/wrappers-resolvers",permalink:"/stashbox/docs/advanced/wrappers-resolvers",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/wrappers-resolvers.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1685618358,formattedLastUpdatedAt:"Jun 1, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Decorators",permalink:"/stashbox/docs/advanced/decorators"},next:{title:"Child containers",permalink:"/stashbox/docs/advanced/child-containers"}},d={},u=[{value:"Pre-defined wrappers &amp; resolvers",id:"pre-defined-wrappers--resolvers",level:2},{value:"Wrappers",id:"wrappers",level:2},{value:"Enumerable",id:"enumerable",level:3},{value:"Lazy",id:"lazy",level:3},{value:"Delegate",id:"delegate",level:3},{value:"Metadata &amp; Tuple",id:"metadata--tuple",level:3},{value:"KeyValuePair &amp; ReadOnlyKeyValue",id:"keyvaluepair--readonlykeyvalue",level:3},{value:"User-defined wrappers &amp; resolvers",id:"user-defined-wrappers--resolvers",level:2},{value:"Visiting order",id:"visiting-order",level:2}],m={toc:u};function v(e){let{components:t,...n}=e;return(0,r.kt)("wrapper",(0,a.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("h1",{id:"wrappers--resolvers"},"Wrappers & resolvers"),(0,r.kt)("p",null,"Stashbox uses so-called ",(0,r.kt)("em",{parentName:"p"},"Wrapper")," and ",(0,r.kt)("em",{parentName:"p"},"Resolver")," implementations to handle special resolution requests that none of the ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-registration--registered-service"},"service registrations")," can fulfill. Functionalities like ",(0,r.kt)("a",{parentName:"p",href:"/docs/advanced/wrappers-resolvers#wrappers"},"wrapper")," and ",(0,r.kt)("a",{parentName:"p",href:"/docs/advanced/special-resolution-cases#unknown-type-resolution"},"unknown type")," resolution, ",(0,r.kt)("a",{parentName:"p",href:"/docs/advanced/child-containers"},"cross-container requests"),", ",(0,r.kt)("a",{parentName:"p",href:"/docs/advanced/special-resolution-cases#optional-value-injection"},"optional")," and ",(0,r.kt)("a",{parentName:"p",href:"/docs/advanced/special-resolution-cases#default-value-injection"},"default value")," injection are all built with resolvers."),(0,r.kt)("h2",{id:"pre-defined-wrappers--resolvers"},"Pre-defined wrappers & resolvers"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"EnumerableWrapper"),": Used to resolve a collection of services wrapped in one of the collection interfaces that a .NET ",(0,r.kt)("inlineCode",{parentName:"li"},"Array")," implements. (",(0,r.kt)("inlineCode",{parentName:"li"},"IEnumerable<>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"IList<>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"ICollection<>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"IReadOnlyList<>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"IReadOnlyCollection<>"),") "),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"LazyWrapper"),": Used to resolve services ",(0,r.kt)("a",{parentName:"li",href:"/docs/advanced/wrappers-resolvers#lazy"},"wrapped")," in ",(0,r.kt)("inlineCode",{parentName:"li"},"Lazy<>"),"."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"FuncWrapper"),": Used to resolve services ",(0,r.kt)("a",{parentName:"li",href:"/docs/advanced/wrappers-resolvers#delegate"},"wrapped")," in a ",(0,r.kt)("inlineCode",{parentName:"li"},"Delegate")," that has a non-void return type like ",(0,r.kt)("inlineCode",{parentName:"li"},"Func<>"),"."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"MetadataWrapper"),": Used to resolve services ",(0,r.kt)("a",{parentName:"li",href:"/docs/advanced/wrappers-resolvers#metadata--tuple"},"wrapped")," in ",(0,r.kt)("inlineCode",{parentName:"li"},"ValueTuple<,>"),", ",(0,r.kt)("inlineCode",{parentName:"li"},"Tuple<,>"),", or ",(0,r.kt)("inlineCode",{parentName:"li"},"Metadata<,>"),"."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"KeyValueWrapper"),": Used to resolve services ",(0,r.kt)("a",{parentName:"li",href:"/docs/advanced/wrappers-resolvers#keyvaluepair--readonlykeyvalue"},"wrapped")," in ",(0,r.kt)("inlineCode",{parentName:"li"},"KeyValuePair<,>")," or ",(0,r.kt)("inlineCode",{parentName:"li"},"ReadOnlyKeyValue<,>"),"."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"ServiceProviderResolver"),": User to resolve the actual scope as ",(0,r.kt)("inlineCode",{parentName:"li"},"IServiceProvider")," when no other implementation is registered."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"OptionalValueResolver"),": Used to resolve optional parameters."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"DefaultValueResolver"),": Used to resolve default values."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"ParentContainerResolver"),": Used to resolve services that are only registered in one of the parent containers."),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"UnknownTypeResolver"),": Used to resolve services that are not registered into the container.")),(0,r.kt)("h2",{id:"wrappers"},"Wrappers"),(0,r.kt)("p",null,"Stashbox can implicitly wrap your services into different data structures. All functionalities covered in the ",(0,r.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution"},"service resolution")," are applied to the wrappers. Every wrapper request starts as a standard resolution; only the result is wrapped in the requested structure."),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("h3",{id:"enumerable"},"Enumerable"),(0,r.kt)("p",null,"Stashbox can compose a collection from each implementation registered to a ",(0,r.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-type--implementation-type"},"service type"),". The requested type can be wrapped by any of the collection interfaces that a .NET ",(0,r.kt)("inlineCode",{parentName:"p"},"Array")," implements.")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"IJob[] jobs = container.Resolve<IJob[]>();\nIEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>();\nIList<IJob> jobs = container.Resolve<IList<IJob>>();\nICollection<IJob> jobs = container.Resolve<ICollection<IJob>>();\nIReadOnlyList<IJob> jobs = container.Resolve<IReadOnlyList<IJob>>();\nIReadOnlyCollection<IJob> jobs = container.Resolve<IReadOnlyCollection<IJob>>();\n")))),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("h3",{id:"lazy"},"Lazy"),(0,r.kt)("p",null,"When requesting ",(0,r.kt)("inlineCode",{parentName:"p"},"Lazy<>"),", the container implicitly constructs a new ",(0,r.kt)("inlineCode",{parentName:"p"},"Lazy<>")," instance with a factory delegate as its constructor argument used to instantiate the underlying service. ")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IJob, DbBackup>();\n\n// new Lazy(() => new DbBackup())\nLazy<IJob> lazyJob = container.Resolve<Lazy<IJob>>();\nIJob job = lazyJob.Value;\n")))),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("h3",{id:"delegate"},"Delegate"),(0,r.kt)("p",null,"When requesting a ",(0,r.kt)("inlineCode",{parentName:"p"},"Delegate"),", the container implicitly creates a factory used to instantiate the underlying service."),(0,r.kt)("p",null,"It's possible to request a delegate that expects some or all of the dependencies as delegate parameters.\nParameters are used for sub-dependencies as well, like: ",(0,r.kt)("inlineCode",{parentName:"p"},"(arg) => new A(new B(arg))")),(0,r.kt)("p",null,"When a dependency is not available as a parameter, it will be resolved from the container directly.")),(0,r.kt)("div",null,(0,r.kt)(o.Z,{mdxType:"Tabs"},(0,r.kt)(l.Z,{value:"Func",label:"Func",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>();\n\n// (conn, logger) => new DbBackup(conn, logger)\nFunc<string, ILogger, IJob> funcOfJob = container\n    .Resolve<Func<string, ILogger, IJob>>();\n    \nIJob job = funcOfJob(config["connectionString"], new ConsoleLogger());\n'))),(0,r.kt)(l.Z,{value:"Custom delegate",label:"Custom delegate",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'private delegate IJob JobFactory(string connectionString, ILogger logger);\n\ncontainer.Register<IJob, DbBackup>();\n\nvar jobDelegate = container.Resolve<JobFactory>();\nIJob job = jobDelegate(config["connectionString"], new ConsoleLogger());\n')))))),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("h3",{id:"metadata--tuple"},"Metadata & Tuple"),(0,r.kt)("p",null,"With the ",(0,r.kt)("inlineCode",{parentName:"p"},".WithMetadata()")," registration option, you can attach additional information to a service.\nTo gather this information, you can request the service wrapped in either ",(0,r.kt)("inlineCode",{parentName:"p"},"Metadata<,>"),", ",(0,r.kt)("inlineCode",{parentName:"p"},"ValueTuple<,>"),", or ",(0,r.kt)("inlineCode",{parentName:"p"},"Tuple<,>"),"."),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"Metadata<,>")," is a type from the ",(0,r.kt)("inlineCode",{parentName:"p"},"Stashbox")," package, so you might prefer using ",(0,r.kt)("inlineCode",{parentName:"p"},"ValueTuple<,>")," or ",(0,r.kt)("inlineCode",{parentName:"p"},"Tuple<,>")," if you want to avoid referencing Stashbox in certain parts of your project."),(0,r.kt)("p",null,"You can also filter a collection of services by their metadata. Requesting ",(0,r.kt)("inlineCode",{parentName:"p"},"IEnumerable<ValueTuple<,>>")," will yield only those services that have the given type of metadata.")),(0,r.kt)("div",null,(0,r.kt)(o.Z,{mdxType:"Tabs"},(0,r.kt)(l.Z,{value:"Single service",label:"Single service",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IJob, DbBackup>(options => options\n    .WithMetadata("connection-string-to-db"));\n\nvar jobWithConnectionString = container.Resolve<Metadata<IJob, string>>();\n// prints: "connection-string-to-db"\nConsole.WriteLine(jobWithConnectionString.Data);\n\nvar alsoJobWithConnectionString = container.Resolve<ValueTuple<IJob, string>>();\n// prints: "connection-string-to-db"\nConsole.WriteLine(alsoJobWithConnectionString.Item2);\n\nvar stillJobWithConnectionString = container.Resolve<Tuple<IJob, string>>();\n// prints: "connection-string-to-db"\nConsole.WriteLine(stillJobWithConnectionString.Item2);\n'))),(0,r.kt)(l.Z,{value:"Collection filtering",label:"Collection filtering",mdxType:"TabItem"},(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IService, Service1>(options => options\n    .WithMetadata("meta-1"));\ncontainer.Register<IService, Service2>(options => options\n    .WithMetadata("meta-2"));\ncontainer.Register<IService, Service3>(options => options\n    .WithMetadata(5));\n\n// the result is: [Service1, Service2]\nvar servicesWithStringMetadata = container.Resolve<ValueTuple<IService, string>[]>();\n\n// the result is: [Service3]\nvar servicesWithIntMetadata = container.Resolve<ValueTuple<IService, int>[]>();\n')))))),(0,r.kt)("admonition",{type:"note"},(0,r.kt)("p",{parentName:"admonition"},"Metadata can also be a complex type e.g., an ",(0,r.kt)("inlineCode",{parentName:"p"},"IDictionary<,>"),".")),(0,r.kt)("admonition",{type:"info"},(0,r.kt)("p",{parentName:"admonition"},"When no service found for a particular metadata type, the container throws a ",(0,r.kt)("a",{parentName:"p",href:"/docs/diagnostics/validation#resolution-validation"},"ResolutionFailedException"),". In case of an ",(0,r.kt)("inlineCode",{parentName:"p"},"IEnumerable<>")," request, an empty collection will be returned for a non-existing metadata.")),(0,r.kt)(i.Z,{mdxType:"CodeDescPanel"},(0,r.kt)("div",null,(0,r.kt)("h3",{id:"keyvaluepair--readonlykeyvalue"},"KeyValuePair & ReadOnlyKeyValue"),(0,r.kt)("p",null,"With named registration, you can give your service unique identifiers. Requesting a service wrapped in a ",(0,r.kt)("inlineCode",{parentName:"p"},"KeyValuePair<object, TYourService>")," or ",(0,r.kt)("inlineCode",{parentName:"p"},"ReadOnlyKeyValue<object, TYourService>")," returns the requested service with its identifier as key."),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"ReadOnlyKeyValue<,>")," is a type from the ",(0,r.kt)("inlineCode",{parentName:"p"},"Stashbox")," package, so you might prefer using ",(0,r.kt)("inlineCode",{parentName:"p"},"KeyValuePair<,>")," if you want to avoid referencing Stashbox in certain parts of your project."),(0,r.kt)("p",null,"Requesting an ",(0,r.kt)("inlineCode",{parentName:"p"},"IEnumerable<KeyValuePair<,>>")," will return all services of the requested type along their identifiers. When a service don't have an identifier the ",(0,r.kt)("inlineCode",{parentName:"p"},"Key")," will be set to ",(0,r.kt)("inlineCode",{parentName:"p"},"null"),".")),(0,r.kt)("div",null,(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IService, Service1>("FirstServiceId");\ncontainer.Register<IService, Service2>("SecondServiceId");\ncontainer.Register<IService, Service3>();\n\nvar serviceKeyValue1 = container\n    .Resolve<KeyValuePair<object, IService>>("FirstServiceId");\n// prints: "FirstServiceId"\nConsole.WriteLine(serviceKeyValue1.Key);\n\nvar serviceKeyValue2 = container\n    .Resolve<ReadOnlyKeyValue<object, IService>>("SecondServiceId");\n// prints: "SecondServiceId"\nConsole.WriteLine(serviceKeyValue2.Key);\n\n// ["FirstServiceId": Service1, "SecondServiceId": Service2, null: Service3 ]\nvar servicesWithKeys = container.Resolve<KeyValuePair<object, IService>[]>();\n')))),(0,r.kt)("admonition",{type:"note"},(0,r.kt)("p",{parentName:"admonition"},"Wrappers can be composed e.g., ",(0,r.kt)("inlineCode",{parentName:"p"},"IEnumerable<Func<ILogger, Tuple<Lazy<IJob>, string>>>"),".")),(0,r.kt)("h2",{id:"user-defined-wrappers--resolvers"},"User-defined wrappers & resolvers"),(0,r.kt)("p",null,"You can add support for more wrapper types by implementing the ",(0,r.kt)("inlineCode",{parentName:"p"},"IServiceWrapper")," interface."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"class CustomWrapper : IServiceWrapper\n{\n    // this method is supposed to generate the expression for the given wrapper's \n    // instantiation when it's selected by the container to resolve the actual service.\n    public Expression WrapExpression(\n        TypeInformation originalTypeInformation, \n        TypeInformation wrappedTypeInformation, \n        ServiceContext serviceContext)\n    {\n        // produce the expression for the wrapper.\n    }\n\n    // this method is called by the container to determine whether a \n    // given requested type is wrapped by a supported wrapper type.\n    public bool TryUnWrap(\n        TypeInformation typeInformation, \n        out TypeInformation unWrappedType)\n    {\n        // this is just a reference implementation of \n        // un-wrapping a service from a given wrapper.\n        if (!CanUnWrapServiceType(typeInformation.Type))\n        {\n            unWrappedType = null;\n            return false;\n        }\n\n        var type = UnWrapServiceType(typeInformation.Type)\n\n        unWrappedType = typeInformation.Clone(type);\n        return true;\n    }\n}\n")),(0,r.kt)("p",null,"You can extend the functionality of the container by implementing the ",(0,r.kt)("inlineCode",{parentName:"p"},"IServiceResolver")," interface."),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"class CustomResolver : IServiceResolver\n{\n    // called to generate the expression for the given service\n    // when this resolver is selected (through CanUseForResolution()) \n    // to fulfill the request.\n    public ServiceContext GetExpression(\n        IResolutionStrategy resolutionStrategy,\n        TypeInformation typeInfo,\n        ResolutionContext resolutionContext)\n    {\n        var expression = GenerateExpression(); // resolution expression generation.\n        return expression.AsServiceContext();\n    }\n\n    public bool CanUseForResolution(\n        TypeInformation typeInfo,\n        ResolutionContext resolutionContext)\n    {\n        // the predicate that determines whether the resolver \n        // is able to resolve the requested service or not.\n        return IsUsableFor(typeInfo);\n    }\n}\n")),(0,r.kt)("p",null,"Then you can register your custom wrapper or resolver like this:"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs"},"container.RegisterResolver(new CustomWrapper());\ncontainer.RegisterResolver(new CustomResolver());\n")),(0,r.kt)("h2",{id:"visiting-order"},"Visiting order"),(0,r.kt)("p",null,"Stashbox visits the wrappers and resolvers in the following order to satisfy the actual resolution request:"),(0,r.kt)("ol",null,(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"EnumerableWrapper")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"LazyWrapper")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"FuncWrapper")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"MetadataWrapper")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"KeyValueWrapper")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("strong",{parentName:"li"},"Custom, user-defined wrappers & resolvers")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"ServiceProviderResolver")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"OptionalValueResolver")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"DefaultValueResolver")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"ParentContainerResolver")),(0,r.kt)("li",{parentName:"ol"},(0,r.kt)("inlineCode",{parentName:"li"},"UnknownTypeResolver"))))}v.isMDXComponent=!0}}]);