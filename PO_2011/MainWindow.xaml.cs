﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace UAM.PTO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BindCommands();
        }

        private void BindCommands()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (s,e) => Commands.OpenExecuted(image,e), Commands.CanOpenExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, (s,e) => Commands.SaveExecuted(image,e), (s,e) => Commands.CanSaveExecute(image,e)));
            this.CommandBindings.Add(new CommandBinding(Commands.Exit, Commands.ExitExecuted, Commands.CanExitExecute));
            this.CommandBindings.Add(new CommandBinding(Commands.Histogram, (s, e) => Commands.HistogramExecuted(image, e), (s, e) => Commands.CanHistogramExecute(image, e)));
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Commands.TryReplaceImageSource(image, ((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
            }
            e.Handled = true;
        }
    }
}
