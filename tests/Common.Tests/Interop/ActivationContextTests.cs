// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop;
using System.Text;
using Xunit;

namespace BadEcho.Tests.Interop;

public class ActivationContextTests
{
    [Fact]
    public void GetManifestResourceStream_AppManifest_ReturnsValid()
    {
        using (var stream = typeof(ActivationScope).Assembly.GetManifestResourceStream("BadEcho.Properties.app.manifest"))
        {
            Assert.NotNull(stream);
        }
    }

    [Fact]
    public void Create_ManifestStream_TemporaryManifestDeleted()
    {
        string marker = Guid.NewGuid().ToString("N");
        string manifest;

        using (Stream? resource = typeof(ActivationScope).Assembly.GetManifestResourceStream("BadEcho.Properties.app.manifest"))
        {
            Assert.NotNull(resource);

            using (var reader = new StreamReader(resource))
            {
                manifest = reader.ReadToEnd();
            }
        }

        // A trailing comment keeps the manifest valid while making any temporary copy written by this test identifiable.
        manifest += $"{Environment.NewLine}<!-- {marker} -->";

        string tempPath = Path.GetTempPath();
        var existingFiles = new HashSet<string>(Directory.EnumerateFiles(tempPath), StringComparer.OrdinalIgnoreCase);

        using (var manifestStream = new MemoryStream(Encoding.UTF8.GetBytes(manifest)))
        using (ActivationScope.Create(manifestStream))
        { }

        IEnumerable<string> leakedFiles = Directory.EnumerateFiles(tempPath)
                                                   .Where(f => !existingFiles.Contains(f))
                                                   .Where(f => ContainsMarker(f, marker));
        Assert.Empty(leakedFiles);

        static bool ContainsMarker(string path, string marker)
        {
            try
            {
                return File.ReadAllText(path).Contains(marker, StringComparison.Ordinal);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
