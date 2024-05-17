using System.Windows;
using System.Windows.Input;

namespace CookingBookWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для Application.xaml
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        public ApplicationWindow()
        {
            InitializeComponent();
        }

        private void HamburgerMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Window window)
            {
                var mouseX = Mouse.GetPosition(window).X;
                if (mouseX > 0 && mouseX < 50)
                {
                }
                else
                {
                    hamburger.IsOpen = false;
                }
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
