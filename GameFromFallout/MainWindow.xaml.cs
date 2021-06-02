using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameFromFallout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string difficult; string ssimvols = ";:#@!$%^&*()-=+{}[],./?<>~`"; int lifesv, scorev = 0; SettingsRecords s; Random r;
        Button tempColor; string answer; Label l, score, lifes; List<Label> labels; bool cont = false, rest = false;
        public MainWindow()
        {
            InitializeComponent(); 
            Uri iconUri = new Uri("Icon.ico", UriKind.RelativeOrAbsolute);
            WindowMain.Icon = BitmapFrame.Create(iconUri);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(@"assets\backgroundImage.png", UriKind.Relative));
            canvasMain.Background = ib;
            r = new Random();
            Canvas rec = new Canvas();
            rec.Height = 40;
            rec.Width = 60;
            ImageBrush ibb = new ImageBrush();
            ibb.ImageSource = new BitmapImage(new Uri(@"assets\powerOn.png", UriKind.Relative));
            rec.Background = ibb;
            Canvas.SetLeft(rec, 750);
            Canvas.SetTop(rec, 525);
            canvasMain.Children.Add(rec);
            rec.MouseLeftButtonUp += new MouseButtonEventHandler(exitButton);
            tempColor = Easy;
            canvasSec.Children.Remove(Back);
        }
        private void exitButton(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void StartEasy(object sender, RoutedEventArgs e)
        {
            difficult = "easy";
            Start();
        }

        private void StartMedium(object sender, RoutedEventArgs e)
        {
            difficult = "medium";
            Start();
        }

        private void StartHard(object sender, RoutedEventArgs e)
        {
            difficult = "hard";
            Start();
        }
        private void Start()
        {
            canvasSec.Children.Clear();
            canvasSec.Children.Add(Back);
            if (labels != null) labels.Clear();
            if (difficult == "easy")
                tempColor = Medium;
            else tempColor = Easy;
            //TextBox tb = new TextBox();
            if (scorev == 0)
            {
                score = new Label();
                score.Content = "SOMCO INDUSTRIES (TM) TERMINAL PROTOCOL \nENTER THE PASSWORD";
                score.FontSize = tempColor.FontSize + 2;
                score.FontStyle = tempColor.FontStyle;
                score.FontFamily = tempColor.FontFamily;
                score.BorderBrush = tempColor.BorderBrush;
                score.Cursor = Cursors.Arrow;
                score.FocusVisualStyle = tempColor.FocusVisualStyle;
                score.Foreground = tempColor.Foreground;
                score.Background = tempColor.Background;
                Canvas.SetLeft(score, 20);
                Canvas.SetTop(score, 20);
            }
            canvasSec.Children.Add(score);
            if (difficult == "hard") lifesv = 2;
            else lifesv = 3;
            lifes = new Label();
            lifes.Content = "TRIES LEFT: " + (lifesv + 1).ToString();
            lifes.FontSize = tempColor.FontSize;
            lifes.FontStyle = tempColor.FontStyle;
            lifes.FontFamily = tempColor.FontFamily;
            lifes.BorderBrush = tempColor.BorderBrush;
            lifes.Cursor = Cursors.Arrow;
            lifes.FocusVisualStyle = tempColor.FocusVisualStyle;
            lifes.Foreground = tempColor.Foreground;
            lifes.Background = tempColor.Background;
            Canvas.SetLeft(lifes, 20);
            Canvas.SetTop(lifes, 80);
            canvasSec.Children.Add(lifes);

            

            XmlSerializer xs = new XmlSerializer(typeof(SettingsRecords));
            Stream fstr = null;
            fstr = File.OpenRead("assets/settings.xml");
            s = (SettingsRecords)xs.Deserialize(fstr);
            if (s.WordsCount == 0)
                s.UpdateWords();
            s.Difficult = difficult;
            fstr.Close();

            List<Button> fieldAdresses = new List<Button>();
            int j = 0; int ti = 0;
            for (int i = 0; i < 28; i++)
            {
                fieldAdresses.Add(new Button());
                NewButton(fieldAdresses[i]);
                if (i == 0)
                {
                    fieldAdresses[i].Content += "0x" + To16SS() + To16SS() + To16SS() + To16SS();
                }
                else fieldAdresses[i].Content = Add((string)fieldAdresses[i - 1].Content);
                Canvas.SetLeft(fieldAdresses[i], 25 + j);
                Canvas.SetTop(fieldAdresses[i], 110 + ti * 20);
                ti++;
                if (i == 13) { j = 310; ti = 0; }
                canvasSec.Children.Add(fieldAdresses[i]);
                fieldAdresses[i].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                fieldAdresses[i].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
            }   //adresses

            List<Button> field = new List<Button>();
            ti = 0; j = 0; int tj = 0;
            List<string> temp;
            List<string> variantes = new List<string>();
            if (difficult == "easy") temp = s.EasyWords;
            else if (difficult == "medium") temp = s.MediumWords;
            else temp = s.HardWords;
            int insword = r.Next(0, 4); int count = 0;            
            for (int i = 0; i < 28; i++)
            {
                if (i != insword)
                {
                    for (int k = 100; k < 300; k+=10)
                    {
                        field.Add(new Button());
                        NewButton(field[count]);
                        Canvas.SetLeft(field[count], k + tj);
                        Canvas.SetTop(field[count], 110 + ti * 20);
                        field[count].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].Click += new RoutedEventHandler(ClickToWord);
                        field[count].Content = ssimvols[r.Next(0, ssimvols.Length)];
                        canvasSec.Children.Add(field[count]);
                        count++;
                    }
                }
                if (i == insword)
                {
                    int upk; int k;
                    if (difficult == "easy") upk = r.Next(0, 20);
                    else if (difficult == "medium") upk = r.Next(0, 15);
                    else upk = r.Next(0, 10);
                    for (k = 100; k < 50 + upk * 10; k += 10)
                    {
                        field.Add(new Button());
                        NewButton(field[count]);
                        Canvas.SetLeft(field[count], k + tj);
                        Canvas.SetTop(field[count], 110 + ti * 20);
                        field[count].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].Click += new RoutedEventHandler(ClickToWord);
                        field[count].Content = ssimvols[r.Next(0, ssimvols.Length)];
                        canvasSec.Children.Add(field[count]);
                        count++;
                    }
                    field.Add(new Button());
                    NewButton(field[count]);
                    Canvas.SetLeft(field[count], k + tj);
                    Canvas.SetTop(field[count], 110 + ti * 20);
                    Canvas.SetZIndex(field[count], 1);
                    field[count].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                    field[count].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
                    field[count].Click += new RoutedEventHandler(ClickToWord);
                    field[count].Content = temp[r.Next(0, temp.Count)];
                    variantes.Add((string)field[count].Content);
                    insword = r.Next(i + 1, i + 5);
                    canvasSec.Children.Add(field[count]);
                    k += 90;
                    count++;
                    for (; k < 300; k += 10)
                    {
                        field.Add(new Button());
                        NewButton(field[count]);
                        Canvas.SetLeft(field[count], k + tj);
                        Canvas.SetTop(field[count], 110 + ti * 20);
                        field[count].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].Click += new RoutedEventHandler(ClickToWord);
                        field[count].Content = ssimvols[r.Next(0, ssimvols.Length)];
                        canvasSec.Children.Add(field[count]);
                        count++;
                    }
                }
                ti++;
                if (i == 13) { ti = 0; tj = 310; }
            }
            answer = variantes[r.Next(0,variantes.Count)];
            l = new Label();
            l.FontSize = tempColor.FontSize + 3;
            l.FontStyle = tempColor.FontStyle;
            l.FontFamily = tempColor.FontFamily;
            l.BorderBrush = tempColor.BorderBrush;
            l.Cursor = Cursors.Arrow;
            l.FocusVisualStyle = tempColor.FocusVisualStyle;
            l.Foreground = tempColor.Foreground;
            l.Background = tempColor.Background;
            l.Content = '>';
            Canvas.SetLeft(l, 615);
            Canvas.SetTop(l, 365);
            canvasSec.Children.Add(l);
            labels = new List<Label>();
        }
        private void ClickToWord(object sender, RoutedEventArgs e)
        {
            if (!cont)
            {
                Button temp = (Button)e.Source;
                if (temp.Content == answer)
                {
                    for (int i = 0; i < labels.Count; i++)
                        canvasSec.Children.Remove(labels[i]);
                    labels.Clear();
                }
                labels.Add(new Label());
                labels[labels.Count - 1].FontSize = tempColor.FontSize + 1;
                labels[labels.Count - 1].FontStyle = tempColor.FontStyle;
                labels[labels.Count - 1].FontFamily = tempColor.FontFamily;
                labels[labels.Count - 1].BorderBrush = tempColor.BorderBrush;
                labels[labels.Count - 1].Cursor = Cursors.Arrow;
                labels[labels.Count - 1].FocusVisualStyle = tempColor.FocusVisualStyle;
                labels[labels.Count - 1].Foreground = tempColor.Foreground;
                labels[labels.Count - 1].Background = tempColor.Background;
                labels[labels.Count - 1].Content = temp.Content;
                Canvas.SetLeft(labels[labels.Count - 1], 615);
                Canvas.SetTop(labels[labels.Count - 1], 365 - labels.Count * 80);
                canvasSec.Children.Add(labels[labels.Count - 1]);
                if (temp.Content == answer)
                {
                    labels[labels.Count - 1].Content += "\nACCESS GRANTED\nPRESS ANY KEY \nTO CONTINUE";
                    cont = true;
                    scorev++;
                    score.Content = "SOMCO INDUSTRIES (TM) TERMINAL PROTOCOL X" + (scorev).ToString() + "\nENTER THE PASSWORD";
                    return;
                }
                string tmp;
                int tmplength;
                if (difficult == "easy") tmplength = 6;
                else if (difficult == "medium") tmplength = 10;
                else tmplength = 12;
                try
                {
                    tmp = (string)temp.Content;
                }
                catch
                {
                    labels[labels.Count - 1].Content += "\nACCESS DENIED" + "\n0/" + tmplength.ToString() + " CORRECT";
                    lifesv--;
                    lifes.Content = "TRIES LEFT: " + (lifesv + 1).ToString();
                    if (lifesv < 0)
                    {
                        canvasSec.Children.Clear();
                        Canvas.SetLeft(lifes, 300);
                        Canvas.SetTop(lifes, 200);
                        canvasSec.Children.Add(lifes);
                        lifes.Content = "TERMINAL IS LOCKED.\nPLEASE CONTACT TO SYSTEM ADMINISTRATOR\nPRESS BACK KEY TO RESTART GAME";
                        rest = true;
                        scorev = 0;
                        canvasSec.Children.Add(Back);
                    }
                    return;
                }
                if (lifesv <= 0)
                {
                    canvasSec.Children.Clear();
                    Canvas.SetLeft(lifes, 300);
                    Canvas.SetTop(lifes, 200);
                    lifes.Content = "TERMINAL IS LOCKED.\nPLEASE CONTACT TO SYSTEM ADMINISTRATOR\nPRESS BACK KEY TO RESTART GAME";
                    rest = true;
                    scorev = 0;
                    canvasSec.Children.Add(Back);
                    canvasSec.Children.Add(lifes);
                    return;
                }
                int corrects = 0;
                for (int i = 0; i < tmplength; i++)
                {
                    if (answer[i] == tmp[i]) corrects++;
                }
                labels[labels.Count - 1].Content += "\nACCESS DENIED" + "\n" + corrects.ToString() + "/" + tmplength.ToString() + " CORRECT";
                lifesv--;
                lifes.Content = "TRIES LEFT: " + (lifesv + 1).ToString();
            }
        }
        private void NewButton(Button x)
        {
            x.FontSize = tempColor.FontSize;
            x.FontStyle = tempColor.FontStyle;
            x.FontFamily = tempColor.FontFamily;
            x.BorderBrush = tempColor.BorderBrush;
            x.Cursor = Cursors.Arrow;
            x.FocusVisualStyle = tempColor.FocusVisualStyle;
            x.Foreground = tempColor.Foreground;
            x.Background = tempColor.Background;
        }
        private string To16SS()
        {
            string temp; int temp1 = r.Next(0,16);
            switch(temp1)
            {
                case 10: temp = "A"; break;
                case 11: temp = "B"; break;
                case 12: temp = "C"; break;
                case 13: temp = "D"; break;
                case 14: temp = "E"; break;
                case 15: temp = "F"; break;
                default: temp = temp1.ToString(); break;
            }
            return temp;
        }
        private string Add(string text)
        {
            string temp;
            temp = text.Substring(2, 4);
            if (temp[3] == 'F')
            {
                temp = temp.Insert(3, AddChar(temp[3]));
                temp = temp.Remove(4, 1);
                if (temp[2] == 'F')
                {
                    temp = temp.Insert(2, AddChar(temp[2]));
                    temp = temp.Remove(3, 1);
                    if (temp[1] == 'F')
                    {
                        temp = temp.Insert(1, AddChar(temp[1]));
                        temp = temp.Remove(2, 1);
                        temp = temp.Insert(0, AddChar(temp[0]));
                        temp = temp.Remove(1, 1);
                    }
                    else
                    {
                        temp = temp.Insert(1, AddChar(temp[1]));
                        temp = temp.Remove(2,1);
                    }
                }
                else
                {
                    temp = temp.Insert(2, AddChar(temp[2]));
                    temp = temp.Remove(3, 1);
                }
            }
            else
            {
                temp = temp.Insert(3, AddChar(temp[3]));
                temp = temp.Remove(4, 1);
            }
            temp = temp.Insert(0, "0x");
            return temp;
        }
        private string AddChar(char v)
        {
            string temp = "";
            switch (v)
            {
                case '0': temp += '1'; break;
                case '1': temp += '2'; break;
                case '2': temp += '3'; break;
                case '3': temp += '4'; break;
                case '4': temp += '5'; break;
                case '5': temp += '6'; break;
                case '6': temp += '7'; break;
                case '7': temp += '8'; break;
                case '8': temp += '9'; break;
                case '9': temp += 'A'; break;
                case 'A': temp += 'B'; break;
                case 'B': temp += 'C'; break;
                case 'C': temp += 'D'; break;
                case 'D': temp += 'E'; break;
                case 'E': temp += 'F'; break;
                case 'F': temp += '0'; break;
            }
            return temp;
        }
        private void MouseEnterStartButtons(object sender, MouseEventArgs e)
        {
                Button temp1 = (Button)e.Source;
                Brush temp = temp1.Background;
                temp1.Background = temp1.Foreground;
                temp1.BorderBrush = temp1.Foreground;
                temp1.Foreground = temp;
                if (l != null && !cont)
                    l.Content = ">" + temp1.Content;
        }


        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            canvasSec.Children.Clear();
            canvasSec.Children.Add(Easy);
            canvasSec.Children.Add(Medium);
            canvasSec.Children.Add(Hard);
        }

        private void PressDownKey(object sender, KeyEventArgs e)
        {
            if (rest)
            {
                rest = false;

                canvasSec.Children.Clear();
                canvasSec.Children.Add(Easy);
                canvasSec.Children.Add(Medium);
                canvasSec.Children.Add(Hard);
            }
            if (cont)
            {
                cont = false;
                Start();
            }
        }
    }
}
