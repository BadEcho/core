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

using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a movable camera for viewing orthogonally projected planes.
/// </summary>
public sealed class Camera
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
            new RectangleF(PointF.Empty,
                           new SizeF(parameters.BackBufferWidth * 2,
                                     parameters.BackBufferHeight * 2)));
        
        Origin = new Vector2(_viewportConnector.VirtualSize.Width / 2f, _viewportConnector.VirtualSize.Height / 2f);
    }

    /// <summary>
    /// TODO: Move to public constants class for reference purposes.
    /// </summary>
    public static int DeadZoneCategory
        => 0x10000000;

    /// <summary>
    /// Gets or sets the current drawing location of the camera.
    /// </summary>
    public Vector2 Position
    { get; set; }

    /// <summary>
    /// Gets the amount that the camera is currently being rotated about its point of rotation.
    /// </summary>
    public float Angle
    { get; set; }

    /// <summary>
    /// Gets the bounding rectangle of visible content, with coordinates corresponding to the virtual size of the content,
    /// which is its size prior to any scaling meant to fit it to the render-target surface is applied.
    /// </summary>
    public RectangleF Bounds
        => GetBounds(Position);

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

    private Vector2 DesiredPosition
    { get; set; }

    private RectangleF DesiredBounds
        => GetBounds(DesiredPosition);

    /// <summary>
    /// Restricts the movement of this camera to the specified area.
    /// </summary>
    /// <param name="contentSize">The area this camera can display.</param>
    public void LockToContent(SizeF? contentSize)
        => ContentSize = contentSize;

    /// <summary>
    /// Configures the camera to move with the target so that it is always at the center.
    /// </summary>
    /// <param name="followTarget">The target to follow with the camera.</param>
    public void Follow(Collider followTarget)
    {
        Require.NotNull(followTarget, nameof(followTarget));

        SizeF cameraSize = Bounds.Size;
        SizeF targetSize = followTarget.Bounds.Size;
        
        var deadZoneSize 
            = new SizeF(cameraSize.Width / 2 - targetSize.Width, cameraSize.Height / 2 - targetSize.Height);
        
        Follow(followTarget, deadZoneSize);
    }

    /// <summary>
    /// Configures the camera to move with the target when it collides against one of the camera's bordering
    /// dead zones.
    /// </summary>
    /// <param name="followTarget">The target to follow with the camera.</param>
    /// <param name="deadZoneSize">
    /// The size of the dead zones in relation to the edges of the camera they occupy.
    /// </param>
    public void Follow(Collider followTarget, SizeF deadZoneSize)
    {
        Require.NotNull(followTarget, nameof(followTarget));

        _deadZoneEngine.UnregisterAll();

        _deadZoneEngine.Register(CreateDeadZone(MovementDirection.Up));
        _deadZoneEngine.Register(CreateDeadZone(MovementDirection.Left));
        _deadZoneEngine.Register(CreateDeadZone(MovementDirection.Right));
        _deadZoneEngine.Register(CreateDeadZone(MovementDirection.Down));
            
        _deadZoneEngine.Register(followTarget);

        followTarget.CollidableCategories |= DeadZoneCategory;

        DeadZone CreateDeadZone(MovementDirection direction)
        {
            return new DeadZone(this, direction, deadZoneSize)
                   {
                       CollidableCategories = followTarget.Category
                   };
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
        => GetVirtualViewMatrix(Position * parallaxFactor) * _viewportConnector.GetScaleMatrix();

    /// <summary>
    /// Directly moves the camera to the specified position.
    /// </summary>
    /// <param name="position">The position to center the camera on.</param>
    public void MoveTo(Vector2 position)
    {   // The "Position" property specifies the set of coordinates for the camera's upper-left corner.
        // We can offset the value provided to this function by the center of world-space to shift said
        // value to the center of the camera's view.
        Position = position - Origin;
        DesiredPosition = Position;
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

    /// <summary>
    /// Performs any necessary updates to the position and state of the camera.
    /// </summary>
    public void Update()
    {
        _deadZoneEngine.Update();

        float positionX = MathHelper.Lerp(Position.X, DesiredPosition.X, 0.1f);
        float positionY = MathHelper.Lerp(Position.Y, DesiredPosition.Y, 0.1f);

        Position = new Vector2(positionX, positionY);

        if (ContentSize == null)
            return;

        SizeF contentSize = ContentSize.Value;

        RectangleF bounds = Bounds;
        var minimum = Vector2.Zero;
        var maximum = new Vector2(contentSize.Width - bounds.Width, contentSize.Height - bounds.Height);

        Position = Vector2.Clamp(Position, minimum, maximum);
    }

    private RectangleF GetBounds(Vector2 position)
    {
        // The virtual view matrix will be free of any scaling used to fit the content to the render-target surface.
        Matrix view = GetVirtualViewMatrix(position);

        Matrix viewProjection = view.MultiplyBy2DProjection(_viewportConnector.VirtualSize);

        // We can easily get our bounds by using the corners of a bounding frustum that uses our view * projection matrix.
        var frustum = new BoundingFrustum(viewProjection);
        Vector3[] corners = frustum.GetCorners();
        Vector3 topLeft = corners[0];
        Vector3 bottomRight = corners[2];

        return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    }
    
    private Matrix GetVirtualViewMatrix(Vector2 position)
        => Matrix.CreateTranslation(new Vector3(-position, 0f))
           * Matrix.CreateTranslation(new Vector3(-Origin, 0f))
           * Matrix.CreateRotationZ(Angle)
           * Matrix.CreateScale(Zoom, Zoom, 1f)
           * Matrix.CreateTranslation(new Vector3(Origin, 0f));

    /// <summary>
    /// Provides a collider that will result in the camera moving in a specified direction when a
    /// follow target collides with it.
    /// </summary>
    private sealed class DeadZone : Collider
    {
        private readonly Camera _camera;
        private readonly MovementDirection _direction;
        private readonly SizeF _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadZone"/> class.
        /// </summary>
        /// <param name="camera">The camera this dead zone is for.</param>
        /// <param name="direction">The direction the camera will move when colliding with this dead zone.</param>
        /// <param name="size">The size of the dead zone in relation to the edge of the camera it occupies.</param>
        public DeadZone(Camera camera, MovementDirection direction, SizeF size)
        {
            IsAlwaysDirty = true;
            Category = DeadZoneCategory;

            _camera = camera;
            _direction = direction;
            _size= size;
        }

        /// <inheritdoc/>
        public override IShape Bounds
        {
            get
            {
                RectangleF cameraBounds = _camera.DesiredBounds;

                return _direction switch
                {
                    MovementDirection.Up
                        => new RectangleF(cameraBounds.X + _camera.DeadZoneOffset.X,
                                          cameraBounds.Y + _camera.DeadZoneOffset.Y,
                                          cameraBounds.Width,
                                          _size.Height),
                    MovementDirection.Left
                        => new RectangleF(cameraBounds.X + _camera.DeadZoneOffset.X,
                                          cameraBounds.Y + _size.Height + _camera.DeadZoneOffset.Y,
                                          _size.Width,
                                          cameraBounds.Height - _size.Height * 2),
                    MovementDirection.Right
                        => new RectangleF(cameraBounds.Right - _size.Width + _camera.DeadZoneOffset.X,
                                          cameraBounds.Y + _size.Height + _camera.DeadZoneOffset.Y,
                                          _size.Width,
                                          cameraBounds.Height - _size.Height * 2),
                    MovementDirection.Down
                        => new RectangleF(cameraBounds.X + _camera.DeadZoneOffset.X,
                                          cameraBounds.Bottom - _size.Height + _camera.DeadZoneOffset.Y,
                                          cameraBounds.Width,
                                          _size.Height),
                    MovementDirection.None => RectangleF.Empty,
                    _ => throw new InvalidOperationException(Strings.InvalidDeadZoneDirection)
                };
            }
        }
        
        /// <inheritdoc/>
        public override bool ResolveCollision(Collider collider) 
        {   // May take multiple passes, as dead zone position is based on the effective bounding frustum, not the camera position.
            while (Bounds.Intersects(collider.Bounds))
            {   
                Vector2 penetration = Bounds.CalculatePenetration(collider.Bounds);
                Vector2 normalizedPenetration = penetration.Normalized();
                Vector2 normalizedDeadZoneOffset = _camera.DeadZoneOffset.Normalized();

                // If penetration is pushing against an offset dead zone, we need to move the dead zone back towards its "origin"
                // before adjusting the camera position.
                if (Vector2.Dot(normalizedPenetration, normalizedDeadZoneOffset) < 0f)
                {
                    _camera.DeadZoneOffset += penetration;
                    continue;
                }

                // Move the camera by the amount of penetration into our dead zone.
                _camera.DesiredPosition += penetration;
                RectangleF cameraBounds = _camera.Bounds;
                
                if (_camera.ContentSize != null)
                {   // If dead zone penetration would result in the camera being pushed beyond content size limits, then we want
                    // to instead move the dead zone, allowing the target to enter into the area, until the dead zone is offscreen.
                    SizeF contentSize = _camera.ContentSize.Value;
                    
                    var minimum = Vector2.Zero;
                    var maximum = contentSize - cameraBounds.Size;

                    if (_camera.Position.LessThanAny(minimum) || _camera.Position.GreaterThanAny(maximum))
                    {   // We take into account the direction of penetration when calculating effective dead zone size.
                        Vector2 effectiveOffset = (_camera.DeadZoneOffset * normalizedPenetration).Abs();
                        SizeF displacedSize = _size.Subtract(effectiveOffset); 

                        // If the effective dead zone size is almost nonexistent, then we return false to signal the collision
                        // is not resolved. This should cause the target object to either "bump" into the border of
                        // the camera, preventing traversal, or perhaps cause a transition to another screen to occur.
                        if (displacedSize.Width <= 1f || displacedSize.Height <= 1f)
                        {
                            _camera.DesiredPosition -= penetration;
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
