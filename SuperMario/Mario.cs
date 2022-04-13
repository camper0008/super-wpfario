using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuperMario {
    enum MarioAnimationState {
        Standing,
        Running,
        Jumping,
    }
    class Mario : Sprite, DynamicSprite, AnimatedSprite {
        MarioAnimationState AnimState = MarioAnimationState.Standing;
        int MarioRunFrame = 0;
        bool StateChanged = true;
        bool FacingRight = true;
        public Mario(Vector2 pos, Vector2 size) : base(pos, size, Utils.ImageFromPath("sprites/mario0.png"), null) { }
        public void Tick() {
            int modifier = 1;
            int movement = 0;
            if (this.Ctx!.IsKeyDown(Key.LeftShift) || this.Ctx!.IsKeyDown(Key.RightShift))
                modifier = 3;
            if (this.Ctx!.IsKeyDown(Key.D))
                movement += 2 * modifier;
            if (this.Ctx!.IsKeyDown(Key.A))
                movement -= 2 * modifier;

            if (movement != 0) {
                this.StateChanged = true;
                AnimState = MarioAnimationState.Running;
                FacingRight = movement > 0;
                MarioRunFrame += 1;
                if (MarioRunFrame > 23)
                    MarioRunFrame = 0;
            } else if (this.AnimState != MarioAnimationState.Standing) {
                this.StateChanged = true;
                MarioRunFrame = 0;
                AnimState = MarioAnimationState.Standing;
            }

            this.Pos.x += movement;
        }

        public void Animate() {
            if (!StateChanged) return;

            string path;
            switch (this.AnimState) {
                case MarioAnimationState.Standing:
                    path = "sprites/mario0.png";
                    break;
                case MarioAnimationState.Running:
                    int clamped = (int)Math.Floor(MarioRunFrame / 8.0);
                    path = $"sprites/mario{clamped + 1}.png";
                    break;
                default:
                    path = "sprites/mario0.png";
                    break;
            }
            ImageSource src = FacingRight ? Utils.BitmapSourceFromPath(path) : Utils.FlippedBitmapSourceFromPath(path);
            Img.Source = src;
        }

    }

}


