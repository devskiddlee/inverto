using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

using GameOverlay.Drawing;
using GameOverlay.Windows;
using Swed64;
using Point = GameOverlay.Drawing.Point;
using Rectangle = GameOverlay.Drawing.Rectangle;
using SolidBrush = GameOverlay.Drawing.SolidBrush;

namespace cs2Cheat
{

    class ConvexHull
    {
        public static double cross(Vector2 O, Vector2 A, Vector2 B)
        {
            return (A.X - O.X) * (B.Y - O.Y) - (A.Y - O.Y) * (B.X - O.X);
        }

        public static List<Vector2> GetConvexHull(List<Vector2> points)
        {
            if (points == null)
                return null;

            if (points.Count() <= 1)
                return points;

            int n = points.Count(), k = 0;
            List<Vector2> H = new List<Vector2>(new Vector2[2 * n]);

            points.Sort((a, b) =>
                 a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));

            // Build lower hull
            for (int i = 0; i < n; ++i)
            {
                while (k >= 2 && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                    k--;
                H[k++] = points[i];
            }

            // Build upper hull
            for (int i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                    k--;
                H[k++] = points[i];
            }

            return H.Take(k - 1).ToList();
        }
    }

    public class esp : IDisposable
    {

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private readonly GraphicsWindow _window;

        private readonly Dictionary<string, GameOverlay.Drawing.SolidBrush> _brushes;
        private readonly Dictionary<string, GameOverlay.Drawing.Font> _fonts;
        private readonly Dictionary<string, GameOverlay.Drawing.Image> _images;

        Swed swed = new Swed("cs2");
        public Offsets offsets;
        IntPtr client;
        float headSize = 1000;
        float headSizeMult = 10;
        public Entity localPlayer = new Entity();
        public List<Entity> entities = new List<Entity>();
        public bool teamCheck = true;
        Program program;

        Vector2 windowLocation = new Vector2(0, 0);
        Vector2 windowSize = new Vector2(1920, 1080);
        Vector2 lineOrigin = new Vector2(1920 / 2, 1080);
        Vector2 windowCenter = new Vector2(1920 / 2, 1080 / 2);

        public IntPtr localPlayerController;

        public int fps_count = 0;
        public int fps = 0;
        public int currentTheme = 0;
        public float max_radar = 2292.5f;
        public bool extraCrosshair = true;
        public float crosshairSize = 5f;

        public bool espToggle = true;
        public bool name = true;
        public bool directionTracer = true;
        public bool healthBox = true;
        public bool boneEsp = true;
        public bool healthText = true;
        public bool boxEsp = true;
        public bool velDisplay = true;
        public bool showRecoil = true;
        public bool disableAngleDiff = true;
        public bool radarHack = true;
        public bool menuToggled = false;

        float lsdRainbowProgress = 0f;

        public uint fov = 90;

        public Vector4 espColor = new Vector4(1, 0, 0, 1);
        public Vector4 healthBarColor = new Vector4(0, 1, 0, 1);
        public Vector4 healthTextColor = new Vector4(0, 1, 0, 1);
        public Vector4 directionTracerColor = new Vector4(1, 1, 0, 1);
        public Vector4 playerNameTextColor = new Vector4(1, 1, 1, 1);
        public Vector4 aimLockedColor = new Vector4(0, 1, 0, 1);
        public Vector4 closestColor = new Vector4(0, 0, 1, 1);
        public Vector4 boneColor = new Vector4(1, 1, 1, 0.8f);
        public Vector4 boxColor = new Vector4(1, 0, 0, 0.2f);

        public float flashAlpha = 0;

        public Vector2[][] mesh2dList = [];


        public float width = 2f;
        public float maxAngleDiff = 100f;

        public bool aimbot = true;

        public static int Vector4ToArgb(Vector4 vector4)
        {
            int alpha = (int)(vector4.W * 255);
            int red = (int)(vector4.X * 255);
            int green = (int)(vector4.Y * 255);
            int blue = (int)(vector4.Z * 255);

            return (alpha << 24) | (red << 16) | (green << 8) | blue;
        }

        bool IsPixelInsideScreen(Vector2 pixel)
        {
            return pixel.X > windowLocation.X && pixel.X < windowLocation.X + windowSize.X && pixel.Y > windowLocation.Y && pixel.Y < windowLocation.Y + windowSize.Y;
        }

        public static List<Vector3> GetCuboidCorners(Vector3 start, Vector3 end, float width, float height)
        {
            Vector3 direction = end - start;
            float length = direction.Length();
            Vector3 forward = Vector3.Normalize(direction);

            // Choose a world up that is not parallel to forward
            Vector3 worldUp = new Vector3(0, 1, 0);
            if (Vector3.Dot(forward, worldUp) > 0.999f)
                worldUp = new Vector3(1, 0, 0);

            Vector3 right = Vector3.Normalize(Vector3.Cross(worldUp, forward));
            Vector3 up = Vector3.Normalize(Vector3.Cross(forward, right));

            float w = width / 2f;
            float h = height / 2f;

            // Near face (start)
            Vector3 p0 = start + (-w * right) + (-h * up); // bottom-left-near
            Vector3 p1 = start + (w * right) + (-h * up); // bottom-right-near
            Vector3 p2 = start + (w * right) + (h * up); // top-right-near
            Vector3 p3 = start + (-w * right) + (h * up); // top-left-near

            // Far face (end)
            Vector3 offset = forward * length;
            Vector3 p4 = p0 + offset; // bottom-left-far
            Vector3 p5 = p1 + offset; // bottom-right-far
            Vector3 p6 = p2 + offset; // top-right-far
            Vector3 p7 = p3 + offset; // top-left-far

            return new List<Vector3> { p0, p1, p2, p3, p4, p5, p6, p7 };
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

        Vector2 rotate_point(float cx, float cy, float angleInRads, Vector2 p)
        {
            double s = Math.Sin(angleInRads);
            double c = Math.Cos(angleInRads);

            p.X -= cx;
            p.Y -= cy;

            double xnew = p.X * c - p.Y* s;
            double ynew = p.X * s + p.Y * c;

            p.X = (float)xnew + cx;
            p.Y = (float)ynew + cy;

            return p;
        }

        float CalcPixelDist(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
        }

        float CalcMagnitude(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
        }

        void UpdateEntities()
        {
            IntPtr entityList = swed.ReadPointer(client, offsets.entityList);

            IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

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
                entity.dist = CalcMagnitude(new Vector3(localPlayer.origin.X, localPlayer.origin.Y, 0), new Vector3(entity.origin.X, entity.origin.Y, 0));

                entity.pos_offset = new Vector2(entity.origin.X - localPlayer.origin.X, entity.origin.Y - localPlayer.origin.Y);

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

            try
            {
                //entities.Remove(entities.Last());

                entities = entities.OrderBy(o => o.angleDiff).ToList();
            }
            catch (Exception ex)
            {

            }
        }

        float[] euler_to_quaternion(Vector3 r) {
            float yaw = r.Y;
            float pitch = r.X;
            float roll = r.Z;
            float qx = (float)(Math.Sin(roll / 2) * Math.Cos(pitch / 2) * Math.Cos(yaw / 2) - Math.Cos(roll / 2) * Math.Sin(pitch / 2) * Math.Sin(yaw / 2));
            float qy = (float)(Math.Cos(roll / 2) * Math.Sin(pitch / 2) * Math.Cos(yaw / 2) + Math.Sin(roll / 2) * Math.Cos(pitch / 2) * Math.Sin(yaw / 2));
            float qz = (float)(Math.Cos(roll / 2) * Math.Cos(pitch / 2) * Math.Sin(yaw / 2) - Math.Sin(roll / 2) * Math.Sin(pitch / 2) * Math.Cos(yaw / 2));
            float qw = (float)(Math.Cos(roll / 2) * Math.Cos(pitch / 2) * Math.Cos(yaw / 2) + Math.Sin(roll / 2) * Math.Sin(pitch / 2) * Math.Sin(yaw / 2));
            return [qx, qy, qz, qw];
        }

        float[] quaternion_mult(float[] q, float[] r) {
            return [r[0]* q[0]-r[1]* q[1]-r[2]* q[2]-r[3]* q[3],
                    r[0]*q[1]+r[1]* q[0]-r[2]* q[3]+r[3]* q[2],
                    r[0]*q[2]+r[1]* q[3]+r[2]* q[0]-r[3]* q[1],
                    r[0]*q[3]-r[1]* q[2]+r[2]* q[1]+r[3]* q[0]];
        }

        Vector3 point_rotation_by_quaternion(Vector3 point, float[] q) {
            float[] r = [0, point.X, point.Y, point.Z];
            float[] q_conj = [q[0], -1 * q[1], -1 * q[2], -1 * q[3]];
            float[] new_q = quaternion_mult(quaternion_mult(q, r), q_conj);
            return new Vector3(new_q[1], new_q[2], new_q[3]);
        }

        void UpdateEntity(Entity entity)
        {

            entity.origin = swed.ReadVec(entity.address, offsets.origin);
            entity.viewOffset = new Vector3(0, 0, 65);
            entity.abs = Vector3.Add(entity.origin, entity.viewOffset);
            IntPtr sceneNode = swed.ReadPointer(entity.address, offsets.gameScene);
            IntPtr boneMatrix = swed.ReadPointer(sceneNode, offsets.modelState + offsets.boneArray);
            var currentViewMatrix = ReadMatrix(client + offsets.viewmatrix);

            entity.bone_pos = new List<Vector3>();
            entity.bone_screen_pos = new List<Vector2>();
            foreach (int bone in Enum.GetValues(typeof(bone_ids)))
            {
                Vector3 bone_pos = swed.ReadVec(boneMatrix + bone * 32);
                entity.bone_pos.Add(bone_pos);
                entity.bone_screen_pos.Add(WorldToScreen(currentViewMatrix, bone_pos, (int)windowSize.X, (int)windowSize.Y));
            }

            entity.head = swed.ReadVec(boneMatrix + 6 * 32);
            entity.angleEye = swed.ReadVec(entity.address, offsets.eyeAngles);

            entity.headScreenPos = Vector2.Add(WorldToScreen(currentViewMatrix, entity.head, (int)windowSize.X, (int)windowSize.Y), windowLocation);
            entity.originScreenPos = Vector2.Add(WorldToScreen(currentViewMatrix, entity.origin, (int)windowSize.X, (int)windowSize.Y), windowLocation);
            entity.absScreenPos = Vector2.Add(WorldToScreen(currentViewMatrix, entity.abs, (int)windowSize.X, (int)windowSize.Y), windowLocation);

            entity.health = swed.ReadInt(entity.address, offsets.health);
            entity.teamNum = swed.ReadInt(entity.address, offsets.teamNum);
            entity.absVeloctiy = swed.ReadVec(entity.address, offsets.absVelocity);
            entity.jumpFlag = 6555;
            entity.magnitude = CalcMagnitude(localPlayer.origin, entity.origin);
            entity.angleDiff = CalcPixelDist(windowCenter, entity.absScreenPos);
        }

        void ReloadEntities()
        {
            entities.Clear();

            localPlayer.address = swed.ReadPointer(client, offsets.localPlayer);
            UpdateEntity(localPlayer);

            UpdateEntities();
        }

        public esp()
        {
            _brushes = new Dictionary<string, GameOverlay.Drawing.SolidBrush>();
            _fonts = new Dictionary<string, GameOverlay.Drawing.Font>();
            _images = new Dictionary<string, GameOverlay.Drawing.Image>();

            var gfx = new GameOverlay.Drawing.Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true
            };

            _window = new GraphicsWindow(0, 0, 1920, 1080, gfx)
            {
                FPS = 120,
                IsTopmost = true,
                IsVisible = true
            };

            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
            _window.SetupGraphics += _window_SetupGraphics;
        }

        SolidBrush solidBrushFromVec4(GameOverlay.Drawing.Graphics gfx, Vector4 cl)
        {
            return gfx.CreateSolidBrush(GameOverlay.Drawing.Color.FromARGB(Vector4ToArgb(cl)));
        }

        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            if (e.RecreateResources)
            {
                foreach (var pair in _brushes) pair.Value.Dispose();
                foreach (var pair in _images) pair.Value.Dispose();
            }

            _brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
            _brushes["black"] = gfx.CreateSolidBrush(0, 0, 0, 100);
            _brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
            _brushes["darkred"] = gfx.CreateSolidBrush(155, 0, 0);
            _brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
            _brushes["background"] = gfx.CreateSolidBrush(0, 0, 0, 0);

            if (e.RecreateResources) return;

            _fonts["arial"] = gfx.CreateFont("Arial", 12);
            _fonts["consolas"] = gfx.CreateFont("Consolas", 14);
        }

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in _brushes) pair.Value.Dispose();
            foreach (var pair in _fonts) pair.Value.Dispose();
            foreach (var pair in _images) pair.Value.Dispose();
        }

        void draw3dBoxAroundLine(Graphics gfx, ViewMatrix currentViewMatrix, Vector3 start, Vector3 end, double cos, double sin, float size, bool outline = true, bool straight = true)
        {
            float x_offset = end.X - start.X;
            float y_offset = end.Y - start.Y;

            Vector3 v1 = Vector3.Add(start, new Vector3((float)(-size * cos - -size * sin), (float)(-size * sin + -size * cos), 0));
            Vector3 v2 = Vector3.Add(start, new Vector3((float)(size * cos - -size * sin), (float)(size * sin + -size * cos), 0));
            Vector3 v3 = Vector3.Add(start, new Vector3((float)(-size * cos - size * sin), (float)(-size * sin + size * cos), 0));
            Vector3 v4 = Vector3.Add(start, new Vector3((float)(size * cos - size * sin), (float)(size * sin + size * cos), 0));

            Vector3 v5 = new Vector3(v1.X, v1.Y, end.Z);
            Vector3 v6 = new Vector3(v2.X, v2.Y, end.Z);
            Vector3 v7 = new Vector3(v3.X, v3.Y, end.Z);
            Vector3 v8 = new Vector3(v4.X, v4.Y, end.Z);

            if (!straight)
            {
                v5 = Vector3.Add(v5, new Vector3(x_offset, y_offset, 0));
                v6 = Vector3.Add(v6, new Vector3(x_offset, y_offset, 0));
                v7 = Vector3.Add(v7, new Vector3(x_offset, y_offset, 0));
                v8 = Vector3.Add(v8, new Vector3(x_offset, y_offset, 0));
            }

            Vector2 p1 = WorldToScreen(currentViewMatrix, v1, (int)windowSize.X, (int)windowSize.Y);
            Vector2 p2 = WorldToScreen(currentViewMatrix, v2, (int)windowSize.X, (int)windowSize.Y);
            Vector2 p3 = WorldToScreen(currentViewMatrix, v3, (int)windowSize.X, (int)windowSize.Y);
            Vector2 p4 = WorldToScreen(currentViewMatrix, v4, (int)windowSize.X, (int)windowSize.Y);

            Vector2 p5 = WorldToScreen(currentViewMatrix, v5, (int)windowSize.X, (int)windowSize.Y);
            Vector2 p6 = WorldToScreen(currentViewMatrix, v6, (int)windowSize.X, (int)windowSize.Y);
            Vector2 p7 = WorldToScreen(currentViewMatrix, v7, (int)windowSize.X, (int)windowSize.Y);
            Vector2 p8 = WorldToScreen(currentViewMatrix, v8, (int)windowSize.X, (int)windowSize.Y);

            Vector3[] vectors = [v1, v2, v3, v4, v5, v6, v7, v8];
            Vector2[] points = [p1, p2, p3, p4, p5, p6, p7, p8];

            var color = _brushes["box"];

            bool valid = true;

            foreach (Vector2 pt in points)
                if (!IsPixelInsideScreen(pt))
                    valid = false;

            if (!valid)
                return;

            //feet
            gfx.FillTriangle(color,
                p1.X, p1.Y,
                p2.X, p2.Y,
                p3.X, p3.Y
                );
            gfx.FillTriangle(color,
                p4.X, p4.Y,
                p2.X, p2.Y,
                p3.X, p3.Y
                );

            //head
            gfx.FillTriangle(color,
                p5.X, p5.Y,
                p6.X, p6.Y,
                p7.X, p7.Y
                );
            gfx.FillTriangle(color,
                p8.X, p8.Y,
                p6.X, p6.Y,
                p7.X, p7.Y
                );

            //side north
            gfx.FillTriangle(color,
                p3.X, p3.Y,
                p4.X, p4.Y,
                p8.X, p8.Y
                );
            gfx.FillTriangle(color,
                p3.X, p3.Y,
                p7.X, p7.Y,
                p8.X, p8.Y
                );

            //side south
            gfx.FillTriangle(color,
                p1.X, p1.Y,
                p2.X, p2.Y,
                p6.X, p6.Y
                );
            gfx.FillTriangle(color,
                p1.X, p1.Y,
                p5.X, p5.Y,
                p6.X, p6.Y
                );

            //side east
            gfx.FillTriangle(color,
                p2.X, p2.Y,
                p4.X, p4.Y,
                p8.X, p8.Y
                );
            gfx.FillTriangle(color,
                p2.X, p2.Y,
                p6.X, p6.Y,
                p8.X, p8.Y
                );

            //side west
            gfx.FillTriangle(color,
                p1.X, p1.Y,
                p3.X, p3.Y,
                p7.X, p7.Y
                );
            gfx.FillTriangle(color,
                p1.X, p1.Y,
                p5.X, p5.Y,
                p7.X, p7.Y
                );

            if (outline)
            {
                color = _brushes["white"];

                Vector2[] o_points = ConvexHull.GetConvexHull(points.ToList()).ToArray();

                Vector2? last_point = null;
                foreach(Vector2 pt in o_points)
                {
                    if (last_point == null)
                    {
                        last_point = pt;
                        continue;
                    }

                    gfx.DrawLine(color, last_point.Value.X, last_point.Value.Y, pt.X, pt.Y, width);
                    last_point = pt;
                }
                gfx.DrawLine(color, last_point.Value.X, last_point.Value.Y, o_points[0].X, o_points[0].Y, width);
            }
        }

        Vector3 newAngles = new Vector3();
        Vector3 oldAngles = new Vector3();

        float m_pitch = 0.022f;
        float m_yaw = 0.022f;
        float sens = 2.0f;

        List<Vector3> recoilpoints = new List<Vector3>();
        Vector3 recoilreference = new Vector3();

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

        static string getWhatsappPathStatic()
        {
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\whatsapp";
        }

        public inverto.Utils.Triangle[] meshList = [];

        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            calculateRecoilOffset();

            _brushes["normalColor"] =           solidBrushFromVec4(gfx, espColor);
            _brushes["directionCrosshair"] =    solidBrushFromVec4(gfx, directionTracerColor);
            _brushes["healthBar"] =             solidBrushFromVec4(gfx, healthBarColor);
            _brushes["healthTextC"] =           solidBrushFromVec4(gfx, healthTextColor);
            _brushes["playerText"] =            solidBrushFromVec4(gfx, playerNameTextColor);
            _brushes["closestEnemy"] =          solidBrushFromVec4(gfx, closestColor);
            _brushes["aimLockedEnemy"] =        solidBrushFromVec4(gfx, aimLockedColor);
            _brushes["bone"] =                  solidBrushFromVec4(gfx, boneColor);
            _brushes["box"] =                   solidBrushFromVec4(gfx, boxColor);

            Random rnd = new Random();

            var padding = 5;
            var infoText = new StringBuilder()
                .Append("FPS: ").Append(gfx.FPS.ToString().PadRight(padding))
                .Append("FrameCount: ").Append(e.FrameCount.ToString().PadRight(padding))
                .Append("DeltaTime: ").Append(e.DeltaTime.ToString().PadRight(padding))
                .ToString();

            gfx.ClearScene(_brushes["background"]);

            if (flashAlpha > 0)
            {
                gfx.ClearScene(solidBrushFromVec4(gfx, new Vector4(0.05f, 0.05f, 0.05f, flashAlpha)));
            }

            if (aimbot && !disableAngleDiff)
                gfx.DrawCircle(_brushes["white"], new Circle(
                    windowCenter.X,
                    windowCenter.Y,
                    maxAngleDiff * (90 / fov)), width);

            var currentViewMatrix = ReadMatrix(client + offsets.viewmatrix);

            ReloadEntities();


            List<Vector2> list = new List<Vector2>();
            List<float> hS = new List<float>();
            List<Vector2> c_list = new List<Vector2>();
            List<float> c_hS = new List<float>();
            List<string> names = new List<string>();
            List<Vector2> headLoc = new List<Vector2>();
            List<float> angleDiffs = new List<float>();
            List<int> healthList = new List<int>();
            List<Vector2> feetList = new List<Vector2>();
            List<List<Vector2>> bonesList = new List<List<Vector2>>();
            List<Vector2> offsetList = new List<Vector2>();
            List<float> distList = new List<float>();
            List<List<bone_ids>> boneIDs = new List<List<bone_ids>>();
            List<Vector3> origins = new List<Vector3>();
            List<Vector3> heads = new List<Vector3>();
            List<List<Vector3>> bones3d = new List<List<Vector3>>();
            List<Vector3> angles = new List<Vector3>();
            List<bool> visibleList = new List<bool>();
            bool isInReach = false;
            foreach (var entity in entities)
            {
                try
                {
                    float hSTemp = headSize * headSizeMult / CalcMagnitude(localPlayer.origin, entity.origin);
                    float c_hSTemp = hSTemp / 2;
                    float x = entity.headScreenPos.X;
                    float y = entity.headScreenPos.Y;
                    headLoc.Add(new Vector2(x, y));
                    names.Add(entity.name);
                    list.Add(new Vector2(x - hSTemp / 2, y - hSTemp / 2));
                    c_list.Add(new Vector2(x, y));
                    hS.Add(hSTemp);
                    c_hS.Add(c_hSTemp);
                    angleDiffs.Add(entity.angleDiff);
                    healthList.Add(entity.health);
                    feetList.Add(entity.originScreenPos);
                    bonesList.Add(entity.bone_screen_pos);
                    origins.Add(entity.origin);
                    heads.Add(entity.head);
                    angles.Add(entity.angleEye);
                    bones3d.Add(entity.bone_pos);
                    visibleList.Add(entity.visible);
                    var tempBoneIds = new List<bone_ids>();
                    foreach (bone_ids bone in Enum.GetValues(typeof(bone_ids)))
                    {
                        tempBoneIds.Add(bone);
                    }
                    boneIDs.Add(tempBoneIds);
                    offsetList.Add(entity.pos_offset);
                    distList.Add(entity.dist);
                }
                catch (Exception ex)
                {
                    list.Add(new Vector2(-1000, -1000));
                    hS.Add(1);
                    Console.WriteLine("Error Drawing ESP");
                }
            }

            int index = 0;
            foreach (var vec in headLoc)
            {
                if (!directionTracer)
                    break;

                var color = _brushes["directionCrosshair"];
                if (index == 0 && isInReach)
                    color = _brushes["closestEnemy"];
                if (index == 0 && isInReach && (GetAsyncKeyState(program.AIMBOT_HOTKEY) < 0 || GetAsyncKeyState(program.ONLY_AIMBOT_HOTKEY) < 0))
                    color = _brushes["aimLockedEnemy"];

                double sin = vec.Y - windowCenter.Y;
                double cos = vec.X - windowCenter.X;
                double dist = Math.Sqrt(cos * cos + sin * sin);
                sin = sin / dist;
                cos = cos / dist;

                float angle = (float)Math.Atan2(sin, cos);

                int sx = (int)(cos * 10);
                int sy = (int)(sin * 10);

                int ex = (int)(cos * 20);
                int ey = (int)(sin * 20);

                if (dist > 20 && IsPixelInsideScreen(vec))
                {
                    gfx.DrawLine(color, new Line(
                        new Point(
                            windowCenter.X + sx,
                            windowCenter.Y + sy),
                        new Point(
                            windowCenter.X + ex,
                            windowCenter.Y + ey)
                        ), width);
                }

                index++;
            }

            index = 0;
            foreach (Vector2 offset in offsetList)
            {
                if (!radarHack)
                    break;

                Vector3 a = CalcAngles(new Vector3(localPlayer.pos_offset.X, localPlayer.pos_offset.Y, 0), new Vector3(offset.X, offset.Y, 0));
                double Aangle = (double)swed.ReadFloat(client, offsets.viewangles + 0x4) - a.X + 90;
                double Asin = -Math.Sin(Aangle / 180 * Math.PI);
                double Acos = -Math.Cos(Aangle / 180 * Math.PI);
                int x = 125 + (int)(distList[index] / max_radar * 125 * Acos);
                int y = 125 + (int)(distList[index] / max_radar * 125 * Asin);

                if (x > 250) x = 250;
                if (x < 0) x = 0;
                if (y > 250) y = 250;
                if (y < 0) y = 0;

                gfx.DrawCircle(_brushes["darkred"], new Circle(25 + x - 2, 25 + y - 2, 5), width);
                gfx.FillCircle(_brushes["red"], new Circle(25 + x - 2, 25 + y - 2, 5));
                index++;
            }

            if (espToggle)
            {
                foreach (Vector2[] mesh in mesh2dList)
                {
                    Vector2 old_pos = new Vector2(0, 0);
                    foreach (Vector2 pt in mesh)
                    {
                        if (old_pos != new Vector2(0, 0))
                        {
                            gfx.DrawLine(_brushes["white"], pt.X, pt.Y, old_pos.X, old_pos.Y, width);
                        }
                        gfx.DrawCircle(_brushes["white"], pt.X, pt.Y, 3, width);
                        old_pos = pt;
                    }
                }

                if (entities.Count > 0)
                {
                    index = 0;
                    SolidBrush color;
                    if (entities[0].angleDiff <= maxAngleDiff)
                        isInReach = true;
                    foreach (var vec in list)
                    {
                        color = _brushes["normalColor"];
                        if (index == 0 && isInReach)
                            color = _brushes["closestEnemy"];
                        gfx.DrawRectangle(color, new Rectangle(vec.X, vec.Y, vec.X + hS[index], vec.Y + hS[index]), width);
                        if (name)
                            gfx.DrawTextWithBackground(_fonts["arial"], _brushes["playerText"], _brushes["black"], new Point(vec.X, vec.Y - 35), names[index]);
                        if (healthBox)
                            gfx.FillRectangle(_brushes["healthBar"], new Rectangle(
                                vec.X - 0.2f * hS[index],
                                vec.Y,
                                vec.X,
                                vec.Y + (feetList[index].Y - vec.Y) / 100 * healthList[index]));
                        if (healthText)
                            gfx.DrawTextWithBackground(_fonts["arial"], _brushes["healthTextC"], _brushes["black"], new Point(vec.X, vec.Y - 15), $"HP: {healthList[index]}/100");
                        index++;
                    }
                    index = 0;
                    foreach (var vec in c_list)
                    {
                        color = _brushes["normalColor"];
                        if (index == 0 && isInReach)
                            color = _brushes["closestEnemy"];
                        if (index == 0 && isInReach && (GetAsyncKeyState(program.AIMBOT_HOTKEY) < 0 || GetAsyncKeyState(program.ONLY_AIMBOT_HOTKEY) < 0))
                            color = _brushes["aimLockedEnemy"];
                        if (visibleList[index])
                            color = _brushes["black"];

                        gfx.DrawCircle(color, new Circle(vec.X, vec.Y, c_hS[index] / 2), width);
                        index++;
                    }
                    index = 0;
                    foreach (Vector3 vec in origins)
                    {
                        if (!boxEsp)
                            break;

                        Vector3 head = heads[index];

                        double angle = angles[index].Y / 180 * Math.PI;

                        double cos = Math.Cos(angle);
                        double sin = Math.Sin(angle);

                        float eyeRaySize = 20f;

                        Vector3 look = Vector3.Add(head, new Vector3((float)(eyeRaySize * cos - 0 * sin), (float)(eyeRaySize * sin + 0 * cos), 0));

                        Vector2 look2d = WorldToScreen(currentViewMatrix, look, (int)windowSize.X, (int)windowSize.Y);

                        float size = 15f;
                        draw3dBoxAroundLine(gfx, currentViewMatrix, vec, new Vector3(head.X, head.Y, head.Z + 10f), cos, sin, size);

                        gfx.DrawLine(_brushes["white"], headLoc[index].X, headLoc[index].Y, look2d.X, look2d.Y, width);

                        index++;
                    }
                    index = 0;
                    foreach (List<Vector2> bones in bonesList)
                    {
                        if (!boneEsp)
                            break;

                        bool fail = false;

                        foreach (Vector2 bone in bones)
                            if (!IsPixelInsideScreen(bone))
                                fail = true;

                        if (fail)
                            continue;

                        color = _brushes["bone"];

                        Vector3[] bonesPos = bones3d[index].ToArray();

                        Dictionary<Vector3, int[]> boneMap = new Dictionary<Vector3, int[]>
                        {
                            { bonesPos[0] , [1, 11, 14] },
                            { bonesPos[1] , [2] },
                            { bonesPos[2] , [5, 8, 3] },
                            { bonesPos[3] , [4] },
                            { bonesPos[5] , [6] },
                            { bonesPos[6] , [7] },
                            { bonesPos[8] , [9] },
                            { bonesPos[9] , [10] },
                            { bonesPos[11], [12] },
                            { bonesPos[12], [13] },
                            { bonesPos[14], [15] },
                            { bonesPos[15], [16] }
                        };

                        List<Vector2> allPoints = new List<Vector2>();

                        foreach (Vector3 start in boneMap.Keys)
                        {
                            int[] ends = [];
                            boneMap.TryGetValue(start, out ends);

                            foreach (int endIndex in ends)
                            {
                                Vector3 end = bonesPos[endIndex];
                                List<Vector2> sP = new List<Vector2>();
                                foreach (Vector3 point in GetCuboidCorners(start, end, 5, 5))
                                {
                                    Vector2 screen = WorldToScreen(currentViewMatrix, point, (int)windowSize.X, (int)windowSize.Y);
                                    sP.Add(screen);
                                }

                                Vector2 p1 = sP[0];
                                Vector2 p2 = sP[1];
                                Vector2 p3 = sP[3];
                                Vector2 p4 = sP[2];

                                Vector2 p5 = sP[4];
                                Vector2 p6 = sP[5];
                                Vector2 p7 = sP[7];
                                Vector2 p8 = sP[6];


                                Vector2[] points = [p1, p2, p3, p4, p5, p6, p7, p8];

                                allPoints.AddRange(points);

                                var Bcolor = solidBrushFromVec4(gfx, new Vector4(0.5f, 0.5f, 1f, 1f));

                                bool valid = true;

                                foreach (Vector2 pt in points)
                                    if (!IsPixelInsideScreen(pt))
                                        valid = false;

                                if (!valid)
                                    continue;

                                Vector2[] o_points = ConvexHull.GetConvexHull(points.ToList()).ToArray();

                                Vector2? last_point = null;
                                foreach (Vector2 pt in o_points)
                                {
                                    if (last_point == null)
                                    {
                                        last_point = pt;
                                        continue;
                                    }

                                    gfx.DrawLine(color, last_point.Value.X, last_point.Value.Y, pt.X, pt.Y, width);
                                    last_point = pt;
                                }
                                gfx.DrawLine(color, last_point.Value.X, last_point.Value.Y, o_points[0].X, o_points[0].Y, width);


                                continue;
                                
                                //feet
                                gfx.FillTriangle(Bcolor,
                                    p1.X, p1.Y,
                                    p2.X, p2.Y,
                                    p3.X, p3.Y
                                    );
                                gfx.FillTriangle(Bcolor,
                                    p4.X, p4.Y,
                                    p2.X, p2.Y,
                                    p3.X, p3.Y
                                    );

                                //head
                                gfx.FillTriangle(Bcolor,
                                    p5.X, p5.Y,
                                    p6.X, p6.Y,
                                    p7.X, p7.Y
                                    );
                                gfx.FillTriangle(Bcolor,
                                    p8.X, p8.Y,
                                    p6.X, p6.Y,
                                    p7.X, p7.Y
                                    );

                                //side north
                                gfx.FillTriangle(Bcolor,
                                    p3.X, p3.Y,
                                    p4.X, p4.Y,
                                    p8.X, p8.Y
                                    );
                                gfx.FillTriangle(Bcolor,
                                    p3.X, p3.Y,
                                    p7.X, p7.Y,
                                    p8.X, p8.Y
                                    );

                                //side south
                                gfx.FillTriangle(Bcolor,
                                    p1.X, p1.Y,
                                    p2.X, p2.Y,
                                    p6.X, p6.Y
                                    );
                                gfx.FillTriangle(Bcolor,
                                    p1.X, p1.Y,
                                    p5.X, p5.Y,
                                    p6.X, p6.Y
                                    );

                                //side east
                                gfx.FillTriangle(Bcolor,
                                    p2.X, p2.Y,
                                    p4.X, p4.Y,
                                    p8.X, p8.Y
                                    );
                                gfx.FillTriangle(Bcolor,
                                    p2.X, p2.Y,
                                    p6.X, p6.Y,
                                    p8.X, p8.Y
                                    );

                                //side west
                                gfx.FillTriangle(Bcolor,
                                    p1.X, p1.Y,
                                    p3.X, p3.Y,
                                    p7.X, p7.Y
                                    );
                                gfx.FillTriangle(Bcolor,
                                    p1.X, p1.Y,
                                    p5.X, p5.Y,
                                    p7.X, p7.Y
                                    );
                            }
                        }

                        gfx.DrawLine(color, bones[0].X, bones[0].Y, bones[1].X, bones[1].Y, width);
                        gfx.DrawLine(color, bones[2].X, bones[2].Y, bones[5].X, bones[5].Y, width);
                        gfx.DrawLine(color, bones[2].X, bones[2].Y, bones[8].X, bones[8].Y, width);
                        gfx.DrawLine(color, bones[1].X, bones[1].Y, bones[2].X, bones[2].Y, width);
                        gfx.DrawLine(color, bones[2].X, bones[2].Y, bones[3].X, bones[3].Y, width);
                        gfx.DrawLine(color, bones[5].X, bones[5].Y, bones[6].X, bones[6].Y, width);
                        gfx.DrawLine(color, bones[6].X, bones[6].Y, bones[7].X, bones[7].Y, width);
                        gfx.DrawLine(color, bones[8].X, bones[8].Y, bones[9].X, bones[9].Y, width);
                        gfx.DrawLine(color, bones[9].X, bones[9].Y, bones[10].X, bones[10].Y, width);
                        gfx.DrawLine(color, bones[3].X, bones[3].Y, bones[4].X, bones[4].Y, width);
                        gfx.DrawLine(color, bones[0].X, bones[0].Y, bones[11].X, bones[11].Y, width);
                        gfx.DrawLine(color, bones[11].X, bones[11].Y, bones[12].X, bones[12].Y, width);
                        gfx.DrawLine(color, bones[12].X, bones[12].Y, bones[13].X, bones[13].Y, width);
                        gfx.DrawLine(color, bones[0].X, bones[0].Y, bones[14].X, bones[14].Y, width);
                        gfx.DrawLine(color, bones[14].X, bones[14].Y, bones[15].X, bones[15].Y, width);
                        gfx.DrawLine(color, bones[15].X, bones[15].Y, bones[16].X, bones[16].Y, width);
                        index++;
                    }
                }

            }

            if (velDisplay)
            {
                string velXY = (Math.Round(Math.Abs(localPlayer.absVeloctiy.X)) + Math.Round(Math.Abs(localPlayer.absVeloctiy.Y))).ToString();
                string velZ = Math.Round(Math.Abs(localPlayer.absVeloctiy.Z)).ToString();
                while (velXY.Length < 3)
                {
                    velXY = $"0{velXY}";
                }
                while (velZ.Length < 3)
                {
                    velZ = $"0{velZ}";
                }
                string vel = $"({velXY} | {velZ})";
                gfx.DrawText(_fonts["consolas"], _brushes["white"], windowCenter.X-vel.Length*7.6f/2, windowCenter.Y+50f, vel);
            }

            gfx.DrawCrosshair(_brushes["white"], localPlayer.originScreenPos.X, localPlayer.originScreenPos.Y, 5f, 2f, CrosshairStyle.Dot);

            if (extraCrosshair)
                gfx.DrawCrosshair(_brushes["white"], windowCenter.X, windowCenter.Y, crosshairSize, 1.5f, CrosshairStyle.Plus);

            string cX = $"X: {Math.Round(localPlayer.origin.X)}";
            string cY = $"Y: {Math.Round(localPlayer.origin.Y)}";
            string cZ = $"Z: {Math.Round(localPlayer.origin.Z)}";

            List<string> infos =
            [
                infoText,
                cX,
                cY,
                cZ
            ];

            if (flashAlpha > 0.8f)
            {
                infos.Add("You Are Flashed");
            }

            int ix = 1;
            foreach (string info in infos)
            {
                gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], 320, 20*ix+5*(ix-1), info);
                ix++;
            }

            if (showRecoil)
            {
                foreach (Vector3 pt in recoilpoints)
                {
                    gfx.DrawCircle(_brushes["red"], windowCenter.X + pt.X, windowCenter.Y + pt.Y, 3, width);
                }
            }

            if (menuToggled)
            {
                gfx.ClearScene(solidBrushFromVec4(gfx, new Vector4(0, 0, 0, 0.75f)));
                return;
            }

            //menu off theme
            if (!menuToggled)
            {
                switch (currentTheme)
                {
                    case 0:
                        break;
                    case 1:
                        gfx.FillRectangle(solidBrushFromVec4(gfx, new Vector4(0, 0, 0, 0.35f)), 0, 0, windowSize.X, windowSize.Y);
                        break;
                    case 2:
                        lsdRainbowProgress = lsdRainbowProgress + 0.005f + (0.005f * 3f);
                        gfx.FillRectangle(solidBrushFromVec4(gfx, Rainbow(lsdRainbowProgress) - new Vector4(0, 0, 0, 0.75f)), 0, 0, windowSize.X, windowSize.Y);
                        break;
                    case 3:
                        gfx.FillRectangle(solidBrushFromVec4(gfx, new Vector4(0.5f, 0, 0.5f, 0.25f)), 0, 0, windowSize.X, windowSize.Y);
                        break;
                }
            }
        }

        public void Run()
        {
            client = swed.GetModuleBase("client.dll");
            program = new Program();
            _window.Create();
            _window.Join();
        }

        ~esp()
        {
            Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _window.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}