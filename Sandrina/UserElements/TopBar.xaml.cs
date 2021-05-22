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

namespace Sandrina.UserElements {
    /// <summary>
    /// Логика взаимодействия для TopBar.xaml
    /// </summary>
    public partial class TopBar : UserControl {
        public TopBar() {
            InitializeComponent();
        }

        public void ChangeTopBarColor(Color color) {
            TopBarGrid.Background = new SolidColorBrush(color);
		}
        public void ChangeTopBarButtonsMode(Mode mode) {
            switch(mode) {
                case Mode.Dark:
                    Resources["BarButtonForeground"]  = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                    break;

                case Mode.Light:
                    Resources["BarButtonForeground"] = new SolidColorBrush(Color.FromRgb(221, 221, 221));
                    break;
			}
		}
		private void DragWindow(object sender, MouseButtonEventArgs e) {
            if(e.ChangedButton == MouseButton.Left) {
                Window.GetWindow(this).DragMove();
			}
		}
		private void CloseWindow(object sender, RoutedEventArgs e) {
            Window.GetWindow(this).Close();
		}
        private void HideWindow(object sender, RoutedEventArgs e) {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }
        private void ResizeWindow(object sender, RoutedEventArgs e) {
            if(Window.GetWindow(this).WindowState == WindowState.Maximized) {
                Window.GetWindow(this).WindowState = WindowState.Normal;
            } else {
                Window.GetWindow(this).WindowState = WindowState.Maximized;
            }
        }
    }
}
