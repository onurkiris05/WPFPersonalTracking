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
using WPFPersonalTracking.ViewModels;

namespace WPFPersonalTracking
{
    /// <summary>
    /// Interaction logic for PositionPage.xaml
    /// </summary>
    public partial class PositionPage : Window
    {
        public PositionModel Model { get; set; }
        private bool IsModelExist() => Model != null && Model.Id != 0;

        PersonaltrackingContext db = new PersonaltrackingContext();

        public PositionPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = db.Departments.ToList().OrderBy(x => x.DepartmentName);
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
                    db.Positions.Update(pst);
                    db.SaveChanges();
                    MessageBox.Show("Position was updated!");
                }
                else
                {
                    var position = new Position();
                    position.PositionName = txtPositionName.Text;
                    position.DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue);
                    db.Positions.Add(position);
                    db.SaveChanges();
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
    }
}
