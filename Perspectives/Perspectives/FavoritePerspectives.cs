using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace AdamDriscoll.Perspectives
{
    public class FavoritePerspectives 
    {
        private static string favPath;

        static FavoritePerspectives()
        {
            favPath = Path.Combine(Environment.CurrentDirectory, "perspectives.favorites.xml");
        }

        public FavoritePerspectives()
        {
            MyFavorites = new List<Perspective>();
        }

        public void Save()
        {
            TextWriter writer = null;
            try
            {
                writer = new StreamWriter(favPath);
                var s = new XmlSerializer(typeof(List<Perspective>));
                s.Serialize(writer, MyFavorites);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to save perspectives file!", "IO Failure", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public void Load()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(favPath);
                var s = new XmlSerializer(typeof(List<Perspective>));
                MyFavorites = (List<Perspective>)s.Deserialize(reader);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load perspectives file!", "IO Failure", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        public List<Perspective> MyFavorites { get; private set; }



    }
}
