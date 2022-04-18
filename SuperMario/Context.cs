using System;
using System.Windows.Controls;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Input;

namespace SuperMario
{
    class Context
    {
        readonly Sprite[] renderables;
        readonly DynamicSprite[] dynamics;
        readonly Canvas canvas;
        Vector2 cameraOffset = new Vector2(0, 0);
        Dictionary<Key, bool> keysDown = new Dictionary<Key, bool> { };

        public Context(Sprite[] static_objects, DynamicSprite[] enemies, Mario player, Canvas canvas)
        {
            this.canvas = canvas;

            renderables = new Sprite[static_objects.Length + enemies.Length + 1];
            static_objects.CopyTo(renderables, 0);
            enemies.CopyTo(renderables, static_objects.Length);
            renderables[static_objects.Length + enemies.Length] = player;
            for (int i = 0; i < renderables.Length; i++)
            {
                renderables[i].Ctx = this;
                this.canvas.Children.Add(renderables[i].Img);
            }

            dynamics = new DynamicSprite[enemies.Length + 1];
            dynamics[0] = player;
            enemies.CopyTo(dynamics, 1);
        }

        public void SetKeyDown(Key key)
        {
            this.keysDown[key] = true;
        }
        public void SetKeyUp(Key key)
        {
            this.keysDown[key] = false;
        }
        public bool IsKeyDown(Key key)
        {
            if (this.keysDown.ContainsKey(key))
            {
                return this.keysDown[key];
            };
            return false;
        }

        public void RenderTick()
        {
            renderables[0].Img.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < renderables.Length; i++)
                {
                    var anim = renderables[i] as AnimatedSprite;
                    if (anim != null)
                    {
                        anim.Animate();
                    }

                    Canvas.SetLeft(renderables[i].Img, renderables[i].Pos.x + this.cameraOffset.x);
                    Canvas.SetTop(renderables[i].Img, renderables[i].Pos.y + this.cameraOffset.y);
                }
            });
        }

        public void PhysicsTick()
        {
            for (int i = 0; i < dynamics.Length; i++)
            {
                dynamics[i].Tick();
                ((Sprite)dynamics[i]).Pos.x += dynamics[i].Vel().x;
                ((Sprite)dynamics[i]).Pos.y += dynamics[i].Vel().y;
            }
            PhysicsTickCollisions();
        }

        public void PhysicsTickResolveCollisions(DynamicSprite dyn, Sprite other)
        {
            var dyn_sprite = (Sprite)dyn;
            if ((!dyn_sprite.Hitbox.Collides(other.Hitbox)) || (dyn_sprite == other))
                return;

            Vector2 dist = dyn_sprite.Hitbox.AabbDistance(other.Hitbox);

            float xTimeToCollide = dyn.Vel().x != 0 ? Math.Abs(
                dist.x / dyn.Vel().x
            ) : 0;

            float yTimeToCollide = dyn.Vel().y != 0 ? Math.Abs(
                dist.y / dyn.Vel().y
            ) : 0;

            if (dyn.Vel().x != 0 && dyn.Vel().y == 0)
            {
                // resolve x collision first

                if (dyn_sprite.Hitbox.pos.x > other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x + other.Hitbox.size.x;
                }
                else // if (dyn_sprite.Hitbox.pos.x < other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x - dyn_sprite.Hitbox.size.x;
                }

                if (!dyn_sprite.Hitbox.Collides(other.Hitbox)) return;

                if (dyn_sprite.Hitbox.pos.y > other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y + other.Hitbox.size.y;
                }
                else // if (dyn_sprite.Hitbox.pos.y < other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y - dyn_sprite.Hitbox.size.y;
                }
            }
            else if (dyn.Vel().x == 0 && dyn.Vel().y != 0)
            {
                // resolve y collision first

                if (dyn_sprite.Hitbox.pos.y > other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y + other.Hitbox.size.y;
                }
                else // if (dyn_sprite.Hitbox.pos.y < other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y - dyn_sprite.Hitbox.size.y;
                }

                if (!dyn_sprite.Hitbox.Collides(other.Hitbox)) return;

                if (dyn_sprite.Hitbox.pos.x > other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x + other.Hitbox.size.x;
                }
                else // if (dyn_sprite.Hitbox.pos.x < other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x - dyn_sprite.Hitbox.size.x;
                }
            }
            else if (xTimeToCollide > yTimeToCollide)
            {
                // resolve x collision first

                if (dyn_sprite.Hitbox.pos.x > other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x + other.Hitbox.size.x;
                }
                else // if (dyn_sprite.Hitbox.pos.x < other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x - dyn_sprite.Hitbox.size.x;
                }

                if (!dyn_sprite.Hitbox.Collides(other.Hitbox)) return;

                if (dyn_sprite.Hitbox.pos.y > other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y + other.Hitbox.size.y;
                }
                else // if (dyn_sprite.Hitbox.pos.y < other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y - dyn_sprite.Hitbox.size.y;
                }
            }
            else // if (xTimeToCollide > yTimeToCollide)
            {
                // resolve y collision first

                if (dyn_sprite.Hitbox.pos.y > other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y + other.Hitbox.size.y;
                }
                else // if (dyn_sprite.Hitbox.pos.y < other.Hitbox.pos.y)
                {
                    dyn_sprite.Pos.y = other.Hitbox.pos.y - dyn_sprite.Hitbox.size.y;
                }

                if (!dyn_sprite.Hitbox.Collides(other.Hitbox)) return;

                if (dyn_sprite.Hitbox.pos.x > other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x + other.Hitbox.size.x;
                }
                else // if (dyn_sprite.Hitbox.pos.x < other.Hitbox.pos.x)
                {
                    dyn_sprite.Pos.x = other.Hitbox.pos.x - dyn_sprite.Hitbox.size.x;
                }
            }
        }

        public void PhysicsTickCollisions()
        {
            for (int i = 0; i < dynamics.Length; i++)
            {
                for (int j = 0; j < renderables.Length; j++)
                {
                    PhysicsTickResolveCollisions(dynamics[i], renderables[i]);
                }
            }
        }

        public void Tick()
        {
            while (true)
            {
                PhysicsTick();
                RenderTick();
                Thread.Sleep(16);
            }

        }
    }
}


