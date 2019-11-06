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
using System.IO;
using Deploytool.Object;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.ComponentModel;

namespace Deploytool.xaml
{
    /// <summary>
    /// Promot.xaml 的交互逻辑
    /// </summary>
    public partial class Prompt : Window
    {
        FILEUTILS fileutils;
        private List<Prompt_lv_item> ItemsSource = new List<Prompt_lv_item>();//列表源数据
        private List<String> UnSelectItems = new List<String>();//保存未选Soft Name
        public string ListFile = "Prompt.dat";
        //background process initialize
        private BackgroundWorker m_BackgroundWorker;// 申明后台对象
        ManualResetEvent _busy = new ManualResetEvent(true);
        bool paused = false;

        public Prompt(FILEUTILS fileutils)
        {
            InitializeComponent();
            btn_pause.IsEnabled = false;
            this.fileutils = fileutils;
            //lb_summary.Content = "本软件目前运行与U盘,请勾选需复制到" + fileutils.target.Split('\\')[0] + "的项目;复制完成后本软件将会关闭,并打开" + fileutils.target.Split('\\')[0];
            lb_summary.Content = "本软件目前运行与U盘,请勾选需复制到" + fileutils.target + "的项目.";
            bool symbol = Install_Status.Check(fileutils, "VNC",true);
            if(symbol){
                Console.WriteLine("系统中已安装Office.");
                /*var toolTip = new ToolTip();
                toolTip.StaysOpen = true;
                toolTip.Content = "系统中已安装Office.";
                lv_Softs.ToolTip = toolTip;*/
            }
            var ItemsPath = fileutils.Traverse(fileutils.path + @"\softs").Skip(1);
            foreach (string path in ItemsPath)
            {
                Prompt_lv_item item=new Prompt_lv_item();
                item.Name=path.Split('\\').Last();
                item.Path = path;
                ItemsSource.Add(item);
            }
            var StoreItems=LoadList(ListFile);
            if(StoreItems!=null){
            foreach (Prompt_lv_item NewItem in ItemsSource)
            {
                foreach (Prompt_lv_item OldItem in StoreItems)
                {
                    if (NewItem.Name == OldItem.Name)
                    {
                        NewItem.Checked = OldItem.Checked;
                    }
                }
            }
            }
            lv_Softs.ItemsSource = ItemsSource;
            //background process initialize
            m_BackgroundWorker = new BackgroundWorker(); // 实例化后台对象
            m_BackgroundWorker.WorkerReportsProgress = true; // 设置可以通告进度
            m_BackgroundWorker.WorkerSupportsCancellation = true; // 设置可以取消
            m_BackgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
            m_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateProgress);
            m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompletedWork);
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
                lv_Softs.ItemsSource = ItemsSource.Select(x => { x.Checked = true; return x; });
            }
            else
            {
                lv_Softs.ItemsSource = ItemsSource.Select(x => {x.Checked = false;return x;});
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
            Prompt_lv_item Item=ItemsSource.Find(x=>x.Name==Name);
            if (cb.IsChecked == true)
            {
                Item.Checked = true;
            }
            else
            {
                Item.Checked = false;
            }
        }
    private void CopyFiles(object sender, EventArgs e){
            btn_confirm.IsEnabled = false;
            ProgressBar.Value = 0;
        m_BackgroundWorker.RunWorkerAsync();
        btn_pause.IsEnabled = true;
        }
    private void Close(object sender, EventArgs e)
    {
        m_BackgroundWorker.CancelAsync();
        Save_List(ListFile);
        this.Close();
    }
    private List<Prompt_lv_item> LoadList(string path)
    {
        List<Prompt_lv_item> list;
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            list = (List<Prompt_lv_item>)formatter.Deserialize(fs);
            fs.Close();
        }
        catch
        {
            list = null;
        }
        return list;
    }
private void Window_Closed(object sender, EventArgs e){
    Save_List(ListFile);
}
private void Save_List(string path)
{
                //progMgr.Close();
    BinaryFormatter formatter = new BinaryFormatter();
    FileStream fs = new FileStream(path, FileMode.Create);
            formatter.Serialize(fs, new List<Prompt_lv_item>(ItemsSource));
            fs.Close();
}
private void Pause(object sender, EventArgs e)
{
    // Start the worker if it isn't running
    if (paused == false)
    {
        _busy.Reset();
        paused = true;
        btn_pause.Content = "恢复";
    }
    else
    {
        // Unblock the worker 
        _busy.Set();
        paused = false;
        btn_pause.Content = "暂停";
    }
}
void UpdateProgress(object sender, ProgressChangedEventArgs e)
{
    int progress = e.ProgressPercentage;

    ProgressBar.Value = e.ProgressPercentage;
    //label10.Text = string.Format("{0}%", progress);
}
void CompletedWork(object sender, RunWorkerCompletedEventArgs e)
{
    if (e.Error != null)
    {
        System.Windows.Forms.MessageBox.Show("Error: " + e.Error);
    }
    else if (e.Cancelled)
    {
        System.Windows.Forms.MessageBox.Show("Canceled");
    }
    else
    {
        Console.WriteLine("文件复制完成.");
    }
    this.Close();
}
private void Cancel(object sender, EventArgs e)
{
    m_BackgroundWorker.CancelAsync();
}
void DoWork(object sender, DoWorkEventArgs e)
{
    BackgroundWorker bw = sender as BackgroundWorker;
    bw.ReportProgress(10, null);
    Console.WriteLine("文件复制中,请耐心等待...");
    UnSelectItems = ItemsSource.Where(x => x.Checked == false).Select(x => x.Name).ToList();
    IEnumerable<string> FilterDirs = fileutils.FilterFolder(fileutils.path + @"\softs", UnSelectItems);
    IEnumerable<string> FilterFiles = fileutils.GetFiles(fileutils.path + @"\softs", UnSelectItems, SearchOption.TopDirectoryOnly);
    foreach (string item in FilterFiles)
    {
        _busy.WaitOne();
        String ItemName = item.Split('\\').Last();
        String FullPath = fileutils.target + ItemName;
        String DestinationDir = FullPath.Split(new string[] { ".zip" }, StringSplitOptions.None)[0];
        System.IO.File.Copy(item, FullPath, true);
        if (ItemName.Contains(".zip"))
        {
            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(FullPath, DestinationDir);
                DestinationDir = DestinationDir+ @"\"+ItemName.Split(new string[] { ".zip" }, StringSplitOptions.None)[0];
            }
            catch
            {
                Directory.Delete(DestinationDir, true);
                System.IO.Compression.ZipFile.ExtractToDirectory(FullPath, DestinationDir);
                DestinationDir = DestinationDir  +@"\"+ ItemName.Split(new string[] { ".zip" }, StringSplitOptions.None)[0];
            }
            if (ItemName.Contains("关机"))
            {
                fileutils.ShutdownSoftFolderName = DestinationDir;
            }
            else if (ItemName.Contains("客户端"))
            {
                fileutils.PlusbeZKFolderName = DestinationDir;
            }
            else if (ItemName.IndexOf("Office", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                fileutils.OfficeFolderName = DestinationDir;
            }
                    else if (ItemName.Contains("zkplay")|| ItemName.Contains("客户端2"))
                    {
                        fileutils.ZkplayFolderName = DestinationDir;
                    }
                }
                if (bw.CancellationPending)
        {
            e.Cancel = true;
            break;
        }
        bw.ReportProgress(50, null);
        Console.WriteLine(item + "已复制并解压.");
    }
    foreach (string item in FilterDirs)
    {
        _busy.WaitOne();
        String ItemName = item.Split('\\').Last();
        String FullPath = fileutils.target + ItemName;
        fileutils.copyFolder(item, fileutils.target + ItemName);
        if (ItemName.Contains("开关机"))
        {
            fileutils.ShutdownSoftFolderName = FullPath;
        }
        else if (ItemName.Contains("客户端"))
        {
            fileutils.PlusbeZKFolderName = FullPath;
        }
        else if (ItemName.IndexOf("Office", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            fileutils.OfficeFolderName = FullPath;
        }
        if (bw.CancellationPending)
        {
            e.Cancel = true;
            break;
        }
        bw.ReportProgress(100, null);
        Console.WriteLine(item + "已复制.");
    }
    Save_List(ListFile);
}
    }
}
