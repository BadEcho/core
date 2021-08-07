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

using System;
using System.Collections.Generic;
using System.Composition;

namespace BadEcho.Odin.Tests.Extensibility
{
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
}
