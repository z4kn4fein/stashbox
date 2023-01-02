import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Decorators
Stashbox supports decorator [service registration](/docs/getting-started/glossary#service-registration--registered-service) to take advantage of the [Decorator pattern](https://en.wikipedia.org/wiki/Decorator_pattern). This pattern is used to extend the functionality of a class without changing its implementation. This is also what the [Open–closed principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle) stands for; services should be open for extension but closed for modification.

## Simple use-case
We define an `IEventProcessor` service used to process `Event` entities. Then we'll decorate this service with additional validation capabilities:
```cs
class Event { }
class UpdateEvent : Event { }

interface IEventProcessor
{
    void ProcessEvent(Event @event);
}

interface IEventValidator
{
    bool IsValid(Event @event);
}

class EventValidator : IEventValidator
{
    public bool IsValid(Event @event) { /* do the actual validation. */ }
}

class GeneralEventProcessor : IEventProcessor
{
    public void ProcessEvent(Event @event)
    {
        // suppose this method is processing the given event.
        this.DoTheActualProcessing(@event);
    }
}

class ValidatorProcessor : IEventProcessor
{
    private readonly IEventProcessor nextProcessor;
    private readonly IEventValidator eventValidator;
    public ValidatorProcessor(IEventProcessor eventProcessor, IEventValidator eventValidator)
    {
        this.nextProcessor = eventProcessor;
        this.eventValidator = eventValidator;
    }

    public void ProcessEvent(Event @event)
    {
        // validate the event first.
        if (!this.eventValidator.IsValid(@event))
            throw new InvalidEventException();

        // if everything is ok, call the next processor.
        this.nextProcessor.ProcessEvent(@event);
    }
}

using var container = new StashboxContainer();

container.Register<IEventValidator, EventValidator>();
container.Register<IEventProcessor, GeneralEventProcessor>();
container.RegisterDecorator<IEventProcessor, ValidatorProcessor>();

// new ValidatorProcessor(new GeneralEventProcessor(), new EventValidator())
var eventProcessor = container.Resolve<IEventProcessor>();

// process the event.
eventProcessor.ProcessEvent(new UpdateEvent());
```
The `GeneralEventProcessor` is an implementation of `IEventProcessor` and does the actual event processing logic. It does not have any other responsibilities. Rather than putting the event validation's burden onto its shoulder, we create a different service for validation purposes. Instead of injecting the validator into the `GeneralEventProcessor` directly, we let another `IEventProcessor` decorate it like an *event processing pipeline* that validates the event as a first step.

## Multiple decorators

<CodeDescPanel>
<div>

You have the option to register multiple decorators for a service to extend its functionality.

The decoration order will be the same as the registration order of the decorators. The first registered decorator will decorate the service itself. Then, all the subsequent decorators will wrap the already decorated service.

</div>
<div>

```cs
container.Register<IEventProcessor, GeneralProcessor>();
container.RegisterDecorator<IEventProcessor, LoggerProcessor>();
container.RegisterDecorator<IEventProcessor, ValidatorProcessor>();

// new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor()));
var processor = container.Resolve<IEventProcessor>(); 
```

</div>
</CodeDescPanel>

## Conditional decoration

With [conditional resolution](/docs/guides/service-resolution#conditional-resolution) you can control which decorator should be selected to decorate a given service.

<Tabs>
<TabItem value="Decoretee" label="Decoretee">

You have the option to set which decorator should be selected for a given implementation. For a single type filter, you can use the `.WhenDecoratedServiceIs()` configuration option. To select more types, you can use the more generic `.When()` option. 

```cs
container.Register<IEventProcessor, GeneralProcessor>();
container.Register<IEventProcessor, CustomProcessor>();

container.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options
    // select when CustomProcessor or GeneralProcessor is resolved.
    .WhenDecoratedServiceIs<CustomProcessor>()
    .WhenDecoratedServiceIs<GeneralProcessor>());

container.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options
    // select only when GeneralProcessor is resolved.
    .WhenDecoratedServiceIs<GeneralProcessor>());

// [
//    new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())),
//    new LoggerProcessor(new CustomProcessor())
// ]
var processors = container.ResolveAll<IEventProcessor>();
```

</TabItem>
<TabItem value="Named" label="Named">

You can filter for service names to control the decorator selection.
```cs
container.Register<IEventProcessor, GeneralProcessor>("General");
container.Register<IEventProcessor, CustomProcessor>("Custom");

container.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options
    // select when CustomProcessor or GeneralProcessor is resolved.
    .WhenDecoratedServiceIs("General")
    .WhenDecoratedServiceIs("Custom"));

container.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options
    // select only when GeneralProcessor is resolved.
    .WhenDecoratedServiceIs("General"));

// new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor()))
var general = container.Resolve<IEventProcessor>("General");

// new LoggerProcessor(new CustomProcessor())
var custom = container.Resolve<IEventProcessor>("Custom");
```

</TabItem>
<TabItem value="Attribute" label="Attribute">

You can use your custom attributes to control the decorator selection. With **class attributes**, you can mark your classes for decoration.
```cs
class LogAttribute : Attribute { }
class ValidateAttribute : Attribute { }

[Log, Validate]
class GeneralProcessor : IEventProcessor { }

[Log]
class CustomProcessor : IEventProcessor { }

container.Register<IEventProcessor, GeneralProcessor>();
container.Register<IEventProcessor, CustomProcessor>();

container.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options
    // select when the resolving class has 'LogAttribute'.
    .WhenDecoratedServiceHas<LogAttribute>());

container.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options
    // select when the resolving class has 'ValidateAttribute'.
    .WhenDecoratedServiceHas<ValidateAttribute>());

// [
//    new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())),
//    new LoggerProcessor(new CustomProcessor())
// ]
var processors = container.ResolveAll<IEventProcessor>();
```

You can also mark your dependencies for decoration with **property / field / parameter attributes**. 

```cs
class LogAttribute : Attribute { }
class ValidateAttribute : Attribute { }

class ProcessorExecutor
{
    public ProcessorExecutor([Log, Validate]IEventProcessor eventProcessor)
    { }
}

container.Register<ProcessorExecutor>();
container.Register<IEventProcessor, GeneralProcessor>();

container.RegisterDecorator<IEventProcessor, LoggerProcessor>(options => options
    // select when the resolving dependency has 'LogAttribute'.
    .WhenHas<LogAttribute>());

container.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => options
    // select when the resolving dependency has 'ValidateAttribute'.
    .WhenHas<ValidateAttribute>());

// new ProcessorExecutor(new ValidatorProcessor(new LoggerProcessor(new GeneralProcessor())))
var executor = container.ResolveAll<ProcessorExecutor>();
```

</TabItem>
</Tabs>

## Generic decorators
Stashbox supports the registration of open-generic decorators, which allows the extension of open-generic services. 
Inspection of [generic parameter constraints](/docs/advanced/generics#generic-constraints) and [variance handling](/docs/advanced/generics#variance) is supported on generic decorators also.

```cs
interface IEventProcessor<TEvent>
{
    void ProcessEvent(TEvent @event);
}

class GeneralEventProcessor<TEvent> : IEventProcessor<TEvent>
{
    public void ProcessEvent(TEvent @event) { /* suppose this method is processing the given event.*/ }
}

class ValidatorProcessor<TEvent> : IEventProcessor<TEvent>
{
    private readonly IEventProcessor<TEvent> nextProcessor;

    public ValidatorProcessor(IEventProcessor<TEvent> eventProcessor)
    {
        this.nextProcessor = eventProcessor;
    }

    public void ProcessEvent(TEvent @event)
    {
        // validate the event first.
        if (!this.IsValid(@event))
            throw new InvalidEventException();

        // if everything is ok, call the next processor.
        this.nextProcessor.ProcessEvent(@event);
    }
}

using var container = new StashboxContainer();
container.Register(typeof(IEventProcessor<>), typeof(GeneralEventProcessor<>));
container.RegisterDecorator(typeof(IEventProcessor<>), typeof(ValidatorProcessor<>));

// new ValidatorProcessor<UpdateEvent>(new GeneralEventProcessor<UpdateEvent>())
var eventProcessor = container.Resolve<IEventProcessor<UpdateEvent>>();

// process the event.
eventProcessor.ProcessEvent(new UpdateEvent());
```

## Composite pattern

The [Composite pattern](https://en.wikipedia.org/wiki/Composite_pattern) allows a group of objects to be treated the same way as a single instance of the same type. It's useful when you want to use the functionality of multiple instances behind the same interface. You can achieve this by registering a decorator that takes a collection of the same service as a dependency. 

```cs
public class CompositeValidator<TEvent> : IEventValidator<TEvent>
{
    private readonly IEnumerable<IEventValidator<TEvent>> validators;

    public CompositeValidator(IEnumerable<IEventValidator<TEvent>> validators)
    {
        this.validators = validators;
    }

    public bool IsValid(TEvent @event)
    {
        return this.validators.All(validator => validator.IsValid(@event));
    }
}

container.Register(typeof(IEventValidator<>), typeof(EventValidator<>));
container.Register(typeof(IEventValidator<>), typeof(AnotherEventValidator<>));
container.RegisterDecorator(typeof(IEventValidator<>), typeof(CompositeValidator<>));
```

## Decorating multiple services
You have the option to organize similar decorating functionalities for different interfaces into the same decorator class. 
In this example, we would like to validate a given `Event` right before publishing and also before processing. 

```cs
public class EventValidator<TEvent> : IEventProcessor<T>, IEventPublisher<TEvent>
{
    private readonly IEventProcessor<TEvent> processor;
    private readonly IEventPublisher<TEvent> publisher;
    private readonly IEventValidator<TEvent> validator;

    public CompositeValidator(IEventProcessor<TEvent> processor, 
        IEventPublisher<TEvent> publisher, 
        IEventValidator<TEvent> validator)
    {
        this.processor = processor;
        this.publisher = publisher;
        this.validator = validator;
    }

    public void ProcessEvent(TEvent @event)
    {
        // validate the event first.
        if (!this.validator.IsValid(@event))
            throw new InvalidEventException();

        // if everything is ok, call the processor.
        this.processor.ProcessEvent(@event);
    }

    public void PublishEvent(TEvent @event)
    {
        // validate the event first.
        if (!this.validator.IsValid(@event))
            throw new InvalidEventException();

        // if everything is ok, call the publisher.
        this.publisher.PublishEvent(@event);
    }
}

container.Register(typeof(IEventProcessor<>), typeof(EventProcessor<>));
container.Register(typeof(IEventPublisher<>), typeof(EventPublisher<>));
container.Register(typeof(IEventValidator<>), typeof(EventValidator<>));

// without specifying the interface type, the container binds this registration to all of its implemented types
container.RegisterDecorator(typeof(EventValidator<>));
```

:::info
You can also use the [Binding to multiple services](/docs/guides/advanced-registration#binding-to-multiple-services) options.
:::

## Lifetime

<CodeDescPanel>
<div>

Just as other registrations, decorators also can have their lifetime. It means, in addition to the service's lifetime, all decorator's lifetime will be applied to the wrapped service.

:::note
When you don't set a decorator's lifetime, it'll implicitly inherit the decorated service's lifetime.
:::

</div>
<div>

```cs
container.Register<IEventProcessor, GeneralEventProcessor>();
// singleton decorator will change the transient 
// decorated service's lifetime to singleton.
container.RegisterDecorator<IEventProcessor, ValidatorProcessor>(options => 
    options.WithLifetime(Lifetimes.Singleton));
// Singleton[new ValidatorProcessor()](Transien[new GeneralEventProcessor()]) 
var processor = container.Resolve<IEventProcessor>(); 
```

</div>
</CodeDescPanel>

## Wrappers

<CodeDescPanel>
<div>

Decorators are also applied to wrapped services. It means, in addition to the decoration, you can wrap your services in supported [wrappers](/docs/advanced/wrappers-resolvers#wrappers).

</div>
<div>

```cs
container.Register<IEventProcessor, GeneralEventProcessor>();
container.RegisterDecorator<IEventProcessor, ValidatorProcessor>();
// () => new ValidatorProcessor(new GeneralEventProcessor())
var processor = container.Resolve<Func<IEventProcessor>>();
```

</div>
</CodeDescPanel>

## Interception
From the combination of Stashbox's decorator support and [Castle DynamicProxy's](http://www.castleproject.org/projects/dynamicproxy/) proxy generator, we can take advantage of the [Aspect-Oriented Programming's](https://en.wikipedia.org/wiki/Aspect-oriented_programming) benefits. The following example defines a `LoggingInterceptor` that will log additional messages related to the called service methods.

```cs
public class LoggingInterceptor : IInterceptor 
{
    private readonly ILogger logger;

    public LoggingInterceptor(ILogger logger)
    {
        this.logger = logger;
    }

    public void Intercept(IInvocation invocation)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // log before we invoke the intercepted method.
        this.logger.Log($"Method begin: {invocation.GetConcreteMethod().Name}");

        // call the intercepted method.
        invocation.Proceed();

        // log after we invoked the intercepted method and print how long it ran.
        this.logger.Log($"Method end: {invocation.GetConcreteMethod().Name}, execution duration: {stopwatch.ElapsedMiliseconds} ms");
    }
}

// create a DefaultProxyBuilder from the DynamicProxy library.
var proxyBuilder = new DefaultProxyBuilder();

// build a proxy for the IEventProcessor interface.
var eventProcessorProxy = proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(
    typeof(IEventProcessor), 
    new Type[0], 
    ProxyGenerationOptions.Default);

// register the logger for LoggingInterceptor.
container.Register<ILogger, ConsoleLogger>();

// register the service that we will intercept.
container.Register<IEventProcessor, GeneralEventProcessor>();

// register the interceptor.
container.Register<IInterceptor, LoggingInterceptor>();

// register the built proxy as a decorator.
container.RegisterDecorator<IEventProcessor>(eventProcessorProxy);
```