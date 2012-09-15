using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace AdamDriscoll.Perspectives
{
    public class Perspective
    {
        private readonly DTE _dte;
        private static List<Perspective> _cache;
        private FavoritePerspectives _favorites;

        private WindowConfiguration Configuration { get; set; }

        public Perspective(DTE dte)
        {
            _dte = dte;
            _favorites = new FavoritePerspectives();
            _favorites.Load();
        }

        public string Name
        {
            get
            {
                return Configuration.Name;
            }
        }

        public IEnumerable<Perspective> GetPerspectives()
        {
            return GetPerspectives(false);
        }

        public IEnumerable<Perspective> GetPerspectives(bool cached)
        {
            if (cached && _cache != null)
            {
                return _cache;
            }
            var p = new List<Perspective>();
            
            for (var i = 1; i <= _dte.WindowConfigurations.Count; i++)
            {
                try
                {
                    p.Add(new Perspective(_dte) { Configuration = _dte.WindowConfigurations.Item(i) });
                }
                catch
                {
                    continue;
                }
            }

            _cache = p;

            return p;
        }

        public IEnumerable<Perspective> GetFavoritePerspectives()
        {
            return GetPerspectives().Where(m => _favorites.MyFavorites.Select(x => x.Name).Contains(m.Name));
        }

        public Perspective AddNew(string name)
        {
            if (_cache.Any(m => m.Name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("A perspective by that name already exists.");
            }

            var addPer = _dte.WindowConfigurations.Add(name);
            addPer.Update();
            addPer.Apply();
            
            var p = new Perspective(_dte) { Configuration = addPer };

            _cache.Add(p);

            return p;
        }

        public void Apply()
        {
            Configuration.Apply();
        }

        public void Delete()
        {
            Configuration.Delete();

            _cache.Remove(_cache.Where(m => m.Name.Equals(this.Name)).First());
        }

        public void Update()
        {
            Configuration.Update();
        }

        public override string ToString()
        {
            return Configuration.Name;
        }

        public bool Favorite
        {
            get { return _favorites.MyFavorites.Any(m => m.Name.Equals(Name, System.StringComparison.InvariantCultureIgnoreCase)); }
            set
            {
                if (value)
                {
                    if (!Favorite)
                    {
                        var ordinal = _favorites.MyFavorites.Max(m => m.Ordinal) + 1;

                        _favorites.MyFavorites.Add(new Favorite { Name = Name, Ordinal = ordinal });
                        _favorites.Save();
                    }
                }
                else
                {
                    if (Favorite)
                    {
                        var fav = _favorites.MyFavorites.FirstOrDefault(m => m.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));

                        if (fav != null)
                        {
                            _favorites.MyFavorites.Remove(fav);
                            _favorites.Save();
                        }
                    }
                }
            }
        }

        public int FavoriteOrdinal
        {
            get 
            {
                if (Favorite)
                {
                    var fav = _favorites.MyFavorites.FirstOrDefault(m => m.Name.Equals(Name, System.StringComparison.InvariantCultureIgnoreCase));
                    if (fav != null)
                    {
                        return fav.Ordinal;
                    }
                }
                return -1;
            }
        }


        public Perspective Current
        {
            get
            {
                return GetPerspectives().Where(m => m.Name == _dte.WindowConfigurations.ActiveConfigurationName).First();
            }
        }

        public string IconUri
        {
            get
            {
                if (Current.Name.Equals(Name))
                {
                    return
                        "pack://application:,,,/Perspectives;component/navigate-right.ico";
                }
                return string.Empty;
            }
        }

        public string FavoriteIconUrl
        {
            get
            {
                if (!Favorite)
                {
                    return
                        "pack://application:,,,/Perspectives;component/add-to-favorites.ico";
                }
                else
                {
                    return
                        "pack://application:,,,/Perspectives;component/remove-from-favorites.ico";
                }
            }
        }
    }
}
