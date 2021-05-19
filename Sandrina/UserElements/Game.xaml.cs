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

namespace Sandrina.UserElements {
	public partial class Game : UserControl {
		public Game() {
            InitializeComponent();
			#region Инициализация таймеров
			// Таймер температуры
			TimerTemp.Interval = new TimeSpan(0, 0, 0, 10, 0);
            TimerTemp.IsEnabled = true;
            //TimerTemp.Tick += TempChange;

            // Таймер настроения
            TimerFun.Interval = new TimeSpan(0, 0, 0, 1, 0);
            TimerFun.IsEnabled = true;
            //TimerFun.Tick += FunChange;

            // Таймер здоровья
            TimerHP.Interval = new TimeSpan(0, 0, 0, 0, 200);
            TimerHP.IsEnabled = true;
            //TimerHP.Tick += HPChange;
            #endregion
        }
        #region Глобальные переменные
        DispatcherTimer TimerTemp = new DispatcherTimer();
        DispatcherTimer TimerFun = new DispatcherTimer();
        DispatcherTimer TimerHP = new DispatcherTimer();

        int MinusCold = 1;
        int MinusFun = 1;
        int CangesHP = 1;

        bool CheckHeadphone = true;
        Random r = new Random();
        #endregion
        //#region Функции для таймеров
        //// Функция температуры
        //void TempChange(object sender, EventArgs e) {
        //    if (plaid.IsChecked == false) {
        //        Cold.Value -= MinusCold;
        //        ChangeTemp();
        //    } else if (plaid.IsChecked == true) {
        //        Cold.Value += MinusCold;
        //        ChangeTemp();
        //    }
        //}

        //// Функция настроения
        //void FunChange(object sender, EventArgs e) {
        //    Fun.Value -= MinusFun;
        //}

        //// Функция здоровья
        //void HPChange(object sender, EventArgs e) {
        //    if (Cold.Value <= 30 || Cold.Value >= 80 || Fun.Value <= 40) {
        //        HP.Value -= CangesHP;
        //        ChangeHP();
        //    } else if (Fun.Value >= 70) {
        //        HP.Value += CangesHP;
        //        ChangeHP();
        //    }
        //}
        //#endregion
        //#region Изменение цвета показателей
        //void ChangeTemp() {
        //    // TODO Добавить читаемости
        //    if (Cold.Value <= 20) {
        //        Cold.Foreground = new SolidColorBrush(Color.FromRgb(62, 93, 122));
        //    } else if (Cold.Value <= 30) {
        //        Cold.Foreground = new SolidColorBrush(Color.FromRgb(130, 160, 178));
        //    } else if (Cold.Value <= 40) {
        //        Cold.Foreground = new SolidColorBrush(Color.FromRgb(198, 226, 233));
        //    } else if (Cold.Value <= 70) {
        //        Cold.Foreground = new SolidColorBrush(Color.FromRgb(163, 218, 149));
        //    } else if (Cold.Value <= 50) {
        //        Cold.Foreground = new SolidColorBrush(Color.FromRgb(255, 202, 175));
        //    } else if (Cold.Value <= 90) {
        //        Cold.Foreground = new SolidColorBrush(Color.FromRgb(241, 156, 133));
        //    } else if (Cold.Value <= 100) {
        //        Cold.Foreground = new SolidColorBrush(Color.FromRgb(226, 109, 90));
        //    }
        //}

        //void ChangeHP() {
        //    if (HP.Value == 0) {
        //        // TODO ???
        //    } else if (HP.Value <= 20) {
        //        HP.Foreground = new SolidColorBrush(Color.FromRgb(226, 109, 90));
        //    } else if (HP.Value <= 30) {
        //        HP.Foreground = new SolidColorBrush(Color.FromRgb(241, 156, 133));
        //    } else if (HP.Value <= 50) {
        //        HP.Foreground = new SolidColorBrush(Color.FromRgb(255, 202, 175));
        //    } else if (HP.Value <= 70) {
        //        HP.Foreground = new SolidColorBrush(Color.FromRgb(209, 210, 162));
        //    } else if (HP.Value <= 100) {
        //        HP.Foreground = new SolidColorBrush(Color.FromRgb(163, 218, 149));
        //    }
        //}
        //#endregion
        //#region Описание кнопок (фанфики, музыка, пироженки, кофе)
        //private void Fanfiks_Click(object sender, RoutedEventArgs e) {
        //    Fun.Value += r.Next(-10, 20);
        //}

        //private void Music_Click(object sender, RoutedEventArgs e) {
        //    Fun.Value += 10;
        //}

        //private void Cake_Click(object sender, RoutedEventArgs e) {
        //    HP.Value += 10;
        //    Fun.Value += 10;
        //}

        //private void Coffe_Click(object sender, RoutedEventArgs e) {
        //    HP.Value += 15;
        //    Fun.Value += 10;
        //    Cold.Value += 10;
        //}

        //private void headphone_Click(object sender, RoutedEventArgs e) {
        //    if (CheckHeadphone == true) {
        //        Music.IsEnabled = true;
        //        CheckHeadphone = false;
        //    } else if (CheckHeadphone == false) {
        //        Music.IsEnabled = false;
        //        CheckHeadphone = true;
        //    }

        //}
        //#endregion
        //#region Описание смены одежды
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
        //#endregion
    }
}
