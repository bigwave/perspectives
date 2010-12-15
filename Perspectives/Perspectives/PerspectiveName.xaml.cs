using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdamDriscoll.Perspectives
{
    /// <summary>
    /// Interaction logic for PerspectiveName.xaml
    /// </summary>
    public partial class PerspectiveName : Window
    {
        public PerspectiveName()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text))
            {
                System.Windows.MessageBox.Show("Please enter a name.", "Name is required.", MessageBoxButton.OK,
                                               MessageBoxImage.Warning);
                return;
            }

            PerspectiveNameText = txtName.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public string PerspectiveNameText { get; set; }
    }
}
