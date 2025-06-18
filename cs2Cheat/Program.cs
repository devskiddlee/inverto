using ClickableTransparentOverlay;
using ClickableTransparentOverlay.Win32;
using ImGuiNET;
using Swed64;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using whatsapp;
using static whatsapp.Win32;

namespace cs2Cheat
{
    
    public class default_settings
    {
        public int AIMBOT_HOTKEY { get; set; }
        public int ONLY_AIMBOT_HOTKEY { get; set; }
        public int BHOP_HOTKEY { get; set; }
        public int FIRE_HOTKEY { get; set; }
        public int JUMPSHOT_HOTKEY { get; set; }
        public int MENU_HOTKEY { get; set; }
        public int SPINBOT_HOTKEY { get; set; }
        public int TRIGGERBOT_HOTKEY { get; set; }
        public int CHATSPAM_HOTKEY { get; set; }

        public bool instantShoot { get; set; }

        public bool teamCheck { get; set; }

        public int shoot_delay { get; set; }

        public bool esp { get; set; }
        public bool healthBox { get; set; }
        public bool healthText { get; set; }
        public bool directionTracer { get; set; }
        public bool playerNameText { get; set; }
        public bool boneEsp { get; set; }

        public Vector4 espColor { get; set; }
        public Vector4 healthBarColor { get; set; }
        public Vector4 healthTextColor { get; set; }
        public Vector4 directionTracerColor { get; set; }
        public Vector4 playerNameTextColor { get; set; }
        public Vector4 aimLockedColor { get; set; }
        public Vector4 closestColor { get; set; }
        public Vector4 boneColor { get; set; }

        public float meshViewDistance { get; set; }

        public float espWidth { get; set; }

        public bool aimbot_cd { get; set; }
        public bool aimbot { get; set; }
        public bool onlyShootWhenSpotted { get; set; }
        public bool bhop { get; set; }

        public bool jumpShotHack { get; set; }
        public string chatSpamText { get; set; }
        public int rainbowSpeed { get; set; }
        public int aimbotspeed { get; set; }
        public bool rainbowMenu { get; set; }
        public int customtheme { get; set; }
        public float maxAngleDiffAimbot { get; set; }
        public bool simpleSpinBot { get; set; }
        public int currentConfig { get; set; }
    }

    class Program : Overlay
    {
        string clientID = "devskiddlee";

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]

        static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport("User32.dll")]
        public static extern void ShowCursor(bool bShowCursor);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        int fps_offset = 0;


        public RECT GetWindowRect(IntPtr hWnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hWnd, out rect);
            return rect;
        }

        Random rnd = new Random();

        Swed swed = new("cs2");
        Offsets offsets = new Offsets();

        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();

        public int boneid = 58;

        IntPtr client = IntPtr.Zero;

        public int fps_value = 0;

        bool menuToggled = false;

        esp espWin = null;



        Vector2 windowLocation = new Vector2(0, 0);
        Vector2 windowSize = new Vector2(1920, 1080);
        Vector2 windowCenter = new Vector2(1920 / 2, 1080 / 2);

        string url = "";
        bool spinBotToggled = false;
        bool shoot = false;

        const uint INAIR = 65664;
        const uint STANDING = 65665;

        float rainbowProgress = 0;
        float lsdRainbowProgress = 0;
        Vector3 angles;
        IntPtr localPlayerController;

        string[] playerNames = [];
        string[] steamIDs = [];
        string[] configs = [];
        int currPlayerFaceitFinder = 0;

        bool spotted = false;
        public bool rcs = false;
        public bool showRecoil = true;

        public int AIMBOT_HOTKEY = 0x05;         //mouse4
        public int ONLY_AIMBOT_HOTKEY = 0x06;    //mouse5
        public int BHOP_HOTKEY = 0x12;          //alt
        public int FIRE_HOTKEY = 0x01;           //leftclick
        public int JUMPSHOT_HOTKEY = 0x06;     //mouse4
        public int MENU_HOTKEY = (int)VK.INSERT; //insert
        public int SPINBOT_HOTKEY = 226; // <>|
        public int TRIGGERBOT_HOTKEY = (int)VK.KEY_X;
        public int CHATSPAM_HOTKEY = (int)VK.OEM_5; // ^

        public bool instantShoot = true;

        public bool teamCheck = true;

        public int shoot_delay = 400;

        public bool esp = true;
        public bool healthBox = true;
        public bool healthText = true;
        public bool directionTracer = true;
        public bool playerNameText = true;
        public bool boneEsp = true;
        public bool extraCrosshair = true;
        public bool allowWrite = true;
        public bool antiFlash = true;
        public bool boxEsp = true;
        public bool velDisplay = true;
        public bool autoAimWhenVisible = false;
        public bool noAimWhenBehindWall = false;

        float easingFactor = 22f;

        public Vector4 espColor = new Vector4(1, 0, 0, 1);
        public Vector4 healthBarColor = new Vector4(0, 1, 0, 1);
        public Vector4 healthTextColor = new Vector4(0, 1, 0, 1);
        public Vector4 directionTracerColor = new Vector4(1, 1, 0, 1);
        public Vector4 playerNameTextColor = new Vector4(1, 1, 1, 1);
        public Vector4 aimLockedColor = new Vector4(0, 1, 0, 1);
        public Vector4 closestColor = new Vector4(0, 0, 1, 1);
        public Vector4 boneColor = new Vector4(1, 1, 1, 0.8f);
        public Vector4 boxColor = new Vector4(1, 0, 0, 0.2f);

        public float espWidth = 1.5f;

        public bool aimbot_cd = false;
        public bool aimbot = true;
        public bool onlyShootWhenSpotted = true;
        public bool bhop = true;
        public bool counterStrafe = true;
        public bool onlyShootWhenStill = true;
        public bool controlShoot = true;
        public bool disableAngleDiff = true;
        public bool radarHack = true;

        public bool jumpShotHack = false;
        public string chatSpamText = "";
        public float crosshairSize = 5f;
        public int rainbowSpeed = 1;
        public int aimbotspeed = 3000;
        public bool rainbowMenu = false;
        public int customtheme = 0;
        public float maxAngleDiffAimbot = 100f;
        public bool simpleSpinBot = true;
        public int currentConfig = 0;
        string newConfigName = "";

        void createConfig(string name)
        {
            string s = "";
            s += $"radarHack->{radarHack};";
            s += $"disableAngleDiff->{disableAngleDiff};";
            s += $"showRecoil->{showRecoil};";
            s += $"rcs->{rcs};";
            s += $"velDisplay->{velDisplay};";
            s += $"boxColor->{boxColor};";
            s += $"allowWrite->{allowWrite};";
            s += $"antiFlash->{antiFlash};";
            s += $"crosshairSize->{crosshairSize};";
            s += $"extraCrosshair->{extraCrosshair};";
            s += $"counterStrafe->{counterStrafe};";
            s += $"onlyShootWhenStill->{onlyShootWhenStill};";
            s += $"borderColor->{borderColor};";
            s += $"titleBgColor->{titleBgColor};";
            s += $"windowBg->{windowBg};";
            s += $"textColor->{textColor};";
            s += $"tabColor->{tabColor};";
            s += $"tabSelectedColor->{tabSelectedColor};";
            s += $"sliderGrabColor->{sliderGrabColor};";
            s += $"buttonColor->{buttonColor};";
            s += $"buttonClickedColor->{buttonClickedColor};";
            s += $"checkmarkColor->{checkmarkColor};";
            s += $"allBgColor->{allBgColor};";
            s += $"allBgHoverColor->{allBgHoverColor};";
            s += $"headerColor->{headerColor};";
            s += $"headerHoverColor->{headerHoverColor};";
            s += $"AIMBOT_HOTKEY->{AIMBOT_HOTKEY};";
            s += $"ONLY_AIMBOT_HOTKEY->{ONLY_AIMBOT_HOTKEY};";
            s += $"BHOP_HOTKEY->{BHOP_HOTKEY};";
            s += $"FIRE_HOTKEY->{FIRE_HOTKEY};";
            s += $"JUMPSHOT_HOTKEY->{JUMPSHOT_HOTKEY};";
            s += $"MENU_HOTKEY->{MENU_HOTKEY};";
            s += $"SPINBOT_HOTKEY->{SPINBOT_HOTKEY};";
            s += $"TRIGGERBOT_HOTKEY->{TRIGGERBOT_HOTKEY};";
            s += $"CHATSPAM_HOTKEY->{CHATSPAM_HOTKEY};";
            s += $"teamCheck->{teamCheck};";
            s += $"instantShoot->{instantShoot};";
            s += $"shoot_delay->{shoot_delay};";
            s += $"esp->{esp};";
            s += $"healthBox->{healthBox};";
            s += $"healthText->{healthText};";
            s += $"directionTracer->{directionTracer};";
            s += $"playerNameText->{playerNameText};";
            s += $"boneEsp->{boneEsp};";
            s += $"espColor->{espColor};";
            s += $"healthBarColor->{healthBarColor};";
            s += $"healthTextColor->{healthTextColor};";
            s += $"directionTracerColor->{directionTracerColor};";
            s += $"playerNameTextColor->{playerNameTextColor};";
            s += $"aimLockedColor->{aimLockedColor};";
            s += $"closestColor->{closestColor};";
            s += $"boneColor->{boneColor};";
            s += $"espWidth->{espWidth};";
            s += $"aimbot->{aimbot};";
            s += $"onlyShootWhenSpotted->{onlyShootWhenSpotted};";
            s += $"bhop->{bhop};";
            s += $"jumpShotHack->{jumpShotHack};";
            s += $"chatSpamText->{chatSpamText};";
            s += $"aimbotspeed->{aimbotspeed};";
            s += $"rainbowMenu->{rainbowMenu};";
            s += $"rainbowSpeed->{rainbowSpeed};";
            s += $"customtheme->{customtheme};";
            s += $"maxAngleDiffAimbot->{maxAngleDiffAimbot};";
            s += $"simpleSpinBot->{simpleSpinBot};";
            s += $"currentConfig->{currentConfig};";
            Console.WriteLine(s);
            File.WriteAllText($@"{getWhatsappPath()}\configs\{name}.cfg", s);
        }

        Vector4 borderColor = new Vector4(1, 0, 0, 1);
        Vector4 titleBgColor = new Vector4(1, 0, 0, 1);
        Vector4 windowBg = new Vector4(0, 0, 0, 0.75f);
        Vector4 textColor = new Vector4(1f, 0, 0, 1f);
        Vector4 tabColor = new Vector4(0.1f, 0.1f, 0.1f, 1f);
        Vector4 tabSelectedColor = new Vector4(0, 0, 0, 1f);
        Vector4 sliderGrabColor = new Vector4(0, 0, 0, 1f);
        Vector4 buttonColor = new Vector4(0.8f, 0.8f, 0.8f, 1f);
        Vector4 buttonClickedColor = new Vector4(0.9f, 0.9f, 0.9f, 1f);
        Vector4 checkmarkColor = new Vector4(0.6f, 0.6f, 0.6f, 1f);
        Vector4 allBgColor = new Vector4(0.3f, 0.3f, 0.3f, 1f);
        Vector4 allBgHoverColor = new Vector4(0.4f, 0.4f, 0.4f, 1f);
        Vector4 headerColor = new Vector4(0.8f, 0.8f, 0.8f, 1f);
        Vector4 headerHoverColor = new Vector4(0.9f, 0.9f, 0.9f, 1f);

        object safeLoad(string cfgdata, string varName, Type type)
        {
            string value = getBetween(cfgdata, $"{varName}->", ";");
            if (value == "")
                value = GetType().GetField(varName).GetValue(this).ToString();
            if (type == typeof(Vector4))
            {
                return strToVec4(value);
            }
            if (type == typeof(bool))
            {
                return bool.Parse(value);
            }
            if (type == typeof(int))
            {
                return int.Parse(value);
            }
            if (type == typeof(string))
            {
                return value;
            }
            if (type == typeof(float))
            {
                return float.Parse(value);
            }
            if (type == typeof(double))
            {
                return double.Parse(value);
            }
            return null;
        }

        void loadConfig()
        {
            string name = configs[currentConfig];
            string cfgdata = File.ReadAllText($@"{getWhatsappPath()}\configs\{name}");
            borderColor = (Vector4)safeLoad(cfgdata, "borderColor", typeof(Vector4));
            titleBgColor = (Vector4)safeLoad(cfgdata, "titleBgColor", typeof(Vector4));
            windowBg = (Vector4)safeLoad(cfgdata, "windowBg", typeof(Vector4));
            textColor = (Vector4)safeLoad(cfgdata, "textColor", typeof(Vector4));
            tabColor = (Vector4)safeLoad(cfgdata, "tabColor", typeof(Vector4));
            tabSelectedColor = (Vector4)safeLoad(cfgdata, "tabSelectedColor", typeof(Vector4));
            sliderGrabColor = (Vector4)safeLoad(cfgdata, "sliderGrabColor", typeof(Vector4));
            buttonColor = (Vector4)safeLoad(cfgdata, "buttonColor", typeof(Vector4));
            buttonClickedColor = (Vector4)safeLoad(cfgdata, "buttonClickedColor", typeof(Vector4));
            checkmarkColor = (Vector4)safeLoad(cfgdata, "checkmarkColor", typeof(Vector4));
            allBgColor = (Vector4)safeLoad(cfgdata, "allBgColor", typeof(Vector4));
            allBgHoverColor = (Vector4)safeLoad(cfgdata, "allBgHoverColor", typeof(Vector4));
            headerColor = (Vector4)safeLoad(cfgdata, "headerColor", typeof(Vector4));
            boxColor = (Vector4)safeLoad(cfgdata, "boxColor", typeof(Vector4));
            headerHoverColor = (Vector4)safeLoad(cfgdata, "headerHoverColor", typeof(Vector4));
            espColor = (Vector4)safeLoad(cfgdata, "espColor", typeof(Vector4));
            healthBarColor = (Vector4)safeLoad(cfgdata, "healthBarColor", typeof(Vector4));
            healthTextColor = (Vector4)safeLoad(cfgdata, "healthTextColor", typeof(Vector4));
            directionTracerColor = (Vector4)safeLoad(cfgdata, "directionTracerColor", typeof(Vector4));
            playerNameTextColor = (Vector4)safeLoad(cfgdata, "playerNameTextColor", typeof(Vector4));
            aimLockedColor = (Vector4)safeLoad(cfgdata, "aimLockedColor", typeof(Vector4));
            closestColor = (Vector4)safeLoad(cfgdata, "closestColor", typeof(Vector4));
            boneColor = (Vector4)safeLoad(cfgdata, "boneColor", typeof(Vector4));

            AIMBOT_HOTKEY = (int)safeLoad(cfgdata, "AIMBOT_HOTKEY", typeof(int));
            ONLY_AIMBOT_HOTKEY = (int)safeLoad(cfgdata, "ONLY_AIMBOT_HOTKEY", typeof(int));
            BHOP_HOTKEY = (int)safeLoad(cfgdata, "BHOP_HOTKEY", typeof(int));
            FIRE_HOTKEY = (int)safeLoad(cfgdata, "FIRE_HOTKEY", typeof(int));
            JUMPSHOT_HOTKEY = (int)safeLoad(cfgdata, "JUMPSHOT_HOTKEY", typeof(int));
            MENU_HOTKEY = (int)safeLoad(cfgdata, "MENU_HOTKEY", typeof(int));
            SPINBOT_HOTKEY = (int)safeLoad(cfgdata, "SPINBOT_HOTKEY", typeof(int));
            TRIGGERBOT_HOTKEY = (int)safeLoad(cfgdata, "TRIGGERBOT_HOTKEY", typeof(int));
            CHATSPAM_HOTKEY = (int)safeLoad(cfgdata, "CHATSPAM_HOTKEY", typeof(int));
            shoot_delay = (int)safeLoad(cfgdata, "shoot_delay", typeof(int));
            rainbowSpeed = (int)safeLoad(cfgdata, "rainbowSpeed", typeof(int));
            aimbotspeed = (int)safeLoad(cfgdata, "aimbotspeed", typeof(int));
            customtheme = (int)safeLoad(cfgdata, "customtheme", typeof(int));
            currentConfig = (int)safeLoad(cfgdata, "currentConfig", typeof(int));

            instantShoot = (bool)safeLoad(cfgdata, "instantShoot", typeof(bool));
            teamCheck = (bool)safeLoad(cfgdata, "teamCheck", typeof(bool));
            esp = (bool)safeLoad(cfgdata, "esp", typeof(bool));
            healthBox = (bool)safeLoad(cfgdata, "healthBox", typeof(bool));
            healthText = (bool)safeLoad(cfgdata, "healthText", typeof(bool));
            directionTracer = (bool)safeLoad(cfgdata, "directionTracer", typeof(bool));
            playerNameText = (bool)safeLoad(cfgdata, "playerNameText", typeof(bool));
            boneEsp = (bool)safeLoad(cfgdata, "boneEsp", typeof(bool));
            aimbot = (bool)safeLoad(cfgdata, "aimbot", typeof(bool));
            onlyShootWhenSpotted = (bool)safeLoad(cfgdata, "onlyShootWhenSpotted", typeof(bool));
            bhop = (bool)safeLoad(cfgdata, "bhop", typeof(bool));
            jumpShotHack = (bool)safeLoad(cfgdata, "jumpShotHack", typeof(bool));
            rainbowMenu = (bool)safeLoad(cfgdata, "rainbowMenu", typeof(bool));
            simpleSpinBot = (bool)safeLoad(cfgdata, "simpleSpinBot", typeof(bool));
            counterStrafe = (bool)safeLoad(cfgdata, "counterStrafe", typeof(bool));
            onlyShootWhenStill = (bool)safeLoad(cfgdata, "onlyShootWhenStill", typeof(bool));
            extraCrosshair = (bool)safeLoad(cfgdata, "extraCrosshair", typeof(bool));
            allowWrite = (bool)safeLoad(cfgdata, "allowWrite", typeof(bool));
            antiFlash = (bool)safeLoad(cfgdata, "antiFlash", typeof(bool));
            boxEsp = (bool)safeLoad(cfgdata, "boxEsp", typeof(bool));
            velDisplay = (bool)safeLoad(cfgdata, "velDisplay", typeof(bool));
            showRecoil = (bool)safeLoad(cfgdata, "showRecoil", typeof(bool));
            rcs = (bool)safeLoad(cfgdata, "rcs", typeof(bool));
            disableAngleDiff = (bool)safeLoad(cfgdata, "disableAngleDiff", typeof(bool));
            radarHack = (bool)safeLoad(cfgdata, "radarHack", typeof(bool));

            espWidth = (float)safeLoad(cfgdata, "espWidth", typeof(float));
            maxAngleDiffAimbot = (float)safeLoad(cfgdata, "maxAngleDiffAimbot", typeof(float));
            crosshairSize = (float)safeLoad(cfgdata, "crosshairSize", typeof(float));

            File.WriteAllText($@"{getWhatsappPath()}\configs\current.op", name);
        }

        Vector4 strToVec4(string vec)
        {
            string v = vec.Replace("<", "");
            v = v.Replace(">", "");
            string[] values = v.Split(". ");
            List<float> floats = new List<float>();
            foreach (string val in values)
            {
                floats.Add(float.Parse(val));
            }
            return new Vector4(floats[0], floats[1], floats[2], floats[3]);
        }

        void updateConfigList()
        {
            DirectoryInfo d = new DirectoryInfo(@$"{getWhatsappPath()}\configs\");

            FileInfo[] Files = d.GetFiles("*.cfg");
            List<string> files = new List<string>();
            foreach (FileInfo info in Files)
            {
                files.Add(info.Name);
            }
            configs = files.ToArray();
        }

        void saveCurrentConfig()
        {
            string name = configs[currentConfig];
            string s = "";
            s += $"radarHack->{radarHack};";
            s += $"disableAngleDiff->{disableAngleDiff};";
            s += $"showRecoil->{showRecoil};";
            s += $"rcs->{rcs};";
            s += $"velDisplay->{velDisplay};";
            s += $"boxEsp->{boxEsp};";
            s += $"boxColor->{boxColor};";
            s += $"allowWrite->{allowWrite};";
            s += $"antiFlash->{antiFlash};";
            s += $"crosshairSize->{crosshairSize};";
            s += $"extraCrosshair->{extraCrosshair};";
            s += $"counterStrafe->{counterStrafe};";
            s += $"onlyShootWhenStill->{onlyShootWhenStill};";
            s += $"borderColor->{borderColor};";
            s += $"titleBgColor->{titleBgColor};";
            s += $"windowBg->{windowBg};";
            s += $"textColor->{textColor};";
            s += $"tabColor->{tabColor};";
            s += $"tabSelectedColor->{tabSelectedColor};";
            s += $"sliderGrabColor->{sliderGrabColor};";
            s += $"buttonColor->{buttonColor};";
            s += $"buttonClickedColor->{buttonClickedColor};";
            s += $"checkmarkColor->{checkmarkColor};";
            s += $"allBgColor->{allBgColor};";
            s += $"allBgHoverColor->{allBgHoverColor};";
            s += $"headerColor->{headerColor};";
            s += $"headerHoverColor->{headerHoverColor};";
            s += $"AIMBOT_HOTKEY->{AIMBOT_HOTKEY};";
            s += $"ONLY_AIMBOT_HOTKEY->{ONLY_AIMBOT_HOTKEY};";
            s += $"BHOP_HOTKEY->{BHOP_HOTKEY};";
            s += $"FIRE_HOTKEY->{FIRE_HOTKEY};";
            s += $"JUMPSHOT_HOTKEY->{JUMPSHOT_HOTKEY};";
            s += $"MENU_HOTKEY->{MENU_HOTKEY};";
            s += $"SPINBOT_HOTKEY->{SPINBOT_HOTKEY};";
            s += $"TRIGGERBOT_HOTKEY->{TRIGGERBOT_HOTKEY};";
            s += $"CHATSPAM_HOTKEY->{CHATSPAM_HOTKEY};";
            s += $"teamCheck->{teamCheck};";
            s += $"instantShoot->{instantShoot};";
            s += $"shoot_delay->{shoot_delay};";
            s += $"esp->{esp};";
            s += $"healthBox->{healthBox};";
            s += $"healthText->{healthText};";
            s += $"directionTracer->{directionTracer};";
            s += $"playerNameText->{playerNameText};";
            s += $"boneEsp->{boneEsp};";
            s += $"espColor->{espColor};";
            s += $"healthBarColor->{healthBarColor};";
            s += $"healthTextColor->{healthTextColor};";
            s += $"directionTracerColor->{directionTracerColor};";
            s += $"playerNameTextColor->{playerNameTextColor};";
            s += $"aimLockedColor->{aimLockedColor};";
            s += $"closestColor->{closestColor};";
            s += $"boneColor->{boneColor};";
            s += $"espWidth->{espWidth};";
            s += $"aimbot->{aimbot};";
            s += $"onlyShootWhenSpotted->{onlyShootWhenSpotted};";
            s += $"bhop->{bhop};";
            s += $"jumpShotHack->{jumpShotHack};";
            s += $"chatSpamText->{chatSpamText};";
            s += $"aimbotspeed->{aimbotspeed};";
            s += $"rainbowMenu->{rainbowMenu};";
            s += $"rainbowSpeed->{rainbowSpeed};";
            s += $"customtheme->{customtheme};";
            s += $"maxAngleDiffAimbot->{maxAngleDiffAimbot};";
            s += $"simpleSpinBot->{simpleSpinBot};";
            s += $"currentConfig->{currentConfig};";
            File.WriteAllText($@"{getWhatsappPath()}\\configs\\{name}", s);
        }

        void DrawOverlay()
        {
            ImGui.SetNextWindowSize(windowSize);
            ImGui.SetNextWindowPos(windowLocation);

            ImGui.Begin("inverto", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
            );
        }

        private void OpenUrl()
        {
            while (true)
            {
                if (url != "")
                {
                    try
                    {
                        Process.Start(url);
                    }
                    catch
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            url = url.Replace("&", "^&");
                            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            Process.Start("xdg-open", url);
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            Process.Start("open", url);
                        }
                        else
                        {
                            Console.WriteLine("URLERROR");
                        }
                    }
                    url = "";
                }
                Thread.Sleep(10);
            }
        }

        public void DrawMenu()
        {
            ImGui.Begin($"Inverto - Safe & Undetected External | UUID: {clientID}");

            if (ImGui.BeginTabBar("Tabs"))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    ImGui.LabelText("FPS: " + fps_value.ToString(), "");

                    ImGui.Checkbox("Aimbot", ref aimbot);
                    ImGui.SliderFloat("Max Range", ref maxAngleDiffAimbot, 10, 1000);
                    ImGui.Checkbox("Disable Range", ref disableAngleDiff);
                    ImGui.Checkbox("Triggerbot", ref instantShoot);
                    ImGui.Checkbox("Only Shoot when Still", ref onlyShootWhenStill);
                    ImGui.Checkbox("Counterstrafe", ref counterStrafe);
                    ImGui.Checkbox("Recoil Control", ref rcs);
                    ImGui.Checkbox("Bunnyhop", ref bhop);
                    ImGui.Checkbox("Jump Shot", ref jumpShotHack);
                    ImGui.Checkbox("Spinbot", ref simpleSpinBot);
                    ImGui.SliderInt("Shoot Delay (ms)", ref shoot_delay, 50, 1000);
                    ImGui.SliderInt("Aimbot Speed", ref aimbotspeed, 1000, 10000);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("ESP"))
                {
                    if (ImGui.BeginMenu("Wallhack"))
                    {
                        ImGui.Checkbox("Wallhack", ref esp);
                        ImGui.ColorPicker4("", ref espColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Closest Enemy"))
                    {
                        ImGui.ColorPicker4("", ref closestColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Aimlocked Enemy"))
                    {
                        ImGui.ColorPicker4("", ref aimLockedColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Health Box"))
                    {
                        ImGui.Checkbox("Health Box", ref healthBox);
                        ImGui.ColorPicker4("", ref healthBarColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Health Text"))
                    {
                        ImGui.Checkbox("Health Text", ref healthText);
                        ImGui.ColorPicker4("", ref healthTextColor);
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Player Names"))
                    {
                        ImGui.Checkbox("Player Names", ref playerNameText);
                        ImGui.ColorPicker4("", ref playerNameTextColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Direction Tracer"))
                    {
                        ImGui.Checkbox("Direction Tracer", ref directionTracer);
                        ImGui.ColorPicker4("", ref directionTracerColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Bones"))
                    {
                        ImGui.Checkbox("Bones", ref boneEsp);
                        ImGui.ColorPicker4("", ref boneColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("3D Hitbox"))
                    {
                        ImGui.Checkbox("3D Hitbox", ref boxEsp);
                        ImGui.ColorPicker4("", ref boxColor);
                        ImGui.EndMenu();
                    }
                    ImGui.Checkbox("Radar Hack", ref radarHack);
                    ImGui.SliderFloat("ESP Width", ref espWidth, 1, 10);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("HUD"))
                {
                    string[] themes = ["No Theme", "Dark Mode", "LSD", "Pink Glasses"];
                    ImGui.ListBox("Custom Themes", ref customtheme, themes, themes.Count());
                    ImGui.Text("");
                    ImGui.Text("");
                    ImGui.Checkbox("Crosshair", ref extraCrosshair);
                    ImGui.Text("");
                    ImGui.SliderFloat("Crosshair", ref crosshairSize, 1.5f, 25f);
                    ImGui.Text("");
                    ImGui.Text("");
                    ImGui.Checkbox("Velocity Display", ref velDisplay);
                    ImGui.Text("");
                    ImGui.Checkbox("Anti Flashbang", ref antiFlash);
                    ImGui.Text("");
                    ImGui.Checkbox("Show Recoil Points", ref showRecoil);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Window"))
                {
                    if (ImGui.Button("Update Window Data"))
                    {
                        winUpdate();
                    }
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Fun"))
                {
                    ImGui.Checkbox("Rainbow Menu", ref rainbowMenu);
                    ImGui.SliderInt("Rainbow Speed", ref rainbowSpeed, 1, 25);
                    ImGui.LabelText("", "");
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "Chat Spam:");
                    ImGui.InputTextMultiline("Press ^ (ingame) to spam", ref chatSpamText, 100, new Vector2(300, 100));

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Players"))
                {
                    var index = 1;
                    foreach (string player in playerNames)
                    {
                        if (ImGui.BeginMenu(player))
                        {
                            if (ImGui.Button("Open CSStats"))
                            {
                                currPlayerFaceitFinder = index;
                            }

                            ImGui.EndMenu();
                        }
                        index++;
                    }
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Config"))
                {
                    ImGui.ListBox("Configs", ref currentConfig, configs, configs.Count());
                    if (ImGui.Button("Save Config"))
                    {
                        saveCurrentConfig();
                        updateConfigList();
                    }
                    if (ImGui.Button("Load Config"))
                    {
                        updateConfigList();
                        loadConfig();
                    }
                    ImGui.Text("");
                    ImGui.InputTextMultiline("Cfg Name", ref newConfigName, 20, new Vector2(100, 20));
                    if (ImGui.Button("Create Config"))
                    {
                        createConfig(newConfigName);
                        newConfigName = "";
                        updateConfigList();
                    }
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Misc"))
                {
                    ImGui.Checkbox("Check Team?", ref teamCheck);
                    ImGui.TextColored(new Vector4(), "");
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "WARNING: This will potentially get you detected");
                    ImGui.Checkbox("Allow Write", ref allowWrite);
                    ImGui.TextColored(new Vector4(), "");
                    if (ImGui.Button("Exit Cheat (Panic)"))
                    {
                        Close();
                    }

                    /*
                    ImGui.Text("");

                    foreach(FileInfo file in new DirectoryInfo($@"{getWhatsappPath()}\maps").GetFiles("*.tri"))
                    {
                        if (ImGui.Button("Load "+file.Name.Split(".")[0]))
                        {
                            requestedMap = file.Name.Split(".")[0];
                        }
                    }
                    */

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Menu Colors"))
                {
                    if (ImGui.BeginMenu("Border"))
                    {
                        ImGui.ColorPicker4("", ref borderColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Title Background"))
                    {
                        ImGui.ColorPicker4("", ref titleBgColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Menu Background"))
                    {
                        ImGui.ColorPicker4("", ref windowBg);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Text"))
                    {
                        ImGui.ColorPicker4("", ref textColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Tab"))
                    {
                        ImGui.ColorPicker4("", ref tabColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Selected Tab"))
                    {
                        ImGui.ColorPicker4("", ref tabSelectedColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Slider"))
                    {
                        ImGui.ColorPicker4("", ref sliderGrabColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Button"))
                    {
                        ImGui.ColorPicker4("", ref buttonColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Clicked Button"))
                    {
                        ImGui.ColorPicker4("", ref buttonClickedColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Checkmark"))
                    {
                        ImGui.ColorPicker4("", ref checkmarkColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Input Backgrounds"))
                    {
                        ImGui.ColorPicker4("", ref allBgColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Active Input Backgrounds"))
                    {
                        ImGui.ColorPicker4("", ref allBgHoverColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Header"))
                    {
                        ImGui.ColorPicker4("", ref headerColor);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Active Header"))
                    {
                        ImGui.ColorPicker4("", ref headerHoverColor);
                        ImGui.EndMenu();
                    }
                    ImGui.EndTabItem();
                }

            }

            ImGui.EndTabBar();
            ImGui.End();
        }


        public static Vector4 Rainbow(float progress)
        {
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;

            switch ((int)div)
            {
                case 0:
                    return new Vector4(1, (float)ascending / 255, 0, 1);
                case 1:
                    return new Vector4((float)descending / 255, 1, 0, 1);
                case 2:
                    return new Vector4(0, 1, (float)ascending / 255, 1);
                case 3:
                    return new Vector4(0, (float)descending / 255, 1, 1);
                case 4:
                    return new Vector4((float)ascending / 255, 0, 1, 1);
                default:
                    return new Vector4(1, 0, (float)descending / 255, 1);
            }
        }

        void MenuColors()
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            style.WindowBorderSize = 2.0f;
            style.WindowRounding = 5.0f;
            style.Colors[(int)ImGuiCol.Border] = borderColor;
            style.Colors[(int)ImGuiCol.TitleBgActive] = titleBgColor;
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = titleBgColor;
            style.Colors[(int)ImGuiCol.WindowBg] = windowBg;
            style.WindowMinSize = new Vector2(15, 10);
            style.Colors[(int)ImGuiCol.Text] = textColor;
            style.Colors[(int)ImGuiCol.Tab] = tabColor;
            style.Colors[(int)ImGuiCol.TabHovered] = tabSelectedColor;
            style.Colors[(int)ImGuiCol.TabSelected] = tabSelectedColor;
            style.Colors[(int)ImGuiCol.SliderGrab] = sliderGrabColor;
            style.Colors[(int)ImGuiCol.SliderGrabActive] = sliderGrabColor;
            style.Colors[(int)ImGuiCol.Button] = buttonColor;
            style.Colors[(int)ImGuiCol.ButtonActive] = buttonClickedColor;
            style.Colors[(int)ImGuiCol.ButtonHovered] = buttonColor;
            style.Colors[(int)ImGuiCol.CheckMark] = checkmarkColor;
            style.Colors[(int)ImGuiCol.FrameBg] = allBgColor;
            style.Colors[(int)ImGuiCol.FrameBgActive] = allBgHoverColor;
            style.Colors[(int)ImGuiCol.FrameBgHovered] = allBgHoverColor;
            style.Colors[(int)ImGuiCol.Header] = headerColor;
            style.Colors[(int)ImGuiCol.HeaderActive] = headerHoverColor;
            style.Colors[(int)ImGuiCol.HeaderHovered] = headerHoverColor;

            if (rainbowMenu)
            {
                style.Colors[(int)ImGuiCol.Border] = Rainbow(rainbowProgress);
                style.Colors[(int)ImGuiCol.TitleBgActive] = Rainbow(rainbowProgress);
                style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Rainbow(rainbowProgress);
                style.Colors[(int)ImGuiCol.Tab] = Rainbow(rainbowProgress);
                style.Colors[(int)ImGuiCol.TabHovered] = Rainbow(rainbowProgress);
                style.Colors[(int)ImGuiCol.TabSelected] = Rainbow(rainbowProgress);
                rainbowProgress = rainbowProgress + 0.005f + (0.0025f * rainbowSpeed);
            }

            //menu on theme
            if (menuToggled)
            {
                switch (customtheme)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }
            }

        }

        void jumpShot()
        {
            IntPtr playerPawnAddress = localPlayer.address;

            uint fFlag = swed.ReadUInt(playerPawnAddress, offsets.jumpFlag);

            if (fFlag == INAIR && GetAsyncKeyState(JUMPSHOT_HOTKEY) < 0 && jumpShotHack && weaponName == "weapon_ssg08")
            {
                Thread.Sleep(100);
                var velocity = swed.ReadVec(playerPawnAddress, offsets.absVelocity);

                while (velocity.Z > 18f || velocity.Z < -18f)
                {
                    velocity = swed.ReadVec(playerPawnAddress, offsets.absVelocity);
                }

                shoot = true;
                Thread.Sleep(1000);
            }
            Thread.Sleep(2);
        }

        void Bhop()
        {
            while (true)
            {
                if (bhop && GetAsyncKeyState(BHOP_HOTKEY) < 0)
                {
                    IntPtr playerPawnAddress = localPlayer.address;
                    uint fFlag = swed.ReadUInt(playerPawnAddress, offsets.jumpFlag);

                    if (fFlag == STANDING)
                    {
                        mouse_event((long)MouseEventF.Wheel, 0, 0, -(120+rnd.NextInt64(50)), 0);
                        mouse_event((long)MouseEventF.Wheel, 0, 0, (120 + rnd.NextInt64(50)), 0);
                    }
                }
                Thread.Sleep(5);
            }
        }



        void Aimbot()
        {
            if (aimbot && entities.Count > 0)
            {
                if (autoAimWhenVisible && false)
                {
                    Entity? toAim = null;
                    foreach(Entity entity in entities)
                    {
                        if (entity.visible)
                        {
                            toAim = entity;
                            break;
                        }
                    }
                    if (toAim != null)
                    {
                        int entityindex = swed.ReadInt(localPlayer.address, offsets.IDEntIndex);
                        angles = CalcAngles(localPlayer.head, toAim.head);
                        if (recoilpoints.Count() > 0)
                            AimAt(new Vector3(angles.X - recoilpoints.Last().X / easingFactor, angles.Y + recoilpoints.Last().Y / easingFactor, angles.Z));
                        else
                            AimAt(angles);
                        float ownVel = Math.Abs(localPlayer.absVeloctiy.X) + Math.Abs(localPlayer.absVeloctiy.Y) + Math.Abs(localPlayer.absVeloctiy.Z);
                        if (instantShoot && entityindex != -1)
                        {
                            if ((localPlayer.absVeloctiy.Z > 1 || localPlayer.absVeloctiy.Z < -1) && weaponName == "weapon_ssg08")
                                return;

                            if (onlyShootWhenStill)
                            {
                                if (ownVel < 50)
                                {
                                    shoot = true;
                                }
                            }
                            else
                            {
                                shoot = true;
                            }
                        }
                    }
                }
                if (GetAsyncKeyState(ONLY_AIMBOT_HOTKEY) < 0)
                {
                    if (entities.Count > 0)
                    {
                        int entityindex = swed.ReadInt(localPlayer.address, offsets.IDEntIndex);
                        if ((entities[0].angleDiff < maxAngleDiffAimbot * (90 / fov)) || disableAngleDiff)
                        {
                            if (noAimWhenBehindWall && !entities[0].visible)
                                return;
                            angles = CalcAngles(localPlayer.head, entities[0].head);
                            if (recoilpoints.Count() > 0)
                                AimAt(new Vector3(angles.X - recoilpoints.Last().X / easingFactor, angles.Y + recoilpoints.Last().Y/easingFactor, angles.Z));
                            else
                                AimAt(angles);
                            float ownVel = Math.Abs(localPlayer.absVeloctiy.X) + Math.Abs(localPlayer.absVeloctiy.Y) + Math.Abs(localPlayer.absVeloctiy.Z);
                            if (instantShoot && entityindex != -1)
                            {
                                if ((localPlayer.absVeloctiy.Z > 1 || localPlayer.absVeloctiy.Z < -1) && weaponName == "weapon_ssg08")
                                    return;

                                if (onlyShootWhenStill)
                                {
                                    if (ownVel < 50)
                                    {
                                        shoot = true;
                                    }
                                }else
                                {
                                    shoot = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        float CalcPixelDist(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
        }

        Vector3 newAngles = new Vector3();
        Vector3 oldAngles = new Vector3();

        float m_pitch = 0.022f;
        float m_yaw = 0.022f;
        float sens = 2.0f;

        List<Vector3> recoilpoints = new List<Vector3>();
        Vector3 recoilreference = new Vector3();

        void calculateRecoilOffset()
        {
            int ShotsFired = swed.ReadInt(localPlayer.address, offsets.iShotsFired);

            if (ShotsFired > 1)
            {
                Vector3 aimPunch = swed.ReadVec(localPlayer.address, offsets.aimPunchAngle); ;
                newAngles.X = (aimPunch.Y - oldAngles.Y) * 2f / (m_pitch * sens) / 1;
                newAngles.Y = -(aimPunch.X - oldAngles.X) * 2f / (m_yaw * sens) / 1;

                if (newAngles != new Vector3())
                {
                    recoilpoints.Add(Vector3.Add(recoilreference, newAngles));
                    recoilreference = Vector3.Add(recoilreference, newAngles);
                }

                oldAngles = aimPunch;
            }
            else
            {
                recoilpoints.Clear();
                recoilreference = new Vector3();
                oldAngles = new Vector3(0, 0, 0);
            }
        }

        void MoveMouse(float x, float y)
        {
            float vel = 0;
            if (entities.Count() > 0)
            {
                vel = Math.Abs(entities[0].absVeloctiy.X) + Math.Abs(entities[0].absVeloctiy.Y) + Math.Abs(entities[0].absVeloctiy.Z);
            }
            float ownVel = Math.Abs(localPlayer.absVeloctiy.X) + Math.Abs(localPlayer.absVeloctiy.Y) + Math.Abs(localPlayer.absVeloctiy.Z);



            x = x / 360;
            y = y / 360;
            int speed = 4;
            int repeats = Convert.ToInt32(13090 * x);
            repeats = aimbotspeed + Convert.ToInt32(vel * 12) + Convert.ToInt32(ownVel * 12);
            if (repeats > aimbotspeed * 1.5) repeats = Convert.ToInt32(aimbotspeed * 1.5);
            float yPercentage = (x * y);
            int start = 0;
            POINT pt;

            Input[] inputs = new Input[]
                {
                    new Input
                    {
                        type = (int)Win32.InputType.Mouse,
                        u = new InputUnion
                        {
                            mi = new Win32.MouseInput
                            {
                                dx = -Convert.ToInt32(x*repeats),
                                dy = Convert.ToInt32(y*repeats),
                                dwFlags = (uint)(MouseEventF.Move),
                                dwExtraInfo = Win32.GetMessageExtraInfo()
                            }
                        }
                    }
                };

            Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        float oldMoveX = 0;

        void AimAt(Vector3 angles)
        {
            float moveY = angles.Y - swed.ReadFloat(client, offsets.viewangles);
            float moveX = angles.X - swed.ReadFloat(client, offsets.viewangles + 0x4);

            if (moveX > 70)
            {
                moveX = moveX - 360;
            }
            if (moveX < -70)
            {
                moveX = moveX + 360;
            }

            if (allowWrite)
            {
                swed.WriteFloat(client, offsets.viewangles + 0x4, angles.X);
                swed.WriteFloat(client, offsets.viewangles, angles.Y);
                return;
            }

            MoveMouse(moveX, moveY);
            oldMoveX = moveX;
        }

        Vector3 CalcAngles(Vector3 from, Vector3 to)
        {
            float yaw;
            float pitch;

            float deltaX = to.X - from.X;
            float deltaY = to.Y - from.Y;
            yaw = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);

            float deltaZ = to.Z - from.Z;
            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            pitch = -(float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

            return new Vector3(yaw, pitch, 0);
        }

        float CalcMagnitude(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
        }

        void EspOverlay()
        {
            GameOverlay.TimerService.EnableHighPrecisionTimers();

            espWin = new esp();
            espWin.Run();
        }

        void Esp()
        {

            Thread espOverlayThread = new Thread(EspOverlay) { IsBackground = true };
            espOverlayThread.Start();

            while (true)
            {
                espWin.currentTheme = customtheme;
                espWin.menuToggled = menuToggled;
                espWin.radarHack = radarHack;
                espWin.disableAngleDiff = disableAngleDiff;
                espWin.showRecoil = showRecoil;
                espWin.velDisplay = velDisplay;
                espWin.boxEsp = boxEsp;
                espWin.boxColor = boxColor;
                espWin.flashAlpha = flashAlpha;
                espWin.fov = fov;
                espWin.crosshairSize = crosshairSize;
                espWin.extraCrosshair = extraCrosshair;
                espWin.healthText = healthText;
                espWin.healthBox = healthBox;
                espWin.name = playerNameText;
                espWin.directionTracer = directionTracer;
                espWin.espColor = espColor;
                espWin.healthTextColor = healthTextColor;
                espWin.healthBarColor = healthBarColor;
                espWin.playerNameTextColor = playerNameTextColor;
                espWin.directionTracerColor = directionTracerColor;
                espWin.aimLockedColor = aimLockedColor;
                espWin.boneEsp = boneEsp;
                espWin.boneColor = boneColor;
                espWin.closestColor = closestColor;
                espWin.width = espWidth;
                espWin.meshList = meshList;
                espWin.maxAngleDiff = maxAngleDiffAimbot;
                espWin.teamCheck = teamCheck;
                espWin.localPlayerController = localPlayerController;
                espWin.espToggle = esp;
                espWin.offsets = offsets;
                espWin.aimbot = aimbot;
                Thread.Sleep(10);
            }
        }

        bool IsPixelInsideScreen(Vector2 pixel)
        {
            return pixel.X > windowLocation.X && pixel.X < windowLocation.X + windowSize.X && pixel.Y > windowLocation.Y && pixel.Y < windowLocation.Y + windowSize.Y;
        }

        public void winUpdate()
        {
            var window = GetWindowRect(swed.GetProcess().MainWindowHandle);
            windowLocation = new Vector2(window.left, window.top);
            windowSize = Vector2.Subtract(new Vector2(window.right, window.bottom), windowLocation);
            windowCenter = new Vector2(window.right - windowSize.X / 2, window.bottom - windowSize.Y / 2);
        }

        ViewMatrix ReadMatrix(IntPtr matrixAddress)
        {
            var viewMatrix = new ViewMatrix();
            var floatMatrix = swed.ReadMatrix(matrixAddress);

            viewMatrix.m11 = floatMatrix[0];
            viewMatrix.m12 = floatMatrix[1];
            viewMatrix.m13 = floatMatrix[2];
            viewMatrix.m14 = floatMatrix[3];
            viewMatrix.m21 = floatMatrix[4];
            viewMatrix.m22 = floatMatrix[5];
            viewMatrix.m23 = floatMatrix[6];
            viewMatrix.m24 = floatMatrix[7];
            viewMatrix.m31 = floatMatrix[8];
            viewMatrix.m32 = floatMatrix[9];
            viewMatrix.m33 = floatMatrix[10];
            viewMatrix.m34 = floatMatrix[11];
            viewMatrix.m41 = floatMatrix[12];
            viewMatrix.m42 = floatMatrix[13];
            viewMatrix.m43 = floatMatrix[14];
            viewMatrix.m44 = floatMatrix[15];

            return viewMatrix;
        }

        Vector2 WorldToScreen(ViewMatrix matrix, Vector3 pos, int w, int h)
        {
            Vector2 screenCoords = new Vector2();

            float screenW = (matrix.m41 * pos.X) + (matrix.m42 * pos.Y) + (matrix.m43 * pos.Z) + matrix.m44;

            if (screenW > 0.001f)
            {
                float screenX = (matrix.m11 * pos.X) + (matrix.m12 * pos.Y) + (matrix.m13 * pos.Z) + matrix.m14;
                float screenY = (matrix.m21 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m23 * pos.Z) + matrix.m24;

                float camX = w / 2;
                float camY = h / 2;

                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                screenCoords.X = X;
                screenCoords.Y = Y;
                return screenCoords;
            }
            else
            {
                return new Vector2(-99, -99);
            }
        }

        public static List<T> ReadStructsFromFile<T>(string filePath) where T : struct
        {
            List<T> list = new List<T>();
            int structSize = Marshal.SizeOf(typeof(T));

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[structSize];
                while (fs.Read(buffer, 0, structSize) == structSize)
                {
                    list.Add(ByteArrayToStructure<T>(buffer));
                }
            }

            return list;
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                handle.Free();
            }
        }

        public inverto.Utils.Triangle[] meshList = [];
        string currentMap = "";
        string requestedMap = "de_dust2";

        void updateMeshList(string mapName)
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, 1);

            string filePath = @$"{getWhatsappPath()}\maps\{mapName}.tri";
            List<inverto.Utils.Triangle> structs = ReadStructsFromFile<inverto.Utils.Triangle>(filePath);

            Console.WriteLine($"Parsed {structs.Count()} Triangles");

            meshList = structs.ToArray();
        }

        void ShootLogic()
        {
            while (true)
            {
                Thread.Sleep(3);
                if (shoot)
                {
                    int revolver = 0;
                    if (weaponName == "weapon_revolver")
                    {
                        revolver = 200;
                    }
                    Input[] inputs = new Input[]
                    {
                        new Input
                        {
                            type = (int)Win32.InputType.Mouse,
                            u = new InputUnion
                            {
                                mi = new Win32.MouseInput
                                {
                                    dx = 0,
                                    dy = 0,
                                    dwFlags = (uint)(MouseEventF.LeftDown),
                                    dwExtraInfo = Win32.GetMessageExtraInfo()
                                }
                            }
                        }
                    };

                    Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
                    Thread.Sleep(10 + rnd.Next(0, 10) + revolver);
                    inputs = new Input[]
                    {
                        new Input
                        {
                            type = (int)Win32.InputType.Mouse,
                            u = new InputUnion
                            {
                                mi = new Win32.MouseInput
                                {
                                    dx = 0,
                                    dy = 0,
                                    dwFlags = (uint)(MouseEventF.LeftUp),
                                    dwExtraInfo = Win32.GetMessageExtraInfo()
                                }
                            }
                        }
                    };

                    Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
                    float weaponShootDelay;
                    if (!shootDelay.TryGetValue(weaponName, out weaponShootDelay))
                    {
                        weaponShootDelay = shoot_delay;
                    }
                    Thread.Sleep((int)weaponShootDelay);
                    shoot = false;
                }
            }
        }

        void ReloadEntities()
        {
            entities.Clear();

            localPlayerController = swed.ReadPointer(client, offsets.localController);
            localPlayer.address = swed.ReadPointer(client, offsets.localPlayer);
            UpdateEntity(localPlayer);

            UpdateEntities();
        }

        void UpdateEntities()
        {
            IntPtr entityList = swed.ReadPointer(client, offsets.entityList);

            IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

            var tempPlayerNames = new List<string>();
            var tempSteamIDs = new List<string>();

            for (int i = 0; i < 64; i++)
            {
                IntPtr currController = swed.ReadPointer(listEntry, i * 0x78);
                if (currController == IntPtr.Zero) continue;

                int pawnHandle = swed.ReadInt(currController, offsets.playerpawn);
                if (pawnHandle == 0) continue;

                IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                if (listEntry2 == IntPtr.Zero) continue;

                IntPtr currPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
                if (currPawn == IntPtr.Zero) continue;

                Entity entity = new Entity();
                entity.address = currPawn;

                UpdateEntity(entity);

                ulong steamid = swed.ReadULong(currController, offsets.steamid);
                string name = swed.ReadString(currController + offsets.playerName, 128);

                entity.name = name;

                if (steamid != 0)
                {
                    tempSteamIDs.Add(steamid.ToString());
                    tempPlayerNames.Add(name.ToString());
                }

                if (entity.health < 1 || entity.health > 100)
                    continue;

                if (entity.origin.Equals(new Vector3(0f, 0f, 0f)))
                    continue;

                if (entity.teamNum == localPlayer.teamNum && teamCheck)
                    continue;

                if (entity.address == localPlayer.address)
                    continue;

                if (!entities.Any(element => element.origin == entity.origin))
                {
                    entity.visible = true;
                    /*
                    bool? f = null;
                    foreach (inverto.Utils.Triangle tri in meshList)
                    {
                        f = false;
                        if (tri.intersect(localPlayer.head, entity.head))
                        {
                            f = true;
                            break;
                        }
                    }
                    if (f != null)
                    {
                        if (!(bool)f)
                            entity.visible = true;
                    }
                    */
                    entities.Add(entity);
                }
            }

            playerNames = tempPlayerNames.ToArray();
            steamIDs = tempSteamIDs.ToArray();

            try
            {
                entities = entities.OrderBy(o => o.angleDiff).ToList();
            }
            catch (Exception ex)
            {

            }
        }

        void UpdateEntity(Entity entity)
        {

            entity.origin = swed.ReadVec(entity.address, offsets.origin);
            entity.viewOffset = new Vector3(0, 0, 65);
            entity.abs = Vector3.Add(entity.origin, entity.viewOffset);
            IntPtr sceneNode = swed.ReadPointer(entity.address, offsets.gameScene);
            IntPtr boneMatrix = swed.ReadPointer(sceneNode, offsets.modelState + offsets.boneArray);

            entity.head = swed.ReadVec(boneMatrix + 6 * 32);

            entity.angleEye = swed.ReadVec(entity.address, offsets.eyeAngles);

            var currentViewMatrix = ReadMatrix(client + offsets.viewmatrix);
            entity.headScreenPos = Vector2.Add(Vector2.Add(WorldToScreen(currentViewMatrix, entity.head, (int)windowSize.X, (int)windowSize.Y), windowLocation), new Vector2(recoilreference.X, recoilreference.Y+100));
            entity.originScreenPos = Vector2.Add(WorldToScreen(currentViewMatrix, entity.origin, (int)windowSize.X, (int)windowSize.Y), windowLocation);
            entity.absScreenPos = Vector2.Add(WorldToScreen(currentViewMatrix, entity.abs, (int)windowSize.X, (int)windowSize.Y), windowLocation);

            entity.health = swed.ReadInt(entity.address, offsets.health);
            entity.teamNum = swed.ReadInt(entity.address, offsets.teamNum);
            entity.absVeloctiy = swed.ReadVec(entity.address, offsets.absVelocity);
            entity.jumpFlag = 6555;
            entity.magnitude = CalcMagnitude(localPlayer.origin, entity.origin);
            entity.angleDiff = CalcPixelDist(windowCenter, entity.absScreenPos);
        }

        float fpsEsp = 0;

        void fps()
        {
            Thread.Sleep(1000);
            while (true)
            {
                fps_value = ImGui.GetFrameCount() - fps_offset;
                fps_offset = ImGui.GetFrameCount();
                Thread.Sleep(1000);
                fpsEsp = 0;
            }

        }

        bool spinBotToggleCD = false;

        void spinBot()
        {
            while (true)
            {
                if (GetAsyncKeyState(SPINBOT_HOTKEY) < 0 && !spinBotToggleCD)
                {
                    spinBotToggled = !spinBotToggled;
                    spinBotToggleCD = true;
                }
                else if (GetAsyncKeyState(SPINBOT_HOTKEY) >= 0 && spinBotToggleCD)
                {
                    spinBotToggleCD = false;
                }

                if (simpleSpinBot && spinBotToggled)
                {
                    try
                    {
                        MoveMouse(100, 0);
                    }
                    catch (Exception)
                    { }
                }
                Thread.Sleep(10);
            }
        }

        List<string> cheatList = new List<string>();

        bool spamHotkeyCooldown = false;
        bool spamToggle = false;

        void HoldKey(VK key)
        {
            Input[] inputs = new Input[]
                        {
                            new Input
                            {
                                type = (int)Win32.InputType.Keyboard,
                                u = new InputUnion
                                {
                                    ki = new Win32.KeyboardInput
                                    {
                                        wVk = (ushort) key,
                                    }
                                }
                            }
                        };

            Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        void ReleaseKey(VK key)
        {
            Input[] inputs = new Input[]
                        {
                            new Input
                            {
                                type = (int)Win32.InputType.Keyboard,
                                u = new InputUnion
                                {
                                    ki = new Win32.KeyboardInput
                                    {
                                        wVk = (ushort) key,
                                        dwFlags = 0x0002
                                    }
                                }
                            }
                        };

            Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        void PressKey(VK key, int delayInMilli)
        {
            HoldKey(key);
            Thread.Sleep(10);
            ReleaseKey(key);
            Thread.Sleep(delayInMilli);
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Helper
        {
            [FieldOffset(0)] public short Value;
            [FieldOffset(0)] public byte Low;
            [FieldOffset(1)] public byte High;
        }

        bool isPressed(VK key)
        {
            return GetAsyncKeyState((int)key) < 0;
        }

        void CounterStrafe()
        {
            while (true)
            {
                if (counterStrafe)
                {
                    IntPtr playerPawnAddress = localPlayer.address;
                    uint fFlag = swed.ReadUInt(playerPawnAddress, offsets.jumpFlag);

                    if (isPressed(VK.KEY_D))
                    {
                        int msPassed = 0;
                        while (isPressed(VK.KEY_D))
                        {
                            Thread.Sleep(3);
                            msPassed += 3;
                        }

                        if (msPassed > 100 && !(bhop && GetAsyncKeyState(BHOP_HOTKEY) < 0) && fFlag == STANDING)
                        {
                            HoldKey(VK.KEY_A);
                            Thread.Sleep(45);
                            ReleaseKey(VK.KEY_A);
                            Thread.Sleep(10);
                            HoldKey(VK.KEY_A);
                            Thread.Sleep(45);
                            ReleaseKey(VK.KEY_A);
                        }
                    }

                    if (isPressed(VK.KEY_A))
                    {
                        int msPassed = 0;
                        while (isPressed(VK.KEY_A))
                        {
                            Thread.Sleep(3);
                            msPassed += 3;
                        }

                        if (msPassed > 100 && !(bhop && GetAsyncKeyState(BHOP_HOTKEY) < 0) && fFlag == STANDING)
                        {
                            HoldKey(VK.KEY_D);
                            Thread.Sleep(45);
                            ReleaseKey(VK.KEY_D);
                            Thread.Sleep(10);
                            HoldKey(VK.KEY_D);
                            Thread.Sleep(45);
                            ReleaseKey(VK.KEY_D);
                        }
                    }
                }

                Thread.Sleep(3);
            }
        }

        Dictionary<string, float> shootDelay = new Dictionary<string, float>{
            {"weapon_ssg08", 700},
            {"weapon_usp_silencer", 350},
            {"weapon_ak47", 250},
            {"weapon_m4a1", 250},
            {"weapon_m4a1_silencer", 200},
            {"weapon_knife", 0},
            {"weapon_glock", 350},
            {"weapon_elite", 250},
            {"weapon_p250", 350},
            {"weapon_revolver", 100},
            {"weapon_deagle", 900},
            {"weapon_mp7", 200},
            {"weapon_mp9", 200},
            {"weapon_mac10", 200},
            {"weapon_famas", 200},
            {"weapon_galilar", 200},
            {"weapon_awp", 1750},
            {"weapon_scar20", 250},
            {"weapon_g3sg1", 250}
        };

        enum WeaponIds : short
        {
            WEAPON_DEAGLE = 1,
            WEAPON_ELITE,
            WEAPON_FIVESEVEN,
            WEAPON_GLOCK,
            WEAPON_AK47 = 7,
            WEAPON_AUG,
            WEAPON_AWP,
            WEAPON_FAMAS,
            WEAPON_G3SG1,
            WEAPON_GALILAR = 13,
            WEAPON_M249,
            WEAPON_M4A1 = 16,
            WEAPON_MAC10,
            WEAPON_P90 = 19,
            WEAPON_ZONE_REPULSOR,
            WEAPON_MP5SD = 23,
            WEAPON_UMP45,
            WEAPON_XM1014,
            WEAPON_BIZON,
            WEAPON_MAG7,
            WEAPON_NEGEV,
            WEAPON_SAWEDOFF,
            WEAPON_TEC9,
            WEAPON_TASER,
            WEAPON_HKP2000,
            WEAPON_MP7,
            WEAPON_MP9,
            WEAPON_NOVA,
            WEAPON_P250,
            WEAPON_SHIELD,
            WEAPON_SCAR20,
            WEAPON_SG556,
            WEAPON_SSG08,
            WEAPON_KNIFEGG,
            WEAPON_KNIFE,
            WEAPON_FLASHBANG,
            WEAPON_HEGRENADE,
            WEAPON_SMOKEGRENADE,
            WEAPON_MOLOTOV,
            WEAPON_DECOY,
            WEAPON_INCGRENADE,
            WEAPON_C4,
            ITEM_KEVLAR,
            ITEM_ASSAULTSUIT,
            ITEM_HEAVYASSAULTSUIT,
            ITEM_NVG = 54,
            ITEM_DEFUSER,
            ITEM_CUTTERS,
            WEAPON_HEALTHSHOT,
            WEAPON_KNIFE_T = 59,
            WEAPON_M4A1_SILENCER,
            WEAPON_USP_SILENCER,
            WEAPON_CZ75A = 63,
            WEAPON_REVOLVER,
            WEAPON_TAGRENADE = 68,
            WEAPON_FISTS,
            WEAPON_BREACHCHARGE,
            WEAPON_TABLET = 72,
            WEAPON_MELEE = 74,
            WEAPON_AXE,
            WEAPON_HAMMER,
            WEAPON_SPANNER = 78,
            WEAPON_KNIFE_GHOST = 80,
            WEAPON_FIREBOMB,
            WEAPON_DIVERSION,
            WEAPON_FRAG_GRENADE,
            WEAPON_SNOWBALL,
            WEAPON_BUMPMINE,
            WEAPON_KNIFE_BAYONET = 500,
            WEAPON_KNIFE_CSS = 503,
            WEAPON_KNIFE_FLIP = 505,
            WEAPON_KNIFE_GUT,
            WEAPON_KNIFE_KARAMBIT,
            WEAPON_KNIFE_M9_BAYONET,
            WEAPON_KNIFE_TACTICAL,
            WEAPON_KNIFE_FALCHION = 512,
            WEAPON_KNIFE_SURVIVAL_BOWIE = 514,
            WEAPON_KNIFE_BUTTERFLY,
            WEAPON_KNIFE_PUSH,
            WEAPON_KNIFE_CORD,
            WEAPON_KNIFE_CANIS,
            WEAPON_KNIFE_URSUS,
            WEAPON_KNIFE_GYPSY_JACKKNIFE,
            WEAPON_KNIFE_OUTDOOR,
            WEAPON_KNIFE_STILETTO,
            WEAPON_KNIFE_WIDOWMAKER,
            WEAPON_KNIFE_SKELETON = 525,
            WEAPON_KNIFE_KUKRI,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARF5 = 4613,
            CUSTOMPLAYER_CTM_ST6_VARIANTJ = 4619,
            CUSTOMPLAYER_CTM_ST6_VARIANTL = 4680,
            CUSTOMPLAYER_CTM_SWAT_VARIANTE = 4711,
            CUSTOMPLAYER_CTM_SWAT_VARIANTF,
            CUSTOMPLAYER_CTM_SWAT_VARIANTG,
            CUSTOMPLAYER_CTM_SWAT_VARIANTH,
            CUSTOMPLAYER_CTM_SWAT_VARIANTI,
            CUSTOMPLAYER_CTM_SWAT_VARIANTJ,
            CUSTOMPLAYER_TM_BALKAN_VARIANTK = 4718,
            STUDDED_BROKENFANG_GLOVES = 4725,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARF,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARG,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARH,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARJ = 4730,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARI = 4732,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARF1,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARF2,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARF3,
            CUSTOMPLAYER_TM_PROFESSIONAL_VARF4,
            CUSTOMPLAYER_CTM_GENDARMERIE_VARIANTA = 4749,
            CUSTOMPLAYER_CTM_GENDARMERIE_VARIANTB,
            CUSTOMPLAYER_CTM_GENDARMERIE_VARIANTC,
            CUSTOMPLAYER_CTM_GENDARMERIE_VARIANTD,
            CUSTOMPLAYER_CTM_GENDARMERIE_VARIANTE,
            CUSTOMPLAYER_CTM_SWAT_VARIANTK = 4756,
            CUSTOMPLAYER_CTM_DIVER_VARIANTA,
            CUSTOMPLAYER_CTM_DIVER_VARIANTB = 4771,
            CUSTOMPLAYER_CTM_DIVER_VARIANTC,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTA,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTB,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTC,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTD,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTE,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTF,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTB2 = 4780,
            CUSTOMPLAYER_TM_JUNGLE_RAIDER_VARIANTF2,
            STUDDED_BLOODHOUND_GLOVES = 5027,
            T_GLOVES,
            CT_GLOVES,
            SPORTY_GLOVES,
            SLICK_GLOVES,
            LEATHER_HANDWRAPS,
            MOTORCYCLE_GLOVES,
            SPECIALIST_GLOVES,
            STUDDED_HYDRA_GLOVES,
            CUSTOMPLAYER_T_MAP_BASED,
            CUSTOMPLAYER_CT_MAP_BASED,
            CUSTOMPLAYER_TM_ANARCHIST,
            CUSTOMPLAYER_TM_ANARCHIST_VARIANTA,
            CUSTOMPLAYER_TM_ANARCHIST_VARIANTB,
            CUSTOMPLAYER_TM_ANARCHIST_VARIANTC,
            CUSTOMPLAYER_TM_ANARCHIST_VARIANTD,
            CUSTOMPLAYER_TM_PIRATE,
            CUSTOMPLAYER_TM_PIRATE_VARIANTA,
            CUSTOMPLAYER_TM_PIRATE_VARIANTB,
            CUSTOMPLAYER_TM_PIRATE_VARIANTC,
            CUSTOMPLAYER_TM_PIRATE_VARIANTD,
            CUSTOMPLAYER_TM_PROFESSIONAL,
            CUSTOMPLAYER_TM_PROFESSIONAL_VAR1,
            CUSTOMPLAYER_TM_PROFESSIONAL_VAR2,
            CUSTOMPLAYER_TM_PROFESSIONAL_VAR3,
            CUSTOMPLAYER_TM_PROFESSIONAL_VAR4,
            CUSTOMPLAYER_TM_SEPARATIST,
            CUSTOMPLAYER_TM_SEPARATIST_VARIANTA,
            CUSTOMPLAYER_TM_SEPARATIST_VARIANTB,
            CUSTOMPLAYER_TM_SEPARATIST_VARIANTC,
            CUSTOMPLAYER_TM_SEPARATIST_VARIANTD,
            CUSTOMPLAYER_CTM_GIGN,
            CUSTOMPLAYER_CTM_GIGN_VARIANTA,
            CUSTOMPLAYER_CTM_GIGN_VARIANTB,
            CUSTOMPLAYER_CTM_GIGN_VARIANTC,
            CUSTOMPLAYER_CTM_GIGN_VARIANTD,
            CUSTOMPLAYER_CTM_GSG9,
            CUSTOMPLAYER_CTM_GSG9_VARIANTA,
            CUSTOMPLAYER_CTM_GSG9_VARIANTB,
            CUSTOMPLAYER_CTM_GSG9_VARIANTC,
            CUSTOMPLAYER_CTM_GSG9_VARIANTD,
            CUSTOMPLAYER_CTM_IDF,
            CUSTOMPLAYER_CTM_IDF_VARIANTB,
            CUSTOMPLAYER_CTM_IDF_VARIANTC,
            CUSTOMPLAYER_CTM_IDF_VARIANTD,
            CUSTOMPLAYER_CTM_IDF_VARIANTE,
            CUSTOMPLAYER_CTM_IDF_VARIANTF,
            CUSTOMPLAYER_CTM_SWAT,
            CUSTOMPLAYER_CTM_SWAT_VARIANTA,
            CUSTOMPLAYER_CTM_SWAT_VARIANTB,
            CUSTOMPLAYER_CTM_SWAT_VARIANTC,
            CUSTOMPLAYER_CTM_SWAT_VARIANTD,
            CUSTOMPLAYER_CTM_SAS_VARIANTA,
            CUSTOMPLAYER_CTM_SAS_VARIANTB,
            CUSTOMPLAYER_CTM_SAS_VARIANTC,
            CUSTOMPLAYER_CTM_SAS_VARIANTD,
            CUSTOMPLAYER_CTM_ST6,
            CUSTOMPLAYER_CTM_ST6_VARIANTA,
            CUSTOMPLAYER_CTM_ST6_VARIANTB,
            CUSTOMPLAYER_CTM_ST6_VARIANTC,
            CUSTOMPLAYER_CTM_ST6_VARIANTD,
            CUSTOMPLAYER_TM_BALKAN_VARIANTE,
            CUSTOMPLAYER_TM_BALKAN_VARIANTA,
            CUSTOMPLAYER_TM_BALKAN_VARIANTB,
            CUSTOMPLAYER_TM_BALKAN_VARIANTC,
            CUSTOMPLAYER_TM_BALKAN_VARIANTD,
            CUSTOMPLAYER_TM_JUMPSUIT_VARIANTA,
            CUSTOMPLAYER_TM_JUMPSUIT_VARIANTB,
            CUSTOMPLAYER_TM_JUMPSUIT_VARIANTC,
            CUSTOMPLAYER_TM_PHOENIX_HEAVY,
            CUSTOMPLAYER_CTM_HEAVY,
            CUSTOMPLAYER_TM_LEET_VARIANTA = 5100,
            CUSTOMPLAYER_TM_LEET_VARIANTB,
            CUSTOMPLAYER_TM_LEET_VARIANTC,
            CUSTOMPLAYER_TM_LEET_VARIANTD,
            CUSTOMPLAYER_TM_LEET_VARIANTE,
            CUSTOMPLAYER_TM_LEET_VARIANTG,
            CUSTOMPLAYER_TM_LEET_VARIANTH,
            CUSTOMPLAYER_TM_LEET_VARIANTI,
            CUSTOMPLAYER_TM_LEET_VARIANTF,
            CUSTOMPLAYER_TM_LEET_VARIANTJ,
            CUSTOMPLAYER_TM_PHOENIX = 5200,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTA,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTB,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTC,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTD,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTH,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTF,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTG,
            CUSTOMPLAYER_TM_PHOENIX_VARIANTI,
            CUSTOMPLAYER_CTM_FBI = 5300,
            CUSTOMPLAYER_CTM_FBI_VARIANTA,
            CUSTOMPLAYER_CTM_FBI_VARIANTC,
            CUSTOMPLAYER_CTM_FBI_VARIANTD,
            CUSTOMPLAYER_CTM_FBI_VARIANTE,
            CUSTOMPLAYER_CTM_FBI_VARIANTF,
            CUSTOMPLAYER_CTM_FBI_VARIANTG,
            CUSTOMPLAYER_CTM_FBI_VARIANTH,
            CUSTOMPLAYER_CTM_FBI_VARIANTB,
            CUSTOMPLAYER_CTM_ST6_VARIANTK = 5400,
            CUSTOMPLAYER_CTM_ST6_VARIANTE,
            CUSTOMPLAYER_CTM_ST6_VARIANTG,
            CUSTOMPLAYER_CTM_ST6_VARIANTM,
            CUSTOMPLAYER_CTM_ST6_VARIANTI,
            CUSTOMPLAYER_CTM_ST6_VARIANTN,
            CUSTOMPLAYER_TM_BALKAN_VARIANTF = 5500,
            CUSTOMPLAYER_TM_BALKAN_VARIANTI,
            CUSTOMPLAYER_TM_BALKAN_VARIANTG,
            CUSTOMPLAYER_TM_BALKAN_VARIANTJ,
            CUSTOMPLAYER_TM_BALKAN_VARIANTH,
            CUSTOMPLAYER_TM_BALKAN_VARIANTL,
            CUSTOMPLAYER_CTM_SAS = 5600,
            CUSTOMPLAYER_CTM_SAS_VARIANTF,
            CUSTOMPLAYER_CTM_SAS_VARIANTG
        };

        string getWeaponFromId(short id)
        {
            int ix = 0;
            foreach(short wpid in Enum.GetValues(typeof(WeaponIds))) {
                if (wpid == id)
                    return Enum.GetNames(typeof(WeaponIds))[ix].ToLower();
                ix++;
            }
            return "";
        }

        short weaponId = 0;
        string weaponName = "";

        void checkWeapon()
        {
            IntPtr clippingWeapon = swed.ReadPointer(localPlayer.address + offsets.clippingWeapon);
            short viewModelIndex = swed.ReadShort(clippingWeapon, offsets.AttributeManager + offsets.item + offsets.ItemDefinitionIndex);
            weaponId = viewModelIndex;
            weaponName = getWeaponFromId(viewModelIndex).ToLower();
        }

        float flashAlpha = 0f;

        void flashbang()
        {
            if (!antiFlash)
                return;

            float flash = swed.ReadFloat(localPlayer.address, 0x140C);
            flashAlpha = swed.ReadFloat(localPlayer.address, 0x1400);
            if (flash > 0 && allowWrite)
            {
                swed.WriteFloat(localPlayer.address, 0x140C, 0);
            }
        }

        uint fov = 90;

        void checkFov()
        {
            IntPtr cameraServices = swed.ReadPointer(localPlayer.address, offsets.camService);
            uint tempFov = swed.ReadUInt(cameraServices, offsets.fov);
            if (tempFov == 0) tempFov = 90;
            fov = tempFov;
        }

        public bool airStrafe = false;
        void doAirStrafe()
        {
            if (!airStrafe)
                return;

            if (isPressed((VK)BHOP_HOTKEY))
            {
                float x = 0;
                if (isPressed(VK.KEY_A))
                    x += 5;
                if (isPressed(VK.KEY_D))
                    x -= 5;

                MoveMouse(x, 0);
            }
        }

        void otherCheats()
        {
            while (true)
            {
                checkWeapon();
                jumpShot();
                flashbang();
                checkFov();
                calculateRecoilOffset();
                doAirStrafe();

                if (currPlayerFaceitFinder != 0)
                {
                    url = "https://csstats.gg/player/" + steamIDs[currPlayerFaceitFinder - 1].ToString();
                    currPlayerFaceitFinder = 0;
                    menuToggled = false;
                }

                if (requestedMap != currentMap)
                {
                    updateMeshList(requestedMap);
                    currentMap = requestedMap;
                }

                if (spamToggle)
                {
                    PressKey(VK.KEY_Z, 3);
                    foreach (char character in chatSpamText)
                    {
                        if (!spamToggle)
                        {
                            if (!spamHotkeyCooldown)
                            {
                                PressKey(VK.KEY_T, 0);
                                PressKey(VK.ESCAPE, 0);
                                spamHotkeyCooldown = true;
                            }
                            continue;
                        }
                        var helper = new Helper { Value = VkKeyScan(character) };

                        byte virtualKeyCode = helper.Low;
                        byte shiftState = helper.High;

                        VK key = (VK)virtualKeyCode;

                        if ((shiftState & 1) != 0)
                        {
                            PressKey(VK.CAPITAL, 0);
                            PressKey(key, 0);
                            PressKey(VK.CAPITAL, 0);
                        }
                        else
                        {
                            PressKey(key, 0);
                        }

                    }
                    PressKey(VK.RETURN, 0);
                }

                Thread.Sleep(3);
            }
        }

        void spamChatLogic()
        {
            while (true)
            {
                if (GetAsyncKeyState(CHATSPAM_HOTKEY) < 0 && !spamToggle && !menuToggled && chatSpamText != "")
                {
                    while (GetAsyncKeyState(CHATSPAM_HOTKEY) < 0)
                    {
                        Thread.Sleep(10);
                    }
                    spamToggle = true;
                    spamHotkeyCooldown = false;
                }
                else if (GetAsyncKeyState(CHATSPAM_HOTKEY) < 0 && spamToggle && !menuToggled && chatSpamText != "")
                {
                    spamToggle = false;
                    while (GetAsyncKeyState(CHATSPAM_HOTKEY) < 0)
                    {
                        Thread.Sleep(10);
                    }
                }
                Thread.Sleep(10);
            }
        }

        void menuLogic()
        {
            bool menuCooldown = false;
            while (true)
            {
                if (GetAsyncKeyState(MENU_HOTKEY) < 0 && !menuCooldown)
                {
                    menuToggled = !menuToggled;
                    if (menuToggled)
                    {
                        PressKey(VK.KEY_Z, 0);
                    }
                    else
                    {
                        Thread.Sleep(40 + rnd.Next(0, 10));
                        Input[] inputs = new Input[]
                        {
                        new Input
                        {
                            type = (int)Win32.InputType.Mouse,
                            u = new InputUnion
                            {
                                mi = new Win32.MouseInput
                                {
                                    dx = 0,
                                    dy = 0,
                                    dwFlags = (uint)(MouseEventF.RightDown),
                                    dwExtraInfo = Win32.GetMessageExtraInfo()
                                }
                            }
                        }
                    };

                        Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
                        Thread.Sleep(10 + rnd.Next(0, 10));
                        inputs = new Input[]
                        {
                        new Input
                        {
                            type = (int)Win32.InputType.Mouse,
                            u = new InputUnion
                            {
                                mi = new Win32.MouseInput
                                {
                                    dx = 0,
                                    dy = 0,
                                    dwFlags = (uint)(MouseEventF.RightUp),
                                    dwExtraInfo = Win32.GetMessageExtraInfo()
                                }
                            }
                        }
                        };

                        Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
                    }
                    menuCooldown = true;
                }
                else if (GetAsyncKeyState(MENU_HOTKEY) >= 0 && menuCooldown)
                {
                    menuCooldown = false;
                }
                Thread.Sleep(20);
            }
        }

        string get_subs(string full, string start, string end)
        {
            string s1 = full.Split(start)[1];
            return s1.Split(end)[0];
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start().Wait();

            Size size = program.Size;
            size.Width = (int)program.windowSize.X;
            size.Height = (int)program.windowSize.Y;
            program.Size = size;

            program.espWin = new esp();

            Thread mainLogicThread = new Thread(program.MainLogic) { IsBackground = true };
            mainLogicThread.Start();

            Thread shootlogicthread = new Thread(program.ShootLogic) { IsBackground = true };
            shootlogicthread.Start();

            Thread bhoplogicthread = new Thread(program.Bhop) { IsBackground = true };
            bhoplogicthread.Start();

            Thread esplogicthread = new Thread(program.Esp) { IsBackground = true };
            esplogicthread.Start();

            Thread winLogicThread = new Thread(program.fps) { IsBackground = true };
            winLogicThread.Start();

            Thread menuLogicThread = new Thread(program.menuLogic) { IsBackground = true };
            menuLogicThread.Start();

            Thread other = new Thread(program.otherCheats) { IsBackground = true };
            other.Start();

            Thread counterStrafeThread = new Thread(program.CounterStrafe) { IsBackground = true };
            counterStrafeThread.Start();

            Thread urlThread = new Thread(program.OpenUrl) { IsBackground = true };
            urlThread.Start();

            Thread spinBotThread = new Thread(program.spinBot) { IsBackground = true };
            spinBotThread.Start();

            Thread spamLogicThread = new Thread(program.spamChatLogic) { IsBackground = true };
            spamLogicThread.Start();
        }

        void MainLogic()
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, 0);

            var window = GetWindowRect(swed.GetProcess().MainWindowHandle);
            windowLocation = new Vector2(window.left, window.top);
            windowSize = Vector2.Subtract(new Vector2(window.right, window.bottom), windowLocation);
            windowCenter = new Vector2(window.right - windowSize.X / 2, window.bottom - windowSize.Y / 2);

            client = swed.GetModuleBase("client.dll");

            ReloadEntities();

            initOffsets();

            if (!checkConfig())
            {
                createConfig("default");
                currentConfig = 0;
            }

            updateConfigList();
            loadConfig();

            while (true)
            {
                ReloadEntities();

                if (aimbot)
                {
                    Aimbot();
                }

                Thread.Sleep(3);
            }
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        string off = "";
        string cli = "";
        string but = "";

        int Find(string text, int start, string[] masks, char? stopAt = null)
        {
            int offset = start;
            int index = 0;
            string orgtext = text;
            List<int> done = [];
            foreach (string mask in masks)
            {
                text = orgtext.Substring(offset);
                int c = 0;
                int currindex = offset;
                foreach (char ch in text)
                {
                    if (ch == stopAt)
                        return -1;

                    if (ch == mask[c])
                        c++;
                    else
                        c = 0;

                    if (c == mask.Length)
                    {
                        offset = currindex - mask.Length + 1;
                        done.Add(offset);
                        break;
                    }

                    currindex++;
                }
                index++;
            }

            if (done.Count() != masks.Count())
                return -1;

            return offset;
        }

        string getGithubFileContent(string url)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }

        void _updateParseOperations(string[] operations, string file)
        {
            Console.Clear();
            int step = operations.Count();
            string line = $"Offset Parser made by devskiddlee | {file}.cs => [{step} done]";
            Console.WriteLine(line);
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", line.Length)));
            int leng = 10;
            if (leng > step)
                leng = step;
            for (var i = 0; i < leng; i++)
                Console.WriteLine(operations[step - leng + i]);
        }

        string SafeSubstring(string text, int start, int length)
        {
            return text.Length <= start ? ""
                : text.Length - start <= length ? text.Substring(start)
                : text.Substring(start, length);
        }

        void parseOffsets()
        {
            string[] offsets = ["offsets.cs", "client_dll.cs", "buttons.cs"];
            List<string> offset_strings = [];

            foreach (string os in offsets)
            {
                offset_strings.Add(getGithubFileContent($"https://raw.githubusercontent.com/a2x/cs2-dumper/refs/heads/main/output/{os}"));
            }

            List<string> operations = [];
            int index = 0;
            foreach (string data in offset_strings)
            {
                if (offsets[index].Split("\\").Last().Split(".").First() == "buttons")
                {
                    int i = 0;
                    string s = "";
                    while (true)
                    {
                        i = Find(data, i + 6, [" nint "], '}');
                        string name = SafeSubstring(data, i + 6, 100).Split(" ").First();
                        int valIndex = Find(data, 0, ["Buttons", name, "0x"]);
                        if (valIndex < 0)
                            break;
                        string value = SafeSubstring(data, valIndex, 15).Split(";").First();
                        if (i < 0)
                            break;
                        s += $"{name}->{value};";
                        operations.Add($"{name}->{value}");
                        _updateParseOperations(operations.ToArray(), offsets[index].Split("\\").Last().Split(".").First());
                    }

                    File.WriteAllText($@"{getWhatsappPath()}\offsets\buttons.off", s);
                }

                if (offsets[index].Split("\\").Last().Split(".").First() == "offsets")
                {
                    int i = 0;
                    string s = "";
                    while (true)
                    {
                        i = Find(data, i + 6, [" nint "], '}');
                        string name = SafeSubstring(data, i + 6, 100).Split(" ").First();
                        int valIndex = Find(data, 0, ["ClientDll", name, "0x"]);
                        if (valIndex < 0)
                            break;
                        string value = SafeSubstring(data, valIndex, 15).Split(";").First();
                        if (i < 0)
                            break;
                        s += $"{name}->{value};";
                        operations.Add($"{name}->{value}");
                        _updateParseOperations(operations.ToArray(), offsets[index].Split("\\").Last().Split(".").First());
                    }

                    File.WriteAllText($@"{getWhatsappPath()}\offsets\offsets.off", s);
                }

                if (offsets[index].Split("\\").Last().Split(".").First() == "client_dll")
                {
                    int text = Find(data, 0, ["ClientDll"]);
                    int i = text + "ClientDll".Length;
                    List<string> classes = [];
                    List<int> clI = [];
                    while (true)
                    {
                        i = Find(data, i + "public static class ".Length, ["public static class "]);
                        string name = SafeSubstring(data, i + "public static class ".Length, 100).Split(" ").First();
                        if (i < 0)
                            break;
                        classes.Add(name);
                        clI.Add(i);
                    }

                    string s = "";
                    foreach (string cl in classes)
                    {
                        i = Find(data, i + 6, [$"public static class {cl} "]);
                        while (true)
                        {
                            i = Find(data, i + 6, [" nint "], '}');
                            string name = SafeSubstring(data, i + 6, 100).Split(" ").First();
                            int valIndex = Find(data, i + 6, ["0x"]);
                            if (valIndex < 0)
                                break;
                            string value = SafeSubstring(data, valIndex, 15).Split(";").First();
                            if (i < 0)
                                break;
                            s += $"{cl}->{name}->{value};";
                            operations.Add($"{cl}->{name}->{value}");
                            _updateParseOperations(operations.ToArray(), offsets[index].Split("\\").Last().Split(".").First());
                        }
                    }

                    File.WriteAllText($@"{getWhatsappPath()}\offsets\client_dll.off", s);
                }
                index++;
            }
        }

        void updateOffsets()
        {
            Console.WriteLine("Updating Offsets..");

            off = File.ReadAllText($@"{getWhatsappPath()}\offsets\offsets.off");
            cli = File.ReadAllText($@"{getWhatsappPath()}\offsets\client_dll.off");
            but = File.ReadAllText($@"{getWhatsappPath()}\offsets\buttons.off");

            offsets.entityList = getOffset("dwEntityList", off);
            offsets.localPlayer = getOffset("dwLocalPlayerPawn", off);
            offsets.localController = getOffset("dwLocalPlayerController", off);
            offsets.viewmatrix = getOffset("dwViewMatrix", off);
            offsets.viewangles = getOffset("dwViewAngles", off);
            offsets.gameRules = getOffset("dwGameRules", off);
            offsets.globalVars = getOffset("dwGlobalVars", off);
            offsets.playerpawn = getOffset("CCSPlayerController->m_hPlayerPawn", cli);

            offsets.eyeAngles = getOffset("C_CSPlayerPawnBase->m_angEyeAngles", cli);
            offsets.teamNum = getOffset("C_BaseEntity->m_iTeamNum", cli);
            offsets.jumpFlag = getOffset("C_BaseEntity->m_fFlags", cli);
            offsets.health = getOffset("C_BaseEntity->m_iHealth", cli);
            offsets.origin = getOffset("C_BasePlayerPawn->m_vOldOrigin", cli);
            offsets.weapon_services = getOffset("C_BasePlayerPawn->m_pWeaponServices", cli);
            offsets.m_pViewModelServices = getOffset("C_CSPlayerPawnBase->m_pViewModelServices", cli);
            offsets.m_hViewModel = getOffset("CCSPlayer_ViewModelServices->m_hViewModel", cli);
            offsets.m_nViewModelIndex = getOffset("C_BaseViewModel->m_nViewModelIndex", cli);
            offsets.modelState = getOffset("CSkeletonInstance->m_modelState", cli);
            offsets.gameScene = getOffset("C_BaseEntity->m_pGameSceneNode", cli);
            offsets.spottedState = getOffset("C_CSPlayerPawn->m_entitySpottedState", cli);
            offsets.lifeState = getOffset("C_BaseEntity->m_lifeState", cli);

            offsets.camService = getOffset("C_BasePlayerPawn->m_pCameraServices", cli);
            offsets.scoped = getOffset("C_CSPlayerPawn->m_bIsScoped", cli);
            offsets.fov = getOffset("CCSPlayerBase_CameraServices->m_iFOV", cli);
            offsets.absVelocity = getOffset("C_BaseEntity->m_vecAbsVelocity", cli);
            offsets.IDEntIndex = getOffset("C_CSPlayerPawnBase->m_iIDEntIndex", cli);

            offsets.playersAliveCT = getOffset("C_CSPlayerPawn->m_nLastKillerIndex", cli);
            offsets.playersAliveT = getOffset("C_CSPlayerPawn->m_flHitHeading", cli);
            offsets.m_hActiveWeapon = getOffset("CPlayer_WeaponServices->m_hActiveWeapon", cli);
            offsets.aimPunchAngle = getOffset("C_CSPlayerPawn->m_aimPunchAngle", cli);
            offsets.iShotsFired = getOffset("C_CSPlayerPawn->m_iShotsFired", cli);

            offsets.clippingWeapon = getOffset("C_CSPlayerPawnBase->m_pClippingWeapon", cli);
            offsets.m_iAmmoLastCheck = getOffset("C_CSWeaponBase->m_iAmmoLastCheck", cli);
            offsets.vOldOrigin = getOffset("C_BasePlayerPawn->m_vOldOrigin", cli);
            offsets.AttributeManager = getOffset("C_EconEntity->m_AttributeManager", cli);

            offsets.m_hController = getOffset("C_BasePlayerPawn->m_hController", cli);
            offsets.steamid = getOffset("CBasePlayerController->m_steamID", cli);
            offsets.playerName = getOffset("CBasePlayerController->m_iszPlayerName", cli);

            offsets.actionTrackingServices = getOffset("CCSPlayerController->m_pActionTrackingServices", cli);
            offsets.damageDealt = getOffset("CCSPlayerController_ActionTrackingServices->m_unTotalRoundDamageDealt", cli);
            offsets.m_bInReload = getOffset("C_CSWeaponBase->m_bInReload", cli);

            offsets.force_attack = getOffset("attack", but);
            offsets.force_jump = getOffset("jump", but);
            offsets.force_zoom = getOffset("zoom", but);

            Console.WriteLine("Offsets updated. -> " + getGithubFileContent("https://raw.githubusercontent.com/a2x/cs2-dumper/refs/heads/main/output/offsets.cs").ReplaceLineEndings("|").Split("|")[1]);
            File.WriteAllText(@$"{getWhatsappPath()}\\offsets\\last-updated.off", getGithubFileContent("https://raw.githubusercontent.com/a2x/cs2-dumper/refs/heads/main/output/offsets.cs").ReplaceLineEndings("|").Split("|")[1]);
        }

        string getLastUpdatedOffsets()
        {
            if (File.Exists(@$"{getWhatsappPath()}\\offsets\\last-updated.off"))
                return File.ReadAllText(@$"{getWhatsappPath()}\\offsets\\last-updated.off");
            return "";
        }

        string getWhatsappPath()
        {
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\whatsapp"))
            {
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\whatsapp");
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\whatsapp\\offsets");
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\whatsapp\\configs");
            }
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\whatsapp";
        }

        int getOffset(string off, string offsets)
        {
            try
            {
                string o = getBetween(offsets, off, ";");
                o = o.Substring(4, o.Length - 4);
                int result = Convert.ToInt32(o, 16);
                return result;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        void initOffsets()
        {
            if (getGithubFileContent("https://raw.githubusercontent.com/a2x/cs2-dumper/refs/heads/main/output/offsets.cs").ReplaceLineEndings("|").Split("|")[1]
                != getLastUpdatedOffsets())
            {
                IntPtr handle = GetConsoleWindow();
                ShowWindow(handle, 1);
                parseOffsets();
                updateOffsets();
                Console.Clear();
                Thread.Sleep(100);
                ShowWindow(handle, 0);
            }
            else
            {
                updateOffsets();
            }
        }

        bool checkConfig()
        {
            return File.Exists(@$"{getWhatsappPath()}\configs\current.op");
        }

        protected override void Render()
        {
            MenuColors();
            if (menuToggled)
            {
                DrawMenu();
                DrawOverlay();

                ImGui.End();
            }
            else
            {
                DrawOverlay();

                ImGui.End();
            }
        }
    }
}