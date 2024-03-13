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

namespace WPFPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for DepartmentList.xaml
    /// </summary>
    public partial class DepartmentList : UserControl
    {
        public DepartmentList()
        {
            InitializeComponent();
            FillGrid();
        }

        private void FillGrid()
        {
            using (PersonaltrackingContext db = new PersonaltrackingContext())
            {
                List<Department> list = db.Departments.OrderBy
                    (x => x.DepartmentName).ToList();
                gridDepartment.ItemsSource = list;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new DepartmentPage();
            page.ShowDialog();
            FillGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var dpt = (Department)gridDepartment.SelectedItem;
            var page = new DepartmentPage();
            page.Department = dpt;
            page.ShowDialog();
            FillGrid();
        }
    }
}
