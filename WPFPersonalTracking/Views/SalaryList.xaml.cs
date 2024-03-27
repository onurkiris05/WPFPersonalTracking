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
    /// Interaction logic for SalaryList.xaml
    /// </summary>
    public partial class SalaryList : UserControl
    {
        PersonaltrackingContext _db = new();
        List<SalaryDetailModel> _salaries = new();
        List<Position> _positions = new();
        SalaryDetailModel _model = new();

        public SalaryList()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _positions = _db.Positions.ToList();

            FillDataGrid();
            FillDepartmentCombobox(_db.Departments.ToList());
            FillPositionCombobox(_db.Positions.ToList());
            FillMonthCombobox(_db.Salarymonths.ToList());
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
            var page = new SalaryPage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var search = _salaries;

            if (txtUserNo.Text.Trim() != "")
                search = search.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            if (txtName.Text.Trim() != "")
                search = search.Where(x => x.Name.Contains(txtName.Text)).ToList();
            if (txtSurname.Text.Trim() != "")
                search = search.Where(x => x.Surname.Contains(txtSurname.Text)).ToList();
            if (txtYear.Text.Trim() != "")
                search = search.Where(x => x.Year == Convert.ToInt32(txtYear.Text)).ToList();
            if (cmbDepartment.SelectedIndex != -1)
                search = search.Where(x => x.DepartmentId == GetDepartmentId()).ToList();
            if (cmbPosition.SelectedIndex != -1)
                search = search.Where(x => x.PositionId == GetPositionId()).ToList();
            if (cmbMonth.SelectedIndex != -1)
                search = search.Where(x => x.MonthId == GetMonthId()).ToList();

            if (txtSalary.Text.Trim() != "")
            {
                switch (true)
                {
                    case var _ when rbMore.IsChecked == true:
                        search = search.Where(x => x.Amount > Convert.ToInt32(txtSalary.Text)).ToList();
                        break;
                    case var _ when rbLess.IsChecked == true:
                        search = search.Where(x => x.Amount < Convert.ToInt32(txtSalary.Text)).ToList();
                        break;
                    case var _ when rbEquals.IsChecked == true:
                        search = search.Where(x => x.Amount == Convert.ToInt32(txtSalary.Text)).ToList();
                        break;
                }
            }
            gridSalary.ItemsSource = search;
        }

        private void gridSalary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model = (SalaryDetailModel)gridSalary.SelectedItem;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var page = new SalaryPage();
            page.Model = _model;
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                if (_model.Id == 0) return;

                var salaryModel = (SalaryDetailModel)gridSalary.SelectedItem;
                var salary = _db.Salaries.Find(salaryModel.Id);
                _db.Salaries.Remove(salary);
                _db.SaveChanges();
                MessageBox.Show("Salary was deleted!");
                FillDataGrid();
            }
        }
        #endregion

        #region SIDE METHODS
        private void FillDataGrid()
        {
            _salaries = _db.Salaries.Include(x => x.Employee).Include(x => x.Month).Select(x => new SalaryDetailModel()
            {
                UserNo = x.Employee.UserNo,
                Name = x.Employee.Name,
                Surname = x.Employee.Surname,
                Amount = x.Amount,
                EmployeeId = x.EmployeeId,
                Id = x.Id,
                MonthId = x.Month.Id,
                MonthName = x.Month.MonthName,
                Year = x.Year,
                DepartmentId = x.Employee.DepartmentId,
                PositionId = x.Employee.PositionId

            }).OrderByDescending(x => x.Year).OrderByDescending(x => x.MonthId).ToList();

            if (!UserStatic.IsAdmin)
            {
                _salaries = _salaries.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
                txtUserNo.IsEnabled = false;
                txtName.IsEnabled = false;
                txtSurname.IsEnabled = false;
                cmbDepartment.IsEnabled = false;
                cmbPosition.IsEnabled = false;
                btnAdd.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
            }

            gridSalary.ItemsSource = _salaries;
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

        private void FillMonthCombobox(List<Salarymonth> monthList)
        {
            cmbMonth.ItemsSource = monthList;
            cmbMonth.DisplayMemberPath = "MonthName";
            cmbMonth.SelectedValue = "Id";
            cmbMonth.SelectedIndex = -1;
        }

        private void ClearFields()
        {
            txtUserNo.Clear();
            txtName.Clear();
            txtSurname.Clear();
            txtYear.Clear();
            txtSalary.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbMonth.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbPosition.ItemsSource = _positions;
            rbMore.IsChecked = false;
            rbLess.IsChecked = false;
            rbEquals.IsChecked = false;
            gridSalary.ItemsSource = _salaries;
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

        private int GetMonthId()
        {
            var selected = (Salarymonth)cmbMonth.SelectedItem;
            return selected.Id;
        }

        #endregion
    }
}
