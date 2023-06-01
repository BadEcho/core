//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a scene for a game with independently managed content.
/// </summary>
public abstract class GameState : IDisposable
{
    private ContentManager? _contentManager;
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
    /// Gets a value indicating if the state acts like modal dialog on top of other states.
    /// </summary>
    public virtual bool IsModal
        => false;

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
        // Otherwise, we'll transition to hidden one.
        UpdateActivation(time, IsTopmost);
    }

    /// <summary>
    /// Draws the scene associated with the state to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the state.</param>
    public abstract void Draw(SpriteBatch spriteBatch);
    
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
    /// The game the state is being loaded for, which can be used to acquire a <see cref="ContentManager"/> instance for
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
        float activationScale
            = (float) (time.ElapsedGameTime.TotalMilliseconds / ActivationTime.TotalMilliseconds);

        ActivationPercentage += activationScale * (isActivating ? 1 : -1);

        if (!isActivating && ActivationPercentage <= 0 || isActivating && ActivationPercentage >= 1)
        {   // Activation/deactivation of the game state has concluded.
            ActivationPercentage = Math.Clamp(ActivationPercentage, 0, 1);

            ActivationStatus = isActivating ? ActivationStatus.Activated : ActivationStatus.Deactivated;
        }

        // We're still in the process of activating/deactivating the game state.
        ActivationStatus = isActivating ? ActivationStatus.Activating : ActivationStatus.Deactivating;
    }
}
