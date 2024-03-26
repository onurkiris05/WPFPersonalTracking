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
using WPFPersonalTracking.ViewModels;

namespace WPFPersonalTracking
{
    /// <summary>
    /// Interaction logic for SalaryPage.xaml
    /// </summary>
    public partial class SalaryPage : Window
    {
        public SalaryDetailModel Model;
        PersonaltrackingContext _db = new();
        List<Employee> _employeeList = new();
        List<Position> _positions = new();
        int _employeeId = 0;

        public SalaryPage()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _employeeList = _db.Employees.ToList();
            _positions = _db.Positions.ToList();
            gridEmployee.ItemsSource = _employeeList;

            FillDepartmentCombobox(_db.Departments.ToList());
            FillPositionCombobox(_db.Positions.ToList());
            FillMonthCombobox(_db.Salarymonths.ToList());

            if (IsModelExist())
            {
                txtName.Text = Model.Name;
                txtSurname.Text = Model.Surname;
                txtSalary.Text = Model.Amount.ToString();
                txtUserNo.Text = Model.UserNo.ToString();
                txtYear.Text = Model.Year.ToString();
                _employeeId = Model.EmployeeId;
                cmbMonth.SelectedValue = Model.MonthId;
            }
        }

        private void gridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var employee = (Employee)gridEmployee.SelectedItem;
            txtUserNo.Text = employee.UserNo.ToString();
            txtName.Text = employee.Name;
            txtSurname.Text = employee.Surname;
            txtYear.Text = DateTime.Now.Year.ToString();
            txtSalary.Text = employee.Salary.ToString();
            _employeeId = employee.Id;
        }

        private void cmbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridEmployee.ItemsSource = _employeeList.Where(x => x.DepartmentId == GetDepartmentId()).ToList();
            if (cmbDepartment.SelectedIndex != -1)
            {
                cmbPosition.ItemsSource = _positions.Where(x => x.DepartmentId == GetDepartmentId()).ToList();
                cmbPosition.DisplayMemberPath = "PositionName";
                //cmbPosition.SelectedValuePath = "Id";
                cmbPosition.SelectedIndex = -1;
            }
        }

        private void cmbPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridEmployee.ItemsSource = _employeeList.Where(x => x.PositionId == GetPositionId()).ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtSalary.Text.Trim() == "" || txtYear.Text.Trim() == "" || cmbMonth.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill the necessary areas!");
                return;
            }

            if (IsModelExist())
            {
                var salary = _db.Salaries.Find(Model.Id);
                var oldSalary = salary.Amount;
                salary.Amount = Convert.ToInt32(txtSalary.Text);
                salary.Month = (Salarymonth)cmbMonth.SelectedValue;
                salary.Year = Convert.ToInt32(txtYear.Text);
                salary.EmployeeId = _employeeId;
                _db.SaveChanges();

                if (oldSalary < salary.Amount)
                {
                    var employee = _db.Employees.Find(_employeeId);
                    employee.Salary = salary.Amount;
                    _db.SaveChanges();
                }
                MessageBox.Show("Salary was updated!");
            }
            else
            {
                if (_employeeId == 0)
                {
                    MessageBox.Show("Please select an employee from table!");
                    return;
                }

                AddSalary();
                ClearFields();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void FillMonthCombobox(List<Salarymonth> monthList)
        {
            cmbMonth.ItemsSource = monthList;
            cmbMonth.DisplayMemberPath = "MonthName";
            cmbMonth.SelectedValue = "Id";
            cmbMonth.SelectedIndex = -1;
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
            return Model != null && Model.Id != 0;
        }

        private void AddSalary()
        {
            var salary = new Salary();
            salary.EmployeeId = _employeeId;
            salary.Amount = Convert.ToInt32(txtSalary.Text);
            salary.Month = (Salarymonth)cmbMonth.SelectedValue;
            salary.Year = Convert.ToInt32(txtYear.Text);
            _db.Salaries.Add(salary);
            _db.SaveChanges();
            MessageBox.Show("Salary was added!");
            _employeeId = 0;
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtSalary.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            txtYear.Text = DateTime.Now.Year.ToString();
            gridEmployee.ItemsSource = _employeeList;
            cmbPosition.ItemsSource = _positions;
            cmbMonth.SelectedIndex = -1;
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
        }


        #endregion
    }
}
