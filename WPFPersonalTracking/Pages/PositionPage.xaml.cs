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
using WPFPersonalTracking.DetailModels;

namespace WPFPersonalTracking.Pages
{
    /// <summary>
    /// Interaction logic for PositionPage.xaml
    /// </summary>
    public partial class PositionPage : Window
    {
        public PositionDetailModel Model { get; set; }
        PersonaltrackingContext _db = new ();

        public PositionPage()
        {
            InitializeComponent();
        }

        #region EVENT METHODS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = _db.Departments.ToList().OrderBy(x => x.DepartmentName);
            cmbDepartment.ItemsSource = list;
            cmbDepartment.DisplayMemberPath = "DepartmentName";
            cmbDepartment.SelectedValuePath = "Id";
            cmbDepartment.SelectedIndex = -1;

            if (IsModelExist())
            {
                cmbDepartment.SelectedValue = Model.DepartmentId;
                txtPositionName.Text = Model.PositionName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDepartment.SelectedIndex == -1 || txtPositionName.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all areas!");
            }
            else
            {
                if (IsModelExist())
                {
                    var pst = new Position();
                    pst.DepartmentId = (int)cmbDepartment.SelectedValue;
                    pst.Id = Model.Id;
                    pst.PositionName = txtPositionName.Text;
                    _db.Positions.Update(pst);
                    _db.SaveChanges();
                    MessageBox.Show("Position was updated!");
                }
                else
                {
                    var position = new Position();
                    position.PositionName = txtPositionName.Text;
                    position.DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue);
                    _db.Positions.Add(position);
                    _db.SaveChanges();
                    cmbDepartment.SelectedIndex = -1;
                    txtPositionName.Clear();
                    MessageBox.Show("Position was added!");
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region SIDE METHODS
        private bool IsModelExist() => Model != null && Model.Id != 0; 
        #endregion
    }
}
