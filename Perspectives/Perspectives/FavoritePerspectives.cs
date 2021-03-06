﻿using System;
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
            MyFavorites = new List<Favorite>();
        }

        public void Save()
        {
            try
            {
                using (var writer = new StreamWriter(favPath))
                {
                    var s = new XmlSerializer(typeof(List<Favorite>));
                    s.Serialize(writer, MyFavorites);    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to save perspectives file!", "IO Failure", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(favPath))
                {
                    try
                    {
                        using (var reader = new StreamReader(favPath))
                        {
                            var s = new XmlSerializer(typeof(List<Favorite>));
                            MyFavorites = (List<Favorite>)s.Deserialize(reader);
                        }

                    }
                    catch
                    {
                        //Supports backwards compatibility
                        using (var reader = new StreamReader(favPath))
                        {
                            var s = new XmlSerializer(typeof(List<string>));
                            var names = (List<string>)s.Deserialize(reader);

                            int i = 1;
                            foreach (var name in names)
                            {
                                MyFavorites = new List<Favorite>();
                                MyFavorites.Add(new Favorite { Name = name, Ordinal = i });
                                i++;
                            }
                        }

                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception)
            {
                MyFavorites = new List<Favorite>();
            }
        }

        public List<Favorite> MyFavorites { get; private set; }
    }

    public class Favorite
    {
        public string Name { get; set; }
        public int Ordinal { get; set; }
    }
}
