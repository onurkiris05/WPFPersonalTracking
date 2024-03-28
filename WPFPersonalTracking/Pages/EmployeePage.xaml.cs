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
using WPFPersonalTracking.DetailModels;
using WPFPersonalTracking.Statics;

namespace WPFPersonalTracking.Pages
{
    /// <summary>
    /// Interaction logic for EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Window
    {
        public EmployeeDetailModel Model { get; set; }
        PersonaltrackingContext _db = new();
        List<Position> _positions = new();
        OpenFileDialog _dialog = new();

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

            if (IsModelExist()) UpdatePage();

            if (!UserStatic.IsAdmin)
            {
                chisAdmin.IsEnabled = false;
                txtUserNo.IsEnabled = false;
                txtSalary.IsEnabled = false;
                cmbDepartment.IsEnabled = false;
                cmbPosition.IsEnabled = false;
            }
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

        private void txtUserNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!AreFieldsValid()) return;

            if (IsModelExist())
            {
                UpdateEmployee();
            }
            else
            {
                var employee = CreateEmployeeFromFields();
                SaveEmployeeToDatabase(employee);
                ClearAllFields();
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (IsUserNoInUse())
                MessageBox.Show("UserNo is already in use!");
            else
                MessageBox.Show("UserNo is available!");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region SIDE METHODS
        private void UpdatePage()
        {
            cmbDepartment.SelectedValue = GetDepartmentValue(Model.DepartmentId);
            cmbPosition.SelectedValue = GetPositionValue(Model.PositionId);
            txtUserNo.Text = Model.UserNo.ToString();
            txtPassword.Text = Model.Password.ToString();
            txtName.Text = Model.Name.ToString();
            txtSurname.Text = Model.Surname.ToString();
            txtSalary.Text = Model.Salary.ToString();
            txtAdress.AppendText(Model.Adress.ToString());
            picker1.SelectedDate = Model.Birthday;
            chisAdmin.IsChecked = Model.IsAdmin;
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"Images/" + Model.ImagePath, UriKind.RelativeOrAbsolute);
            image.EndInit();
            EmployeeImage.Source = image;
        }

        private void UpdateEmployee()
        {
            Employee employee = _db.Employees.Find(Model.Id);
            if (txtImage.Text.Trim() != "")
            {
                //Delete old image if exist
                if (File.Exists(@"Images//" + employee.ImagePath))
                {
                    File.Delete(@"Images//" + employee.ImagePath);
                    var fileName = "";
                    var unique = Guid.NewGuid().ToString();
                    fileName += unique + System.IO.Path.GetFileName(txtImage.Text);
                    File.Copy(txtImage.Text, @"Images//" + fileName);
                    employee.ImagePath = fileName;
                }
            }
            employee.UserNo = Convert.ToInt32(txtUserNo.Text);
            employee.Password = txtPassword.Text;
            employee.IsAdmin = (bool)chisAdmin.IsChecked;
            var adress = new TextRange(txtAdress.Document.ContentStart, txtAdress.Document.ContentEnd);
            employee.Adress = adress.Text;
            employee.Birthday = picker1.SelectedDate;
            employee.DepartmentId = GetDepartmentId();
            employee.PositionId = GetPositionId();
            employee.Name = txtName.Text;
            employee.Surname = txtSurname.Text;
            employee.Salary = Convert.ToInt32(txtSalary.Text);
            _db.SaveChanges();
            MessageBox.Show("Employee was updated!");
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

        private bool AreFieldsValid()
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

        private bool IsModelExist()
        {
            return Model != null && Model.Id != 0;
        }

        private object GetDepartmentValue(int departmentId)
        {
            return _db.Departments.FirstOrDefault(d => d.Id == departmentId);
        }

        private object GetPositionValue(int positionId)
        {
            return _db.Positions.FirstOrDefault(d => d.Id == positionId);
        }
        #endregion
    }
}
