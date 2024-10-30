using Microsoft.Extensions.DependencyInjection;
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
using WPF_ManageCourt.ViewModel;

namespace WPF_ManageCourt.Controls
{
    /// <summary>
    /// Interaction logic for SideBarControl.xaml
    /// </summary>
    public partial class SidebarControl : UserControl
    {
        public SidebarControl()
        {
            InitializeComponent();
            var serviceProvider = App.ServiceProvider;
            DataContext = serviceProvider.GetService<SidebarViewModel>();
        }
    }
}
