using System;

namespace Stashbox.Attributes;

/// <summary>
/// Represents an attribute for tracking injection methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class InjectionMethodAttribute : Attribute
{ }