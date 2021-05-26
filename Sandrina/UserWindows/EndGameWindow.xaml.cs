using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sandrina.UserElements {
    public partial class EndGameWindow : Window {
        public EndGameWindow() {
            InitializeComponent();
        }

        #region Глобальные переменные
        static string UserHomeDir    = Environment.GetEnvironmentVariable("USERPROFILE") + @"\";
        static string PathToSave     = @"AppData\Roaming\";
        static string SaveFolderName = @"Sandrina\";
        static string FileName       = "save.sav";
        static string FullPathToFile = UserHomeDir + PathToSave + SaveFolderName + FileName;
        const  string SaveSection    = "SAVES";

        static List<UserData> Saves = new List<UserData>();
        static int  CurrentResultIndex = -1;
        static long MaxPossibleScore;
        #endregion 

        #region Получение информации о завершенной игре
        public void SetUpEndGameWindow(long Result, long TotalGameTicks) {
            ResultLabel.Content = $"{Result} очков";
            MaxPossibleScore = TotalGameTicks;

            LoadLeaderBoardFromFile();
            UserData CurrentResult = new UserData();
            CurrentResult.Name = UserNameInput.Text;
            CurrentResult.Score = Result;

            Saves.Add(CurrentResult);
            Saves.Sort((x, y) => {
                if(x.Score.CompareTo(y.Score) != 0) return -x.Score.CompareTo(y.Score);
                else return x.Name.CompareTo(y.Name);
            });
            CurrentResultIndex = Saves.IndexOf(CurrentResult);

            UpdateLeaderBoard();
        }
        #endregion

        #region Сохранение/Загрузка
        bool MakeSureFileExists(string PathToFile) {
            if(!new FileInfo(PathToFile).Exists) {
                string CorrectPath = Environment.GetEnvironmentVariable("USERPROFILE"); // Getting user home directory
                List<string> NextDirectories = PathToFile.Replace(CorrectPath, "").Split('\\').Select(s => @"\" + s).ToList<string>();
                string FileName = NextDirectories.Last<string>();
                NextDirectories.RemoveAt(NextDirectories.Count - 1);

                foreach(string NextDirectoryName in NextDirectories) {
                    CorrectPath += NextDirectoryName;
                    DirectoryInfo DirectoryToCheck = new DirectoryInfo(CorrectPath);
                    if(!DirectoryToCheck.Exists) DirectoryToCheck.Create(); 
                }
                
                FileInfo File = new FileInfo(CorrectPath + FileName);
                if(!File.Exists) {
                    using (var CreatedFile = File.Create()) {
                        CreatedFile.Close();
                    }
                }
            }
            return true;
        }

        string ReadFromFile(string PathToFile) {
            using(FileStream fstream = File.OpenRead(PathToFile)) {
                byte[] bytes = new byte[fstream.Length];
                fstream.Read(bytes, 0, bytes.Length);
                return Decrypt(Encoding.Default.GetString(bytes));
            }
        }
        void WriteToFile(string PathToFile, string Text) {
            using (FileStream fstream = new FileStream(PathToFile, FileMode.OpenOrCreate)) {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(Encrypt(Text));
                fstream.Write(bytes, 0, bytes.Length);
            }
        }

        string Encrypt(string s) { return s; }
        string Decrypt(string s) { return s; }

        void LoadLeaderBoardFromFile() {
            bool isFileCreated = MakeSureFileExists(FullPathToFile);
            string SavedContent = "";
            if(isFileCreated) SavedContent = ReadFromFile(FullPathToFile);
            
            Saves = new List<UserData>();
            foreach (string Line in SavedContent.Split('\n')) {
                if(!string.IsNullOrEmpty(Line)) { 
                    Saves.Add(new UserData());
                    List<string> Splitted = Line.Split(' ').ToList();
                    Saves[Saves.Count - 1].Name  = string.Join(" ", Splitted.Take(Splitted.Count - 1));
                    Saves[Saves.Count - 1].Score = Convert.ToInt32(Splitted.Skip(Splitted.Count - 1).ToList()[0]);
                }
            }
        }
        public void SaveLeaderBoardToFile(object sender, System.ComponentModel.CancelEventArgs e) {
            MakeSureFileExists(FullPathToFile);
            string ContentToSave = string.Join("\n", Saves.Select(ud => $"{ud.Name} {ud.Score}"));
            WriteToFile(FullPathToFile, ContentToSave);
        }
        #endregion

        #region Ввод пользователем имени
        class UserData {
            private string name;
            public string Name { get { return name; }
                set {
                    string InvalidSymbols = @"";
                    bool InvalidName = (value.Length > 20 || value.Length < 1);
                    for (int i = 0; i < InvalidSymbols.Length && !InvalidName; i++) {
                        if(value.IndexOf(InvalidSymbols[i]) > -1) InvalidName = true;
                    }
                    
                    if(InvalidName) name = "<Your name will be here>";
                    else name = value;

                    int j = 1;
                    while(Saves.Select(ud => ud.Name).Where(s => s == name).Count() > 1) {
                        if(name.EndsWith($" ({j - 1})")) name = name.Substring(0, name.Length - 4) + $" ({j++})";
                        else name = name + $" ({j++})";
                    }
                }
            }
            public long  Score { get; set; }
        }
        private void ChangeNameOnLeaderBoard(object sender, TextChangedEventArgs e) {
            if(CurrentResultIndex >= 0) {
                Saves[CurrentResultIndex].Name = UserNameInput.Text;
                UpdateLeaderBoard();
            }
        }
        void UpdateLeaderBoard() {
            string UserDataToString(UserData ud) { return $"{ud.Score}: {ud.Name}"; }

            FlowDocument flowDocument = new FlowDocument();
            foreach (UserData Result in Saves) {
                Paragraph Line = new Paragraph();
                Run LineText = new Run(UserDataToString(Result));

                if(Result.Score >= MaxPossibleScore) LineText.Foreground = new SolidColorBrush(GetColor.DarkGreen );
                else                                 LineText.Foreground = new SolidColorBrush(GetColor.DarkestRed);

                if(Saves.IndexOf(Result) == CurrentResultIndex) LineText.FontWeight = FontWeights.Bold;
                else                                            LineText.FontWeight = FontWeights.Normal;

                Line.Inlines.Add(LineText);
                flowDocument.Blocks.Add(Line);
            }

            LeaderBoard.Document = flowDocument;
        }
        #endregion

        #region Функционал кнопок
        private void CloseWindow(object sender, RoutedEventArgs e) { (Window.GetWindow(this) as EndGameWindow).TopBarElement.CloseWindow(this, e); }
        private void PlayAgain(object sender, RoutedEventArgs e) {
            MainWindow NewGameWindow = new MainWindow();
            NewGameWindow.Show();
            Window.GetWindow(this).Close();
        }
        private void DeleteUserResultFromLeaderBoard(object sender, RoutedEventArgs e) {
            Saves.RemoveAt(CurrentResultIndex);
            CurrentResultIndex = -1;
            UpdateLeaderBoard();

            UserNameInput.Visibility = Visibility.Hidden;
            UserNameLabel.Visibility = Visibility.Hidden;
            (sender as Button).IsEnabled = false;
        }
        #endregion
    }
}
