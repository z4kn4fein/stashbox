using System;

namespace Stashbox.Attributes;

/// <summary>
/// When a parameter is marked with this attribute, the container will pass the given dependency's name to it.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class DependencyNameAttribute : Attribute;