﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuigisMansionRoomViewer.src.ViewModel;
using Microsoft.Win32;
using BinModel.src.Model;
using System.IO;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace LuigisMansionRoomViewer.src
{
    class RoomViewerViewModel : BaseMainWindowViewModel
    {
        List<Model> OpenModels;

        public RoomViewerViewModel()
        {
            OpenModels = new List<Model>();
            Application.Current.MainWindow.Closing += OnMainWindowClosing;
        }

        public override void Open()
        {
            OpenFileDialog openFile = new OpenFileDialog();

            if ((bool)openFile.ShowDialog())
            {
                string fileName = openFile.FileName;
                WindowTitle = fileName;

                switch (Path.GetExtension(fileName))
                {
                    case ".arc":
                        break;
                    case ".bin":
                        OpenModels.Add(OpenBin(fileName));
                        break;
                }
            }
        }

        private Model OpenBin(string fileName)
        {
            return new Model(fileName);
        }

        #region Clean-up before closing
        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            foreach (Model mod in OpenModels)
                mod.Dispose();
        }
        #endregion
    }
}
