using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Odin.Configuration;
using Xunit;

namespace BadEcho.Odin.Tests.Configuration
{
    public class BadEchoSectionGroupTests
    {
        [Fact]
        public void NoConfig_GetSectionGroup()
        {
            var sectionGroup = BadEchoSectionGroup.GetSectionGroup();
        }
    }
}
