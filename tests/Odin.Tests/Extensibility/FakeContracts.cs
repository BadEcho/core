//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensibility;

namespace BadEcho.Odin.Tests.Extensibility;

public static class FakeFilterableIds
{
    public const string AlphaFakeIdValue = "7D638085-3744-41AB-AF6C-4AFC5C2FC806";
    public static Guid AlphaFakeId
        => new(AlphaFakeIdValue);

    public const string BetaFakeIdValue = "B37B492F-9493-4D05-AD04-138E53E73F12";
    public static Guid BetaFakeId
        => new(BetaFakeIdValue);

    public const string GammaFakeIdValue = "E7A75BE5-839E-4CE4-A0AA-61FFADD8675A";
    public static Guid GammaFakeId
        => new(GammaFakeIdValue);

    public const string DeltaFakeIdValue = "C6412E46-3908-4223-9E72-4E8F6246FF4C";
    public static Guid DeltaFakeId
        => new(DeltaFakeIdValue);
}

public interface IFakePart
{
    int DoSomething();
}

public interface IFakeDependency
{ }

public interface IFakePartWithDependencies
{
    IFakeDependency Dependency { get; }
}

public interface IFakePartWithComposedDependencies
{
    IFakeDependency Dependency { get; }
}

public interface IFilterableFakePart : IFilterable
{
    int DoSomething();
}

public interface IFilterableFakeDependency : IFilterable
{ }

public interface IFilterableFakePartWithDependencies : IFilterableFakePart
{
    IFilterableFakeDependency Dependency { get; }
}

public interface IFilterableFakePartWithComposedDependencies : IFilterableFakePartWithDependencies
{ }

public interface IFilterableFakePartWithNonFilterableDependencies : IFilterableFakePart
{
    IFakeDependency Dependency { get; }
}

public interface INonSharedFakePart
{
    int DoSomething();
}

public interface ISharedFakePart
{
    int DoSomething();
}

public interface IUniqueRequirement
{ }