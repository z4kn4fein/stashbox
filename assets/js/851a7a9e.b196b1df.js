"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[153],{3905:(e,t,r)=>{r.d(t,{Zo:()=>d,kt:()=>h});var n=r(7294);function o(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function a(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function s(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?a(Object(r),!0).forEach((function(t){o(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):a(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function i(e,t){if(null==e)return{};var r,n,o=function(e,t){if(null==e)return{};var r,n,o={},a=Object.keys(e);for(n=0;n<a.length;n++)r=a[n],t.indexOf(r)>=0||(o[r]=e[r]);return o}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(n=0;n<a.length;n++)r=a[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(o[r]=e[r])}return o}var c=n.createContext({}),l=function(e){var t=n.useContext(c),r=t;return e&&(r="function"==typeof e?e(t):s(s({},t),e)),r},d=function(e){var t=l(e.components);return n.createElement(c.Provider,{value:t},e.children)},p="mdxType",u={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},v=n.forwardRef((function(e,t){var r=e.components,o=e.mdxType,a=e.originalType,c=e.parentName,d=i(e,["components","mdxType","originalType","parentName"]),p=l(r),v=o,h=p["".concat(c,".").concat(v)]||p[v]||u[v]||a;return r?n.createElement(h,s(s({ref:t},d),{},{components:r})):n.createElement(h,s({ref:t},d))}));function h(e,t){var r=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var a=r.length,s=new Array(a);s[0]=v;var i={};for(var c in t)hasOwnProperty.call(t,c)&&(i[c]=t[c]);i.originalType=e,i[p]="string"==typeof e?e:o,s[1]=i;for(var l=2;l<a;l++)s[l]=r[l];return n.createElement.apply(null,s)}return n.createElement.apply(null,r)}v.displayName="MDXCreateElement"},5162:(e,t,r)=>{r.d(t,{Z:()=>s});var n=r(7294),o=r(6010);const a="tabItem_Ymn6";function s(e){let{children:t,hidden:r,className:s}=e;return n.createElement("div",{role:"tabpanel",className:(0,o.Z)(a,s),hidden:r},t)}},4866:(e,t,r)=>{r.d(t,{Z:()=>w});var n=r(7462),o=r(7294),a=r(6010),s=r(2466),i=r(6550),c=r(1980),l=r(7392),d=r(12);function p(e){return function(e){return o.Children.map(e,(e=>{if(!e||(0,o.isValidElement)(e)&&function(e){const{props:t}=e;return!!t&&"object"==typeof t&&"value"in t}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}(e).map((e=>{let{props:{value:t,label:r,attributes:n,default:o}}=e;return{value:t,label:r,attributes:n,default:o}}))}function u(e){const{values:t,children:r}=e;return(0,o.useMemo)((()=>{const e=t??p(r);return function(e){const t=(0,l.l)(e,((e,t)=>e.value===t.value));if(t.length>0)throw new Error(`Docusaurus error: Duplicate values "${t.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[t,r])}function v(e){let{value:t,tabValues:r}=e;return r.some((e=>e.value===t))}function h(e){let{queryString:t=!1,groupId:r}=e;const n=(0,i.k6)(),a=function(e){let{queryString:t=!1,groupId:r}=e;if("string"==typeof t)return t;if(!1===t)return null;if(!0===t&&!r)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return r??null}({queryString:t,groupId:r});return[(0,c._X)(a),(0,o.useCallback)((e=>{if(!a)return;const t=new URLSearchParams(n.location.search);t.set(a,e),n.replace({...n.location,search:t.toString()})}),[a,n])]}function g(e){const{defaultValue:t,queryString:r=!1,groupId:n}=e,a=u(e),[s,i]=(0,o.useState)((()=>function(e){let{defaultValue:t,tabValues:r}=e;if(0===r.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(t){if(!v({value:t,tabValues:r}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${t}" but none of its children has the corresponding value. Available values are: ${r.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return t}const n=r.find((e=>e.default))??r[0];if(!n)throw new Error("Unexpected error: 0 tabValues");return n.value}({defaultValue:t,tabValues:a}))),[c,l]=h({queryString:r,groupId:n}),[p,g]=function(e){let{groupId:t}=e;const r=function(e){return e?`docusaurus.tab.${e}`:null}(t),[n,a]=(0,d.Nk)(r);return[n,(0,o.useCallback)((e=>{r&&a.set(e)}),[r,a])]}({groupId:n}),m=(()=>{const e=c??p;return v({value:e,tabValues:a})?e:null})();(0,o.useLayoutEffect)((()=>{m&&i(m)}),[m]);return{selectedValue:s,selectValue:(0,o.useCallback)((e=>{if(!v({value:e,tabValues:a}))throw new Error(`Can't select invalid tab value=${e}`);i(e),l(e),g(e)}),[l,g,a]),tabValues:a}}var m=r(2389);const f="tabList__CuJ",E="tabItem_LNqP";function P(e){let{className:t,block:r,selectedValue:i,selectValue:c,tabValues:l}=e;const d=[],{blockElementScrollPositionUntilNextRender:p}=(0,s.o5)(),u=e=>{const t=e.currentTarget,r=d.indexOf(t),n=l[r].value;n!==i&&(p(t),c(n))},v=e=>{let t=null;switch(e.key){case"Enter":u(e);break;case"ArrowRight":{const r=d.indexOf(e.currentTarget)+1;t=d[r]??d[0];break}case"ArrowLeft":{const r=d.indexOf(e.currentTarget)-1;t=d[r]??d[d.length-1];break}}t?.focus()};return o.createElement("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,a.Z)("tabs",{"tabs--block":r},t)},l.map((e=>{let{value:t,label:r,attributes:s}=e;return o.createElement("li",(0,n.Z)({role:"tab",tabIndex:i===t?0:-1,"aria-selected":i===t,key:t,ref:e=>d.push(e),onKeyDown:v,onClick:u},s,{className:(0,a.Z)("tabs__item",E,s?.className,{"tabs__item--active":i===t})}),r??t)})))}function b(e){let{lazy:t,children:r,selectedValue:n}=e;const a=(Array.isArray(r)?r:[r]).filter(Boolean);if(t){const e=a.find((e=>e.props.value===n));return e?(0,o.cloneElement)(e,{className:"margin-top--md"}):null}return o.createElement("div",{className:"margin-top--md"},a.map(((e,t)=>(0,o.cloneElement)(e,{key:t,hidden:e.props.value!==n}))))}function y(e){const t=g(e);return o.createElement("div",{className:(0,a.Z)("tabs-container",f)},o.createElement(P,(0,n.Z)({},e,t)),o.createElement(b,(0,n.Z)({},e,t)))}function w(e){const t=(0,m.Z)();return o.createElement(y,(0,n.Z)({key:String(t)},e))}},8846:(e,t,r)=>{r.d(t,{Z:()=>i});var n=r(7294);const o="codeDescContainer_ie8f",a="desc_jyqI",s="example_eYlF";function i(e){let{children:t}=e,r=n.Children.toArray(t).filter((e=>e));return n.createElement("div",{className:o},n.createElement("div",{className:a},r[0]),n.createElement("div",{className:s},r[1]))}},1212:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>p,contentTitle:()=>l,default:()=>h,frontMatter:()=>c,metadata:()=>d,toc:()=>u});var n=r(7462),o=(r(7294),r(3905)),a=r(8846),s=r(4866),i=r(5162);const c={},l="Decorators",d={unversionedId:"advanced/decorators",id:"advanced/decorators",title:"Decorators",description:"Stashbox supports decorator service registration to take advantage of the Decorator pattern. This pattern is used to extend the functionality of a class without changing its implementation. This is also what the Open\u2013closed principle stands for; services should be open for extension but closed for modification.",source:"@site/docs/advanced/decorators.md",sourceDirName:"advanced",slug:"/advanced/decorators",permalink:"/stashbox/docs/advanced/decorators",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/decorators.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1693855319,formattedLastUpdatedAt:"Sep 4, 2023",frontMatter:{},sidebar:"docs",previous:{title:"Generics",permalink:"/stashbox/docs/advanced/generics"},next:{title:"Wrappers & resolvers",permalink:"/stashbox/docs/advanced/wrappers-resolvers"}},p={},u=[{value:"Simple use-case",id:"simple-use-case",level:2},{value:"Multiple decorators",id:"multiple-decorators",level:2},{value:"Conditional decoration",id:"conditional-decoration",level:2},{value:"Generic decorators",id:"generic-decorators",level:2},{value:"Composite pattern",id:"composite-pattern",level:2},{value:"Decorating multiple services",id:"decorating-multiple-services",level:2},{value:"Lifetime",id:"lifetime",level:2},{value:"Wrappers",id:"wrappers",level:2},{value:"Interception",id:"interception",level:2}],v={toc:u};function h(e){let{components:t,...r}=e;return(0,o.kt)("wrapper",(0,n.Z)({},v,r,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"decorators"},"Decorators"),(0,o.kt)("p",null,"Stashbox supports decorator ",(0,o.kt)("a",{parentName:"p",href:"/docs/getting-started/glossary#service-registration--registered-service"},"service registration")," to take advantage of the ",(0,o.kt)("a",{parentName:"p",href:"https://en.wikipedia.org/wiki/Decorator_pattern"},"Decorator pattern"),". This pattern is used to extend the functionality of a class without changing its implementation. This is also what the ",(0,o.kt)("a",{parentName:"p",href:"https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle"},"Open\u2013closed principle")," stands for; services should be open for extension but closed for modification."),(0,o.kt)("h2",{id:"simple-use-case"},"Simple use-case"),(0,o.kt)("p",null,"We define an ",(0,o.kt)("inlineCode",{parentName:"p"},"IEventProcessor")," service used to process ",(0,o.kt)("inlineCode",{parentName:"p"},"Event")," entities. Then we'll decorate this service with additional validation capabilities:"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"class Event { }\nclass UpdateEvent : Event { }\n\ninterface IEventProcessor\n{\n    void ProcessEvent(Event @event);\n}\n\ninterface IEventValidator\n{\n    bool IsValid(Event @event);\n}\n\nclass EventValidator : IEventValidator\n{\n    public bool IsValid(Event @event) { /* do the actual validation. */ }\n}\n\nclass GeneralEventProcessor : IEventProcessor\n{\n    public void ProcessEvent(Event @event)\n    {\n        // suppose this method is processing the given event.\n        this.DoTheActualProcessing(@event);\n    }\n}\n\nclass ValidatorProcessor : IEventProcessor\n{\n    private readonly IEventProcessor nextProcessor;\n    private readonly IEventValidator eventValidator;\n    public ValidatorProcessor(IEventProcessor eventProcessor, IEventValidator eventValidator)\n    {\n        this.nextProcessor = eventProcessor;\n        this.eventValidator = eventValidator;\n    }\n\n    public void ProcessEvent(Event @event)\n    {\n        // validate the event first.\n        if (!this.eventValidator.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the next processor.\n        this.nextProcessor.ProcessEvent(@event);\n    }\n}\n\nusing var container = new StashboxContainer();\n\ncontainer.Register<IEventValidator, EventValidator>();\ncontainer.Register<IEventProcessor, GeneralEventProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>();\n\n// new ValidatorProcessor(new GeneralEventProcessor(), new EventValidator())\nvar eventProcessor = container.Resolve<IEventProcessor>();\n\n// process the event.\neventProcessor.ProcessEvent(new UpdateEvent());\n")),(0,o.kt)("p",null,"The ",(0,o.kt)("inlineCode",{parentName:"p"},"GeneralEventProcessor")," is an implementation of ",(0,o.kt)("inlineCode",{parentName:"p"},"IEventProcessor")," and does the actual event processing logic. It does not have any other responsibilities. Rather than putting the event validation's burden onto its shoulder, we create a different service for validation purposes. Instead of injecting the validator into the ",(0,o.kt)("inlineCode",{parentName:"p"},"GeneralEventProcessor")," directly, we let another ",(0,o.kt)("inlineCode",{parentName:"p"},"IEventProcessor")," decorate it like an ",(0,o.kt)("em",{parentName:"p"},"event processing pipeline")," that validates the event as a first step."),(0,o.kt)("h2",{id:"multiple-decorators"},"Multiple decorators"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"You have the option to register multiple decorators for a service to extend its functionality."),(0,o.kt)("p",null,"The decoration order will be the same as the registration order of the decorators. The first registered decorator will decorate the service itself. Then, all the subsequent decorators will wrap the already decorated service.")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IEventProcessor, GeneralProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>();\n\n// new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor()));\nvar processor = container.Resolve<IEventProcessor>(); \n")))),(0,o.kt)("h2",{id:"conditional-decoration"},"Conditional decoration"),(0,o.kt)("p",null,"With ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/service-resolution#conditional-resolution"},"conditional resolution")," you can control which decorator should be selected to decorate a given service."),(0,o.kt)(s.Z,{mdxType:"Tabs"},(0,o.kt)(i.Z,{value:"Decoretee",label:"Decoretee",mdxType:"TabItem"},(0,o.kt)("p",null,"You have the option to set which decorator should be selected for a given implementation. For a single type filter, you can use the ",(0,o.kt)("inlineCode",{parentName:"p"},".WhenDecoratedServiceIs()")," configuration option. To select more types, you can use the more generic ",(0,o.kt)("inlineCode",{parentName:"p"},".When()")," option. "),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IEventProcessor, GeneralProcessor>();\ncontainer.Register<IEventProcessor, CustomProcessor>();\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when CustomProcessor or GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs<CustomProcessor>()\n    .WhenDecoratedServiceIs<GeneralProcessor>());\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select only when GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs<GeneralProcessor>());\n\n// [\n//    new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())),\n//    new LoggerProcessor(new CustomProcessor())\n// ]\nvar processors = container.ResolveAll<IEventProcessor>();\n"))),(0,o.kt)(i.Z,{value:"Named",label:"Named",mdxType:"TabItem"},(0,o.kt)("p",null,"You can filter for service names to control the decorator selection."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'container.Register<IEventProcessor, GeneralProcessor>("General");\ncontainer.Register<IEventProcessor, CustomProcessor>("Custom");\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when CustomProcessor or GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs("General")\n    .WhenDecoratedServiceIs("Custom"));\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select only when GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs("General"));\n\n// new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor()))\nvar general = container.Resolve<IEventProcessor>("General");\n\n// new LoggerProcessor(new CustomProcessor())\nvar custom = container.Resolve<IEventProcessor>("Custom");\n'))),(0,o.kt)(i.Z,{value:"Attribute",label:"Attribute",mdxType:"TabItem"},(0,o.kt)("p",null,"You can use your custom attributes to control the decorator selection. With ",(0,o.kt)("strong",{parentName:"p"},"class attributes"),", you can mark your classes for decoration."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"class LogAttribute : Attribute { }\nclass ValidateAttribute : Attribute { }\n\n[Log, Validate]\nclass GeneralProcessor : IEventProcessor { }\n\n[Log]\nclass CustomProcessor : IEventProcessor { }\n\ncontainer.Register<IEventProcessor, GeneralProcessor>();\ncontainer.Register<IEventProcessor, CustomProcessor>();\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when the resolving class has 'LogAttribute'.\n    .WhenDecoratedServiceHas<LogAttribute>());\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select when the resolving class has 'ValidateAttribute'.\n    .WhenDecoratedServiceHas<ValidateAttribute>());\n\n// [\n//    new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())),\n//    new LoggerProcessor(new CustomProcessor())\n// ]\nvar processors = container.ResolveAll<IEventProcessor>();\n")),(0,o.kt)("p",null,"You can also mark your dependencies for decoration with ",(0,o.kt)("strong",{parentName:"p"},"property / field / parameter attributes"),". "),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"class LogAttribute : Attribute { }\nclass ValidateAttribute : Attribute { }\n\nclass ProcessorExecutor\n{\n    public ProcessorExecutor([Log, Validate]IEventProcessor eventProcessor)\n    { }\n}\n\ncontainer.Register<ProcessorExecutor>();\ncontainer.Register<IEventProcessor, GeneralProcessor>();\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when the resolving dependency has 'LogAttribute'.\n    .WhenHas<LogAttribute>());\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select when the resolving dependency has 'ValidateAttribute'.\n    .WhenHas<ValidateAttribute>());\n\n// new ProcessorExecutor(new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())))\nvar executor = container.ResolveAll<ProcessorExecutor>();\n")))),(0,o.kt)("h2",{id:"generic-decorators"},"Generic decorators"),(0,o.kt)("p",null,"Stashbox supports the registration of open-generic decorators, which allows the extension of open-generic services.\nInspection of ",(0,o.kt)("a",{parentName:"p",href:"/docs/advanced/generics#generic-constraints"},"generic parameter constraints")," and ",(0,o.kt)("a",{parentName:"p",href:"/docs/advanced/generics#variance"},"variance handling")," is supported on generic decorators also."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"interface IEventProcessor<TEvent>\n{\n    void ProcessEvent(TEvent @event);\n}\n\nclass GeneralEventProcessor<TEvent> : IEventProcessor<TEvent>\n{\n    public void ProcessEvent(TEvent @event) { /* suppose this method is processing the given event.*/ }\n}\n\nclass ValidatorProcessor<TEvent> : IEventProcessor<TEvent>\n{\n    private readonly IEventProcessor<TEvent> nextProcessor;\n\n    public ValidatorProcessor(IEventProcessor<TEvent> eventProcessor)\n    {\n        this.nextProcessor = eventProcessor;\n    }\n\n    public void ProcessEvent(TEvent @event)\n    {\n        // validate the event first.\n        if (!this.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the next processor.\n        this.nextProcessor.ProcessEvent(@event);\n    }\n}\n\nusing var container = new StashboxContainer();\ncontainer.Register(typeof(IEventProcessor<>), typeof(GeneralEventProcessor<>));\ncontainer.RegisterDecorator(typeof(IEventProcessor<>), typeof(ValidatorProcessor<>));\n\n// new ValidatorProcessor<UpdateEvent>(new GeneralEventProcessor<UpdateEvent>())\nvar eventProcessor = container.Resolve<IEventProcessor<UpdateEvent>>();\n\n// process the event.\neventProcessor.ProcessEvent(new UpdateEvent());\n")),(0,o.kt)("h2",{id:"composite-pattern"},"Composite pattern"),(0,o.kt)("p",null,"The ",(0,o.kt)("a",{parentName:"p",href:"https://en.wikipedia.org/wiki/Composite_pattern"},"Composite pattern")," allows a group of objects to be treated the same way as a single instance of the same type. It's useful when you want to use the functionality of multiple instances behind the same interface. You can achieve this by registering a decorator that takes a collection of the same service as a dependency. "),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"public class CompositeValidator<TEvent> : IEventValidator<TEvent>\n{\n    private readonly IEnumerable<IEventValidator<TEvent>> validators;\n\n    public CompositeValidator(IEnumerable<IEventValidator<TEvent>> validators)\n    {\n        this.validators = validators;\n    }\n\n    public bool IsValid(TEvent @event)\n    {\n        return this.validators.All(validator => validator.IsValid(@event));\n    }\n}\n\ncontainer.Register(typeof(IEventValidator<>), typeof(EventValidator<>));\ncontainer.Register(typeof(IEventValidator<>), typeof(AnotherEventValidator<>));\ncontainer.RegisterDecorator(typeof(IEventValidator<>), typeof(CompositeValidator<>));\n")),(0,o.kt)("h2",{id:"decorating-multiple-services"},"Decorating multiple services"),(0,o.kt)("p",null,"You have the option to organize similar decorating functionalities for different interfaces into the same decorator class.\nIn this example, we would like to validate a given ",(0,o.kt)("inlineCode",{parentName:"p"},"Event")," right before publishing and also before processing. "),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"public class EventValidator<TEvent> : IEventProcessor<T>, IEventPublisher<TEvent>\n{\n    private readonly IEventProcessor<TEvent> processor;\n    private readonly IEventPublisher<TEvent> publisher;\n    private readonly IEventValidator<TEvent> validator;\n\n    public CompositeValidator(IEventProcessor<TEvent> processor, \n        IEventPublisher<TEvent> publisher, \n        IEventValidator<TEvent> validator)\n    {\n        this.processor = processor;\n        this.publisher = publisher;\n        this.validator = validator;\n    }\n\n    public void ProcessEvent(TEvent @event)\n    {\n        // validate the event first.\n        if (!this.validator.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the processor.\n        this.processor.ProcessEvent(@event);\n    }\n\n    public void PublishEvent(TEvent @event)\n    {\n        // validate the event first.\n        if (!this.validator.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the publisher.\n        this.publisher.PublishEvent(@event);\n    }\n}\n\ncontainer.Register(typeof(IEventProcessor<>), typeof(EventProcessor<>));\ncontainer.Register(typeof(IEventPublisher<>), typeof(EventPublisher<>));\ncontainer.Register(typeof(IEventValidator<>), typeof(EventValidator<>));\n\n// without specifying the interface type, the container binds this registration to all of its implemented types\ncontainer.RegisterDecorator(typeof(EventValidator<>));\n")),(0,o.kt)("admonition",{type:"info"},(0,o.kt)("p",{parentName:"admonition"},"You can also use the ",(0,o.kt)("a",{parentName:"p",href:"/docs/guides/advanced-registration#binding-to-multiple-services"},"Binding to multiple services")," options.")),(0,o.kt)("h2",{id:"lifetime"},"Lifetime"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"Just as other registrations, decorators also can have their lifetime. It means, in addition to the service's lifetime, all decorator's lifetime will be applied to the wrapped service."),(0,o.kt)("admonition",{type:"note"},(0,o.kt)("p",{parentName:"admonition"},"When you don't set a decorator's lifetime, it'll implicitly inherit the decorated service's lifetime."))),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IEventProcessor, GeneralEventProcessor>();\n// singleton decorator will change the transient \n// decorated service's lifetime to singleton.\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => \n    options.WithLifetime(Lifetimes.Singleton));\n// Singleton[new ValidatorProcessor()](Transien[new GeneralEventProcessor()]) \nvar processor = container.Resolve<IEventProcessor>(); \n")))),(0,o.kt)("h2",{id:"wrappers"},"Wrappers"),(0,o.kt)(a.Z,{mdxType:"CodeDescPanel"},(0,o.kt)("div",null,(0,o.kt)("p",null,"Decorators are also applied to wrapped services. It means, in addition to the decoration, you can wrap your services in supported ",(0,o.kt)("a",{parentName:"p",href:"/docs/advanced/wrappers-resolvers#wrappers"},"wrappers"),".")),(0,o.kt)("div",null,(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},"container.Register<IEventProcessor, GeneralEventProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>();\n// () => new ValidatorProcessor(new GeneralEventProcessor())\nvar processor = container.Resolve<Func<IEventProcessor>>();\n")))),(0,o.kt)("h2",{id:"interception"},"Interception"),(0,o.kt)("p",null,"From the combination of Stashbox's decorator support and ",(0,o.kt)("a",{parentName:"p",href:"http://www.castleproject.org/projects/dynamicproxy/"},"Castle DynamicProxy's")," proxy generator, we can take advantage of the ",(0,o.kt)("a",{parentName:"p",href:"https://en.wikipedia.org/wiki/Aspect-oriented_programming"},"Aspect-Oriented Programming's")," benefits. The following example defines a ",(0,o.kt)("inlineCode",{parentName:"p"},"LoggingInterceptor")," that will log additional messages related to the called service methods."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-cs"},'public class LoggingInterceptor : IInterceptor \n{\n    private readonly ILogger logger;\n\n    public LoggingInterceptor(ILogger logger)\n    {\n        this.logger = logger;\n    }\n\n    public void Intercept(IInvocation invocation)\n    {\n        var stopwatch = new Stopwatch();\n        stopwatch.Start();\n\n        // log before we invoke the intercepted method.\n        this.logger.Log($"Method begin: {invocation.GetConcreteMethod().Name}");\n\n        // call the intercepted method.\n        invocation.Proceed();\n\n        // log after we invoked the intercepted method and print how long it ran.\n        this.logger.Log($"Method end: {invocation.GetConcreteMethod().Name}, execution duration: {stopwatch.ElapsedMiliseconds} ms");\n    }\n}\n\n// create a DefaultProxyBuilder from the DynamicProxy library.\nvar proxyBuilder = new DefaultProxyBuilder();\n\n// build a proxy for the IEventProcessor interface.\nvar eventProcessorProxy = proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(\n    typeof(IEventProcessor), \n    new Type[0], \n    ProxyGenerationOptions.Default);\n\n// register the logger for LoggingInterceptor.\ncontainer.Register<ILogger, ConsoleLogger>();\n\n// register the service that we will intercept.\ncontainer.Register<IEventProcessor, GeneralEventProcessor>();\n\n// register the interceptor.\ncontainer.Register<IInterceptor, LoggingInterceptor>();\n\n// register the built proxy as a decorator.\ncontainer.RegisterDecorator<IEventProcessor>(eventProcessorProxy);\n')))}h.isMDXComponent=!0}}]);