

namespace SuperMario {

    class Hitbox {
        public Vector2 pos;
        Vector2 size;
        bool active;

        public Hitbox(Vector2 pos, Vector2 size) {
            this.pos = pos;
            this.size = size;
            this.active = true;
        }

        public bool Collides(Hitbox other) {
            if (!this.active || !other.active) {
                return false;
            }

            if (this.pos.x < other.pos.x + other.size.x &&
                this.pos.x + this.size.x > other.pos.x &&
                this.pos.y < other.pos.y + other.size.y &&
                this.size.y + this.pos.y > other.pos.y) {
                return true;
            }

            return false;
        }
    }

}


