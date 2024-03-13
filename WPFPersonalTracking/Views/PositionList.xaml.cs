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
    /// Interaction logic for PositionList.xaml
    /// </summary>
    public partial class PositionList : UserControl
    {
        public PositionList()
        {
            InitializeComponent();
        }

        private void FillGrid()
        {
            var db = new PersonaltrackingContext();
            var list = db.Positions.Include(x => x.Department).Select(a => new
            {
                positionId = a.Id,
                positionName = a.PositionName,
                departmentId = a.DepartmentId,
                departmentName = a.Department.DepartmentName
            }).OrderBy(x => x.positionName).ToList();

            var modellist = new List<PositionModel>();
            foreach (var item in list)
            {
                var model = new PositionModel();
                model.Id = item.positionId;
                model.PositionName = item.positionName;
                model.DepartmentId = item.departmentId;
                model.DepartmentName = item.departmentName;
                modellist.Add(model);
            }

            gridPosition.ItemsSource = modellist;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new PositionPage();
            page.ShowDialog();
            FillGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var model = (PositionModel)gridPosition.SelectedItem;
            if (model != null && model.Id != 0)
            {
                var page = new PositionPage();
                page.Model = model;
                page.ShowDialog();
                FillGrid();
            }
        }
    }
}
