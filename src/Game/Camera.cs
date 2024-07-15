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

using System.Numerics;
using BadEcho.Extensions;
using BadEcho.Game.Properties;
using BadEcho.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace BadEcho.Game;

/// <summary>
/// Provides a movable camera for viewing orthogonally projected planes.
/// </summary>
public sealed class Camera : IPositionalEntity
{
    private readonly ViewportConnector _viewportConnector;
    private readonly CollisionEngine _deadZoneEngine;

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

        PresentationParameters parameters = viewportConnector.Device.PresentationParameters;

        _viewportConnector = viewportConnector;
        _deadZoneEngine = new CollisionEngine(
            new RectangleF(new PointF(-parameters.BackBufferWidth, -parameters.BackBufferHeight), new SizeF(parameters.BackBufferWidth * 2, parameters.BackBufferHeight * 2)));
        
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

    public float MaxSpeed 
    { get; set; }

    /// <inheritdoc />
    public float Angle
    { get; set; }

    /// <inheritdoc />
    public float AngularVelocity
    { get; set; }

    /// <summary>
    /// Gets the bounding rectangle of visible content, with coordinates corresponding to the virtual size of the content,
    /// which is its size prior to any scaling meant to fit it to the render-target surface is applied.
    /// </summary>
    public RectangleF Bounds
    {
        get
        {
            // The virtual view matrix will be free of any scaling used to fit the content to the render-target surface.
            Matrix view = GetVirtualViewMatrix();
            Matrix viewProjection = view.MultiplyBy2DProjection(_viewportConnector.VirtualSize);

            // We can easily get our bounds by using the corners of a bounding frustum that uses our view * projection matrix.
            var frustum = new BoundingFrustum(viewProjection);
            Vector3[] corners = frustum.GetCorners();
            Vector3 topLeft = corners[0];
            Vector3 bottomRight = corners[2];

            return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }
    }

    /// <summary>
    /// Gets the location in world-space where the coordinate axes intersect (i.e., the center).
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

    private SizeF? ContentSize
    { get; set; }

    private Vector2 DeadZoneOffset
    { get; set; }

    private float DeadZoneXOffset
    { get; set; }
    private float DeadZoneYOffset
    { get; set; }

    public void LockToContent(SizeF? contentSize)
        => ContentSize = contentSize;

    public void Follow(ISpatialEntity followTarget)
    {
        Require.NotNull(followTarget, nameof(followTarget));

        SizeF cameraSize = Bounds.Size;
        SizeF targetSize = followTarget.Bounds.Size;
        
        var deadZoneSize 
            = new SizeF(cameraSize.Width / 2 - targetSize.Width, cameraSize.Height / 2 - targetSize.Height);
        
        Follow(followTarget, deadZoneSize);
    }

    public void Follow(ISpatialEntity followTarget, SizeF deadZoneSize)
    {
        Require.NotNull(followTarget, nameof(followTarget));

        _deadZoneEngine.Clear();

        _deadZoneEngine.AddCollidable(new ScrollRegion(this, MovementDirection.Up, deadZoneSize));
        _deadZoneEngine.AddCollidable(new ScrollRegion(this, MovementDirection.Left, deadZoneSize));
        _deadZoneEngine.AddCollidable(new ScrollRegion(this, MovementDirection.Right, deadZoneSize));
        _deadZoneEngine.AddCollidable(new ScrollRegion(this, MovementDirection.Down, deadZoneSize));
        _deadZoneEngine.AddCollidable(followTarget);
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
        => GetVirtualViewMatrix(parallaxFactor) * _viewportConnector.GetScaleMatrix();

    /// <summary>
    /// Directly moves the camera to the specified position.
    /// </summary>
    /// <param name="position">The position to center the camera on.</param>
    public void MoveTo(Vector2 position)
    {   // The "Position" property specifies the set of coordinates for the camera's upper-left corner.
        // We can offset the value provided to this function by the center of world-space to shift said
        // value to the center of the camera's view.
        Position = position - Origin;
    }

    /// <summary>
    /// Increases the camera's amount of zoom.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public void ZoomIn(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        Zoom += ZoomSpeed * (float) time.ElapsedGameTime.TotalSeconds;
    }

    /// <summary>
    /// Decreases the camera's amount of zoom.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public void ZoomOut(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        Zoom -= ZoomSpeed * (float) time.ElapsedGameTime.TotalSeconds;
    }

    public void Update()
    {
        _deadZoneEngine.Update();

        if (ContentSize == null)
            return;

        SizeF contentSize = ContentSize.Value;

        RectangleF bounds = Bounds;
        var minimum = Vector2.Zero;
        var maximum = new Vector2(contentSize.Width - bounds.Width, contentSize.Height - bounds.Height);

        Position = Vector2.Clamp(Position, minimum, maximum);
    }
    
    /// <summary>
    /// Creates spatial regions on the edges of this camera's space that will result in the camera scrolling when entities
    /// collide with it.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public IEnumerable<ISpatialEntity> CreateScrollRegions(SizeF size)
        =>
        [
            new ScrollRegion(this, MovementDirection.Up, size),
            new ScrollRegion(this, MovementDirection.Left, size),
            new ScrollRegion(this, MovementDirection.Right, size),
            new ScrollRegion(this, MovementDirection.Down, size)
        ];

    private Matrix GetVirtualViewMatrix()
        => GetVirtualViewMatrix(Vector2.One);

    private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
        => Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0f))
           * Matrix.CreateTranslation(new Vector3(-Origin, 0f))
           * Matrix.CreateRotationZ(Angle)
           * Matrix.CreateScale(Zoom, Zoom, 1f)
           * Matrix.CreateTranslation(new Vector3(Origin, 0f));

    /// <summary>
    /// Provides a spatial entity that will result in the camera scrolling in a specified direction when an
    /// entity collides with it.
    /// </summary>
    private sealed class ScrollRegion : ISpatialEntity
    {
        private readonly Camera _camera;
        private readonly MovementDirection _direction;
        private readonly SizeF _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollRegion"/> class.
        /// </summary>
        /// <param name="camera">The camera that this region scrolls.</param>
        /// <param name="direction">The direction this region will cause the camera to scroll when collided with.</param>
        /// <param name="size">The size of the region in relation to the edge of the camera it occupies.</param>
        public ScrollRegion(Camera camera, MovementDirection direction, SizeF size)
        {
            _camera = camera;
            _direction = direction;
            _size= size;
        }

        /// <inheritdoc/>
        public IShape Bounds
        {
            get
            {
                RectangleF cameraBounds = _camera.Bounds;

                return _direction switch
                {
                    MovementDirection.Up
                        => new RectangleF(cameraBounds.X, cameraBounds.Y + _camera.DeadZoneOffset.Y, cameraBounds.Width, _size.Height),
                    MovementDirection.Left
                        => new RectangleF(cameraBounds.X + _camera.DeadZoneOffset.X,
                                          cameraBounds.Y + _size.Height,
                                          _size.Width,
                                          cameraBounds.Height - _size.Height * 2),
                    MovementDirection.Right
                        => new RectangleF(cameraBounds.Right - _size.Width + _camera.DeadZoneOffset.X,
                                          cameraBounds.Y + _size.Height,
                                          _size.Width,
                                          cameraBounds.Height - _size.Height * 2),
                    MovementDirection.Down
                        => new RectangleF(cameraBounds.X,
                                          cameraBounds.Bottom - _size.Height + _camera.DeadZoneOffset.Y,
                                          cameraBounds.Width,
                                          _size.Height),
                    MovementDirection.None => RectangleF.Empty,
                    _ => throw new InvalidOperationException(Strings.InvalidScrollRegionDirection)
                };
            }
        }

        public IShape PreviousBounds
            => RectangleF.Empty;
        
        /// <inheritdoc/>
        public bool ResolveCollision(IShape shape) 
        {
            while (Bounds.Intersects(shape))
            {
             //   _offset = SizeF.Empty;
                Vector2 penetration = Bounds.CalculatePenetration(shape);
                Vector2 normalizedPenetration = penetration.Normalized();
                Vector2 normalizedDeadZoneOffset = _camera.DeadZoneOffset.Normalized();

                if (Vector2.Dot(normalizedPenetration, normalizedDeadZoneOffset) < 0f)
                {
                    _camera.DeadZoneOffset += penetration;
                    continue;
                }

                _camera.Position += penetration;
                RectangleF cameraBounds = _camera.Bounds;
                
                if (_camera.ContentSize != null)
                {
                    SizeF contentSize = _camera.ContentSize.Value;
                    
                    var minimum = Vector2.Zero;
                    var maximum = contentSize - cameraBounds.Size;

                    if (_camera.Position.LessThanAny(minimum) || _camera.Position.GreaterThanAny(maximum))
                    {
                        Vector2 effectiveOffset = (_camera.DeadZoneOffset * normalizedPenetration).Abs();
                        SizeF displacedSize = _size.Subtract(effectiveOffset); 

                        if (displacedSize.Width <= 1f || displacedSize.Height <= 1f)
                        {
                            _camera.Position -= penetration;
                            return false;
                        }

                        _camera.DeadZoneOffset += penetration;
                        return true;
                    }
                }
            }

            return true;
        }
    }
}
