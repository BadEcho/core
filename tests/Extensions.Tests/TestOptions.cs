// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------


namespace BadEcho.Extensions.Tests;

/// <suppressions>
/// ReSharper disable NonReadonlyMemberInGetHashCode
/// </suppressions>
public class TestOptions
{
    public string OptionA
    { get; set; } = string.Empty;

    public string OptionB
    { get; set; } = string.Empty;
    
    public override bool Equals(object? obj)
    {
        if (obj is not TestOptions other)
            return false;

        return OptionA == other.OptionA
               && OptionB == other.OptionB;
    }

    public override int GetHashCode()
    {
        return this.GetHashCode(OptionA, OptionB);
    }
}

public class PrimaryFirstOptions : TestOptions
{
    public static string SectionName
        => "PrimaryFirst";
}

public class PrimarySecondOptions : TestOptions
{
    public static string SectionName
        => "PrimarySecond";
}

public class PrimaryNoSectionOptions : TestOptions
{
    public static string SectionName
        => "";
}

public class SecondaryOptions : TestOptions
{
    public static string SectionName
        => "Secondary";
}

public class NonexistentOptions : TestOptions
{
    public static string SectionName
        => "Nonexistent";
}
