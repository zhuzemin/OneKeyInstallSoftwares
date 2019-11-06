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
using Microsoft.Win32;
using log4net;
using PlusbeHard;

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
            int PlusebeZKLoopModeIndex = 5;
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
            string unstartup = "unstartup.dat";

            //ProgramManager initialize
            ProgramManager progMgr;
            public ObservableCollection<INSTALLED> INSTALLEDS { get; set; }
            public ObservableCollection<SHORTCUT> DESKLNKS { get; set; }
            public ObservableCollection<SHORTCUT> STARTMENUS { get; set; }
            public ObservableCollection<SHORTCUT> STARTUPS { get; set; }

            StartupManager StartupMgr;
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

            //resolution
            string screenWidth;
            string screenHeight;



        //main process
        public MainWindow(string[] args)
            {
                AppDomain currentDomain = default(AppDomain);
                currentDomain = AppDomain.CurrentDomain;
                // Handler for unhandled exceptions.
                currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
                // Handler for exceptions in threads behind forms.
                System.Windows.Forms.Application.ThreadException += GlobalThreadExceptionHandler;

                
                //ProgramManager initialize
                InitializeComponent();
                ti_shortcut.Visibility = Visibility.Hidden;
                ti_startmenu.Visibility = Visibility.Hidden;
                ((INotifyCollectionChanged)lv_uninstalled.Items).CollectionChanged += uninstalled_CollectionChanged;
                ((INotifyCollectionChanged)lv_dellnk.Items).CollectionChanged += dellnk_CollectionChanged;
                ((INotifyCollectionChanged)lv_delstartmenu.Items).CollectionChanged += delstartmenu_CollectionChanged;
                ((INotifyCollectionChanged)lv_del_startup.Items).CollectionChanged += unstartup_CollectionChanged;

                btn_pause.IsEnabled = false;
                //cb_dns.Visibility = Visibility.Hidden;
                //tb_dns.Visibility = Visibility.Hidden;
                //tb_mask.Visibility = Visibility.Hidden;
                //lb_mask.Visibility = Visibility.Hidden;

                //background process initialize
                m_BackgroundWorker = new BackgroundWorker(); // 实例化后台对象
                m_BackgroundWorker.WorkerReportsProgress = true; // 设置可以通告进度
                m_BackgroundWorker.WorkerSupportsCancellation = true; // 设置可以取消
                m_BackgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
                m_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateProgress);
                m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompletedWork);

                //ConsoleManager initialize
                ConsoleManager.Toggle();
            //SmartPss smartpss = new SmartPss();
            //smartpss.confirm();
            Hardware_Info.DeviceDetail();
                screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
                screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
                Console.WriteLine("分辨率: "+screenWidth+"x"+screenHeight);
            WinActivation winact = new WinActivation(fileutils);
            if (!winact.IsWindowsActivated())
            {
                Console.WriteLine("================Windows未激活!=================");
                cb_activation.IsChecked = true;
            }



            //ProgramManager initialize
            //ShortcutManager() must init before programManager()
            shortcutManager();
                programManager();
                startupManager();

                //config store initialize
                tb_ip.Text = cfg.tb_ip;
                /*TreeViewItem item = (TreeViewItem)treeview_IP.SelectedItem;
                IPDISTRIBUTION device=(IPDISTRIBUTION) item.Tag;
                tb_ip.Text = device.ip;*/
                tb_vnc_key.Text=cfg.tb_vnc_key;
                tb_vnc_pwd.Text=cfg.tb_vnc_pwd;
                tb_gateway.Text = cfg.tb_gateway;
                cb_waken.IsChecked = cfg.cb_waken;
                cb_firewall.IsChecked = cfg.cb_firewall;
                cb_ip.IsChecked = cfg.cb_ip;
                cb_gateway.IsChecked = cfg.cb_gateway;
                cb_vnc.IsChecked = cfg.cb_vnc;
                cb_klite.IsChecked = cfg.cb_klite;
                cb_power.IsChecked = cfg.cb_power;
                cb_zoom.IsChecked = cfg.cb_zoom;
                tb_PlusbeZK_SN.Text = HardJM.GetSN();
                tb_PlusbeZK_key.Text = HardJM.GetKey();
                cb_PowerButton.IsChecked = cfg.cb_PowerButton;
                tb_dns.Text = cfg.tb_dns;
                cb_dns.IsChecked = cfg.cb_dns;
                tb_mask.Text=cfg.tb_mask;
                tb_title.Text = cfg.tb_title;
                cb_chrome.IsChecked = cfg.cb_chrome;
                tb_DefaltShare.Text = Environment.SystemDirectory;
                cb_DefaltShare.IsChecked = cfg.cb_DefaltShare;
                cb_ForceGuest.IsChecked = cfg.cb_ForceGuest;
                cb_LimitBlankPasswordUse.IsChecked = cfg.cb_LimitBlankPasswordUse;
                cb_UacRemoteRestric.IsChecked = cfg.cb_UacRemoteRestric;
                tb_userName.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                if (DetectSystem.WindowsVersion().Contains("Windows 10"))
                {
                    cb_EnableSMB1.IsChecked = true;
                }
                cb_del_2345.IsChecked = cfg.cb_del_2345;
                tb_PlusebeZK_Content_Server.Text = cfg.tb_Content_Server;
                cb_flash.IsChecked = cfg.cb_flash;
            tb_screenWidth.Text = screenWidth;
            tb_screenHeight.Text = screenHeight;
            cb_wuauserv.IsChecked = cfg.cb_wuauserv;
            cb_rdp.IsChecked = cfg.cb_rdp;
            /*
            bool authorize = true;
            if(args.Length>=1){
                if (args[0] == "admin")
                {
                    authorize = true;
                }
            }
            if(license.check()==0){
                authorize=true;
            }
            if(authorize){
                    cb_PlusebeZK_reg.IsEnabled = true;
                    cb_PlusebeZK_reg.IsChecked = cfg.cb_PlusebeZK_reg;
                    tb_PlusbeZK_key.Text = HardJM.GetKey();
                }
            */
            //tb_vnc_key.Text = "BR43B-6H233-X43PN-Z6RS7-M5ZTA";
            //tb_vnc_pwd.Text = "baidu.com";

            //flashdisk relate initialize
            //total path of self;
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
                string[] array = path.Split(new[] { @"\" }, StringSplitOptions.None);
                string name = array.Last();
                string folder = path.Replace(name, "");
                string currentdisk = array[0];
                currentdisk = Regex.Match(currentdisk, @"\w").Groups[0].Value;
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                Boolean inremovable = false;
                foreach (DriveInfo d in allDrives)
                {
                    string disk = Regex.Match(d.Name, @"\w").Groups[0].Value;
                    if (disk == currentdisk && d.DriveType.ToString() == "Removable")
                    {
                        inremovable = true;
                    }
                    else
                    {
                        if (allDrives.Length == 2)
                        {
                            fileutils.target = "C:\\";
                        }
                        else
                        {
                            if (d.DriveType.ToString() == "Fixed" && disk != "C" && disk != currentdisk)
                            {
                                fileutils.target = disk + ":\\";
                                //fileutils.target =  "D:\\";
                            }
                        }
                    }
                    /*inremovable = true;
                    if (d.DriveType.ToString() == "Removable")
                    {
                        flashdisk = d;
                        string keyword = "*IP分配*.txt";
                        string[] files = fileutils.searchFile(flashdisk.Name + @"部署工具", keyword);
                        string file = files.OrderByDescending(pathx => File.GetLastWriteTime(pathx)).FirstOrDefault();
                        tb_ipfile.Text = file;
                        tb_save.Text = tb_ipfile.Text;
                        IPDistribution();
                    }*/

                }
                //inremovable = true;
                if (inremovable)
                {
                    //Copy file prompt
                    Prompt prompt = new Prompt(fileutils);
                    prompt.ShowDialog();
                    /*System.Windows.Forms.MessageBox.Show("本软件目前运行与U盘,是否复制到" + fileutils.target + ",并打开文件夹?");
                    if (System.Windows.Forms.MessageBox.Show("Yes or no", "本软件目前运行与U盘,是否复制到" + fileutils.target + ",并打开文件夹?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    {
                        fileutils.copyFolder(folder, fileutils.target);
                    }*/
                    // opens the folder in explorer
                    //System.Diagnostics.Process.Start(fileutils.target);
                    string Layers = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";
                    Registry.SetValue(Layers, fileutils.target + @"\Deploytool.exe", "RUNASADMIN");

                    /*System.Environment.Exit(1);
                    else
                    {
                        string drive = System.IO.Path.GetPathRoot(fileutils.path);
                        if (System.Windows.Forms.MessageBox.Show("Yes or no", "复制\"开关机软件\"/\"中控客户端\"到" + drive,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                        {
                            fileutils.copyFolder(fileutils.path + @"\softs", drive);
                        }
                    }*/
                }
                else
                {
                    fileutils.target = fileutils.path;
                }
                tb_ipfile.Text = fileutils.path + @"\IP表.csv";
                tb_save.Text = tb_ipfile.Text;
                IPDistribution();
                if (fileutils.OfficeFolderName != null)
                {
                    tb_Office_Path.Text = fileutils.OfficeFolderName;
                    cb_office.IsChecked = cfg.cb_office;
                }
                else
                {
                    tb_Office_Path.Text = cfg.tb_Office_Path;
                    cb_office.IsChecked = cfg.cb_office;
                }
                if (fileutils.PlusbeZKFolderName != null)
                {
                    tb_PlusbeZK_Path.Text = fileutils.PlusbeZKFolderName;
                    cb_PlusebeZK_reg.IsChecked = true;
                    cb_PlusebeZK_Content_Server.IsChecked = cfg.cb_Content_Server;
                    cb_PlusebeZK_RunAsAdmin.IsChecked = true;
                    cb_PlusebeZK_TopMost.IsChecked = true;
                    cb_PlusebeZK_LoopMode.IsChecked = true;
                    cb_PlusebeZK_AutoPlay.IsChecked = true;
                    cb_PlusebeZK_Resolution.IsChecked = true;
                    cb_PlusebeZK_Startup.IsChecked = true;
                    cb_PlusebeZK_HideCursor.IsChecked = true;
                    tb_PlusebeZK_Interval.Text = cfg.tb_PlusebeZK_Interval;
                    PlusebeZKLoopModeIndex = int.Parse(cfg.PlusebeZKLoopModeIndex);
                }
                else
                {
                    tb_PlusbeZK_Path.Text = cfg.tb_PlusbeZK_Path;
                    cb_PlusebeZK_reg.IsChecked = cfg.cb_PlusebeZK_reg;
                    cb_PlusebeZK_Content_Server.IsChecked = cfg.cb_Content_Server;
                    cb_PlusebeZK_RunAsAdmin.IsChecked = cfg.cb_PlusebeZK_RunAsAdmin;
                    cb_PlusebeZK_LoopMode.IsChecked = cfg.cb_PlusebeZK_LoopMode;
                    cb_PlusebeZK_AutoPlay.IsChecked = cfg.cb_PlusebeZK_AutoPlay;
                    cb_PlusebeZK_Resolution.IsChecked = cfg.cb_PlusebeZK_Resolution;
                    cb_PlusebeZK_Startup.IsChecked = cfg.cb_PlusebeZK_Startup;
                    cb_PlusebeZK_HideCursor.IsChecked = cfg.cb_PlusebeZK_HideCursor;
                    cb_PlusebeZK_TopMost.IsChecked = cfg.cb_PlusebeZK_TopMost;
                    tb_PlusebeZK_Interval.Text = cfg.tb_PlusebeZK_Interval;
                    PlusebeZKLoopModeIndex = int.Parse(cfg.PlusebeZKLoopModeIndex);

                }
                if (fileutils.ShutdownSoftFolderName != null)
                {
                    tb_ShotdownSoft_Path.Text = fileutils.ShutdownSoftFolderName;
                    cb_swsoft_startup.IsChecked = true;
                    cb_swsoft_privilege.IsChecked = true;
                }
                else
                {
                    tb_ShotdownSoft_Path.Text = cfg.tb_ShotdownSoft_Path;
                    cb_swsoft_startup.IsChecked = cfg.cb_swsoft_startup;
                    cb_swsoft_privilege.IsChecked = cfg.cb_swsoft_privilege;
                }
            if (fileutils.ZkplayFolderName != null)
            {
                tb_Zkplay_Path.Text = fileutils.ZkplayFolderName;
            }
            else
            {
                tb_Zkplay_Path.Text = cfg.tb_Zkplay_Path;
                fileutils.ZkplayFolderName = tb_Zkplay_Path.Text;
            }
            cb_Zkplay_reg.IsChecked = cfg.cb_Zkplay_reg;
            cb_Zkplay_Startup.IsChecked = cfg.cb_Zkplay_Startup;
            cb_Zkplay_Env.IsChecked = cfg.cb_Zkplay_Env;
            if (Directory.Exists(tb_Zkplay_Path.Text))
            {
                Zkplay zkplay = new Zkplay(fileutils);
                string sn = zkplay.GetGUID(zkplay.SoftPath);
                tb_Zkplay_SN.Text = sn;
                tb_Zkplay_key.Text = Zkplay.Encrypt(sn);
            }

        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
{
    BTN_EXEC.IsEnabled = false;
    ProgressBar.Value = 0;

    Console.WriteLine("开始配置...");
    CB_STATE cb_state = new CB_STATE();
    fileutils.PlusbeZKFolderName = tb_PlusbeZK_Path.Text;
    PlusbeZK plusbezk = new PlusbeZK(fileutils);
    if ((bool)cb_ip.IsChecked)
    {
        string mask = tb_mask.Text;
        adapter.setIP(tb_ip.Text, mask);
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
        string result=null;
        Match m = Regex.Match(origin, tb_ip.Text, RegexOptions.IgnoreCase);
      if (m.Success) { 
          Regex rgx = new Regex("(.*)(,|\t)"+tb_ip.Text);
          result = rgx.Replace(origin, ipdist.area + "," + tb_title.Text + "," + tb_ip.Text + "," + adapter.mac.Replace(":", "-") + "," + screenWidth + "x" + screenHeight + "," + tb_userName.Text + "," + fileutils.ZkplayFolderName);
      }
      else
      {
          result = origin + ipdist.area + "," + tb_title.Text + "," + tb_ip.Text + "," + adapter.mac.Replace(":", "-") + "," + screenWidth + "x" + screenHeight + "," + tb_userName.Text + "," + fileutils.ZkplayFolderName;
      }
    using(StreamWriter sw = new StreamWriter(
    new FileStream(tb_ipfile.Text, FileMode.Open, FileAccess.ReadWrite),
    Encoding.UTF8
))
        //using (StreamWriter sw = File.CreateText(tb_ipfile.Text))
        {
            sw.WriteLine(result);
        }

        //fileutils.append(tb_ip.Text + "----" + adapter.mac, path + @"部署工具\IP分配文件.txt");
        /*if (flashdisk != null)
        {
            REMOVEDRIVE rmdrive = new REMOVEDRIVE(fileutils);
            //rmdrive.eject(flashdisk.Name);
            Console.WriteLine("IP和MAC已记录在文件: "+path+". 可拔出U盘,部署将继续运行.");
        }*/
    }
    if ((bool)cb_gateway.IsChecked)
    {
        adapter.setGateway(tb_gateway.Text);
        Console.WriteLine("网关设置完成.");
    }
    if ((bool)cb_dns.IsChecked){
        adapter.setDNS(tb_dns.Text);
        Console.WriteLine("DNS设置完成.");
    }
    if ((bool)cb_waken.IsChecked)
    {
        //Console.WriteLine(adapter.index.PadLeft(4, '0'));
        adapter.allowAwaken();
        Console.WriteLine("\"运行网卡唤醒\"启用.");
    }
    if ((bool)cb_firewall.IsChecked)
    {
        //firewall.disable();
                firewall.StartType("demand");
                //firewall.Stop();
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
        fileutils.ShutdownSoftFolderName=tb_ShotdownSoft_Path.Text;
        switchSoft sw = new switchSoft(fileutils);
        sw.version = fileutils.ShutdownSoftFolderName + @"\" + sw.softName;
        sw.addStartup();
        Console.WriteLine("\"开关机软件\"自启设置完成.");
        sw.runAsAdmin();
        Console.WriteLine("\"开关机软件\"管理员权限设置完成.");
        sw.Exec();
        //winupdate.enable();
    }
    if ((bool)cb_vnc.IsChecked)
    {
        cb_state.vnc = true;
    }
    if ((bool)cb_klite.IsChecked)
    {
        cb_state.klite = true;
    }
    if ((bool)cb_chrome.IsChecked)
    {
        cb_state.chrome = true;
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
        fileutils.OfficeFolderName = tb_Office_Path.Text;
        cb_state.office = true;
    }
            if ((bool)cb_Zkplay_reg.IsChecked)
            {
                Zkplay.WrtiteKey(tb_Zkplay_key.Text);
                Console.WriteLine("Zkplay激活完成.");
            }
            if ((bool)cb_Zkplay_Startup.IsChecked)
            {
                Zkplay zkplay = new Zkplay(fileutils);
                zkplay.addStartup();
                Console.WriteLine("Zkplay开机启动完成.");
            }
            if ((bool)cb_Zkplay_Env.IsChecked)
            {
                cb_state.runtime = true;
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
            //File.Delete(item.path);
            //lv_delstartmenu.Items.Remove(item);
        }
    }
    if (lv_del_startup.Items != null)
    {
            registry regedit=new registry();
        foreach (SHORTCUT item in lv_del_startup.Items)
        {
                Console.WriteLine("删除启动项: "+item.path);
            if(item.type=="reg"){
                regedit.deleteValue(item.path);
            }
            else if (item.type == "lnk")
            {
                //Console.WriteLine(item.name);
                File.Delete(item.path);
            }
            //lv_delstartmenu.Items.Remove(item);
        }
    }
    if ((bool)cb_PlusebeZK_reg.IsChecked)
    {
                plusbezk.Reg();
                plusbezk.Save();
                Console.WriteLine("PlusebeZK注册完成.");

    }
    if ((bool)cb_PlusebeZK_Content_Server.IsChecked)
    {
                plusbezk.ContentServer(tb_PlusebeZK_Content_Server.Text, tb_PlusebeZK_Identity.Text, tb_PlusebeZK_Interval.Text);
                plusbezk.Save();
                Console.WriteLine("PlusebeZK内容服务器设置完成.");
    }
            if ((bool)cb_PlusebeZK_Resolution.IsChecked)
            {
                plusbezk.Resolution(tb_screenWidth.Text, tb_screenHeight.Text);
                plusbezk.Save();
                Console.WriteLine("PlusebeZK分辨率设置完成.");
            }
            if ((bool)cb_PlusebeZK_Startup.IsChecked)
            {
                plusbezk.addStartup();
                Console.WriteLine("PlusebeZK开机启设置完成.");
            }
            if ((bool)cb_PlusebeZK_HideCursor.IsChecked)
            {
                plusbezk.HideCursor();
                plusbezk.Save();
                Console.WriteLine("PlusebeZK藏鼠标设置完成.");
            }
            if ((bool)cb_PlusebeZK_TopMost.IsChecked)
            {
                plusbezk.TopMost();
                plusbezk.Save();
                Console.WriteLine("PlusebeZK置顶设置完成.");
            }
            if ((bool)cb_PlusebeZK_AutoPlay.IsChecked)
            {
                plusbezk.AutoPlay();
                plusbezk.Save();
                Console.WriteLine("PlusebeZK自播放设置完成.");
            }
            if ((bool)cb_PlusebeZK_RunAsAdmin.IsChecked)
            {
                plusbezk.runAsAdmin();
                plusbezk.Save();
                Console.WriteLine("PlusebeZK管理员运行设置完成.");
            }
            if ((bool)cb_PlusebeZK_LoopMode.IsChecked)
            {
                plusbezk.LoopMode(PlusebeZKLoopModeIndex.ToString());
                plusbezk.Save();
                Console.WriteLine("PlusebeZK播放循环模式设置完成.");
            }

            if ((bool)cb_LimitBlankPasswordUse.IsChecked|| (bool)cb_ForceGuest.IsChecked)
    {
                string reg = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                Registry.SetValue(reg, "LocalAccountTokenFilterPolicy", 1, RegistryValueKind.DWord);
                //cb_state.grouppolicy = true;
                reg = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Lsa";
                Registry.SetValue(reg, "LimitBlankPasswordUse", 0, RegistryValueKind.DWord);
                Console.WriteLine("关 '空密码受限'完成.");
                Console.WriteLine("关 'UAC远端限制'完成.");
            }

            if ((bool)cb_ForceGuest.IsChecked)
            {
                //cb_state.grouppolicy = true;
                string reg = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Lsa";
                Registry.SetValue(reg, "ForceGuest", 0, RegistryValueKind.DWord);
                Console.WriteLine("关 '远端号限Guest'完成.");
            }

            if ((bool)cb_rdp.IsChecked)
            {
                //cb_state.grouppolicy = true;
                string reg = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server";
                Registry.SetValue(reg, "fDenyTSConnections", 0, RegistryValueKind.DWord);
                reg = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp";
                Registry.SetValue(reg, "UserAuthentication", 0, RegistryValueKind.DWord);
                Console.WriteLine("开 '远程桌面'完成.");
            }

            if ((bool)cb_DefaltShare.IsChecked)
    {
        string reg = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters";
        Registry.SetValue(reg, "AutoShareWks", 1, RegistryValueKind.DWord);
        Registry.SetValue(reg, "AutoShareServer", 1, RegistryValueKind.DWord);
        Console.WriteLine("默认共享设置完成. *重启生效");
    }

    if ((bool)cb_del_2345.IsChecked)
    {
        List<String> PossiblePath = DriveInfo.GetDrives().Where(drive => drive.Name!=System.IO.Path.GetPathRoot(Environment.SystemDirectory)).Select(drive=>drive.Name).ToList();
        //PossiblePath.Add(Environment.SpecialFolder.ProgramFilesX86.ToString());
        foreach (String path in PossiblePath)
        if (Directory.GetDirectories(path, "2345Softs", SearchOption.TopDirectoryOnly).Length > 0)
        {
            String[] ADExePaths=fileutils.searchFile(path,"*MiniPage*.exe");
            foreach(string AD in ADExePaths){
                string batch = "taskkill /im "+AD;
                cmd.ExecuteCommand(batch);
                File.Delete(AD);
            }
            break;
        }
        Console.WriteLine("2345弹窗程序已删除.");
    }


    if ((bool)cb_EnableSMB1.IsChecked)
    {
        //cb_state.smb1 = true;
                string reg = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters";
                Registry.SetValue(reg, "SMB1", 1, RegistryValueKind.DWord);
                Console.WriteLine("SMB1已开. *重启生效");
            }
            if ((bool)cb_flash.IsChecked)
    {
        cb_state.flash = true;
    }
            if ((bool)cb_activation.IsChecked)
            {
                cb_state.activation = true;
            }
            if ((bool)cb_wuauserv.IsChecked)
            {
                winupdate.StartType("demand");
                winupdate.disable();
                //winupdate.Stop();
                Console.WriteLine("Windows升级已关闭.");
            }

            m_BackgroundWorker.RunWorkerAsync(cb_state);
            btn_pause.IsEnabled = true;
            Window_Closed(sender,  e);
             //cmd.ExecuteCommand("shutdown /r");
}
        private void progMgr_add(object sender, RoutedEventArgs e)
        {
            var items = lv_installed.SelectedItems.Cast<INSTALLED>().ToArray();
            foreach (INSTALLED item in items)
            {
                //Console.WriteLine(item.UninstallString);
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
        private void startupMgr_add(object sender, RoutedEventArgs e)
        {
            var items = lv_startup.SelectedItems.Cast<SHORTCUT>().ToArray();
            foreach (SHORTCUT item in items)
            {
                //Console.WriteLine(item.UninstallString);
                if (!lv_del_startup.Items.Contains(item))
                {
                    lv_del_startup.Items.Add(item);
                    //INSTALLEDS.Remove((INSTALLED)item);
                }
            }

        }
        private void startupMgr_del(object sender, RoutedEventArgs e)
        {
            var items = lv_del_startup.SelectedItems.Cast<SHORTCUT>().ToArray();
            foreach (var item in items)
            {
                lv_del_startup.Items.Remove(item);
                //INSTALLEDS.Add((INSTALLED)item);
            }
        }
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // ... Get the ComboBox reference.
            var comboBox = sender as System.Windows.Controls.ComboBox;
            List<string> data = new List<string>();

            foreach (ManagementObject oReturn in adapter.interfaceCollection)
            {
                if (oReturn.Properties["NetConnectionID"].Value != null)
                {
                    string interfaceName = oReturn.Properties["NetConnectionID"].Value.ToString();
                    data.Add(interfaceName);
                    if (interfaceName.Contains("本地连接")|| interfaceName.Contains("以太网")) comboBox.SelectedItem = interfaceName;

                }
            }


            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            //if(data.Count!=0)comboBox.SelectedIndex = AdapterIndex;
        }



        private void cbb_LoopMode_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>() {"单个循环","视频列表循环","播完黑屏","最后一帧","返回屏保","图文视频循环" };
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = data;
            comboBox.SelectedIndex = PlusebeZKLoopModeIndex;
        }



        private void cbb_LoopMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            PlusebeZKLoopModeIndex = comboBox.SelectedIndex;

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
                    adapter.interfaceName = value;
                    adapter.device = oReturn["Name"].ToString();
                    adapter.mac = oReturn["MACAddress"].ToString();
                    adapter.index = oReturn["Index"].ToString();
                    adapter.guid = oReturn["GUID"].ToString();
            tb_device.Text = adapter.device;
            tb_mac.Text = adapter.mac;
                        btn_attribute.IsEnabled = true;
                        break;
                    }
                    else
                    {
                        tb_device.Text = "网卡被禁用";
                        tb_mac.Text = "网卡被禁用";
                        btn_attribute.IsEnabled = false;
                    }
                }
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {

            cfg.cb_waken = (bool)cb_waken.IsChecked;
            cfg.cb_firewall = (bool)cb_firewall.IsChecked;
            cfg.cb_ip = (bool)cb_ip.IsChecked;
            cfg.cb_dns = (bool)cb_dns.IsChecked;
            cfg.cb_gateway = (bool)cb_gateway.IsChecked;
            cfg.cb_swsoft_startup = (bool)cb_swsoft_startup.IsChecked;
            cfg.cb_swsoft_privilege = (bool)cb_swsoft_privilege.IsChecked;
            cfg.cb_vnc = (bool)cb_vnc.IsChecked;
            cfg.cb_klite = (bool)cb_klite.IsChecked;
            cfg.cb_office = (bool)cb_office.IsChecked;
            cfg.cb_power = (bool)cb_power.IsChecked;
            cfg.cb_zoom = (bool)cb_zoom.IsChecked;
            cfg.cb_PowerButton = (bool)cb_PowerButton.IsChecked;
            cfg.cb_chrome = (bool)cb_chrome.IsChecked;
            cfg.cb_del_2345 = (bool)cb_del_2345.IsChecked;
            cfg.cb_flash=(bool)cb_flash.IsChecked;
            cfg.cb_PlusebeZK_reg = (bool)cb_PlusebeZK_reg.IsChecked;
            cfg.cb_Content_Server = (bool)cb_PlusebeZK_Content_Server.IsChecked;
            cfg.cb_PlusebeZK_RunAsAdmin = (bool)cb_PlusebeZK_RunAsAdmin.IsChecked;
            cfg.cb_PlusebeZK_LoopMode = (bool)cb_PlusebeZK_LoopMode.IsChecked;
            cfg.cb_PlusebeZK_AutoPlay = (bool)cb_PlusebeZK_AutoPlay.IsChecked;
            cfg.cb_PlusebeZK_Resolution = (bool)cb_PlusebeZK_Resolution.IsChecked;
            cfg.cb_PlusebeZK_Startup = (bool)cb_PlusebeZK_Startup.IsChecked;
            cfg.cb_PlusebeZK_HideCursor = (bool)cb_PlusebeZK_HideCursor.IsChecked;
            cfg.cb_PlusebeZK_TopMost = (bool)cb_PlusebeZK_TopMost.IsChecked;
            cfg.cb_wuauserv = (bool)cb_wuauserv.IsChecked;
            cfg.cb_activation = (bool)cb_activation.IsChecked;
            cfg.cb_DefaltShare = (bool)cb_DefaltShare.IsChecked;
            cfg.cb_UacRemoteRestric = (bool)cb_UacRemoteRestric.IsChecked;
            cfg.cb_ForceGuest = (bool)cb_ForceGuest.IsChecked;
            cfg.cb_LimitBlankPasswordUse = (bool)cb_LimitBlankPasswordUse.IsChecked;
            cfg.cb_EnableSMB1 = (bool)cb_EnableSMB1.IsChecked;
            cfg.cb_rdp = (bool)cb_rdp.IsChecked;
            cfg.tb_gateway = tb_gateway.Text;
            cfg.tb_ip = tb_ip.Text;
            cfg.tb_dns = tb_dns.Text;
            cfg.tb_mask = tb_mask.Text;
            cfg.tb_vnc_key = tb_vnc_key.Text;
            cfg.tb_vnc_pwd = tb_vnc_pwd.Text;
            cfg.tb_title = tb_title.Text;
            cfg.tb_Office_Path = tb_Office_Path.Text;
            cfg.tb_ShotdownSoft_Path = tb_ShotdownSoft_Path.Text;
            cfg.tb_PlusbeZK_Path = tb_PlusbeZK_Path.Text;
            cfg.tb_Content_Server = tb_PlusebeZK_Content_Server.Text;
            cfg.tb_PlusebeZK_Interval = tb_PlusebeZK_Interval.Text;
            cfg.PlusebeZKLoopModeIndex = PlusebeZKLoopModeIndex.ToString();
            cfg.tb_Zkplay_Path = tb_Zkplay_Path.Text;
            cfg.cb_Zkplay_reg = (bool)cb_Zkplay_reg.IsChecked;
            cfg.cb_Zkplay_Startup = (bool)cb_Zkplay_Startup.IsChecked;
            cfg.cb_Zkplay_Env = (bool)cb_Zkplay_Env.IsChecked;
            cfg.save();
            //progMgr.Close();
            FileStream fs = new FileStream(uninstall, FileMode.Create);
            formatter.Serialize(fs, new List<INSTALLED>(lv_uninstalled.Items.Cast<INSTALLED>().ToArray()));
            fs.Close();
            fs = new FileStream(unstartup, FileMode.Create);
            formatter.Serialize(fs, new List<SHORTCUT>(lv_del_startup.Items.Cast<SHORTCUT>().ToArray()));
            fs.Close();
            /*if (flashdisk != null)
            {
                System.IO.File.Copy(cfg.file, flashdisk + "部署工具/config.ini", true);
            }*/
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
                List<List<INSTALLED>> Result2 = progMgr.ListCompare(Result1,items);
                INSTALLEDS = new ObservableCollection<INSTALLED>(Result2[0]);
                //lv_uninstalled.ItemsSource = items;
                items = Result2[1];
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
    void startupManager()
    {
        try
        {
            FileStream fs = new FileStream(unstartup, FileMode.Open);
            var items = (List<SHORTCUT>)formatter.Deserialize(fs);
            fs.Close();
            StartupMgr = new StartupManager(fileutils);
            List<List<SHORTCUT>> Result2 = StartupMgr.ListCompare(StartupMgr.GetStartupList(), items);
            STARTUPS = new ObservableCollection<SHORTCUT>(Result2[0]);
            items = Result2[1];
            foreach (var item in items)
            {
                lv_del_startup.Items.Add(item);
            }
            lv_startup.ItemsSource = STARTUPS;
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
            /*
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
             */

        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {

            INSTALLED lv_uninstalled_item = ((INSTALLED)e.OldItems[0]);
                    INSTALLEDS.Add(lv_uninstalled_item);
            /*
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
              */
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
    private void unstartup_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        int limit = 6;
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // scroll the new item into view   
            //listView.ScrollIntoView(e.NewItems[0]);
            //Console.WriteLine(((INSTALLED)e.NewItems[0]).name);
            SHORTCUT lv_del_startup_item = ((SHORTCUT)e.NewItems[0]);
            STARTUPS.Remove(lv_del_startup_item);
            /*
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
             */

        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {

            SHORTCUT lv_del_startup_item = ((SHORTCUT)e.OldItems[0]);
            STARTUPS.Add(lv_del_startup_item);
            /*
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
              */
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
        openfiledialog.Filter = "acceptable file format (*.txt;*.csv)|*.txt;*.csv|All files (*.*)|*.*";

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
        if ((TreeViewItem)treeview_ip.SelectedItem != null)
        {
        TreeViewItem item = (TreeViewItem)treeview_ip.SelectedItem;
        ipdist = (IPDISTRIBUTION)item.Tag;
        tb_title.Text = ipdist.device;
        tb_ip.Text = ipdist.ip;
        tb_PlusebeZK_Identity.Text = tb_ip.Text;
        }
    }
    void IPDistribution()
    {
        treeview_ip.Items.Clear();
        try
        {
            InputFile = new List<string>(File.ReadAllLines(tb_ipfile.Text, System.Text.Encoding.Default));
        }
        catch(Exception err){}
        if (InputFile != null)
        {
            foreach (var line in InputFile)
            {
                ipdist = new IPDISTRIBUTION(line);
                TreeViewItem item = new TreeViewItem();
                item.Header = ipdist.area + " - " + ipdist.device + " - " + ipdist.ip + " - " + ipdist.mac + " - " + ipdist.SN;
                item.Tag = ipdist;
                /*item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);*/
                treeview_ip.Items.Add(item);
            }
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
           Console.WriteLine("部署完成.\n*部分软件安装可能仍在后台进行,请注意控制台提示.");
                //System.Windows.Forms.MessageBox.Show("部署完成.\n*部分软件安装可能仍在后台进行,请注意控制台提示.");
           BTN_EXEC.IsEnabled = true;
        }
    }
    void DoWork(object sender, DoWorkEventArgs e) {
        BackgroundWorker bw = sender as BackgroundWorker;
        CB_STATE cb_state = (CB_STATE)e.Argument;
            if (cb_state.runtime)
            {
                Runtime runtime = new Runtime(fileutils);
                Console.WriteLine("运行库安装中...");
                runtime.install();
                bool symbol=Install_Status.Check(fileutils,runtime.SoftName);
                Console.WriteLine("运行库安装完成.");
            }
            if (cb_state.vnc)
        {
            VNC vnc = new VNC(fileutils);
            Console.WriteLine("VNC安装中...");
            this.Dispatcher.Invoke(() =>
    {
            vnc.licenseKey = tb_vnc_key.Text;
            vnc.password = tb_vnc_pwd.Text;
    });
            vnc.install();
            bool symbol=Install_Status.Check(fileutils,vnc.SoftName);
            Console.WriteLine("VNC安装完成.");
        }
        bw.ReportProgress(25, null);
        if (cb_state.klite)
        {
            KLITE klite = new KLITE(fileutils);
            Console.WriteLine("K-Lite安装中...");
            klite.install();
            bool symbol = Install_Status.Check(fileutils, klite.SoftName);
            Console.WriteLine("K-Lite安装完成.");
        }
        bw.ReportProgress(50, null);
        if (cb_state.office)
        {
            OFFICE office = new OFFICE(fileutils);
            Console.WriteLine("MS Office安装中...");
            office.install();
            bool symbol = Install_Status.Check(fileutils, office.SoftName);
            Console.WriteLine("Office安装完成.");
        }
        if (cb_state.chrome)
        {
            CHROME chrome = new CHROME(fileutils);
            Console.WriteLine("Google Chrome安装中...");
            chrome.install();
            bool symbol = Install_Status.Check(fileutils, chrome.SoftName);
            Console.WriteLine("Google Chrome安装完成.");
        }
        bw.ReportProgress(75, null);
        if (cb_state.uninstall != null)
        {
            foreach (INSTALLED item in lv_uninstalled.Items)
            {
                _busy.WaitOne();
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                Console.WriteLine(item.name + "卸载中...");
                if(item.name.Contains("安全卫士 - 2345")){
                foreach (SHORTCUT lnk in lv_allstartmenu.Items)
        {
                if (lnk.name.Contains("2345安全中心"))
                {
                    Process.Start(lnk.path);
                }

        }
                }
                else { 
                    progMgr.uninstall(item);
                }
                //lv_uninstalled.Items.Remove(item);
                Console.WriteLine(item.name + "卸载完成.");
            }
        }
        if(cb_state.smb1){
            Console.WriteLine("SMB1组件安装中...");
            string batch = "powershell Enable-WindowsOptionalFeature -Online -FeatureName smb1protocol";
            cmd.ExecuteCommand(batch);
            Console.WriteLine("SMB1组件安装完成");
        }
        if (cb_state.flash)
        {
            Console.WriteLine("FlashPlayer安装中...");
            FLASH flash=new FLASH(fileutils);
            flash.install();
            //bool symbol = Install_Status.Check(fileutils, "Flash");
            Console.WriteLine("FlashPlayer安装完成.");

        }
        if (cb_state.grouppolicy)
        {
            AutoIt autoit = new AutoIt(fileutils);
            string script = System.IO.Path.GetDirectoryName(autoit.version) + @"\gp(CN).au3";
            autoit.Run(script);
            Console.WriteLine("关 '远端号限Guest'完成.");
            Console.WriteLine("关 '空密码受限'完成.");

        }
            if (cb_state.activation)
            {
                Console.WriteLine("Windows激活中...");
                WinActivation winact = new WinActivation(fileutils);
                winact.Activation();
            }
            bw.ReportProgress(100, null);
    }
    private void Cancel(object sender, EventArgs e)
    {
        m_BackgroundWorker.CancelAsync();
        BTN_EXEC.IsEnabled = true;
    }
        private void adapter_attribute(object sender, RoutedEventArgs e)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(@"::{208D2C60-3AEA-1069-A2D7-08002B30309D}\::{7007ACC7-3202-11D1-AAD2-00805FC1270E}\::" + adapter.guid);
        startInfo.UseShellExecute = true;
        Process.Start(startInfo);
        Thread.Sleep(1000);
        //IntPtr ArrtibuteHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, adapter.interfaceName + " Properties");
        IntPtr ArrtibuteHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, adapter.interfaceName + " 属性");
        const int BM_CLICK = 0xF5;
        if (ArrtibuteHwnd != IntPtr.Zero)
        {
            //Console.WriteLine("找到窗口");
            //IntPtr TabHwnd = FindWindowEx(ArrtibuteHwnd, IntPtr.Zero, null, "Networking");   //获得按钮的句柄  
            IntPtr TabHwnd = FindWindowEx(ArrtibuteHwnd, IntPtr.Zero, null, "网络");   //获得按钮的句柄  
            if (TabHwnd != IntPtr.Zero)
            {
                //Console.WriteLine("找到子窗口");
                //IntPtr btnHwnd = FindWindowEx(TabHwnd, IntPtr.Zero, null, "&Configure...");   //获得按钮的句柄  
                IntPtr btnHwnd = FindWindowEx(TabHwnd, IntPtr.Zero, null, "配置(&C)...");   //获得按钮的句柄  
                if (btnHwnd != IntPtr.Zero)
                {
                    //Console.WriteLine("找到按钮");
                    SendMessage(btnHwnd, BM_CLICK, IntPtr.Zero, null);
                    Thread.Sleep(1000);
                    IntPtr PropertiesHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, adapter.device+" 属性");
                    if (PropertiesHwnd != IntPtr.Zero)
                    {                         
                        //Console.WriteLine("找到按钮");
                        ClickOnPointTool.ClickOnPoint(PropertiesHwnd, new System.Drawing.Point(100, 20));
                    }
                }
            }
        }
    }
    private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = default(Exception);
        ex = (Exception)e.ExceptionObject;
        ILog log = LogManager.GetLogger(typeof(MainWindow));
        log.Error(ex.Message + "\n" + ex.StackTrace);
    }

    private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        Exception ex = default(Exception);
        ex = e.Exception;
        ILog log = LogManager.GetLogger(typeof(MainWindow)); //Log4NET
        log.Error(ex.Message + "\n" + ex.StackTrace);
    }
    private void ShutdownSoft_Browser(object sender, EventArgs e)
    {
        using (var fbd = new FolderBrowserDialog())
        {
            DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                //string[] files = Directory.GetFiles(fbd.SelectedPath);
                tb_ShotdownSoft_Path.Text = fbd.SelectedPath;
                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }
    }
    private void Office_Browser(object sender, EventArgs e)
    {
        using (var fbd = new FolderBrowserDialog())
        {
            DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                //string[] files = Directory.GetFiles(fbd.SelectedPath);
                tb_Office_Path.Text = fbd.SelectedPath;
                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }
    }
    private void PlusbeZK_Browser(object sender, EventArgs e)
    {
        using (var fbd = new FolderBrowserDialog())
        {
            DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                //string[] files = Directory.GetFiles(fbd.SelectedPath);
                tb_PlusbeZK_Path.Text = fbd.SelectedPath;
                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }
    }
        private void Zkplay_Browser(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //string[] files = Directory.GetFiles(fbd.SelectedPath);
                    tb_Zkplay_Path.Text = fbd.SelectedPath;
                    fileutils.ZkplayFolderName = tb_Zkplay_Path.Text;
                    Zkplay zkplay = new Zkplay(fileutils);
                    string sn = zkplay.GetGUID(zkplay.SoftPath);
                    tb_Zkplay_SN.Text = sn;
                    tb_Zkplay_key.Text = Zkplay.Encrypt(sn);
                    //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                }
            }
        }
        private void PathCheck(object sender, RoutedEventArgs e)
    {
        System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
        if (cb.Name.Contains("swsoft")){
            if (!Directory.Exists(tb_ShotdownSoft_Path.Text))
            {
                Console.WriteLine("关机软件路径空或不存在.");
                cb.IsChecked = false;
            }
        }
        else if (cb.Name.Contains("PlusebeZK") || cb.Name.Contains("Content_Server"))
        {
            if (!Directory.Exists(tb_PlusbeZK_Path.Text))
            {
                Console.WriteLine("中控软件路径空或不存在.");
                cb.IsChecked = false;
            }
        }
            else if (cb.Name.Contains("Zkplay") )
            {
                if (!Directory.Exists(tb_Zkplay_Path.Text))
                {
                    Console.WriteLine("Zkplay路径空或不存在.");
                    cb.IsChecked = false;
                }
            }
            else if (cb.Name.Contains("office"))
        {
            if (!Directory.Exists(tb_Office_Path.Text))
            {
                Console.WriteLine("Office路径空或不存在.");
                cb.IsChecked = false;
            }
        }
            else if (cb.Name.Contains("klite"))
            {
                KLITE kilte = new KLITE(fileutils);
                if (kilte.version==null)
                {
                    Console.WriteLine("K-Lite安装文件不存在.");
                    cb.IsChecked = false;

                }
            }
            else if (cb.Name.Contains("chrome"))
            {
                CHROME chrome = new CHROME(fileutils);
                if (chrome.version == null)
                {
                    Console.WriteLine("Chrome安装文件不存在.");
                    cb.IsChecked = false;

                }
            }
            else if (cb.Name.Contains("vnc"))
            {
                VNC vnc = new VNC(fileutils);
                if (vnc.version == null)
                {
                    Console.WriteLine("RealVNCv6安装文件不存在.");
                    cb.IsChecked = false;

                }
            }
            else if (cb.Name.Contains("flash"))
            {
                FLASH flash = new FLASH(fileutils);
                if (flash.files.Length==0)
                {
                    Console.WriteLine("Flash(无广告)安装文件不存在.");
                    cb.IsChecked = false;

                }
            }
            else if (cb.Name.Contains("activation"))
            {
                WinActivation winact=new WinActivation(fileutils);
                if (winact.OemPath == null)
                {
                    Console.WriteLine("Oem7F7.exe不存在.");
                    cb.IsChecked = false;
                }
                else if (winact.KmsPath == null)
                {
                    Console.WriteLine("KMSAuto Net.exe不存在.");
                    cb.IsChecked = false;
                }
            }
        }
    }
}
