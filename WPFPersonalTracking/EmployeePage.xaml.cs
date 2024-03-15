using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Window
    {
        PersonaltrackingContext _db = new();
        List<Position> _positions = new();
        OpenFileDialog _dialog = new OpenFileDialog();

        public EmployeePage()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _positions = _db.Positions.ToList();
            FillPositionCombobox(_positions);
            FillDepartmentCombobox(_db.Departments.ToList());
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

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            if (_dialog.ShowDialog() == true)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(_dialog.FileName);
                img.EndInit();
                EmployeeImage.Source = img;
                txtImage.Text = _dialog.FileName;
            }
        }

        private void txtUserNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            var employee = CreateEmployeeFromFields();
            SaveEmployeeToDatabase(employee);
            ClearAllFields();
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (IsUserNoInUse())
                MessageBox.Show("UserNo is already in use!");
            else
                MessageBox.Show("UserNo is available!");
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

        private bool ValidateFields()
        {
            if (IsAnyFieldEmpty())
            {
                MessageBox.Show("Please fill all the necessary areas!");
                return false;
            }

            if (IsUserNoInUse())
            {
                MessageBox.Show("UserNo is already in use!");
                return false;
            }

            return true;
        }

        private bool IsUserNoInUse()
        {
            var uniqueList = _db.Employees.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            return uniqueList.Count > 0;
        }

        private bool IsAnyFieldEmpty()
        {
            return string.IsNullOrWhiteSpace(txtUserNo.Text)
                || string.IsNullOrWhiteSpace(txtPassword.Text)
                || string.IsNullOrWhiteSpace(txtName.Text)
                || string.IsNullOrWhiteSpace(txtSurname.Text)
                || string.IsNullOrWhiteSpace(txtSalary.Text)
                || string.IsNullOrWhiteSpace(txtImage.Text)
                || txtAdress.Document.Blocks.Count == 0
                || picker1.SelectedDate == null
                || cmbDepartment.SelectedIndex == -1
                || cmbPosition.SelectedIndex == -1;
        }

        private Employee CreateEmployeeFromFields()
        {
            var employee = new Employee
            {
                UserNo = Convert.ToInt32(txtUserNo.Text),
                Password = txtPassword.Text,
                Name = txtName.Text,
                Surname = txtSurname.Text,
                Salary = Convert.ToInt32(txtSalary.Text),
                DepartmentId = GetDepartmentId(),
                PositionId = GetPositionId(),
                Adress = new TextRange(txtAdress.Document.ContentStart, txtAdress.Document.ContentEnd).Text,
                Birthday = picker1.SelectedDate,
                IsAdmin = (bool)chisAdmin.IsChecked,
                ImagePath = GenerateImagePath()
            };

            File.Copy(txtImage.Text, @"Images//" + employee.ImagePath);

            return employee;
        }

        private void SaveEmployeeToDatabase(Employee employee)
        {
            _db.Employees.Add(employee);
            _db.SaveChanges();
            MessageBox.Show("Employee was added!");
        }

        private void ClearAllFields()
        {
            txtUserNo.Clear();
            txtPassword.Clear();
            txtName.Clear();
            txtSurname.Clear();
            txtSalary.Clear();
            txtImage.Clear();
            txtAdress.Document.Blocks.Clear();
            picker1.SelectedDate = DateTime.Today;
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbPosition.ItemsSource = _positions;
            chisAdmin.IsChecked = false;
            EmployeeImage.Source = new BitmapImage();
        }

        private string GenerateImagePath()
        {
            var unique = Guid.NewGuid().ToString();
            return unique + _dialog.SafeFileName;
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
