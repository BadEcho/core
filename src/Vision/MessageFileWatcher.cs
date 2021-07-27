//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using BadEcho.Odin;
using BadEcho.Odin.Extensions;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides a watcher for a Vision module's message file that will notify listeners upon any changes being made to it.
    /// </summary>
    internal sealed class MessageFileWatcher : IMessageFileProvider, IDisposable
    {
        private readonly FileSystemWatcher _watcher;

        /// <inheritdoc/>
        public event EventHandler<EventArgs<string>>? NewMessages;

        /// <inheritdoc/>
        public string? CurrentMessages 
        { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFileWatcher"/> class.
        /// </summary>
        /// <param name="module">The vision module whose message file this watcher will be monitoring.</param>
        /// <param name="messageFilesDirectory"
        /// >The path relative from Vision's base directory to the directory where all message files are being written to.
        /// </param>
        public MessageFileWatcher(IVisionModule module, string messageFilesDirectory)
        {
            Require.NotNull(module, nameof(module));

            // FileSystemWatcher requires a valid path value -- it doesn't treat an empty path as the current directory.
            if (string.IsNullOrEmpty(messageFilesDirectory)) 
                messageFilesDirectory = AppContext.BaseDirectory;

            string messageFilePath = Path.Combine(messageFilesDirectory, module.MessageFile);

            if (File.Exists(messageFilePath))
                CurrentMessages = File.ReadAllText(messageFilePath);

            _watcher = new FileSystemWatcher
                       {
                           Filter = module.MessageFile,
                           NotifyFilter = NotifyFilters.LastWrite,
                           Path = messageFilesDirectory
                       };

            _watcher.Changed += HandleMessageFileChanged;
            _watcher.EnableRaisingEvents = true;
        }

        /// <inheritdoc/>
        public void Dispose()
            => _watcher.Dispose();

        private void HandleMessageFileChanged(object sender, FileSystemEventArgs e)
        {
            var messageFile = new FileInfo(e.FullPath);

            // Injected Omnified code will be writing to the message file at high frequency, so we should assume the file is
            // almost always open by that process.
            CurrentMessages = messageFile.ReadAllText(FileShare.ReadWrite);

            // TODO: Add mechanism to return only new messages if ProcessNewMessagesOnly is true.
            if (!string.IsNullOrEmpty(CurrentMessages))
                NewMessages?.Invoke(this, new EventArgs<string>(CurrentMessages));
        }
    }
}
