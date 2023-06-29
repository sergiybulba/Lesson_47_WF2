/* WinForms, homework № 3, 17.06.2023												
												
task № 1: Labirint	
 
1. Реалізувати переміщення персонажа лабіринтом стрілками. Персонаж не повинен проходити крізь стіни. 
Якщо персонаж дійшов до виходу з лабіринту в правому нижньому кутку - гра закінчується перемогою 
(вивести діалог з повідомленням "перемога - знайдений вихід").

2. Реалізувати підрахунок зібраних медалей, кількість медалей у наявності виводити в заголовок вікна 
(починаючи з нуля медалей). Якщо всі медалі лабіринту зібрані – гра закінчується перемогою (вивести діалог із
повідомленням "перемога – медалі зібрані").

3. Реалізувати систему "здоров'я персонажа": спочатку здоров'я лише на рівні 100% (виводити поточний стан 
здоров'я в заголовок вікна). Перетин з кожним ворогом забирає від 20 до 25% здоров'я, причому ворог зникає. 
Додати новий тип об'єктів лабіринту - "ліки", який при зборі поправляє здоров'я на 5%. Здоров'я персонажа не 
може бути більше 100%, тобто якщо здоров'я вже на максимумі, то ліки не можна підібрати. Якщо здоров'я 
закінчилося (упало до 0%) - гра закінчується поразкою (вивести діалог із повідомленням "ураження - закінчилося здоров'я").

4. До проекту гри Лабіринт додати StatusStrip на форму. Вивести у статус-стрип кількість очок здоров'я, 
ігровий час (скільки часу запущено рівень), кількість кроків, які зробив ГГ (колобок).
 */


using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Maze
{
    public partial class Form1 : Form
    {
        // размеры лабиринта в ячейках 16х16 пикселей
        int columns = 20;
        int rows = 20;

        int pictureSize = 16; // ширина и высота одной ячейки

        Labirint l; // ссылка на логику всего происходящего в лабиринте

        public Form1()
        {
            InitializeComponent();
            Options();
            StartGame();
            timer1.Start();
        }

        public void Options()
        {
            Text = "Maze";
            BackColor = Color.FromArgb(255, 92, 118, 137);

            // размеры клиентской области формы (того участка, на котором размещаются ЭУ)
            ClientSize = new Size(columns * pictureSize, rows * pictureSize + statusStrip1.Height);

            StartPosition = FormStartPosition.CenterScreen;
        }

        public void StartGame()
        {
            l = new Labirint(this, columns, rows);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)                // хід вправо
            {
                // проверка на то, свободна ли ячейка справа
                if (l.objects[l.CharY, l.CharX + 1].type == MazeObject.MazeObjectType.MEDAL) // проверяем ячейку правее на 1 позицию, является ли она медалькою
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharX++;
                    l.countCharMedal++;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY, l.CharX + 1].type == MazeObject.MazeObjectType.ENEMY) // проверяем ячейку правее на 1 позицию, является ли она ворогом
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharX++;
                    l.health -= 20;     // зустріч з ворогом - мінус 20 % здоров'я
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY, l.CharX + 1].type == MazeObject.MazeObjectType.HEALTH) // проверяем ячейку правее на 1 позицию, является ли она ліками
                {
                    if (l.health < 100)     // якщо здоров'я 100 % - ліки не дозволяються
                    {
                        SwapHall();         // зміна поточної клітинки на hall
                        l.CharX++;
                        if (l.health + 5 > 100)    // приймання ліків - плюс 5 % здоров'я
                            l.health = 100;     
                        else 
                            l.health += 5;
                        SwapChar();         // зміна сусідньої клітинки на char
                    }
                }
                else if (l.objects[l.CharY, l.CharX + 1].type == MazeObject.MazeObjectType.HALL) // проверяем ячейку правее на 1 позицию, является ли она коридором
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharX++;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                Text = "Maze  medal(s): f-" + l.countFormMedal + ", c-" + l.countCharMedal;
                CheckHealth();          // перевірка стану здоров'я
                CheckMedalWin();        // перевірка на виграш коли зібрані всі медальки
                CheckFinishWin();       // перевірка на виграш коли знайдено вихід
            }


            else if (e.KeyCode == Keys.Left)                // хід вліво
            {
                if (l.objects[l.CharY, l.CharX - 1].type == MazeObject.MazeObjectType.MEDAL) // проверяем ячейку левее на 1 позицию, является ли она медалькою
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharX--;
                    l.countCharMedal++;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY, l.CharX - 1].type == MazeObject.MazeObjectType.ENEMY) // проверяем ячейку правее на 1 позицию, является ли она ворогом
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharX--;
                    l.health -= 20;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY, l.CharX - 1].type == MazeObject.MazeObjectType.HEALTH) // проверяем ячейку правее на 1 позицию, является ли она ліками
                {
                    if (l.health < 100)
                    {
                        SwapHall();         // зміна поточної клітинки на hall
                        l.CharX--;
                        if (l.health + 5 > 100)
                            l.health = 100;
                        else
                            l.health += 5;
                        SwapChar();         // зміна сусідньої клітинки на char
                    }
                }
                else if (l.objects[l.CharY, l.CharX - 1].type == MazeObject.MazeObjectType.HALL) // проверяем ячейку левее на 1 позицию, является ли она коридором
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharX--;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                Text = "Maze  medal(s): f-" + l.countFormMedal + ", c-" + l.countCharMedal;
                CheckHealth();
                CheckMedalWin();
                CheckFinishWin();
            }


            else if (e.KeyCode == Keys.Up)                // хід вверх
            {
                if (l.objects[l.CharY - 1, l.CharX].type == MazeObject.MazeObjectType.MEDAL) // проверяем ячейку вверх на 1 позицию, является ли она медалькою
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharY--;
                    l.countCharMedal++;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY - 1, l.CharX].type == MazeObject.MazeObjectType.ENEMY) // проверяем ячейку правее на 1 позицию, является ли она ворогом
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharY--;
                    l.health -= 20;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY - 1, l.CharX].type == MazeObject.MazeObjectType.HEALTH) // проверяем ячейку правее на 1 позицию, является ли она ліками
                {
                    if (l.health < 100)
                    {
                        SwapHall();         // зміна поточної клітинки на hall
                        l.CharY--;
                        if (l.health + 5 > 100)
                            l.health = 100;
                        else
                            l.health += 5;
                        SwapChar();         // зміна сусідньої клітинки на char
                    }
                }
                else if (l.objects[l.CharY - 1, l.CharX].type == MazeObject.MazeObjectType.HALL) // проверяем ячейку вверх на 1 позицию, является ли она коридором
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharY--;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                Text = "Maze  medal(s): f-" + l.countFormMedal + ", c-" + l.countCharMedal;
                CheckHealth();
                CheckMedalWin();
                CheckFinishWin();
            }


            else if (e.KeyCode == Keys.Down)                // хід вниз
            {
                if (l.objects[l.CharY + 1, l.CharX].type == MazeObject.MazeObjectType.MEDAL) // проверяем ячейку вниз на 1 позицию, является ли она медалькою
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharY++;
                    l.countCharMedal++;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY + 1, l.CharX].type == MazeObject.MazeObjectType.ENEMY) // проверяем ячейку правее на 1 позицию, является ли она ворогом
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharY++;
                    l.health -= 20;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                else if (l.objects[l.CharY + 1, l.CharX].type == MazeObject.MazeObjectType.HEALTH) // проверяем ячейку правее на 1 позицию, является ли она ліками
                {
                    if (l.health < 100)
                    {
                        SwapHall();         // зміна поточної клітинки на hall
                        l.CharY++;
                        if (l.health + 5 > 100)
                            l.health = 100;
                        else
                            l.health += 5;
                        SwapChar();         // зміна сусідньої клітинки на char
                    }
                }
                else if (l.objects[l.CharY + 1, l.CharX].type == MazeObject.MazeObjectType.HALL) // проверяем ячейку вниз на 1 позицию, является ли она коридором
                {
                    SwapHall();         // зміна поточної клітинки на hall
                    l.CharY++;
                    SwapChar();         // зміна сусідньої клітинки на char
                }
                Text = "Maze  medal(s): f-" + l.countFormMedal + ", c-" + l.countCharMedal;
                CheckHealth();
                CheckMedalWin();
                CheckFinishWin();
            }
            if (l.steps == 1)               // запуск секундоміра гри
                l.timeStart = System.DateTime.Now;
        }

        private void CheckHealth()           // перевірка чи в героя залишилося здоров'я
        {
            if (l.health <= 0)
            {
                toolStripStatusLabel2.Text = "steps: " + l.steps + ",";     // відображення останнього кроку
                timer1.Stop();
                MessageBox.Show("Oops... You lost! There's no health");
                this.Close();
            }
        }
        private void CheckFinishWin()           // перевірка чи досяг герой виходу з лабіринта
        {
            if ((l.CharX == 0 || l.CharX == columns-1 || l.CharY == 0 || l.CharY == rows - 1) // якщо досягнуто краю лабіринту, і ця клітинка - не стартова
                && (l.CharX != l.StartX && l.CharY != l.StartY))
            {
                toolStripStatusLabel2.Text = "steps: " + l.steps + ",";     // відображення останнього кроку
                timer1.Stop(); 
                MessageBox.Show("Congratulation! You won! Exit has found!");
                this.Close();
            }
        }
        private void CheckMedalWin()           // перевірка чи зібрав герой всі медальки
        {
            if (l.countFormMedal != 0 && l.countCharMedal == l.countFormMedal)
            {
                toolStripStatusLabel2.Text = "steps: " + l.steps + ",";     // відображення останнього кроку
                timer1.Stop(); 
                MessageBox.Show("Congratulation! You won! Medals are collected!");
                this.Close();
            }
        }
        private void SwapHall()                 // метод для зміни поточної клітинки на hall - замість char
        {
            l.objects[l.CharY, l.CharX].texture = MazeObject.images[(int)MazeObject.MazeObjectType.HALL]; // hall
            l.images[l.CharY, l.CharX].BackgroundImage = l.objects[l.CharY, l.CharX].texture;
            l.objects[l.CharY, l.CharX].type = MazeObject.MazeObjectType.HALL;
            l.steps++;      // підрахунок кроків
        }
        private void SwapChar()                 // метод для зміни поточної клітинки на char
        {
            l.objects[l.CharY, l.CharX].texture = MazeObject.images[(int)MazeObject.MazeObjectType.CHAR]; // character
            l.images[l.CharY, l.CharX].BackgroundImage = l.objects[l.CharY, l.CharX].texture;
            l.objects[l.CharY, l.CharX].type = MazeObject.MazeObjectType.CHAR;
        }

        private void timer1_Tick(object sender, System.EventArgs e)         // подія таймеру - відображення інформація statusStrip 
        {
            toolStripStatusLabel1.Text = "Health: " + l.health + "%," ;
            toolStripStatusLabel2.Text = "steps: " + l.steps + ",";
            if (l.steps >= 1)
                toolStripStatusLabel3.Text = "time: " + (System.DateTime.Now - l.timeStart).Seconds + " seconds";
        }
    }
}
