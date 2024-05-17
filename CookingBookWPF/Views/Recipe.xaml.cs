using System.Windows.Controls;

namespace CookingBookWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для Recipe.xaml
    /// </summary>
    public partial class RecipePage : UserControl
    {
        public RecipePage()
        {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                scroll.LineUp();
            }
            else
            {
                scroll.LineDown();
            }
            e.Handled = true;
        }
    }
}
