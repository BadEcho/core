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

    private ContentManager? _contentManager;
    private AlphaSpriteEffect? _alphaEffect;
    private bool _isExiting;
    private bool _disposed;

    /// <summary>
    /// Gets a value indicating this state has the topmost scene in the z-order.
    /// </summary>
    public bool IsTopmost 
    { get; internal set; }
     
    /// <summary>
    /// Gets a <see cref="ActivationStatus"/> value that specifies this state's current activation phase.
    /// </summary>
    public ActivationStatus ActivationStatus
    { get; private set; }

    /// <summary>
    /// Gets or sets the amount of time required for this state's activation and deactivation.
    /// </summary>
    public TimeSpan ActivationTime
    { get; set; }

    /// <summary>
    /// Gets a percentage value representing the activation progress, with a minimum value of 0 meaning fully deactivated and a maximum
    /// value of 1 meaning fully activated.
    /// </summary>
    public float ActivationPercentage
    { get; private set; }

    /// <summary>
    /// Gets a value indicating if the state has finished exiting from the screen and is ready for full removal.
    /// </summary>
    public bool HasExited
    { get; private set; }

    /// <summary>
    /// Gets or sets the transitions this state undergoes when activating and deactivating.
    /// </summary>
    public Transitions StateTransitions
    { get; set; }
    
    /// <summary>
    /// Gets or sets the exponential power of the transitional animation interpolation.
    /// </summary>
    public double PowerCurve
    { get; set; } = 3.0;

    /// <summary>
    /// Gets a value indicating if the state acts like modal dialog on top of other states.
    /// </summary>
    public virtual bool IsModal
        => false;

    /// <summary>
    /// Gets the state manager this state has been added to.
    /// </summary>
    protected internal StateManager? Manager
    { get; internal set; }

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
    /// Gets a value indicating if the clipping regions will be honored while a state is activating and deactivating.
    /// </summary>
    protected virtual bool ClipDuringTransitions
        => true;

    /// <summary>
    /// Gets the base matrix to apply to position, rotation, scale, and depth data.
    /// </summary>
    protected virtual Matrix MatrixTransform
        => Matrix.Identity;

    /// <summary>
    /// Gets the drawing order for sprite and text drawing.
    /// </summary>
    protected virtual SpriteSortMode SortMode
        => SpriteSortMode.Deferred;

    /// <summary>
    /// Performs any necessary updates to the state, including its position, activation status, and other state-specific
    /// concerns.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public virtual void Update(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        if (HasExited)
            return;

        if (_isExiting)
        {
            UpdateActivation(time, false);

            if (ActivationStatus == ActivationStatus.Deactivated)
                HasExited = true;

            return;
        }

        // If this game state is at the top of the z-order, we'll transition to a visible state.
        // Otherwise, we'll transition to a hidden one.
        UpdateActivation(time, IsTopmost);
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
            transform *= Matrix.CreateRotationZ(MathHelper.ToRadians(ActivationPercentage * 360));

        var alpha = StateTransitions.HasFlag(Transitions.Fade)
            ? CalculateAnimationCurve()
            : 1f;

        bool clippingEnabled
            = ActivationStatus is ActivationStatus.Activated or ActivationStatus.Deactivated || ClipDuringTransitions;

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
    /// Prepares the state for its eventual removal from an active state manager by first transitioning it to a deactivated status.
    /// </summary>
    public void Exit() 
        => _isExiting = true;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Loads resources needed by the state and prepares it to be drawn to the screen.
    /// </summary>
    /// <param name="game">
    /// The game the state is being loaded for, which is used to acquire a <see cref="ContentManager"/> instance for
    /// loading resources.
    /// </param>
    internal void Load(Microsoft.Xna.Framework.Game game)
    {
        Require.NotNull(game, nameof(game));

        if (_contentManager != null)
            return;

        _contentManager = new ContentManager(game.Services, "Content");

        LoadContent(_contentManager);
    }

    /// <summary>
    /// Unloads resources previously loaded by the state, called when the state no longer needs to be drawn
    /// to the screen.
    /// </summary>
    internal void Unload()
    {
        if (_contentManager == null)
            return;

        _contentManager.Unload();
        _contentManager = null;
    }
    
    /// <summary>
    /// Loads state-specific resources using the provided content manager.
    /// </summary>
    /// <param name="contentManager">The content manager to use to load the content.</param>
    protected abstract void LoadContent(ContentManager contentManager);

    /// <summary>
    /// Executes the custom rendering logic required to draw the state to the screen.
    /// </summary>
    /// <param name="spriteBatch">
    /// A <see cref="ConfiguredSpriteBatch"/> instance with an active batch operation to draw this state to.
    /// </param>
    protected abstract void DrawCore(ConfiguredSpriteBatch spriteBatch);

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
            _contentManager?.Dispose();
        }

        _disposed = true;
    }

    private void UpdateActivation(GameUpdateTime time, bool isActivating)
    {
        float activationScale = ActivationTime != TimeSpan.Zero
            ? (float) (time.ElapsedGameTime.TotalMilliseconds / ActivationTime.TotalMilliseconds)
            : 1;

        ActivationPercentage += activationScale * (isActivating ? 1 : -1);

        if (!isActivating && ActivationPercentage <= 0 || isActivating && ActivationPercentage >= 1)
        {   // Activation/deactivation of the game state has concluded.
            ActivationPercentage = Math.Clamp(ActivationPercentage, 0, 1);

            ActivationStatus = isActivating ? ActivationStatus.Activated : ActivationStatus.Deactivated;
            return;
        }

        // We're still in the process of activating/deactivating the game state.
        ActivationStatus = isActivating ? ActivationStatus.Activating : ActivationStatus.Deactivating;
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
        => ActivationStatus == ActivationStatus.Activating
            ? (float) (1 - Math.Pow(1 - ActivationPercentage, PowerCurve))  // Ease-out
            : (float) Math.Pow(ActivationPercentage, PowerCurve);           // Ease-in

    private Matrix CreateZoomTransform(Viewport viewport)
    {
        var origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

        return
            Matrix.CreateTranslation(new Vector3(-origin, 0f))
            * Matrix.CreateScale(ActivationPercentage, ActivationPercentage, 1f)
            * Matrix.CreateTranslation(new Vector3(origin, 0f));
    }
}
