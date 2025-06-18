using CS2Dumper;
using CS2Dumper.Offsets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CS2Dumper.Schemas.ClientDll;

namespace cs2Cheat
{
    public class Offsets
    {
        public int viewmatrix = int.Parse(ClientDll.dwViewMatrix.ToString());
        public int viewangles = int.Parse(ClientDll.dwViewAngles.ToString());
        public int localPlayer = int.Parse(ClientDll.dwLocalPlayerPawn.ToString());
        public int localController = int.Parse(ClientDll.dwLocalPlayerController.ToString());
        public int entityList = int.Parse(ClientDll.dwEntityList.ToString());
        public int playerpawn = int.Parse(CCSPlayerController.m_hPlayerPawn.ToString());
        public int gameRules = int.Parse(ClientDll.dwGameRules.ToString());
        public int globalVars = int.Parse(ClientDll.dwGlobalVars.ToString());
        public int boneArray = 0x80;
        public int teamNum = int.Parse(C_BaseEntity.m_iTeamNum.ToString());
        public int eyeAngles = int.Parse(C_CSPlayerPawnBase.m_angEyeAngles.ToString());
        public int jumpFlag = int.Parse(C_BaseEntity.m_fFlags.ToString());
        public int health = int.Parse(C_BaseEntity.m_iHealth.ToString());
        public int origin = int.Parse(C_BasePlayerPawn.m_vOldOrigin.ToString());
        public int weapon_services = int.Parse(C_BasePlayerPawn.m_pWeaponServices.ToString());
        public int m_iAmmo = 0x60;
        public int m_pViewModelServices = int.Parse(C_CSPlayerPawnBase.m_pViewModelServices.ToString());
        public int m_hViewModel = int.Parse(CCSPlayer_ViewModelServices.m_hViewModel.ToString());
        public int m_nViewModelIndex = int.Parse(C_BaseViewModel.m_nViewModelIndex.ToString());
        public int h_myWeapons = 0x40;
        public int modelState = int.Parse(CSkeletonInstance.m_modelState.ToString());
        public int gameScene = int.Parse(C_BaseEntity.m_pGameSceneNode.ToString());
        public int spottedState = int.Parse(C_CSPlayerPawn.m_entitySpottedState.ToString());
        public int spotted = 0x8;
        public int lifeState = int.Parse(C_BaseEntity.m_lifeState.ToString());
        public int camService = int.Parse(C_BasePlayerPawn.m_pCameraServices.ToString());
        public int scoped = int.Parse(C_CSPlayerPawn.m_bIsScoped.ToString());
        public int fov = int.Parse(CCSPlayerBase_CameraServices.m_iFOV.ToString());
        public int force_jump = int.Parse(Buttons.jump.ToString());
        public int force_zoom = int.Parse(Buttons.zoom.ToString());
        public int force_attack = int.Parse(Buttons.attack.ToString());
        public int absVelocity = int.Parse(C_BaseEntity.m_vecAbsVelocity.ToString());
        public int IDEntIndex = int.Parse(C_CSPlayerPawnBase.m_iIDEntIndex.ToString());
        public int playersAliveCT = int.Parse(C_CSPlayerPawn.m_nLastKillerIndex.ToString());
        public int playersAliveT = int.Parse(C_CSPlayerPawn.m_flHitHeading.ToString());
        public int m_hActiveWeapon = int.Parse(CPlayer_WeaponServices.m_hActiveWeapon.ToString());
        public int aimPunchAngle = int.Parse(C_CSPlayerPawn.m_aimPunchAngle.ToString());
        public int iShotsFired = int.Parse(C_CSPlayerPawn.m_iShotsFired.ToString());
        public int clippingWeapon = int.Parse(C_CSPlayerPawnBase.m_pClippingWeapon.ToString());
        public int ItemDefinitionIndex = 0x1BA;
        public int m_iAmmoLastCheck = int.Parse(C_CSWeaponBase.m_iAmmoLastCheck.ToString());
        public int vOldOrigin = int.Parse(C_BasePlayerPawn.m_vOldOrigin.ToString());
        public int AttributeManager = int.Parse(C_EconEntity.m_AttributeManager.ToString());
        public int item = 0x50;
        public int m_hController = int.Parse(C_BasePlayerPawn.m_hController.ToString());
        public int steamid = int.Parse(CBasePlayerController.m_steamID.ToString());
        public int playerName = int.Parse(CBasePlayerController.m_iszPlayerName.ToString());
        public int actionTrackingServices = int.Parse(CCSPlayerController.m_pActionTrackingServices.ToString());
        public int damageDealt = int.Parse(CCSPlayerController_ActionTrackingServices.m_unTotalRoundDamageDealt.ToString());
        public int m_bInReload = int.Parse(C_CSWeaponBase.m_bInReload.ToString());
    }
}
