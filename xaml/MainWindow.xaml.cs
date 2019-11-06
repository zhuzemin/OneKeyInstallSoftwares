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
using Deploytool.func;
using Deploytool.lib;
using Deploytool.Object;
using System.IO;
using System.Text.RegularExpressions;
using System.Management;
using Deploytool.xaml;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Deploytool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
            //adapter relate initialize
            ADAPTER adapter = new ADAPTER();
            int AdapterIndex = 0;
            [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
            private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
            [DllImport("User32.dll", EntryPoint = "SendMessage")]
            private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
            
            //flashdisk relate initialize
            FILEUTILS fileutils = new FILEUTILS();
            DriveInfo flashdisk; 
            BinaryFormatter formatter = new BinaryFormatter();
            string uninstall = "uninstall.dat";
            string dellnk = "dellnk.dat";

            //ProgramManager initialize
            ProgramManager progMgr;
            public ObservableCollection<INSTALLED> INSTALLEDS { get; set; }
            public ObservableCollection<SHORTCUT> DESKLNKS { get; set; }
            public ObservableCollection<SHORTCUT> STARTMENUS { get; set; }

            //config store initialize
            CONFIG cfg = new CONFIG();

            //input file initialize
            System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();
            List<string> InputFile;
            IPDISTRIBUTION ipdist;

            //background process initialize
            private BackgroundWorker m_BackgroundWorker;// 申明后台对象
            ManualResetEvent _busy = new ManualResetEvent(true);
            bool paused = false;

            //main process
            public MainWindow()
            {
                //ProgramManager initialize
                InitializeComponent();
                ((INotifyCollectionChanged)lv_uninstalled.Items).CollectionChanged += uninstalled_CollectionChanged;
                ((INotifyCollectionChanged)lv_dellnk.Items).CollectionChanged += dellnk_CollectionChanged;
                ((INotifyCollectionChanged)lv_delstartmenu.Items).CollectionChanged += delstartmenu_CollectionChanged;

                //background process initialize
                m_BackgroundWorker = new BackgroundWorker(); // 实例化后台对象
                m_BackgroundWorker.WorkerReportsProgress = true; // 设置可以通告进度
                m_BackgroundWorker.WorkerSupportsCancellation = true; // 设置可以取消
                m_BackgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
                m_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateProgress);
                m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompletedWork);

                //ConsoleManager initialize
                ConsoleManager.Toggle();

                //ProgramManager initialize
                //ShortcutManager() must init before programManager()
                shortcutManager();
                programManager();


                //config store initialize
                tb_ip.Text = cfg.tb_ip;
                /*TreeViewItem item = (TreeViewItem)treeview_IP.SelectedItem;
                IPDISTRIBUTION device=(IPDISTRIBUTION) item.Tag;
                tb_ip.Text = device.ip;*/
                tb_gateway.Text = cfg.tb_gateway;
                cb_waken.IsChecked = cfg.cb_waken;
                cb_firewall.IsChecked = cfg.cb_firewall;
                cb_ip.IsChecked = cfg.cb_ip;
                cb_gateway.IsChecked = cfg.cb_gateway;
                cb_swsoft_startup.IsChecked = cfg.cb_swsoft_startup;
                cb_swsoft_privilege.IsChecked = cfg.cb_swsoft_privilege;
                cb_vnc.IsChecked = cfg.cb_vnc;
                cb_klite.IsChecked = cfg.cb_klite;
                cb_office.IsChecked = cfg.cb_office;
                cb_power.IsChecked = cfg.cb_power;
                cb_zoom.IsChecked = cfg.cb_zoom;

                //flashdisk relate initialize
                //total path of self;
                string path = System.Reflection.Assembly.GetEntryAssembly().Location;
                string[] array = path.Split(new[] { @"\" }, StringSplitOptions.None);
                string name = array.Last();
                string folder = path.Replace(name, "");
                string currentdisk = array[0];
                currentdisk = Regex.Match(currentdisk, @"\w").Groups[0].Value;
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    string disk = Regex.Match(d.Name, @"\w").Groups[0].Value;
                    if (disk == currentdisk)
                    {
                       if (d.DriveType.ToString() == "Removable")
                        {
                            //Copy file prompt
                            Prompt prompt = new Prompt(fileutils);
                            prompt.ShowDialog();
                            //System.Windows.Forms.MessageBox.Show("本软件目前运行与U盘,是否复制到" + fileutils.target + ",并打开文件夹?");
                            if (System.Windows.Forms.MessageBox.Show("Yes or no", "本软件目前运行与U盘,是否复制到" + fileutils.target + ",并打开文件夹?",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                            {
                                fileutils.copyFolder(folder, fileutils.target);
                            }// opens the folder in explorer
                            System.Diagnostics.Process.Start(fileutils.target);
                            System.Environment.Exit(1);
                        }
                        else
                        {
                            string drive = System.IO.Path.GetPathRoot(fileutils.path);
                            if (System.Windows.Forms.MessageBox.Show("Yes or no", "复制\"开关机软件\"/\"中控客户端\"到" + drive,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                            {
                                fileutils.copyFolder(fileutils.path + @"\softs", drive);
                            }
                        }
                    }
                        if (d.DriveType.ToString() == "Removable")
                        {
                            flashdisk = d;
                            string keyword = "*IP分配*.txt";
                            string[] files = fileutils.searchFile(flashdisk.Name + @"部署工具", keyword);
                            string file = files.OrderByDescending(pathx => File.GetLastWriteTime(pathx)).FirstOrDefault();
                            tb_ipfile.Text = file;
                            tb_save.Text = tb_ipfile.Text;
                            IPDistribution();
                        }

                    }
                }


        private void Button_Click_1(object sender, RoutedEventArgs e)
{
    Console.WriteLine("开始配置...");
    CB_STATE cb_state = new CB_STATE();
    if ((bool)cb_ip.IsChecked)
    {
        string mask = "255.255.255.0";
        adapter.setIP(tb_ip.Text, mask);
        cfg.tb_ip = tb_ip.Text;
        Console.WriteLine("IP设置完成.");
        string path;
        if (flashdisk == null)
        {
            path = Regex.Match(tb_save.Text, @"\w").Groups[0].Value;
        }
        else
        {
            path = flashdisk.Name;
        }
        string origin = File.ReadAllText(tb_ipfile.Text, System.Text.Encoding.Default);
        //string origin = File.ReadAllText(tb_ipfile.Text, Encoding.UTF8);
        Regex rgx = new Regex(tb_ip.Text);
        string result = rgx.Replace(origin, tb_ip.Text + "," + adapter.mac);
        using(StreamWriter sw = new StreamWriter(
    new FileStream(tb_ipfile.Text, FileMode.Open, FileAccess.ReadWrite),
    Encoding.UTF8
))
        //using (StreamWriter sw = File.CreateText(tb_ipfile.Text))
        {
            sw.WriteLine(result);
        }

        //fileutils.append(tb_ip.Text + "----" + adapter.mac, path + @"部署工具\IP分配文件.txt");
        if (flashdisk != null)
        {
            REMOVEDRIVE rmdrive = new REMOVEDRIVE(fileutils);
            //rmdrive.eject(flashdisk.Name);
            Console.WriteLine("IP和MAC已记录在文件: "+path+". 可拔出U盘,部署将继续运行.");
        }
    }
    if ((bool)cb_gateway.IsChecked)
    {
        adapter.setGateway(tb_gateway.Text);
        Console.WriteLine("网关设置完成.");
    }
    if ((bool)cb_waken.IsChecked)
    {
        //Console.WriteLine(adapter.index.PadLeft(4, '0'));
        adapter.allowAwaken();
        Console.WriteLine("\"运行网卡唤醒\"启用.");
    }
    if ((bool)cb_firewall.IsChecked)
    {
        firewall.disable();
        Console.WriteLine("防火墙关闭.");
    }
    if ((bool)cb_zoom.IsChecked)
    {
        textZoom.DEFAULT();
        Console.WriteLine("桌面缩放关闭.");
    }
    if ((bool)cb_swsoft_startup.IsChecked)
    {
        string drive = System.IO.Path.GetPathRoot(fileutils.path);
        switchSoft sw = new switchSoft(fileutils);
        string[] array = System.IO.Path.GetDirectoryName(sw.version).Split(new[] { @"\" }, StringSplitOptions.None);
        sw.version = drive+array.Last()+@"\"+sw.softName;
        sw.addStartup();
        Console.WriteLine("\"开关机软件\"自启设置完成.");
        sw.runAsAdmin();
        Console.WriteLine("\"开关机软件\"管理员权限设置完成.");
        //winupdate.enable();
    }
    if ((bool)cb_fusion.IsChecked)
    {
        string source = @"";
        string target = @"";
        //fileutils.copyFolder();
    }
    if ((bool)cb_vnc.IsChecked)
    {
        cb_state.vnc = true;
    }
    if ((bool)cb_klite.IsChecked)
    {
        cb_state.klite = true;
    }
    if ((bool)cb_power.IsChecked)
    {
        power.alwaysOn();
        Console.WriteLine("电源常开设置完成.");
    }
    if (lv_uninstalled.Items != null)
    {
        cb_state.uninstall = lv_uninstalled.Items;
    }
    if ((bool)cb_office.IsChecked)
    {
        cb_state.office = true;
    }
    if (lv_dellnk.Items != null)
    {
        foreach (SHORTCUT item in lv_dellnk.Items)
        {
            File.Delete(item.path);
            lv_dellnk.Items.Remove(item);
        }
    }
    if (lv_delstartmenu.Items != null)
    {
        foreach (SHORTCUT item in lv_delstartmenu.Items)
        {
            File.Delete(item.path);
            lv_delstartmenu.Items.Remove(item);
        }
    }
            m_BackgroundWorker.RunWorkerAsync(cb_state);

             //cmd.ExecuteCommand("shutdown /r");
}
        private void progMgr_add(object sender, RoutedEventArgs e)
        {
            var items = lv_installed.SelectedItems.Cast<INSTALLED>().ToArray();
            foreach (INSTALLED item in items)
            {
                if (!lv_uninstalled.Items.Contains(item))
                {
                    lv_uninstalled.Items.Add(item);
                //INSTALLEDS.Remove((INSTALLED)item);
                }
            }

        }
        private void progMgr_del(object sender, RoutedEventArgs e)
        {
            var items = lv_uninstalled.SelectedItems.Cast<INSTALLED>().ToArray();
            foreach (var item in items)
            {
                lv_uninstalled.Items.Remove(item);
                //INSTALLEDS.Add((INSTALLED)item);
            }
        }
        private void desklnk_add(object sender, RoutedEventArgs e)
        {
            var items = lv_alllnk.SelectedItems.Cast<SHORTCUT>().ToArray();
            foreach (SHORTCUT item in items)
            {
                if (!lv_dellnk.Items.Contains(item))
                {
                    lv_dellnk.Items.Add(item);
                    //INSTALLEDS.Remove((INSTALLED)item);
                }
            }

        }
        private void desklnk_del(object sender, RoutedEventArgs e)
        {
            var items = lv_dellnk.SelectedItems.Cast<SHORTCUT>().ToArray();
            foreach (var item in items)
            {
                lv_dellnk.Items.Remove(item);
                //INSTALLEDS.Add((INSTALLED)item);
            }
        }
        private void startmenu_add(object sender, RoutedEventArgs e)
        {
            var items = lv_allstartmenu.SelectedItems.Cast<SHORTCUT>().ToArray();
            foreach (SHORTCUT item in items)
            {
                if (!lv_delstartmenu.Items.Contains(item))
                {
                    lv_delstartmenu.Items.Add(item);
                    //INSTALLEDS.Remove((INSTALLED)item);
                }
            }

        }
        private void startmenu_del(object sender, RoutedEventArgs e)
        {
            var items = lv_delstartmenu.SelectedItems.Cast<SHORTCUT>().ToArray();
            foreach (var item in items)
            {
                lv_delstartmenu.Items.Remove(item);
                //INSTALLEDS.Add((INSTALLED)item);
            }
        }
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();

            foreach (ManagementObject oReturn in adapter.interfaceCollection)
            {
                if (oReturn.Properties["NetConnectionID"].Value != null)
                {
                    string interfaceName = oReturn.Properties["NetConnectionID"].Value.ToString();
                    data.Add(interfaceName);
                }
            }

            // ... Get the ComboBox reference.
            var comboBox = sender as System.Windows.Controls.ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = AdapterIndex;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // ... Get the ComboBox.
            var comboBox = sender as System.Windows.Controls.ComboBox;
            AdapterIndex = comboBox.SelectedIndex;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            foreach (ManagementObject oReturn in adapter.interfaceCollection)
            {
                if (oReturn.Properties["NetConnectionID"].Value != null && oReturn.Properties["NetConnectionID"].Value.Equals(value))
                {
                    if (oReturn["MACAddress"] != null)
                    {
                        foreach (PropertyData prop in oReturn.Properties)
                        {
                            if (prop.Value != null)
                            {
                                //MessageBox.Show(prop.Name+" : "+ prop.Value);
                            }
                        }
                    }
                    adapter.interfaceName = value;
                    adapter.device = oReturn["Name"].ToString();
                    adapter.mac = oReturn["MACAddress"].ToString();
                    adapter.index = oReturn["Index"].ToString();
                    adapter.guid = oReturn["GUID"].ToString();
                    break;
                }
            }
            tb_device.Text = adapter.device;
            tb_mac.Text = adapter.mac;
        }
        private void Window_Closed(object sender, EventArgs e)
        {

            cfg.cb_waken = (bool)cb_waken.IsChecked;
            cfg.cb_firewall = (bool)cb_firewall.IsChecked;
            cfg.cb_ip = (bool)cb_ip.IsChecked;
            cfg.cb_gateway = (bool)cb_gateway.IsChecked;
            cfg.cb_swsoft_startup = (bool)cb_swsoft_startup.IsChecked;
            cfg.cb_swsoft_privilege = (bool)cb_swsoft_privilege.IsChecked;
            cfg.cb_vnc = (bool)cb_vnc.IsChecked;
            cfg.cb_klite = (bool)cb_klite.IsChecked;
            cfg.cb_office = (bool)cb_office.IsChecked;
            cfg.cb_power = (bool)cb_power.IsChecked;
            cfg.cb_zoom = (bool)cb_zoom.IsChecked;
            cfg.save();
            //progMgr.Close();
            FileStream fs = new FileStream(uninstall, FileMode.Create);
            formatter.Serialize(fs, new List<INSTALLED>(lv_uninstalled.Items.Cast<INSTALLED>().ToArray()));
            fs.Close();
            if (flashdisk != null)
            {
                System.IO.File.Copy(cfg.file, flashdisk + "部署工具/config.ini", true);
            }
        }



        
        void programManager()
        {
            try
            {
                FileStream fs = new FileStream(uninstall, FileMode.Open);
                var items = (List<INSTALLED>)formatter.Deserialize(fs);
                fs.Close();
                    progMgr = new ProgramManager(fileutils);
                List<INSTALLED> Result1 = progMgr.ListDistinct(progMgr.installeds());
                Result1 = progMgr.ListCompare(Result1,items);
                INSTALLEDS = new ObservableCollection<INSTALLED>(Result1);
                //lv_uninstalled.ItemsSource = items;
                foreach (var item in items)
                    {
                        lv_uninstalled.Items.Add(item);
                        //INSTALLEDS.Remove((INSTALLED)item);
                    }
                lv_installed.ItemsSource=INSTALLEDS;
            }
            catch (Exception err) { }
        }
    void shortcutManager()
    {
        try
            {
                ShortcutManager lnkmgr = new ShortcutManager(fileutils);
                DESKLNKS = new ObservableCollection<SHORTCUT>(lnkmgr.GetDesktopShortcut());
                lv_alllnk.ItemsSource = DESKLNKS;
                STARTMENUS = new ObservableCollection<SHORTCUT>(lnkmgr.GetStartMenu());
                lv_allstartmenu.ItemsSource = STARTMENUS;
        }
            
        catch (Exception err) { }
    }
    private void uninstalled_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        int limit = 6;
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // scroll the new item into view   
            //listView.ScrollIntoView(e.NewItems[0]);
            //Console.WriteLine(((INSTALLED)e.NewItems[0]).name);
            INSTALLED lv_uninstalled_item = ((INSTALLED)e.NewItems[0]);
                    INSTALLEDS.Remove(lv_uninstalled_item);
            foreach (SHORTCUT item in lv_alllnk.Items)
            {

                int diff = LevenshteinDistance.Compute(item.name, lv_uninstalled_item.name);
                if (diff <= limit)
                {
                    lv_dellnk.Items.Add(item);
                    break;
                }
            }
            foreach (SHORTCUT item in lv_allstartmenu.Items)
            {

                int diff = LevenshteinDistance.Compute(item.name, lv_uninstalled_item.name);
                if (diff <= limit)
                {
                    lv_delstartmenu.Items.Add(item);
                    break;
                }
            }

        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {

            INSTALLED lv_uninstalled_item = ((INSTALLED)e.OldItems[0]);
                    INSTALLEDS.Add(lv_uninstalled_item);
            foreach (SHORTCUT item in lv_dellnk.Items)
            {
                int diff = LevenshteinDistance.Compute(item.name, lv_uninstalled_item.name);
                if (diff < limit)
                {
                    lv_dellnk.Items.Remove(item);
                    break;
                }
            }
            foreach (SHORTCUT item in lv_delstartmenu.Items)
            {
                int diff = LevenshteinDistance.Compute(item.name, lv_uninstalled_item.name);
                if (diff < limit)
                {
                    lv_delstartmenu.Items.Remove(item);
                    break;
                }
            }
        }
    }
    private void dellnk_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        int limit = 10;
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // scroll the new item into view   
            //listView.ScrollIntoView(e.NewItems[0]);
            //Console.WriteLine(((INSTALLED)e.NewItems[0]).name);
            SHORTCUT lv_dellnk_item = ((SHORTCUT)e.NewItems[0]);
            DESKLNKS.Remove(lv_dellnk_item);

        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {

            SHORTCUT lv_dellnk_item = ((SHORTCUT)e.OldItems[0]);
            DESKLNKS.Add(lv_dellnk_item);
        }
    }
    private void delstartmenu_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        int limit = 10;
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // scroll the new item into view   
            //listView.ScrollIntoView(e.NewItems[0]);
            //Console.WriteLine(((INSTALLED)e.NewItems[0]).name);
            SHORTCUT lv_delstartmenu_item = ((SHORTCUT)e.NewItems[0]);
            STARTMENUS.Remove(lv_delstartmenu_item);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {

            SHORTCUT lv_uninstalled_item = ((SHORTCUT)e.OldItems[0]);
            STARTMENUS.Add(lv_uninstalled_item);
        }
    }
    void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (ti_main.IsSelected)
        {
            //do work when tab is changed
            System.Windows.Application.Current.MainWindow.Width = 850;
        }
        else if (ti_uninstall.IsSelected)
        {
            System.Windows.Application.Current.MainWindow.Width = 1150;
        }

    }
    private void Button_Click_4(object sender, EventArgs e)
    {
        //设定Filter，过滤档案
        openfiledialog.Filter = "text file (*.txt)|*.txt|逗号分隔文件(*.csv)|*.csv|All files (*.*)|*.*";

        //设定起始目录为程式目录
        //OpenFileDialog.InitialDirectory = Application.StartupPath;
        openfiledialog.RestoreDirectory = true;
        //设定dialog的Title
        openfiledialog.Title = "选择IP分配方案";
        //假如使用者按下OK钮，则将档案名称显示于TextBox1上
        if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            tb_ipfile.Text = openfiledialog.FileName;
            tb_save.Text = openfiledialog.FileName;
            IPDistribution();
             
        }

    }
    private void SelectionChanged(object sender, EventArgs e)
    {
        TreeViewItem item = (TreeViewItem)treeview_ip.SelectedItem;
        ipdist = (IPDISTRIBUTION)item.Tag;
        tb_ip.Text = ipdist.ip;
    }
    void IPDistribution()
    {
        treeview_ip.Items.Clear();
        InputFile = new List<string>(File.ReadAllLines(tb_ipfile.Text, System.Text.Encoding.Default));
        foreach (var line in InputFile)
        {
            ipdist = new IPDISTRIBUTION(line);
            TreeViewItem item = new TreeViewItem();
            item.Header = ipdist.device+" - "+ipdist.ip+" - "+ipdist.mac;
            item.Tag = ipdist;
            /*item.FontWeight = FontWeights.Normal;
            item.Items.Add(dummyNode);
            item.Expanded += new RoutedEventHandler(folder_Expanded);*/
            treeview_ip.Items.Add(item);
        }
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

        //progressBar1.Value = e.ProgressPercentage;
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
           Console.WriteLine("所有部署完成.");
           System.Windows.Forms.MessageBox.Show("Completed");
        }
    }
    void DoWork(object sender, DoWorkEventArgs e) {
        CB_STATE cb_state = (CB_STATE)e.Argument;
        if (cb_state.vnc)
        {
            VNC vnc = new VNC(fileutils);
            Console.WriteLine("VNC安装中...");
            vnc.install();
            Console.WriteLine("VNC安装完成.");
        }
        if (cb_state.klite)
        {
            KLITE klite = new KLITE(fileutils);
            Console.WriteLine("K-Lite安装中...");
            klite.install();
            Console.WriteLine("K-Lite安装完成.");
        }
        if (cb_state.office)
        {
            OFFICE office = new OFFICE(fileutils);
            Console.WriteLine("MS Office安装中...");
            office.install();
            Console.WriteLine("MS Office安装完成.");
        }
        if (cb_state.uninstall != null)
        {
            foreach (INSTALLED item in lv_uninstalled.Items)
            {
                Console.WriteLine(item.name+"卸载中...");
                progMgr.uninstall(item);
                //lv_uninstalled.Items.Remove(item);
                Console.WriteLine(item.name + "卸载完成.");
            }
        }
    }
    private void Cancel(object sender, EventArgs e)
    {
        m_BackgroundWorker.CancelAsync();
    }
    private void adapter_attribute(object sender, RoutedEventArgs e)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(@"::{208D2C60-3AEA-1069-A2D7-08002B30309D}\::{7007ACC7-3202-11D1-AAD2-00805FC1270E}\::" + adapter.guid);
        startInfo.UseShellExecute = true;
        Process.Start(startInfo);
        Thread.Sleep(3000);
        IntPtr ArrtibuteHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, adapter.interfaceName+" 属性");
        const int BM_CLICK = 0xF5;
        if (ArrtibuteHwnd != IntPtr.Zero)
        {
            //Console.WriteLine("找到窗口");
            IntPtr TabHwnd = FindWindowEx(ArrtibuteHwnd, IntPtr.Zero, null, "网络");   //获得按钮的句柄  
            if (TabHwnd != IntPtr.Zero)
            {
                //Console.WriteLine("找到子窗口");
                IntPtr btnHwnd = FindWindowEx(TabHwnd, IntPtr.Zero, null, "配置(&C)...");   //获得按钮的句柄  
                if (btnHwnd != IntPtr.Zero)
                {
                    //Console.WriteLine("找到按钮");
                    SendMessage(btnHwnd, BM_CLICK, IntPtr.Zero, null);
                }
            }
        }
    }
    }
}
