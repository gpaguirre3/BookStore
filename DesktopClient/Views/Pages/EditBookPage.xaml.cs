﻿using DesktopClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace DesktopClient.Views.Pages
{
    /// <summary>
    /// Lógica de interacción para EditBookPage.xaml
    /// </summary>
    public partial class EditBookPage : INavigableView<ViewModels.EditBookViewModel>
    {
        public ViewModels.EditBookViewModel ViewModel { get; }

        public EditBookPage(ViewModels.EditBookViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
        private void authorSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ViewModel.AuthorSuggestionChosenCommand.Execute(args);
        }
    }
}
