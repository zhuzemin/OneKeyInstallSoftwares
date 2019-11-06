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
        public string tb_ip { get; set; }
        public string tb_gateway { get; set; }

        public CONFIG()
        {
            try
            {
                file = "config.ini";
                cfg = parser.ReadFile(file);
                tb_ip = cfg["Network"]["tb_ip"];
                tb_gateway = cfg["Network"]["tb_gateway"];
                cb_waken = bool.Parse(cfg["Network"]["cb_waken"]);
                cb_firewall = bool.Parse(cfg["Network"]["cb_firewall"]);
                cb_ip = bool.Parse(cfg["Network"]["cb_ip"]);
                cb_gateway = bool.Parse(cfg["Network"]["cb_gateway"]);
                cb_swsoft_startup = bool.Parse(cfg["Install"]["cb_swsoft_startup"]);
                cb_swsoft_privilege = bool.Parse(cfg["Install"]["cb_swsoft_privilege"]);
                cb_vnc = bool.Parse(cfg["Install"]["cb_vnc"]);
                cb_klite = bool.Parse(cfg["Install"]["cb_klite"]);
                cb_office = bool.Parse(cfg["Install"]["cb_office"]);
                cb_power = bool.Parse(cfg["Other"]["cb_power"]);
                cb_zoom = bool.Parse(cfg["Other"]["cb_zoom"]);
            }
            catch (Exception err) { }

        }
        public void save()
        {
            cfg["Network"]["tb_ip"] = tb_ip;
            cfg["Network"]["tb_gateway"] = tb_gateway;
            cfg["Network"]["cb_waken"] = cb_waken.ToString();
            cfg["Network"]["cb_firewall"] = cb_firewall.ToString();
            cfg["Network"]["cb_ip"] = cb_ip.ToString();
            cfg["Network"]["cb_gateway"] = cb_gateway.ToString();
            cfg["Install"]["cb_swsoft_startup"] = cb_swsoft_startup.ToString();
            cfg["Install"]["cb_swsoft_privilege"] = cb_swsoft_privilege.ToString();
            cfg["Install"]["cb_vnc"] = cb_vnc.ToString();
            cfg["Install"]["cb_klite"] = cb_klite.ToString();
            cfg["Install"]["cb_office"] = cb_office.ToString();
            cfg["Other"]["cb_power"] = cb_power.ToString();
            cfg["Other"]["cb_zoom"] = cb_zoom.ToString();
            parser.WriteFile(file, cfg);
        }
    }
}
