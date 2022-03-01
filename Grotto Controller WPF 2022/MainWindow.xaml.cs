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
using Ookii.Dialogs;
using Renci.SshNet;
using System.Windows.Threading;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace Grotto_Controller_WPF_2022
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> lstElfNames = new List<string>();
        List<string> lstGrottoNames = new List<string>();
        List<string> lstLocalConns = new List<string>();
        List<string> lstAll = new List<string>();
        List<string> lstCount = new List<string>();
        List<string> lstFamilyNames = new List<string>();
        List<string> lstRef = new List<string>();
        DispatcherTimer monitorTimer = new DispatcherTimer();
        DispatcherTimer checkInTimer = new DispatcherTimer();
        DispatcherTimer departureTimer = new DispatcherTimer();
        DispatcherTimer waitTimer = new DispatcherTimer();
        int i, j, intTime1, intTime2;
        DateTime w1, w2, w3, w4, w5, w6, w7, w8;
        int intd,intattempts, intO;
        string strread;

        public MainWindow()
        {
            InitializeComponent();
            loadSettings();
            monitorTimer.Stop();

            loadTimers();
        }

        private void loadTimers()
        {
            txtCheckInTimer.Text = Properties.Settings.Default.cCheckinTimer.ToString();
            txtMontiorTimer.Text = Properties.Settings.Default.cMonitorTimer.ToString();

            monitorTimer.Interval = TimeSpan.FromSeconds(Properties.Settings.Default.cMonitorTimer);
            monitorTimer.Tick += monitorTick;

            checkInTimer.Interval = TimeSpan.FromSeconds(Properties.Settings.Default.cCheckinTimer);
            checkInTimer.Tick += checkInTick;

            departureTimer.Interval = TimeSpan.FromSeconds(.5);
            departureTimer.Tick += departureTick;

            waitTimer.Interval = TimeSpan.FromSeconds(1);
            waitTimer.Tick += waitTick;
            waitTimer.Start();
        }

        void waitTick (object sender, EventArgs e)
        {
            //set variables
            DateTime time1 = DateTime.Now;
            double seconds = 0;
            TimeSpan time;
            string str;

            //calculate time for first wait
            seconds = (time1.Subtract(w1).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG1.Content = str;

            //calculate time for second wait
            seconds = (time1.Subtract(w2).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG2.Content = str;

            seconds = (time1.Subtract(w3).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG3.Content = str;

            seconds = (time1.Subtract(w4).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG4.Content = str;

            seconds = (time1.Subtract(w5).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG5.Content = str;

            seconds = (time1.Subtract(w6).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG6.Content = str;

            seconds = (time1.Subtract(w7).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG7.Content = str;

            seconds = (time1.Subtract(w8).TotalSeconds);
            time = TimeSpan.FromSeconds(seconds);
            str = time.ToString(@"hh\:mm\:ss");
            lblCWaitG8.Content = str;
        }
        
        void monitorTick(object sender, EventArgs e)
        {

            //read downloaded questions file
            var fileStream = new FileStream(, FileMode.Open, System.IO.FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            fileStream.Close();
        }

        void checkInTick(object sender, EventArgs e)
        {

        }
        private void loadSettings()
        {
            //load elf names
            loadElfNames();
            //load local connections
            lstLocalConns.Clear();
            lstLocalConns = Properties.Settings.Default.cLocalCons.ToString().Split(',').ToList();
            txtLocalCon1.Text = lstLocalConns[0].ToString();
            txtLocalCon2.Text = lstLocalConns[1].ToString();
            txtLocalCon3.Text = lstLocalConns[2].ToString();
            txtLocalCon4.Text = lstLocalConns[3].ToString();
            txtLocalCon5.Text = lstLocalConns[4].ToString();
            txtLocalCon6.Text = lstLocalConns[5].ToString();
            txtLocalCon7.Text = lstLocalConns[6].ToString();
            txtLocalCon8.Text = lstLocalConns[7].ToString();
            txtBlueCorridor.Text = Properties.Settings.Default.cElfUpdateBlue.ToString();
            txtRedCorridor.Text = Properties.Settings.Default.cElfUpdateRed.ToString();
            txtLocalLocation.Text = Properties.Settings.Default.cLocalSave.ToString();
            txtMonitorLoc.Text = Properties.Settings.Default.cMonitorLoc.ToString();
            txtDepartureLoc.Text = Properties.Settings.Default.cDepartureLoc.ToString();
            txtBackupLoc.Text = Properties.Settings.Default.cBackupLocation.ToString();
            //load ftp details
            loadFTPDetails();
            //are you using the check in machine
            if (Properties.Settings.Default.cUserCheckIn==true)
            {
                rbUseCheckIn.IsChecked = true;
                rbDontUseCheckin.IsChecked = false;
            }
            else
            {
                rbUseCheckIn.IsChecked=false;
                rbDontUseCheckin.IsChecked = true;
            }
            //load grotto names
            loadGrottoNames();
            //load dates and times
            loadDatesandTimes();
        }

        private void loadDatesandTimes()
        {
            //add days to combo box
            i = 0;
            cboDay.Items.Clear();
            while (i<31)
            {
                cboDay.Items.Add(i.ToString());
                i = i + 1;
            }
            //add months to combobox
            cboMonth.Items.Clear();
            cboMonth.Items.Add("January");
            cboMonth.Items.Add("February");
            cboMonth.Items.Add("March");
            cboMonth.Items.Add("April");
            cboMonth.Items.Add("May");
            cboMonth.Items.Add("June");
            cboMonth.Items.Add("July");
            cboMonth.Items.Add("August");
            cboMonth.Items.Add("September");
            cboMonth.Items.Add("October");
            cboMonth.Items.Add("November");
            cboMonth.Items.Add("December");
            //add years to combobox
            cboYear.Items.Clear();
            i = 0;
            j=DateTime.Now.Year-1;
            while (i<5)
            {
                cboYear.Items.Add(j).ToString();
                j = j + 1;
                i = i + 1;
            }
            //add times to combobox
            cboTime.Items.Clear();
            string strTime = "";
            i = 8;
            j = 0;
            while (i<22)
            {
                j = 0;
                while (j<60)
                {
                    if (j==0)
                    {
                        strTime = i.ToString() + ":00";
                    }
                    else
                    {
                        strTime = i.ToString() + ":" +j.ToString();
                    }
                    cboTime.Items.Add(strTime);
                    j = j + 10;
                }
                i = i + 1;
            }

        }

        private void loadGrottoNames()
        {
            lstGrottoNames.Clear();
            lstGrottoNames=Properties.Settings.Default.cGrottoNames.ToString().Split(',').ToList();
            txtGrottoName1.Text = lstGrottoNames[0].ToString();
            txtGrottoName2.Text=lstGrottoNames[1].ToString();   
            txtGrottoName3.Text = lstGrottoNames[2].ToString();
            txtGrottoName4.Text = lstGrottoNames[3].ToString();
            txtGrottoName5.Text = lstGrottoNames[4].ToString();
            txtGrottoName6.Text = lstGrottoNames[5].ToString();
            txtGrottoName7.Text = lstGrottoNames[6].ToString();
            txtGrottoName8.Text = lstGrottoNames[7].ToString();
            lblG1GName.Content = lstGrottoNames[0].ToString();
            lblG2GName.Content = lstGrottoNames[1].ToString();
            lblG3GName.Content = lstGrottoNames[2].ToString();
            lblG4GName.Content = lstGrottoNames[3].ToString();
            lblG5GName.Content = lstGrottoNames[4].ToString();
            lblG6GName.Content = lstGrottoNames[5].ToString();
            lblG7GName.Content = lstGrottoNames[6].ToString();
            lblG8GName.Content = lstGrottoNames[7].ToString();
        }

        private void loadFTPDetails()
        {
            txtFTPUsername.Text = Properties.Settings.Default.cFTPUsername.ToString();
            txtFTPPort.Text = Properties.Settings.Default.cFTPPort.ToString();
            txtFTPPassword.Text = Properties.Settings.Default.cFTPPassword.ToString();
            txtFTPHost.Text = Properties.Settings.Default.cFTPHost.ToString();
            txtFTPAdult.Text = Properties.Settings.Default.cFTPAdult.ToString();
            txtFTPChild.Text = Properties.Settings.Default.cFTPChild.ToString();
            txtFTPFamily.Text = Properties.Settings.Default.cFTPFamily.ToString();
            txtFTPFolder.Text = Properties.Settings.Default.cFTPFolder.ToString();
            txtFTPSaved.Text = Properties.Settings.Default.cFTPSaved.ToString();
        }
        private void loadElfNames()
        {
            lstElfNames.Clear();
            lstElfNames = Properties.Settings.Default.cElfNames.ToString().Split(',').ToList();
            txtElfName1.Text = lstElfNames[0];
            txtElfName2.Text = lstElfNames[1];
            txtElfName3.Text = lstElfNames[2];
            txtElfName4.Text = lstElfNames[3];
            txtElfName5.Text = lstElfNames[4];
            txtElfName6.Text = lstElfNames[5];
            txtElfName7.Text = lstElfNames[6];
            txtElfName8.Text = lstElfNames[7];
            lblG1EName.Content = lstElfNames[0].ToString();
            lblG2EName.Content = lstElfNames[1].ToString();
            lblG3EName.Content = lstElfNames[2].ToString();
            lblG4EName.Content = lstElfNames[3].ToString();
            lblG5EName.Content = lstElfNames[4].ToString();
            lblG6EName.Content = lstElfNames[5].ToString();
            lblG7EName.Content = lstElfNames[6].ToString();
            lblG8EName.Content = lstElfNames[7].ToString();
        }

        private void cmdCheckPassword_Click(object sender, RoutedEventArgs e)
        {
            checkPin();
        }

        private void checkPin()
        {
            if (txtPassword.Password == Properties.Settings.Default.cPin.ToString())
            {
                tbSettings.Visibility = Visibility.Visible;
            }
            txtPassword.Password = "";
            txtPassword.Focus();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmdBrowseLocal1(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the First Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon1.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdBrowseLocal2(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Second Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon2.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdBrowseLocal3(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Third Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon3.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdBrowseLocal4(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Fourth Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon4.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdBrowseLocal5(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Fifth Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon5.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdBrowseLocal6(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Sixth Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon6.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdBrowseLocal7(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Seventh Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon7.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdBrowseLocal8(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Eighth Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalCon8.Text = newSelectedFolderPath;
                updateLocalLocations();
            }
        }

        private void cmdUpdateElfNames_Click(object sender, RoutedEventArgs e)
        {
            if (txtElfName1.Text == "" || txtElfName2.Text == "" || txtElfName3.Text == "" || txtElfName4.Text == "" || txtElfName5.Text == "" || txtElfName6.Text == "" || txtElfName7.Text == "" || txtElfName8.Text == "")
            {
                MessageBox.Show("Please make sure you fill in all the elf names, the names have not been updated", "Elf Name Error", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else
            {
                string strTemp;
                strTemp = txtElfName1.Text.Replace(",", "") + "," + txtElfName2.Text.Replace(",", "") + "," + txtElfName3.Text.Replace(",", "") + "," + txtElfName4.Text.Replace(",", "");
                strTemp = strTemp + "," + txtElfName5.Text.Replace(",", "") + "," + txtElfName6.Text.Replace(",", "") + "," + txtElfName7.Text.Replace(",", "") + "," + txtElfName8.Text.Replace(",", "");
                Properties.Settings.Default.cElfNames = strTemp;
                Properties.Settings.Default.Save();
            }
        }

        private void cmdChangePin_Click(object sender, RoutedEventArgs e)
        {
            if (txtOldPin.Text == Properties.Settings.Default.cPin)
            {
                if (txtNewPin1.Text == txtNewPin2.Text)
                {
                    Properties.Settings.Default.cPin = txtNewPin1.Text;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("New pins don't match, please try again","Pin error",MessageBoxButton.OK,MessageBoxImage.Error);
                    txtNewPin2.Focus();
                }
            }
            else
            {
                MessageBox.Show("Old pin is not correct, please try again","Pin Error",MessageBoxButton.OK,MessageBoxImage.Hand);
                txtOldPin.Focus();
            }
        }

        private void cmdCancelFTP_Click(object sender, RoutedEventArgs e)
        {
            loadFTPDetails();
        }

        private void cmdUpdateFTP(object sender, RoutedEventArgs e)
        {
            //check ftp details by connecting to remote server
            //access server
            string Host = txtFTPHost.Text;
            int Port = Int32.Parse(txtFTPPort.Text);
            string Username = txtFTPUsername.Text;
            string Password = txtFTPPassword.Text;

            bool bSuccess = false;

            try
            {
                using (var sftp = new SftpClient(Host, Port, Username, Password))
                {
                    sftp.Connect(); //connect to server
                    sftp.Disconnect();
                    bSuccess = true;
                }
            }
            catch
            {
                MessageBox.Show("Couldn't connect to server with provided FTP details, please reenter and try again. These have not been saved", "FTP connection failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                bSuccess = false;
            }

            if (bSuccess == true)
            {
                Properties.Settings.Default.cFTPHost = txtFTPHost.Text;
                Properties.Settings.Default.cFTPPassword = txtFTPPassword.Text;
                Properties.Settings.Default.cFTPPort = Int32.Parse(txtFTPPort.Text);
                Properties.Settings.Default.cFTPUsername = txtFTPUsername.Text;
                Properties.Settings.Default.cFTPChild = txtFTPChild.Text;
                Properties.Settings.Default.cFTPAdult = txtFTPAdult.Text;
                Properties.Settings.Default.cFTPFamily = txtFTPFamily.Text;
                Properties.Settings.Default.cFTPFolder = txtFTPFolder.Text;
                Properties.Settings.Default.cFTPSaved = txtFTPSaved.Text;
                Properties.Settings.Default.Save();
                MessageBox.Show("FTP details test successful. FTP details stored", "FTP Details", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void cmdUpdateTimers_Click(object sender, RoutedEventArgs e)
        {
            bool cSuccess;
            bool mSuccess; 
            string strMessage="";
            string strTitle="";
            try
            {
                Properties.Settings.Default.cCheckinTimer = Int32.Parse(txtCheckInTimer.Text);
                Properties.Settings.Default.Save();
                cSuccess = true;
            }
            catch
            {
                cSuccess = false;
            }

            try
            {
                Properties.Settings.Default.cMonitorTimer = Int32.Parse(txtMontiorTimer.Text);
                Properties.Settings.Default.Save();
                mSuccess = true;
            }
            catch
            {
                mSuccess = false;
            }

            if (cSuccess==false)
            {
                strMessage = "The interval for the Check In Timer is not a valid number, please reenter and try again";
                strTitle = "Check In Timer Error";
            }
            if (mSuccess == false)
            {
                strMessage = "The interval for the Monitor Timer is not a valid number, please reenter and try again";
                strTitle = "Monitor Timer Error";
            }
            if (cSuccess == false && mSuccess==false)
            {
                strMessage = "The intervals for the Check In Timer and the Monitor Timer are not valid numbers, please reenter and try again";
                strTitle = "Timer Errors";
            }
            if (cSuccess==false || mSuccess == false)
            {
                MessageBox.Show(strMessage,strTitle,MessageBoxButton.OK,MessageBoxImage.Error);
            }
            loadTimers();

        }

        private void rbUseCheckIn_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.cUserCheckIn = true;
            Properties.Settings.Default.Save();
        }

        private void rbDontUseCheckin_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.cUserCheckIn = false;
            Properties.Settings.Default.Save();
        }

        private void cmdRedCorridor_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Red Corridor Location";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtRedCorridor.Text = newSelectedFolderPath;
                Properties.Settings.Default.cElfUpdateRed = newSelectedFolderPath;
                Properties.Settings.Default.Save();
            }
        }

        private void cmdBlueCorridor_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Blue Corridor Location";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtBlueCorridor.Text = newSelectedFolderPath;
                Properties.Settings.Default.cElfUpdateBlue = newSelectedFolderPath;
                Properties.Settings.Default.Save();
            }
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tbSettings.Visibility = Visibility.Hidden;
        }

        private void TabItem_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            tbSettings.Visibility = Visibility.Hidden;
        }

        private void cmdLocalBrowse_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the First Local Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtLocalLocation.Text = newSelectedFolderPath;
               Properties.Settings.Default.cLocalSave = newSelectedFolderPath;
                Properties.Settings.Default.Save();
            }
        }

        private void tbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbSettings.Visibility= Visibility.Hidden;
        }

        private void cmdUpdateGrottoNames_Click(object sender, RoutedEventArgs e)
        {
            if (txtGrottoName1.Text == "" || txtGrottoName2.Text == "" || txtGrottoName3.Text =="" || txtGrottoName4.Text == "" || txtGrottoName5.Text == "" || txtGrottoName6.Text == "" || txtGrottoName7.Text == "" || txtGrottoName8.Text == "")
            {
                MessageBox.Show("Please make sure you fill in all the grotto names","Missing Names",MessageBoxButton.OK,MessageBoxImage.Hand);
            }
            else
            {
                string strTemp = "";
                strTemp = txtGrottoName1.Text + "," + txtGrottoName2.Text + "," + txtGrottoName3.Text + "," + txtGrottoName4.Text + "," + txtGrottoName5.Text + "," + txtGrottoName6.Text + "," + txtGrottoName7.Text + "," + txtGrottoName8.Text;
                Properties.Settings.Default.cGrottoNames = strTemp;
                Properties.Settings.Default.Save();
                loadGrottoNames();
            }
        }

        private void cmdMonitorBrowse_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Monitor Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtMonitorLoc.Text = newSelectedFolderPath;
                Properties.Settings.Default.cMonitorLoc = newSelectedFolderPath;
                Properties.Settings.Default.Save();
            }
        }

        private void chkG1Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG1Open.IsChecked == true)
            {
                cmdG1Call.IsEnabled= true;
                cmdG1OR.IsEnabled= true;
                cmdG1.IsEnabled= true;
                chkG1Open.Content = "Open";
            }
            else
            {
                cmdG1.IsEnabled = false;
                cmdG1Call.IsEnabled = false;
                cmdG1OR.IsEnabled= false;
                chkG1Open.Content = "Closed";
            }
        }

        private void chkG2Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG2Open.IsChecked == true)
            {
                cmdG2Call.IsEnabled = true;
                cmdG2OR.IsEnabled = true;
                cmdG2.IsEnabled = true;
                chkG2Open.Content = "Open";
            }
            else
            {
                cmdG2.IsEnabled = false;
                cmdG2Call.IsEnabled = false;
                cmdG2OR.IsEnabled = false;
                chkG2Open.Content = "Closed";
            }
        }

        private void chkG3Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG3Open.IsChecked == true)
            {
                cmdG3Call.IsEnabled = true;
                cmdG3OR.IsEnabled = true;
                cmdG3.IsEnabled = true;
                chkG3Open.Content = "Open";
            }
            else
            {
                cmdG3.IsEnabled = false;
                cmdG3Call.IsEnabled = false;
                cmdG3OR.IsEnabled = false;
                chkG3Open.Content = "Closed";
            }
        }

        private void chkG4Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG4Open.IsChecked == true)
            {
                cmdG4Call.IsEnabled = true;
                cmdG4OR.IsEnabled = true;
                cmdG4.IsEnabled = true;
                chkG4Open.Content = "Open";
            }
            else
            {
                cmdG4.IsEnabled = false;
                cmdG4Call.IsEnabled = false;
                cmdG4OR.IsEnabled = false;
                chkG4Open.Content = "Closed";
            }
        }

        private void chkG5Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG5Open.IsChecked == true)
            {
                cmdG5Call.IsEnabled = true;
                cmdG5OR.IsEnabled = true;
                cmdG5.IsEnabled = true;
                chkG5Open.Content = "Open";
            }
            else
            {
                cmdG5.IsEnabled = false;
                cmdG5Call.IsEnabled = false;
                cmdG5OR.IsEnabled = false;
                chkG5Open.Content = "Closed";
            }
        }

        private void chkG6Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG6Open.IsChecked == true)
            {
                cmdG6Call.IsEnabled = true;
                cmdG6OR.IsEnabled = true;
                cmdG6.IsEnabled = true;
                chkG6Open.Content = "Open";
            }
            else
            {
                cmdG6.IsEnabled = false;
                cmdG6Call.IsEnabled = false;
                cmdG6OR.IsEnabled = false;
                chkG6Open.Content = "Closed";
            }
        }

        private void chkG7Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG7Open.IsChecked == true)
            {
                cmdG7Call.IsEnabled = true;
                cmdG7OR.IsEnabled = true;
                cmdG7.IsEnabled = true;
                chkG7Open.Content = "Open";
            }
            else
            {
                cmdG7.IsEnabled = false;
                cmdG7Call.IsEnabled = false;
                cmdG7OR.IsEnabled = false;
                chkG7Open.Content = "Closed";
            }
        }

        private void chkG8Open_Checked(object sender, RoutedEventArgs e)
        {
            if (chkG8Open.IsChecked == true)
            {
                cmdG8Call.IsEnabled = true;
                cmdG8OR.IsEnabled = true;
                cmdG8.IsEnabled = true;
                chkG8Open.Content = "Open";
            }
            else
            {
                cmdG8.IsEnabled = false;
                cmdG8Call.IsEnabled = false;
                cmdG8OR.IsEnabled = false;
                chkG8Open.Content = "Closed";
            }
        }

        private void cmdDeoartureBrowse_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Monitor Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtDepartureLoc.Text = newSelectedFolderPath;
                Properties.Settings.Default.cDepartureLoc = newSelectedFolderPath;
                Properties.Settings.Default.Save();
            }
        }

        private void cmdG1Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 1;
            intattempts = 0;
        }

        private void callFamily()
        {
            bool dSuccess = false;
            departureTimer.Stop();
            //create random number
            Random rd = new Random();
            int rand_num = rd.Next(0, 99999);
            Random rd1 = new Random();
            int rand_num1 = rd1.Next(0, 99999);
            string strRand = rd.ToString()+rd1.ToString();
            //create message for sending to departure board
            string strMessage = txtFamily.Text + " please see " + lstElfNames[intd - 1].ToString();
            try
            {
                // Write file using StreamWriter  
                strread = Properties.Settings.Default.cDepartureLoc + "\\Departure.txt";
                using (StreamWriter writer = new StreamWriter(strread))
                {
                    writer.WriteLine(strRand);
                    writer.WriteLine(txtRef.Text);
                    writer.WriteLine(strMessage);
                }
                dSuccess = true;
            }
            catch //if for some reason the departure board can't be written too then we start a timer and try again in .5 seconds
            {
                if (intattempts > 5)
                {
                    MessageBox.Show("Can't write to the departure board, please check the connection and try again", "Departure Board Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    dSuccess = false;
                    departureTimer.Start();
                    intattempts = intattempts + 1;
                }
            }
            //if the file is written successfully then we update the Controllers screen
            if (dSuccess==true)
            {
                if (intd == 1) { lblG1Called.Content = txtFamily.Text; w1 = DateTime.Now; lblCWaitG1.Visibility = Visibility.Visible; }
                if (intd == 2) { lblG2Called.Content = txtFamily.Text; w2 = DateTime.Now;  lblCWaitG2.Visibility = Visibility.Visible; }
                if (intd == 3) { lblG3Called.Content = txtFamily.Text; w3 = DateTime.Now; lblCWaitG3.Visibility = Visibility.Visible; }
                if (intd == 4) { lblG4Called.Content = txtFamily.Text; w4 = DateTime.Now; lblCWaitG4.Visibility = Visibility.Visible; }
                if (intd == 5) { lblG5Called.Content = txtFamily.Text; w5 = DateTime.Now; lblCWaitG5.Visibility = Visibility.Visible; }
                if (intd == 6) { lblG6Called.Content = txtFamily.Text; w6 = DateTime.Now;  lblCWaitG6.Visibility = Visibility.Visible; }
                if (intd == 7) { lblG7Called.Content = txtFamily.Text; w7 = DateTime.Now; lblCWaitG7.Visibility = Visibility.Visible; }
                if (intd == 8) { lblG8Called.Content = txtFamily.Text; w8 = DateTime.Now; lblCWaitG8.Visibility = Visibility.Visible; }
                txtFamily.Text = "";
                txtRef.Text = "";
            }
        }

        private void cmdG1_Click(object sender, RoutedEventArgs e)
        {
            sendToGrotto();
        }

        private void sendToGrotto()
        {

        }

        private void cmdG1OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 1;
            overrideGrotto();
        }

        private void cmdG2OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 2;
            overrideGrotto();
        }

        private void cmdG3OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 3;
            overrideGrotto();
        }

        private void cmdG4OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 4;
            overrideGrotto();
        }

        private void cmdG5OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 5;
            overrideGrotto();
        }

        private void cmdG6OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 6;
            overrideGrotto();
        }

        private void cmdG7OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 7;
            overrideGrotto();
        }

        private void cmdG8OR_Click(object sender, RoutedEventArgs e)
        {
            intO = 8;
            overrideGrotto();
        }

        private void cmdSubmitTime_Click(object sender, RoutedEventArgs e)
        {
            if (cboDay.SelectedIndex== -1 || cboMonth.SelectedIndex== -1 ||cboTime.SelectedIndex== -1||cboYear.SelectedIndex== -1)
            {
                MessageBox.Show("Please ensure you have selected a date and time before submitting","missing date and time",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
            else
            {
                retrieveBookings();
            }
        }

        private void cmdBackupBrowse_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the Backup Location folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                string newSelectedFolderPath = folderDialog.SelectedPath;
                txtBackupLoc.Text = newSelectedFolderPath;
                Properties.Settings.Default.cBackupLocation = newSelectedFolderPath;
                Properties.Settings.Default.Save();
            }
        }

        private void retrieveBookings()
        {
            bool dsuccess = false;
            //attempt to connect to fusemetrix and pull bookings from today
            //this is downloadBooking22.php found on sferver
            {


                try
                {
                    //retrieve list of bookings for this date and time
                    //send date and time to php code
                    string sTime = cboTime.Text;
                    string sDate = cboDay.Text + "/" + cboMonth.Text + "/" + cboYear.Text;
                    NameValueCollection nv = new NameValueCollection();
                    nv.Add("QTime", sTime);
                    nv.Add("QDate", sDate);

                    var url = "https://4k-photos.co.uk/downloadBooking22.php";

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Method = "POST";

                    httpRequest.ContentType = "application/x-www-form-urlencoded";

                    var data = "QDate=" + sDate + "&QTime=" + sTime;

                    //this sends the request for date and time to a php script on the server which in turn 
                    //creates a new file with the username date time and bookings1.txt
                    //eg 23/12/2021 10:00 would be 231220211000Bookings1.txt
                    using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                    }

                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string result = streamReader.ReadToEnd();
                    }

                    //open the txt file and read the contents to a list
                    var list = new List<string>();

                    //download new file
                    string Host = Properties.Settings.Default.cFTPHost.ToString();
                    int Port = Int32.Parse(Properties.Settings.Default.cFTPPort.ToString());
                    string Username = Properties.Settings.Default.cFTPUsername.ToString();
                    string Password = Properties.Settings.Default.cFTPPassword.ToString();


                    string strTime = sTime.Replace(":", "");
                    string strName = cboDay.Text + cboMonth.Text + cboYear.Text + strTime.ToString() + "Bookings1.txt";
                    string strRemoteFolder = strName;
                    string strLocalFolder = Properties.Settings.Default.cLocalSave.ToString() + "\\" + strName.ToString();
                    using (var sftp = new SftpClient(Host, Port, Username, Password))
                    {
                        sftp.Connect();

                        //download new booking file
                        using (var file = File.OpenWrite(strLocalFolder))
                        {
                            sftp.DownloadFile(strRemoteFolder, file);//download file
                        }

                        sftp.Disconnect();
                    }
                    readThisTimesBookings();

                    dsuccess = true;
                }
                catch
                {
                    MessageBox.Show("Can't read FTP site, attempting to read local save", "Can't connect to FTP", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    dsuccess = false;
                }
            }

            //if can't access the internet/connect to fusemetrix
            //read the file created today of all bookings
            if (dsuccess == false)
            {

                try
                {
                    //read saved file from today with this mornings data
                    var list = new List<string>();
                    string strLocalFolder = Properties.Settings.Default.cBackupLocation.ToString() + "//" + cboDay.Text + cboMonth.Text + cboYear.Text + "BookingsBart.txt";
                    var fileStream = new FileStream(strLocalFolder, FileMode.Open, System.IO.FileAccess.Read);
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            list.Add(line);
                        }
                    }
                    fileStream.Close();

                    //read through file and find the time of this booking
                    string strTime1 = cboTime.Text;
                    string strTime2 = cboTime.Items[cboTime.SelectedIndex + 1].ToString();
                    //first time slot - actual booking
                    i = 0;
                    while (i < list.Count)
                    {
                        if (list[i].ToString() == strTime1.ToString())
                        {
                            intTime1 = i;
                            i = list.Count;
                        }
                        i = i + 1;
                    }
                    //second time slot - next booking
                    i = 0;
                    while (i < list.Count)
                    {
                        if (list[i].ToString() == strTime2.ToString())
                        {
                            intTime2 = i;
                            i = list.Count;
                        }
                        i = i + 1;
                    }

                    //create a new text file using the data between the two above integers

                    string path = Properties.Settings.Default.cLocalSave.ToString() + "\\" + cboDay.Text + cboMonth.Text + cboYear.Text + cboTime.Text.Replace(":", "") + "Bookings1.txt";
                    if (!File.Exists(path))
                    {
                        // Create a file to write to.
                        using (StreamWriter sw = File.CreateText(path))
                        {
                            i = intTime1;
                            while (i < intTime2)
                            {
                                sw.WriteLine(list[i].ToString());
                                i = i + 1;
                            }
                        }
                    }

                    readThisTimesBookings();

                    dsuccess = true;
                }
                catch
                {
                    MessageBox.Show("Can't read or find local folder, attempting to read from nearby machines", "No local save", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dsuccess = false;
                }
            }

            if (dsuccess == false)
            {
                //if it can't read the ftp site or local folders, check nearby machines
                List<string> lstNearby = new List<string>();
                lstNearby = Properties.Settings.Default.cLocalCons.Split(',').ToList();
                i = 0;
                var list1 = new List<string>();
                while (i < lstNearby.Count)
                {
                    string strNearby = lstNearby[i].ToString() + "//" + cboDay.Text + cboMonth.Text + cboYear.Text + "BookingsBart.txt";
                    try
                    {
                        var fileStream = new FileStream(strNearby, FileMode.Open, System.IO.FileAccess.Read);
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                list1.Add(line);
                            }
                        }
                        fileStream.Close();
                        i = lstNearby.Count;
                        dsuccess = true;

                        //read through file and find the time of this booking
                        string strTime1 = cboTime.Text;
                        string strTime2 = cboTime.Items[cboTime.SelectedIndex + 1].ToString();
                        //first time slot - actual booking
                        i = 0;
                        while (i < list1.Count)
                        {
                            if (list1[i].ToString() == strTime1.ToString())
                            {
                                intTime1 = i;
                                i = list1.Count;
                            }
                            i = i + 1;
                        }
                        //second time slot - next booking
                        i = 0;
                        while (i < list1.Count)
                        {
                            if (list1[i].ToString() == strTime2.ToString())
                            {
                                intTime2 = i;
                                i = list1.Count;
                            }
                            i = i + 1;
                        }

                        //create a new text file using the data between the two above integers

                        string path = Properties.Settings.Default.cLocalSave.ToString() + "\\" + cboDay.Text + cboMonth.Text + cboYear.Text + cboTime.Text.Replace(":", "") + "Bookings1.txt";
                        if (!File.Exists(path))
                        {
                            // Create a file to write to.
                            using (StreamWriter sw = File.CreateText(path))
                            {
                                i = intTime1;
                                while (i < intTime2)
                                {
                                    sw.WriteLine(list1[i].ToString());
                                    i = i + 1;
                                }
                            }
                        }

                        readThisTimesBookings();
                    }
                    catch
                    {
                        i = i + 1;
                        dsuccess = false;
                    }
                }
            }

            if (dsuccess == false)
            {
                MessageBox.Show("Can't download bookings for this session, this is probably down to missing data and no internet connection", "Missing session information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void readThisTimesBookings()
        {
            string sTime = cboTime.Text;
            string strTime = sTime.Replace(":", "");
            string strName = cboDay.Text + cboMonth.Text + cboYear.Text + strTime.ToString() + "Bookings1.txt";
            string strLocalFolder = Properties.Settings.Default.cLocalSave.ToString() + "\\" + strName.ToString();

            lstAll.Clear();
            //read new file of decoded customer information
            var fileStream = new FileStream(strLocalFolder, FileMode.Open, System.IO.FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lstAll.Add(line);
                }
            }
            fileStream.Close();

            //count the amount of families in the session
            lstCount.Clear();
            lstFamilyNames.Clear();
            i = 0;
            while (i < lstAll.Count)
            {
                List<string> lstItems = new List<string>();
                lstItems = lstAll[i].Split(',').ToList();
                lstCount.Add(lstItems[0].ToString());
                if (lstItems[1].ToString() == "Address")
                {
                    List<string> lstFName = new List<string>();
                    lstFName = lstItems[2].Split(' ').ToList();
                    try
                    {
                        lstFamilyNames.Add(lstFName[lstFName.Count - 1].ToString());
                    }
                    catch
                    {
                        lstFamilyNames.Add(lstItems[2].ToString());
                    }
                }
                i = i + 1;
            }

            //retrieve the booking references
            lstRef.Clear();
            lstRef = lstCount.Distinct().ToList();
        }

        private void overrideGrotto()
        {
            bool dSuccess = false;
            
            //create random number
            Random rd = new Random();
            int rand_num = rd.Next(0, 99999);
            Random rd1 = new Random();
            int rand_num1 = rd1.Next(0, 99999);
            string strRand = rd.ToString() + rd1.ToString();

            if (intO == 1) { strread = lstLocalConns[0].ToString() + "//Override.txt"; cmdG1OR.IsEnabled = false; }
            if (intO == 2) { strread = lstLocalConns[1].ToString() + "//Override.txt"; cmdG2OR.IsEnabled = false; }
            if (intO == 3) { strread = lstLocalConns[2].ToString() + "//Override.txt"; cmdG3OR.IsEnabled = false; }
            if (intO == 4) { strread = lstLocalConns[3].ToString() + "//Override.txt"; cmdG4OR.IsEnabled = false; }
            if (intO == 5) { strread = lstLocalConns[4].ToString() + "//Override.txt"; cmdG5OR.IsEnabled = false; }
            if (intO == 6) { strread = lstLocalConns[5].ToString() + "//Override.txt"; cmdG6OR.IsEnabled = false; }
            if (intO == 7) { strread = lstLocalConns[6].ToString() + "//Override.txt"; cmdG7OR.IsEnabled = false; }
            if (intO == 8) { strread = lstLocalConns[7].ToString() + "//Override.txt"; cmdG8OR.IsEnabled = false; }
            
            //create message for sending to correct grotto
            try
            {
                // Write file using StreamWriter  
                using (StreamWriter writer = new StreamWriter(strread))
                {
                    writer.WriteLine(strRand);
                }
                dSuccess = true;
            }
            catch
            {
                dSuccess = false;
            }
        }

        void departureTick(object sender, EventArgs e)
        {
            callFamily();
        }

        private void cmdG2Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 2;
            intattempts = 0;
        }

        private void cmdG3Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 3;
            intattempts = 0;
        }

        private void cmdG4Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 4;
            intattempts = 0;
        }

        private void cmdG5Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 5;
            intattempts = 0;
        }

        private void cmdG6Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 6;
            intattempts = 0;
        }

        private void cmdG7Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 7;
        }

        private void cmdG8Call_Click(object sender, RoutedEventArgs e)
        {
            callFamily();
            intd = 8;
            intattempts = 0;
        }

        private void updateLocalLocations()
        {
            string strTemp;
            strTemp = txtLocalCon1.Text.Replace(",", "") + "," + txtLocalCon2.Text.Replace(",", "") + "," + txtLocalCon3.Text.Replace(",", "") + "," + txtLocalCon4.Text.Replace(",", "");
            strTemp = strTemp + "," + txtLocalCon5.Text.Replace(",", "") + "," + txtLocalCon6.Text.Replace(",", "") + "," + txtLocalCon7.Text.Replace(",", "") + "," + txtLocalCon8.Text.Replace(",", "");
            Properties.Settings.Default.cLocalCons = strTemp;
            Properties.Settings.Default.Save();
        }
    }
}
