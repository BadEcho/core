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

using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Provides a movable camera for viewing orthogonally projected planes.
/// </summary>
public sealed class Camera : IPositionalEntity
{
    private readonly ViewportConnector _viewportConnector;
    
    private float _zoom = 1f;
    private float _minimumZoom = 0.1f;
    private float _maximumZoom = float.MaxValue;
    private float _zoomSpeed = 0.3f;

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class.
    /// </summary>
    /// <param name="viewportConnector">A connection to the render-target surface that the camera will be looking at.</param>
    public Camera(ViewportConnector viewportConnector)
    {
        Require.NotNull(viewportConnector, nameof(viewportConnector));

        _viewportConnector = viewportConnector;
        
        Origin = new Vector2(_viewportConnector.VirtualSize.Width / 2f, _viewportConnector.VirtualSize.Height / 2f);
    }

    /// <inheritdoc />
    public Vector2 Position 
    { get; set; }

    /// <inheritdoc />
    public Vector2 LastMovement 
    { get; set; }

    /// <inheritdoc />
    public Vector2 Velocity 
    { get; set; }

    /// <inheritdoc />
    public float Angle 
    { get; set; }

    /// <inheritdoc />
    public float AngularVelocity 
    { get; set; }

    /// <summary>
    /// Gets the location in world space where the coordinate axes intersect (i.e., the center).
    /// </summary>
    public Vector2 Origin
    { get; }

    /// <summary>
    /// Gets the location in visible space where the coordinate axes intersect (i.e., the center).
    /// </summary>
    public Vector2 Center
        => Origin + Position;

    /// <summary>
    /// Gets or sets the amount that the camera is zoomed in or out on the visible surface.
    /// </summary>
    /// <remarks>
    /// The amount of zoom is used to define the scale matrix created when this camera is calculating
    /// its view matrix. The default value of <c>1</c> results in the visible surface being displayed
    /// by the camera at its actual size.
    /// </remarks>
    public float Zoom
    {
        get => _zoom;
        set => _zoom = Math.Max(MinimumZoom, Math.Min(MaximumZoom, value));
    }

    /// <summary>
    /// Gets or sets the minimum amount of zoom allowed.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Attempted to set the minimum amount of zoom to a value less than or equal to zero.
    /// </exception>
    public float MinimumZoom
    {
        get => _minimumZoom;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), Strings.MinimumZoomCannotBeZeroOrLess);

            _minimumZoom = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum amount of zoom allowed.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Attempted to set the maximum amount of zoom to a value less than or equal to zero.
    /// </exception>
    public float MaximumZoom
    {
        get => _maximumZoom;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), Strings.MaximumZoomCannotBeZeroOrLess);

            _maximumZoom = value;
        }
    }

    /// <summary>
    /// Gets the rate at which the zoom level changes for every second of user input.
    /// </summary>
    public float ZoomSpeed
    {
        get => _zoomSpeed;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), Strings.ZoomSpeedCannotBeNegative);

            _zoomSpeed = value;
        }
    }

    /// <summary>
    /// Gets a matrix used to transform world-space coordinates into the camera's view-space coordinates.
    /// </summary>
    /// <returns>A transformation matrix that represents this camera's view.</returns>
    public Matrix GetViewMatrix()
        => GetViewMatrix(Vector2.One);

    /// <summary>
    /// Gets a matrix used to transform world-space coordinates into the camera's view-space coordinates.
    /// </summary>
    /// <param name="parallaxFactor">The amount by which the visible surface moves in relation to the camera.</param>
    /// <returns>A transformation matrix that represents this camera's view.</returns>
    /// <remarks>
    /// The <c>parallaxFactor</c> parameter can be used to apply a depth effect to the camera's view. This effect can be
    /// avoided by specifying a value of <see cref="Vector2.One"/>, which will result in the position of the projected surface
    /// to change at the same rate as the position of the camera.
    /// </remarks>
    public Matrix GetViewMatrix(Vector2 parallaxFactor)
        => Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0f)) 
            * Matrix.CreateTranslation(new Vector3(-Origin, 0f))
            * Matrix.CreateRotationZ(Angle)
            * Matrix.CreateScale(Zoom, Zoom, 1f)
            * Matrix.CreateTranslation(new Vector3(Origin, 0f))
            * _viewportConnector.GetScaleMatrix();

    /// <summary>
    /// Directly moves the camera to the specified position.
    /// </summary>
    /// <param name="position">The position to center the camera on.</param>
    public void MoveTo(Vector2 position)
    {   // Simply setting the position to the the provided value will place it in the upper-left corner of the
        // camera's view. We can center the camera in on the position instead by subtracting our origin from
        // the position, which will shift it to the center of our view. Perfect!
        Position = position - Origin;
    }

    /// <summary>
    /// Increases the camera's amount of zoom.
    /// </summary>
    /// <param name="state">The state of the game at this given point in time.</param>
    public void ZoomIn(GameState state)
    {
        Require.NotNull(state, nameof(state));

        Zoom += ZoomSpeed * (float) state.Time.ElapsedGameTime.TotalSeconds;
    }

    /// <summary>
    /// Decreases the camera's amount of zoom.
    /// </summary>
    /// <param name="state">The state of the game at this given point in time.</param>
    public void ZoomOut(GameState state)
    {
        Require.NotNull(state, nameof(state));

        Zoom -= ZoomSpeed * (float) state.Time.ElapsedGameTime.TotalSeconds;
    }
}
