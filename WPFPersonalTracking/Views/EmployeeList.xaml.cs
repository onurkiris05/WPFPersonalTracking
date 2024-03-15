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
    /// Interaction logic for EmployeeList.xaml
    /// </summary>
    public partial class EmployeeList : UserControl
    {
        PersonaltrackingContext _db = new();
        List<Position> _positions = new();
        List<EmployeeDetailModel> _employeeList = new();

        public EmployeeList()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new EmployeePage();
            page.ShowDialog();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _positions = _db.Positions.ToList();
            FillPositionCombobox(_positions);
            FillDepartmentCombobox(_db.Departments.ToList());

            // Load employees to table
            _employeeList = _db.Employees
                .Include(x => x.Position)
                .Include(x => x.Department)
                .Select(x => new EmployeeDetailModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Adress = x.Adress,
                    Birthday = (DateTime)x.Birthday,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.DepartmentName,
                    IsAdmin = x.IsAdmin,
                    Password = x.Password,
                    PositionId = x.PositionId,
                    PositionName = x.Position.PositionName,
                    Salary = x.Salary,
                    Surname = x.Surname,
                    UserNo = x.UserNo,
                }).ToList();

            gridEmployee.ItemsSource = _employeeList;
        }

        private void cmbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDepartment.SelectedIndex != -1)
            {
                cmbPosition.ItemsSource = _positions.Where(x => x.DepartmentId == GetDepartmentId()).ToList();
                cmbPosition.DisplayMemberPath = "PositionName";
                //cmbPosition.SelectedValuePath = "Id";
                cmbPosition.SelectedIndex = -1;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Search with different fields if there is one
            var searchList = _employeeList;

            if (!string.IsNullOrWhiteSpace(txtUserNo.Text))
                searchList = searchList.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            if (!string.IsNullOrWhiteSpace(txtName.Text))
                searchList = searchList.Where(x => x.Name.Contains(txtName.Text)).ToList();
            if (!string.IsNullOrWhiteSpace(txtSurname.Text))
                searchList = searchList.Where(x => x.Surname.Contains(txtSurname.Text)).ToList();
            if (cmbPosition.SelectedIndex != -1)
                searchList = searchList.Where(x => x.PositionId == GetPositionId()).ToList();
            if (cmbDepartment.SelectedIndex != -1)
                searchList = searchList.Where(x => x.DepartmentId == GetDepartmentId()).ToList();

            gridEmployee.ItemsSource = searchList;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // Reset all fields
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbPosition.ItemsSource = _positions;
            gridEmployee.ItemsSource = _employeeList;
        }
        #endregion

        #region SIDE METHODS
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

        #endregion


    }
}
