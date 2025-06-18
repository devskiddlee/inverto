using ClickableTransparentOverlay.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace cs2Cheat
{
    public class ViewMatrix
    {
        public float m11, m12, m13, m14;
        public float m21, m22, m23, m24;
        public float m31, m32, m33, m34;
        public float m41, m42, m43, m44;

        public int AIMBOT_HOTKEY = 0x05;         //mouse4
        public int ONLY_AIMBOT_HOTKEY = 0x06;    //mouse5
        public int BHOP_HOTKEY = 0x12;           //alt
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

        public Vector4 espColor = new Vector4(1, 0, 0, 1);
        public Vector4 healthBarColor = new Vector4(0, 1, 0, 1);
        public Vector4 healthTextColor = new Vector4(0, 1, 0, 1);
        public Vector4 directionTracerColor = new Vector4(1, 1, 0, 1);
        public Vector4 playerNameTextColor = new Vector4(1, 1, 1, 1);
        public Vector4 aimLockedColor = new Vector4(0, 1, 0, 1);
        public Vector4 closestColor = new Vector4(0, 0, 1, 1);
        public Vector4 boneColor = new Vector4(1, 1, 1, 0.8f);

        public float espWidth = 1.5f;

        public bool aimbot_cd = false;
        public bool aimbot = true;
        public bool onlyShootWhenSpotted = true;
        public bool bhop = true;

        public bool jumpShotHack = false;
        public string chatSpamText = "";
        public int rainbowSpeed = 1;
        public int aimbotspeed = 3000;
        public bool rainbowMenu = false;
        public int customtheme = 0;
        public float maxAngleDiffAimbot = 100f;
        public bool simpleSpinBot = true;
        public int currentConfig = 0;
    }
}
