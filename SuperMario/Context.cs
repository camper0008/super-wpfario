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
        public readonly Mario mario;
        public readonly Luigi? luigi;
        readonly DynamicSprite[] dynamics;
        readonly Canvas canvas;
        Vector2 cameraOffset = new Vector2(0, 0);
        Dictionary<Key, bool> keysDown = new Dictionary<Key, bool> { };

        Sprite[] InitializeRenderables(Sprite[] statics, DynamicSprite[] dynamics, Mario mario, Luigi? luigi)
        {
            var renderables = new Sprite[
                statics.Length +
                dynamics.Length +
                1 +
                (luigi != null ? 1 : 0)
            ];

            statics.CopyTo(renderables, 0);
            dynamics.CopyTo(renderables, statics.Length);
            renderables[statics.Length + dynamics.Length] = mario;
            if (luigi != null)
                renderables[statics.Length + dynamics.Length + 1] = luigi;

            for (int i = 0; i < renderables.Length; i++)
            {
                renderables[i].Ctx = this;
                this.canvas.Children.Add(renderables[i].Img);
            }

            return renderables;
        }
        DynamicSprite[] InitializeDynamics(DynamicSprite[] npds, Mario mario, Luigi? luigi)
        {
            var res = new DynamicSprite[
                npds.Length +
                1 +
                (luigi != null ? 1 : 0)
            ];
            res[0] = mario;
            if (luigi != null)
                res[1] = luigi;

            npds.CopyTo(res, 1 + (luigi != null ? 1 : 0));

            return res;

        }
        public Context(Sprite[] statics, DynamicSprite[] npds, Canvas canvas, Mario mario, Luigi? luigi)
        {
            this.canvas = canvas;
            this.renderables = this.InitializeRenderables(statics, npds, mario, luigi);
            this.dynamics = InitializeDynamics(npds, mario, luigi);

            this.mario = mario;
            this.luigi = luigi;
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

        public Hitbox[] CollidingObjects(Hitbox hitbox)
        {
            var res = new List<Hitbox>();

            for (int i = 0; i < this.renderables.Length; i++)
            {
                if (hitbox.Collides(renderables[i].Hitbox))
                {
                    res.Add(renderables[i].Hitbox);
                }
            }

            return res.ToArray();
        }

        public void RenderTick()
        {
            this.cameraOffset.x = Math.Max(this.cameraOffset.x, this.mario.Pos.x + (int)(this.mario.Hitbox.size.x * 0.5) - 700);
            renderables[0].Img.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < renderables.Length; i++)
                {
                    var anim = renderables[i] as AnimatedSprite;
                    if (anim != null)
                    {
                        anim.Animate();
                    }

                    Canvas.SetLeft(renderables[i].Img, renderables[i].Pos.x - this.cameraOffset.x);
                    Canvas.SetTop(renderables[i].Img, renderables[i].Pos.y - this.cameraOffset.y);
                }
            });
        }

        public void PhysicsTick()
        {
            for (int i = 0; i < dynamics.Length; i++)
            {
                dynamics[i].Tick();
            }
            PhysicsTickCollisions();
        }

        public void PhysicsTickResolveCollisions(DynamicSprite dyn, Sprite other)
        {
            var dyn_sprite = (Sprite)dyn;
            if ((!dyn_sprite.Hitbox.Collides(other.Hitbox)))
                return;

            int xWorkToResolve;
            int xResolved;
            if (dyn_sprite.Hitbox.pos.x > other.Hitbox.pos.x + other.Hitbox.size.x * 0.5)
            {
                xResolved = (other.Hitbox.pos.x + other.Hitbox.size.x);
                xWorkToResolve = xResolved - dyn_sprite.Hitbox.pos.x;
            }
            else // if (dyn_sprite.Hitbox.pos.x < other.Hitbox.pos.x)
            {
                xResolved = (other.Hitbox.pos.x - dyn_sprite.Hitbox.size.x);
                xWorkToResolve = dyn_sprite.Hitbox.pos.x - xResolved;
            }

            int yWorkToResolve;
            int yResolved;
            if (dyn_sprite.Hitbox.pos.y > other.Hitbox.pos.y + other.Hitbox.size.y * 0.5)
            {
                yResolved = (other.Hitbox.pos.y + other.Hitbox.size.y);
                yWorkToResolve = yResolved - dyn_sprite.Hitbox.pos.y;
            }
            else // if (dyn_sprite.Hitbox.pos.x < other.Hitbox.pos.x)
            {
                yResolved = (other.Hitbox.pos.y - dyn_sprite.Hitbox.size.y);
                yWorkToResolve = dyn_sprite.Hitbox.pos.y - yResolved;
            }


            if (Math.Abs(xWorkToResolve) < Math.Abs(yWorkToResolve))
            {
                // resolve x first, since it would be the easiest fix

                dyn_sprite.Pos.x = xResolved;

                if (!dyn_sprite.Hitbox.Collides(other.Hitbox)) return;

                dyn_sprite.Pos.y = yResolved;
            }
            else // if (yWorkToResolve < xWorkToResolve)
            {
                // resolve y first

                dyn_sprite.Pos.y = yResolved;

                if (!dyn_sprite.Hitbox.Collides(other.Hitbox)) return;

                dyn_sprite.Pos.x = xResolved;
            }

        }

        public void PhysicsTickCollisions()
        {
            if (mario.Hitbox.pos.x < cameraOffset.x)
                mario.Pos.x = cameraOffset.x;
            for (int dyn_idx = 0; dyn_idx < dynamics.Length; dyn_idx++)
            {
                for (int ren_idx = 0; ren_idx < renderables.Length; ren_idx++)
                {
                    PhysicsTickResolveCollisions(dynamics[dyn_idx], renderables[ren_idx]);
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


