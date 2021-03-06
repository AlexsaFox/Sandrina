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

		static public Color Blue	    { get; } = FromHex("#7ed6df");
		static public Color DarkestRed  { get; } = FromHex("#86271e");
		static public Color DarkBlue    { get; } = FromHex("#3c6382");
		static public Color DarkCyan    { get; } = FromHex("#78e08f");
		static public Color DarkGreen   { get; } = FromHex("#023c03");
        static public Color DarkPeach   { get; } = FromHex("#c07c6a");
		static public Color DarkRed     { get; } = FromHex("#c0392b");
		static public Color Green       { get; } = FromHex("#6ab04c");
        static public Color LightYellow { get; } = FromHex("#f1ffc4");
		static public Color Orange      { get; } = FromHex("#e67e22");
		static public Color PastelBlue  { get; } = FromHex("#60a3bc");
		static public Color Red		    { get; } = FromHex("#e74c3c");
		static public Color Yellow      { get; } = FromHex("#f6db6f");
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
            TotalGameTicks = Levels.Select(lvl => lvl.TimeLengthInGameTicks).Aggregate((x, y) => x + y);
            StartNextLevel();
            #endregion
        }

        #region Глобальные переменные
        // Некоторые временные интервалы			   d     h     m     s    ms
        TimeSpan GameTickLegth         =  new TimeSpan(0,    0,    0,    1,    0);
        TimeSpan OneTimeIncreaseLength =  new TimeSpan(0,    0,    0,    0,  100);
        TimeSpan ColorChangeLength     =  new TimeSpan(0,    0,    0,    0,  250);

        // Главный таймер
        DispatcherTimer GameTimer = new DispatcherTimer();

        #region Переменные изменения состояния
        double HpPerGameTick                  =     1;
                                              
        double FunPerGameTick                 =    -5;
        long TicksWithMusicOnLeft             =    20;
                                              
        double TemperaturePerGameTick         =     0;
        double FreezingSpeed                  =  -0.3;
        const double FreezingMaxSpeed         =    -5;
        double WarmingPlaidSpeed              =   0.5;
        const double WarmingPlaidMaxSpeed     =     3;
        int TicksFromCofeeToIceCream          =     0;
        const int MinTicksFromCofeeToIceCream =     2;

        double EnergyPerGameTick              =    -5;
        double EnergyBooster                  =     1;
                                              
        double SocializationPerGameTick       =    -5;
        #endregion                            
                                              
        int CurrentLevelNumber                =     0;
        long TicksTillLevelChange             =     0;
        long GameTicksAlive                   =     0;
        long TotalGameTicks                   =     0;

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
            GameTicksAlive++;
            ChangeLevelBarState();
            ChangeGameScreenOutfit();

            if(HpBar.Value == 0) {
                OverGame();
            }
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
            double ChangeHealth = 0;

            // Изменение веселья
            if (FunBar.IsVisible) {
                AnimateBarLength(FunBar, FunPerGameTick);

                if(FunBar.Value >= 97) {
                    if(rand.Next(0, 3) == 0) ChangeHealth -= 5;
                }
                if(FunBar.Value <= 20) {
                    ChangeHealth -= 5;
                }

                if(HeadphonesCheckBox.IsChecked == true) TicksWithMusicOnLeft = Math.Max(  0, TicksWithMusicOnLeft - 1);
                else                                     TicksWithMusicOnLeft = Math.Min(100, TicksWithMusicOnLeft + 1);

                if(TicksWithMusicOnLeft < 0) ChangeHealth -= 2;
            }

            // Изменение температуры
            if (TemperatureBar.IsVisible) {
                if (PlaidCheckBox.IsChecked == true) {
                    if (TemperaturePerGameTick < WarmingPlaidMaxSpeed) TemperaturePerGameTick += WarmingPlaidSpeed;
                    if (TemperatureBar.Value >= 90) TemperaturePerGameTick = 0;
                } else {
                    if (TemperaturePerGameTick > FreezingMaxSpeed) TemperaturePerGameTick += FreezingSpeed;
                }
                AnimateBarLength(TemperatureBar, TemperaturePerGameTick);

                if(TemperatureBar.Value >= 75) ChangeHealth -= (TemperatureBar.Value - 75)/5;
                if(TemperatureBar.Value <= 25) ChangeHealth -= (TemperatureBar.Value)     /5;

                if(TicksFromCofeeToIceCream > 0) TicksFromCofeeToIceCream--;
                if(TicksFromCofeeToIceCream < 0) TicksFromCofeeToIceCream++;
            }

            // Изменение энергии
            if (EnergyBar.IsVisible) {
                AnimateBarLength(EnergyBar, EnergyPerGameTick);

                if(EnergyBar.Value <= 20) ChangeHealth -= rand.Next(0, 10);

                switch(EnergyBar.Value) {
                    case double db when (90 <= db && db <= 100):
                        EnergyBooster = 1.2;
                        break;
                    
                    case double db when (50 <= db && db < 90):
                        EnergyBooster = 1.0;
                        break;
                    
                    case double db when (20 <= db && db < 50):
                        EnergyBooster = 0.8;
                        break;
                    
                    case double db when (0 <= db && db < 20):
                        EnergyBooster = 0.5;
                        break;
                }
            }

            // Изменение социализации
            if (SocializationBar.IsVisible) {
                AnimateBarLength(SocializationBar, SocializationPerGameTick);

                if(SocializationBar.Value <= 20) ChangeHealth -= rand.Next(0, 10);
            }
            
            // Изменение здоровья
            if (HpBar.IsVisible) {
                AnimateBarLength(HpBar, HpPerGameTick + ChangeHealth);
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

        bool MusicButtonCooldownIsRunning = false;
        void SetCooldown(Button button, TimeSpan CooldownLength) {
            button.IsEnabled = false;
            if(button.Name == "MusicButton") MusicButtonCooldownIsRunning = true;

            DispatcherTimer Cooldown = new DispatcherTimer();
            Cooldown.Interval = CooldownLength;
            Cooldown.Tick += (sender, e) => RemoveCooldown(sender, e, button);
            Cooldown.Start();
        }

        void RemoveCooldown(object sender, EventArgs e, Button button) {
            if(button.Content.ToString() == "Музыка") {
                MusicButtonCooldownIsRunning = false;

                if (HeadphonesCheckBox.IsChecked == true) MusicButton.IsEnabled = true;
                else                                      MusicButton.IsEnabled = false;
            } else {
                button.IsEnabled = true;
            }

            (sender as DispatcherTimer).Stop();
        }

        void UpdateStats(Dictionary<BarType, double> ChangeStats) {
            if (ChangeStats[BarType.Hp]            != 0 && HpBar           .IsVisible) AnimateBarLength(HpBar,            (ChangeStats[BarType.Hp]            > 0 ? ChangeStats[BarType.Hp]            * EnergyBooster : ChangeStats[BarType.Hp]           ), OneTimeIncreaseLength);
            if (ChangeStats[BarType.Fun]           != 0 && FunBar          .IsVisible) AnimateBarLength(FunBar,           (ChangeStats[BarType.Fun]           > 0 ? ChangeStats[BarType.Fun]           * EnergyBooster : ChangeStats[BarType.Fun]          ), OneTimeIncreaseLength);
            if (ChangeStats[BarType.Temperature]   != 0 && TemperatureBar  .IsVisible) AnimateBarLength(TemperatureBar,   (ChangeStats[BarType.Temperature]   > 0 ? ChangeStats[BarType.Temperature]   * EnergyBooster : ChangeStats[BarType.Temperature]  ), OneTimeIncreaseLength);
            if (ChangeStats[BarType.Energy]        != 0 && EnergyBar       .IsVisible) AnimateBarLength(EnergyBar,         ChangeStats[BarType.Energy],                                                                                                       OneTimeIncreaseLength);
            if (ChangeStats[BarType.Socialization] != 0 && SocializationBar.IsVisible) AnimateBarLength(SocializationBar, (ChangeStats[BarType.Socialization] > 0 ? ChangeStats[BarType.Socialization] * EnergyBooster : ChangeStats[BarType.Socialization]), OneTimeIncreaseLength);
        }

        private void FanFictionsClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();

            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = rand.Next(-5, 16);
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 5;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void SeriesClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = rand.Next(-5, 16);
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 0;
            ChangeStats[BarType.Socialization] = 5;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void CakesClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = -5;
            ChangeStats[BarType.Fun]           = 10;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = 5;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 4, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void CoffeeClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 5;
            ChangeStats[BarType.Temperature]   = 10;
            ChangeStats[BarType.Energy]        = 15;
            ChangeStats[BarType.Socialization] = 0;

            if(TicksFromCofeeToIceCream < 0) ChangeStats[BarType.Hp] -= 10;
            TicksFromCofeeToIceCream += MinTicksFromCofeeToIceCream;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void IceCreamClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 10;
            ChangeStats[BarType.Temperature]   = -10;
            ChangeStats[BarType.Energy]        = 5;
            ChangeStats[BarType.Socialization] = 0;
            
            if(TicksFromCofeeToIceCream > 0) ChangeStats[BarType.Hp] -= 10;
            TicksFromCofeeToIceCream -= MinTicksFromCofeeToIceCream;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void SleepClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 10;
            ChangeStats[BarType.Fun]           = -10;
            ChangeStats[BarType.Temperature]   = rand.Next(-5, 6);
            ChangeStats[BarType.Energy]        = 60;
            ChangeStats[BarType.Socialization] = -10;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 10, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void DanceClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();

            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = rand.Next(5, 16);
            ChangeStats[BarType.Temperature]   = 5;
            ChangeStats[BarType.Energy]        = -10;
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void SocialMediaClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = rand.Next(-5, 11);
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = -5;
            ChangeStats[BarType.Socialization] = 10;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void WalkClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 15;
            ChangeStats[BarType.Temperature]   = -5;
            ChangeStats[BarType.Energy]        = -10;
            ChangeStats[BarType.Socialization] = 20;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 3, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void MusicClick(object sender, RoutedEventArgs e) {
            var ChangeStats = GetStatsDictionary();
            
            ChangeStats[BarType.Hp]            = 0;
            ChangeStats[BarType.Fun]           = 15;
            ChangeStats[BarType.Temperature]   = 0;
            ChangeStats[BarType.Energy]        = rand.Next(-5, 16);
            ChangeStats[BarType.Socialization] = 0;

            TimeSpan Cooldown = new TimeSpan(0, 0, 0, 5, 0);
            SetCooldown(sender as Button, Cooldown);
            UpdateStats(ChangeStats);
        }

        private void ChangeMusicState(object sender, RoutedEventArgs e) {
            if(!MusicButtonCooldownIsRunning) { 
                if ((sender as CheckBox).IsChecked == true) MusicButton.IsEnabled = true;
                else MusicButton.IsEnabled = false;
            }

            ChangeMusicVisibility();
        }
        private void ChangePlaidState(object sender, RoutedEventArgs e) {
            ChangePlaidVisibility();
        }
        #endregion

        #region Уровни
        abstract class Level {
            public Color BarColor { get; set; }
            public long TimeLengthInGameTicks { get; set; }
        }
        class AddItemsLevel : Level {
            public List<UIElement> NewUIElements { get; set; }

            public AddItemsLevel(List<UIElement> NewUIElements, Color BarColor, long TimeLengthInGameTicks) {
                this.NewUIElements = NewUIElements;
                this.BarColor = BarColor;
                this.TimeLengthInGameTicks = TimeLengthInGameTicks;
            }
            public AddItemsLevel() : this(new List<UIElement>(), Colors.White, 1) { }
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

        static List<Level> Levels = new List<Level>();
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
                        HeadphonesCheckBox
                    },
                    GetColor.Green,
                    20
                ),
                new AddItemsLevel(		// Описание второго уровня
					new List<UIElement> {
                        TemperatureLabel,
                        TemperatureBar,
                        CofeeButton,
                        IceCreamButton,
                        PlaidCheckBox
                    },
                    GetColor.Green,
                    30
                ),
                new AddItemsLevel(		// Описание третьего уровня
					new List<UIElement> {
                        EnergyLabel,
                        EnergyBar,
                        SleepButton,
                        DanceButton
                    },
                    GetColor.Green,
                    60
                ),
                new AddItemsLevel(		// Описание четвертого уровня
					new List<UIElement> {
                        SocializationLabel,
                        SocializationBar,
                        SocialMediaButton,
                        WalkButton
                    },
                    GetColor.Green,
                    90
                ),
                new IncreaseDifficultyLevel(		// Описание пятого уровня
					new Dictionary<BarType, double> {
                        { BarType.Hp,               0 },
                        { BarType.Fun,           -7.5 },
                        { BarType.Temperature,   -0.5 },
                        { BarType.Energy,        -7.5 },
                        { BarType.Socialization, -7.5 }
                    },
                    new TimeSpan(0, 0, 0, 1, 0),
                    GetColor.DarkRed,
                    120
                ),
            };
        }

        void OverGame() {
            GameTimer.Stop();
            EndGameWindow ResultWindow = new EndGameWindow();
            ResultWindow.Show();
            ResultWindow.SetUpEndGameWindow(GameTicksAlive, TotalGameTicks);
            Window.GetWindow(this).Close();
        }

        void StartNextLevel() {
            if(CurrentLevelNumber == Levels.Count) {
                OverGame();
                return;
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
        
        #region Смена изображения на вкладке игры
        void ChangeGameScreenHairStyle() {
            if(LooseHairRB.IsChecked == true && LightHairRB .IsChecked == true) SandrinaHairImage.Source = new BitmapImage(new Uri(@"/Images/Hair/SandrinaLooseLightHair.png" , UriKind.Relative));
            if(LooseHairRB.IsChecked == true && BlondeHairRB.IsChecked == true) SandrinaHairImage.Source = new BitmapImage(new Uri(@"/Images/Hair/SandrinaLooseBlondeHair.png", UriKind.Relative));
            if(BunHairRB  .IsChecked == true && LightHairRB .IsChecked == true) SandrinaHairImage.Source = new BitmapImage(new Uri(@"/Images/Hair/SandrinaBunLightHair.png"   , UriKind.Relative));
            if(BunHairRB  .IsChecked == true && BlondeHairRB.IsChecked == true) SandrinaHairImage.Source = new BitmapImage(new Uri(@"/Images/Hair/SandrinaBunBlondeHair.png"  , UriKind.Relative));
        }

        void ChangeGameScreenOutfit() {
            string OutfitFolderName = "";
            string StateFileName    = "";

            switch(GetSelectedOutitType()) {
                case Outfit.Polina:
                    OutfitFolderName = "PolinaOutfitStates";
                    break;
                    
                case Outfit.Sasha:
                    OutfitFolderName = "SashaOutfitStates";
                    break;
                    
                case Outfit.FirstExtra:
                    OutfitFolderName = "FirstExtraOutfitStates";
                    break;
                    
                case Outfit.SecondExtra:
                    OutfitFolderName = "SecondExtraOutfitStates";
                    break;
            }

            switch(HpBar.Value) {
                case double db when 75 <= db && db <= 100:
                    StateFileName = "VeryHappy";
                    break;
                    
                case double db when 50 <= db && db < 75:
                    StateFileName = "Happy";
                    break;
                    
                case double db when 25 <= db && db < 50:
                    StateFileName = "Normal";
                    break;
                    
                case double db when 0 <= db && db < 25:
                    StateFileName = "Sad";
                    break;
            }
            
            SandrinaStateImage.Source = new BitmapImage(new Uri($@"/Images/Outfits/WithSapo/{OutfitFolderName}/{StateFileName}.png"  , UriKind.Relative));
        }

        void ChangeMusicVisibility() {
            if(HeadphonesCheckBox.IsChecked == true) SanrdinaHeadphonesImage.Visibility = Visibility.Visible;
            else                                     SanrdinaHeadphonesImage.Visibility = Visibility.Hidden;
        }

        void ChangePlaidVisibility() {
            if(PlaidCheckBox.IsChecked == true) {
                SanrdinaPlaidForegroundImage.Visibility = Visibility.Visible;
                SanrdinaPlaidBackgroundImage.Visibility = Visibility.Visible;
            }
            else {
                SanrdinaPlaidForegroundImage.Visibility = Visibility.Hidden;
                SanrdinaPlaidBackgroundImage.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Функционал гардероба
        enum Outfit { None, Polina, Sasha, FirstExtra, SecondExtra }

        private void HairStyleColorClick(object sender, RoutedEventArgs e) {
            LooseLightHairImage.Visibility  = Visibility.Hidden;
            LooseBlondeHairImage.Visibility = Visibility.Hidden;
            BunLightHairImage.Visibility    = Visibility.Hidden;
            BunBlondeHairImage.Visibility   = Visibility.Hidden;

            if(LooseHairRB.IsChecked == true && LightHairRB .IsChecked == true) LooseLightHairImage.Visibility  = Visibility.Visible;
            if(LooseHairRB.IsChecked == true && BlondeHairRB.IsChecked == true) LooseBlondeHairImage.Visibility = Visibility.Visible;
            if(BunHairRB  .IsChecked == true && LightHairRB .IsChecked == true) BunLightHairImage.Visibility    = Visibility.Visible;
            if(BunHairRB  .IsChecked == true && BlondeHairRB.IsChecked == true) BunBlondeHairImage.Visibility   = Visibility.Visible;

            ChangeGameScreenHairStyle();
        }

        Outfit GetSelectedOutitType() {
            switch(WardrobeListbox.SelectedItem) {
                case Image Img when Img.Name == "PolinaOutfit":
                    return Outfit.Polina;
                    
                case Image Img when Img.Name == "SashaOutfit":
                    return Outfit.Sasha;
                    
                case Image Img when Img.Name == "FirstExtraOutfit":
                    return Outfit.FirstExtra;
                    
                case Image Img when Img.Name == "SecondExtraOutfit":
                    return Outfit.SecondExtra;

                default:
                    return Outfit.None;
            }
        }

        private void ChangeOutfit(object sender, SelectionChangedEventArgs e) {
            SandrinaDefaultOutfit    .Visibility = Visibility.Hidden;
            SanrdinaPolinaOutfit     .Visibility = Visibility.Hidden;
            SandrinaSashaOutfit      .Visibility = Visibility.Hidden;
            SandrinaFirstExtraOutfit .Visibility = Visibility.Hidden;
            SandrinaSecondExtraOutfit.Visibility = Visibility.Hidden;

            switch(GetSelectedOutitType()) {
                case Outfit.Polina:
                    SanrdinaPolinaOutfit.Visibility = Visibility.Visible;
                    break;
                    
                case Outfit.Sasha:
                    SandrinaSashaOutfit.Visibility = Visibility.Visible;
                    break;
                    
                case Outfit.FirstExtra:
                    SandrinaFirstExtraOutfit.Visibility = Visibility.Visible;
                    break;
                    
                case Outfit.SecondExtra:
                    SandrinaSecondExtraOutfit.Visibility = Visibility.Visible;
                    break;
            }

            ChangeGameScreenOutfit();
            EnableGameTab();
        }
        void EnableGameTab() {
            if(!GameTab.IsEnabled) {
                GameTab.IsEnabled = true;
                (GameTab.ToolTip as ToolTip).Content = "Следи за показателями!";
            }
        }

        private void ChangeGridVisibility(object sender, RoutedEventArgs e) {
            if((sender as MenuItem).Name == "OutfitGridMenuItem") {
                if(GridWithHair.IsVisible) {
                    GridWithOutfits.Visibility = Visibility.Visible;
                    GridWithHair   .Visibility = Visibility.Hidden;
                } else {
                    if(GridWithOutfits.IsVisible) GridWithOutfits.Visibility = Visibility.Hidden;
                    else                          GridWithOutfits.Visibility = Visibility.Visible;
                }
            } else if((sender as MenuItem).Name == "HairGridMenuItem") {
                if(GridWithOutfits.IsVisible) {
                    GridWithOutfits.Visibility = Visibility.Hidden;
                    GridWithHair   .Visibility = Visibility.Visible;
                } else {
                    if(GridWithHair.IsVisible) GridWithHair.Visibility = Visibility.Hidden;
                    else                          GridWithHair.Visibility = Visibility.Visible;
                }
            }
        }
        #endregion
    }
}
