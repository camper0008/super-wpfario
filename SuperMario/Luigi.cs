using System;
using System.Windows.Media;
using System.Windows.Input;

namespace SuperMario
{
    class Luigi : Mario, AnimatedSprite
    {
        public Luigi(Vector2 pos, Vector2 size) : base(pos, size)
        {
            base.JumpKey = Key.Up;
            base.LeftKey = Key.Left;
            base.RightKey = Key.Right;

        }
        new public void Animate()
        {
            string path;
            switch (this.AnimState)
            {
                case PlayerAnimationState.Dead:
                    path = "sprites/luigi_dead.png";
                    break;
                case PlayerAnimationState.Standing:
                    path = "sprites/luigi_stand.png";
                    break;
                case PlayerAnimationState.Running:
                    var clamped = (int)Math.Floor(RunFrame / 4.0);
                    path = $"sprites/luigi_run_{clamped}.png";
                    break;
                case PlayerAnimationState.Jumping:
                    path = $"sprites/luigi_jump.png";
                    break;
                default:
                    path = "sprites/luigi_stand.png";
                    break;
            }
            ImageSource src = FacingRight ? Utils.BitmapSourceFromPath(path) : Utils.FlippedBitmapSourceFromPath(path);
            Img.Source = src;
        }

    }

}


