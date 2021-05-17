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
	public partial class Instructions : UserControl {
		public Instructions() {
			InitializeComponent();
		}

		#region Глобальные переменные
		Color LightYellow = Color.FromRgb(241, 255, 196);
		#endregion

		private void GotoGame(object sender, RoutedEventArgs e) {
			MainWindow parentWindow = (MainWindow)Window.GetWindow(this);
			parentWindow.GotoGame(this);
			parentWindow.ChangeTopBarColor(LightYellow);
			parentWindow.ChangeTopBarButtonsMode(Mode.Dark);
		}
	}
}
