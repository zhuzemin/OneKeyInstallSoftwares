using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IniParser;
using IniParser.Model;

namespace Deploytool.lib
{
    class CONFIG
    {
        private FileIniDataParser parser = new FileIniDataParser();
        private IniData cfg;
        public string file { set; get; }
        public bool cb_ip { get; set; }
        public bool cb_gateway { get; set; }
        public bool cb_waken { get; set; }
        public bool cb_firewall { get; set; }
        public bool cb_swsoft_startup { get; set; }
        public bool cb_swsoft_privilege { get; set; }
        public bool cb_vnc { get; set; }
        public bool cb_klite { get; set; }
        public bool cb_office { get; set; }
        public bool cb_power { get; set; }
        public bool cb_zoom { get; set; }
        public bool cb_PowerButton { get; set; }
        public bool cb_dns { get; set; }
        public bool cb_chrome { get; set; }
        public bool cb_del_2345 { get; set; }
        public bool cb_flash { get; set; }
        public bool cb_EnableSMB1 { get; set; }
        public bool cb_LimitBlankPasswordUse { get; set; }
        public bool cb_ForceGuest { get; set; }
        public bool cb_UacRemoteRestric { get; set; }
        public bool cb_DefaltShare { get; set; }
        public bool cb_activation { get; set; }
        public bool cb_wuauserv { get; set; }
        public bool cb_PlusebeZK_reg { get; set; }
        public bool cb_Content_Server { get; set; }
        public bool cb_PlusebeZK_LoopMode { get; set; }
        public bool cb_PlusebeZK_AutoPlay { get; set; }
        public bool cb_PlusebeZK_Resolution { get; set; }
        public bool cb_PlusebeZK_Startup { get; set; }
        public bool cb_PlusebeZK_HideCursor { get; set; }
        public bool cb_PlusebeZK_TopMost { get; set; }
        public bool cb_PlusebeZK_RunAsAdmin { get; set; }
        public bool cb_rdp { get; set; }
        public string PlusebeZKLoopModeIndex { get; set; }
        public string tb_Content_Server { get; set; }
        public string tb_PlusebeZK_Interval { get; set; }
        public string tb_PlusbeZK_Path { get; set; }
        public string tb_ip { get; set; }
        public string tb_gateway { get; set; }
        public string tb_vnc_key { get; set; }
        public string tb_vnc_pwd { get; set; }
        public string tb_dns { get; set; }
        public string tb_mask { get; set; }
        public string tb_title { get; set; }
        public string tb_ShotdownSoft_Path { get; set; }
        public string tb_Office_Path { get; set; }
        public string tb_Zkplay_Path { get; set; }
        public bool cb_Zkplay_reg { get; set; }
        public bool cb_Zkplay_Startup { get; set; }
        public bool cb_Zkplay_Env { get; set; }

        public CONFIG()
        {
            try
            {
                file = "config.ini";
                cfg = parser.ReadFile(file);
                tb_ip = cfg["Network"]["tb_ip"];
                tb_dns = cfg["Network"]["tb_dns"];
                tb_gateway = cfg["Network"]["tb_gateway"];
                tb_mask = cfg["Network"]["tb_mask"];
                tb_title = cfg["Network"]["tb_title"];
                tb_vnc_pwd = cfg["Install"]["tb_vnc_pwd"];
                tb_vnc_key = cfg["Install"]["tb_vnc_key"];
                tb_ShotdownSoft_Path = cfg["Install"]["tb_ShotdownSoft_Path"];
                tb_Office_Path = cfg["Install"]["tb_Office_Path"];
                tb_PlusbeZK_Path = cfg["PlusebeZK"]["tb_PlusbeZK_Path"];
                tb_Content_Server = cfg["PlusebeZK"]["tb_Content_Server"];
                PlusebeZKLoopModeIndex = cfg["PlusebeZK"]["PlusebeZKLoopModeIndex"];
                tb_PlusebeZK_Interval = cfg["PlusebeZK"]["tb_PlusebeZK_Interval"];
                cb_waken = bool.Parse(cfg["Network"]["cb_waken"]);
                cb_firewall = bool.Parse(cfg["Network"]["cb_firewall"]);
                cb_ip = bool.Parse(cfg["Network"]["cb_ip"]);
                cb_dns = bool.Parse(cfg["Network"]["cb_dns"]);
                cb_gateway = bool.Parse(cfg["Network"]["cb_gateway"]);
                cb_swsoft_startup = bool.Parse(cfg["Install"]["cb_swsoft_startup"]);
                cb_swsoft_privilege = bool.Parse(cfg["Install"]["cb_swsoft_privilege"]);
                cb_vnc = bool.Parse(cfg["Install"]["cb_vnc"]);
                cb_klite = bool.Parse(cfg["Install"]["cb_klite"]);
                cb_office = bool.Parse(cfg["Install"]["cb_office"]);
                cb_chrome = bool.Parse(cfg["Install"]["cb_chrome"]);
                cb_flash = bool.Parse(cfg["Install"]["cb_flash"]);
                cb_power = bool.Parse(cfg["Other"]["cb_power"]);
                cb_zoom = bool.Parse(cfg["Other"]["cb_zoom"]);
                cb_PowerButton = bool.Parse(cfg["Other"]["cb_PowerButton"]);
                cb_del_2345 = bool.Parse(cfg["Other"]["cb_del_2345"]);
                cb_activation = bool.Parse(cfg["Other"]["cb_activation"]);
                cb_wuauserv = bool.Parse(cfg["Other"]["cb_wuauserv"]);
                cb_rdp = bool.Parse(cfg["Other"]["cb_rdp"]);
                cb_EnableSMB1 = bool.Parse(cfg["PsExec"]["cb_EnableSMB1"]);
                cb_LimitBlankPasswordUse = bool.Parse(cfg["PsExec"]["cb_LimitBlankPasswordUse"]);
                cb_ForceGuest = bool.Parse(cfg["PsExec"]["cb_ForceGuest"]);
                cb_UacRemoteRestric = bool.Parse(cfg["PsExec"]["cb_UacRemoteRestric"]);
                cb_DefaltShare = bool.Parse(cfg["PsExec"]["cb_DefaltShare"]);
                cb_PlusebeZK_reg = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_reg"]);
                cb_Content_Server = bool.Parse(cfg["PlusebeZK"]["cb_Content_Server"]);
                cb_PlusebeZK_RunAsAdmin = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_RunAsAdmin"]);
                cb_PlusebeZK_AutoPlay = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_AutoPlay"]);
                cb_PlusebeZK_Resolution = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_Resolution"]);
                cb_PlusebeZK_Startup = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_Startup"]);
                cb_PlusebeZK_HideCursor = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_HideCursor"]);
                cb_PlusebeZK_TopMost = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_TopMost"]);
                cb_PlusebeZK_LoopMode = bool.Parse(cfg["PlusebeZK"]["cb_PlusebeZK_LoopMode"]);
                cb_Zkplay_Startup = bool.Parse(cfg["Zkplay"]["cb_Zkplay_Startup"]);
                cb_Zkplay_reg = bool.Parse(cfg["Zkplay"]["cb_Zkplay_reg"]);
                tb_Zkplay_Path = cfg["Zkplay"]["tb_Zkplay_Path"];
                cb_Zkplay_Env = bool.Parse(cfg["Zkplay"]["cb_Zkplay_Env"]);

            }
            catch (Exception err) { }

        }
        public void save()
        {
            cfg["Network"]["tb_ip"] = tb_ip;
            cfg["Network"]["tb_dns"] = tb_dns;
            cfg["Network"]["tb_gateway"] = tb_gateway;
            cfg["Network"]["tb_mask"] = tb_mask;
            cfg["Network"]["tb_title"] = tb_title;
            cfg["Install"]["tb_vnc_key"] = tb_vnc_key;
            cfg["Install"]["tb_vnc_pwd"] = tb_vnc_pwd;
            cfg["Install"]["tb_ShotdownSoft_Path"] = tb_ShotdownSoft_Path;
            cfg["PlusebeZK"]["tb_PlusbeZK_Path"] = tb_PlusbeZK_Path;
            cfg["PlusebeZK"]["tb_Content_Server"] = tb_Content_Server;
            cfg["PlusebeZK"]["tb_PlusebeZK_Interval"] = tb_PlusebeZK_Interval;
            cfg["PlusebeZK"]["PlusebeZKLoopModeIndex"] = PlusebeZKLoopModeIndex;
            cfg["PlusebeZK"]["cb_PlusebeZK_RunAsAdmin"] = cb_PlusebeZK_RunAsAdmin.ToString();
            cfg["PlusebeZK"]["cb_PlusebeZK_LoopMode"] = cb_PlusebeZK_LoopMode.ToString();
            cfg["PlusebeZK"]["cb_PlusebeZK_AutoPlay"] = cb_PlusebeZK_AutoPlay.ToString();
            cfg["PlusebeZK"]["cb_PlusebeZK_Resolution"] = cb_PlusebeZK_Resolution.ToString();
            cfg["PlusebeZK"]["cb_PlusebeZK_Startup"] = cb_PlusebeZK_Startup.ToString();
            cfg["PlusebeZK"]["cb_PlusebeZK_HideCursor"] = cb_PlusebeZK_HideCursor.ToString();
            cfg["PlusebeZK"]["cb_PlusebeZK_TopMost"] = cb_PlusebeZK_TopMost.ToString();
            cfg["PlusebeZK"]["cb_Content_Server"] = cb_Content_Server.ToString();
            cfg["PlusebeZK"]["cb_PlusebeZK_reg"] = cb_PlusebeZK_reg.ToString();
            cfg["Install"]["tb_Office_Path"] = tb_Office_Path;
            cfg["Network"]["cb_waken"] = cb_waken.ToString();
            cfg["Network"]["cb_firewall"] = cb_firewall.ToString();
            cfg["Network"]["cb_ip"] = cb_ip.ToString();
            cfg["Network"]["cb_dns"] = cb_dns.ToString();
            cfg["Network"]["cb_gateway"] = cb_gateway.ToString();
            cfg["Install"]["cb_swsoft_startup"] = cb_swsoft_startup.ToString();
            cfg["Install"]["cb_swsoft_privilege"] = cb_swsoft_privilege.ToString();
            cfg["Install"]["cb_vnc"] = cb_vnc.ToString();
            cfg["Install"]["cb_klite"] = cb_klite.ToString();
            cfg["Install"]["cb_office"] = cb_office.ToString();
            cfg["Install"]["cb_chrome"] = cb_chrome.ToString();
            cfg["Install"]["cb_flash"] = cb_flash.ToString();
            cfg["Other"]["cb_power"] = cb_power.ToString();
            cfg["Other"]["cb_zoom"] = cb_zoom.ToString();
            cfg["Other"]["cb_PowerButton"] = cb_PowerButton.ToString();
            cfg["Other"]["cb_del_2345"] = cb_del_2345.ToString();
            cfg["Other"]["cb_activation"] = cb_activation.ToString();
            cfg["Other"]["cb_wuauserv"] = cb_wuauserv.ToString();
            cfg["Other"]["cb_rdp"] = cb_rdp.ToString();
            cfg["PsExec"]["cb_DefaltShare"] = cb_DefaltShare.ToString();
            cfg["PsExec"]["cb_UacRemoteRestric"] = cb_UacRemoteRestric.ToString();
            cfg["PsExec"]["cb_ForceGuest"] = cb_ForceGuest.ToString();
            cfg["PsExec"]["cb_LimitBlankPasswordUse"] = cb_LimitBlankPasswordUse.ToString();
            cfg["PsExec"]["cb_EnableSMB1"] = cb_EnableSMB1.ToString();
            cfg["Zkplay"]["cb_Zkplay_Startup"] = cb_Zkplay_Startup.ToString();
            cfg["Zkplay"]["cb_Zkplay_reg"] = cb_Zkplay_reg.ToString();
            cfg["Zkplay"]["tb_Zkplay_Path"] = tb_Zkplay_Path.ToString();
            cfg["Zkplay"]["cb_Zkplay_Env"] = cb_Zkplay_Env.ToString();
            parser.WriteFile(file, cfg);
        }
    }
}
