"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[153],{8946:(e,n,r)=>{r.r(n),r.d(n,{assets:()=>v,contentTitle:()=>l,default:()=>p,frontMatter:()=>c,metadata:()=>d,toc:()=>h});var t=r(5893),o=r(1151),s=r(8846),a=r(4866),i=r(5162);const c={},l="Decorators",d={id:"advanced/decorators",title:"Decorators",description:"Stashbox supports decorator service registration to take advantage of the Decorator pattern. This pattern is used to extend the functionality of a class without changing its implementation. This is also what the Open\u2013closed principle stands for; services should be open for extension but closed for modification.",source:"@site/docs/advanced/decorators.md",sourceDirName:"advanced",slug:"/advanced/decorators",permalink:"/stashbox/docs/advanced/decorators",draft:!1,unlisted:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/advanced/decorators.md",tags:[],version:"current",lastUpdatedBy:"Peter Csajtai",lastUpdatedAt:1712573937,formattedLastUpdatedAt:"Apr 8, 2024",frontMatter:{},sidebar:"docs",previous:{title:"Generics",permalink:"/stashbox/docs/advanced/generics"},next:{title:"Wrappers & resolvers",permalink:"/stashbox/docs/advanced/wrappers-resolvers"}},v={},h=[{value:"Simple use-case",id:"simple-use-case",level:2},{value:"Multiple decorators",id:"multiple-decorators",level:2},{value:"Conditional decoration",id:"conditional-decoration",level:2},{value:"Generic decorators",id:"generic-decorators",level:2},{value:"Composite pattern",id:"composite-pattern",level:2},{value:"Decorating multiple services",id:"decorating-multiple-services",level:2},{value:"Lifetime",id:"lifetime",level:2},{value:"Wrappers",id:"wrappers",level:2},{value:"Interception",id:"interception",level:2}];function u(e){const n={a:"a",admonition:"admonition",code:"code",em:"em",h1:"h1",h2:"h2",p:"p",pre:"pre",strong:"strong",...(0,o.a)(),...e.components};return(0,t.jsxs)(t.Fragment,{children:[(0,t.jsx)(n.h1,{id:"decorators",children:"Decorators"}),"\n",(0,t.jsxs)(n.p,{children:["Stashbox supports decorator ",(0,t.jsx)(n.a,{href:"/docs/getting-started/glossary#service-registration--registered-service",children:"service registration"})," to take advantage of the ",(0,t.jsx)(n.a,{href:"https://en.wikipedia.org/wiki/Decorator_pattern",children:"Decorator pattern"}),". This pattern is used to extend the functionality of a class without changing its implementation. This is also what the ",(0,t.jsx)(n.a,{href:"https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle",children:"Open\u2013closed principle"})," stands for; services should be open for extension but closed for modification."]}),"\n",(0,t.jsx)(n.h2,{id:"simple-use-case",children:"Simple use-case"}),"\n",(0,t.jsxs)(n.p,{children:["We define an ",(0,t.jsx)(n.code,{children:"IEventProcessor"})," service used to process ",(0,t.jsx)(n.code,{children:"Event"})," entities. Then we'll decorate this service with additional validation capabilities:"]}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"class Event { }\nclass UpdateEvent : Event { }\n\ninterface IEventProcessor\n{\n    void ProcessEvent(Event @event);\n}\n\ninterface IEventValidator\n{\n    bool IsValid(Event @event);\n}\n\nclass EventValidator : IEventValidator\n{\n    public bool IsValid(Event @event) { /* do the actual validation. */ }\n}\n\nclass GeneralEventProcessor : IEventProcessor\n{\n    public void ProcessEvent(Event @event)\n    {\n        // suppose this method is processing the given event.\n        this.DoTheActualProcessing(@event);\n    }\n}\n\nclass ValidatorProcessor : IEventProcessor\n{\n    private readonly IEventProcessor nextProcessor;\n    private readonly IEventValidator eventValidator;\n    public ValidatorProcessor(IEventProcessor eventProcessor, IEventValidator eventValidator)\n    {\n        this.nextProcessor = eventProcessor;\n        this.eventValidator = eventValidator;\n    }\n\n    public void ProcessEvent(Event @event)\n    {\n        // validate the event first.\n        if (!this.eventValidator.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the next processor.\n        this.nextProcessor.ProcessEvent(@event);\n    }\n}\n\nusing var container = new StashboxContainer();\n\ncontainer.Register<IEventValidator, EventValidator>();\ncontainer.Register<IEventProcessor, GeneralEventProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>();\n\n// new ValidatorProcessor(new GeneralEventProcessor(), new EventValidator())\nvar eventProcessor = container.Resolve<IEventProcessor>();\n\n// process the event.\neventProcessor.ProcessEvent(new UpdateEvent());\n"})}),"\n",(0,t.jsxs)(n.p,{children:["The ",(0,t.jsx)(n.code,{children:"GeneralEventProcessor"})," is an implementation of ",(0,t.jsx)(n.code,{children:"IEventProcessor"})," and does the actual event processing logic. It does not have any other responsibilities. Rather than putting the event validation's burden onto its shoulder, we create a different service for validation purposes. Instead of injecting the validator into the ",(0,t.jsx)(n.code,{children:"GeneralEventProcessor"})," directly, we let another ",(0,t.jsx)(n.code,{children:"IEventProcessor"})," decorate it like an ",(0,t.jsx)(n.em,{children:"event processing pipeline"})," that validates the event as a first step."]}),"\n",(0,t.jsx)(n.h2,{id:"multiple-decorators",children:"Multiple decorators"}),"\n",(0,t.jsxs)(s.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"You have the option to register multiple decorators for a service to extend its functionality."}),(0,t.jsx)(n.p,{children:"The decoration order will be the same as the registration order of the decorators. The first registered decorator will decorate the service itself. Then, all the subsequent decorators will wrap the already decorated service."})]}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IEventProcessor, GeneralProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>();\n\n// new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor()));\nvar processor = container.Resolve<IEventProcessor>(); \n"})})})]}),"\n",(0,t.jsx)(n.h2,{id:"conditional-decoration",children:"Conditional decoration"}),"\n",(0,t.jsxs)(n.p,{children:["With ",(0,t.jsx)(n.a,{href:"/docs/guides/service-resolution#conditional-resolution",children:"conditional resolution"})," you can control which decorator should be selected to decorate a given service."]}),"\n",(0,t.jsxs)(a.Z,{children:[(0,t.jsxs)(i.Z,{value:"Decoretee",label:"Decoretee",children:[(0,t.jsxs)(n.p,{children:["You have the option to set which decorator should be selected for a given implementation. For a single type filter, you can use the ",(0,t.jsx)(n.code,{children:".WhenDecoratedServiceIs()"})," configuration option. To select more types, you can use the more generic ",(0,t.jsx)(n.code,{children:".When()"})," option."]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IEventProcessor, GeneralProcessor>();\ncontainer.Register<IEventProcessor, CustomProcessor>();\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when CustomProcessor or GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs<CustomProcessor>()\n    .WhenDecoratedServiceIs<GeneralProcessor>());\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select only when GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs<GeneralProcessor>());\n\n// [\n//    new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())),\n//    new LoggerProcessor(new CustomProcessor())\n// ]\nvar processors = container.ResolveAll<IEventProcessor>();\n"})})]}),(0,t.jsxs)(i.Z,{value:"Named",label:"Named",children:[(0,t.jsx)(n.p,{children:"You can filter for service names to control the decorator selection."}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'container.Register<IEventProcessor, GeneralProcessor>("General");\ncontainer.Register<IEventProcessor, CustomProcessor>("Custom");\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when CustomProcessor or GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs("General")\n    .WhenDecoratedServiceIs("Custom"));\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select only when GeneralProcessor is resolved.\n    .WhenDecoratedServiceIs("General"));\n\n// new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor()))\nvar general = container.Resolve<IEventProcessor>("General");\n\n// new LoggerProcessor(new CustomProcessor())\nvar custom = container.Resolve<IEventProcessor>("Custom");\n'})})]}),(0,t.jsxs)(i.Z,{value:"Attribute",label:"Attribute",children:[(0,t.jsxs)(n.p,{children:["You can use your custom attributes to control the decorator selection. With ",(0,t.jsx)(n.strong,{children:"class attributes"}),", you can mark your classes for decoration."]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"class LogAttribute : Attribute { }\nclass ValidateAttribute : Attribute { }\n\n[Log, Validate]\nclass GeneralProcessor : IEventProcessor { }\n\n[Log]\nclass CustomProcessor : IEventProcessor { }\n\ncontainer.Register<IEventProcessor, GeneralProcessor>();\ncontainer.Register<IEventProcessor, CustomProcessor>();\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when the resolving class has 'LogAttribute'.\n    .WhenDecoratedServiceHas<LogAttribute>());\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select when the resolving class has 'ValidateAttribute'.\n    .WhenDecoratedServiceHas<ValidateAttribute>());\n\n// [\n//    new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())),\n//    new LoggerProcessor(new CustomProcessor())\n// ]\nvar processors = container.ResolveAll<IEventProcessor>();\n"})}),(0,t.jsxs)(n.p,{children:["You can also mark your dependencies for decoration with ",(0,t.jsx)(n.strong,{children:"property / field / parameter attributes"}),"."]}),(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"class LogAttribute : Attribute { }\nclass ValidateAttribute : Attribute { }\n\nclass ProcessorExecutor\n{\n    public ProcessorExecutor([Log, Validate]IEventProcessor eventProcessor)\n    { }\n}\n\ncontainer.Register<ProcessorExecutor>();\ncontainer.Register<IEventProcessor, GeneralProcessor>();\n\ncontainer.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options\n    // select when the resolving dependency has 'LogAttribute'.\n    .WhenHas<LogAttribute>());\n\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options\n    // select when the resolving dependency has 'ValidateAttribute'.\n    .WhenHas<ValidateAttribute>());\n\n// new ProcessorExecutor(new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())))\nvar executor = container.ResolveAll<ProcessorExecutor>();\n"})})]})]}),"\n",(0,t.jsx)(n.h2,{id:"generic-decorators",children:"Generic decorators"}),"\n",(0,t.jsxs)(n.p,{children:["Stashbox supports the registration of open-generic decorators, which allows the extension of open-generic services.\nInspection of ",(0,t.jsx)(n.a,{href:"/docs/advanced/generics#generic-constraints",children:"generic parameter constraints"})," and ",(0,t.jsx)(n.a,{href:"/docs/advanced/generics#variance",children:"variance handling"})," is supported on generic decorators also."]}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"interface IEventProcessor<TEvent>\n{\n    void ProcessEvent(TEvent @event);\n}\n\nclass GeneralEventProcessor<TEvent> : IEventProcessor<TEvent>\n{\n    public void ProcessEvent(TEvent @event) { /* suppose this method is processing the given event.*/ }\n}\n\nclass ValidatorProcessor<TEvent> : IEventProcessor<TEvent>\n{\n    private readonly IEventProcessor<TEvent> nextProcessor;\n\n    public ValidatorProcessor(IEventProcessor<TEvent> eventProcessor)\n    {\n        this.nextProcessor = eventProcessor;\n    }\n\n    public void ProcessEvent(TEvent @event)\n    {\n        // validate the event first.\n        if (!this.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the next processor.\n        this.nextProcessor.ProcessEvent(@event);\n    }\n}\n\nusing var container = new StashboxContainer();\ncontainer.Register(typeof(IEventProcessor<>), typeof(GeneralEventProcessor<>));\ncontainer.RegisterDecorator(typeof(IEventProcessor<>), typeof(ValidatorProcessor<>));\n\n// new ValidatorProcessor<UpdateEvent>(new GeneralEventProcessor<UpdateEvent>())\nvar eventProcessor = container.Resolve<IEventProcessor<UpdateEvent>>();\n\n// process the event.\neventProcessor.ProcessEvent(new UpdateEvent());\n"})}),"\n",(0,t.jsx)(n.h2,{id:"composite-pattern",children:"Composite pattern"}),"\n",(0,t.jsxs)(n.p,{children:["The ",(0,t.jsx)(n.a,{href:"https://en.wikipedia.org/wiki/Composite_pattern",children:"Composite pattern"})," allows a group of objects to be treated the same way as a single instance of the same type. It's useful when you want to use the functionality of multiple instances behind the same interface. You can achieve this by registering a decorator that takes a collection of the same service as a dependency."]}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"public class CompositeValidator<TEvent> : IEventValidator<TEvent>\n{\n    private readonly IEnumerable<IEventValidator<TEvent>> validators;\n\n    public CompositeValidator(IEnumerable<IEventValidator<TEvent>> validators)\n    {\n        this.validators = validators;\n    }\n\n    public bool IsValid(TEvent @event)\n    {\n        return this.validators.All(validator => validator.IsValid(@event));\n    }\n}\n\ncontainer.Register(typeof(IEventValidator<>), typeof(EventValidator<>));\ncontainer.Register(typeof(IEventValidator<>), typeof(AnotherEventValidator<>));\ncontainer.RegisterDecorator(typeof(IEventValidator<>), typeof(CompositeValidator<>));\n"})}),"\n",(0,t.jsx)(n.h2,{id:"decorating-multiple-services",children:"Decorating multiple services"}),"\n",(0,t.jsxs)(n.p,{children:["You have the option to organize similar decorating functionalities for different interfaces into the same decorator class.\nIn this example, we would like to validate a given ",(0,t.jsx)(n.code,{children:"Event"})," right before publishing and also before processing."]}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"public class EventValidator<TEvent> : IEventProcessor<T>, IEventPublisher<TEvent>\n{\n    private readonly IEventProcessor<TEvent> processor;\n    private readonly IEventPublisher<TEvent> publisher;\n    private readonly IEventValidator<TEvent> validator;\n\n    public CompositeValidator(IEventProcessor<TEvent> processor, \n        IEventPublisher<TEvent> publisher, \n        IEventValidator<TEvent> validator)\n    {\n        this.processor = processor;\n        this.publisher = publisher;\n        this.validator = validator;\n    }\n\n    public void ProcessEvent(TEvent @event)\n    {\n        // validate the event first.\n        if (!this.validator.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the processor.\n        this.processor.ProcessEvent(@event);\n    }\n\n    public void PublishEvent(TEvent @event)\n    {\n        // validate the event first.\n        if (!this.validator.IsValid(@event))\n            throw new InvalidEventException();\n\n        // if everything is ok, call the publisher.\n        this.publisher.PublishEvent(@event);\n    }\n}\n\ncontainer.Register(typeof(IEventProcessor<>), typeof(EventProcessor<>));\ncontainer.Register(typeof(IEventPublisher<>), typeof(EventPublisher<>));\ncontainer.Register(typeof(IEventValidator<>), typeof(EventValidator<>));\n\n// without specifying the interface type, the container binds this registration to all of its implemented types\ncontainer.RegisterDecorator(typeof(EventValidator<>));\n"})}),"\n",(0,t.jsx)(n.admonition,{type:"info",children:(0,t.jsxs)(n.p,{children:["You can also use the ",(0,t.jsx)(n.a,{href:"/docs/guides/advanced-registration#binding-to-multiple-services",children:"Binding to multiple services"})," options."]})}),"\n",(0,t.jsx)(n.h2,{id:"lifetime",children:"Lifetime"}),"\n",(0,t.jsxs)(s.Z,{children:[(0,t.jsxs)("div",{children:[(0,t.jsx)(n.p,{children:"Just as other registrations, decorators also can have their lifetime. It means, in addition to the service's lifetime, all decorator's lifetime will be applied to the wrapped service."}),(0,t.jsx)(n.admonition,{type:"note",children:(0,t.jsx)(n.p,{children:"When you don't set a decorator's lifetime, it'll implicitly inherit the decorated service's lifetime."})})]}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IEventProcessor, GeneralEventProcessor>();\n// singleton decorator will change the transient \n// decorated service's lifetime to singleton.\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => \n    options.WithLifetime(Lifetimes.Singleton));\n// Singleton[new ValidatorProcessor()](Transien[new GeneralEventProcessor()]) \nvar processor = container.Resolve<IEventProcessor>(); \n"})})})]}),"\n",(0,t.jsx)(n.h2,{id:"wrappers",children:"Wrappers"}),"\n",(0,t.jsxs)(s.Z,{children:[(0,t.jsx)("div",{children:(0,t.jsxs)(n.p,{children:["Decorators are also applied to wrapped services. It means, in addition to the decoration, you can wrap your services in supported ",(0,t.jsx)(n.a,{href:"/docs/advanced/wrappers-resolvers#wrappers",children:"wrappers"}),"."]})}),(0,t.jsx)("div",{children:(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:"container.Register<IEventProcessor, GeneralEventProcessor>();\ncontainer.RegisterDecorator<IEventProcessor, ValidatorProcessor>();\n// () => new ValidatorProcessor(new GeneralEventProcessor())\nvar processor = container.Resolve<Func<IEventProcessor>>();\n"})})})]}),"\n",(0,t.jsx)(n.h2,{id:"interception",children:"Interception"}),"\n",(0,t.jsxs)(n.p,{children:["From the combination of Stashbox's decorator support and ",(0,t.jsx)(n.a,{href:"http://www.castleproject.org/projects/dynamicproxy/",children:"Castle DynamicProxy's"})," proxy generator, we can take advantage of the ",(0,t.jsx)(n.a,{href:"https://en.wikipedia.org/wiki/Aspect-oriented_programming",children:"Aspect-Oriented Programming's"})," benefits. The following example defines a ",(0,t.jsx)(n.code,{children:"LoggingInterceptor"})," that will log additional messages related to the called service methods."]}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-cs",children:'public class LoggingInterceptor : IInterceptor \n{\n    private readonly ILogger logger;\n\n    public LoggingInterceptor(ILogger logger)\n    {\n        this.logger = logger;\n    }\n\n    public void Intercept(IInvocation invocation)\n    {\n        var stopwatch = new Stopwatch();\n        stopwatch.Start();\n\n        // log before we invoke the intercepted method.\n        this.logger.Log($"Method begin: {invocation.GetConcreteMethod().Name}");\n\n        // call the intercepted method.\n        invocation.Proceed();\n\n        // log after we invoked the intercepted method and print how long it ran.\n        this.logger.Log($"Method end: {invocation.GetConcreteMethod().Name}, execution duration: {stopwatch.ElapsedMiliseconds} ms");\n    }\n}\n\n// create a DefaultProxyBuilder from the DynamicProxy library.\nvar proxyBuilder = new DefaultProxyBuilder();\n\n// build a proxy for the IEventProcessor interface.\nvar eventProcessorProxy = proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(\n    typeof(IEventProcessor), \n    new Type[0], \n    ProxyGenerationOptions.Default);\n\n// register the logger for LoggingInterceptor.\ncontainer.Register<ILogger, ConsoleLogger>();\n\n// register the service that we will intercept.\ncontainer.Register<IEventProcessor, GeneralEventProcessor>();\n\n// register the interceptor.\ncontainer.Register<IInterceptor, LoggingInterceptor>();\n\n// register the built proxy as a decorator.\ncontainer.RegisterDecorator<IEventProcessor>(eventProcessorProxy);\n'})})]})}function p(e={}){const{wrapper:n}={...(0,o.a)(),...e.components};return n?(0,t.jsx)(n,{...e,children:(0,t.jsx)(u,{...e})}):u(e)}},5162:(e,n,r)=>{r.d(n,{Z:()=>a});r(7294);var t=r(4334);const o={tabItem:"tabItem_Ymn6"};var s=r(5893);function a(e){let{children:n,hidden:r,className:a}=e;return(0,s.jsx)("div",{role:"tabpanel",className:(0,t.Z)(o.tabItem,a),hidden:r,children:n})}},4866:(e,n,r)=>{r.d(n,{Z:()=>I});var t=r(7294),o=r(4334),s=r(2466),a=r(6550),i=r(469),c=r(1980),l=r(7392),d=r(12);function v(e){return t.Children.toArray(e).filter((e=>"\n"!==e)).map((e=>{if(!e||(0,t.isValidElement)(e)&&function(e){const{props:n}=e;return!!n&&"object"==typeof n&&"value"in n}(e))return e;throw new Error(`Docusaurus error: Bad <Tabs> child <${"string"==typeof e.type?e.type:e.type.name}>: all children of the <Tabs> component should be <TabItem>, and every <TabItem> should have a unique "value" prop.`)}))?.filter(Boolean)??[]}function h(e){const{values:n,children:r}=e;return(0,t.useMemo)((()=>{const e=n??function(e){return v(e).map((e=>{let{props:{value:n,label:r,attributes:t,default:o}}=e;return{value:n,label:r,attributes:t,default:o}}))}(r);return function(e){const n=(0,l.l)(e,((e,n)=>e.value===n.value));if(n.length>0)throw new Error(`Docusaurus error: Duplicate values "${n.map((e=>e.value)).join(", ")}" found in <Tabs>. Every value needs to be unique.`)}(e),e}),[n,r])}function u(e){let{value:n,tabValues:r}=e;return r.some((e=>e.value===n))}function p(e){let{queryString:n=!1,groupId:r}=e;const o=(0,a.k6)(),s=function(e){let{queryString:n=!1,groupId:r}=e;if("string"==typeof n)return n;if(!1===n)return null;if(!0===n&&!r)throw new Error('Docusaurus error: The <Tabs> component groupId prop is required if queryString=true, because this value is used as the search param name. You can also provide an explicit value such as queryString="my-search-param".');return r??null}({queryString:n,groupId:r});return[(0,c._X)(s),(0,t.useCallback)((e=>{if(!s)return;const n=new URLSearchParams(o.location.search);n.set(s,e),o.replace({...o.location,search:n.toString()})}),[s,o])]}function g(e){const{defaultValue:n,queryString:r=!1,groupId:o}=e,s=h(e),[a,c]=(0,t.useState)((()=>function(e){let{defaultValue:n,tabValues:r}=e;if(0===r.length)throw new Error("Docusaurus error: the <Tabs> component requires at least one <TabItem> children component");if(n){if(!u({value:n,tabValues:r}))throw new Error(`Docusaurus error: The <Tabs> has a defaultValue "${n}" but none of its children has the corresponding value. Available values are: ${r.map((e=>e.value)).join(", ")}. If you intend to show no default tab, use defaultValue={null} instead.`);return n}const t=r.find((e=>e.default))??r[0];if(!t)throw new Error("Unexpected error: 0 tabValues");return t.value}({defaultValue:n,tabValues:s}))),[l,v]=p({queryString:r,groupId:o}),[g,f]=function(e){let{groupId:n}=e;const r=function(e){return e?`docusaurus.tab.${e}`:null}(n),[o,s]=(0,d.Nk)(r);return[o,(0,t.useCallback)((e=>{r&&s.set(e)}),[r,s])]}({groupId:o}),m=(()=>{const e=l??g;return u({value:e,tabValues:s})?e:null})();(0,i.Z)((()=>{m&&c(m)}),[m]);return{selectedValue:a,selectValue:(0,t.useCallback)((e=>{if(!u({value:e,tabValues:s}))throw new Error(`Can't select invalid tab value=${e}`);c(e),v(e),f(e)}),[v,f,s]),tabValues:s}}var f=r(2389);const m={tabList:"tabList__CuJ",tabItem:"tabItem_LNqP"};var P=r(5893);function E(e){let{className:n,block:r,selectedValue:t,selectValue:a,tabValues:i}=e;const c=[],{blockElementScrollPositionUntilNextRender:l}=(0,s.o5)(),d=e=>{const n=e.currentTarget,r=c.indexOf(n),o=i[r].value;o!==t&&(l(n),a(o))},v=e=>{let n=null;switch(e.key){case"Enter":d(e);break;case"ArrowRight":{const r=c.indexOf(e.currentTarget)+1;n=c[r]??c[0];break}case"ArrowLeft":{const r=c.indexOf(e.currentTarget)-1;n=c[r]??c[c.length-1];break}}n?.focus()};return(0,P.jsx)("ul",{role:"tablist","aria-orientation":"horizontal",className:(0,o.Z)("tabs",{"tabs--block":r},n),children:i.map((e=>{let{value:n,label:r,attributes:s}=e;return(0,P.jsx)("li",{role:"tab",tabIndex:t===n?0:-1,"aria-selected":t===n,ref:e=>c.push(e),onKeyDown:v,onClick:d,...s,className:(0,o.Z)("tabs__item",m.tabItem,s?.className,{"tabs__item--active":t===n}),children:r??n},n)}))})}function x(e){let{lazy:n,children:r,selectedValue:o}=e;const s=(Array.isArray(r)?r:[r]).filter(Boolean);if(n){const e=s.find((e=>e.props.value===o));return e?(0,t.cloneElement)(e,{className:"margin-top--md"}):null}return(0,P.jsx)("div",{className:"margin-top--md",children:s.map(((e,n)=>(0,t.cloneElement)(e,{key:n,hidden:e.props.value!==o})))})}function b(e){const n=g(e);return(0,P.jsxs)("div",{className:(0,o.Z)("tabs-container",m.tabList),children:[(0,P.jsx)(E,{...e,...n}),(0,P.jsx)(x,{...e,...n})]})}function I(e){const n=(0,f.Z)();return(0,P.jsx)(b,{...e,children:v(e.children)},String(n))}},8846:(e,n,r)=>{r.d(n,{Z:()=>a});var t=r(7294);const o={codeDescContainer:"codeDescContainer_ie8f",desc:"desc_jyqI",example:"example_eYlF"};var s=r(5893);function a(e){let{children:n}=e,r=t.Children.toArray(n).filter((e=>e));return(0,s.jsxs)("div",{className:o.codeDescContainer,children:[(0,s.jsx)("div",{className:o.desc,children:r[0]}),(0,s.jsx)("div",{className:o.example,children:r[1]})]})}},1151:(e,n,r)=>{r.d(n,{Z:()=>i,a:()=>a});var t=r(7294);const o={},s=t.createContext(o);function a(e){const n=t.useContext(s);return t.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function i(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(o):e.components||o:a(e.components),t.createElement(s.Provider,{value:n},e.children)}}}]);