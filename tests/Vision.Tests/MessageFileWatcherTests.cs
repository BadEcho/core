//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Presentation.ViewModels;
using BadEcho.Vision.Extensibility;
using Xunit;

namespace BadEcho.Vision.Tests;

public class MessageFileWatcherTests : IDisposable
{
    private readonly FakeVisionModule _module;
    private readonly MessageFileWatcher _watcher;

    private bool _disposed;

    public MessageFileWatcherTests()
    {
        _module = new FakeVisionModule();

        _watcher = new MessageFileWatcher(_module, string.Empty);
    }
        
    public void Dispose()
    {
        if (_disposed)
            return;

        _watcher.Dispose();

        _disposed = true;
    }

    [Fact]
    public void Write_ChangeProcessed()
    {
        Assert.RaisesAsync<EventArgs<string>>(e => _watcher.NewMessages += e,
                                              e => _watcher.NewMessages -= e,
                                              () => Task.Run(Change));
        void Change()
        {
            using (var writer = File.AppendText(_module.MessageFile))
            {
                writer.WriteLine("A change");
            }
        }
    }

    private sealed class FakeVisionModule : IVisionModule
    {
        public AnchorPointLocation Location
            => AnchorPointLocation.TopLeft;

        public GrowthDirection GrowthDirection
            => GrowthDirection.Vertical;

        public string MessageFile
            => "testMessage.json";

        public bool ProcessNewMessagesOnly
            => false;
            
        public IViewModel EnableModule(IMessageFileProvider messageProvider) 
            => new FakeViewModel();
    }

    private sealed class FakeViewModel : ViewModel
    {
        /// <inheritdoc />
        public override void Disconnect()
        { }
    }
}