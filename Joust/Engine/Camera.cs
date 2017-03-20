using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joust.Engine
{
    public class Camera : PositionedObject, ICamera2D
    {
        #region Properties
        public Vector2 Origin { get; set; }
        public Vector2 ScreenCenter { get; protected set; }
        public Matrix Transform { get; set; }
        public IFocusable Focus { get; set; }
        public float MoveSpeed { get; set; }
        #endregion

        public Camera(Game game, IFocusable focused) : base(game)
        {
            Focus = focused;
        }

        public override void Initialize()
        {
            ScreenCenter = new Vector2(Services.WindowHeight / 2, Services.WindowWidth / 2);
            Scale = 1;
            MoveSpeed = 1.25f;

            base.Initialize();
        }

        public override void BeginRun()
        {

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            Origin = ScreenCenter / Scale;
            // Create the Transform used by any
            // spritebatch process
            Transform = Matrix.Identity *
                        Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                        Matrix.CreateRotationZ(RotationInRadians) *
                        Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                        Matrix.CreateScale(new Vector3(Scale));

            // Move the Camera to the position that it needs to go
            Position.X += (Focus.Position.X - Position.X) * MoveSpeed * ElapsedGameTime;
            Position.Y += (Focus.Position.Y - Position.Y) * MoveSpeed * ElapsedGameTime;

            base.Update(gameTime);
        }
        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInView(Vector2 position, Texture2D texture)
        {
            // If the object is not within the horizontal bounds of the screen

            if ((position.X + texture.Width) < (Position.X - Origin.X) || (position.X) > (Position.X + Origin.X))
                return false;

            // If the object is not within the vertical bounds of the screen
            if ((position.Y + texture.Height) < (Position.Y - Origin.Y) || (position.Y) > (Position.Y + Origin.Y))
                return false;

            // In View
            return true;
        }
    }
}
