using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace LAB_5
{
    public partial class Form1 : Form
    {
        List<string> results = new List<string>();
        
        public Form1()
        {
            InitializeComponent();
         
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public List<string> SplitText(string fileName)
        {
            List<string> textByWords = new List<string>();
            File.OpenRead(fileName);
            string text = File.ReadAllText(fileName);
            string[] words = text.Split(' ', '.', ',', '!', '?', '(', ')', '=', '+','-', '\n');
            foreach (string temp in words)
            {
                if (!textByWords.Contains(temp))
                {
                    textByWords.Add(temp);
                }
            }
            return textByWords;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы|*.txt";
            openFileDialog1.ShowDialog();
            label1.Text = openFileDialog1.FileName;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }

        public static List<string> searchWords(object obj)
        {
           Tuple <List<string>, string, int> obj1 = (Tuple<List<string>, string, int>) obj;
            string str = obj1.Item2;
            int wordLen=str.Length;
             String word = str.ToUpper();
            int maxDistance = obj1.Item3;
            List<string> tempList = new List<string>();
            foreach (string str1 in obj1.Item1)
            {
                int tempLen = str1.Length;
                int distance;

                if (wordLen == 0) 
                {
                    distance = tempLen;
                }

                string temp = str1.ToUpper();

                int  [,] matrix = new int [wordLen+1 , tempLen+1];
                for (int i = 0; i <= wordLen; i++) matrix[i, 0] = i;
                for (int j = 0; j <= tempLen; j++) matrix[0, j] = j;

                for (int i = 1; i <= wordLen; i++)
                {
                    for (int j = 1; j <= tempLen; j++)
                    {
                        int symbEqual = (
                            (word.Substring(i - 1, 1) == 
                            temp.Substring(j - 1, 1)) ? 0 : 1);

                        int ins = matrix[i, j - 1] + 1; //Добавление             
                        int del = matrix[i - 1, j] + 1; //Удаление             
                        int subst = matrix[i - 1, j - 1] + symbEqual;

                        //Элемент матрицы вычисляется              
                        //как минимальный из трех случаев             
                        matrix[i, j] = Math.Min(Math.Min(ins, del), subst);

                       if ((i > 1) && (j > 1) &&
                            (word.Substring(i - 1, 1) == temp.Substring(j - 2, 1)) &&
                            (word.Substring(i - 2, 1) == temp.Substring(j - 1, 1)))
                        {
                            matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + symbEqual);
                        }
                        
                        
                    }
                }


                if (matrix[wordLen, tempLen] <= maxDistance)
                {
                    tempList.Add(temp + "  (" + matrix[wordLen, tempLen] + ")");
                }
            }
            

            return tempList;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            List<string> results=new List<string>();
            if (label1.Text == "File Name") MessageBox.Show("Choose file", "Error");
            else if (textBox1.Text.Length == 0) MessageBox.Show("Enter the word", "Error");
            else if (textBox2.Text.Length == 0) MessageBox.Show("Enter Levenshtein distance", "Error");
            else if (textBox3.Text.Length ==0) MessageBox.Show("Enter number of threads", "Error");
            else
            {
                listBox1.Items.Clear();
                results=subArrays(SplitText(label1.Text));    
            }
            MessageBoxButtons butons = MessageBoxButtons.YesNo;
            DialogResult YesNo;
            YesNo =MessageBox.Show("Make a report?", "Choose the answer", butons);
            if (YesNo == DialogResult.Yes)
            {
                makeReport(results);
            }

           
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        public List<string> subArrays(List<string>list)
        {
           
            int numberOfThreads;
            int.TryParse(textBox3.Text, out numberOfThreads);

            int destination;
            int.TryParse(textBox2.Text, out destination);

            int numberOfelements = list.Count;

            string str = textBox1.Text;

            int numberOfelementsInSubArray = numberOfelements / numberOfThreads;

            List<MinMax> borders = new List<MinMax>();

            Task<List<string>>[] tasks = new Task<List<string>>[numberOfThreads];

            Stopwatch timer = new Stopwatch();

            for (int i = 0; i < numberOfThreads; i++)
            {
                if ((i+1)!=numberOfThreads)
                {
                    MinMax temp = new MinMax(i * numberOfelementsInSubArray, (i + 1) * numberOfelementsInSubArray - 1);
                    borders.Add(temp);
                }
                else
                {
                    MinMax temp = new MinMax(i * numberOfelementsInSubArray, numberOfelements-1);
                    borders.Add(temp);
                }
            }

            timer.Start();

            for (int i = 0; i < numberOfThreads; i++)
            { 
                List<string> tempList = list.GetRange(borders[i].Min, borders[i].Max-borders[i].Min);
                tasks[i] = new Task<List<string>>(searchWords, new Tuple<List<string>, string, int>(tempList,str,destination));
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            timer.Stop();

            label6.Text = timer.Elapsed.ToString();

            List<string> results = new List<string>();
            for (int i = 0; i < numberOfThreads; i++)
            {           
                listBox1.Items.Add("Поток " + (i+1).ToString() + ":");
                results.Add("Поток " + (i + 1).ToString() + ":");
                foreach (var x in tasks[i].Result)
                {
                    listBox1.Items.Add(x.ToString());
                    results.Add(x.ToString());
                }
            }

            return results;

        }

        public void makeReport(List<string> results)
        {
            string ReportFileName = "Report_" + DateTime.Now.ToString("dd_MM_yyyy_hhmmss")+".txt";
           
            
            StringBuilder b=new StringBuilder();
            
            foreach (string x in results) b.AppendLine(x.ToString());
                
            

            File.AppendAllText(ReportFileName, b.ToString());

        }

    }
}
