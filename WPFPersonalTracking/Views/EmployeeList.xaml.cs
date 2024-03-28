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
using WPFPersonalTracking.DetailModels;
using WPFPersonalTracking.Pages;
using Task = WPFPersonalTracking.DB.Task;

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
        EmployeeDetailModel _model = new();

        public EmployeeList()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _positions = _db.Positions.ToList();
            FillDataGrid();
        }

        private void gridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model = (EmployeeDetailModel)gridEmployee.SelectedItem;
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new EmployeePage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            gridEmployee.ItemsSource = FilterByFields();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelExist()) return;

            var page = new EmployeePage();
            page.Model = _model;
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelExist()) return;

            if (MessageBox.Show("Are you sure to delete?", "Question",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                var employee = _db.Employees.Find(_model.Id);
                _db.Employees.Remove(employee);
                _db.SaveChanges();
                MessageBox.Show("Employee was deleted!");
                FillDataGrid();
            }
        }
        #endregion

        #region SIDE METHODS
        private void FillDataGrid()
        {
            FillPositionCombobox(_db.Positions.ToList());
            FillDepartmentCombobox(_db.Departments.ToList());

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
                    ImagePath = x.ImagePath
                }).ToList();

            gridEmployee.ItemsSource = _employeeList;
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

        private void ClearFields()
        {
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbPosition.ItemsSource = _positions;
            gridEmployee.ItemsSource = _employeeList;
        }

        private List<EmployeeDetailModel> FilterByFields()
        {
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
            return searchList;
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

        private bool IsModelExist()
        {
            if (_model != null && _model.Id != 0) return true;

            MessageBox.Show("Please select an employee from table!");
            return false;
        }
        #endregion
    }
}
