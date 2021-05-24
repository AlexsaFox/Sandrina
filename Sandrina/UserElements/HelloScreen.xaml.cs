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
	public partial class HelloScreen : UserControl {
		public HelloScreen() {
			InitializeComponent();
		}

		private void GotoInstructions(object sender, RoutedEventArgs e) {
			MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
			mainWindow.GotoInstructions(this);
			mainWindow.TopBarElement.ChangeTopBarColor(GetColor.DarkPeach);
			mainWindow.TopBarElement.ChangeTopBarButtonsMode(Mode.Light);
		}
	}
}
