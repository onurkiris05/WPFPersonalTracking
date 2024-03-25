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
using Task = WPFPersonalTracking.DB.Task;

namespace WPFPersonalTracking.ViewModels
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Window
    {
        PersonaltrackingContext _db = new();
        List<Employee> _employeeList = new();
        List<Position> _positions = new();
        int _employeeId = 0;

        public TaskPage()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _employeeList = _db.Employees.OrderBy(x => x.Name).ToList();
            _positions = _db.Positions.ToList();
            gridEmployee.ItemsSource = _employeeList;

            FillDepartmentCombobox(_db.Departments.ToList());
            FillPositionCombobox(_db.Positions.ToList());
        }

        private void gridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var employee = (Employee)gridEmployee.SelectedItem;
            txtUserNo.Text = employee.UserNo.ToString();
            txtName.Text = employee.Name;
            txtSurname.Text = employee.Surname;
            _employeeId = employee.Id;
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!AreFieldsValid()) return;

            // Add task to db
            var task = new Task();
            task.EmployeeId = _employeeId;
            task.TaskStartDate = DateTime.Now;
            task.TaskTitle = txtTitle.Text;
            task.TaskContent = txtContent.Text;
            task.TaskState = Definitions.TaskStates.OnEmployee;
            _db.Tasks.Add(task);
            _db.SaveChanges();
            MessageBox.Show("Task was added!");
            _employeeId = 0;
            ClearFields();
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

        private bool AreFieldsValid()
        {
            if (_employeeId == 0)
            {
                MessageBox.Show("Please select an employee from table!");
                return false;
            }

            if (txtTitle.Text.Trim() == "" || txtContent.Text.Trim() == "")
            {
                MessageBox.Show("Please fill the necessary fields!");
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            txtContent.Clear();
            txtTitle.Clear();
            txtUserNo.Clear();
            txtName.Clear();
            txtSurname.Clear();
        }
        #endregion
    }
}
