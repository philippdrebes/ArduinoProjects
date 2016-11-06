using StatusMonitor.Business;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<string> AvailablePorts { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Initialize();
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            // Title = StatusMonitor.App.Properties.Resources.Title;
        }

        private void Initialize()
        {
            // Arduino connection
            AvailablePorts = new ObservableCollection<string>();
            AvailablePorts.Add(StatusMonitor.App.Properties.Resources.CmbArduinoSerialPortDefault);

            foreach (string port in ArduinoCommunicator.Instance.AvailablePorts)
                AvailablePorts.Add(port);

            // TeamCity connection
            UpdateTeamCityViewContent();
            TeamCityWatcher.Instance.Start();

            lbTeamCityProjects.ItemsSource = TeamCityWatcher.Instance.Projects;
        }

        public void UpdateTeamCityViewContent()
        {
            TeamCityServerPath = AppSettingsHelper.Instance.Settings?.TeamCityServerPath;
            TeamCityUsername = AppSettingsHelper.Instance.Settings?.TeamCityUserName;
            pwTcPassword.Password = !string.IsNullOrEmpty(AppSettingsHelper.Instance.Settings?.TeamCityPassword) ? "If you can see this: Nice try!" : "";
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

        private void Window_Closed(object sender, EventArgs e)
        {
            TeamCityWatcher.Instance.Stop();
        }

        private void cmbArduinoPort_Selected(object sender, RoutedEventArgs e)
        {
            if (ArduinoCommunicator.Instance.IsRunning) ArduinoCommunicator.Instance.Stop();
            if (cmbArduinoPort.SelectedItem.ToString() != Properties.Resources.CmbArduinoSerialPortDefault)
                ArduinoCommunicator.Instance.Start(cmbArduinoPort.SelectedItem.ToString());
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
