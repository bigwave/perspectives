using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace AdamDriscoll.Perspectives
{
    public class Perspective
    {
        private readonly DTE _dte;

        private WindowConfiguration Configuration { get; set; }

        public Perspective(DTE dte)
        {
            this._dte = dte;
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
            List<Perspective> p = new List<Perspective>();
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
            return p;
        }

        public Perspective AddNew(string name)
        {
            var addPer = _dte.WindowConfigurations.Add(name);
            addPer.Update();
            addPer.Apply();
            
            return new Perspective(_dte) { Configuration = addPer };
        }

        public void Apply()
        {
            Configuration.Apply();
        }

        public void Delete()
        {
            Configuration.Delete();
        }

        public void Update()
        {
            Configuration.Update();
        }

        public override string ToString()
        {
            return Configuration.Name;
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
                        "pack://application:,,,/Perspectives;component/current.ico";
                }
                return string.Empty;
            }
        }
    }
}
