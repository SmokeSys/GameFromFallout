using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameFromFallout
{

    [Serializable]
    public class SettingsRecords
    {
        public string Difficult { get; set; }
        public List<string> EasyWords { get; set; }
        public List<string> MediumWords { get; set; }
        public List<string> HardWords { get; set; }
        public int WordsCount { get; set; }
        public SettingsRecords()
        {
            EasyWords = new List<string>();
            MediumWords = new List<string>();
            HardWords = new List<string>();
            //SortWords();
        }
        public void UpdateWords()
        {
            StreamReader fstr = new StreamReader("words.txt");
            string temp = "";
            while ((temp = fstr.ReadLine()) != null)
            {
                if (temp.Length == 6)
                {
                    EasyWords.Add(temp.ToUpper());
                    WordsCount++;
                }
                else if (temp.Length == 10)
                {
                    MediumWords.Add(temp.ToUpper());
                    WordsCount++;
                }
                else if (temp.Length == 12)
                {
                    HardWords.Add(temp.ToUpper());
                    WordsCount++;
                }
            }
        }
        private void SortWords()
        {
            StreamReader fstr = new StreamReader("words.txt");
            string temp = "";
            List<string> tempL = new List<string>();
            while ((temp = fstr.ReadLine()) != null)
            {
                temp = new string((from c in temp
                                   where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                   select c).ToArray());
                if (temp.Length < 6) continue;
                tempL.Add(temp);
            }
            fstr.Close();
            StreamWriter sw = new StreamWriter("words.txt");
            sw.Write(tempL[0]);
            for (int i = 1; i < tempL.Count; i++)
            {
                sw.WriteLine(tempL[i]);
            }
            sw.Close();
        }
    }
}
