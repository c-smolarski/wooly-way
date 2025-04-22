using System;
using System.Collections.Generic;
using Com.IsartDigital.WoolyWay.Data;
using Godot;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.GameObjects
{
    public partial class Obstacle : GameObject
    {
        [Export] private Sprite2D rock;
        [Export] private Sprite2D bush;

        private const float RANDOM_POS_MODIFIER = 10f;
        private const float BUSH_PERCENTAGE = 25f;
        private const string POLYGON_PATH = "Polygon";
        private const int PERCENTAGE = 100;

        private static List<Texture2D> rockTextures = new();
        private static List<Texture2D> bushTextures = new();

        private RandomNumberGenerator rand = new RandomNumberGenerator();

        static Obstacle()
        {
            FilePath.FetchAllFromFile(out rockTextures, FilePath.ROCKS_TEXTURES_DIR);
            FilePath.FetchAllFromFile(out bushTextures, FilePath.BUSHES_TEXTURES_DIR);
        }

        public override void _Ready()
        {
            base._Ready();
            rand.Randomize();

        }
        /// <summary>
        /// Makes the wall transparent if in the outskirt of the map otherwise, it turns into a rock
        /// </summary>
        public void InitWall()
        {
            List<Tile> lNeighbors = Grid.Neighbors(Grid.ObjectDict[this]);
            int lNumWallNeighbors = 0;

            foreach (Tile tile in lNeighbors)
            {
                if (Grid.ObjectDict.Contains(tile) && Grid.ObjectDict[tile] is Obstacle)
                {
                    lNumWallNeighbors++;
                    if (lNumWallNeighbors > 1)
                    {
                        Grid.ObjectDict[this].Visible = false;
                        return;
                    }
                }
            }
            bool lIsRock = rand.Randf() > BUSH_PERCENTAGE / PERCENTAGE;

            rock.Texture = rockTextures[rand.RandiRange(0, rockTextures.Count - 1)];
            bush.Texture = bushTextures[rand.RandiRange(0, bushTextures.Count - 1)];

            bush.Visible = !(rock.Visible = lIsRock);

            bush.Position = rock.Position += new Vector2(
                rand.RandfRange(-1, 1) * RANDOM_POS_MODIFIER,
                rand.RandfRange(-1, 1) * RANDOM_POS_MODIFIER
                );

            GetNode<Polygon2D>(POLYGON_PATH).Visible = true;
        }
    }
}
