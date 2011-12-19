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
            favPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Perspectives\\perspectives.favorites.xml");

            var info = new FileInfo(favPath);

            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }

        }

        public FavoritePerspectives()
        {
            MyFavorites = new List<string>();
        }

        public void Save()
        {
            TextWriter writer = null;
            try
            {
                writer = new StreamWriter(favPath);
                var s = new XmlSerializer(typeof(List<string>));
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
                var s = new XmlSerializer(typeof(List<string>));
                MyFavorites = (List<string>)s.Deserialize(reader);
            }
            catch (Exception)
            {
                MyFavorites = new List<string>();
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

        public List<string> MyFavorites { get; private set; }



    }
}
