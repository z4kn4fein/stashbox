"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[429],{9267:(e,n,r)=>{r.r(n),r.d(n,{assets:()=>u,contentTitle:()=>c,default:()=>v,frontMatter:()=>l,metadata:()=>d,toc:()=>p});var s=r(4848),t=r(8453),i=r(7470),a=r(1470),o=r(9365);const l={},c="Wrappers & resolvers",d={id:"advanced/wrappers-resolvers",title:"Wrappers & resolvers",description:"Stashbox uses so-called Wrapper and Resolver implementations to handle special resolution requests that none of the service registrations can fulfill. Functionalities like wrapper and unknown type resolution, cross-container requests, optional and default value injection are all built with resolvers.",source:"@site/docs/advanced/wrappers-resolvers.md",sourceDirName:"advanced",slug:"/advanced/wrappers-resolvers",permalink:"/stashbox/docs/advanced/wrappers-resolvers",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/wrappers-resolvers.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1734704927,formattedLastUpdatedAt:"Dec 20, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Decorators",permalink:"/stashbox/docs/advanced/decorators"},next:{title:"Child containers",permalink:"/stashbox/docs/advanced/child-containers"}},u={},p=[{value:"Pre-defined wrappers &amp; resolvers",id:"pre-defined-wrappers--resolvers",level:2},{value:"Wrappers",id:"wrappers",level:2},{value:"Enumerable",id:"enumerable",level:3},{value:"Lazy",id:"lazy",level:3},{value:"Delegate",id:"delegate",level:3},{value:"Metadata &amp; Tuple",id:"metadata--tuple",level:3},{value:"KeyValuePair &amp; ReadOnlyKeyValue",id:"keyvaluepair--readonlykeyvalue",level:3},{value:"User-defined wrappers &amp; resolvers",id:"user-defined-wrappers--resolvers",level:2},{value:"Visiting order",id:"visiting-order",level:2}];function h(e){const n={a:"a",admonition:"admonition",code:"code",em:"em",h1:"h1",h2:"h2",h3:"h3",li:"li",ol:"ol",p:"p",pre:"pre",strong:"strong",ul:"ul",...(0,t.R)(),...e.components};return(0,s.jsxs)(s.Fragment,{children:[(0,s.jsx)(n.h1,{id:"wrappers--resolvers",children:"Wrappers & resolvers"}),"\n",(0,s.jsxs)(n.p,{children:["Stashbox uses so-called ",(0,s.jsx)(n.em,{children:"Wrapper"})," and ",(0,s.jsx)(n.em,{children:"Resolver"})," implementations to handle special resolution requests that none of the ",(0,s.jsx)(n.a,{href:"/docs/getting-started/glossary#service-registration--registered-service",children:"service registrations"})," can fulfill. Functionalities like ",(0,s.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#wrappers",children:"wrapper"})," and ",(0,s.jsx)(n.a,{href:"/docs/advanced/special-resolution-cases#unknown-type-resolution",children:"unknown type"})," resolution, ",(0,s.jsx)(n.a,{href:"/docs/advanced/child-containers",children:"cross-container requests"}),", ",(0,s.jsx)(n.a,{href:"/docs/advanced/special-resolution-cases#optional-value-injection",children:"optional"})," and ",(0,s.jsx)(n.a,{href:"/docs/advanced/special-resolution-cases#default-value-injection",children:"default value"})," injection are all built with resolvers."]}),"\n",(0,s.jsx)(n.h2,{id:"pre-defined-wrappers--resolvers",children:"Pre-defined wrappers & resolvers"}),"\n",(0,s.jsxs)(n.ul,{children:["\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"EnumerableWrapper"}),": Used to resolve a collection of services wrapped in one of the collection interfaces that a .NET ",(0,s.jsx)(n.code,{children:"Array"})," implements. (",(0,s.jsx)(n.code,{children:"IEnumerable<>"}),", ",(0,s.jsx)(n.code,{children:"IList<>"}),", ",(0,s.jsx)(n.code,{children:"ICollection<>"}),", ",(0,s.jsx)(n.code,{children:"IReadOnlyList<>"}),", ",(0,s.jsx)(n.code,{children:"IReadOnlyCollection<>"}),")"]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"LazyWrapper"}),": Used to resolve services ",(0,s.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#lazy",children:"wrapped"})," in ",(0,s.jsx)(n.code,{children:"Lazy<>"}),"."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"FuncWrapper"}),": Used to resolve services ",(0,s.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#delegate",children:"wrapped"})," in a ",(0,s.jsx)(n.code,{children:"Delegate"})," that has a non-void return type like ",(0,s.jsx)(n.code,{children:"Func<>"}),"."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"MetadataWrapper"}),": Used to resolve services ",(0,s.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#metadata--tuple",children:"wrapped"})," in ",(0,s.jsx)(n.code,{children:"ValueTuple<,>"}),", ",(0,s.jsx)(n.code,{children:"Tuple<,>"}),", or ",(0,s.jsx)(n.code,{children:"Metadata<,>"}),"."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"KeyValueWrapper"}),": Used to resolve services ",(0,s.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#keyvaluepair--readonlykeyvalue",children:"wrapped"})," in ",(0,s.jsx)(n.code,{children:"KeyValuePair<,>"})," or ",(0,s.jsx)(n.code,{children:"ReadOnlyKeyValue<,>"}),"."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"ServiceProviderResolver"}),": Used to resolve the actual scope as ",(0,s.jsx)(n.code,{children:"IServiceProvider"})," when no other implementation is registered."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"OptionalValueResolver"}),": Used to resolve optional parameters."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"DefaultValueResolver"}),": Used to resolve default values."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"ParentContainerResolver"}),": Used to resolve services that are only registered in one of the parent containers."]}),"\n",(0,s.jsxs)(n.li,{children:[(0,s.jsx)(n.code,{children:"UnknownTypeResolver"}),": Used to resolve services that are not registered into the container."]}),"\n"]}),"\n",(0,s.jsx)(n.h2,{id:"wrappers",children:"Wrappers"}),"\n",(0,s.jsxs)(n.p,{children:["Stashbox can implicitly wrap your services into different data structures. All functionalities covered in the ",(0,s.jsx)(n.a,{href:"/docs/guides/service-resolution",children:"service resolution"})," are applied to the wrappers. Every wrapper request starts as a standard resolution; only the result is wrapped in the requested structure."]}),"\n",(0,s.jsxs)(i.A,{children:[(0,s.jsxs)("div",{children:[(0,s.jsx)(n.h3,{id:"enumerable",children:"Enumerable"}),(0,s.jsxs)(n.p,{children:["Stashbox can compose a collection from each implementation registered to a ",(0,s.jsx)(n.a,{href:"/docs/getting-started/glossary#service-type--implementation-type",children:"service type"}),". The requested type can be wrapped by any of the collection interfaces that a .NET ",(0,s.jsx)(n.code,{children:"Array"})," implements."]})]}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"IJob[] jobs = container.Resolve<IJob[]>();\nIEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>();\nIList<IJob> jobs = container.Resolve<IList<IJob>>();\nICollection<IJob> jobs = container.Resolve<ICollection<IJob>>();\nIReadOnlyList<IJob> jobs = container.Resolve<IReadOnlyList<IJob>>();\nIReadOnlyCollection<IJob> jobs = container.Resolve<IReadOnlyCollection<IJob>>();\n"})})})]}),"\n",(0,s.jsxs)(i.A,{children:[(0,s.jsxs)("div",{children:[(0,s.jsx)(n.h3,{id:"lazy",children:"Lazy"}),(0,s.jsxs)(n.p,{children:["When requesting ",(0,s.jsx)(n.code,{children:"Lazy<>"}),", the container implicitly constructs a new ",(0,s.jsx)(n.code,{children:"Lazy<>"})," instance with a factory delegate as its constructor argument used to instantiate the underlying service."]})]}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"container.Register<IJob, DbBackup>();\n\n// new Lazy(() => new DbBackup())\nLazy<IJob> lazyJob = container.Resolve<Lazy<IJob>>();\nIJob job = lazyJob.Value;\n"})})})]}),"\n",(0,s.jsxs)(i.A,{children:[(0,s.jsxs)("div",{children:[(0,s.jsx)(n.h3,{id:"delegate",children:"Delegate"}),(0,s.jsxs)(n.p,{children:["When requesting a ",(0,s.jsx)(n.code,{children:"Delegate"}),", the container implicitly creates a factory used to instantiate the underlying service."]}),(0,s.jsxs)(n.p,{children:["It's possible to request a delegate that expects some or all of the dependencies as delegate parameters.\nParameters are used for sub-dependencies as well, like: ",(0,s.jsx)(n.code,{children:"(arg) => new A(new B(arg))"})]}),(0,s.jsx)(n.p,{children:"When a dependency is not available as a parameter, it will be resolved from the container directly."})]}),(0,s.jsx)("div",{children:(0,s.jsxs)(a.A,{children:[(0,s.jsx)(o.A,{value:"Func",label:"Func",children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>();\n\n// (conn, logger) => new DbBackup(conn, logger)\nFunc<string, ILogger, IJob> funcOfJob = container\n    .Resolve<Func<string, ILogger, IJob>>();\n    \nIJob job = funcOfJob(config["connectionString"], new ConsoleLogger());\n'})})}),(0,s.jsx)(o.A,{value:"Custom delegate",label:"Custom delegate",children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:'private delegate IJob JobFactory(string connectionString, ILogger logger);\n\ncontainer.Register<IJob, DbBackup>();\n\nvar jobDelegate = container.Resolve<JobFactory>();\nIJob job = jobDelegate(config["connectionString"], new ConsoleLogger());\n'})})})]})})]}),"\n",(0,s.jsxs)(i.A,{children:[(0,s.jsxs)("div",{children:[(0,s.jsx)(n.h3,{id:"metadata--tuple",children:"Metadata & Tuple"}),(0,s.jsxs)(n.p,{children:["With the ",(0,s.jsx)(n.code,{children:".WithMetadata()"})," registration option, you can attach additional information to a service.\nTo gather this information, you can request the service wrapped in either ",(0,s.jsx)(n.code,{children:"Metadata<,>"}),", ",(0,s.jsx)(n.code,{children:"ValueTuple<,>"}),", or ",(0,s.jsx)(n.code,{children:"Tuple<,>"}),"."]}),(0,s.jsxs)(n.p,{children:[(0,s.jsx)(n.code,{children:"Metadata<,>"})," is a type from the ",(0,s.jsx)(n.code,{children:"Stashbox"})," package, so you might prefer using ",(0,s.jsx)(n.code,{children:"ValueTuple<,>"})," or ",(0,s.jsx)(n.code,{children:"Tuple<,>"})," if you want to avoid referencing Stashbox in certain parts of your project."]}),(0,s.jsxs)(n.p,{children:["You can also filter a collection of services by their metadata. Requesting ",(0,s.jsx)(n.code,{children:"IEnumerable<ValueTuple<,>>"})," will yield only those services that have the given type of metadata."]})]}),(0,s.jsx)("div",{children:(0,s.jsxs)(a.A,{children:[(0,s.jsx)(o.A,{value:"Single service",label:"Single service",children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:'container.Register<IJob, DbBackup>(options => options\n    .WithMetadata("connection-string-to-db"));\n\nvar jobWithConnectionString = container.Resolve<Metadata<IJob, string>>();\n// prints: "connection-string-to-db"\nConsole.WriteLine(jobWithConnectionString.Data);\n\nvar alsoJobWithConnectionString = container.Resolve<ValueTuple<IJob, string>>();\n// prints: "connection-string-to-db"\nConsole.WriteLine(alsoJobWithConnectionString.Item2);\n\nvar stillJobWithConnectionString = container.Resolve<Tuple<IJob, string>>();\n// prints: "connection-string-to-db"\nConsole.WriteLine(stillJobWithConnectionString.Item2);\n'})})}),(0,s.jsx)(o.A,{value:"Collection filtering",label:"Collection filtering",children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:'container.Register<IService, Service1>(options => options\n    .WithMetadata("meta-1"));\ncontainer.Register<IService, Service2>(options => options\n    .WithMetadata("meta-2"));\ncontainer.Register<IService, Service3>(options => options\n    .WithMetadata(5));\n\n// the result is: [Service1, Service2]\nvar servicesWithStringMetadata = container.Resolve<ValueTuple<IService, string>[]>();\n\n// the result is: [Service3]\nvar servicesWithIntMetadata = container.Resolve<ValueTuple<IService, int>[]>();\n'})})})]})})]}),"\n",(0,s.jsx)(n.admonition,{type:"note",children:(0,s.jsxs)(n.p,{children:["Metadata can also be a complex type e.g., an ",(0,s.jsx)(n.code,{children:"IDictionary<,>"}),"."]})}),"\n",(0,s.jsx)(n.admonition,{type:"info",children:(0,s.jsxs)(n.p,{children:["When no service found for a particular metadata type, the container throws a ",(0,s.jsx)(n.a,{href:"/docs/diagnostics/validation#resolution-validation",children:"ResolutionFailedException"}),". In case of an ",(0,s.jsx)(n.code,{children:"IEnumerable<>"})," request, an empty collection will be returned for a non-existing metadata."]})}),"\n",(0,s.jsxs)(i.A,{children:[(0,s.jsxs)("div",{children:[(0,s.jsx)(n.h3,{id:"keyvaluepair--readonlykeyvalue",children:"KeyValuePair & ReadOnlyKeyValue"}),(0,s.jsxs)(n.p,{children:["With named registration, you can give your service unique identifiers. Requesting a service wrapped in a ",(0,s.jsx)(n.code,{children:"KeyValuePair<object, TYourService>"})," or ",(0,s.jsx)(n.code,{children:"ReadOnlyKeyValue<object, TYourService>"})," returns the requested service with its identifier as key."]}),(0,s.jsxs)(n.p,{children:[(0,s.jsx)(n.code,{children:"ReadOnlyKeyValue<,>"})," is a type from the ",(0,s.jsx)(n.code,{children:"Stashbox"})," package, so you might prefer using ",(0,s.jsx)(n.code,{children:"KeyValuePair<,>"})," if you want to avoid referencing Stashbox in certain parts of your project."]}),(0,s.jsxs)(n.p,{children:["Requesting an ",(0,s.jsx)(n.code,{children:"IEnumerable<KeyValuePair<,>>"})," will return all services of the requested type along their identifiers. When a service don't have an identifier the ",(0,s.jsx)(n.code,{children:"Key"})," will be set to ",(0,s.jsx)(n.code,{children:"null"}),"."]})]}),(0,s.jsx)("div",{children:(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:'container.Register<IService, Service1>("FirstServiceId");\ncontainer.Register<IService, Service2>("SecondServiceId");\ncontainer.Register<IService, Service3>();\n\nvar serviceKeyValue1 = container\n    .Resolve<KeyValuePair<object, IService>>("FirstServiceId");\n// prints: "FirstServiceId"\nConsole.WriteLine(serviceKeyValue1.Key);\n\nvar serviceKeyValue2 = container\n    .Resolve<ReadOnlyKeyValue<object, IService>>("SecondServiceId");\n// prints: "SecondServiceId"\nConsole.WriteLine(serviceKeyValue2.Key);\n\n// ["FirstServiceId": Service1, "SecondServiceId": Service2, null: Service3 ]\nvar servicesWithKeys = container.Resolve<KeyValuePair<object, IService>[]>();\n'})})})]}),"\n",(0,s.jsx)(n.admonition,{type:"note",children:(0,s.jsxs)(n.p,{children:["Wrappers can be composed e.g., ",(0,s.jsx)(n.code,{children:"IEnumerable<Func<ILogger, Tuple<Lazy<IJob>, string>>>"}),"."]})}),"\n",(0,s.jsx)(n.h2,{id:"user-defined-wrappers--resolvers",children:"User-defined wrappers & resolvers"}),"\n",(0,s.jsxs)(n.p,{children:["You can add support for more wrapper types by implementing the ",(0,s.jsx)(n.code,{children:"IServiceWrapper"})," interface."]}),"\n",(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"class CustomWrapper : IServiceWrapper\n{\n    // this method is supposed to generate the expression for the given wrapper's \n    // instantiation when it's selected by the container to resolve the actual service.\n    public Expression WrapExpression(\n        TypeInformation originalTypeInformation, \n        TypeInformation wrappedTypeInformation, \n        ServiceContext serviceContext)\n    {\n        // produce the expression for the wrapper.\n    }\n\n    // this method is called by the container to determine whether a \n    // given requested type is wrapped by a supported wrapper type.\n    public bool TryUnWrap(Type type, out Type unWrappedType)\n    {\n        // this is just a reference implementation of \n        // un-wrapping a service from a given wrapper.\n        if (!CanUnWrapServiceType(type))\n        {\n            unWrappedType = typeof(object);\n            return false;\n        }\n\n        unWrappedType = UnWrapServiceType(type);\n        return true;\n    }\n}\n"})}),"\n",(0,s.jsxs)(n.p,{children:["You can extend the functionality of the container by implementing the ",(0,s.jsx)(n.code,{children:"IServiceResolver"})," interface."]}),"\n",(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"class CustomResolver : IServiceResolver\n{\n    // called to generate the expression for the given service\n    // when this resolver is selected (through CanUseForResolution()) \n    // to fulfill the request.\n    public ServiceContext GetExpression(\n        IResolutionStrategy resolutionStrategy,\n        TypeInformation typeInfo,\n        ResolutionContext resolutionContext)\n    {\n        var expression = GenerateExpression(); // resolution expression generation.\n        return expression.AsServiceContext();\n    }\n\n    public bool CanUseForResolution(\n        TypeInformation typeInfo,\n        ResolutionContext resolutionContext)\n    {\n\t    // the predicate that determines whether the resolver \n        // is able to resolve the requested service or not.\n        return IsUsableFor(typeInfo);\n    }\n}\n"})}),"\n",(0,s.jsx)(n.p,{children:"Then you can register your custom wrapper or resolver like this:"}),"\n",(0,s.jsx)(n.pre,{children:(0,s.jsx)(n.code,{className:"language-cs",children:"container.RegisterResolver(new CustomWrapper());\ncontainer.RegisterResolver(new CustomResolver());\n"})}),"\n",(0,s.jsx)(n.h2,{id:"visiting-order",children:"Visiting order"}),"\n",(0,s.jsx)(n.p,{children:"Stashbox visits the wrappers and resolvers in the following order to satisfy the actual resolution request:"}),"\n",(0,s.jsxs)(n.ol,{children:["\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"EnumerableWrapper"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"LazyWrapper"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"FuncWrapper"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"MetadataWrapper"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"KeyValueWrapper"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.strong,{children:"Custom, user-defined wrappers & resolvers"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"ServiceProviderResolver"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"OptionalValueResolver"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"DefaultValueResolver"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"ParentContainerResolver"})}),"\n",(0,s.jsx)(n.li,{children:(0,s.jsx)(n.code,{children:"UnknownTypeResolver"})}),"\n"]})]})}function v(e={}){const{wrapper:n}={...(0,t.R)(),...e.components};return n?(0,s.jsx)(n,{...e,children:(0,s.jsx)(h,{...e})}):h(e)}},9365:(e,n,r)=>{r.d(n,{A:()=>a});r(6540);var s=r(870);const t={tabItem:"tabItem_Ymn6"};var i=r(4848);function a(e){let{children:n,hidden:r,className:a}=e;return(0,i.jsx)("div",{role:"tabpanel",className:(0,s.A)(t.tabItem,a),hidden:r,children:n})}},1470:(e,n,r)=>{r.d(n,{A:()=>I});var s=r(6540),t=r(870),i=r(3104),a=r(6347),o=r(205),l=r(7485),c=r(1682),d=r(9466);function u(e){return s.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,s.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function p(e){const{values:n,children:r}=e;return(0,s.useMemo)((()=>{const e=n??function(e){return u(e).map((e=>{let{props:{value:n,label:r,attributes:s,default:t}}=e;return{value:n,label:r,attributes:s,default:t}}))}(r);return function(e){const n=(0,c.X)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,r])}function h(e){let{value:n,tabValues:r}=e;return r.some((e=>e.value===n))}function v(e){let{queryString:n=!1,groupId:r}=e;const t=(0,a.W6)(),i=function(e){let{queryString:n=!1,groupId:r}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!r)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return r??null}({queryString:n,groupId:r});return[(0,l.aZ)(i),(0,s.useCallback)((e=>{if(!i)return;const n=new URLSearchParams(t.location.search);n.set(i,e),t.replace({...t.location,search:n.toString()})}),[i,t])]}function x(e){const{defaultValue:n,queryString:r=!1,groupId:t}=e,i=p(e),[a,l]=(0,s.useState)((()=>function(e){let{defaultValue:n,tabValues:r}=e;if(0===r.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!h({value:n,tabValues:r}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${r.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const s=r.find((e=>e.default))??r[0];if(!s)throw new Error("Unexpected error: 0 tabValues");return s.value}({defaultValue:n,tabValues:i}))),[c,u]=v({queryString:r,groupId:t}),[x,j]=function(e){let{groupId:n}=e;const r=function(e){return e?`docusaurus.tab.${e}`:null}(n),[t,i]=(0,d.Dv)(r);return[t,(0,s.useCallback)((e=>{r&&i.set(e)}),[r,i])]}({groupId:t}),g=(()=>{const e=c??x;return h({value:e,tabValues:i})?e:null})();(0,o.A)((()=>{g&&l(g)}),[g]);return{selectedValue:a,selectValue:(0,s.useCallback)((e=>{if(!h({value:e,tabValues:i}))throw new Error(`Can't select invalid tab value=${e}`);l(e),u(e),j(e)}),[u,j,i]),tabValues:i}}var j=r(2303);const g={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var b=r(4848);function f(e){let{className:n,block:r,selectedValue:s,selectValue:a,tabValues:o}=e;const l=[],{blockElementScrollPositionUntilNextRender:c}=(0,i.a_)(),d=e=>{const n=e.currentTarget,r=l.indexOf(n),t=o[r].value;t!==s&&(c(n),a(t))},u=e=>{let n=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const r=l.indexOf(e.currentTarget)+1;n=l[r]??l[0];break}case"ArrowLeft":{const r=l.indexOf(e.currentTarget)-1;n=l[r]??l[l.length-1];break}}n?.focus()};return(0,b.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,t.A)("tabs",{"tabs--block":r},n),children:o.map((e=>{let{value:n,label:r,attributes:i}=e;return(0,b.jsx)("li",{role:"tab",tabIndex:s===n?0:-1,"aria-selected":s===n,ref:e=>l.push(e),onKeyDown:u,onClick:d,...i,className:(0,t.A)("tabs__item",g.tabItem,i?.className,{"tabs__item--active":s===n}),children:r??n},n)}))})}function m(e){let{lazy:n,children:r,selectedValue:t}=e;const i=(Array.isArray(r)?r:[r]).filter(Boolean);if(n){const e=i.find((e=>e.props.value===t));return e?(0,s.cloneElement)(e,{className:"margin-top--md"}):null}return(0,b.jsx)("div",{className:"margin-top--md",children:i.map(((e,n)=>(0,s.cloneElement)(e,{key:n,hidden:e.props.value!==t})))})}function y(e){const n=x(e);return(0,b.jsxs)("div",{className:(0,t.A)("tabs-container",g.tabList),children:[(0,b.jsx)(f,{...e,...n}),(0,b.jsx)(m,{...e,...n})]})}function I(e){const n=(0,j.A)();return(0,b.jsx)(y,{...e,children:u(e.children)},String(n))}},7470:(e,n,r)=>{r.d(n,{A:()=>a});var s=r(6540);const t={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var i=r(4848);function a(e){let{children:n}=e,r=s.Children.toArray(n).filter((e=>e));return(0,i.jsxs)("div",{className:t.codeDescContainer,children:[(0,i.jsx)("div",{className:t.desc,children:r[0]}),(0,i.jsx)("div",{className:t.example,children:r[1]})]})}},8453:(e,n,r)=>{r.d(n,{R:()=>a,x:()=>o});var s=r(6540);const t={},i=s.createContext(t);function a(e){const n=s.useContext(i);return s.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function o(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(t):e.components||t:a(e.components),s.createElement(i.Provider,{value:n},e.children)}}}]);