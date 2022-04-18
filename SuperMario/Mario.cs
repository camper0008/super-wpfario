using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuperMario
{
    enum MarioAnimationState
    {
        Standing,
        Walking,
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
        int TimeFallen = 0;
        bool InAir = true;
        bool Sprinting = false;
        bool StoppedJump = false;
        public Mario(Vector2 pos, Vector2 size) : base(pos, size, Utils.ImageFromPath("sprites/mario_stand.png"), null) { }
        public void Tick()
        {
            this.velocity = new Vector2(0, 0);

            if (this.Ctx!.IsKeyDown(Key.W) && !StoppedJump)
            {
                velocity.y -= 3 * (14 - TimeFallen);
            }
            else
            {
                StoppedJump = true;
                velocity.y += Math.Max(0, Math.Min(40, 3 * (TimeFallen - 5)));
            }

            int modifier = 6;
            Sprinting = false;
            if (this.Ctx!.IsKeyDown(Key.LeftShift) || this.Ctx!.IsKeyDown(Key.RightShift))
            {
                modifier = 24;
                Sprinting = true;
            }
            if (this.Ctx!.IsKeyDown(Key.D))
                velocity.x += 1 * modifier;
            if (this.Ctx!.IsKeyDown(Key.A))
                velocity.x -= 1 * modifier;


            var collisions = this.Ctx!.CollidingObjects(this.Hitbox);

            this.Hitbox.pos.y++;
            var isStanding = this.Ctx!.CollidingObjects(this.Hitbox);
            this.Hitbox.pos.y--;

            InAir = collisions.Length == 0 && isStanding.Length == 0;

            if (InAir)
            {
                TimeFallen++;
            }
            else
            {
                TimeFallen = 0;
                StoppedJump = false;
            }

            this.CheckAnimationState();

            this.Pos.x += velocity.x;
            this.Pos.y += velocity.y;

        }

        void CheckAnimationState()
        {
            if (velocity.x != 0)
            {
                this.StateChanged = true;
                AnimState = MarioAnimationState.Running;
                FacingRight = velocity.x > 0;
                if (Sprinting)
                {
                    MarioRunFrame += 2;
                }
                else
                {
                    MarioRunFrame += 1;
                }
                MarioRunFrame %= 12;
            }
            else if (this.AnimState != MarioAnimationState.Standing)
            {
                this.StateChanged = true;
                MarioRunFrame = 0;
                AnimState = MarioAnimationState.Standing;
            }

            if (this.InAir)
            {
                this.AnimState = MarioAnimationState.Jumping;
            }
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
                    var clamped = (int)Math.Floor(MarioRunFrame / 4.0);
                    path = $"sprites/mario_run_{clamped}.png";
                    break;
                case MarioAnimationState.Jumping:
                    path = $"sprites/mario_jump.png";
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


