using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Game_NuPogodi
{
    public partial class Form4 : Form
    {
        string[,] score = new string[10, 2];

        string full_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"saves\", "save.txt");

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            label3.Text = Form1.result.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = String.Concat(textBox1.Text.Where(c => !Char.IsWhiteSpace(c))); //удаление пробелов

            var f4 = new Form4();
            while (String.IsNullOrEmpty(textBox1.Text)) //появление формы до тех пор пока игрок не нажмёт на кнопку для сохранения
            {
                textBox1.Text = "player";
                f4.Show();
            }

            int k = 0;

            var str = File.ReadAllText(full_path);

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

            string[,] session_score = new string[1, 2];

            session_score[0, 0] = textBox1.Text;
            session_score[0, 1] = Form1.result.ToString();

            if (Int32.Parse(session_score[0,1]) > Int32.Parse(score[9,1]))
            {
                score[9, 0] = session_score[0, 0];
                score[9, 1] = session_score[0, 1];
            }

            for (int i = 0; i < (words.Length - 2) / 2; i++) //сортировка
            {
                for (int j = 0; j < (words.Length - i - 2) / 2; j++)
                {
                    if (Int32.Parse(score[j, 1]) < Int32.Parse(score[j + 1, 1]))
                    {
                        string temp_score = score[j + 1, 1];
                        string temp_name = score[j + 1, 0];

                        score[j + 1, 1] = score[j, 1];
                        score[j + 1, 0] = score[j, 0];

                        score[j, 1] = temp_score;
                        score[j, 0] = temp_name;
                    }

                }
            }

            string[] new_words = new string[10];

            for (int i = 0; i < 10; i++)
            {
                new_words[i] = score[i, 0] + " " + score[i, 1] + " ";
                new_words[i] = new_words[i].Replace("\r\n", "");
            }

            File.WriteAllText(full_path, string.Empty); //удаление всех строк в файле путем записи пустой строки вместо всего файла
            File.AppendAllLines(full_path, new_words); //запись рекордов в файл

            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
