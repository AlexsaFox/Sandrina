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
		static public Color GetMixedColor(Color c1, Color c2, double FirstToSecond = 0.5) {
			return Color.FromArgb(Convert.ToByte(c1.A * FirstToSecond + c2.A * (1 - FirstToSecond)),
								  Convert.ToByte(c1.R * FirstToSecond + c2.R * (1 - FirstToSecond)),
								  Convert.ToByte(c1.G * FirstToSecond + c2.G * (1 - FirstToSecond)),
								  Convert.ToByte(c1.B * FirstToSecond + c2.B * (1 - FirstToSecond)));
		}

		static public Color DarkRed { get; } = FromHex("#c0392b");
		static public Color Red		{ get; } = FromHex("#e74c3c");
		static public Color Orange  { get; } = FromHex("#e67e22");
		static public Color Yellow  { get; } = FromHex("#f1c40f");
		static public Color Green   { get; } = FromHex("#6ab04c");
		static public Color Blue	{ get; } = FromHex("#7ed6df");
	}
	#endregion

	public partial class Game : UserControl {
		public Game() {
            InitializeComponent();

			#region Инициализация таймера
			GameTimer.Interval  = GameTickLegth;
			GameTimer.Tick     += ChangeStats;
			GameTimer.IsEnabled = true;
            #endregion
        }

		#region Глобальные переменные
		// Некоторые временные интервалы			   d     h     m     s    ms
		TimeSpan GameTickLegth			= new TimeSpan(0,    0,    0,    1,    0);
		TimeSpan OneTimeIncreaseLength	= new TimeSpan(0,    0,    0,    0,  150);
		TimeSpan ColorChangeLength		= new TimeSpan(0,    0,    0,    0,  250);

		// Главный таймер
		DispatcherTimer GameTimer = new DispatcherTimer();

		#region Переменные изменения состояния
		double HpPerGameTick = 1;

		double FunPerGameTick = -1;

		double TemperaturePerGameTick		=	  0;
		const double FreezingSpeed			= -0.05;
		const double FreezingMaxSpeed		=    -3;
		const double WarmingPlaidSpeed		=  0.05;
		const double WarmingPlaidMaxSpeed	= 	  2;

		double EnergyPerGameTick = -1;

		double SocializationPerGameTick = -1;
		#endregion

		Random rand = new Random();

		Color a = GetColor.GetMixedColor(GetColor.Red, GetColor.Green);
		#endregion

		#region Анимация Progressbar-ов
		void AnimateBarLength(ProgressBar Bar, double Change) { AnimateBarLength(Bar, Change, GameTickLegth); }
		void AnimateBarLength(ProgressBar Bar, double Change, TimeSpan Time) {
			DoubleAnimation Animation = new DoubleAnimation();
			Animation.From = Bar.Value;
			Animation.To = Bar.Value + Change;
			Animation.Duration = Time;
			Bar.BeginAnimation(ProgressBar.ValueProperty, Animation);
		}
		void UpdateColors() {
			// Изменение цвета шкалы здоровья
			switch(HpBar.Value) {
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
			/// ???

			// Изменение цвета шкалы температуры
			switch(TemperatureBar.Value) {
				case double db when (90 < db && db <= 100):
					AnimateBarColor(TemperatureBar, GetColor.Red);
					break;

				case double db when (75 < db && db <= 90):
					AnimateBarColor(TemperatureBar, GetColor.GetMixedColor(GetColor.Green, GetColor.Red, 1 - (db - 75) / 15), GameTickLegth);
					break;

				case double db when (25 < db && db <= 75):
					AnimateBarColor(TemperatureBar, GetColor.Green);
					break;

				case double db when (10 < db && db <= 25):
					AnimateBarColor(TemperatureBar, GetColor.GetMixedColor(GetColor.Blue, GetColor.Green, 1 - (db - 10) / 15), GameTickLegth);
					break;

				case double db when (0 <= db && db <= 10):
					AnimateBarColor(TemperatureBar, GetColor.Blue);
					break;
			}

			// Изменение цвета шкалы энергии
			switch(EnergyBar.Value) {
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

		#region Функции для таймера
		void ChangeStats(object sender, EventArgs e) {
			// Изменение здоровья
			AnimateBarLength(HpBar, HpPerGameTick);

			// Изменение веселья
			AnimateBarLength(FunBar, FunPerGameTick);

			// Изменение температуры
			if(Plaid.IsChecked == true) {
				if (TemperaturePerGameTick < 0) TemperaturePerGameTick = 0;
				if (TemperaturePerGameTick < WarmingPlaidMaxSpeed) TemperaturePerGameTick += WarmingPlaidSpeed;
				if (TemperatureBar.Value >= 70) TemperaturePerGameTick = 0;
			} else {
				if(TemperaturePerGameTick > FreezingMaxSpeed) TemperaturePerGameTick += FreezingSpeed;
			}
			AnimateBarLength(TemperatureBar, TemperaturePerGameTick);

			// Изменение энергии
			AnimateBarLength(EnergyBar, EnergyPerGameTick);

			// Изменение социализации
			AnimateBarLength(SocializationBar, SocializationPerGameTick);

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
		Dictionary<string, double> GetStatsDictionary() {
			return new Dictionary<string, double> {
				{ "Hp",				0 },
				{ "Fun",			0 },
				{ "Temperature",	0 },
				{ "Energy",			0 },
				{ "Socialization",  0 }
			};
		}

		void SetCooldown(Button button, TimeSpan CooldownLength) {
			button.IsEnabled = false;

			DispatcherTimer Cooldown = new DispatcherTimer();
			Cooldown.Interval = CooldownLength;
			Cooldown.Tick += (sender, e) => RemoveCooldown(sender, e, button);
			Cooldown.IsEnabled = true;
		}

		void RemoveCooldown(object sender, EventArgs e, Button button) {
			if(!(button.Content.ToString() == "Музыка" && Headphones.IsChecked == false)) button.IsEnabled = true;
			(sender as DispatcherTimer).Stop();
		}

		void UpdateStats(Dictionary<string, double> ChangeStats) {
			if (ChangeStats["Hp"]			 != 0) AnimateBarLength(HpBar,			  ChangeStats["Hp"],			OneTimeIncreaseLength);
			if (ChangeStats["Fun"]			 != 0) AnimateBarLength(FunBar,			  ChangeStats["Fun"],			OneTimeIncreaseLength);
			if (ChangeStats["Temperature"]   != 0) AnimateBarLength(TemperatureBar,   ChangeStats["Temperature"],	OneTimeIncreaseLength);
			if (ChangeStats["Energy"]		 != 0) AnimateBarLength(EnergyBar,		  ChangeStats["Energy"],		OneTimeIncreaseLength);
			if (ChangeStats["Socialization"] != 0) AnimateBarLength(SocializationBar, ChangeStats["Socialization"], OneTimeIncreaseLength);
		}

		private void FanFictionsClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void SeriesClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void CakesClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void CoffeeClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void IceCreamClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void SleepClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void DanceClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void SocialMediaClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void WalkClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void MusicClick(object sender, RoutedEventArgs e) {
			var ChangeStats = GetStatsDictionary();

			ChangeStats["Hp"]			 =  0;
			ChangeStats["Fun"]			 =  0;
			ChangeStats["Temperature"]   =  0;
			ChangeStats["Energy"]		 =  0;
			ChangeStats["Socialization"] =  0;

			TimeSpan Cooldown = new TimeSpan(0,    0,    0,    3,    0);
			SetCooldown(sender as Button, Cooldown);
			UpdateStats(ChangeStats);
		}

		private void ChangeMusicState(object sender, RoutedEventArgs e) {
			if ((sender as CheckBox).IsChecked == true) MusicButton.IsEnabled = true;
			else MusicButton.IsEnabled = false;
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
