using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize the database
            DatabaseHelper.InitializeDatabase();

            // Show the main window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

}
