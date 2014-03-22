using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows;
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
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
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

        internal DTE Dte { get; set; }

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

            var dte = (DTE)GetService(typeof(DTE));
            (window as PerspectivesToolWindow).SetDte(dte);

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

            Dte = (DTE)GetService(typeof(DTE));

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID saveCommandId = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidSave);
                OleMenuCommand saveMenuItem = new OleMenuCommand(SaveCurrent, saveCommandId);
                mcs.AddCommand(saveMenuItem);

                CommandID saveToolbarCommandId = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidSaveToolbar);
                OleMenuCommand saveToolbarItem = new OleMenuCommand(SaveCurrent, saveToolbarCommandId);
                mcs.AddCommand(saveToolbarItem);

                CommandID menuCommandID = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidSaveAs);
                OleMenuCommand menuItem = new OleMenuCommand(SaveAsCurrent, menuCommandID);
                mcs.AddCommand(menuItem);

                CommandID toolbarSaveAsCommandID = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidSaveAsToolbar);
                OleMenuCommand saveAsItem = new OleMenuCommand(SaveAsCurrent, toolbarSaveAsCommandID);
                mcs.AddCommand(saveAsItem);

                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidViewManager);
                OleMenuCommand menuToolWin = new OleMenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand( menuToolWin );

                CommandID toolBarId = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidToolbar);
                OleMenuCommand toolitem = new OleMenuCommand(SaveAsCurrent, toolBarId);
                mcs.AddCommand(toolitem);

                CommandID toolbarDropDown = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidFavoriteDropDownList);
                OleMenuCommand comboBox = new OleMenuCommand(ComboBoxChanged, toolbarDropDown);
                mcs.AddCommand(comboBox);

                CommandID toolbarDropDownItems = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidFavoriteDropDownListItems);
                OleMenuCommand comboBoxItems = new OleMenuCommand(ComboBoxPopulate, toolbarDropDownItems);
                mcs.AddCommand(comboBoxItems);

                CommandID fav1Id = new CommandID(GuidList.guidPerspectivesCmdSet, (int)PkgCmdIDList.cmdidApplyFav1);
                OleMenuCommand fav1Command = new OleMenuCommand(ApplyFav1, fav1Id);
                mcs.AddCommand(fav1Command);
            }
        }

        private const int baseToolbarId = (int)PkgCmdIDList.cmdidToolbar;
        private int _lastInt = 1;
        private int _lastToolbarId = 1;

        private void ApplyFav1(object sender, EventArgs args)
        {
            ApplyFavorite(1);
        }
        private void ApplyFav2(object sender, EventArgs args)
        {
            ApplyFavorite(2);
        }
        private void ApplyFav3(object sender, EventArgs args)
        {
            ApplyFavorite(3);
        }
        private void ApplyFav4(object sender, EventArgs args)
        {
            ApplyFavorite(4);
        }
        private void ApplyFav5(object sender, EventArgs args)
        {
            ApplyFavorite(5);
        }

        private void ApplyFavorite(int ordinal)
        {
            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);
            var fav = per.GetFavoritePerspectives().FirstOrDefault(m => m.FavoriteOrdinal == ordinal);

            fav.Apply();
        }
        
        private void ComboBoxPopulate(object sender, EventArgs args)
        {
            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);

            var comboValues = per.GetPerspectives().Select(m => m.Name).ToArray();
            var eventArgs = args as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                IntPtr output = eventArgs.OutValue;
                if (output != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(comboValues,
                        output);
                }
            }
        }

        private void ComboBoxChanged(object sender, EventArgs args)
        {
            var eventArgs = args as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                object input = eventArgs.InValue;
				IntPtr vOut = eventArgs.OutValue;

				var dte = (DTE)GetService(typeof(DTE));
				var per = new Perspective(dte);

				if (vOut != IntPtr.Zero && input != null)
				{
					throw (new ArgumentException(@"Both In Out Params Illegal"));
				}
				else if (vOut != IntPtr.Zero)
				{
					// From http://dotneteers.net/blogs/divedeeper/archive/2008/07/14/LearnVSXNowPart25.aspx
					// --- The IDE is requesting the current value for the combo
					Marshal.GetNativeVariantForObject(per.Current.Name, vOut);
				}
				else if (input != null)
				{
					var perspectiveName = input.ToString();

					per = per.GetPerspectives().FirstOrDefault(m => m.Name.Equals(perspectiveName, StringComparison.OrdinalIgnoreCase));

					if (per != null)
					{
						per.Apply();
					}
				}
            }
        }

        private void SaveCurrent(object sender, EventArgs args)
        {
            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);

            per.Current.Update();
        }

        private void RefreshPerspectivesManager()
        {
            ToolWindowPane window = this.FindToolWindow(typeof(PerspectivesToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            (window as PerspectivesToolWindow).RefreshUI();
        }

        private void SaveAsCurrent(object sender, EventArgs args)
        {
            var response = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for your perspective.", "Enter name");

            if (String.IsNullOrWhiteSpace(response))
            {
                return;
            }

            var dte = (DTE)GetService(typeof(DTE));
            var per = new Perspective(dte);
            try
            {
                per.AddNew(response);    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error adding new perspective", MessageBoxButton.OK, MessageBoxImage.Error);
                SaveAsCurrent(sender, args);
            }

            RefreshPerspectivesManager();
        }

        #endregion

    }
}
