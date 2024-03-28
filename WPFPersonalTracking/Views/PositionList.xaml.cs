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

namespace WPFPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for PositionList.xaml
    /// </summary>
    public partial class PositionList : UserControl
    {
        PersonaltrackingContext _db = new();
        PositionDetailModel _model = new();

        public PositionList()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }

        private void gridPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model = (PositionDetailModel)gridPosition.SelectedItem;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var page = new PositionPage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelExist()) return;

            var page = new PositionPage();
            page.Model = _model;
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
                var position = _db.Positions.Find(_model.Id);
                _db.Positions.Remove(position);
                _db.SaveChanges();
                MessageBox.Show("Position was deleted!");
                FillDataGrid();
            }
        }
        #endregion

        #region SIDE METHODS
        private void FillDataGrid()
        {
            var list = _db.Positions.Include(x => x.Department).Select(a => new
            {
                positionId = a.Id,
                positionName = a.PositionName,
                departmentId = a.DepartmentId,
                departmentName = a.Department.DepartmentName
            }).OrderBy(x => x.positionName).ToList();

            var modellist = new List<PositionDetailModel>();
            foreach (var item in list)
            {
                var model = new PositionDetailModel();
                model.Id = item.positionId;
                model.PositionName = item.positionName;
                model.DepartmentId = item.departmentId;
                model.DepartmentName = item.departmentName;
                modellist.Add(model);
            }

            gridPosition.ItemsSource = modellist;
        }

        private bool IsModelExist()
        {
            if (_model != null && _model.Id != 0) return true;

            MessageBox.Show("Please select a position from table!");
            return false;
        }
        #endregion
    }
}
