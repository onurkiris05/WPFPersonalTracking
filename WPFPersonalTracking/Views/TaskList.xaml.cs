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
using WPFPersonalTracking.Statics;

namespace WPFPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for TaskList.xaml
    /// </summary>
    public partial class TaskList : UserControl
    {
        PersonaltrackingContext _db = new();
        List<Position> _positions = new();
        List<TaskDetailModel> _taskList = new();
        List<TaskDetailModel> _searchList = new();
        TaskDetailModel _model = new();

        public TaskList()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();

            if (!UserStatic.IsAdmin)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
                btnApprove.SetValue(Grid.ColumnProperty, 1);
                btnApprove.Content = "Delivery";
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

        private void gridTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model = (TaskDetailModel)gridTask.SelectedItem;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new TaskPage();
            page.ShowDialog();

            FillDataGrid();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            gridTask.ItemsSource = FilterByFields();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var page = new TaskPage();
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

                var taskModel = (TaskDetailModel)gridTask.SelectedItem;
                var task = _db.Tasks.First(x => x.Id == taskModel.Id);
                _db.Tasks.Remove(task);
                _db.SaveChanges();
                MessageBox.Show("Task was deleted!");
                FillDataGrid();
            }
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelExist()) return;

            if (UserStatic.IsAdmin & _model.TaskState == Definitions.TaskStates.OnEmployee)
                MessageBox.Show("Before approve a task, Task must be delivered!");
            else if (_model.TaskState == Definitions.TaskStates.Approved)
                MessageBox.Show("This task is already approved!");
            else if (!UserStatic.IsAdmin & _model.TaskState == Definitions.TaskStates.Delivered)
                MessageBox.Show("This task is already delivered!");
            else
            {
                var task = _db.Tasks.Find(_model.Id);
                if (UserStatic.IsAdmin)
                    task.TaskState = Definitions.TaskStates.Approved;
                else
                    task.TaskState = Definitions.TaskStates.Delivered;

                _db.SaveChanges();
                MessageBox.Show("Task was updated!");
                FillDataGrid();
            }

        }
        #endregion

        #region SIDE METHODS
        private void FillDataGrid()
        {
            _taskList = _db.Tasks.Include(x => x.TaskStateNavigation).Include(x => x.Employee)
                .ThenInclude(x => x.Department).ThenInclude(x => x.Positions).Select(x => new TaskDetailModel()
                {
                    Id = x.Id,
                    EmployeeId = (int)x.EmployeeId,
                    Name = x.Employee.Name,
                    StateName = x.TaskStateNavigation.StateName,
                    Surname = x.Employee.Surname,
                    TaskContent = x.TaskContent,
                    TaskDeliveryDate = x.TaskDeliveryDate,
                    TaskStartDate = (DateTime)x.TaskStartDate,
                    TaskState = (int)x.TaskState,
                    TaskTitle = x.TaskTitle,
                    UserNo = x.Employee.UserNo,
                    DepartmentId = x.Employee.DepartmentId,
                    PositionId = x.Employee.PositionId,
                }).ToList();

            if (!UserStatic.IsAdmin)
            {
                _taskList = _taskList.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
                txtUserNo.IsEnabled = false;
                txtName.IsEnabled = false;
                txtSurname.IsEnabled = false;
                cmbDepartment.IsEnabled = false;
                cmbPosition.IsEnabled = false;
            }

            gridTask.ItemsSource = _taskList;
            _searchList = _taskList;
            _positions = _db.Positions.ToList();

            FillDepartmentCombobox(_db.Departments.ToList());
            FillPositionCombobox(_db.Positions.ToList());
            FillStateCombobox(_db.Taskstates.ToList());
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

        private void FillStateCombobox(List<Taskstate> stateList)
        {
            cmbState.ItemsSource = stateList;
            cmbState.DisplayMemberPath = "StateName";
            cmbState.SelectedValue = "Id";
            cmbState.SelectedIndex = -1;
        }

        private void ClearFields()
        {
            txtUserNo.Clear();
            txtName.Clear();
            txtSurname.Clear();
            dpDelivery.SelectedDate = DateTime.Today;
            dpStart.SelectedDate = DateTime.Today;
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbState.SelectedIndex = -1;
            cmbPosition.ItemsSource = _positions;
            rbDelivery.IsChecked = false;
            rbStart.IsChecked = false;
            gridTask.ItemsSource = _taskList;
        }

        private List<TaskDetailModel> FilterByFields()
        {
            var search = _searchList;

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
                search = search.Where(x => x.TaskState == GetTaskStateId()).ToList();
            if (rbStart.IsChecked == true)
                search = search.Where(x => x.TaskStartDate > dpStart.SelectedDate && x.TaskStartDate < dpDelivery.SelectedDate).ToList();
            if (rbDelivery.IsChecked == true)
                search = search.Where(x => x.TaskDeliveryDate > dpStart.SelectedDate && x.TaskDeliveryDate < dpDelivery.SelectedDate).ToList();

            return search;
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

        private int GetTaskStateId()
        {
            var selected = (Taskstate)cmbState.SelectedItem;
            return selected.Id;
        }

        private bool IsModelExist()
        {
            if (_model != null && _model.Id != 0) return true;

            MessageBox.Show("Please select a task from table!");
            return false;
        }

        #endregion
    }
}
