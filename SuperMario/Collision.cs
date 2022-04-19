

namespace SuperMario
{

    class Hitbox
    {
        public Vector2 pos;
        public Vector2 size;
        public bool Active = true;

        public Hitbox(Vector2 pos, Vector2 size)
        {
            this.pos = pos;
            this.size = size;
        }

        public bool Collides(Hitbox other)
        {
            if (this == other || !this.Active || !other.Active) return false;
            if (this.pos.x < other.pos.x + other.size.x &&
                this.pos.x + this.size.x > other.pos.x &&
                this.pos.y < other.pos.y + other.size.y &&
                this.size.y + this.pos.y > other.pos.y)
            {
                return true;
            }

            return false;
        }

    }

}


