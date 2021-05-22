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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;


namespace Sandrina.UserElements {
	#region Цвета
	class GetColor {
		static public Color FromHex(string Hex) {
			Hex = Hex.ToUpper();
			if (Hex.StartsWith("#")) Hex = Hex.Substring(1);
			string Alphabet = "0123456789ABCDEF";

			if (Hex.Length == 6) {
				byte[] RGB = new byte[3];

				for (int i = 0; i < 3; i++) {
					RGB[i] = Convert.ToByte(16 * Alphabet.IndexOf(Hex[i * 2]) + Alphabet.IndexOf(Hex[i * 2 + 1]));
				}

				return Color.FromRgb(RGB[0], RGB[1], RGB[2]);
			} else if (Hex.Length == 8) {
				byte[] RGB = new byte[4];

				for (int i = 0; i < 4; i++) {
					RGB[i] = Convert.ToByte(16 * Alphabet.IndexOf(Hex[i * 2]) + Alphabet.IndexOf(Hex[i * 2 + 1]));
				}

				return Color.FromArgb(RGB[0], RGB[1], RGB[2], RGB[3]);
			} else return Color.FromRgb(255, 255, 255);
		}
		static public Color Mixed(Color c1, Color c2, double FirstToSecondRatio = 0.5) {
			return Color.FromArgb(Convert.ToByte(c1.A * FirstToSecondRatio + c2.A * (1 - FirstToSecondRatio)),
								  Convert.ToByte(c1.R * FirstToSecondRatio + c2.R * (1 - FirstToSecondRatio)),
								  Convert.ToByte(c1.G * FirstToSecondRatio + c2.G * (1 - FirstToSecondRatio)),
								  Convert.ToByte(c1.B * FirstToSecondRatio + c2.B * (1 - FirstToSecondRatio)));
		}

		static public Color DarkRed    { get; } = FromHex("#c0392b");
		static public Color Red		   { get; } = FromHex("#e74c3c");
		static public Color Orange     { get; } = FromHex("#e67e22");
		static public Color Yellow     { get; } = FromHex("#f6db6f");
		static public Color Green      { get; } = FromHex("#6ab04c");
		static public Color Blue	   { get; } = FromHex("#7ed6df");
		static public Color DarkCyan   { get; } = FromHex("#78e08f");
		static public Color PastelBlue { get; } = FromHex("#60a3bc");
		static public Color DarkBlue   { get; } = FromHex("#3c6382");
	}
    #endregion

    public partial class Game : UserControl {
        public Game() {
            InitializeComponent();

            #region Инициализация таймера
            GameTimer.Interval = GameTickLegth;
            GameTimer.Tick += GameTick;
            GameTimer.IsEnabled = true;
            #endregion
            #region Инициализация уровней
            SetLevelsSettings();
            StartNextLevel();
            #endregion
        }

        #region Глобальные переменные
        // Некоторые временные интервалы			   d     h     m     s    ms
        TimeSpan GameTickLegth         =  new TimeSpan(0,    0,    0,    1,    0);
        TimeSpan OneTimeIncreaseLength =  new TimeSpan(0,    0,    0,    0,  150);
        TimeSpan ColorChangeLength     =  new TimeSpan(0,    0,    0,    0,  250);

        // Главный таймер
        DispatcherTimer GameTimer = new DispatcherTimer();

        #region Переменные изменения состояния
        double HpPerGameTick              =     1;

        double FunPerGameTick             =    -1;

        double TemperaturePerGameTick     =     0;
        double FreezingSpeed              = -0.05;
        const double FreezingMaxSpeed     =    -3;
        double WarmingPlaidSpeed          =  0.05;
        const double WarmingPlaidMaxSpeed =     2;

        double EnergyPerGameTick          =    -1;

        double SocializationPerGameTick   =    -1;
        #endregion
        
        int CurrentLevelNumber            =     0;
        long TicksTillLevelChange         =     0;

        Random rand = new Random();
        void ResetGlobalVariables() {
            GameTickLegth         =  new TimeSpan(0,    0,    0,    1,    0);
            OneTimeIncreaseLength =  new TimeSpan(0,    0,    0,    0,  150);
            ColorChangeLength     =  new TimeSpan(0,    0,    0,    0,  250);
            HpPerGameTick              =     1;
            FunPerGameTick             =    -1;
            TemperaturePerGameTick     =     0;
            FreezingSpeed              = -0.05;
            WarmingPlaidSpeed          =  0.05;
            CurrentLevelNumber         =     0;
            TicksTillLevelChange       =     0;
        }
        #endregion

        #region Анимация Progressbar-ов
        void AnimateBarLength(ProgressBar Bar, double Change) { AnimateBarLength(Bar, Change, GameTickLegth); }
        void AnimateBarLength(ProgressBar Bar, double From, double To) { AnimateBarLength(Bar, From, To, GameTickLegth); }
        void AnimateBarLength(ProgressBar Bar, double Change, TimeSpan Time) { AnimateBarLength(Bar, Bar.Value, Bar.Value + Change, Time); }
        void AnimateBarLength(ProgressBar Bar, double From, double To, TimeSpan Time, FillBehavior fillBehavior = FillBehavior.HoldEnd) {
            DoubleAnimation Animation = new DoubleAnimation();
            Animation.From = From;
            Animation.To = To;
            Animation.Duration = Time;
            Animation.FillBehavior = fillBehavior;
            Bar.BeginAnimation(ProgressBar.ValueProperty, Animation);
        }
        void UpdateColors() {
            // Изменение цвета шкалы здоровья
            switch (HpBar.Value) {
                case double db when (50 < db && db <= 100):
                    AnimateBarColor(HpBar, GetColor.Green);
                    break;

                case double db when (30 < db && db <= 50):
                    AnimateBarColor(HpBar, GetColor.Orange);
                    break;

                case double db when (10 < db && db <= 30):
                    AnimateBarColor(HpBar, GetColor.Red);
                    break;

                case double db when (0 <= db && db <= 10):
                    AnimateBarColor(HpBar, GetColor.DarkRed);
                    break;
            }

            // Изменение цвета шкалы веселья
            switch (FunBar.Value) {
                case double db when (70 < db && db <= 100):
                    AnimateBarColor(FunBar, GetColor.Green);
                    break;

                case double db when (40 < db && db <= 70):
                    AnimateBarColor(FunBar, GetColor.DarkCyan);
                    break;

                case double db when (20 < db && db <= 40):
                    AnimateBarColor(FunBar, GetColor.PastelBlue);
                    break;

                case double db when (0 <= db && db <= 20):
                    AnimateBarColor(FunBar, GetColor.DarkBlue);
                    break;
            }

            // Изменение цвета шкалы температуры
            switch (TemperatureBar.Value) {
                case double db when (90 < db && db <= 100):
                    AnimateBarColor(TemperatureBar, GetColor.Red);
                    break;

                case double db when (75 < db && db <= 90):
                    AnimateBarColor(TemperatureBar, GetColor.Mixed(GetColor.Green, GetColor.Red, 1 - (db - 75) / 15), GameTickLegth);
                    break;

                case double db when (25 < db && db <= 75):
                    AnimateBarColor(TemperatureBar, GetColor.Green);
                    break;

                case double db when (10 < db && db <= 25):
                    AnimateBarColor(TemperatureBar, GetColor.Mixed(GetColor.Blue, GetColor.Green, 1 - (db - 10) / 15), GameTickLegth);
                    break;

                case double db when (0 <= db && db <= 10):
                    AnimateBarColor(TemperatureBar, GetColor.Blue);
                    break;
            }

            // Изменение цвета шкалы энергии
            switch (EnergyBar.Value) {
                case double db when (50 < db && db <= 100):
                    AnimateBarColor(EnergyBar, GetColor.Green);
                    break;

                case double db when (30 < db && db <= 50):
                    AnimateBarColor(EnergyBar, GetColor.Yellow);
                    break;

                case double db when (10 < db && db <= 30):
                    AnimateBarColor(EnergyBar, GetColor.Orange);
                    break;

                case double db when (0 <= db && db <= 10):
                    AnimateBarColor(EnergyBar, GetColor.Red);
                    break;
            }

            // Изменение цвета шкалы социализации
            switch (SocializationBar.Value) {
                case double db when (50 < db && db <= 100):
                    AnimateBarColor(SocializationBar, GetColor.Green);
                    break;

                case double db when (30 < db && db <= 50):
                    AnimateBarColor(SocializationBar, GetColor.Yellow);
                    break;

                case double db when (10 < db && db <= 30):
                    AnimateBarColor(SocializationBar, GetColor.Orange);
                    break;

                case double db when (0 <= db && db <= 10):
                    AnimateBarColor(SocializationBar, GetColor.Red);
                    break;
            }
        }
        void AnimateBarColor(ProgressBar Bar, Color NewColor) { AnimateBarColor(Bar, NewColor, ColorChangeLength); }
        void AnimateBarColor(ProgressBar Bar, Color NewColor, TimeSpan Time) {
            ColorAnimation Animation = new ColorAnimation();
            Animation.From = (Bar.Foreground as SolidColorBrush).Color;
            Animation.To = NewColor;
            Animation.Duration = Time;
            Bar.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, Animation);
        }
        #endregion

        #region Функционал для таймера
        void GameTick(object sender, EventArgs e) {
            ChangeStats();
            ChangeLevelBarState();
        }

        double LevelProgress() {
            return (1 - (double)TicksTillLevelChange/Levels[CurrentLevelNumber - 1].TimeLengthInGameTicks) * 100;
        }

        void ChangeLevelBarState() { 
            TicksTillLevelChange -= 1;
            double Progress = LevelProgress();
            AnimateBarLength(LevelBar, LevelBar.Value, Progress, GameTickLegth);
            if(TicksTillLevelChange < 0) {
                LevelBar.Value = 0;
                StartNextLevel();
            }
        }

        void ChangeStats() {
            // Изменение здоровья
            if (HpBar.IsVisible) {
                AnimateBarLength(HpBar, HpPerGameTick);
            }

            // Изменение веселья
            if (FunBar.IsVisible) {
                AnimateBarLength(FunBar, FunPerGameTick);
            }

            // Изменение температуры
            if (TemperatureBar.IsVisible) {
                if (PlaidCheckBox.IsChecked == true) {
                    if (TemperaturePerGameTick < WarmingPlaidMaxSpeed) TemperaturePerGameTick += WarmingPlaidSpeed;
                    if (TemperatureBar.Value >= 70) TemperaturePerGameTick = 0;
                } else {
                    if (TemperaturePerGameTick > FreezingMaxSpeed) TemperaturePerGameTick += FreezingSpeed;
                }
                AnimateBarLength(TemperatureBar, TemperaturePerGameTick);
            }

            // Изменение энергии
            if (EnergyBar.IsVisible) {
                AnimateBarLength(EnergyBar, EnergyPerGameTick);
            }

            // Изменение социализации
            if (SocializationBar.IsVisible) {
                AnimateBarLength(SocializationBar, SocializationPerGameTick);
            }

            UpdateColors();
        }

        private void TabChanged(object sender, RoutedEventArgs e) {
            var Selected = Tabs.SelectedItem as TabItem;

            if (Selected.Name == "GameTab") {
                GameTimer.IsEnabled = true;
            } else {
                GameTimer.IsEnabled = false;
            }
        }
        #endregion

        #region Функционал взаимодействия
        enum BarType { Hp, Fun, Temperature, Energy, Socialization };

        Dictionary<BarType, double> GetStatsDictionary() {
            return new Dictionary<BarType, double> {
                { BarType.Hp,            0 },
                { BarType.Fun,           0 },
                { BarType.Temperature,   0 },
                { BarType.Energy,        0 },
                { BarType.Socialization, 0 }
            };
        }

        void SetCooldown(Button button, TimeSpan CooldownLength) {
            button.IsEnabled = false;

            DispatcherTimer Cooldown = new DispatcherTimer();
            Cooldown.Interval = CooldownLength;
            Cooldown.Tick += (sender, e) => RemoveCooldown(sender, e, button);
            Cooldown.Start();
        }

        void RemoveCooldown(object sender, EventArgs e, Button button) {
            if (!(button.Content.ToString() == "Музыка" && HeadphonesCheckBox.IsChecked == false)) button.IsEnabled = true;
            (sender as DispatcherTimer).Stop();
        }

        void UpdateStats(Dictionary<BarType, double> ChangeStats) {
            if (ChangeStats[BarType.Hp]            != 0 && HpBar           .IsVisible) AnimateBarLength(HpBar,            ChangeStats[BarType.Hp],            OneTimeIncreaseLength);
            if (ChangeStats[BarType.Fun]           != 0 && FunBar          .IsVisible) AnimateBarLength(FunBar,           ChangeStats[BarType.Fun],           OneTimeIncreaseLength);
            if (ChangeStats[BarType.Temperature]   != 0 && TemperatureBar  .IsVisible) AnimateBarLength(TemperatureBar,   ChangeStats[BarType.Temperature],   OneTimeIncreaseLength);
            if (ChangeStats[BarType.Energy]        != 0 && EnergyBar       .IsVisible) AnimateBarLength(EnergyBar,        ChangeStats[BarType.Energy],        OneTimeIncreaseLength);
            if (ChangeStats[BarType.Socialization] != 0 && SocializationBar.IsVisible) AnimateBarLength(SocializationBar, ChangeStats[BarType.Socialization], OneTimeIncreaseLength);
        }

        private void FanFictionsClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();

            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void SeriesClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void CakesClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void CoffeeClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void IceCreamClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void SleepClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void DanceClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();

            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void SocialMediaClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void WalkClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void MusicClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 0;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void ChangeMusicState(object sender, RoutedEventArgs e) {
            if ((sender as CheckBox).IsChecked == true) MusicButton.IsEnabled = true;
            else MusicButton.IsEnabled = false;
        }
        #endregion

        #region Одежда (???)
        enum OutfitType {

        }
        #endregion

        #region Уровни
        abstract class Level {
            public Color BarColor { get; set; }
            public long TimeLengthInGameTicks { get; set; }
        }
        class AddItemsLevel : Level {
            public List<UIElement> NewUIElements { get; set; }
            public List<OutfitType> RewardOutfit { get; set; }

            public AddItemsLevel(List<UIElement> NewUIElements, List<OutfitType> RewardOutfit, Color BarColor, long TimeLengthInGameTicks) {
                this.NewUIElements = NewUIElements;
                this.RewardOutfit = RewardOutfit;
                this.BarColor = BarColor;
                this.TimeLengthInGameTicks = TimeLengthInGameTicks;
            }
            public AddItemsLevel() : this(new List<UIElement>(), new List<OutfitType>(), Colors.White, 1) { }
        }
        class IncreaseDifficultyLevel : Level {
            public Dictionary<BarType, double> NewBarSpeed { get; set; }
            public TimeSpan NewGameSpeed { get; set; }

            public IncreaseDifficultyLevel(Dictionary<BarType, double> NewBarSpeed, TimeSpan NewGameSpeed, Color BarColor, long TimeLengthInGameTicks) {
                this.NewBarSpeed = NewBarSpeed;
                this.NewGameSpeed = NewGameSpeed;
                this.BarColor = BarColor;
                this.TimeLengthInGameTicks = TimeLengthInGameTicks;
            }
            public IncreaseDifficultyLevel() : this(new Dictionary<BarType, double>(), new TimeSpan(1), Colors.White, 1) { }
        }

        List<Level> Levels = new List<Level>();
        void SetLevelsSettings() {
            Levels = new List<Level> {
                new AddItemsLevel(		// Описание первого уровня
					new List<UIElement> {
                        HpLabel,
                        HpBar,
                        FunLabel,
                        FunBar,
                        FanFictionsButton,
                        SeriesButton,
                        CakesButton,
                        MusicButton,
                        PlaidCheckBox,
                        HeadphonesCheckBox
                    },
                    new List<OutfitType> { },
                    GetColor.Green,
                    5
                ),
                new AddItemsLevel(		// Описание второго уровня
					new List<UIElement> {
                        TemperatureLabel,
                        TemperatureBar,
                        CofeeButton,
                        IceCreamButton,
                        PlaidCheckBox
                    },
                    new List<OutfitType> { },
                    GetColor.Green,
                    5
                ),
                new AddItemsLevel(		// Описание третьего уровня
					new List<UIElement> {
                        EnergyLabel,
                        EnergyBar,
                        SleepButton,
                        DanceButton
                    },
                    new List<OutfitType> { },
                    GetColor.Green,
                    5
                ),
                new AddItemsLevel(		// Описание четвертого уровня
					new List<UIElement> {
                        SocializationLabel,
                        SocializationBar,
                        SocialMediaButton,
                        WalkButton
                    },
                    new List<OutfitType> { },
                    GetColor.Green,
                    5
                ),
                new IncreaseDifficultyLevel(		// Описание пятого уровня
					new Dictionary<BarType, double> {
                        { BarType.Hp,              1 },
                        { BarType.Fun,            -1 },
                        { BarType.Temperature,  0.05 },
                        { BarType.Energy,         -1 },
                        { BarType.Socialization,  -1 }
                    },
                    new TimeSpan(0, 0, 0, 1, 0),
                    GetColor.DarkRed,
                    5
                ),
            };
        }

        void StartNextLevel() {
            if(CurrentLevelNumber == Levels.Count) { // Если игра пройдена
                GameTimer.Stop();
            } 

            Level CurrentLevel = Levels[CurrentLevelNumber++];
            CurrentLevelLabel.Content = CurrentLevelNumber.ToString();

            switch(CurrentLevel) {
                case AddItemsLevel AILevel:
                    foreach(UIElement elem in AILevel.NewUIElements) {
                        elem.Visibility = Visibility.Visible;
                    }
                    break;

                case IncreaseDifficultyLevel IDLevel:
                    HpPerGameTick            = IDLevel.NewBarSpeed[BarType.Hp];
                    FunPerGameTick           = IDLevel.NewBarSpeed[BarType.Fun];
                    FreezingSpeed            = IDLevel.NewBarSpeed[BarType.Temperature];
                    EnergyPerGameTick        = IDLevel.NewBarSpeed[BarType.Energy];
                    SocializationPerGameTick = IDLevel.NewBarSpeed[BarType.Socialization];
                    GameTickLegth = IDLevel.NewGameSpeed;
                    GameTimer.Interval = GameTickLegth;
                    break;
            }

            LevelBar.Foreground = new SolidColorBrush(CurrentLevel.BarColor);
            TicksTillLevelChange = CurrentLevel.TimeLengthInGameTicks - 1;
            AnimateBarLength(LevelBar, 0, 100 * 1d/CurrentLevel.TimeLengthInGameTicks, GameTickLegth);
        }
        #endregion

        #region Описание смены одежды (закомментировано)
        //private void Get_dressed_Click(object sender, RoutedEventArgs e) {
        //    if (Polly.IsChecked == true) {
        //        CostumePolly.Visibility = Visibility.Visible;
        //    }
        //    if (Polly.IsChecked == false) {
        //        CostumePolly.Visibility = Visibility.Hidden;
        //    }
        //    if (Alex.IsChecked == true) {
        //        AlexCostume.Visibility = Visibility.Visible;
        //    }
        //    if (Alex.IsChecked == false) {
        //        AlexCostume.Visibility = Visibility.Hidden;
        //    }
        //    if (Polly.IsChecked == true || Alex.IsChecked == true) {
        //        Main.IsEnabled = true;
        //    }
        //}

        //private void Pets_Checked(object sender, RoutedEventArgs e) {
        //    Cat.IsEnabled = true;
        //    Dog.IsEnabled = true;
        //}

        //private void Pets_Unchecked(object sender, RoutedEventArgs e) {
        //    Cat.IsEnabled = false;
        //    Dog.IsEnabled = false;
        //    CatImage.Visibility = Visibility.Hidden;
        //    DogImage.Visibility = Visibility.Hidden;
        //    Cat.IsChecked = false;
        //    Dog.IsChecked = false;
        //}

        //private void Cat_Checked(object sender, RoutedEventArgs e) { CatImage.Visibility = Visibility.Visible; }
        //private void Cat_Unchecked(object sender, RoutedEventArgs e) { CatImage.Visibility = Visibility.Hidden; }
        //private void Dog_Checked(object sender, RoutedEventArgs e) { DogImage.Visibility = Visibility.Visible; }
        //private void Dog_Unchecked(object sender, RoutedEventArgs e) { DogImage.Visibility = Visibility.Hidden; }

        //private void Polly_Checked(object sender, RoutedEventArgs e) {
        //    Image Photo = new Image();
        //    Photo.Height = 100;
        //    Photo.Width = 80;
        //    // TODO: Photo.Source = new BitmapImage(new Uri("D:\\МШП\\Проекты\\По шарпу\\Картинки\\Фон Одежда Полины.jpeg"));
        //    Box.Items.Add(Photo);
        //}

        //private void Alex_Checked(object sender, RoutedEventArgs e) {
        //    Image Photo1 = new Image();
        //    Photo1.Height = 100;
        //    Photo1.Width = 80;
        //    // TODO: Photo1.Source = new BitmapImage(new Uri("D:\\МШП\\Проекты\\По шарпу\\Картинки\\Фон Одежда Саши.jpeg"));
        //    Box.Items.Add(Photo1);
        //}
        #endregion
    }
}
