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
using Deploytool.lib;

namespace Deploytool.xaml
{
    /// <summary>
    /// Promot.xaml 的交互逻辑
    /// </summary>
    public partial class Prompt : Window
    {
        FILEUTILS fileutils;
        private List<string> selectSoft = new List<string>();//保存多选Soft Name
        private List<string> Softs = new List<string>();//列表源数据

        public Prompt(FILEUTILS fileutils)
        {
            InitializeComponent();
            this.fileutils = fileutils;
            lb_summary.Content = "本软件目前运行与U盘,请勾选需复制到" + fileutils.target.Split('\\')[0] + "的项目.";
            Softs = fileutils.Traverse(fileutils.path + @"\softs").Select(x => x.Split('\\').Last()).ToList();
            Softs.Remove("softs");
            //Console.WriteLine(Softs[1]);
            lv_Softs.ItemsSource = Softs;
        }
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Select_All(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                selectSoft = Softs;
            }
            else
            {
                selectSoft.Clear();
            }
        }
        /// 由ChecBox的Click事件来记录被选中行的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            string Name = cb.Tag.ToString(); 
            if (cb.IsChecked == true)
            {
                selectSoft.Add(Name);  //如果选中就保存id
            }
            else
            {
                selectSoft.Remove(Name);   //如果选中取消就删除里面的id
            }
        }
    private void CopyFiles(object sender, EventArgs e){
        fileutils.copyFolder(fileutils.path, fileutils.target);
        foreach (string soft in selectSoft)
        {
                if (soft.Contains(".exe"))
                {
                    System.IO.File.Copy(fileutils.path + @"\softs\" + soft, fileutils.target.Split('\\')[0] + '\\' + soft);
                }
                else
                {
                    fileutils.copyFolder(fileutils.path + @"\softs\" + soft, fileutils.target.Split('\\')[0] + '\\' + soft);
                }
            }
        this.Close();
        }

    }
}
