using System.Collections.Generic;
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
            var name = new PerspectiveName();
            var result = name.ShowDialog();

            if (!result.HasValue || result.Value == false)
            {
                return;
            }

            var perspective = new Perspective(Dte);
            perspective.AddNew(name.PerspectiveNameText);
            
            RefreshUi();
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

    }
}