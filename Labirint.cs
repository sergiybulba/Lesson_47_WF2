using System;
using System.Windows.Forms;
using System.Drawing;

namespace Maze
{
    class Labirint
    {
        // позиция главного персонажа
        public int CharX { get; set; }
        public int CharY { get; set; }
        public int StartX { get; set; } // точка входу в лабіринт
        public int StartY { get; set; }
        public int countFormMedal = 0; public int countCharMedal = 0;   // медалі на формі і медалі, зібрані героєм
        public int health = 100;    // здоров'я

        int height; // высота лабиринта (количество строк)
        int width; // ширина лабиринта (количество столбцов в каждой строке)

        public MazeObject[,] objects;

        public PictureBox[,] images;

        public static Random r = new Random();

        public Form parent;

        public Labirint(Form parent, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.parent = parent;

            objects = new MazeObject[height, width];
            images = new PictureBox[height, width];

            CharX = StartX = 0;
            CharY = StartY = 2;

            Generate();
        }

        void Generate()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    MazeObject.MazeObjectType current = MazeObject.MazeObjectType.HALL;

                    // в 1 случае из 5 - ставим стену
                    if (r.Next(5) == 0)
                    {
                        current = MazeObject.MazeObjectType.WALL;
                    }

                    // в 1 случае из 50 - кладём медаль
                    if (r.Next(50) == 0)
                    {
                        current = MazeObject.MazeObjectType.MEDAL;
                    }

                    // в 1 случае из 50 - размещаем врага
                    if (r.Next(50) == 0)
                    {
                        current = MazeObject.MazeObjectType.ENEMY;
                    }

                    // в 1 случае из 25 - розміщуємо ліки
                    if (r.Next(25) == 0)
                    {
                        current = MazeObject.MazeObjectType.HEALTH;
                    }

                    // стены по периметру обязательны
                    if (y == 0 || x == 0 || y == height - 1 | x == width - 1)
                    {
                        current = MazeObject.MazeObjectType.WALL;
                    }

                    // наш персонажик
                    if (x == CharX && y == CharY)
                    {
                        current = MazeObject.MazeObjectType.CHAR;
                    }

                    // есть выход, и соседняя ячейка справа всегда свободна
                    if (x == CharX + 1 && y == CharY || x == width - 1 && y == height - 3)
                    {
                        current = MazeObject.MazeObjectType.HALL;
                    }

                    objects[y, x] = new MazeObject(current);
                    images[y, x] = new PictureBox();
                    images[y, x].Location = new Point(x * objects[y, x].width, y * objects[y, x].height);
                    images[y, x].Parent = parent;
                    images[y, x].Width = objects[y, x].width;
                    images[y, x].Height = objects[y, x].height;
                    images[y, x].BackgroundImage = objects[y, x].texture;
                }
            }

            for (int y = 0; y < height; y++)        // підрахунок медальок на формі
            {
                for (int x = 0; x < width; x++)
                {
                    if (objects[y, x].texture == MazeObject.images[(int)MazeObject.MazeObjectType.MEDAL])
                        countFormMedal++;
                }
            }
        }
    }
}
