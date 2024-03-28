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
using System.Windows.Shapes;
using WPFPersonalTracking.DB;
using WPFPersonalTracking.DetailModels;
using WPFPersonalTracking.Statics;

namespace WPFPersonalTracking.Pages
{
    /// <summary>
    /// Interaction logic for PermissionPage.xaml
    /// </summary>
    public partial class PermissionPage : Window
    {
        public PermissionDetailModel Model;
        PersonaltrackingContext _db = new();

        public PermissionPage()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserNo.Text = UserStatic.UserNo.ToString();

            if (!IsModelExist()) return;

            UpdatePage();
        }

        private void dpStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateDayAmount();
        }

        private void dpEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateDayAmount();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!AreFieldsValid()) return;

            if (IsModelExist())
                UpdatePermission();
            else
            {
                AddPermission();
                ClearFields();
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region SIDE METHODS
        private void CalculateDayAmount()
        {
            if (dpStart.SelectedDate != null && dpEnd.SelectedDate != null)
            {
                var tsPermissionDay = (TimeSpan)(dpEnd.SelectedDate - dpStart.SelectedDate);
                txtDayAmount.Text = tsPermissionDay.TotalDays.ToString();
            }
        }

        private void UpdatePage()
        {
            txtUserNo.Text = Model.UserNo.ToString();
            txtDayAmount.Text = Model.DayAmount.ToString();
            txtExplanation.Text = Model.Explanation;
            dpStart.SelectedDate = Model.StartDate;
            dpEnd.SelectedDate = Model.EndDate;
        }

        private bool AreFieldsValid()
        {
            if (txtDayAmount.Text.Trim() == "")
            {
                MessageBox.Show("Please select start and end date!");
                return false;
            }
            else if (Convert.ToInt32(txtDayAmount.Text) <= 0)
            {
                MessageBox.Show("Permission day must be bigger than zero!");
                return false;
            }
            else if (txtExplanation.Text.Trim() == "")
            {
                MessageBox.Show("Please write your permission reason!");
                return false;
            }
            return true;
        }

        private void AddPermission()
        {
            var permission = new Permission();
            permission.EmployeeId = UserStatic.EmployeeId;
            permission.UserNo = UserStatic.UserNo;
            permission.PermissionState = Definitions.PermissionStates.OnEmployee;
            permission.StartDate = dpStart.SelectedDate;
            permission.EndDate = dpEnd.SelectedDate;
            permission.PermissionAmount = Convert.ToInt32(txtDayAmount.Text);
            permission.Explanation = txtExplanation.Text;
            _db.Permissions.Add(permission);
            _db.SaveChanges();
            MessageBox.Show("Permission was added!");
        }

        private void UpdatePermission()
        {
            var permission = _db.Permissions.Find(Model.Id);
            permission.StartDate = dpStart.SelectedDate;
            permission.EndDate = dpEnd.SelectedDate;
            permission.PermissionAmount = Convert.ToInt32(txtDayAmount.Text);
            permission.Explanation = txtExplanation.Text;
            _db.SaveChanges();
            MessageBox.Show("Permission was added!");
        }

        private void ClearFields()
        {
            dpStart.SelectedDate = DateTime.Today;
            dpEnd.SelectedDate = DateTime.Today;
            txtExplanation.Clear();
            txtDayAmount.Clear();
        }

        private bool IsModelExist() => Model != null && Model.Id != 0;

        #endregion
    }
}
