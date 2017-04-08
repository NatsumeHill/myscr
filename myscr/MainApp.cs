using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace myscr
{
    class MainApp: System.Windows.Application
    {
        public void InitializeComponent(int entry)
        {

            #line 5 "..\..\App.xaml"
            if(entry == 1)
                this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);
            else if(entry ==2)
                this.StartupUri = new System.Uri("Setting.xaml", System.UriKind.Relative);
            #line default
            #line hidden
        }

        [STAThread]
        public static void Main(string[] args)
        {
            MainApp app = new MainApp();
            if (args.Length!=0)
            {
                if (args[0].Substring(0, 2).Equals("/c"))
                {
                    app.InitializeComponent(2);
                }
                if(args[0].Substring(0, 2).Equals("/s"))
                    app.InitializeComponent(1);
            }
            else
            {
                app.InitializeComponent(1);

            }
            app.Run();
        }
    }
}
