using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BadEcho.Game;

/// <summary>
/// Provides a shim conforming to MonoGame's standard two-stage initialization process for a game component requiring
/// its content dependencies at the time of instance creation. 
/// </summary>
/// <typeparam name="T">The inner game component contained by this shim.</typeparam>
/// <remarks>
/// We aren't dealing with COM, but the dusty, archaic architecture we are interfacing with befits some good ol' COM
/// shimming (as opposed to adapting).
/// </remarks>
public sealed class GameComponentShim<T> : DrawableGameComponent
    where T : DrawableGameComponent
{
    private readonly Func<Microsoft.Xna.Framework.Game, ContentManager, T> _componentFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameComponentShim{T}"/> class.
    /// </summary>
    public GameComponentShim(Microsoft.Xna.Framework.Game game,
                             Func<Microsoft.Xna.Framework.Game, ContentManager, T> componentFactory)
        : base(game)
    {
        Require.NotNull(componentFactory, nameof(componentFactory));

        _componentFactory = componentFactory;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }


}