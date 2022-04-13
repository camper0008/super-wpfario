using System;
using System.Windows.Controls;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Input;

namespace SuperMario {
    class Context {
        readonly Sprite[] renderables;
        readonly DynamicSprite[] dynamics;
        readonly Canvas canvas;
        Vector2 cameraOffset = new Vector2(0, 0);
        Dictionary<Key, bool> keysDown = new Dictionary<Key, bool> { };

        public Context(Sprite[] static_objects, DynamicSprite[] enemies, Mario player, Canvas canvas) {
            this.canvas = canvas;

            renderables = new Sprite[static_objects.Length + enemies.Length + 1];
            static_objects.CopyTo(renderables, 0);
            enemies.CopyTo(renderables, static_objects.Length);
            renderables[static_objects.Length + enemies.Length] = player;
            for (int i = 0; i < renderables.Length; i++) {
                renderables[i].Ctx = this;
                this.canvas.Children.Add(renderables[i].Img);
            }

            dynamics = new DynamicSprite[enemies.Length + 1];
            dynamics[0] = player;
            enemies.CopyTo(dynamics, 1);
        }

        public void SetKeyDown(Key key) {
            this.keysDown[key] = true;
        }
        public void SetKeyUp(Key key) {
            this.keysDown[key] = false;
        }
        public bool IsKeyDown(Key key) {
            if (this.keysDown.ContainsKey(key)) {
                return this.keysDown[key];
            };
            return false;
        }

        public void RenderTick() {
            renderables[0].Img.Dispatcher.Invoke(() => {
                for (int i = 0; i < renderables.Length; i++) {
                    var anim = renderables[i] as AnimatedSprite;
                    if (anim != null) {
                        anim.Animate();
                    }

                    Canvas.SetLeft(renderables[i].Img, renderables[i].Pos.x + this.cameraOffset.x);
                    Canvas.SetTop(renderables[i].Img, renderables[i].Pos.y + this.cameraOffset.y);
                }
            });
        }

        public void PhysicsTick() {
            for (int i = 0; i < dynamics.Length; i++) {
                dynamics[i].Tick();
            }
            PhysicsTickCollisions();
        }

        public void PhysicsTickCollisions() {
            for (int i = 0; i < dynamics.Length; i++) {
                for (int j = 0; j < renderables.Length; j++) {
                    var dyn = dynamics[i];
                    var other = renderables[j];
                }
            }
        }

        public void Tick() {
            while (true) {
                PhysicsTick();
                RenderTick();
                Thread.Sleep(16);
            }

        }
    }
}


