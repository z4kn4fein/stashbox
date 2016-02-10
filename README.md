# stashbox [![Build status](https://ci.appveyor.com/api/projects/status/0849ee6awjyxohei/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox/branch/master) [![Coverage Status](https://coveralls.io/repos/z4kn4fein/stashbox/badge.svg?branch=master&service=github)](https://coveralls.io/github/z4kn4fein/stashbox?branch=master) [![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/badge/gitter-join%20chat-green.svg)](https://gitter.im/z4kn4fein/stashbox?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet Version](http://img.shields.io/nuget/v/Stashbox.svg?style=flat)](https://www.nuget.org/packages/Stashbox/) [![NuGet Downloads](http://img.shields.io/nuget/dt/Stashbox.svg?style=flat)](https://www.nuget.org/packages/Stashbox/)
Stashbox is a lightweight, portable dependency injection framework for .NET based solutions.

##Service Registration
###Standard
The following example shows how you can bind your service to an interface or abstract base class.
```c#
container.RegisterType<ICreature, Elf>();
//or without generic parameters
container.RegisterType(typeof(ICreature), typeof(Elf));

//resolution
var creature = container.Resolve<ICreature>();
//or without generic parameters
var creature = container.Resolve(typeof(ICreature));
```
> The container will resolve the *Elf* type as an *ICreature*.

You can also register a service to itself, without specifying a base.
```c#
container.RegisterType<Elf>();
//or without generic parameters
container.RegisterType(typeof(Elf));

//resolution
var elf = container.Resolve<Elf>();
//or without generic parameters
var elf = container.Resolve(typeof(Elf));
```
> **Every function on the container is available with generic parameters and with simple type arguments as well.**

###Named
If you want to bind more implementations to a single service type then you can name your registration in the following way:
```c#
container.RegisterType<ICreature, Dwarf>("Bruenor");
container.RegisterType<ICreature, Drow>("Drizzt");

//resolution
var creature = container.Resolve<ICreature>("Bruenor");
```
> **Every registration can be named.**

###Instance
In some cases, you may want to use already instantiated services as dependencies:
```c#
var elf = new Elf();
container.RegisterInstance<ICreature>(elf);
```
> The container will return with the prepared instance every time when an ICreature is being resolved.

###ReMap
Stashbox supports the remapping of already registered services:
```c#
container.RegisterType<IDwarf, Bruenor>();
container.ReMap<IDwarf, Pwent>();
```
###BuildUp
Similar to the *Instance* registration except that with this type of registration the container will execute further operations (property injection, container extensions, etc.) on the registered instance.
```c#
var elf = new Elf();
container.BuildUp<ICreature>(elf);
```
###Factory
In some cases, you may want to use a specific factory method for creating an instance from your service. 
```c#
container.PrepareType<IDrow, Drizzt>().WithFactoryParameters(weapon => new Drizzt(weapon)).Register();

//resolution
container.Resolve<IDrow>(factoryParameters: new[] { MagicalWeapons.Twinkle });
```
> Right now just the factory methods with only 0,1,2 or 3 parameters are supported.

###Injection parameters
If you have some special parameters in your service's constructor which you'd like to set manually (primitive types for example, or some pre-evaluated values) you can use injection parameters.
```c#
class Drizzt : IDrow
{
	public Drizzt(IWeapon leftHand, IWeapon rightHand)
	{
		//...
	}
}
container.RegisterType<IWeapon, Twinkle>();
container.PrepareType<IDrow, Drizzt>().WithInjectionParameters(new InjectionParameter { Value = new Icingdeath(), Name = "rightHand" }).Register();
```
> The configuration above indicates that Stashbox will inject the pre-evaluated *Icingdeath* object as Drizzt's right-hand weapon and the other one will be resolved through the standard registration.

##Lifetime
Stashbox supports the lifetime management of service registrations, which means you can customize the way how Stashbox will manage the lifetime of your services.
###Transient lifetime
If you register your service with transient lifetime, Stashbox will always create a new instance from it.
```c#
container.PrepareType<IDrow, Drizzt>().WithLifetime(new TransientLifetime()).Register();
```
> If you don't specify any lifetime manager, Stashbox will use TransientLifetime by default.

###Singleton lifetime
If you register your service with singleton lifetime, Stashbox will create an instance from the requested service at the first resolution and stores it for the further requests.
```c#
container.PrepareType<IDrow, Drizzt>().WithLifetime(new SingletonLifetime()).Register();
```
###Custom lifetime
If you'd like to use a custom lifetime then you can create your own by implementing the `ILifetime` interface and pass it to the `WithLifetime()` method.
```c#
class DwarvenLifetime : ILifetime
{
	//...
}

container.PrepareType<IDwarf, Bruenor>().WithLifetime(new DwarvenLifetime()).Register();
```
> You can check the already implemented `TransientLifetime` or `SingletonLifetime` as an example for a custom lifetime.

##Overrides
In some cases, you may want to use pre-evaluated values at resolution time to override the parameters which would be resolved by the container. You have two options to achieve this.
###Type overrides
Override parameters by their type:
```c#
class Catti : IHuman
{
	public Catti(IWeapon weapon)
	{
		//...
	}
}

container.RegisterType<IWeapon, Khazidhea>();
container.RegisterType<IHuman, Catti>();

//resolution
var catti = container.Resolve<IHuman>(overrides: new[] { new TypeOverride(typeof(IWeapon), new Taulmaril()) });
```
> The configuration above indicates that instead of the standard registration, Stashbox will override Catti's weapon with Taulmaril.  

Or by their name:
###Named overrides
```c#
var catti = container.Resolve<IHuman>(overrides: new[] { new NamedOverride("weapon", new Taulmaril()) });
```
##Attributes
For a deeper customization you can use the following built-in attributes to get more control on the dependency resolution.
###Constructor injection
If you'd like to specify your preferences about which constructor you want to use for resolution, you can use the `InjectionConstructor` attribute.
```c#
class Bruenor : IDwarf
{
	public Bruenor(IWeapon weapon)
	{ 
		//...
	}
	
	[InjectionConstructor]
	public Bruenor(IShield shield, IWeapon weapon)
	{
		//...
	}
}
```
If you have multiple registrations for a service, you can use the `Dependency` attribute to specify which one you want to being injected.
```c#
class Drizzt : IDrow
{
	public Drizzt([Dependency("Twinkle")]IWeapon leftHand, [Dependency("Icingdeath")]IWeapon rightHand)
	{
		//...
	}
}

container.RegisterType<IWeapon, Twinkle>("Twinkle");
container.RegisterType<IWeapon, Icingdeath>("Icingdeath");
```
###Member injection
Stashbox supports property and field injection, again with the `Dependency` attribute.
```c#
class Drizzt : IDrow
{
	[Dependency("Icingdeath")]
	public IWeapon RightHand { get; set; }
	
	[Dependency("Twinkle")]
	private IWeapon leftHand;
}

container.RegisterType<IWeapon, Twinkle>("Twinkle");
container.RegisterType<IWeapon, Icingdeath>("Icingdeath");
```
###Injection method
Stashbox supports the invocation of methods at resolve time if you decorate them with the `InjectionMethod` attribute.
```c#
class Drizzt : IDrow
{
	[InjectionMethod]
	public void CallGuenhwyvar([Dependency("orcs")]IEnumerable<ICreature> targets)
	{
		//...
	}
}
```
##Generics
Stashbox supports the registration of open generic types:
```c#
interface IDrow<TLeftHand, TRightHand> 
{
	//...
}

class Drow<TLeftHand, TRightHand> : IDrow<TLeftHand, TRightHand> 
{
	public Drow(TLeftHand leftHand, TRightHand rightHand)
	{
		//...
	}
}

container.RegisterType(typeof(IDrow<,>), typeof(Drow<,>));
container.RegisterType<Twinkle>();
container.RegisterType<Icingdeath>();

//resolution
var drizzt = container.Resolve<IDrow<Twinkle, Icingdeath>>();
```
##Conditional resolution
If you want to specify in which cases you want to inject some special dependencies, you can use the conditional resolution feature of Stashbox.
###Parent type filter
```c#
class Wulfgar : IBarbarian
{
	[Dependency]
	public IWeapon Weapon { get; set; }
}

container.PrepareType<IWeapon, AegisFang>().WhenDependantIs<Wulfgar>().Register();
```
> The constraint above indicates that Stashbox will choose the `AegisFang` implementation of the `IWeapon` interface when `Wulfgar` is being resolved.

###Attribute filter
If you set attribute filter for your registration, Stashbox will inject it only, if the dependency is decorated with that attribute:
```c#
class NeutralGoodAttribute : Attribute { }
class ChaoticEvilAttribute : Attribute { }

class Drizzt : IDrow
{
	[Dependency, NeutralGood]
	public IPatron Patron { get; set; }
}

class Yvonnel : IDrow
{
	[Dependency, ChaoticEvil]
	public IPatron Patron { get; set; }
}

container.PrepareType<IPatron, Mielikki>().WhenHas<NeutralGood>().Register();
container.PrepareType<IPatron, Lolth>().WhenHas<ChaoticEvil>().Register();
```
##Multi resolution
Stashbox allows you to resolve all the available services bound to an interface or base class.
```c#
container.RegisterType<ICreature, Dwarf>("Bruenor");
container.RegisterType<ICreature, Drow>("Drizzt");
container.RegisterType<ICreature, Human>("Catti-brie");
container.RegisterType<ICreature, Barbarian>("Wulfgar");
container.RegisterType<ICreature, Halfling>("Regis");

//resolution
var companionsOfTheHall = container.ResolveAll<ICreature>();

//or
class CompanionsOfTheHall
{
	public CompanionsOfTheHall(IEnumerable<ICreature> members)
	{
		//...
	}
	
	//or
	
	public CompanionsOfTheHall(ICreature[] members)
	{
		//...
	}
}
```
##`Lazy<T>`
Stashbox supports the lazy resolution of the services:
```c#
container.RegisterType<ICreature, Dwarf>("Bruenor");
container.RegisterType<ICreature, Drow>("Drizzt");
container.RegisterType<ICreature, Human>("Catti-brie");
container.RegisterType<ICreature, Barbarian>("Wulfgar");
container.RegisterType<ICreature, Halfling>("Regis");

//resolution
var lazyMemberOfTheCompanionsOfTheHall = container.Resolve<Lazy<ICreature>>("Regis");

//or
class CompanionsOfTheHall
{
	[Dependency("Regis")]
	public Lazy<ICreature> Regis { get; set; }

	public CompanionsOfTheHall([Dependency("Bruenor")]ICreature bruenor, [Dependency("Drizzt")]ICreature drizzt, [Dependency("Catti-brie")]ICreature cattibrie, [Dependency("Wulfgar")]ICreature wulfgar)
	{
		//...
	}
}
```
##Resolvers
Stashbox internally maintains a resolution graph between the registered services with a `Resolver` instance assigned to each of them. For the faster resolution it also builds an Expression tree through all the dependencies.
Stashbox contains three pre-defined Resolvers:
* `ContainerResolver`: It mainly uses the container itself for resolving dependencies.
* `EnumerableResolver`: It resolves every registered type of a service as an `IEnumerable` or array.
* `LazyResolver`: It can resolve services registered into the container wrapped into a `Lazy<T>`.

You can also specify a special rule to resolve services and extend the functionality of Stashbox. To achieve this you can register a custom `Resolver` implementation into Stashbox:
```c#
class MagicalWeaponResolver : Resolver
{
	 public MagicalWeaponResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
		}
		
	public override object Resolve(ResolutionInfo resolutionInfo)
    {
        //..
    }

    public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
    {
        //..
    }
}
The `TypeInformation` parameters holds information about the type which was actually requested from the container.
The `ResolutionInfo` parameter holds information about the current resolution context (factory parameters, overrides, etc.)
The `ResolutionInfoExpression` parameter represents an `ParameterExpression` which will be passed as a parameter to the lambda expression built by the resolution graph. 
```
Then you can register your `Resolver`:
```c#
container.RegisterResolver((context, typeInfo) => new MagicalWeaponResolver(context, typeInfo),
	(context, typeInfo) => Gauntlgrym.CanProduce(typeInfo));
```
> The first parameter is a factory delegate for creating a resolver instance. The second parameter is a predicate delegate which can decide the actual type can be resolved by the resolver or not.

Stashbox will choose from the resolvers to accomplish the current resolution request in the following order:
1. `ContainerResolver`
2. `EnumerableResolver`
3. `LazyResolver`
4. Custom, external resolvers
5. Parent container resolvers

##Child container
Stashbox supports the creation of child containers which you can use for separating scopes for your services. If you creates a new child container it will remain in a child-parent relationship with the original one, which means when you want to resolve something which is not available in the child, it will try to resolve it through it's parent.
###Using a child container
```c#
class Drizzt : IDrow
{
	[Dependency("Icingdeath")]
	public IWeapon RightHand { get; set; }
	
	[Dependency("Twinkle")]
	public IWeapon LeftHand { get; set; }
}

container.RegisterType<IWeapon, Twinkle>("Twinkle");

var child = container.CreateChildContainer();
child.RegisterType<IWeapon, Icingdeath>("Icingdeath");
child.RegisterType<IDrow, Drizzt>();

var drizzt = child.Resolve<IDrow>();
```
> In the example below, `Twinkle` will be injected by the parent container, but if you registers a new service with the name Twinkle into the child, the child's one will be used.
> The extensions and the resolvers are being copied to the child from the parent.

##Extensions
If you'd like to extend the functionality of Stashbox by your own custom logic you can use the container extensions.
###RegistrationExtension
With this type of extension you can inject your custom operations into the service registration flow.
To create a `RegistrationExtension` you should implement the `IRegistrationExtension` interface:
```c#
public class ContainerExtension : IRegistrationExtension
{
	public void Initialize(IContainerContext containerContext)
	{
		//will be called when the extension is being registered into Stashbox.
	}
	
	void OnRegistration(IContainerContext containerContext, RegistrationInfo registrationInfo, InjectionParameter[] injectionParameters = null)
	{
		//will be called when a service is being registered into Stashbox.
	}
	
	public void CleanUp()
	{
		//will be called when Stashbox is going to be disposed.
	}
	
	public IContainerExtension CreateCopy()
	{
		//will be called when a child container is being created from the container which holds the extension.
	}
}
```
###PostBuildExtension
With this type of extension you can write custom operations to extend an object produced by the container.
To create a `PostBuildExtension` you should implement the `IPostBuildExtension` interface:
```c#
public class ContainerExtension : IPostBuildExtension
{
	public void Initialize(IContainerContext containerContext)
	{
		//will be called when the extension is being registered into Stashbox.
	}
	
	object PostBuild(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            TypeInformation resolveType, InjectionParameter[] injectionParameters = null);
	{
		//will be called when a service is constructed by the container.
	}
	
	public void CleanUp()
	{
		//will be called when Stashbox is going to be disposed.
	}
	
	public IContainerExtension CreateCopy()
	{
		//will be called when a child container is being created from the container which holds the extension.
	}
}
```
###Extension registration
```c#
container.RegisterExtension(new ContainerExtension());
```
> For a complete example you can check the [decorator extension](https://github.com/z4kn4fein/stashbox-decoratorextension) for Stashbox.
