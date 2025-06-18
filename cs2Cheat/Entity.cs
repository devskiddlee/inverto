using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace cs2Cheat
{
    public enum bone_ids
    {
        BONE_HEAD = 6,
        BONE_NECK = 0,
        BONE_SPINE = 4,
        BONE_SPINE_1 = 2,
        BONE_HIP = 5,
        BONE_LEFT_SHOULDER = 8,
        BONE_LEFT_ARM = 9,
        BONE_LEFT_HAND = 10,
        BONE_RIGHT_SHOULDER = 13,
        BONE_RIGHT_ARM = 14,
        BONE_RIGHT_HAND = 15,

        BONE_LEFT_HIP = 22,
        BONE_LEFT_KNEE = 23,
        BONE_LEFT_FEET = 24,

        BONE_RIGHT_HIP = 25,
        BONE_RIGHT_KNEE = 26,
        BONE_RIGHT_FEET = 27,
    };

    public class Entity
    {
        public IntPtr address { get; set; }
        public int health { get; set; }
        public Vector3 origin { get; set; }
        public Vector3 head { get; set; }

        public Vector3 absVeloctiy { get; set; }

        public int teamNum { get; set; }
        public int jumpFlag { get; set; }
        public Vector3 abs { get; set; }
        public Vector2 originScreenPos { get; set; }
        public Vector2 absScreenPos { get; set; }

        public Vector3 viewOffset { get; set; }

        public Vector2 headScreenPos { get; set; }

        public Vector3 angleEye { get; set; }

        public bool visible { get; set; }

        public float magnitude { get; set; }

        public float angleDiff { get; set; }

        public int jumpButton { get; set; }

        public String name { get; set; }

        public List<Vector3> bone_pos { get; set; }

        public List<Vector2> bone_screen_pos { get; set; }

        public float dist { get; set; }

        public Vector2 pos_offset { get; set; }
    }
}
