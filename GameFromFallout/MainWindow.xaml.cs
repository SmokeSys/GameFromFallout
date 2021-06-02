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
#region Fields
        string difficulty; 
        string spamSybmols = ";:#@!$%^&*()-=+{}[],./?<>~`"; 
        int lifesValue, scoreValue = 0; 
        SettingsRecords setRec; 
        Random random;
        Button tempButton;  //tak i ne vspomnil, zachem eto nujno :( 
        string answer; 
        Label tempLabel, score, lifes; 
        List<Label> labels; 
        bool cont = false, rest = false;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent(); 

            Uri iconUri = new Uri("Icon.ico", UriKind.RelativeOrAbsolute);
            WindowMain.Icon = BitmapFrame.Create(iconUri);

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(@"assets\backgroundImage.png", UriKind.Relative));
            canvasMain.Background = ib;

            random = new Random();

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

            tempButton = Easy;

            canvasSec.Children.Remove(Back);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes main field
        /// </summary>
        private void Start()
        {
            //clears the area
            canvasSec.Children.Clear();
            canvasSec.Children.Add(Back);
            if (labels != null) labels.Clear();

            //*???*
            if (difficulty == "easy")
                tempButton = Medium;
            else tempButton = Easy;

            //*creating a label for display the score*
            if (scoreValue == 0)
            {
                score = new Label();
                score.Content = "SOMCO INDUSTRIES (TM) TERMINAL PROTOCOL \nENTER THE PASSWORD";
                score.FontSize = tempButton.FontSize + 2;
                score.FontStyle = tempButton.FontStyle;
                score.FontFamily = tempButton.FontFamily;
                score.BorderBrush = tempButton.BorderBrush;
                score.Cursor = Cursors.Arrow;
                score.FocusVisualStyle = tempButton.FocusVisualStyle;
                score.Foreground = tempButton.Foreground;
                score.Background = tempButton.Background;
                Canvas.SetLeft(score, 20);
                Canvas.SetTop(score, 20);
            }
            canvasSec.Children.Add(score);

            //*creating a label for display the lifes*
            if (difficulty == "hard") lifesValue = 2;
            else lifesValue = 3;
            lifes = new Label();
            lifes.Content = "TRIES LEFT: " + (lifesValue + 1).ToString();
            lifes.FontSize = tempButton.FontSize;
            lifes.FontStyle = tempButton.FontStyle;
            lifes.FontFamily = tempButton.FontFamily;
            lifes.BorderBrush = tempButton.BorderBrush;
            lifes.Cursor = Cursors.Arrow;
            lifes.FocusVisualStyle = tempButton.FocusVisualStyle;
            lifes.Foreground = tempButton.Foreground;
            lifes.Background = tempButton.Background;
            Canvas.SetLeft(lifes, 20);
            Canvas.SetTop(lifes, 80);
            canvasSec.Children.Add(lifes);
            //


            //---birthday gift opening--- 
            //*get words from file*
            XmlSerializer xs = new XmlSerializer(typeof(SettingsRecords));
            Stream fstr = null;
            fstr = File.OpenRead("assets/settings.xml");
            setRec = (SettingsRecords)xs.Deserialize(fstr);
            if (setRec.WordsCount == 0)
            {
                try { setRec.UpdateWords(); }                
                catch { MessageBox.Show("word.txt file empty or doesnt exist"); Close(); }
                if (setRec.WordsCount < 10)
                {
                    MessageBox.Show("Update word.txt file"); Close();
                }
            }
            setRec.Difficult = difficulty;
            fstr.Close();

            //*creating address buttons (ex.0x00AAAA)*
            List<Button> fieldAdresses = new List<Button>();
            int j = 0; int ti = 0;  // for positioning
            for (int i = 0; i < 28; i++)
            {
                fieldAdresses.Add(new Button());
                NewButton(fieldAdresses[i]);
                if (i == 0)
                {
                    fieldAdresses[i].Content += "0x" + To16SS() + To16SS() + To16SS() + To16SS();
                }
                else fieldAdresses[i].Content = To16Add((string)fieldAdresses[i - 1].Content);
                Canvas.SetLeft(fieldAdresses[i], 25 + j);
                Canvas.SetTop(fieldAdresses[i], 110 + ti * 20);
                ti++;
                if (i == 13) { j = 310; ti = 0; }
                canvasSec.Children.Add(fieldAdresses[i]);
                fieldAdresses[i].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                fieldAdresses[i].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
            }   

            //*creating the main field*
            List<Button> field = new List<Button>();
            ti = 0; j = 0; int tj = 0; //for positioning
            List<string> tempWordsList;
            List<string> variantes = new List<string>(); //words in this cycle
            switch (difficulty)
            {
                case "easy": tempWordsList = setRec.EasyWords; break;
                case "medium": tempWordsList = setRec.MediumWords; break;
                default: tempWordsList = setRec.HardWords; break;
            }
            int insword = random.Next(0, 4); int count = 0; 
            //*main content creator*
            for (int i = 0; i < 28; i++)
            {
                //looks like spam generator
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
                        field[count].Content = spamSybmols[random.Next(0, spamSybmols.Length)];
                        canvasSec.Children.Add(field[count]);
                        count++;
                    }
                }
                //*words generator*
                if (i == insword)
                {
                    int upk; //???
                    int k;
                    switch (difficulty)
                    {
                        case "easy": upk = random.Next(0, 20); break;
                        case "medium": upk = random.Next(0, 15); break;
                        default: upk = random.Next(0, 10); break;
                    }
                    //another spam generator
                    for (k = 100; k < 50 + upk * 10; k += 10)
                    {
                        field.Add(new Button());
                        NewButton(field[count]);
                        Canvas.SetLeft(field[count], k + tj);
                        Canvas.SetTop(field[count], 110 + ti * 20);
                        field[count].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].Click += new RoutedEventHandler(ClickToWord);
                        field[count].Content = spamSybmols[random.Next(0, spamSybmols.Length)];
                        canvasSec.Children.Add(field[count]);
                        count++;
                    }

                    //word buttons formatter
                    field.Add(new Button());
                    NewButton(field[count]);
                    Canvas.SetLeft(field[count], k + tj);
                    Canvas.SetTop(field[count], 110 + ti * 20);
                    Canvas.SetZIndex(field[count], 1);
                    field[count].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                    field[count].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
                    field[count].Click += new RoutedEventHandler(ClickToWord);
                    field[count].Content = tempWordsList[random.Next(0, tempWordsList.Count)];
                    variantes.Add((string)field[count].Content);
                    insword = random.Next(i + 1, i + 5);
                    canvasSec.Children.Add(field[count]);
                    k += 90;
                    count++;
                    //spam generator, heh
                    for (; k < 300; k += 10)
                    {
                        field.Add(new Button());
                        NewButton(field[count]);
                        Canvas.SetLeft(field[count], k + tj);
                        Canvas.SetTop(field[count], 110 + ti * 20);
                        field[count].MouseEnter += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].MouseLeave += new MouseEventHandler(MouseEnterStartButtons);
                        field[count].Click += new RoutedEventHandler(ClickToWord);
                        field[count].Content = spamSybmols[random.Next(0, spamSybmols.Length)];
                        canvasSec.Children.Add(field[count]);
                        count++;
                    }
                }
                ti++;
                if (i == 13) { ti = 0; tj = 310; }
            }
            //*choosing the correct answer*
            answer = variantes[random.Next(0,variantes.Count)];
            // label ">" for decoration 
            tempLabel = new Label();
            tempLabel.FontSize = tempButton.FontSize + 3;
            tempLabel.FontStyle = tempButton.FontStyle;
            tempLabel.FontFamily = tempButton.FontFamily;
            tempLabel.BorderBrush = tempButton.BorderBrush;
            tempLabel.Cursor = Cursors.Arrow;
            tempLabel.FocusVisualStyle = tempButton.FocusVisualStyle;
            tempLabel.Foreground = tempButton.Foreground;
            tempLabel.Background = tempButton.Background;
            tempLabel.Content = '>';
            Canvas.SetLeft(tempLabel, 615);
            Canvas.SetTop(tempLabel, 365);
            canvasSec.Children.Add(tempLabel);

            labels = new List<Label>();
        }
        /// <summary>
        /// Handles clicks on a word/spam
        /// </summary>
        /// <param name="sender">Word and Spam Buttons from main field</param>
        /// <param name="e"></param>
        private void ClickToWord(object sender, RoutedEventArgs e)
        {
            if (!cont)  //bool continue, true if an answer is found
            {
                Button temp = (Button)e.Source;
                if (temp.Content == answer)
                {
                    //clears the right part
                    for (int i = 0; i < labels.Count; i++)
                        canvasSec.Children.Remove(labels[i]);
                    labels.Clear();
                }
                //answer decorations
                labels.Add(new Label());
                labels[labels.Count - 1].FontSize = tempButton.FontSize + 1;
                labels[labels.Count - 1].FontStyle = tempButton.FontStyle;
                labels[labels.Count - 1].FontFamily = tempButton.FontFamily;
                labels[labels.Count - 1].BorderBrush = tempButton.BorderBrush;
                labels[labels.Count - 1].Cursor = Cursors.Arrow;
                labels[labels.Count - 1].FocusVisualStyle = tempButton.FocusVisualStyle;
                labels[labels.Count - 1].Foreground = tempButton.Foreground;
                labels[labels.Count - 1].Background = tempButton.Background;
                labels[labels.Count - 1].Content = temp.Content;
                Canvas.SetLeft(labels[labels.Count - 1], 615);
                Canvas.SetTop(labels[labels.Count - 1], 365 - labels.Count * 80);
                canvasSec.Children.Add(labels[labels.Count - 1]);
                //winning label
                if (temp.Content == answer)
                {
                    labels[labels.Count - 1].Content += "\nACCESS GRANTED\nPRESS ANY KEY \nTO CONTINUE";
                    cont = true;
                    scoreValue++;
                    score.Content = "SOMCO INDUSTRIES (TM) TERMINAL PROTOCOL X" + (scoreValue).ToString() + "\nENTER THE PASSWORD";
                    return;
                }
                //*click wrong*
                string tmp;
                int tmplength;
                switch (difficulty)
                {
                    case "easy": tmplength = 6; break;
                    case "medium": tmplength = 10; break;
                    default: tmplength = 12; break;
                }
                try
                {
                    tmp = (string)temp.Content;
                }
                //displays selection on the right side, shows errors and subtracts lives
                catch
                {
                    labels[labels.Count - 1].Content += "\nACCESS DENIED" + "\n0/" + tmplength.ToString() + " CORRECT";
                    lifesValue--;
                    lifes.Content = "TRIES LEFT: " + (lifesValue + 1).ToString();
                    if (lifesValue < 0)
                    {
                        canvasSec.Children.Clear();
                        Canvas.SetLeft(lifes, 300);
                        Canvas.SetTop(lifes, 200);
                        canvasSec.Children.Add(lifes);
                        lifes.Content = "TERMINAL IS LOCKED.\nPLEASE CONTACT TO SYSTEM ADMINISTRATOR\nPRESS BACK KEY TO RESTART GAME";
                        rest = true;
                        scoreValue = 0;
                        canvasSec.Children.Add(Back);
                    }
                    return;
                }
                //no life :(
                if (lifesValue <= 0)
                {
                    canvasSec.Children.Clear();
                    Canvas.SetLeft(lifes, 300);
                    Canvas.SetTop(lifes, 200);
                    lifes.Content = "TERMINAL IS LOCKED.\nPLEASE CONTACT TO SYSTEM ADMINISTRATOR\nPRESS BACK KEY TO RESTART GAME";
                    rest = true;
                    scoreValue = 0;
                    canvasSec.Children.Add(Back);
                    canvasSec.Children.Add(lifes);
                    return;
                }
                //oh, there are still a few lifes!
                int corrects = 0;
                for (int i = 0; i < tmplength; i++)
                {
                    if (answer[i] == tmp[i]) corrects++;
                }
                labels[labels.Count - 1].Content += "\nACCESS DENIED" + "\n" + corrects.ToString() + "/" + tmplength.ToString() + " CORRECT";
                lifesValue--;
                lifes.Content = "TRIES LEFT: " + (lifesValue + 1).ToString();
            }
        }

        #endregion

        #region HelpersMethods

        /// <summary>
        /// Initializes next level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Return to home screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            canvasSec.Children.Clear();
            canvasSec.Children.Add(Easy);
            canvasSec.Children.Add(Medium);
            canvasSec.Children.Add(Hard);
        }

        /// <summary>
        /// Changes the display of button
        /// </summary>
        /// <param name="sender">Choosen button</param>
        /// <param name="e"></param>
        private void MouseEnterStartButtons(object sender, MouseEventArgs e)
        {
            Button temp1 = (Button)e.Source;
            Brush temp = temp1.Background;
            temp1.Background = temp1.Foreground;
            temp1.BorderBrush = temp1.Foreground;
            temp1.Foreground = temp;
            if (tempLabel != null && !cont)
                tempLabel.Content = ">" + temp1.Content;
        }

        /// <summary>
        /// Creates a button by template
        /// </summary>
        /// <param name="x"></param>
        private void NewButton(Button x)
        {
            x.FontSize = tempButton.FontSize;
            x.FontStyle = tempButton.FontStyle;
            x.FontFamily = tempButton.FontFamily;
            x.BorderBrush = tempButton.BorderBrush;
            x.Cursor = Cursors.Arrow;
            x.FocusVisualStyle = tempButton.FocusVisualStyle;
            x.Foreground = tempButton.Foreground;
            x.Background = tempButton.Background;
        }

        /// <summary>
        /// Helper method for generating addresses
        /// </summary>
        /// <returns>Hex in string type</returns>
        private string To16SS()
        {
            string temp; int temp1 = random.Next(0, 16);
            switch (temp1)
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

        /// <summary>
        /// Helper method for generating addresses
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string To16Add(string text)
        {
            string temp;
            temp = text.Substring(2, 4);
            if (temp[3] == 'F')
            {
                temp = temp.Insert(3, To16AddChar(temp[3]));
                temp = temp.Remove(4, 1);
                if (temp[2] == 'F')
                {
                    temp = temp.Insert(2, To16AddChar(temp[2]));
                    temp = temp.Remove(3, 1);
                    if (temp[1] == 'F')
                    {
                        temp = temp.Insert(1, To16AddChar(temp[1]));
                        temp = temp.Remove(2, 1);
                        temp = temp.Insert(0, To16AddChar(temp[0]));
                        temp = temp.Remove(1, 1);
                    }
                    else
                    {
                        temp = temp.Insert(1, To16AddChar(temp[1]));
                        temp = temp.Remove(2, 1);
                    }
                }
                else
                {
                    temp = temp.Insert(2, To16AddChar(temp[2]));
                    temp = temp.Remove(3, 1);
                }
            }
            else
            {
                temp = temp.Insert(3, To16AddChar(temp[3]));
                temp = temp.Remove(4, 1);
            }
            temp = temp.Insert(0, "0x");
            return temp;
        }

        /// <summary>
        /// Helper method for generating addresses
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private string To16AddChar(char v)
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

        /// <summary>
        /// Closes window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton(object sender, MouseEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Sets difficulty as easy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartEasy(object sender, RoutedEventArgs e)
        {
            difficulty = "easy";
            Start();
        }

        /// <summary>
        /// Sets difficulty as medium
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMedium(object sender, RoutedEventArgs e)
        {
            difficulty = "medium";
            Start();
        }

        /// <summary>
        /// Sets difficulty as hard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartHard(object sender, RoutedEventArgs e)
        {
            difficulty = "hard";
            Start();
        }

        #endregion
    }
}
