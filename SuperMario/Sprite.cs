using System.Windows.Controls;

namespace SuperMario
{
    class Sprite
    {
        Vector2 pos;
        Context? ctx;
        Hitbox hitbox;
        Image img;

        public Sprite(Vector2 pos, Vector2 size, Image img, Hitbox? hitbox)
        {
            this.hitbox = hitbox ?? new Hitbox(pos, size);
            this.pos = pos;
            this.img = img;
            img.Width = size.x;
            img.Height = size.y;
        }

        public Vector2 Pos
        {
            get { return pos; }
            set { pos = value; hitbox.pos = value; }
        }
        public Context Ctx
        {
            get { return ctx!; }
            set { ctx = value; }
        }
        public Image Img
        {
            get { return img; }
            set { img = value; }
        }
        public Hitbox Hitbox
        {
            get { return hitbox; }
        }
    }

    interface DynamicSprite
    {
        public abstract Vector2 Vel();
        public abstract void Tick();
    }
    interface AnimatedSprite
    {
        public abstract void Animate();

    }
}


