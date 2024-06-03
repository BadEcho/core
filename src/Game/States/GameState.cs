//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Game.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a scene for a game with independently managed content.
/// </summary>
public abstract class GameState : IDisposable
{
    private const Transitions MOVEMENT_FLAGS =
        Transitions.MoveLeft | Transitions.MoveRight | Transitions.MoveUp | Transitions.MoveDown;

    private AlphaSpriteEffect? _alphaEffect;
    private bool _isClosing;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameState"/> class.
    /// </summary>
    /// <param name="game">The game this state is for.</param>
    protected GameState(Microsoft.Xna.Framework.Game game)
    {
        Require.NotNull(game, nameof(game));

        Content = new ContentManager(game.Services, "Content");
        Game = game;
    }

    /// <summary>
    /// Gets a <see cref="TransitionStatus"/> value that specifies this state's current transition phase.
    /// </summary>
    public TransitionStatus TransitionStatus
    { get; private set; }

    /// <summary>
    /// Gets or sets the amount of time required for this state's transitions.
    /// </summary>
    public TimeSpan TransitionTime
    { get; set; }

    /// <summary>
    /// Gets a percentage value representing the transition progress, with a minimum value of 0, meaning
    /// fully exited off the screen, and a maximum value of 1, meaning fully entered onto the screen.
    /// </summary>
    public float TransitionPercentage
    { get; private set; }

    /// <summary>
    /// Gets a value indicating if a closing state has finished its exit transition and is ready for full removal.
    /// </summary>
    public bool HasClosed
    { get; private set; }

    /// <summary>
    /// Gets or sets the transitions this state undergoes when entering and exiting the screen.
    /// </summary>
    public Transitions StateTransitions
    { get; set; }
    
    /// <summary>
    /// Gets or sets the exponent of the power function used to interpolate transitional animations.
    /// </summary>
    public double PowerCurve
    { get; set; } = 3.0;

    /// <summary>
    /// Gets a value indicating if the state acts like modal dialog on top of other states.
    /// </summary>
    public virtual bool IsModal
        => false;

    /// <summary>
    /// Gets this state's <see cref="ContentManager"/> instnace.
    /// </summary>
    protected ContentManager Content
    { get; }

    /// <summary>
    /// Gets the game this state is for.
    /// </summary>
    protected Microsoft.Xna.Framework.Game Game
    { get; }

    /// <summary>
    /// Gets the state manager this state has been added to.
    /// </summary>
    protected StateManager? Manager
    { get; private set; }

    /// <summary>
    /// Gets or sets the coordinates to the upper-left corner of the state's content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used when performing movement transitions to begin the transition at a position bordering visible content. Often a state's 
    /// content does not encompass the entire screen.
    /// </para>
    /// <para>
    /// Leave this at the default to move content end-to-end across the entire viewport.
    /// </para>
    /// </remarks>
    protected Point ContentOrigin
    { get; set; }

    /// <summary>
    /// Gets a value indicating if this type of state honors clipping regions while transitioning.
    /// </summary>
    protected virtual bool ClipDuringTransitions
        => true;

    /// <summary>
    /// Gets the base matrix that this type of state applies to position, rotation, scale, and depth data.
    /// </summary>
    protected virtual Matrix MatrixTransform
        => Matrix.Identity;

    /// <summary>
    /// Gets the drawing order this type of state uses for sprite and text drawing.
    /// </summary>
    protected virtual SpriteSortMode SortMode
        => SpriteSortMode.Deferred;

    /// <summary>
    /// Gets a value indicating if this type of state transitions onto (and remains on) the screen, even if it is not
    /// the active state (i.e., it is not the topmost in the z-order).
    /// </summary>
    protected virtual bool AlwaysDisplay
        => false;

    /// <summary>
    /// Performs any necessary updates to the state, including its position, transition status, and other state-specific
    /// concerns.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    /// <param name="isActive">
    /// Value indicating whether this is the active state on top of all others in the z-order and, therefore, should be
    /// visible on the screen.
    /// </param>
    /// <remarks> 
    /// By default, a state must be the active state to enter the screen and will begin to exit off the screen if not.
    /// To change this behavior, override <see cref="AlwaysDisplay"/> so that it returns true.
    /// </remarks>
    public void Update(GameUpdateTime time, bool isActive)
    {
        Require.NotNull(time, nameof(time));

        if (HasClosed)
            return;

        if (_isClosing)
        {
            UpdateTransition(time, false);

            if (TransitionStatus == TransitionStatus.Exited)
                HasClosed = true;

            return;
        }

        UpdateCore(time, isActive);
        UpdateTransition(time, isActive || AlwaysDisplay);
    }

    /// <summary>
    /// Draws the scene associated with the state to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the state.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var transform = MatrixTransform;
        
        if ((StateTransitions & MOVEMENT_FLAGS) != 0)
            transform *= CreateMoveTransform(viewport);

        if (StateTransitions.HasFlag(Transitions.Zoom))
            transform *= CreateZoomTransform(viewport);

        if (StateTransitions.HasFlag(Transitions.Rotate))
            transform *= Matrix.CreateRotationZ(MathHelper.ToRadians(TransitionPercentage * 360));

        var alpha = StateTransitions.HasFlag(Transitions.Fade)
            ? CalculateAnimationCurve()
            : 1f;

        bool clippingEnabled
            = TransitionStatus is TransitionStatus.Entered or TransitionStatus.Exited || ClipDuringTransitions;

        _alphaEffect ??= new AlphaSpriteEffect(spriteBatch.GraphicsDevice);

        _alphaEffect.Alpha = alpha;
        _alphaEffect.MatrixTransform = transform;

        var configuredSpriteBatch = new ConfiguredSpriteBatch(
            spriteBatch,
            SortMode,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            rasterizerState: new RasterizerState { ScissorTestEnable = clippingEnabled },
            matrixTransform: transform);

        configuredSpriteBatch.LoadEffect(_alphaEffect);
        configuredSpriteBatch.Begin();
        
        DrawCore(configuredSpriteBatch);

        configuredSpriteBatch.End();
    }
    
    /// <summary>
    /// Handles the input being currently sent by the user.
    /// </summary>
    public virtual void ProcessInput()
    { }

    /// <summary>
    /// Prepares the state for its eventual removal from an active state manager by first performing an
    /// exit transition.
    /// </summary>
    public void Close() 
        => _isClosing = true;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Associates this state with the provided state manager in preparation for being drawn to the screen.
    /// </summary>
    /// <param name="manager">The state manager this state is being loaded into.</param>
    internal void Load(StateManager manager)
    {
        Require.NotNull(manager, nameof(manager));

        OnLoad(manager);

        Manager = manager;
    }

    /// <summary>
    /// Executes custom state-specific update logic.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    /// <param name="isActive">
    /// Value indicating whether this is the active state on top of all others in the z-order.
    /// </param>
    protected abstract void UpdateCore(GameUpdateTime time, bool isActive);

    /// <summary>
    /// Executes the custom rendering logic required to draw the state to the screen.
    /// </summary>
    /// <param name="spriteBatch">
    /// A <see cref="ConfiguredSpriteBatch"/> instance with an active batch operation to draw this state to.
    /// </param>
    protected abstract void DrawCore(ConfiguredSpriteBatch spriteBatch);

    /// <summary>
    /// Called when this state is being loaded into a state manager in preparation for being drawn to the screen.
    /// </summary>
    /// <param name="manager">The state manager this state is being loaded into.</param>
    protected virtual void OnLoad(StateManager manager)
    {
        Require.NotNull(manager, nameof(manager));
    }

    /// <summary>
    /// Releases unmanaged and (optionally) managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Content.Dispose();
        }

        _disposed = true;
    }

    private void UpdateTransition(GameUpdateTime time, bool isActive)
    {
        float transitionScale = TransitionTime != TimeSpan.Zero
            ? (float) (time.ElapsedGameTime.TotalMilliseconds / TransitionTime.TotalMilliseconds)
            : 1;

        TransitionPercentage += transitionScale * (isActive ? 1 : -1);

        if (!isActive && TransitionPercentage <= 0 || isActive && TransitionPercentage >= 1)
        {   // Reached end of transition phase.
            TransitionPercentage = Math.Clamp(TransitionPercentage, 0, 1);

            TransitionStatus = isActive ? TransitionStatus.Entered : TransitionStatus.Exited;
            return;
        }

        // We're still in the process of transitioning the game state on or off the screen.
        TransitionStatus = isActive ? TransitionStatus.Entering : TransitionStatus.Exiting;
    }

    private Matrix CreateMoveTransform(Viewport viewport)
    {
        var position = (StateTransitions & MOVEMENT_FLAGS) switch
        {
            Transitions.MoveLeft => new Vector3(CalculateDisplacement(viewport, true, true), 0f, 0f),
            Transitions.MoveRight => new Vector3(CalculateDisplacement(viewport, true, false), 0f, 0f),
            Transitions.MoveUp => new Vector3(0f, CalculateDisplacement(viewport, false, true), 0f),
            Transitions.MoveDown => new Vector3(0f, CalculateDisplacement(viewport, false, false), 0f),
            _ => Vector3.Zero
        };

        return Matrix.CreateTranslation(position);
    }

    private float CalculateDisplacement(Viewport viewport, bool horizontal, bool positiveOrigination)
    {
        // The directional multiplier.
        float dX = positiveOrigination ? 1f : -1f;

        return horizontal
            ? dX * (viewport.Width - ContentOrigin.X) - dX * (viewport.Width - ContentOrigin.X) * CalculateAnimationCurve()
            : dX * (viewport.Height - ContentOrigin.Y) - dX * (viewport.Height - ContentOrigin.Y) * CalculateAnimationCurve();
    }

    // Will make this configurable if there's ever a need for more control over the easing functions.
    private float CalculateAnimationCurve()
        => TransitionStatus == TransitionStatus.Entering
            ? (float) (1 - Math.Pow(1 - TransitionPercentage, PowerCurve))  // Ease-out
            : (float) Math.Pow(TransitionPercentage, PowerCurve);           // Ease-in

    private Matrix CreateZoomTransform(Viewport viewport)
    {
        var origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

        return
            Matrix.CreateTranslation(new Vector3(-origin, 0f))
            * Matrix.CreateScale(TransitionPercentage, TransitionPercentage, 1f)
            * Matrix.CreateTranslation(new Vector3(origin, 0f));
    }
}
