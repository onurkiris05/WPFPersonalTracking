using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFPersonalTracking.DB;

namespace WPFPersonalTracking
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        PersonaltrackingContext _db = new();

        public LoginPage()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void txtUserNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!AreFieldsValid()) return;

            var employee = _db.Employees.FirstOrDefault(x => x.UserNo == Convert.ToInt32(txtUserNo.Text) &&
            x.Password.Equals(txtPassword.Text));

            if (employee != null && employee.Id != 0)
            {
                Visibility = Visibility.Collapsed;
                var main = new MainWindow();
                UserStatic.EmployeeId = employee.Id;
                UserStatic.IsAdmin = employee.IsAdmin;
                UserStatic.Name = employee.Name;
                UserStatic.Surname = employee.Surname;
                UserStatic.UserNo = employee.UserNo;
                main.ShowDialog();

                if (!MainWindow.IsClosing)
                {
                    txtPassword.Clear();
                    txtUserNo.Clear();
                    Visibility = Visibility.Visible;
                }
            }
            else
                MessageBox.Show("Please make sure your password and user no are correct!");

        }
        #endregion

        #region SIDE METHODS
        private bool AreFieldsValid()
        {
            if (txtUserNo.Text.Trim() == "" || txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Please fill the necessary fields!");
                return false;
            }
            return true;
        }
        #endregion
    }
}
