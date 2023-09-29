using System;
using System.Windows.Forms;
using System.IO;

namespace Game_NuPogodi
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e) //отображение рекордов из файла при загрузке формы
        {
            string full_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"saves\", "save.txt");
            var str = File.ReadAllText(full_path);

            string[,] score = new string[10, 2];
            int k = 0;

            string[] words = str.Split(new char[] { ' ' });

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    score[i, j] = words[k];
                    k++;
                }
            }
            k = 0;

            string[] new_words = new string[10];

            for (int i = 0; i < 10; i++)
            {
                new_words[i] = score[i, 0] + " " + score[i, 1] + " ";
                new_words[i] = new_words[i].Replace("\r\n", "");
                listBox1.Items.Add(new_words[i]);
            }
        }
    }
}