//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;

namespace BadEcho.Extensibility.Tests;

public sealed class PluggablePart
{
    [ImportMany]
    public IEnumerable<IFakePart>? FakeParts
    { get; set; }
}

public sealed class PluggablePartWithFilterableImports
{
    [Import]
    public IFilterableFakePart? FakePart
    { get; set; }
}

public sealed class PluggablePartAndDependency : IFakeDependency
{
    [ImportMany]
    public IEnumerable<IFakePartWithComposedDependencies>? FakeParts
    { get; set; }
}

public sealed class PluggablePartAndFilterableDependency : IFilterableFakeDependency
{
    public PluggablePartAndFilterableDependency(Guid familyId) 
        => FamilyId = familyId;

    [Import]
    public IFilterableFakePartWithComposedDependencies? FakePart
    { get; set; }

    public Guid FamilyId 
    { get; }
}