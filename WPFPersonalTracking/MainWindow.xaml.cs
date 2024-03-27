using System.Text;
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

namespace WPFPersonalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsClosing { get; private set; } = false;
        PersonaltrackingContext _db = new();

        public MainWindow()
        {
            InitializeComponent();
            lblWindowName.Content = "Task List";
            DataContext = new TaskViewModel();
        }

        private void btnDepartment_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Department List";
            DataContext = new DepartmentViewModel();
        }

        private void btnPosition_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Position List";
            DataContext = new PositionViewModel();
        }

        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (!UserStatic.IsAdmin)
            {
                Employee employee = _db.Employees.Find(UserStatic.EmployeeId);
                var model = new EmployeeDetailModel();
                model.Adress = employee.Adress;
                model.Birthday = (DateTime)employee.Birthday;
                model.DepartmentId = employee.DepartmentId;
                model.Id = employee.Id;
                model.ImagePath = employee.ImagePath;
                model.IsAdmin = employee.IsAdmin;
                model.Name = employee.Name;
                model.Password = employee.Password;
                model.PositionId = employee.PositionId;
                model.Salary = employee.Salary;
                model.Surname = employee.Surname;
                model.UserNo = employee.UserNo;

                var page = new EmployeePage();
                page.Model = model;
                page.ShowDialog();
            }
            else
            {
                lblWindowName.Content = "Employee List";
                DataContext = new EmployeeViewModel();
            }
        }

        private void btnTask_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Task List";
            DataContext = new TaskViewModel();
        }

        private void btnSalary_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Salary List";
            DataContext = new SalaryViewModel();
        }

        private void btnPermission_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Permission List";
            DataContext = new PermissionViewModel();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to logout?", "Question",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit?", "Question",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                IsClosing = true;
                Application.Current.Shutdown();
            }
        }

        private void PersonalMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserStatic.IsAdmin)
            {
                stackDepartment.Visibility = Visibility.Hidden;
                stackPosition.Visibility = Visibility.Hidden;
                stackLogout.SetValue(Grid.RowProperty, 5);
                stackExit.SetValue(Grid.RowProperty, 6);
            }
        }
    }
}