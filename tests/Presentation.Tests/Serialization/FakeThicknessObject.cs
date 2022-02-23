//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Windows;
using BadEcho.Presentation.Serialization;

namespace BadEcho.Presentation.Tests.Serialization;

public sealed class FakeThicknessObject
{
    [JsonConverter(typeof(JsonThicknessConverter))]
    public Thickness SomeThickness
    { get; set; }
}