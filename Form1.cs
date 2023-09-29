using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Game_NuPogodi
{
    public partial class Form1 : Form
    {
        public static int result = 0; //значение которое будет передано в таблицу рекордов по окончанию игры

        int player_pos = 1;
        int unused_pos = 0; //неиспользованный лоток

        int collected_eggs = 0;
        double dropped_eggs = 0;

        double prev_egg_value = 0; //если предыдущее кол-во упавших яйц равно текущему то некоторые условия не будут проверяться

        int zayac_chance = 15; //шанс появления зайца в процентах, проверяется при появлении яйца

        bool isEggBlinking1 = false;
        bool isEggBlinking2 = false;
        bool isEggBlinking3 = false;

        int delay = 1000; //задержка до появления следующего яйца
        int move_delay = 1000; //задержка между смещениями яйца
        int zayac_delay = 5000; //время в течение которого заяц показывается

        double zayac_multiplier = 1; //множитель очков связанный с появлением зайца
        int final_score = 0;
            
        int delta_x = 18; //смещение яйца по х
        int delta_y = 8; //смещение яйца по y

        bool isGameRunning = false;

        Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
        }
            
        private void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves"));

            string filename = "save.txt";
            string full_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"saves\", filename); // путь до файла сохранения

            if (!File.Exists(full_path)) //создать файл и заполнить его если он не существует
            {
                FileStream fs = File.Create(full_path);

                string[] temp = new string[10];
                for (int i = 0; i < 10; i++)
                {
                    temp[i] = "player 0 ";
                }

                fs.Close();
                File.AppendAllLines(full_path, temp);
            }

            var pb1 = new PictureBox(); //рамка около игрового поля
            pb1.BackColor = Color.Transparent;
            pb1.Location = new Point(267, 83);
            pb1.Size = new Size(730, 520);
            pb1.Image = Properties.Resources.game_title2;
            this.Controls.Add(pb1);
            pb1.SendToBack();            
        }

        private void checkDIsplayChange()
        {
            if (prev_egg_value != dropped_eggs)
            {
                switch (dropped_eggs)
                {
                    case 0.5:
                        isEggBlinking1 = true;
                        break;
                    case 1.0:
                        isEggBlinking1 = false;
                        egg1.Image = Properties.Resources.crashed_egg;
                        break;
                    case 1.5:
                        isEggBlinking1 = false;
                        isEggBlinking2 = true;
                        egg1.Image = Properties.Resources.crashed_egg;
                        break;
                    case 2.0:
                        isEggBlinking2 = false;
                        egg2.Image = Properties.Resources.crashed_egg;
                        break;
                    case 2.5:
                        isEggBlinking2 = false;
                        isEggBlinking3 = true;
                        egg2.Image = Properties.Resources.crashed_egg;
                        break;
                    case 3.0:
                        isEggBlinking3 = false;
                        egg3.Image = Properties.Resources.crashed_egg;
                        break;
                    case 3.5:
                        isEggBlinking3 = false;
                        egg3.Image = Properties.Resources.crashed_egg;
                        break;
                }
                checkBlinkingEgg();
            }
        }
        private async void checkBlinkingEgg()
        {
            int delay_blink = 500;

            while (isEggBlinking1 == true)
            {
                egg1.Image = Properties.Resources.crashed_egg_50;
                await Task.Delay(delay_blink);
                egg1.Image = Properties.Resources.crashed_egg;
                await Task.Delay(delay_blink);

            } 
            while (isEggBlinking2 == true)
            {
                egg2.Image = Properties.Resources.crashed_egg_50;
                await Task.Delay(delay_blink);
                egg2.Image = Properties.Resources.crashed_egg;
                await Task.Delay(delay_blink);
            }
            while (isEggBlinking3 == true)
            {
                egg3.Image = Properties.Resources.crashed_egg_50;
                await Task.Delay(delay);
                egg3.Image = Properties.Resources.crashed_egg;
                await Task.Delay(delay);
            }
            prev_egg_value = dropped_eggs;
        }

        private async void egg_Procession(int pos) //создание и передвижение яиц
        {
            int x = 0;
            int y = 0;

            while (pos == unused_pos) //перебор позиции до тех пор пока она не будет отличаться от неиспользуемого лотка в игре А
            {
                int random_pos = (rnd.Next() % 4 + 1);
                pos = random_pos;
            }

            switch (pos)
            {
                case 1:
                    x = 372;
                    y = 289;
                    break;
                case 2:
                    x = 372;
                    y = 384;
                    break;
                case 3:
                    x = 865;
                    y = 290;
                    break;
                case 4:
                    x = 865;
                    y = 385;
                    break;
            }

            var pb = new PictureBox();
            pb.Image = Properties.Resources.new_egg;
            pb.Location = new Point(x, y);
            pb.Size = new Size(24, 20);
            pb.BackColor = Color.Transparent;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pb);
            pb.BringToFront();

            if (pos == 1 || pos == 2)
            {
                for (int i = 1; i < 6; i++)
                {
                    await Task.Delay(move_delay);
                    pb.Location = new Point(x + delta_x * i, y + delta_y * i);
                }
                await Task.Delay(move_delay);
            } else {
                for (int i = 1; i < 6; i++)
                {
                    await Task.Delay(move_delay);
                    pb.Location = new Point(x - delta_x * i, y + delta_y * i);
                }
                await Task.Delay(move_delay);
            }

            if (isGameRunning)
            {
                if (pos == player_pos)
                {
                    collected_eggs += 1;
                    label1.Text = collected_eggs.ToString();
                    pb.Dispose();
                } else {
                    dropped_eggs += zayac_multiplier;
                    label2.Text = dropped_eggs.ToString();
                    pb.Dispose();
                    breakEgg(pos);
                    checkDIsplayChange();
                }
            } else {
                pb.Dispose();
            }

            string eggs = collected_eggs.ToString();

            for (int i = 1; i < eggs.Length + 1; i++) 
            {
                switch (i)
                {
                    case 1:
                        switch (eggs[0])
                        {
                            case '0':
                                num3.Image = Properties.Resources.digit_0;
                                break;
                            case '1':
                                num3.Image = Properties.Resources.digit_1;
                                break;
                            case '2':
                                num3.Image = Properties.Resources.digit_2;
                                break;
                            case '3':
                                num3.Image = Properties.Resources.digit_3;
                                break;
                            case '4':
                                num3.Image = Properties.Resources.digit_4;
                                break;
                            case '5':
                                num3.Image = Properties.Resources.digit_5;
                                break;
                            case '6':
                                num3.Image = Properties.Resources.digit_6;
                                break;
                            case '7':
                                num3.Image = Properties.Resources.digit_7;
                                break;
                            case '8':
                                num3.Image = Properties.Resources.digit_8;
                                break;
                            case '9':
                                num3.Image = Properties.Resources.digit_9;
                                break;
                        }
                        break;
                    case 2:
                        switch (eggs[1])
                        {
                            case '0':
                                num3.Image = Properties.Resources.digit_0;
                                break;
                            case '1':
                                num3.Image = Properties.Resources.digit_1;
                                break;
                            case '2':
                                num3.Image = Properties.Resources.digit_2;
                                break;
                            case '3':
                                num3.Image = Properties.Resources.digit_3;
                                break;
                            case '4':
                                num3.Image = Properties.Resources.digit_4;
                                break;
                            case '5':
                                num3.Image = Properties.Resources.digit_5;
                                break;
                            case '6':
                                num3.Image = Properties.Resources.digit_6;
                                break;
                            case '7':
                                num3.Image = Properties.Resources.digit_7;
                                break;
                            case '8':
                                num3.Image = Properties.Resources.digit_8;
                                break;
                            case '9':
                                num3.Image = Properties.Resources.digit_9;
                                break;
                        }
                        switch (eggs[0])
                        {
                            case '0':
                                num2.Image = Properties.Resources.digit_0;
                                break;
                            case '1':
                                num2.Image = Properties.Resources.digit_1;
                                break;
                            case '2':
                                num2.Image = Properties.Resources.digit_2;
                                break;
                            case '3':
                                num2.Image = Properties.Resources.digit_3;
                                break;
                            case '4':
                                num2.Image = Properties.Resources.digit_4;
                                break;
                            case '5':
                                num2.Image = Properties.Resources.digit_5;
                                break;
                            case '6':
                                num2.Image = Properties.Resources.digit_6;
                                break;
                            case '7':
                                num2.Image = Properties.Resources.digit_7;
                                break;
                            case '8':
                                num2.Image = Properties.Resources.digit_8;
                                break;
                            case '9':
                                num2.Image = Properties.Resources.digit_9;
                                break;
                        }
                        break;
                    case 3:
                        switch (eggs[2])
                        {
                            case '0':
                                num3.Image = Properties.Resources.digit_0;
                                break;
                            case '1':
                                num3.Image = Properties.Resources.digit_1;
                                break;
                            case '2':
                                num3.Image = Properties.Resources.digit_2;
                                break;
                            case '3':
                                num3.Image = Properties.Resources.digit_3;
                                break;
                            case '4':
                                num3.Image = Properties.Resources.digit_4;
                                break;
                            case '5':
                                num3.Image = Properties.Resources.digit_5;
                                break;
                            case '6':
                                num3.Image = Properties.Resources.digit_6;
                                break;
                            case '7':
                                num3.Image = Properties.Resources.digit_7;
                                break;
                            case '8':
                                num3.Image = Properties.Resources.digit_8;
                                break;
                            case '9':
                                num3.Image = Properties.Resources.digit_9;
                                break;
                        }
                        switch (eggs[1])
                        {
                            case '0':
                                num2.Image = Properties.Resources.digit_0;
                                break;
                            case '1':
                                num2.Image = Properties.Resources.digit_1;
                                break;
                            case '2':
                                num2.Image = Properties.Resources.digit_2;
                                break;
                            case '3':
                                num2.Image = Properties.Resources.digit_3;
                                break;
                            case '4':
                                num2.Image = Properties.Resources.digit_4;
                                break;
                            case '5':
                                num2.Image = Properties.Resources.digit_5;
                                break;
                            case '6':
                                num2.Image = Properties.Resources.digit_6;
                                break;
                            case '7':
                                num2.Image = Properties.Resources.digit_7;
                                break;
                            case '8':
                                num2.Image = Properties.Resources.digit_8;
                                break;
                            case '9':
                                num2.Image = Properties.Resources.digit_9;
                                break;
                        }
                        switch (eggs[0])
                        {
                            case '0':
                                num1.Image = Properties.Resources.digit_0;
                                break;
                            case '1':
                                num1.Image = Properties.Resources.digit_1;
                                break;
                            case '2':
                                num1.Image = Properties.Resources.digit_2;
                                break;
                            case '3':
                                num1.Image = Properties.Resources.digit_3;
                                break;
                            case '4':
                                num1.Image = Properties.Resources.digit_4;
                                break;
                            case '5':
                                num1.Image = Properties.Resources.digit_5;
                                break;
                            case '6':
                                num1.Image = Properties.Resources.digit_6;
                                break;
                            case '7':
                                num1.Image = Properties.Resources.digit_7;
                                break;
                            case '8':
                                num1.Image = Properties.Resources.digit_8;
                                break;
                            case '9':
                                num1.Image = Properties.Resources.digit_9;
                                break;
                        }
                        break;
                }
            }
        }

        private async void breakEgg(int pos)
        {
            int delay_break = 400;
            int delay_smol = 150;

            if (pos == 1 || pos == 2)
            {
                splash_left.Visible = true;
                await Task.Delay(delay_break);
                if (zayac_multiplier == 0.5)
                {
                    smol_l0.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_l0.Visible = false;
                    smol_l1.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_l1.Visible = false;
                    smol_l2.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_l2.Visible = false;
                    smol_l3.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_l3.Visible = false;
                }
                splash_left.Visible = false;
            } else
            {
                splash_right.Visible = true;
                await Task.Delay(delay_break);
                if (zayac_multiplier == 0.5)
                {
                    smol_r0.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_r0.Visible = false;
                    smol_r1.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_r1.Visible = false;
                    smol_r2.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_r2.Visible = false;
                    smol_r3.Visible = true;
                    await Task.Delay(delay_smol);
                    smol_r3.Visible = false;
                }
                splash_right.Visible = false;
            }
        }

        private async void show_Zayac()
        {
            zayac.Visible = true;
            zayac_multiplier = 0.5;
            await Task.Delay(zayac_delay);
            zayac.Visible = false;
            zayac_multiplier = 1;
        }

        private async void button5_Click(object sender, EventArgs e) //игра А
        {
            titleA.Visible = true;
            isGameRunning = true;

            button5.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;

            int i = 0;
            while (i < 9999)
            {
                int random_val = (rnd.Next() % 4 + 1);
                int random_zayac = (rnd.Next() % 100);

                egg_Procession(random_val);
                if (random_zayac < zayac_chance)
                {
                    show_Zayac();
                }

                await Task.Delay(delay);

                if (dropped_eggs >= 3)
                {
                    end_Game();
                    break;
                }

                if (delay > 250) //ограничение на задержку
                {
                    delay -= 2; 
                }

                if (move_delay > 250)
                {
                    delay -= 2;
                }

                if ((collected_eggs % 100) == 0 & (collected_eggs != 0)) //каждые 100 очков темп сбавляется
                {
                    delay += 150;
                }

                if ((collected_eggs == 200) || (collected_eggs == 500)) //при счёте в 200 или 500 очков количество штрафных очков сбрасывается до 0
                {
                    dropped_eggs = 0;
                }

                if (collected_eggs > 999)
                {
                    collected_eggs = 0;
                    final_score = 999; //согласно оригинальной игре в таблице рекордов максимальное значение счёта составляет 999 очков
                    num1.Image = Properties.Resources.digit_0;
                    num2.Image = Properties.Resources.digit_0;
                    num3.Image = Properties.Resources.digit_0;
                }

                     if (dropped_eggs == 0) //неиспользуемый лоток в игре А
                {unused_pos = 2;}
                else if (dropped_eggs == 0.5 || dropped_eggs == 1)
                {unused_pos = 4;}
                else if (dropped_eggs == 1.5 || dropped_eggs == 2)
                {unused_pos = 1;} 
                else
                {unused_pos = 3;}

                i++;
            }
        }
        private async void button10_Click(object sender, EventArgs e) //игра Б
        {
            titleB.Visible = true;
            isGameRunning = true;

            button5.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;

            int i = 0;
            while (i < 9999)
            {
                int random_val = (rnd.Next() % 4 + 1);
                int random_zayac = (rnd.Next() % 100);

                egg_Procession(random_val);
                if (random_zayac < zayac_chance)
                {
                    show_Zayac();
                }

                await Task.Delay(delay);

                if (dropped_eggs >= 3)
                {
                    end_Game();
                    break;
                }

                if (delay > 250) //ограничение на задержку
                {
                    delay -= 2;
                }

                if (move_delay > 250)
                {
                    delay -= 2;
                }

                if ((collected_eggs % 100) == 0 & (collected_eggs != 0)) //каждые 100 очков темп сбавляется
                {
                    delay += 75;
                }

                if ((collected_eggs == 200) || (collected_eggs == 500)) //при счёте в 200 или 500 очков количество штрафных очков сбрасывается до 0
                {
                    dropped_eggs = 0;
                }

                if (collected_eggs > 999)
                {
                    collected_eggs = 0;
                    final_score = 999; //согласно оригинальной игре в таблице рекордов максимальное значение счёта составляет 999 очков
                    num1.Image = Properties.Resources.digit_0;
                    num2.Image = Properties.Resources.digit_0;
                    num3.Image = Properties.Resources.digit_0;
                }

                i++;
            }
        }
        private void button11_Click(object sender, EventArgs e) //рекорды
        {
            var Form2 = new Form2();
            Form2.Show();
        }
        private void button12_Click(object sender, EventArgs e) //правила
        {
            var Form3 = new Form3();
            Form3.Show();
        }

        private async void end_Game()
        {
            isGameRunning = false;
            await Task.Delay(delay + move_delay * 6);

            if (final_score == 999)
            {
                result = final_score;
            }
            else
            {
                result = collected_eggs;
            }

            dropped_eggs = 0;
            collected_eggs = 0;

            label1.Text = collected_eggs.ToString();
            label2.Text = dropped_eggs.ToString();

            var Form4 = new Form4();
            Form4.Show();

            button5.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;

            egg1.Image = Properties.Resources.crashed_egg_50;
            egg2.Image = Properties.Resources.crashed_egg_50;
            egg3.Image = Properties.Resources.crashed_egg_50;

            num1.Image = Properties.Resources.digit_0;
            num2.Image = Properties.Resources.digit_0;
            num3.Image = Properties.Resources.digit_0;

            titleA.Visible = false;
            titleB.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e) //кнопка лево вверх
        {
            volk.Location = new Point(480, 308);
            volk.Image = Properties.Resources.volk_left_up;
            player_pos = 1;
            volk.BringToFront();
        }
        private void button2_Click(object sender, EventArgs e) //кнопка лево вниз
        {
            volk.Location = new Point(480, 308);
            volk.Image = Properties.Resources.volk_left_down;
            player_pos = 2;
            volk.BringToFront();
        }
        private void button3_Click(object sender, EventArgs e) //кнопка право вверх
        {
            volk.Location = new Point(598, 308);
            volk.Image = Properties.Resources.volk_right_up;
            player_pos = 3;
            volk.BringToFront();
        }
        private void button4_Click(object sender, EventArgs e) //кнопка право вниз
        {
            volk.Location = new Point(598, 308);
            volk.Image = Properties.Resources.volk_right_down;
            player_pos = 4;
            volk.BringToFront();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var Form4 = new Form4();
            Form4.Show();
        }
    }
}