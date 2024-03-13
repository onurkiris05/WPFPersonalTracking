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

namespace WPFPersonalTracking
{
    /// <summary>
    /// Interaction logic for DepartmentPage.xaml
    /// </summary>
    public partial class DepartmentPage : Window
    {
        public Department Department { get; set; }
        private bool IsDepartmentExist() => Department != null && Department.Id != 0;

        public DepartmentPage()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtDepartmentName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter depertmant name!");
            }
            else
            {
                using (PersonaltrackingContext db = new PersonaltrackingContext())
                {
                    if (IsDepartmentExist())
                    {
                        var update = new Department();
                        update.DepartmentName = txtDepartmentName.Text;
                        update.Id = Department.Id;
                        db.Departments.Update(update);
                        db.SaveChanges();
                        MessageBox.Show($"'{Department.DepartmentName}' " +
                            $"updated to '{txtDepartmentName.Text}' successfully!");
                        txtDepartmentName.Text = "";
                    }
                    else
                    {
                        var dpt = new Department();
                        dpt.DepartmentName = txtDepartmentName.Text;
                        db.Departments.Add(dpt);
                        db.SaveChanges();
                        txtDepartmentName.Clear();
                        MessageBox.Show("Department added!");
                    }

                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsDepartmentExist())
            {
                txtDepartmentName.Text = Department.DepartmentName;
            }
        }
    }
}
