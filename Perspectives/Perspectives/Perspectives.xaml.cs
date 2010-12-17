using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EnvDTE;

namespace AdamDriscoll.Perspectives
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class PerspectivesControl : UserControl
    {
        public PerspectivesControl()
        {
            InitializeComponent();
        }

        public DTE Dte { get; set; }

        public void SetPerspectives(IEnumerable<Perspective> perspectives)
        {
            lstPerspectives.ItemsSource = perspectives;
        }

        public void RefreshUi()
        {
            var perspective = new Perspective(Dte);
            SetPerspectives(perspective.GetPerspectives());
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            var view = GetPerspective();

            if (view == null) return;

            view.Apply();
            RefreshUi();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            nameGrid.Visibility = Visibility.Visible;

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var view = GetPerspective();

            if (view == null) return;

            view.Update();
            RefreshUi();
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var view = GetPerspective();

            if (view == null) return;

            view.Delete();
            RefreshUi();
        }

        private Perspective GetPerspective()
        {
            var selectedView = (lstPerspectives.SelectedItem as Perspective);
            if (selectedView == null)
            {
                MessageBox.Show("Please select a perspective.", "Select a Perspective", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            return selectedView;
        }

        private void Fav_Click(object sender, RoutedEventArgs e)
        {
            var view = GetPerspective();

            if (view == null) return;

            var p = new FavoritePerspectives();
            p.Load();

            if (p.MyFavorites.Contains(view))
            {
                p.MyFavorites.Remove(view);
            }
            else
            {
                p.MyFavorites.Add(view);
            }
            
            p.Save();
            
            RefreshUi();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var name = txtName.Text;
            var perspective = new Perspective(Dte);

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a name.", "Empty name", MessageBoxButton.OK,
                                                            MessageBoxImage.Information);
                return;
            }

            if (perspective.GetPerspectives(true).Any(m => m.Name.Equals(name)))
            {
                MessageBox.Show("A perspective with that name already exits.", "Duplicate name", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                return;
            }

            perspective.AddNew(name);

            txtName.Text = string.Empty;
            nameGrid.Visibility = System.Windows.Visibility.Collapsed;

            RefreshUi();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txtName.Text = string.Empty;
            nameGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void FocusNew()
        {
            nameGrid.Visibility = System.Windows.Visibility.Visible;
            txtName.Focus();
        }

    }
}