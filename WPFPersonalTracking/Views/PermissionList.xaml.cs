using Microsoft.EntityFrameworkCore;
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
using WPFPersonalTracking.DB;
using WPFPersonalTracking.ViewModels;

namespace WPFPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for PermissionList.xaml
    /// </summary>
    public partial class PermissionList : UserControl
    {
        PersonaltrackingContext _db = new();
        List<PermissionDetailModel> _permissions = new();
        List<Position> _positions = new();
        List<Permissionstate> _states = new();
        PermissionDetailModel _model = new();

        public PermissionList()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _positions = _db.Positions.ToList();
            _states = _db.Permissionstates.ToList();

            FillDepartmentCombobox(_db.Departments.ToList());
            FillPositionCombobox(_positions);
            FillStateCombobox(_states);
            FillDataGrid();
        }

        private void gridPermission_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model = (PermissionDetailModel)gridPermission.SelectedItem;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new PermissionPage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var search = _permissions;

            if (txtUserNo.Text.Trim() != "")
                search = search.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            if (txtName.Text.Trim() != "")
                search = search.Where(x => x.Name.Contains(txtName.Text)).ToList();
            if (txtSurname.Text.Trim() != "")
                search = search.Where(x => x.Surname.Contains(txtSurname.Text)).ToList();
            if (cmbDepartment.SelectedIndex != -1)
                search = search.Where(x => x.DepartmentId == GetDepartmentId()).ToList();
            if (cmbPosition.SelectedIndex != -1)
                search = search.Where(x => x.PositionId == GetPositionId()).ToList();
            if (cmbState.SelectedIndex != -1)
                search = search.Where(x => x.PermissionState == GetStateId()).ToList();
            if (rbStartDate.IsChecked == true)
                search = search.Where(x => x.StartDate > dpStart.SelectedDate && x.StartDate < dpEnd.SelectedDate).ToList();
            if (rbEndDate.IsChecked == true)
                search = search.Where(x => x.EndDate > dpStart.SelectedDate && x.EndDate < dpEnd.SelectedDate).ToList();
            if (txtDayAmount.Text.Trim() != "")
                search = search.Where(x => x.DayAmount == Convert.ToInt32(txtDayAmount.Text)).ToList();

            gridPermission.ItemsSource = search;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelExist()) return;

            var page = new PermissionPage();
            page.Model = _model;
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (IsModelExist() && _model.PermissionState == Definitions.PermissionStates.OnEmployee)
            {
                var permission = _db.Permissions.Find(_model.Id);
                permission.PermissionState = Definitions.PermissionStates.Approved;
                _db.SaveChanges();
                MessageBox.Show("Permission was approved!");
                FillDataGrid();
            }
        }

        private void btnDisapprove_Click(object sender, RoutedEventArgs e)
        {
            if (IsModelExist() && _model.PermissionState == Definitions.PermissionStates.OnEmployee)
            {
                var permission = _db.Permissions.Find(_model.Id);
                permission.PermissionState = Definitions.PermissionStates.Disapproved;
                _db.SaveChanges();
                MessageBox.Show("Permission was disapproved!");
                FillDataGrid();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelExist()) return;

            if (MessageBox.Show("Are you sure to delete?", "Question",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                var permission = _db.Permissions.Find(_model.Id);
                _db.Permissions.Remove(permission);
                _db.SaveChanges();
                MessageBox.Show("Permission was deleted!");
                FillDataGrid();
            }
        }
        #endregion

        #region SIDE METHODS
        private void FillDataGrid()
        {
            _permissions = _db.Permissions
                .Include(x => x.Employee)
                .Include(x => x.PermissionStateNavigation)
                .Select(x => new PermissionDetailModel()
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    UserNo = x.Employee.UserNo,
                    Name = x.Employee.Name,
                    Surname = x.Employee.Surname,
                    StartDate = (DateTime)x.StartDate,
                    EndDate = x.EndDate,
                    StateName = x.PermissionStateNavigation.StateName,
                    DayAmount = x.PermissionAmount,
                    Explanation = x.Explanation,
                    PermissionState = x.PermissionState,
                    DepartmentId = x.Employee.DepartmentId,
                    PositionId = x.Employee.PositionId
                }).OrderByDescending(x => x.StartDate).ToList();
            gridPermission.ItemsSource = _permissions;
        }

        private void FillPositionCombobox(List<Position> positionList)
        {
            cmbPosition.ItemsSource = positionList;
            cmbPosition.DisplayMemberPath = "PositionName";
            cmbPosition.SelectedValue = "Id";
            cmbPosition.SelectedIndex = -1;
        }

        private void FillDepartmentCombobox(List<Department> departmentList)
        {
            cmbDepartment.ItemsSource = departmentList;
            cmbDepartment.DisplayMemberPath = "DepartmentName";
            cmbDepartment.SelectedValue = "Id";
            cmbDepartment.SelectedIndex = -1;
        }

        private void FillStateCombobox(List<Permissionstate> stateList)
        {
            cmbState.ItemsSource = stateList;
            cmbState.DisplayMemberPath = "PermissionState";
            cmbState.SelectedValue = "Id";
            cmbState.SelectedIndex = -1;
        }

        private int GetDepartmentId()
        {
            var selected = (Department)cmbDepartment.SelectedItem;
            return selected.Id;
        }

        private int GetPositionId()
        {
            var selected = (Position)cmbPosition.SelectedItem;
            return selected.Id;
        }

        private int GetStateId()
        {
            var selected = (Permissionstate)cmbState.SelectedItem;
            return selected.Id;
        }

        private bool IsModelExist()
        {
            if (_model != null && _model.Id != 0) return true;

            MessageBox.Show("Please select a permission from table!");
            return false;
        }

        private void ClearFields()
        {
            txtDayAmount.Clear();
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbState.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbPosition.ItemsSource = _positions;
            dpStart.SelectedDate = DateTime.Today;
            dpEnd.SelectedDate = DateTime.Today;
            rbStartDate.IsChecked = false;
            rbEndDate.IsChecked = false;
            gridPermission.ItemsSource = _permissions;
        }
        #endregion
    }
}
