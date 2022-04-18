

namespace SuperMario
{

    class Hitbox
    {
        public Vector2 pos;
        public Vector2 size;

        public Hitbox(Vector2 pos, Vector2 size)
        {
            this.pos = pos;
            this.size = size;
        }

        public bool Collides(Hitbox other)
        {
            if (this == other) return false;
            if (this.pos.x < other.pos.x + other.size.x &&
                this.pos.x + this.size.x > other.pos.x &&
                this.pos.y < other.pos.y + other.size.y &&
                this.size.y + this.pos.y > other.pos.y)
            {
                return true;
            }

            return false;
        }

        public Vector2 AabbDistance(Hitbox other)
        {
            Vector2 dist = new Vector2(0, 0);

            if (this.pos.x < other.pos.x)
            {
                dist.x = other.pos.x - (this.pos.x + this.size.x);
            }
            else //if (this.pos.x > other.pos.x)
            {
                dist.x = this.pos.x - (other.pos.x + other.size.x);
            }

            if (this.pos.y < other.pos.y)
            {
                dist.y = other.pos.x - (this.pos.y + this.size.y);
            }
            else //if (this.pos.y > other.pos.y)
            {
                dist.y = this.pos.y - (other.pos.y + other.size.y);
            }

            return dist;
        }
    }

}


