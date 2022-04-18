using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuperMario
{
    enum MarioAnimationState
    {
        Standing,
        Running,
        Jumping,
    }
    class Mario : Sprite, DynamicSprite, AnimatedSprite
    {
        MarioAnimationState AnimState = MarioAnimationState.Standing;
        int MarioRunFrame = 0;
        bool StateChanged = true;
        bool FacingRight = true;
        Vector2 velocity = new(0, 0);
        public Mario(Vector2 pos, Vector2 size) : base(pos, size, Utils.ImageFromPath("sprites/mario_stand.png"), null) { }
        public void Tick()
        {
            int modifier = 1;
            Vector2 vel = new Vector2(0, 0);
            if (this.Ctx!.IsKeyDown(Key.LeftShift) || this.Ctx!.IsKeyDown(Key.RightShift))
                modifier = 3;
            if (this.Ctx!.IsKeyDown(Key.D))
                vel.x += 2 * modifier;
            if (this.Ctx!.IsKeyDown(Key.A))
                vel.x -= 2 * modifier;

            if (vel.x != 0)
            {
                this.StateChanged = true;
                AnimState = MarioAnimationState.Running;
                FacingRight = vel.x > 0;
                MarioRunFrame += 1;
                if (MarioRunFrame > 23)
                    MarioRunFrame = 0;
            }
            else if (this.AnimState != MarioAnimationState.Standing)
            {
                this.StateChanged = true;
                MarioRunFrame = 0;
                AnimState = MarioAnimationState.Standing;
            }

            this.velocity.x = vel.x;
            this.velocity.y = 3;
        }

        public Vector2 Vel()
        {
            return this.velocity;
        }

        public void Animate()
        {
            if (!StateChanged) return;

            string path;
            switch (this.AnimState)
            {
                case MarioAnimationState.Standing:
                    path = "sprites/mario_stand.png";
                    break;
                case MarioAnimationState.Running:
                    int clamped = (int)Math.Floor(MarioRunFrame / 8.0);
                    path = $"sprites/mario_run_{clamped}.png";
                    break;
                default:
                    path = "sprites/mario_stand.png";
                    break;
            }
            ImageSource src = FacingRight ? Utils.BitmapSourceFromPath(path) : Utils.FlippedBitmapSourceFromPath(path);
            Img.Source = src;
        }

    }

}


