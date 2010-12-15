using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace AdamDriscoll.Perspectives
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(PerspectivesToolWindow))]
    [Guid(GuidList.guidPerspectivesPkgString)]
    //[ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    public sealed class PerspectivesPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public PerspectivesPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(PerspectivesToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            var dte = (DTE) GetService(typeof (DTE));
            (window as PerspectivesToolWindow).SetDte(dte);
            (window as PerspectivesToolWindow).SetPerspectives(new Perspective(dte).GetPerspectives());

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID saveCommandId = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidSave);
                MenuCommand saveMenuItem = new MenuCommand(SaveCurrent, saveCommandId);
                mcs.AddCommand(saveMenuItem);

                CommandID menuCommandID = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidSaveAs);
                MenuCommand menuItem = new MenuCommand(SaveAsCurrent, menuCommandID);
                mcs.AddCommand(menuItem);

                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidViewManager);
                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand( menuToolWin );

                InitMRUMenu(mcs);
            }
        }

        private void InitMRUMenu(OleMenuCommandService mcs)
        {
           var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);

            int i = 0;
            foreach(var perspective in per.GetPerspectives())
            {
                var cmdID = new CommandID(
                    GuidList.guidPerspectivesCmdSet, 0x0109 + i);
                var mc = new OleMenuCommand(Apply, cmdID);
                mc.BeforeQueryStatus += new EventHandler(mc_BeforeQueryStatus);
                mcs.AddCommand(mc);
                i++;
            }
        }

        void mc_BeforeQueryStatus(object sender, EventArgs e)
        {
            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);

            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if (null != menuCommand)
            {
                int MRUItemIndex = menuCommand.CommandID.ID - 0x0109;
                if (MRUItemIndex >= 0 && MRUItemIndex < per.GetPerspectives().Count())
                {
                    menuCommand.Text = per.GetPerspectives().ElementAt(MRUItemIndex).Name;
                }
            }
        }

        private void Apply(object sender, EventArgs args)
        {
            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);

            var menuCommand = sender as OleMenuCommand;
            if (null != menuCommand)
            {
                int MRUItemIndex = menuCommand.CommandID.ID - 0x0109;
                if (MRUItemIndex >= 0 && MRUItemIndex < per.GetPerspectives().Count())
                {
                    per.GetPerspectives().ElementAt(MRUItemIndex).Apply();
                }
            }
        }

        private void SaveCurrent(object sender, EventArgs args)
        {
            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);

            per.Current.Apply();
        }

        private void SaveAsCurrent(object sender, EventArgs args)
        {
            var name = new PerspectiveName();
            var result = name.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                return;
            }

            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);

            per.AddNew(name.PerspectiveNameText);
        }

        #endregion

    }
}
