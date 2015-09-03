using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace AdamDriscoll.Perspectives
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("af0132a0-b958-4962-b4d6-6d90d3362318")]
    public class PerspectivesToolWindow : ToolWindowPane
    {
        private PerspectivesControl _control;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public PerspectivesToolWindow() :
            base(null)
        {
            

            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 302;
            this.BitmapIndex = 0;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            _control = new PerspectivesControl();


            base.Content = _control;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _control.Loaded += new RoutedEventHandler(_control_Loaded);
        }

        void _control_Loaded(object sender, RoutedEventArgs e)
        {
            var dte = (DTE)GetService(typeof(EnvDTE.DTE));
            _control.Dte = dte;
            _control.RefreshUi();
        }

        public void SetPerspectives(IEnumerable<Perspective> perspectives)
        {
          _control.SetPerspectives(perspectives);
        }

        public void SetDte(DTE dte)
        {
            _control.Dte = dte;
        }

        public void RefreshUI()
        {
            _control.RefreshUi();
        }

    }
}
