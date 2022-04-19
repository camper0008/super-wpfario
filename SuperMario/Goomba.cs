using System.Windows.Media;

namespace SuperMario
{
    class Goomba : Sprite, DynamicSprite, AnimatedSprite
    {
        bool Dead = false;
        int RunFrame = 0;
        Vector2 velocity = new(0, 0);
        bool FacingRight = true;
        int TimeFallen = 0;
        bool InAir = true;
        public Goomba(Vector2 pos, Vector2 size) : base(pos, size, Utils.ImageFromPath("sprites/goomba_0.png"), null) { }

        void Kill()
        {
            Dead = true;
            this.Hitbox.Active = false;
            this.Img.Dispatcher.Invoke(() => {
                this.Img.RenderTransform = new RotateTransform(180);
            });
        }

        public void Tick()
        {
            if (this.Dead)
            {
                this.TimeFallen++;
                this.Pos.y += TimeFallen;
                return;
            }

            this.velocity = new Vector2(0, TimeFallen);

            if (this.FacingRight)
            {
                this.Hitbox.pos.x++;
                FacingRight = this.Ctx!.CollidingObjects(this.Hitbox).Length == 0;
                this.Hitbox.pos.x--;
            }
            else
            {
                this.Hitbox.pos.x--;
                FacingRight = !(this.Ctx!.CollidingObjects(this.Hitbox).Length == 0);
                this.Hitbox.pos.x++;
            }

            if (this.FacingRight)
                velocity.x += 3;
            else
                velocity.x -= 3;

            var colliding = this.Ctx!.CollidingObjects(this.Hitbox).Length > 0;
            this.Hitbox.pos.y++;
            var standing = this.Ctx!.CollidingObjects(this.Hitbox).Length > 0;
            this.Hitbox.pos.y--;

            InAir = !colliding && !standing;

            if (InAir)
            {
                TimeFallen++;
            }
            else
            {
                TimeFallen = 0;
            }

            if (Ctx!.mario.Hitbox.Collides(this.Hitbox))
            {
                var KillMarioHitbox = new Hitbox(
                    new Vector2(this.Hitbox.pos.x, this.Hitbox.pos.y + (int)(this.Hitbox.size.y * 0.4)),
                    new Vector2(this.Hitbox.size.x, (int)(this.Hitbox.size.y * 0.6))
                );

                if (Ctx!.mario.Hitbox.Collides(KillMarioHitbox))
                {
                    // kill mario
                }
                else
                {
                    // kill goomba
                    this.Kill();
                }

            }

            this.Hitbox.pos.y--;
            var hitRoof = this.Ctx!.CollidingObjects(this.Hitbox).Length > 0;
            this.Hitbox.pos.y++;

            this.CheckAnimationState();

            this.Pos.x += velocity.x;
            this.Pos.y += velocity.y;

        }

        void CheckAnimationState()
        {
            RunFrame++;
            if (RunFrame > 15)
                RunFrame = 0;
        }

        public void Animate()
        {
            var clamped = (int)(RunFrame / 8);
            var path = $"sprites/goomba_{clamped}.png";
            ImageSource src = Utils.BitmapSourceFromPath(path);
            Img.Source = src;
        }

    }

}


