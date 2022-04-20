using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuperMario
{
    enum PlayerAnimationState
    {
        Standing,
        Walking,
        Running,
        Jumping,
        Dead,
    }
    class Mario : Sprite, DynamicSprite, AnimatedSprite
    {
        protected PlayerAnimationState AnimState = PlayerAnimationState.Standing;
        protected int RunFrame = 0;
        protected bool FacingRight = true;
        protected Vector2 velocity = new(0, 0);
        protected int TimeFallen = 0;
        protected bool InAir = true;
        protected bool Sprinting = false;
        protected bool StoppedJump = false;
        protected bool Dead = false;
        protected Key JumpKey = Key.W;
        protected Key LeftKey = Key.A;
        protected Key RightKey = Key.D;
        public Mario(Vector2 pos, Vector2 size) : base(pos, size, Utils.ImageFromPath("sprites/mario_stand.png"), null) { }
        public void Kill()
        {
            this.Dead = true;
            this.TimeFallen = 0;
            this.Hitbox.Active = false;
            this.AnimState = PlayerAnimationState.Dead;
        }
        public void Tick()
        {
            if (this.Dead)
            {
                this.TimeFallen++;
                this.Pos.y = this.Pos.y += TimeFallen;
                return;
            }
            this.velocity = new Vector2(0, 0);

            if (this.Ctx!.IsKeyDown(this.JumpKey) && !StoppedJump)
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
            if (this.Ctx!.IsKeyDown(this.RightKey))
                velocity.x += 1 * modifier;
            if (this.Ctx!.IsKeyDown(this.LeftKey))
                velocity.x -= 1 * modifier;


            var collisions = this.Ctx!.CollidingObjects(this.Hitbox);

            this.Hitbox.pos.y++;
            var isStanding = this.Ctx!.CollidingObjects(this.Hitbox).Length > 0;
            this.Hitbox.pos.y--;

            InAir = collisions.Length == 0 && !isStanding;

            if (InAir)
            {
                TimeFallen++;
            }
            else
            {
                TimeFallen = 0;
                StoppedJump = false;
            }

            this.Hitbox.pos.y--;
            var hitRoof = this.Ctx!.CollidingObjects(this.Hitbox).Length > 0;
            this.Hitbox.pos.y++;

            if (hitRoof && !isStanding && InAir)
            {
                TimeFallen = Math.Max(14, TimeFallen);
            }


            this.CheckAnimationState();

            this.Pos.x += velocity.x;
            this.Pos.y += velocity.y;

        }

        void CheckAnimationState()
        {
            if (this.Dead)
            {
                this.AnimState = PlayerAnimationState.Dead;
                return;
            }
            if (velocity.x != 0)
            {
                AnimState = PlayerAnimationState.Running;
                FacingRight = velocity.x > 0;
                if (Sprinting)
                {
                    RunFrame += 2;
                }
                else
                {
                    RunFrame += 1;
                }
                RunFrame %= 12;
            }
            else if (this.AnimState != PlayerAnimationState.Standing)
            {
                RunFrame = 0;
                AnimState = PlayerAnimationState.Standing;
            }
            if (this.InAir)
            {
                this.AnimState = PlayerAnimationState.Jumping;
            }
        }

        public void Animate()
        {
            string path;
            switch (this.AnimState)
            {
                case PlayerAnimationState.Dead:
                    path = "sprites/mario_dead.png";
                    break;
                case PlayerAnimationState.Standing:
                    path = "sprites/mario_stand.png";
                    break;
                case PlayerAnimationState.Running:
                    var clamped = (int)Math.Floor(RunFrame / 4.0);
                    path = $"sprites/mario_run_{clamped}.png";
                    break;
                case PlayerAnimationState.Jumping:
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


