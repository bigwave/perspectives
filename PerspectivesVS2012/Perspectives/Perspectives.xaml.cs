using System;
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
            if (Dte == null) return;
            var perspective = new Perspective(Dte);
            SetPerspectives(perspective.GetPerspectives());
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var view = (sender as Button).DataContext as Perspective;

                if (view != null)
                {
                    view.Apply();

                    RefreshUi();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var view = (sender as Button).DataContext as Perspective;

                if (view != null)
                {
                    view.Update();

                    RefreshUi();
                }
            }
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
              

                    var view = (sender as Button).DataContext as Perspective;
                    if (MessageBox.Show( string.Format("Delete {0}?",view.Name), "Delete View", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) //Ask to delete View jacobcordingley 3/25/2013
                    {
                    if (view != null)
                    {
                        view.Delete();
                        RefreshUi();
                    }
                }
            }
        }

        private void Fav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var view = (sender as Button).DataContext as Perspective;

                if (view != null)
                {
                    view.Favorite = !view.Favorite;
                    RefreshUi();                        
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshUi();
        }

    }
}