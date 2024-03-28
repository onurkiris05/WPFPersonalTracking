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
using WPFPersonalTracking.Pages;

namespace WPFPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for DepartmentList.xaml
    /// </summary>
    public partial class DepartmentList : UserControl
    {
        PersonaltrackingContext _db = new();
        Department _model = new();

        public DepartmentList()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }

        private void gridDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model = (Department)gridDepartment.SelectedItem;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new DepartmentPage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelExist()) return;

            var page = new DepartmentPage();
            page.Department = _model;
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
                var department = _db.Departments.Find(_model.Id);
                _db.Departments.Remove(department);
                _db.SaveChanges();
                MessageBox.Show("Department was deleted!");
                FillDataGrid();
            }
        }
        #endregion

        #region SIDE METHODS
        private void FillDataGrid()
        {
            List<Department> list = _db.Departments.OrderBy(x => x.DepartmentName).ToList();
            gridDepartment.ItemsSource = list;
        }

        private bool IsModelExist()
        {
            if (_model != null && _model.Id != 0) return true;

            MessageBox.Show("Please select a department from table!");
            return false;
        }
        #endregion
    }
}
