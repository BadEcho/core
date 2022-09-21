//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace BadEcho.Game.Tests;

/// <summary>
/// Provides a runner for unit tests that test content being loaded from the content pipeline.
/// </summary>
internal sealed class ContentTestRunner : Microsoft.Xna.Framework.Game
{
    private readonly Action<Microsoft.Xna.Framework.Game> _contentAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTestRunner"/> class.
    /// </summary>
    /// <param name="contentAction">The content loading test action to execute.</param>
    private ContentTestRunner(Action<Microsoft.Xna.Framework.Game> contentAction)
    {
        _contentAction = contentAction;

        _ = new GraphicsDeviceManager(this)
            {  
                PreferredBackBufferHeight = 1,
                PreferredBackBufferWidth = 1,
                IsFullScreen = false
            };

        Content.RootDirectory = "Content";
    }

    /// <inheritdoc/>
    protected override void LoadContent()
    {
        base.LoadContent();

        _contentAction(this);

        Exit();
    }

    protected override void Initialize()
    {
        _contentAction(this);

        base.Initialize();
    }

    /// <summary>
    /// Runs a content pipeline unit test.
    /// </summary>
    /// <param name="contentAction">The content loading test action to execute.</param>
    public static void Run(Action<Microsoft.Xna.Framework.Game> contentAction)
    {
        using (var testLoader = new ContentTestRunner(contentAction))
        {
            testLoader.Run();
        }
    }
}
