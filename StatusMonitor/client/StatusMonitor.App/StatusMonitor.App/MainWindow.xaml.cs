using StatusMonitor.Model;
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

namespace StatusMonitor.App
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string TeamCityServerPath
        {
            get { return (string)GetValue(TeamCityServerPathProperty); }
            set { SetValue(TeamCityServerPathProperty, value); }
        }
        public static readonly DependencyProperty TeamCityServerPathProperty =
            DependencyProperty.Register("TeamCityServerPath", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public string TeamCityUsername
        {
            get { return (string)GetValue(TeamCityUsernameProperty); }
            set { SetValue(TeamCityUsernameProperty, value); }
        }
        public static readonly DependencyProperty TeamCityUsernameProperty =
            DependencyProperty.Register("TeamCityUsername", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Initialize();
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            // Title = WPFLocalizationForDummies.Properties.Resources.Title;
        }

        private void Initialize()
        {
            UpdateTeamCityViewContent();
            TeamCityWatcher.Instance.Start();
        }

        public void UpdateTeamCityViewContent()
        {
            TeamCityServerPath = AppSettingsHelper.Instance.Settings.TeamCityServerPath;
            TeamCityUsername = AppSettingsHelper.Instance.Settings.TeamCityUserName;
            pwTcPassword.Password = !string.IsNullOrEmpty(AppSettingsHelper.Instance.Settings.TeamCityPassword) ? "If you can see this: Nice try!" : "";
        }

        private void BtnTeamCityConnect_OnClick(object sender, RoutedEventArgs e)
        {
            AppSettingsHelper.Instance.Settings.TeamCityServerPath = TeamCityServerPath;
            AppSettingsHelper.Instance.Settings.TeamCityUserName = TeamCityUsername;

            if (pwTcPassword.Password != "If you can see this: Nice try!")
                AppSettingsHelper.Instance.Settings.TeamCityPassword = pwTcPassword.Password;

            AppSettingsHelper.Instance.Save();

            TeamCityWatcher.Instance.Start();
        }
    }
}
