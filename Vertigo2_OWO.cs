using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyOwoVest;

using UnityEngine;
using Vertigo2.Weapons;
using Vertigo2.Interaction;
using Vertigo2;
using Vertigo2.Player;
using Valve.VR;

[assembly: MelonInfo(typeof(Vertigo2_OWO.Vertigo2_OWO), "Vertigo2_OWO", "1.1.3", "Florian Fahrenberger")]
[assembly: MelonGame("Zulubo Productions", "vertigo2")]

namespace Vertigo2_OWO
{
    public class Vertigo2_OWO : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        private static int rightHand = ((int)SteamVR_Input_Sources.RightHand);
        private static bool isRightHanded = true;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
        }



        #region Healing

        [HarmonyPatch(typeof(NanitePen), "Inject", new Type[] { })]
        public class bhaptics_InjectNanitePen
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("Healing");
            }
        }

        [HarmonyPatch(typeof(HealthStation), "Inject", new Type[] { })]
        public class bhaptics_InjectHealthStation
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("Healing");
            }
        }

        [HarmonyPatch(typeof(HealthFruit), "Bite", new Type[] { })]
        public class bhaptics_HealthFruitBite
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("Healing");
            }
        }

        #endregion

        #region Weapons

        [HarmonyPatch(typeof(Gun), "ShootHaptics")]
        public class bhaptics_GunFeedback
        {
            [HarmonyPostfix]
            public static void Postfix(Gun __instance, float length, float power)
            {
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                bool twoHanded = (__instance.heldEquippable.otherHandHolding);
                float intensity = Math.Max(power * 2.0f, 1.2f);

                if (__instance.name == "MeatNailer")
                {
                    tactsuitVr.MeatNailerRecoil(isRightHand, 1.0f, twoHanded);
                    return;
                }

                tactsuitVr.GunRecoil(isRightHand, intensity, twoHanded);
            }
        }

        [HarmonyPatch(typeof(QuadBow), "ReleaseString", new Type[] { typeof(VertigoHand) })]
        public class bhaptics_BowFeedback
        {
            [HarmonyPostfix]
            public static void Postfix(QuadBow __instance, VertigoHand hand)
            {
                bool isRightHand = (((int)hand.inputSource) == rightHand);
                if (isRightHand) tactsuitVr.PlayBackFeedback("BowRelease_R");
                else tactsuitVr.PlayBackFeedback("BowRelease_L");
            }
        }

        [HarmonyPatch(typeof(Tailgun), "ShootHaptics", new Type[] { })]
        public class bhaptics_TailgunFeedback
        {
            [HarmonyPostfix]
            public static void Postfix(Tailgun __instance)
            {
                if (__instance.handle_L.isBeingHeld) tactsuitVr.GunRecoil(false, 0.7f);
                if (__instance.handle_R.isBeingHeld) tactsuitVr.GunRecoil(true, 0.7f);
            }
        }

        [HarmonyPatch(typeof(Enlighten), "Haptics", new Type[] { typeof(float), typeof(float), typeof(float), typeof(float)})]
        public class bhaptics_EnlightenFeedback
        {
            [HarmonyPostfix]
            public static void Postfix(Enlighten __instance)
            {
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                bool twoHanded = (__instance.heldEquippable.otherHandHolding);
                float intensity = 0.6f;
                tactsuitVr.EnlightenRecoil(isRightHand, intensity, twoHanded);
            }
        }

        [HarmonyPatch(typeof(HammerSickle), "Blade_OnImpact", new Type[] { typeof(Collider), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(float) })]
        public class bhaptics_HammerSickleCollide
        {
            [HarmonyPostfix]
            public static void Postfix(HammerSickle __instance, float normalizedSpeed, bool ___energyOn)
            {
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                float intensity = normalizedSpeed;
                if (!___energyOn) intensity /= 2.0f;
                tactsuitVr.SwordRecoil(isRightHand, intensity);
            }
        }

        [HarmonyPatch(typeof(HammerSickle), "ThrowProjectile", new Type[] {  })]
        public class bhaptics_HammerSickleShoot
        {
            [HarmonyPostfix]
            public static void Postfix(HammerSickle __instance)
            {
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                tactsuitVr.ThrowRecoil(isRightHand);
            }
        }

        [HarmonyPatch(typeof(Blafaladaciousnesticles), "OnImpact", new Type[] { typeof(Collider), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(float) })]
        public class bhaptics_SwordFeedback
        {
            [HarmonyPostfix]
            public static void Postfix(Blafaladaciousnesticles __instance, float normalizedSpeed)
            {
                if (normalizedSpeed <= 0.0) return;
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                float intensity = normalizedSpeed;
                tactsuitVr.SwordRecoil(isRightHand, intensity);
            }
        }

        [HarmonyPatch(typeof(Blafaladaciousnesticles), "Deflect", new Type[] { typeof(Vector3), typeof(Vector3) })]
        public class bhaptics_SwordDeflect
        {
            [HarmonyPostfix]
            public static void Postfix(Blafaladaciousnesticles __instance)
            {
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                float intensity = 1.0f;
                tactsuitVr.SwordRecoil(isRightHand, intensity);
            }
        }

        [HarmonyPatch(typeof(AmmoBelt), "TakeAmmo", new Type[] { typeof(VertigoHand) })]
        public class bhaptics_TakeAmmo
        {
            [HarmonyPostfix]
            public static void Postfix(AmmoBelt __instance)
            {
                tactsuitVr.GrabAmmo(isRightHanded);
            }
        }

        [HarmonyPatch(typeof(Gun), "TryLoad", new Type[] { typeof(Ammo) })]
        public class bhaptics_LoadGun
        {
            [HarmonyPostfix]
            public static void Postfix(Equippable __instance)
            {
                if (!__instance.reloadReady) return;
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                tactsuitVr.Reload(isRightHand);
            }
        }

        [HarmonyPatch(typeof(Ionizer), "EjectMag", new Type[] {  })]
        public class bhaptics_EjectMag
        {
            [HarmonyPostfix]
            public static void Postfix(Ionizer __instance)
            {
                bool isRightHand = (((int)__instance.inputSource) == rightHand);
                tactsuitVr.EjectMag(isRightHand);
            }
        }



        #endregion

        #region Damage

        private static (float, float) getAngleAndShift(VertigoPlayer player, HitInfo hit)
        {
            // bhaptics pattern starts in the front, then rotates to the left. 0° is front, 90° is left, 270° is right.
            // y is "up", z is "forward" in local coordinates
            Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
            Vector3 hitPosition = hit.hitPoint - player.position;
            Quaternion PlayerRotation = player.head.rotation;
            Vector3 playerDir = PlayerRotation.eulerAngles;
            // get rid of the up/down component to analyze xz-rotation
            Vector3 flattenedHit = new Vector3(hitPosition.x, 0f, hitPosition.z);
            // get angle. .Net < 4.0 does not have a "SignedAngle" function...
            float hitAngle = Vector3.Angle(flattenedHit, patternOrigin);
            // check if cross product points up or down, to make signed angle myself
            Vector3 crossProduct = Vector3.Cross(flattenedHit, patternOrigin);
            if (crossProduct.y > 0f) { hitAngle *= -1f; }
            // relative to player direction
            float myRotation = hitAngle - playerDir.y;
            // switch directions (bhaptics angles are in mathematically negative direction)
            myRotation *= -1f;
            // convert signed angle into [0, 360] rotation
            if (myRotation < 0f) { myRotation = 360f + myRotation; }

            // up/down shift is in y-direction
            // in Vertigo 2, the torso Transform has y=0 at the neck,
            // and the torso ends at roughly -0.5 (that's in meters)
            // so cap the shift to [-0.5, 0]...
            float hitShift = hitPosition.y;
            float upperBound = 0.0f;
            float lowerBound = -0.5f;
            if (hitShift > upperBound) { hitShift = 0.5f; }
            else if (hitShift < lowerBound) { hitShift = -0.5f; }
            // ...and then spread/shift it to [-0.5, 0.5], which is how bhaptics expects it
            else { hitShift = (hitShift - lowerBound) / (upperBound - lowerBound) - 0.5f; }


            return (myRotation, hitShift);
        }

        [HarmonyPatch(typeof(VertigoPlayer), "Hit", new Type[] { typeof(HitInfo), typeof(bool) })]
        public class bhaptics_PlayerHit
        {
            [HarmonyPostfix]
            public static void Postfix(VertigoPlayer __instance, HitInfo hit, bool crit)
            {
                // Start heart beat if low on health
                if (__instance.health < 0.3f * __instance.maxHealth) tactsuitVr.PlayBackFeedback("ThreeHeartBeats");
                // if (crit) { tactsuitVr.LOG("Critical hit!"); }
                // Non-directional environmental feedback
                if ((hit.damageType & DamageType.Fire) == DamageType.Fire)
                {
                    tactsuitVr.PlayBackFeedback("Fire");
                    return;
                }
                if ((hit.damageType & DamageType.Poison) == DamageType.Poison)
                {
                    tactsuitVr.PlayBackFeedback("Poison");
                    return;
                }
                /*
                if ((hit.damageType & DamageType.Electricity) == DamageType.Electricity)
                {
                    tactsuitVr.PlayBackFeedback("Electrocution");
                    return;
                }
                */
                if ((hit.damageType & DamageType.Explosion) == DamageType.Explosion)
                {
                    tactsuitVr.PlayBackFeedback("Explosion");
                    return;
                }
                if ((hit.damageType & DamageType.Drowning) == DamageType.Drowning)
                {
                    tactsuitVr.PlayBackFeedback("Drowning");
                    return;
                }
                if ((hit.damageType & DamageType.Radiation) == DamageType.Radiation)
                {
                    tactsuitVr.PlayBackFeedback("Radiation");
                    return;
                }
                // Directional feedback
                float hitAngle;
                float hitShift;
                string damageType = "Hit";
                switch (hit.damageType)
                {
                    case DamageType.Enlightenment:
                        damageType = "Hit";
                        break;
                    case DamageType.Bullet:
                        damageType = "Hit";
                        break;
                    case DamageType.Laser:
                        damageType = "Hit";
                        break;
                    case DamageType.Impact:
                        damageType = "Hit";
                        break;
                    case DamageType.Blade:
                        damageType = "Hit";
                        break;
                    case DamageType.Bite:
                        damageType = "Hit";
                        break;
                    case DamageType.Plasma:
                        damageType = "Hit";
                        break;
                    case DamageType.Heat:
                        damageType = "Hit";
                        break;
                    case DamageType.Cold:
                        damageType = "Hit";
                        break;
                    case DamageType.Electricity:
                        damageType = "Hit";
                        break;
                    case DamageType.Antimatter:
                        damageType = "Hit";
                        break;
                    case DamageType.Hyperdimensional:
                        damageType = "Hit";
                        break;
                    case DamageType.Generic:
                        damageType = "Hit";
                        break;
                    default:
                        damageType = "Hit";
                        tactsuitVr.LOG("New damageType in hit: " + hit.damageType.ToString());
                        break;
                }
                // Default hit if not registered yet
                (hitAngle, hitShift) = getAngleAndShift(__instance, hit);
                tactsuitVr.PlayBackHit(damageType, hitAngle, hitShift);
            }
        }

        #endregion

        #region Movement

        [HarmonyPatch(typeof(Elevator), "Move")]
        public class bhaptics_ElevatorMove
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("ElevatorTingle");
            }
        }

        [HarmonyPatch(typeof(VertigoCharacterController), "DoTeleportAnim", new Type[] { typeof(Vector3), typeof(TeleportSurface) })]
        public class bhaptics_Teleport
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("Teleport");
            }
        }

        [HarmonyPatch(typeof(LaunchLily), "Jump", new Type[] { typeof(VertigoPlayer) })]
        public class bhaptics_LaunchLily
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("LaunchPad");
            }
        }

        [HarmonyPatch(typeof(JumpPad), "Jump", new Type[] { typeof(VertigoPlayer) })]
        public class bhaptics_JumpPad
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("LaunchPad");
            }
        }

        #endregion

        #region Special events

        [HarmonyPatch(typeof(CopterAirChase), "MissileHitCopter", new Type[] { })]
        public class bhaptics_MissileHitCopter
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("Explosion");
            }
        }

        [HarmonyPatch(typeof(CopterAirChase), "Crash", new Type[] { })]
        public class bhaptics_CopterCrash
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("Explosion");
            }
        }

        [HarmonyPatch(typeof(Cyberjoseph), "BigStompNoDamage", new Type[] { })]
        public class bhaptics_BigStompNoDamage
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("ShakeGround");
            }
        }

        [HarmonyPatch(typeof(Cyberjoseph), "BothFeetStomp", new Type[] { })]
        public class bhaptics_BothFeetStomp
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("ShakeGround");
            }
        }

        [HarmonyPatch(typeof(Cyberjoseph), "FaceSlamNoDamage", new Type[] { })]
        public class bhaptics_FaceSlamNoDamage
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("ShakeGround");
            }
        }

        [HarmonyPatch(typeof(Cyberjoseph), "SuperSlam", new Type[] { })]
        public class bhaptics_SuperSlam
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlayBackFeedback("ShakeGround");
            }
        }

        #endregion

        #region Explosions

        [HarmonyPatch(typeof(Explosion), "Explode", new Type[] { })]
        public class bhaptics_ExplosionExplode
        {
            [HarmonyPostfix]
            public static void Postfix(Explosion __instance)
            {
                if ((__instance.maxDamage == 0.0f) && (!__instance.applyScreenShake)) return;
                float distance = (__instance.transform.position - Vertigo2.AI.AIManager.world.player.position).magnitude;
                float max_dist = 150.0f;
                if (distance > max_dist) return;
                float intensityScale = 1f;
                if (__instance.maxForce > 0f) intensityScale = Math.Min(__instance.maxForce / 20f, 1.0f);
                /*
                tactsuitVr.LOG("Explosion: " + __instance.maxDamage.ToString());
                tactsuitVr.LOG("Can hurt: " + __instance.canHurtSourceEntity.ToString());
                tactsuitVr.LOG("maxForce: " + __instance.maxForce.ToString());
                tactsuitVr.LOG("maxRadius: " + __instance.maxRadius.ToString());
                tactsuitVr.LOG("maxShockwave: " + __instance.maxShockwaveMass.ToString());
                tactsuitVr.LOG("damageScale: " + __instance.playerDamageScale.ToString());
                tactsuitVr.LOG("screenShake: " + __instance.screenShakeMagnitude.ToString());
                tactsuitVr.LOG(" ");
                */
                float intensity = ((max_dist - distance)/max_dist) * ((max_dist - distance) / max_dist) * intensityScale;
                if (intensity < 0.0f) return;
                if (intensity < 0.8f) tactsuitVr.PlayBackFeedback("ExplosionDistant");
                else tactsuitVr.PlayBackFeedback("Explosion");
            }
        }

        #endregion

    }
}
