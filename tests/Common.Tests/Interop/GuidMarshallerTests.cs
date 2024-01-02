//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

public unsafe class GuidMarshallerTests
{
    // Represents the GUID: D3012989-1FDC-4412-8069-DE04C6198E9A
    private const uint DATA1 = 0xD3012989;
    private const ushort DATA2 = 0x1FDC;
    private const ushort DATA3 = 0x4412;
    private const byte DATA4_1 = 0x80;
    private const byte DATA4_2 = 0x69;
    private const byte DATA4_3 = 0xDE;
    private const byte DATA4_4 = 0x04;
    private const byte DATA4_5 = 0xC6;
    private const byte DATA4_6 = 0x19;
    private const byte DATA4_7 = 0x8E;
    private const byte DATA4_8 = 0x9A;

    private readonly Guid _guid = new(DATA1,
                                      DATA2,
                                      DATA3,
                                      DATA4_1,
                                      DATA4_2,
                                      DATA4_3,
                                      DATA4_4,
                                      DATA4_5,
                                      DATA4_6,
                                      DATA4_7,
                                      DATA4_8);
    [Fact]
    public void ConvertToUnmanaged_SameData()
    {
        GUID unmanaged = GuidMarshaller.ConvertToUnmanaged(_guid);

        Assert.Equal(DATA1, unmanaged.Data1);
        Assert.Equal(DATA2, unmanaged.Data2);
        Assert.Equal(DATA3, unmanaged.Data3);

        Assert.Equal(DATA4_1, unmanaged.Data4[0]);
        Assert.Equal(DATA4_2, unmanaged.Data4[1]);
        Assert.Equal(DATA4_3, unmanaged.Data4[2]);
        Assert.Equal(DATA4_4, unmanaged.Data4[3]);
        Assert.Equal(DATA4_5, unmanaged.Data4[4]);
        Assert.Equal(DATA4_6, unmanaged.Data4[5]);
        Assert.Equal(DATA4_7, unmanaged.Data4[6]);
        Assert.Equal(DATA4_8, unmanaged.Data4[7]);
    }

    [Fact]
    public void ConvertToManaged_SameData()
    {
        var unmanaged = new GUID
                        {
                            Data1 = DATA1,
                            Data2 = DATA2,
                            Data3 = DATA3
                        };

        unmanaged.Data4[0] = DATA4_1;
        unmanaged.Data4[1] = DATA4_2;
        unmanaged.Data4[2] = DATA4_3;
        unmanaged.Data4[3] = DATA4_4;
        unmanaged.Data4[4] = DATA4_5;
        unmanaged.Data4[5] = DATA4_6;
        unmanaged.Data4[6] = DATA4_7;
        unmanaged.Data4[7] = DATA4_8;

        Guid managed = GuidMarshaller.ConvertToManaged(unmanaged);

        Assert.Equal(_guid, managed);
    }
}
