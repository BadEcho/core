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
using System.IO;
using System.Threading.Tasks;
using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;
using BadEcho.Omnified.Vision.Extensibility;
using Xunit;

namespace BadEcho.Omnified.Vision.Tests
{
    public class MessageFileWatcherTests : IDisposable
    {
        private readonly FakeVisionModule _module;
        private readonly MessageFileWatcher _watcher;

        public MessageFileWatcherTests()
        {
            _module = new FakeVisionModule();

            _watcher = new MessageFileWatcher(_module, string.Empty);
        }
        
        public void Dispose() 
            => _watcher.Dispose();

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
            public AnchorPointLocation DefaultLocation
                => AnchorPointLocation.TopLeft;

            public GrowthDirection GrowthDirection
                => GrowthDirection.Vertical;

            public string MessageFile
                => "testMessage.json";

            public bool ProcessNewMessagesOnly
                => false;
            
            public IViewModel EnableModule(IMessageFileProvider messageProvider) 
                => throw new NotImplementedException();
        }
    }
}
