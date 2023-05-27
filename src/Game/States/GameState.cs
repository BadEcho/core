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
public abstract class GameState
{
    private ContentManager? _contentManager;
    private float _activationPercentage;
    private bool _isUnloading;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameState"/> class.
    /// </summary>
    /// <param name="manager">The manager orchestrating </param>
    protected GameState(StateManager manager)
    {
        Require.NotNull(manager, nameof(manager));

        Manager = manager;
    }

    /// <summary>
    /// Gets a value indicating if this state's scene can receive input.
    /// </summary>
    public bool IsReceivingInput 
    { get; internal set; }

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
    /// Gets or sets the amount of time required for the state's activation and deactivation.
    /// </summary>
    public TimeSpan ActivationTime
    { get; set; }

    /// <summary>
    /// Gets the manager orchestrating this and other game states.
    /// </summary>
    protected StateManager Manager
    { get; }

    /// <summary>
    /// Loads resources needed by the state and prepares it to be drawn to the screen.
    /// </summary>
    public void Load()
    {
        if (_contentManager != null)
            return;

        _contentManager = new ContentManager(Manager.Game.Services, "Content");

        LoadContent(_contentManager);
    }

    /// <summary>
    /// Unloads resources previously loaded by the state, called when the state no longer needs to be drawn
    /// to the screen.
    /// </summary>
    public void Unload()
    {
        if (_contentManager == null)
            return;

        _contentManager.Unload();
        _contentManager = null;
    }

    /// <summary>
    /// Performs any necessary updates to the state, including its position, activation status, and other state-specific
    /// concerns.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public virtual void Update(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        if (_isUnloading)
        {
            UpdateActivation(time, false);

            if (ActivationStatus == ActivationStatus.Deactivated)
                Manager.RemoveState(this);

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
    /// Loads state-specific resources using the provided content manager.
    /// </summary>
    /// <param name="contentManager">The content manager to use to load the content.</param>
    protected abstract void LoadContent(ContentManager contentManager);

    private void UpdateActivation(GameUpdateTime time, bool isActivating)
    {
        float activationScale
            = (float) (time.ElapsedGameTime.TotalMilliseconds / ActivationTime.TotalMilliseconds);

        _activationPercentage += activationScale * (isActivating ? 1 : -1);

        if (!isActivating && _activationPercentage <= 0 || isActivating && _activationPercentage >= 1)
        {   // Activation/deactivation of the game state has concluded.
            _activationPercentage = Math.Clamp(_activationPercentage, 0, 1);

            ActivationStatus = isActivating ? ActivationStatus.Activated : ActivationStatus.Deactivated;
        }

        // We're still in the process of activating/deactivating the game state.
        ActivationStatus = isActivating ? ActivationStatus.Activating : ActivationStatus.Deactivating;
    }
}
