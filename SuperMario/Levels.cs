using System;
using System.IO;
using System.Collections.Generic;

namespace SuperMario
{
    class Level
    {
        public Mario mario;
        public Luigi? luigi;
        public Sprite[] statics;
        public DynamicSprite[] enemies;

        public Level(Mario mario, Luigi? luigi, Sprite[] statics, DynamicSprite[] enemies)
        {
            this.mario = mario;
            this.luigi = luigi;
            this.statics = statics;
            this.enemies = enemies;
        }
    }

    class LevelUtils
    {
        const int BLOCK_SIZE = 80;
        public const int CANVAS_WIDTH = 1400;
        public const int CANVAS_HEIGHT = 900;

        public static Level LevelFromTxt(string path)
        {

            string wd = Directory.GetCurrentDirectory();
            string file_content = File.ReadAllText(wd + "/" + path);

            Mario mario = new Mario(new Vector2(0, 0), new Vector2(BLOCK_SIZE, BLOCK_SIZE));
            Luigi? luigi = null;
            var statics = new List<Sprite>();
            var enemies = new List<DynamicSprite>();

            var level_dimensions = file_content.Split("\n");
            Array.Reverse(level_dimensions);
            for (int y = 0; y < level_dimensions.Length; y++)
            {
                for (int x = 0; x < level_dimensions[y].Length; x++)
                {
                    switch (level_dimensions[y][x])
                    {
                        case '#':
                            {
                                var groundImage = Utils.ImageFromPath(@"sprites/ground.png");
                                var ground = new Sprite(
                                    new Vector2(BLOCK_SIZE * x, Convert.ToInt32(CANVAS_HEIGHT - (BLOCK_SIZE * (1 + y)))),
                                    new Vector2(BLOCK_SIZE, BLOCK_SIZE),
                                    groundImage, null
                                );

                                statics.Add(ground);
                                break;
                            }
                        case 'M':
                            {
                                mario = new Mario(new Vector2(x * BLOCK_SIZE, CANVAS_HEIGHT - (BLOCK_SIZE * (1 + y))), new Vector2(BLOCK_SIZE, BLOCK_SIZE));
                                break;
                            }
                        case 'L':
                            {
                                luigi = new Luigi(new Vector2(x * BLOCK_SIZE, CANVAS_HEIGHT - (BLOCK_SIZE * (1 + y))), new Vector2(BLOCK_SIZE, BLOCK_SIZE));
                                break;
                            }
                        case 'G':
                            {
                                var goomba = new Goomba(new Vector2(x * BLOCK_SIZE, CANVAS_HEIGHT - (BLOCK_SIZE * (1 + y))), new Vector2(BLOCK_SIZE, BLOCK_SIZE));
                                enemies.Add(goomba);
                                break;
                            }
                    }
                }
            }

            return new Level(mario, luigi, statics.ToArray(), enemies.ToArray());
        }
    }
}