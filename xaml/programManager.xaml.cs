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
using Deploytool.Object;
using Deploytool.func;
using Deploytool.lib;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace Deploytool.xaml
{
    /// <summary>
    /// uninstall.xaml 的交互逻辑
    /// </summary>
    public partial class programManager : Window
    {

        BinaryFormatter formatter = new BinaryFormatter();
        string file = "uninstall.dat";
        FILEUTILS fileutils = new FILEUTILS();
        public programManager()
        {
            InitializeComponent();
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                if (fs.Length > 0)
                {
                    var items = (ArrayList)formatter.Deserialize(fs);
                    foreach (var item in items)
                    {
                        lv_uninstalled.Items.Add(item);
                    }
                }
                fs.Close();
            }
            catch (Exception err) { }
            ProgramManager progMgr = new ProgramManager(fileutils);
            lv_installed.ItemsSource = progMgr.installeds();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var items = lv_installed.SelectedItems;
            foreach (INSTALLED item in items)
            {
                Console.WriteLine(item.name);
                if (!lv_uninstalled.Items.Contains(item))
                {
                    lv_uninstalled.Items.Add(item);
                }
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var items = lv_uninstalled.SelectedItems.Cast<INSTALLED>().ToArray();
            foreach (var item in items)
            {
                lv_uninstalled.Items.Remove(item);
            }
        }
        private void Window_Closing(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(file, FileMode.Create);
            formatter.Serialize(fs, new ArrayList(lv_uninstalled.Items));
            fs.Close();
        }
   }
}
