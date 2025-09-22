
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


public class Main : Script
{
    private float HealthReference;
    private float ArmorReference;
    private string LowH = "HeistCelebPassBW";
    private string MedH = "DeathFailOut";
    private int Camera;
    private Prop current1 = (Prop)null;
    private static Model model = (Model)(string)null;
    private int current;
    public Prop WeaponOnBack;
    static private int HeldWeapon;
    private bool attached = false;
    public bool on = false;
    public bool ontwo = false;
    public bool onthree = false;
    bool SlowMotoggle = false;
    bool SlowMotoggle2 = false;
    //

    private List<Entity> droppedWeapons = new List<Entity>();
    private Dictionary<Entity, WeaponProperties> weaponProperties = new Dictionary<Entity, WeaponProperties>();
    private Entity closestDroppedWeapon = (Entity)null;
    private Entity weaponToAttach = (Entity)null;


    
    public static bool IsCarryingWeaponAsProp;
    private bool isCarryingWeaponAsProp = false;
    private WeaponProperties storedWeaponProps = (WeaponProperties)null;
    private Entity carriedWeaponEntity = (Entity)null;
    //public static Weapon current = null;


    private static readonly HashSet<int> TargetInteriorIDs = new HashSet<int>
{
    62978, 94978, 37122, 34818, 30722, 80386,
    115458, 35586, 29698, 59138, 48130
};


    //
    private int[,] CharacterClothes = new int[3, 3]
    {
    {
      8,
      17,
      0
    },
    {
      8,
      6,
      14
    },
    {
      8,
      8,
      14
    }
    };
    private int[,] CharacterJailClothes = new int[3, 3]
    {
    {
      3,
      0,
      0
    },
    {
      3,
      0,
      0
    },
    {
      3,
      0,
      0
    }
    };



    private void OnTick(object sender, EventArgs e)
    {
        
        ///visibleWeapon2();
       // this.VisibleWeaponFunc();
        this.RagDollWeaponDrop();
        this.RemoveWeaponsWhenDead();
        this.ShowVisibleArmor();
        this.PedReaction();
        this.WeaponsAreScary();
        this.lowHealthFX();
        this.SlowMo();
        this.noHealthRegeneration();
        this.GiveWeaponToOthers();
       // this.Hint();
        this.weaponPickupTick();
    }
    public static int GetMeleeCount(Ped ped)
    {
        int meleeCount = 0;
        foreach (Main.BigMelee bigMelee in Enum.GetValues(typeof(Main.BigMelee)))
        {
            if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
            {
        (InputArgument) ped.Handle,
        (InputArgument) bigMelee.GetHashCode(),
        (InputArgument) false
            }))
                ++meleeCount;
        }
        return meleeCount;
    }

    public static int GetSmallWeaponCount(Ped ped)
    {
        int smallWeaponCount = 0;
        foreach (Main.SmallWeapons smallWeapons in Enum.GetValues(typeof(Main.SmallWeapons)))
        {
            if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
            {
        (InputArgument) ped.Handle,
        (InputArgument) smallWeapons.GetHashCode(),
        (InputArgument) false
            }))
                ++smallWeaponCount;
        }
        return smallWeaponCount;
    }

    public static int GetExplosivesCount(Ped ped)
    {
        int explosivesCount = 0;
        foreach (Main.Explosives explosives in Enum.GetValues(typeof(Main.Explosives)))
        {
            if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
            {
        (InputArgument) ped.Handle,
        (InputArgument) explosives.GetHashCode(),
        (InputArgument) false
            }))
                ++explosivesCount;
        }
        return explosivesCount;
    }

    public static int AllWeaponsCount(Ped ped)
    {
        int num = 0;
        foreach (Main.AllWeapons allWeapons in Enum.GetValues(typeof(Main.AllWeapons)))
        {
            if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
            {
        (InputArgument) ped.Handle,
        (InputArgument) allWeapons.GetHashCode(),
        (InputArgument) false
            }))
                ++num;
        }
        return num;
    }

    public static int GetBigWeaponCount(Ped ped)
    {
        int bigWeaponCount = 0;
        foreach (Main.BigWeapons bigWeapons in Enum.GetValues(typeof(Main.BigWeapons)))
        {
            if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
            {
        (InputArgument) ped.Handle,
        (InputArgument) bigWeapons.GetHashCode(),
        (InputArgument) false
            }))
                ++bigWeaponCount;
        }
        return bigWeaponCount;
    }

    public static int GetMiscCount(Ped ped)
    {
        int miscCount = 0;
        foreach (Main.Misc misc in Enum.GetValues(typeof(Main.Misc)))
        {
            if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
            {
        (InputArgument) ped.Handle,
        (InputArgument) misc.GetHashCode(),
        (InputArgument) false
            }))
                ++miscCount;
        }
        return miscCount;
    }

    public static List<long> GET_POLICE_PED_MODELS()
    {
        return new List<long>()
    {
      368603149L,
      1581098148L,
      2595446627L,
      4074414829L,
      1702441027L,
      4028996995L,
      1490458366L,
      1925237458L,
      2374966032L
    };
    }

    public static int GET_PLAYER_GROUP(Player player)
    {
        return Function.Call<int>(Hash.GET_PLAYER_GROUP, new InputArgument[1]
        {
      (InputArgument) Game.Player
        });
    }

    public static bool PED_IS_POLICE_FORCE(Ped ped)
    {
        return Main.GET_POLICE_PED_MODELS().Contains((long)ped.Model.Hash);
    }

    public static bool IS_PED_IN_GROUP(Ped ped, int groupId)
    {
        return Function.Call<bool>(Hash.IS_PED_IN_GROUP, new InputArgument[2]
        {
      (InputArgument) (Entity) ped,
      (InputArgument) groupId
        });
    }

    private void PedReaction()
    {
        if (Function.Call<bool>(Hash.GET_MISSION_FLAG, Array.Empty<InputArgument>()) || !Game.Player.IsPlaying || !Game.Player.Character.IsInVehicle() || Main.PED_IS_POLICE_FORCE(Game.Player.Character) || Game.Player.Character.IsInTaxi)
            return;
        foreach (Ped ped in ((IEnumerable<Ped>)World.GetNearbyPeds(Game.Player.Character, 1f)).ToList<Ped>().ToArray())
        {
            if (!Main.PED_IS_POLICE_FORCE(ped))
            {
                int playerGroup = Main.GET_PLAYER_GROUP(Game.Player);
                if (ped.IsAlive && ped.IsHuman && ped.IsInVehicle() && !ped.IsInTaxi && !ped.IsFleeing && !ped.IsShooting && !ped.IsReloading && !ped.IsRagdoll && !ped.IsGettingIntoVehicle && !ped.IsGettingUp && !ped.IsInCombat && ped.IsVisible && !ped.IsInMeleeCombat && !Main.IS_PED_IN_GROUP(ped, playerGroup) && ped.GetRelationshipWithPed(Game.Player.Character) == Relationship.Pedestrians)
                {
                    if (ped.Model == new Model(PedHash.Bouncer01SMM) && ped.IsAlive)
                        break;
                    Vehicle lastVehicle = Game.Player.LastVehicle;
                    System.Random random = new System.Random();
                    Ped driver = ped.CurrentVehicle.Driver;
                    Ped targetedEntity = Game.Player.TargetedEntity as Ped;
                    ped.Task.ClearAll();
                    if (ped.Gender == Gender.Male)
                    {
                        if (random.Next(1, 8) >= 2)
                            ped.Task.FightAgainst(Game.Player.Character);
                        else
                            ped.Task.FleeFrom(Game.Player.Character);
                    }
                    else
                        ped.Task.FleeFrom(Game.Player.Character);
                }
            }
        }
    }

    public static bool PED_IS_ARMED(Ped ped)
    {
        return ped.Weapons.Current.IsPresent && ped.Weapons.Current.Hash != WeaponHash.Unarmed;
    }

    public static bool WEAPON_IS_FIREARM(WeaponHash weaponHash)
    {
        return Main.GET_FIREARM().Contains((long)weaponHash);
    }

    public static bool IS_PED_GROUP_MEMBER(Ped ped, int groupId)
    {
        return Function.Call<bool>(Hash.IS_PED_GROUP_MEMBER, new InputArgument[2]
        {
      (InputArgument) (Entity) ped,
      (InputArgument) groupId
        });
    }

    public static bool HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT(Entity entity1, Entity entity2)
    {
        return Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY, new InputArgument[2]
        {
      (InputArgument) entity1,
      (InputArgument) entity2
        });
    }

    

    private void WeaponsAreScary()
    {
        if (Function.Call<bool>(Hash.GET_MISSION_FLAG, Array.Empty<InputArgument>()) || !Game.Player.IsPlaying || (!this.isCarryingWeaponAsProp || IsPlayerInTargetInterior()) || !Main.PED_IS_ARMED(Game.Player.Character) || !carriedWeaponEntity.Exists() || Main.PED_IS_POLICE_FORCE(Game.Player.Character) || !Main.WEAPON_IS_FIREARM(Game.Player.Character.Weapons.Current.Hash) || Game.Player.Character.IsInVehicle())
            return;
        foreach (Ped ped in ((IEnumerable<Ped>)World.GetNearbyPeds(Game.Player.Character, 13f)).ToList<Ped>().ToArray())
        {
            if (Main.HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT((Entity)ped, (Entity)Game.Player.Character))
            {
                if (!Main.PED_IS_POLICE_FORCE(ped) && !IS_PED_IN_GROUP(ped, GET_PLAYER_GROUP(Game.Player)))
                {
                    if (ped.IsInVehicle())
                        break;
                    int playerGroup = Main.GET_PLAYER_GROUP(Game.Player);
                    if (ped.IsAlive && ped.IsHuman && !ped.IsFleeing && !ped.IsShooting && !ped.IsReloading && !ped.IsRagdoll && !ped.IsGettingIntoVehicle && !ped.IsGettingUp && !ped.IsInCombat && ped.IsVisible && !ped.IsInMeleeCombat && !Main.IS_PED_GROUP_MEMBER(ped, playerGroup))
                    {
                        switch (ped.GetRelationshipWithPed(Game.Player.Character))
                        {
                            case Relationship.Dislike:
                                ped.Task.AimAt((Entity)Game.Player.Character, this.Interval);
                                break;
                            case Relationship.Hate:
                                ped.Task.ShootAt(Game.Player.Character, this.Interval);
                                break;
                            case Relationship.Pedestrians:
                                if (ped.Model == new Model(PedHash.Bouncer01SMM) && ped.IsAlive)
                                    return;
                                if ((double)ped.Position.DistanceTo(Game.Player.Character.Position) < 13.0 / 3.0)
                                {
                                    ped.Task.ClearAll();
                                    if (ped.Gender == Gender.Male)
                                    {
                                        ped.Task.FleeFrom(Game.Player.Character);
                                        break;
                                    }
                                    ped.Task.ReactAndFlee(Game.Player.Character);
                                    break;
                                }
                                if (ped.IsInVehicle())
                                {
                                    ped.Task.LookAt((Entity)Game.Player.Character);
                                    break;
                                }
                                ped.Task.TurnTo((Entity)Game.Player.Character);
                                break;
                        }
                    }
                }
                else if (Game.Player.WantedLevel < 1 && !IS_PED_IN_GROUP(ped, GET_PLAYER_GROUP(Game.Player)))
                {
                    ped.Task.Arrest(Game.Player.Character);
                    Game.Player.WantedLevel = 1;
                }
            }
        }
    }

    private void drunkCam()
    {
        if (!Game.Player.Character.IsAlive || Game.IsCutsceneActive || Game.IsLoading || !GameplayCamera.IsRendering)
            return;
        Function.Call(Hash.SHAKE_GAMEPLAY_CAM, new InputArgument[2]
        {
      (InputArgument) "DRUNK_SHAKE",
      (InputArgument) 1f
        });
    }

    public Main()
    {
       
        this.Tick += new EventHandler(this.OnTick);
        this.KeyDown += new KeyEventHandler(this.DropWeapon2);
        this.KeyDown += new KeyEventHandler(this.AttemptPickupWeapon2);
    }

    private void DropWeapon(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.K)
            return;
        int ammo;
        ammo = Game.Player.Character.Weapons.Current.Ammo;
       // Function.Call(Hash.TASK_PLAY_ANIM, (InputArgument)Game.Player.Character, (InputArgument)"anim@heists@narcotics@trash", (InputArgument)"drop_side", (InputArgument)4f, (InputArgument) (- 4f), (InputArgument)2000, (InputArgument)(Enum)AnimationFlags.UpperBodyOnly, (InputArgument)8, (InputArgument)0.2f, (InputArgument)false, (InputArgument)false, (InputArgument)false);

        Function.Call(Hash.SET_PED_DROPS_INVENTORY_WEAPON, new InputArgument[6]
        {
      (InputArgument) (Entity) Game.Player.Character,
      (InputArgument) (int) Game.Player.Character.Weapons.Current.Hash,
      (InputArgument) 0.1,
      (InputArgument) 0.1,
      (InputArgument)(-0.1),
      (InputArgument) ammo
        });
    }

    private void RequestWeaponAsset(WeaponHash hash)
    {
        Function.Call(Hash.REQUEST_WEAPON_ASSET, (InputArgument)(int)hash, (InputArgument)31, (InputArgument)0);
        int gameTime = Game.GameTime;
        while (!Function.Call<bool>(Hash.HAS_WEAPON_ASSET_LOADED, (InputArgument)(int)hash))
        {
            Script.Yield();
            if (Game.GameTime - gameTime > 5000)
            {
                Notification.PostTicker("~r~Timed out loading weapon asset: " + hash.ToString(), true, false);
                break;
            }
        }
    }
    private WeaponProperties GetWeaponProperties(Weapon weapon)
    {
        WeaponProperties weaponProperties = new WeaponProperties()
        {
            Hash = weapon.Hash,
            Ammo = weapon.Ammo,
            TintIndex = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)weapon.Hash),
            Components = new List<WeaponComponentHash>()
        };
        foreach (WeaponComponent component in weapon.Components)
        {
            if (component.Active)
            {
                weaponProperties.Components.Add(component.ComponentHash);
                string str = component.ComponentHash.ToString();
                if (str.Contains("VARMOD_") || str.Contains("CAMO"))
                    weaponProperties.Finish = (int)component.ComponentHash;
            }
        }
        return weaponProperties;
    }
    private Entity GetClosestDroppedWeapon(Vector3 playerPos, float maxDist)
    {
        Entity closestDroppedWeapon = (Entity)null;
        float num1 = maxDist;
        foreach (Entity droppedWeapon in this.droppedWeapons)
        {
            if (droppedWeapon != (Entity)null && droppedWeapon.Exists())
            {
                float num2 = playerPos.DistanceTo(droppedWeapon.Position);
                if ((double)num2 < (double)num1)
                {
                    closestDroppedWeapon = droppedWeapon;
                    num1 = num2;
                }
            }
        }
        return closestDroppedWeapon;
    }

    private void GiveWeaponToPlayer(WeaponProperties properties)
    {
        this.RequestWeaponAsset(properties.Hash);
        Function.Call(Hash.GIVE_WEAPON_TO_PED, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)properties.Hash, (InputArgument)properties.Ammo, (InputArgument)false, (InputArgument)true);
        Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)properties.Hash, (InputArgument)properties.TintIndex);
        foreach (WeaponComponentHash component in properties.Components)
            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)properties.Hash, (InputArgument)(int)component);
        if (properties.Finish != 0)
            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)properties.Hash, (InputArgument)properties.Finish);
        Function.Call(Hash.SET_PED_AMMO, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)properties.Hash, (InputArgument)properties.Ammo);
        Game.Player.Character.Weapons.Select(properties.Hash);
    }

    private void Hint()
    {

        if (this.closestDroppedWeapon == (Entity)null || !this.closestDroppedWeapon.Exists())
            return;
        if ((double)Game.Player.Character.Position.DistanceTo(this.closestDroppedWeapon.Position) <= 1.3300000429153442)
        GTA.UI.Screen.ShowHelpTextThisFrame("~BLIP_INFO_ICON~  ~" + "E" + "~ Pick Up Weapon", true);
    }

    private void DropWeapon2(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.K)
            return;
        Ped character = Game.Player.Character;
        Weapon current = character.Weapons.Current;
        if (current == null || current.Hash == WeaponHash.Unarmed)
            return;
        Function.Call(Hash.REQUEST_ANIM_DICT, (InputArgument)"anim@heists@narcotics@trash");
        while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, (InputArgument)"anim@heists@narcotics@trash"))
            Script.Wait(50);
        WeaponProperties weaponProperties = this.GetWeaponProperties(current);
        Function.Call(Hash.TASK_PLAY_ANIM, (InputArgument)character.Handle, (InputArgument)"anim@heists@narcotics@trash", (InputArgument)"drop_side", (InputArgument)4f, (InputArgument) (- 4f), (InputArgument)2000, (InputArgument)(Enum)AnimationFlags.UpperBodyOnly, (InputArgument)8, (InputArgument)0.2f, (InputArgument)false, (InputArgument)false, (InputArgument)false);
       // Script.Wait(0);
        Entity entity = Function.Call<Entity>(Hash.GET_CURRENT_PED_WEAPON_ENTITY_INDEX, (InputArgument)(Entity)character, (InputArgument)(int)current.Hash);
        if (entity != (Entity)null && entity.Exists())
        {
            Vector3 position = entity.Position;
            Vector3 rotation = entity.Rotation;
            this.RequestWeaponAsset(weaponProperties.Hash);
            Entity key = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)weaponProperties.Hash, (InputArgument)1, (InputArgument)position.X, (InputArgument)position.Y, (InputArgument)position.Z, (InputArgument)true, (InputArgument)1f, (InputArgument)0);
            if (key == (Entity)null || !key.Exists())
                return;
            Function.Call(Hash.SET_ENTITY_ROTATION, (InputArgument)key.Handle, (InputArgument)rotation.X, (InputArgument)rotation.Y, (InputArgument)rotation.Z, (InputArgument)2, (InputArgument)true);
            Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)key.Handle, (InputArgument)weaponProperties.TintIndex);
            foreach (WeaponComponentHash component in weaponProperties.Components)
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)key.Handle, (InputArgument)(int)component);
            if (weaponProperties.Finish != 0)
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)key.Handle, (InputArgument)weaponProperties.Finish);
            Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)key.Handle, (InputArgument)weaponProperties.TintIndex);
            Function.Call(Hash.SET_ENTITY_DYNAMIC, (InputArgument)key.Handle, (InputArgument)true);
            Vector3 forwardVector = character.ForwardVector;
            Vector3 vector3 = new Vector3(forwardVector.X * 2f, forwardVector.Y * 2f, 2f);
            Function.Call(Hash.APPLY_FORCE_TO_ENTITY, (InputArgument)key.Handle, (InputArgument)1, (InputArgument)vector3.X, (InputArgument)vector3.Y, (InputArgument)vector3.Z, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)0, (InputArgument)false, (InputArgument)true, (InputArgument)true, (InputArgument)false, (InputArgument)true);
            Function.Call(Hash.SET_ENTITY_HAS_GRAVITY, (InputArgument)key.Handle, (InputArgument)true);
            this.droppedWeapons.Add(key);
            this.weaponProperties[key] = weaponProperties;
        }
        character.Weapons.Select(WeaponHash.Unarmed);
        character.Task.ClearAll();
        character.Weapons.Remove(current);
    }

    private void DropWeapon3()
    {
        
        Ped character = Game.Player.Character;
        Weapon current = character.Weapons.Current;
        if (current == null || current.Hash == WeaponHash.Unarmed)
            return;
        Function.Call(Hash.REQUEST_ANIM_DICT, (InputArgument)"anim@heists@narcotics@trash");
        while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, (InputArgument)"anim@heists@narcotics@trash"))
            Script.Wait(50);
        WeaponProperties weaponProperties = this.GetWeaponProperties(current);
        Function.Call(Hash.TASK_PLAY_ANIM, (InputArgument)character.Handle, (InputArgument)"anim@heists@narcotics@trash", (InputArgument)"drop_side", (InputArgument)4f, (InputArgument)(-4f), (InputArgument)2000, (InputArgument)(Enum)AnimationFlags.UpperBodyOnly, (InputArgument)8, (InputArgument)0.2f, (InputArgument)false, (InputArgument)false, (InputArgument)false);
        // Script.Wait(0);
        Entity entity = Function.Call<Entity>(Hash.GET_CURRENT_PED_WEAPON_ENTITY_INDEX, (InputArgument)(Entity)character, (InputArgument)(int)current.Hash);
        if (entity != (Entity)null && entity.Exists())
        {
            Vector3 position = entity.Position;
            Vector3 rotation = entity.Rotation;
            this.RequestWeaponAsset(weaponProperties.Hash);
            Entity key = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)weaponProperties.Hash, (InputArgument)1, (InputArgument)position.X, (InputArgument)position.Y, (InputArgument)position.Z, (InputArgument)true, (InputArgument)1f, (InputArgument)0);
            if (key == (Entity)null || !key.Exists())
                return;
            Function.Call(Hash.SET_ENTITY_ROTATION, (InputArgument)key.Handle, (InputArgument)rotation.X, (InputArgument)rotation.Y, (InputArgument)rotation.Z, (InputArgument)2, (InputArgument)true);
            Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)key.Handle, (InputArgument)weaponProperties.TintIndex);
            foreach (WeaponComponentHash component in weaponProperties.Components)
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)key.Handle, (InputArgument)(int)component);
            if (weaponProperties.Finish != 0)
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)key.Handle, (InputArgument)weaponProperties.Finish);
            Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)key.Handle, (InputArgument)weaponProperties.TintIndex);
            Function.Call(Hash.SET_ENTITY_DYNAMIC, (InputArgument)key.Handle, (InputArgument)true);
            Vector3 forwardVector = character.ForwardVector;
            Vector3 vector3 = new Vector3(forwardVector.X * 2f, forwardVector.Y * 2f, 2f);
            Function.Call(Hash.APPLY_FORCE_TO_ENTITY, (InputArgument)key.Handle, (InputArgument)1, (InputArgument)vector3.X, (InputArgument)vector3.Y, (InputArgument)vector3.Z, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)0, (InputArgument)false, (InputArgument)true, (InputArgument)true, (InputArgument)false, (InputArgument)true);
            Function.Call(Hash.SET_ENTITY_HAS_GRAVITY, (InputArgument)key.Handle, (InputArgument)true);
            this.droppedWeapons.Add(key);
            this.weaponProperties[key] = weaponProperties;
        }
        
        //character.Weapons.Select(WeaponHash.Unarmed);
        character.Task.ClearAll();
        character.Weapons.Remove(current);
    }
    private void RagdollDrop()
    {

        Ped character = Game.Player.Character;
        Weapon current = character.Weapons.Current;
        if (current == null || current.Hash == WeaponHash.Unarmed)
            return;
        WeaponProperties weaponProperties = this.GetWeaponProperties(current);
        Entity entity = Function.Call<Entity>(Hash.GET_CURRENT_PED_WEAPON_ENTITY_INDEX, (InputArgument)(Entity)character, (InputArgument)(int)current.Hash);
        if (entity != (Entity)null && entity.Exists())
        {
            Vector3 position = entity.Position;
            Vector3 rotation = entity.Rotation;
            this.RequestWeaponAsset(weaponProperties.Hash);
            Entity key = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)weaponProperties.Hash, (InputArgument)1, (InputArgument)position.X, (InputArgument)position.Y, (InputArgument)position.Z, (InputArgument)true, (InputArgument)1f, (InputArgument)0);
            if (key == (Entity)null || !key.Exists())
                return;
            Function.Call(Hash.SET_ENTITY_ROTATION, (InputArgument)key.Handle, (InputArgument)rotation.X, (InputArgument)rotation.Y, (InputArgument)rotation.Z, (InputArgument)2, (InputArgument)true);
            Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)key.Handle, (InputArgument)weaponProperties.TintIndex);
            foreach (WeaponComponentHash component in weaponProperties.Components)
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)key.Handle, (InputArgument)(int)component);
            if (weaponProperties.Finish != 0)
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)key.Handle, (InputArgument)weaponProperties.Finish);
            Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)key.Handle, (InputArgument)weaponProperties.TintIndex);
            Function.Call(Hash.SET_ENTITY_DYNAMIC, (InputArgument)key.Handle, (InputArgument)true);
            Vector3 forwardVector = character.ForwardVector;
            Vector3 vector3 = new Vector3(forwardVector.X * 0f, forwardVector.Y * 2f, 0f);
            Function.Call(Hash.APPLY_FORCE_TO_ENTITY, (InputArgument)key.Handle, (InputArgument)1, (InputArgument)vector3.X, (InputArgument)vector3.Y, (InputArgument)vector3.Z, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)0, (InputArgument)false, (InputArgument)true, (InputArgument)true, (InputArgument)false, (InputArgument)true);
            Function.Call(Hash.SET_ENTITY_HAS_GRAVITY, (InputArgument)key.Handle, (InputArgument)true);
            this.droppedWeapons.Add(key);
            this.weaponProperties[key] = weaponProperties;
        }

        //character.Weapons.Select(WeaponHash.Unarmed);
        character.Task.ClearAll();
        character.Weapons.Remove(current);
    }



    private void AttemptPickupWeapon2(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.E || !Game.Player.Character.IsOnFoot || Game.IsControlPressed(GTA.Control.Sprint) || (GetBigWeaponCount(Game.Player.Character) > 0 && Game.Player.Character.Weapons.Current == WeaponHash.Unarmed))
            return;
        Entity closestDroppedWeapon = this.GetClosestDroppedWeapon(Game.Player.Character.Position, 3f);
        if (closestDroppedWeapon == (Entity)null || !this.weaponProperties.ContainsKey(closestDroppedWeapon))
        {
            GTA.UI.Screen.ShowSubtitle("No weapon or NPC nearby to pick up.", 2000);
        }
        else
        {
            if (Game.Player.Character.Weapons.Current != null || Main.GetBigWeaponCount(Game.Player.Character) >= 2)
                this.DropWeapon3();
            this.weaponToAttach = closestDroppedWeapon;
            float num1 = this.weaponToAttach.Position.Z - Function.Call<Vector3>(Hash.GET_PED_BONE_COORDS, (InputArgument)(Entity)Game.Player.Character, (InputArgument)24816, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)0.0f).Z;
            float num2 = 0.5f;
            float num3 = 0.3353f;
            if ((double)num1 < -(double)num2)
                this.PlayPickupAnimation(Game.Player.Character);
            else if ((double)num1 > (double)num3)
                this.PlayHighPickupAnimation(Game.Player.Character);
            else
                this.PlayMidPickupAnimation(Game.Player.Character);
        }
    }


    private void weaponPickupTick()
    {
        if (this.weaponToAttach != (Entity)null)
        {
            WeaponProperties weaponProperty = this.weaponProperties[this.weaponToAttach];
            this.GiveWeaponToPlayer(weaponProperty);
            this.droppedWeapons.Remove(this.weaponToAttach);
            this.weaponProperties.Remove(this.weaponToAttach);
            this.weaponToAttach.Delete();
            this.weaponToAttach = (Entity)null;
            Game.Player.Character.Task.ClearAll();
        }
        if (AllWeaponsCount(Game.Player.Character) > 1)
        {
                DropWeapon3();
        }
        if (GetBigWeaponCount(Game.Player.Character) > 0 && Game.Player.Character.Armor == 0 && Game.Player.Character.Weapons.Current != WeaponHash.Unarmed && !IsPlayerInTargetInterior())
        {

            //Function.Call(Hash.SET_CAN_PED_SELECT_INVENTORY_WEAPON, (InputArgument)Game.Player.Character, (InputArgument)WeaponHash.Unarmed, (InputArgument)false);
           // Game.Player.Character.CanSwitchWeapons = false;
        }
        else
        {
          // Game.Player.Character.CanSwitchWeapons = true;
           // Function.Call(Hash.SET_CAN_PED_SELECT_INVENTORY_WEAPON, (InputArgument)Game.Player.Character, (InputArgument)WeaponHash.Unarmed, (InputArgument)true);
        }
            //if (!Game.Player.Character.IsAlive && weaponToAttach != null)
        //    this.weaponToAttach = (Entity)null;

        if (Function.Call<bool>(Hash.IS_PED_USING_ACTION_MODE, (InputArgument)Game.Player.Character))
            Function.Call(Hash.SET_PED_USING_ACTION_MODE, (InputArgument)Game.Player.Character, (InputArgument)false, (InputArgument)(-1), (InputArgument)"DEFAULT_ACTION");

        // if(!IsPlayerInTargetInterior() && Game.Player.Character.Armor == 0)
        // SwitchToFirstAvailableWeaponIfUnarmed(preferredWeapons);

        if (GetBigWeaponCount(Game.Player.Character) > 0)
            {
                if (!Game.IsControlJustPressed(GTA.Control.Context))
                {
                    Script.Wait(50);
                }
                if
                (!Function.Call<bool>(Hash.IS_PED_ARMED, (InputArgument)(Entity)Game.Player.Character, (InputArgument)7) || Game.Player.Character.Weapons.Current == WeaponHash.Unarmed)
                {
                    if (!Game.Player.Character.IsInVehicle() || !Function.Call<bool>(Hash.IS_PED_GETTING_INTO_A_VEHICLE, (InputArgument)Game.Player.Character))
                        ToggleCarryModeOn();
                }
                else
                {

                    ToggleCarryModeOff();
                }

            }

        
        else if (this.isCarryingWeaponAsProp)
            ToggleCarryModeOff();

        //if (IsPlayerInTargetInterior())
          //  ToggleCarryModeOn();

        if (Game.Player.Character.IsInVehicle() || !Game.Player.Character.IsAlive)
            ToggleCarryModeOff();





        /*     if (Game.Player.Character.Weapons.Current.Ammo > 0 && Game.Player.Character.Weapons.Current.Hash != WeaponHash.Unarmed)
             {
                 Weapon blue = Game.Player.Character.Weapons.Current;
                 if (blue.Ammo == 0)
                 {
                     SwitchToWeaponIfUnarmed(blue);
                 }
             }*/
        //   int interiorID = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character);
        //   GTA.UI.Screen.ShowSubtitle("Current Interior ID: " + interiorID);


    }

    private void addComponents()
    {

        Weapon current = Game.Player.Character.Weapons.Current;
        if (current == null || current.Hash == WeaponHash.Unarmed)
            return;
        this.storedWeaponProps = new WeaponProperties()
        {
            Hash = current.Hash,
            Ammo = current.Ammo,
            TintIndex = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)current.Hash)
        };
        foreach (WeaponComponent component in current.Components)
        {
            if (component.Active)
                this.storedWeaponProps.Components.Add(component.ComponentHash);
        }
    }

    public static void SwitchToFirstAvailableWeaponIfUnarmed(List<WeaponHash> weaponList)
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return;

        // Check if player is currently unarmed
        if (player.Weapons.Current.Hash != WeaponHash.Unarmed) return;

        foreach (WeaponHash weapon in weaponList)
        {
            if (player.Weapons.HasWeapon(weapon))
            {
                player.Weapons.Select(weapon);
               GTA.UI.Screen.ShowSubtitle("Switched to: " + weapon.ToString());
                return;
            }
        }

        GTA.UI.Screen.ShowSubtitle("No weapons from list found in inventory.");
    }


    public static bool IsPlayerInTargetInterior()
    {
        int currentInterior = InteriorUtils.GetPlayerInteriorID();
        return TargetInteriorIDs.Contains(currentInterior);
    }


    private void PlayPickupAnimation(Ped playerPed)
    {
        Function.Call(Hash.REQUEST_ANIM_DICT, (InputArgument)"amb@medic@standing@kneel@base");
        while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, (InputArgument)"amb@medic@standing@kneel@base"))
            Script.Wait(10);
        Function.Call(Hash.TASK_PLAY_ANIM, (InputArgument)playerPed.Handle, (InputArgument)"amb@medic@standing@kneel@base", (InputArgument)"base", (InputArgument)4f, (InputArgument) (- 4f), (InputArgument)2500, (InputArgument)(Enum)AnimationFlags.Loop, (InputArgument)0.2f, (InputArgument)false, (InputArgument)false, (InputArgument)true);
        Function.Call(Hash.REQUEST_ANIM_DICT, (InputArgument)"anim@move_m@trash");
        while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, (InputArgument)"anim@move_m@trash"))
            Script.Wait(10);
        Script.Wait(300);
        if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)playerPed.Handle, (InputArgument)"anim@move_m@trash", (InputArgument)"pickup", (InputArgument)3))

            Function.Call(Hash.TASK_PLAY_ANIM, (InputArgument)playerPed.Handle, (InputArgument)"anim@move_m@trash", (InputArgument)"pickup", (InputArgument)8f, (InputArgument)(-8f), (InputArgument)1000, (InputArgument)(Enum)(AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary), (InputArgument)0.3f, (InputArgument)false, (InputArgument)false, (InputArgument)true);
        //this.kneelStartTime = Game.GameTime;
    }

    private void PlayMidPickupAnimation(Ped playerPed)
    {
        string str = "anim@scripted@player@freemode@gen_grab@male@";
        Function.Call(Hash.REQUEST_ANIM_DICT, (InputArgument)str);
        while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, (InputArgument)str))
            Script.Wait(10);
        if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)playerPed.Handle, (InputArgument)str, (InputArgument)"medrh", (InputArgument)3))

            Function.Call(Hash.TASK_PLAY_ANIM, (InputArgument)playerPed.Handle, (InputArgument)str, (InputArgument)"med_rh", (InputArgument)4f, (InputArgument)(-4f), (InputArgument)1500, (InputArgument)(Enum)(AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary), (InputArgument)0.3f, (InputArgument)false, (InputArgument)false, (InputArgument)true);
        Script.Wait(300);
    }

    private void PlayHighPickupAnimation(Ped playerPed)
    {
        string str = "anim@scripted@player@freemode@gen_grab@male@";
        Function.Call(Hash.REQUEST_ANIM_DICT, (InputArgument)str);
        while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, (InputArgument)str))
            Script.Wait(10);
        if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)playerPed.Handle, (InputArgument)str, (InputArgument)"high_rh", (InputArgument)3))

            Function.Call(Hash.TASK_PLAY_ANIM, (InputArgument)playerPed.Handle, (InputArgument)str, (InputArgument)"high_rh", (InputArgument)4f, (InputArgument)(-4f), (InputArgument)1500, (InputArgument)(Enum)(AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary), (InputArgument)0.3f, (InputArgument)false, (InputArgument)false, (InputArgument)true);
        Script.Wait(300);
    }


    private void HurtScreenFX()
    {
        if (!Game.Player.Character.IsAlive)
            return;
        float num1 = this.HealthReference - (float)Game.Player.Character.Health;
        if ((double)num1 > 5.0)
        {
            float num2 = num1 * 100f;
            if ((double)num2 > 1000.0)
                num2 = 1000f;
            if ((double)num2 < 5.0)
                num2 = 5f;
            Function.Call(Hash.ANIMPOSTFX_PLAY, new InputArgument[3]
            {
        (InputArgument) "MinigameEndTrevor",
        (InputArgument) (int) num2,
        (InputArgument) false
            });
        }
        if ((double)this.ArmorReference - (double)Game.Player.Character.Armor <= 5.0)
            return;
        Function.Call(Hash.ANIMPOSTFX_PLAY, new InputArgument[3]
        {
      (InputArgument) "FocusOut",
      (InputArgument) 500,
      (InputArgument) false
        });
    }

    private void noHealthRegeneration()
    {
        Function.Call(Hash.SET_PLAYER_HEALTH_RECHARGE_MULTIPLIER, Game.Player, 0.0);
    }
    private void lowHealthFX()
    {
        if (Game.Player.Character.Health < 50)
        {
            if (Game.Player.Character.Health < 25)
            {
                if (Function.Call<bool>(Hash.ANIMPOSTFX_IS_RUNNING, new InputArgument[1]
                {
          (InputArgument) this.MedH
                }))
                    Function.Call(Hash.ANIMPOSTFX_STOP, new InputArgument[1]
                    {
            (InputArgument) this.MedH
                    });
                if (Function.Call<bool>(Hash.ANIMPOSTFX_IS_RUNNING, new InputArgument[1]
                {
          (InputArgument) this.LowH
                }))
                    return;
                Function.Call(Hash.ANIMPOSTFX_PLAY, new InputArgument[3]
                {
          (InputArgument) this.LowH,
          (InputArgument) 0,
          (InputArgument) true
                });
            }
            else
            {
                if (Function.Call<bool>(Hash.ANIMPOSTFX_IS_RUNNING, new InputArgument[1]
                {
          (InputArgument) this.LowH
                }))
                    Function.Call(Hash.ANIMPOSTFX_STOP, new InputArgument[1]
                    {
            (InputArgument) this.LowH
                    });
                if (!Function.Call<bool>(Hash.ANIMPOSTFX_IS_RUNNING, new InputArgument[1]
                {
          (InputArgument) this.MedH
                }))
                    Function.Call(Hash.ANIMPOSTFX_PLAY, new InputArgument[3]
                    {
            (InputArgument) this.MedH,
            (InputArgument) 0,
            (InputArgument) true
                    });
            }
        }
        else
        {
            if (Function.Call<bool>(Hash.ANIMPOSTFX_IS_RUNNING, new InputArgument[1]
            {
        (InputArgument) this.MedH
            }))
            {
                Function.Call(Hash.ANIMPOSTFX_STOP, new InputArgument[1]
                {
          (InputArgument) this.MedH
                });
                Function.Call(Hash.ANIMPOSTFX_PLAY, new InputArgument[3]
                {
          (InputArgument) "MinigameendFranklin",
          (InputArgument) 1000,
          (InputArgument) false
                });
            }
            if (Function.Call<bool>(Hash.ANIMPOSTFX_IS_RUNNING, new InputArgument[1]
            {
        (InputArgument) this.LowH
            }))
            {
                Function.Call(Hash.ANIMPOSTFX_STOP, new InputArgument[1]
                {
          (InputArgument) this.LowH
                });
                Function.Call(Hash.ANIMPOSTFX_PLAY, new InputArgument[3]
                {
          (InputArgument) "MinigameendFranklin",
          (InputArgument) 1000,
          (InputArgument) false
                });
            }
        }
    }

    private void SlowMo ()
    {
        Ped player = Game.Player.Character; // player variable
        Vehicle veh = Game.Player.Character.CurrentVehicle; // vehicle variable
        bool invehicle = Function.Call<bool>(Hash.IS_PED_SITTING_IN_ANY_VEHICLE, player);
        if (Game.IsControlPressed(GTA.Control.Aim) && Game.IsControlPressed(GTA.Control.PhoneDown)) //Turn on script with Aim key + Dpad Down
        {
            if (SlowMotoggle)
            {
                SlowMotoggle = false;
                GTA.Native.Function.Call(Hash.SET_TIME_SCALE, 1.0f); //return to normal speed when script is OFF
                Wait(300);
            }
            else if (!SlowMotoggle)
            {
                SlowMotoggle = true;
                GTA.Native.Function.Call(Hash.SET_TIME_SCALE, 0.25f); //How slow the game will be when the script is enabled (Default = 25% of normal speed)
                Wait(300);
            }
        }
        if (invehicle == true && SlowMotoggle)
        {
            GTA.Native.Function.Call(Hash.SET_TIME_SCALE, 0.2f); //How slow the game will be when the script is enabled and you are in a vehicle (Default = 20% of normal speed)
        }
        if (invehicle == true && !SlowMotoggle)
        {
            GTA.Native.Function.Call(Hash.SET_TIME_SCALE, 1.0f); //return to normal speed when script is OFF while in a vehicle
        }


    }
    private void firstPersonTransitionFX()
    {
        if (Game.Player.Character.IsSittingInVehicle())
        {
            if (Function.Call<int>(Hash.GET_FOLLOW_VEHICLE_CAM_VIEW_MODE, new InputArgument[0]) == 4 && this.Camera != 4)
                Function.Call(Hash.ENABLE_ALIEN_BLOOD_VFX, new InputArgument[3]
                {
          (InputArgument) "FocusOut",
          (InputArgument) 500,
          (InputArgument) false
                });
            this.Camera = Function.Call<int>(Hash.GET_FOLLOW_VEHICLE_CAM_VIEW_MODE, new InputArgument[0]);
        }
        else
        {
            if (Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE, new InputArgument[0]) == 4 && this.Camera != 4)
            {
                string str = "SwitchHUDMichaelOut";
                if (Game.Player.Character.Model == (Model)"player_zero")
                    str = "SwitchHUDMichaelOut";
                if (Game.Player.Character.Model == (Model)"player_one")
                    str = "SwitchHUDFranklinOut";
                if (Game.Player.Character.Model == (Model)"player_two")
                    str = "SwitchHUDTrevorOut";
                Function.Call(Hash.ENABLE_ALIEN_BLOOD_VFX, new InputArgument[3]
                {
          (InputArgument) str,
          (InputArgument) 1500,
          (InputArgument) false
                });
            }
            this.Camera = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE, new InputArgument[0]);
        }
    }

    private void ToggleCarryModeOn()
    {
        if (Function.Call<bool>(Hash.IS_PED_GETTING_INTO_A_VEHICLE, (InputArgument)Game.Player.Character) || Game.Player.Character.IsInVehicle())
        return;
        Ped character = Game.Player.Character;
        if (!this.isCarryingWeaponAsProp)
        {
            Weapon current3 = character.Weapons.Current;
            WeaponHash current = character.Weapons.Current;
            foreach (Main.BigWeapons bigWeapons in Enum.GetValues(typeof(Main.BigWeapons)))
            {
                if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
                {
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) bigWeapons.GetHashCode(),
            (InputArgument) false
                }))
                {
                   current  = (WeaponHash)bigWeapons.GetHashCode();
                    current3 = character.Weapons[current];
                    break;
                }
            }
                    
            if (current3 == null || current3 == WeaponHash.Unarmed)
                return;
            
            
            this.storedWeaponProps = new WeaponProperties()
            {
                Hash = current3.Hash,
                Ammo = current3.Ammo,
                TintIndex = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, (InputArgument)character.Handle, (InputArgument)(int)current3.Hash)
            };
            foreach (WeaponComponent component in current3.Components)
            {
                if (component.Active)
                    this.storedWeaponProps.Components.Add(component.ComponentHash);
            }
            //character.Weapons.Remove(current.Hash);
            character.Weapons.Select(WeaponHash.Unarmed);
            this.carriedWeaponEntity = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)character.Position.X, (InputArgument)character.Position.Y, (InputArgument)(character.Position.Z + 0.2f), (InputArgument)true, (InputArgument)0.0f, (InputArgument)0);
            if (this.carriedWeaponEntity == (Entity)null || !this.carriedWeaponEntity.Exists())
            {
                Notification.PostTicker("Failed to create carried weapon prop.", true, false);
            }
            else 
            {
                Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)this.storedWeaponProps.TintIndex);
                foreach (int component in this.storedWeaponProps.Components)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)component);
                int num = Function.Call<int>(Hash.GET_PED_BONE_INDEX, (InputArgument)character.Handle, (InputArgument)18905);
                if (Game.Player.Character.Armor == 0)
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)character.Handle, (InputArgument)num, (InputArgument)0.15f, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument)(-110f), (InputArgument)(-50f), (InputArgument)0.0f, (InputArgument)false, (InputArgument)false, (InputArgument)false, (InputArgument)false, (InputArgument)2, (InputArgument)true);
                else
                {
                    int num2 = Function.Call<int>(Hash.GET_PED_BONE_INDEX, new InputArgument[2]
                {
          (InputArgument) (Entity) Game.Player.Character,
          (InputArgument) (Enum) Bone.SkelSpine3
                });
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                        {
            (InputArgument) (InputArgument) this.carriedWeaponEntity,
            (InputArgument) (InputArgument) character.Handle,
            (InputArgument) num2,
            (InputArgument) 0.075f,
            (InputArgument)(0.235f),
            (InputArgument)(-0.02f),
            (InputArgument) 0.0f,
            (InputArgument) 165f,
            (InputArgument) 0.0f,
            (InputArgument) true,
            (InputArgument) true,
            (InputArgument) false,
            (InputArgument) true,
            (InputArgument) 2,
            (InputArgument) true
                        });
                    }
                this.isCarryingWeaponAsProp = true;
                IsCarryingWeaponAsProp = true;
                
                Notification.PostTicker("~r~Weapon~w~ is now in ~b~C~r~a~y~r~w~r~g~y~b~ M~g~o~y~d~b~e.", true, false);
            }
        }
    }


    private void ToggleCarryModeOff()
    {
        Ped character = Game.Player.Character;
        if (this.isCarryingWeaponAsProp)
        {

            if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
            {
                Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                this.carriedWeaponEntity.Delete();
            }
            if (this.storedWeaponProps != null)
            {
                /*
                this.RequestWeaponAssetInternal(this.storedWeaponProps.Hash);
                Function.Call(Hash.GIVE_WEAPON_TO_PED, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)false, (InputArgument)true);
                Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.TintIndex);
                foreach (WeaponComponentHash component in this.storedWeaponProps.Components)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)(int)component);
                if (this.storedWeaponProps.Finish != 0)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Finish);
                Function.Call(Hash.SET_PED_AMMO, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo);
                */
                //character.Weapons.Select(this.storedWeaponProps.Hash);
            }
            this.isCarryingWeaponAsProp = false;
            IsCarryingWeaponAsProp = false;
            if (Game.Player.Character.IsAlive || !Game.Player.Character.IsInVehicle())
            Notification.PostTicker("~r~Weapon~w~ re-equipped ~b~n~r~o~y~r~w~m~g~a~y~l~w~l~r~y~b~.", true, false);
            

        }
    }


    private void ToggleCarryMode()
    {
        Ped character = Game.Player.Character;
        if (!this.isCarryingWeaponAsProp)
        {
            Weapon current = character.Weapons.Current;
            if (current == null || current.Hash == WeaponHash.Unarmed)
                return;
            this.storedWeaponProps = new WeaponProperties()
            {
                Hash = current.Hash,
                Ammo = current.Ammo,
                TintIndex = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, (InputArgument)character.Handle, (InputArgument)(int)current.Hash)
            };
            foreach (WeaponComponent component in current.Components)
            {
                if (component.Active)
                    this.storedWeaponProps.Components.Add(component.ComponentHash);
            }
            //character.Weapons.Remove(current.Hash);
            character.Weapons.Select(WeaponHash.Unarmed);
            this.carriedWeaponEntity = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)character.Position.X, (InputArgument)character.Position.Y, (InputArgument)(character.Position.Z + 0.2f), (InputArgument)true, (InputArgument)0.0f, (InputArgument)0);
            if (this.carriedWeaponEntity == (Entity)null || !this.carriedWeaponEntity.Exists())
            {
                Notification.PostTicker("Failed to create carried weapon prop.", true, false);
            }
            else
            {
                Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)this.storedWeaponProps.TintIndex);
                foreach (int component in this.storedWeaponProps.Components)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)component);
                int num = Function.Call<int>(Hash.GET_PED_BONE_INDEX, (InputArgument)character.Handle, (InputArgument)18905);
                Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)character.Handle, (InputArgument)num, (InputArgument)0.15f, (InputArgument)0.0f, (InputArgument)0.0f, (InputArgument) (- 110f), (InputArgument)(-50f), (InputArgument)0.0f, (InputArgument)false, (InputArgument)false, (InputArgument)false, (InputArgument)false, (InputArgument)2, (InputArgument)true);
                this.isCarryingWeaponAsProp = true;
                IsCarryingWeaponAsProp = true;
                Notification.PostTicker("Weapon is now in Carry Mode.", true, false);
            }
        }
        else
        {
            if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
            {
                Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                this.carriedWeaponEntity.Delete();
            }
            if (this.storedWeaponProps != null)
            {
                /*
                this.RequestWeaponAssetInternal(this.storedWeaponProps.Hash);
                Function.Call(Hash.GIVE_WEAPON_TO_PED, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)false, (InputArgument)true);
                Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.TintIndex);
                foreach (WeaponComponentHash component in this.storedWeaponProps.Components)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)(int)component);
                if (this.storedWeaponProps.Finish != 0)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Finish);
                Function.Call(Hash.SET_PED_AMMO, (InputArgument)character.Handle, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo);
                */
                character.Weapons.Select(this.storedWeaponProps.Hash);
            }
            this.isCarryingWeaponAsProp = false;
            IsCarryingWeaponAsProp = false;
            Notification.PostTicker("Weapon re‑equipped normally.", true, false);
        }
    }

    private void RequestWeaponAssetInternal(WeaponHash hash)
    {
        Function.Call(Hash.REQUEST_WEAPON_ASSET, (InputArgument)(int)hash, (InputArgument)31, (InputArgument)0);
        int gameTime = Game.GameTime;
        while (!Function.Call<bool>(Hash.HAS_WEAPON_ASSET_LOADED, (InputArgument)(int)hash))
        {
            Script.Yield();
            if (Game.GameTime - gameTime > 5000)
                break;
        }
    }
    /*
    private void VisibleWeaponFunc()
    {
        int num1;
        if (Main.GetBigWeaponCount(Game.Player.Character) >= 1)
            num1 = Function.Call<bool>(Hash.IS_PED_ARMED, new InputArgument[2]
            {
        (InputArgument) (Entity) Game.Player.Character,
        (InputArgument) 7
            }) ? 1 : 0;
        else
            num1 = 0;
        if (num1 != 0)
        {
            Main.model = Game.Player.Character.Weapons.Current.Model;
            this.current1 = Game.Player.Character.Weapons.CurrentWeaponObject;
            this.attached = !this.current1.IsVisible;

            Weapon current = Game.Player.Character.Weapons.Current;

            this.storedWeaponProps = new WeaponProperties()
            {
                Hash = current.Hash,
                Ammo = current.Ammo,
                TintIndex = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)current.Hash)
            };
            foreach (WeaponComponent component in current.Components)
            {
                if (component.Active)
                    this.storedWeaponProps.Components.Add(component.ComponentHash);
            }

        

                if (this.attached && !this.on && Main.GetBigWeaponCount(Game.Player.Character) >= 1 && !Game.Player.IsDead)
            {
               // this.WeaponOnBack = World.CreateProp(Main.model, Game.Player.Character.Position, false, false);


                this.carriedWeaponEntity = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)Game.Player.Character.Position.X, (InputArgument)Game.Player.Character.Position.Y, (InputArgument)(Game.Player.Character.Position.Z + 0.2f), (InputArgument)true, (InputArgument)0.0f, (InputArgument)0);

                foreach (int component in this.storedWeaponProps.Components)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)carriedWeaponEntity.Handle, (InputArgument)component);


                int num2 = Function.Call<int>(Hash.GET_PED_BONE_INDEX, new InputArgument[2]
                {
          (InputArgument) (Entity) Game.Player.Character,
          (InputArgument) (Enum) Bone.SkelSpine3
                });
                if (Game.Player.Character.Armor < 1)
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                    {
            (InputArgument) (Entity) this.carriedWeaponEntity,
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) num2,
            (InputArgument) 0.075f,
            (InputArgument)(-0.15f),
            (InputArgument)(-0.02f),
            (InputArgument) 0.0f,
            (InputArgument) 165f,
            (InputArgument) 0.0f,
            (InputArgument) true,
            (InputArgument) true,
            (InputArgument) false,
            (InputArgument) true,
            (InputArgument) 2,
            (InputArgument) true
                    });
                else if (Game.Player.Character.Armor > 0)
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                    {
            (InputArgument) (Entity) this.carriedWeaponEntity,
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) num2,
            (InputArgument) 0.075f,
            (InputArgument)(0.235f),
            (InputArgument)(-0.02f),
            (InputArgument) 0.0f,
            (InputArgument) 165f,
            (InputArgument) 0.0f,
            (InputArgument) true,
            (InputArgument) true,
            (InputArgument) false,
            (InputArgument) true,
            (InputArgument) 2,
            (InputArgument) true
                    });
                this.on = true;
            }
            else
            {
                if (this.attached || Main.GetBigWeaponCount(Game.Player.Character) < 1 || (Game.Player.Character.IsClimbing || Game.Player.Character.IsVaulting || Game.Player.Character.IsSwimmingUnderWater || Game.Player.Character.IsSwimming || Game.Player.Character.IsGettingIntoVehicle || Game.Player.Character.IsJacking || !this.on) && (!Game.Player.Character.IsFalling && !Game.Player.Character.IsRagdoll || !this.on))
                    return;
                if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))

                    Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary | AnimationFlags.HideWeapon);
                Script.Wait(500);
                //  this.WeaponOnBack.Delete();
                //  this.WeaponOnBack = (Prop)null;
                //if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
             //   {
                  //  Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                  //  this.carriedWeaponEntity.Delete();
             //   }
                //this.carriedWeaponEntity = (Entity)null;
                this.on = false;
                //Main.model = (Model)(string)null;
                //Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary);
            }
        }
        else
        {
            int num3;
            if (Main.GetBigWeaponCount(Game.Player.Character) >= 1 && !Game.Player.Character.IsInVehicle())
            {
                if (Game.Player.Character.Weapons.Current.Hash != WeaponHash.Unarmed)
                    num3 = !Function.Call<bool>(Hash.IS_PED_ARMED, new InputArgument[2]
                    {
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) 7
                    }) ? 1 : 0;
                else
                    num3 = 1;
            }
            else
                num3 = 0;
            if (num3 != 0)
            {
                this.attached = true;
                foreach (Main.BigWeapons bigWeapons in Enum.GetValues(typeof(Main.BigWeapons)))
                {
                    if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
                    {
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) bigWeapons.GetHashCode(),
            (InputArgument) false
                    }))
                    {
                        this.current = bigWeapons.GetHashCode();
                        break;
                    }
                }
                if (this.attached && !this.on && Main.GetBigWeaponCount(Game.Player.Character) >= 1 && !Game.Player.Character.IsDead)
                {
                    if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))
                    Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary);
                    Script.Wait(500);
                   // this.WeaponOnBack = World.CreateProp(Function.Call<Model>(Hash.GET_WEAPONTYPE_MODEL, new InputArgument[1]
                //    {
          //  (InputArgument) this.current
          //          }), Game.Player.Character.Position, false, false);

                    this.carriedWeaponEntity = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)Game.Player.Character.Position.X, (InputArgument)Game.Player.Character.Position.Y, (InputArgument)(Game.Player.Character.Position.Z + 0.2f), (InputArgument)true, (InputArgument)0.0f, (InputArgument)0);

                    foreach (int component in this.storedWeaponProps.Components)
                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)carriedWeaponEntity.Handle, (InputArgument)component);
                   
                    
                    int num4 = Function.Call<int>(Hash.GET_PED_BONE_INDEX, new InputArgument[2]
                    {
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) (Enum) Bone.SkelSpine3
                    });
                    if (Game.Player.Character.Armor < 1)
                        Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                        {
              (InputArgument) (Entity) this.carriedWeaponEntity,
              (InputArgument) (Entity) Game.Player.Character,
              (InputArgument) num4,
              (InputArgument) 0.075f,
              (InputArgument)(-0.15f),
              (InputArgument)(-0.02f),
              (InputArgument) 0.0f,
              (InputArgument) 165f,
              (InputArgument) 0.0f,
              (InputArgument) true,
              (InputArgument) true,
              (InputArgument) false,
              (InputArgument) true,
              (InputArgument) 2,
              (InputArgument) true
                        });
                    else
                        Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                        {
              (InputArgument) (Entity) this.carriedWeaponEntity,
              (InputArgument) (Entity) Game.Player.Character,
              (InputArgument) num4,
              (InputArgument) 0.075f,
              (InputArgument) 0.235f,
              (InputArgument)(-0.02f),
              (InputArgument) 0.0f,
              (InputArgument) 155f,
              (InputArgument) 0.0f,
              (InputArgument) true,
              (InputArgument) true,
              (InputArgument) false,
              (InputArgument) true,
              (InputArgument) 2,
              (InputArgument) true
                        });
                    this.on = true;
                }
                else
                {
                    if (this.attached || Main.GetBigWeaponCount(Game.Player.Character) < 1 || (Game.Player.Character.IsClimbing || Game.Player.Character.IsVaulting || Game.Player.Character.IsSwimmingUnderWater || Game.Player.Character.IsSwimming || Game.Player.Character.IsGettingIntoVehicle || Game.Player.Character.IsJacking || !this.on) && (!Game.Player.Character.IsFalling && !Game.Player.Character.IsRagdoll || !this.on))
                        return;
                    if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))
                    Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary | AnimationFlags.HideWeapon);
                    Script.Wait(500);
                    //  this.WeaponOnBack.Delete();
                    //  this.WeaponOnBack = (Prop)null;
                  //  if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
                  //  {
                       // Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                       // this.carriedWeaponEntity.Delete();
                  //  }
                    // this.carriedWeaponEntity = (Entity)null;

                    this.on = false;
                    //Main.model = (Model)(string)null;
                    //Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary);
                }
            }
            else
            {
                if (Main.GetBigWeaponCount(Game.Player.Character) != 0 || !this.on || !this.on)
                    return;
                if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))

                    Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary | AnimationFlags.HideWeapon);
                Script.Wait(500);
                //this.WeaponOnBack.Delete();
                //this.WeaponOnBack = (Prop)null;
            //    if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
            //    {
                  //  Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                  //  this.carriedWeaponEntity.Delete();
            //    }
                //this.carriedWeaponEntity = (Entity)null;
                this.on = false;
               // Main.model = (Model)(string)null;
                //Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary);
            }
        }
    }


    */
    private void visibleWeapon2()
    {
        int num1;

        if (Main.GetBigWeaponCount(Game.Player.Character) >= 1)
            num1 = Function.Call<bool>(Hash.IS_PED_ARMED, new InputArgument[2]
            {
        (InputArgument) (Entity) Game.Player.Character,
        (InputArgument) 7
            }) ? 1 : 0;
        else
            num1 = 0;
        if (num1 != 0)
        {
           

            Weapon current = Game.Player.Character.Weapons.Current;

            this.current1 = Game.Player.Character.Weapons.CurrentWeaponObject;
            if (current1 == null) return;


            this.attached = !current1.IsVisible;
            if (current == null || current.Hash == WeaponHash.Unarmed)
                return;
            this.storedWeaponProps = new WeaponProperties()
            {
                Hash = current.Hash,
                Ammo = current.Ammo,
                TintIndex = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, (InputArgument)Game.Player.Character.Handle, (InputArgument)(int)current.Hash)
            };
            foreach (WeaponComponent component in current.Components)
            {
                if (component.Active)
                    this.storedWeaponProps.Components.Add(component.ComponentHash);
            }

            

            if (attached && !this.on && Main.GetBigWeaponCount(Game.Player.Character) >= 1 && !Game.Player.IsDead)
            {

                this.carriedWeaponEntity = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)Game.Player.Character.Position.X, (InputArgument)Game.Player.Character.Position.Y, (InputArgument)(Game.Player.Character.Position.Z + 0.2f), (InputArgument)true, (InputArgument)0.0f, (InputArgument)0);

                foreach (int component in this.storedWeaponProps.Components)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)carriedWeaponEntity.Handle, (InputArgument)component);


                int num2 = Function.Call<int>(Hash.GET_PED_BONE_INDEX, new InputArgument[2]
                {
          (InputArgument) (Entity) Game.Player.Character,
          (InputArgument) (Enum) Bone.SkelSpine3
                });
                if (Game.Player.Character.Armor < 1)
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                    {
            (InputArgument) (Entity) this.carriedWeaponEntity,
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) num2,
            (InputArgument) 0.075f,
            (InputArgument)(-0.15f),
            (InputArgument)(-0.02f),
            (InputArgument) 0.0f,
            (InputArgument) 165f,
            (InputArgument) 0.0f,
            (InputArgument) true,
            (InputArgument) true,
            (InputArgument) false,
            (InputArgument) true,
            (InputArgument) 2,
            (InputArgument) true
                    });
                else if (Game.Player.Character.Armor > 0)
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                    {
            (InputArgument) (Entity) this.carriedWeaponEntity,
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) num2,
            (InputArgument) 0.075f,
            (InputArgument)(0.235f),
            (InputArgument)(-0.02f),
            (InputArgument) 0.0f,
            (InputArgument) 165f,
            (InputArgument) 0.0f,
            (InputArgument) true,
            (InputArgument) true,
            (InputArgument) false,
            (InputArgument) true,
            (InputArgument) 2,
            (InputArgument) true
                    });
                this.on = true;
            }
            else
            {
                if (attached || Main.GetBigWeaponCount(Game.Player.Character) < 1 || (Game.Player.Character.IsClimbing || Game.Player.Character.IsVaulting || Game.Player.Character.IsSwimmingUnderWater || Game.Player.Character.IsSwimming || Game.Player.Character.IsGettingIntoVehicle || Game.Player.Character.IsJacking || !this.on) && (!Game.Player.Character.IsFalling && !Game.Player.Character.IsRagdoll || !this.on))
                    return;
                if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))
                   Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary | AnimationFlags.HideWeapon);
                Script.Wait(500);
                //  this.WeaponOnBack.Delete();
                //  this.WeaponOnBack = (Prop)null;
                if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
                   {
                  Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                  this.carriedWeaponEntity.Delete();
                   }
                //this.carriedWeaponEntity = (Entity)null;
                this.on = false;
                //Main.model = (Model)(string)null;
                //Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary);
            }
        }
        else
        {
            int num3;
            if (Main.GetBigWeaponCount(Game.Player.Character) >= 1 && !Game.Player.Character.IsInVehicle())
            {
                if (Game.Player.Character.Weapons.Current.Hash != WeaponHash.Unarmed)
                    num3 = !Function.Call<bool>(Hash.IS_PED_ARMED, new InputArgument[2]
                    {
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) 7
                    }) ? 1 : 0;
                else
                    num3 = 1;
            }
            else
                num3 = 0;
            if (num3 != 0)
            {
                this.attached = true;
                foreach (Main.BigWeapons bigWeapons in Enum.GetValues(typeof(Main.BigWeapons)))
                {
                    if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, new InputArgument[3]
                    {
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) bigWeapons.GetHashCode(),
            (InputArgument) false
                    }))
                    {
                        current = bigWeapons.GetHashCode();
                        
                        break;
                    }
                }
                
                if (attached && !this.on && Main.GetBigWeaponCount(Game.Player.Character) >= 1 && !Game.Player.Character.IsDead)
                {
                    if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))
                        Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary);
                    Script.Wait(500);
                    // this.WeaponOnBack = World.CreateProp(Function.Call<Model>(Hash.GET_WEAPONTYPE_MODEL, new InputArgument[1]
                    //    {
                    //  (InputArgument) this.current
                    //          }), Game.Player.Character.Position, false, false);

                    this.carriedWeaponEntity = Function.Call<Entity>(Hash.CREATE_WEAPON_OBJECT, (InputArgument)(int)this.storedWeaponProps.Hash, (InputArgument)this.storedWeaponProps.Ammo, (InputArgument)Game.Player.Character.Position.X, (InputArgument)Game.Player.Character.Position.Y, (InputArgument)(Game.Player.Character.Position.Z + 0.2f), (InputArgument)true, (InputArgument)0.0f, (InputArgument)0);

                    foreach (int component in this.storedWeaponProps.Components)
                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, (InputArgument)carriedWeaponEntity.Handle, (InputArgument)component);


                    int num4 = Function.Call<int>(Hash.GET_PED_BONE_INDEX, new InputArgument[2]
                    {
            (InputArgument) (Entity) Game.Player.Character,
            (InputArgument) (Enum) Bone.SkelSpine3
                    });
                    if (Game.Player.Character.Armor < 1)
                        Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                        {
              (InputArgument) (Entity) this.carriedWeaponEntity,
              (InputArgument) (Entity) Game.Player.Character,
              (InputArgument) num4,
              (InputArgument) 0.075f,
              (InputArgument)(-0.15f),
              (InputArgument)(-0.02f),
              (InputArgument) 0.0f,
              (InputArgument) 165f,
              (InputArgument) 0.0f,
              (InputArgument) true,
              (InputArgument) true,
              (InputArgument) false,
              (InputArgument) true,
              (InputArgument) 2,
              (InputArgument) true
                        });
                    else
                        Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, new InputArgument[15]
                        {
              (InputArgument) (Entity) this.carriedWeaponEntity,
              (InputArgument) (Entity) Game.Player.Character,
              (InputArgument) num4,
              (InputArgument) 0.075f,
              (InputArgument) 0.235f,
              (InputArgument)(-0.02f),
              (InputArgument) 0.0f,
              (InputArgument) 155f,
              (InputArgument) 0.0f,
              (InputArgument) true,
              (InputArgument) true,
              (InputArgument) false,
              (InputArgument) true,
              (InputArgument) 2,
              (InputArgument) true
                        });
                    this.on = true;
                }
                else
                {
                    if (attached || Main.GetBigWeaponCount(Game.Player.Character) < 1 || (Game.Player.Character.IsClimbing || Game.Player.Character.IsVaulting || Game.Player.Character.IsSwimmingUnderWater || Game.Player.Character.IsSwimming || Game.Player.Character.IsGettingIntoVehicle || Game.Player.Character.IsJacking || !this.on) && (!Game.Player.Character.IsFalling && !Game.Player.Character.IsRagdoll || !this.on))
                        return;
                    if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))
                        Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary | AnimationFlags.HideWeapon);
                    Script.Wait(500);
                    //  this.WeaponOnBack.Delete();
                    //  this.WeaponOnBack = (Prop)null;
                      if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
                      {
                     Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                     this.carriedWeaponEntity.Delete();
                      }
                    // this.carriedWeaponEntity = (Entity)null;

                    this.on = false;
                    //Main.model = (Model)(string)null;
                    //Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary);
                }
            }
            else
            {
                if (Main.GetBigWeaponCount(Game.Player.Character) != 0 || !this.on || !this.on)
                    return;
                if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument)(Entity)Game.Player.Character, (InputArgument)"mp_arrest_paired", (InputArgument)"cop_p1_rf_right_0", (InputArgument)3))

                    Game.Player.Character.Task.PlayAnimation("mp_arrest_paired", "cop_p1_rf_right_0", 8f, 500, AnimationFlags.UpperBodyOnly | AnimationFlags.Secondary | AnimationFlags.HideWeapon);
                Script.Wait(500);
                //this.WeaponOnBack.Delete();
                //this.WeaponOnBack = (Prop)null;
                    if (this.carriedWeaponEntity != (Entity)null && this.carriedWeaponEntity.Exists())
                    {
                  Function.Call(Hash.DETACH_ENTITY, (InputArgument)this.carriedWeaponEntity.Handle, (InputArgument)true, (InputArgument)true);
                  this.carriedWeaponEntity.Delete();
                    
                    }
                //this.carriedWeaponEntity = (Entity)null;
                this.on = false;
            }
        }

    }





    private void RagDollWeaponDrop()
    {
        if (!Game.Player.Character.IsRagdoll)
            return;
        RagdollDrop();
    }

    private void shootDriver()
    {
        if (!Game.Player.Character.IsGettingIntoVehicle)
            return;
        Vehicle vehicleTryingToEnter = Game.Player.Character.VehicleTryingToEnter;
        VehicleSeat seat = (VehicleSeat)Function.Call<int>(Hash.GET_SEAT_PED_IS_TRYING_TO_ENTER, new InputArgument[1]
        {
      (InputArgument) (Entity) Game.Player.Character
        });
        Ped pedOnSeat = vehicleTryingToEnter.GetPedOnSeat(seat);
        if (Game.Player.Character.Weapons.Current.Hash != WeaponHash.Unarmed && Game.IsControlJustPressed(GTA.Control.Attack) && pedOnSeat.IsAlive)
        {
            Game.Player.Character.Accuracy = 100;
            Vector3 vector3 = Function.Call<Vector3>(Hash.GET_PED_BONE_COORDS, new InputArgument[2]
            {
        (InputArgument) (Entity) pedOnSeat,
        (InputArgument) (Enum) Bone.IKHead
            });
            Function.Call(Hash.SET_PED_SHOOTS_AT_COORD, new InputArgument[5]
            {
        (InputArgument) (Entity) Game.Player.Character,
        (InputArgument) vector3.X,
        (InputArgument) vector3.Y,
        (InputArgument) vector3.Z,
        (InputArgument) true
            });
            Script.Wait(500);
            if (pedOnSeat.IsDead)
            {
                Game.Player.Character.Task.ClearAllImmediately();
                Game.Player.Character.Task.EnterVehicle(vehicleTryingToEnter, VehicleSeat.Driver, 2000, 1f , EnterVehicleFlags.None);
            }
        }
    }

    private void RemoveWeaponsWhenDead()
    {
        if (!Game.Player.Character.IsDead)
            return;
        Game.Player.Character.Weapons.RemoveAll();
    }

    private void ShowVisibleArmor()
    {
        if (Game.Player.Character.Armor > 0)
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, new InputArgument[4]
            {
        (InputArgument) (Entity) Game.Player.Character,
        (InputArgument) this.CharacterClothes[Main.Helpers.GetPlayerID(), 0],
        (InputArgument) this.CharacterClothes[Main.Helpers.GetPlayerID(), 1],
        (InputArgument) 0
            });
        else
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, new InputArgument[4]
            {
        (InputArgument) (Entity) Game.Player.Character,
        (InputArgument) this.CharacterClothes[Main.Helpers.GetPlayerID(), 0],
        (InputArgument) this.CharacterClothes[Main.Helpers.GetPlayerID(), 2],
        (InputArgument) 0
            });
    }

    private void GiveWeaponToOthers()
    {

        if (Game.Player.Character.IsSittingInVehicle())
            return;
        if (Game.IsControlPressed(GTA.Control.MultiplayerInfo))
        {


            Ped closestPed = World.GetClosestPed(Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2f, 1f);
            if ((Entity)closestPed != (Entity)null && !closestPed.IsSittingInVehicle() && closestPed.Exists() && closestPed.IsAlive && closestPed.IsHuman)
            {
                if (Game.Player.Character.Weapons.Current.Hash != WeaponHash.Unarmed)
                {
                    GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to give your " + Game.Player.Character.Weapons.Current.Hash.ToString() + " to this NPC.", true);
                    if (Game.IsControlJustReleased(GTA.Control.Context))
                    {
                        List<int> intList = new List<int>();
                        if (!Game.Player.Character.IsSittingInVehicle())
                        {

                            if ((Entity)closestPed != (Entity)null && !closestPed.IsSittingInVehicle() && closestPed.Exists() && closestPed.IsAlive && (Entity)closestPed != (Entity)Game.Player.Character && closestPed.IsHuman)
                            {
                                WeaponHash hash1 = Game.Player.Character.Weapons.Current.Hash;
                                WeaponHash hash2 = closestPed.Weapons.Current.Hash;
                                if (hash1 != WeaponHash.Unarmed && WEAPON_IS_FIREARM(Game.Player.Character.Weapons.Current.Hash))
                                {
                                    foreach (int component in this.components)
                                    {
                                        if (Function.Call<bool>(Hash.HAS_WEAPON_GOT_WEAPON_COMPONENT, (InputArgument)Game.Player.Character.Weapons.CurrentWeaponObject, (InputArgument)component))
                                            intList.Add(component);
                                    }
                                    Game.Player.Character.Weapons.Remove(Game.Player.Character.Weapons.Current);
                                    closestPed.Weapons.Give(hash1, -1, true, true);
                                    foreach (int num in intList)
                                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)closestPed, (InputArgument)(int)closestPed.Weapons.Current.Hash, (InputArgument)num);
                                }
                                else if (hash2 != WeaponHash.Unarmed && WEAPON_IS_FIREARM(closestPed.Weapons.Current.Hash))
                                {
                                    foreach (int component in this.components)
                                    {
                                        if (Function.Call<bool>(Hash.HAS_WEAPON_GOT_WEAPON_COMPONENT, (InputArgument)(int)closestPed.Weapons.Current.Hash, (InputArgument)component))
                                            intList.Add(component);
                                    }
                                    closestPed.Weapons.Remove(closestPed.Weapons.Current);
                                    Game.Player.Character.Weapons.Give(hash2, 300, true, true);
                                    foreach (int num in intList)
                                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)Game.Player.Character, (InputArgument)(int)Game.Player.Character.Weapons.Current.Hash, (InputArgument)num);
                                }
                            }
                        }
                        intList.Clear();
                    }
                }
                else if (closestPed.Weapons.Current.Hash != WeaponHash.Unarmed && WEAPON_IS_FIREARM(closestPed.Weapons.Current.Hash))
                {
                    GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to get this NPC's " + closestPed.Weapons.Current.Hash.ToString() + ".", true);
                    if (Game.IsControlJustReleased(GTA.Control.Context))
                    {
                        List<int> intList = new List<int>();
                        if (!Game.Player.Character.IsSittingInVehicle())
                        {

                            if ((Entity)closestPed != (Entity)null && !closestPed.IsSittingInVehicle() && closestPed.Exists() && closestPed.IsAlive && (Entity)closestPed != (Entity)Game.Player.Character && closestPed.IsHuman)
                            {
                                WeaponHash hash1 = Game.Player.Character.Weapons.Current.Hash;
                                WeaponHash hash2 = closestPed.Weapons.Current.Hash;
                                if (hash1 != WeaponHash.Unarmed && WEAPON_IS_FIREARM(Game.Player.Character.Weapons.Current.Hash))
                                {
                                    foreach (int component in this.components)
                                    {
                                        if (Function.Call<bool>(Hash.HAS_WEAPON_GOT_WEAPON_COMPONENT, (InputArgument)Game.Player.Character.Weapons.CurrentWeaponObject, (InputArgument)component))
                                            intList.Add(component);
                                    }
                                    Game.Player.Character.Weapons.Remove(Game.Player.Character.Weapons.Current);
                                    closestPed.Weapons.Give(hash1, -1, true, true);
                                    foreach (int num in intList)
                                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)closestPed, (InputArgument)(int)closestPed.Weapons.Current.Hash, (InputArgument)num);
                                }
                                else if (hash2 != WeaponHash.Unarmed && WEAPON_IS_FIREARM(closestPed.Weapons.Current.Hash))
                                {
                                    foreach (int component in this.components)
                                    {
                                        if (Function.Call<bool>(Hash.HAS_WEAPON_GOT_WEAPON_COMPONENT, (InputArgument)(int)closestPed.Weapons.Current.Hash, (InputArgument)component))
                                            intList.Add(component);
                                    }
                                    closestPed.Weapons.Remove(closestPed.Weapons.Current);
                                    Game.Player.Character.Weapons.Give(hash2, 300, true, true);
                                    foreach (int num in intList)
                                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, (InputArgument)Game.Player.Character, (InputArgument)(int)Game.Player.Character.Weapons.Current.Hash, (InputArgument)num);
                                }
                            }
                        }
                        intList.Clear();

                    }
                }
            }
        }
    }

    private void jailClothes()
    {
        if (!Game.Player.Character.IsDead)
            return;
        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, new InputArgument[4]
        {
      (InputArgument) (Entity) Game.Player.Character,
      (InputArgument) this.CharacterJailClothes[Main.Helpers.GetPlayerID(), 0],
      (InputArgument) this.CharacterJailClothes[Main.Helpers.GetPlayerID(), 1],
      (InputArgument) 0
        });
    }

    private void DisplayHelpTextThisFrame(string text)
    {
        Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP, new InputArgument[1]
        {
      (InputArgument) "STRING"
        });
        Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, new InputArgument[1]
        {
      (InputArgument) text
        });
        Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_HELP, new InputArgument[4]
        {
      (InputArgument) 0,
      (InputArgument) 0,
      (InputArgument) 1,
      (InputArgument)(-1)
        });
    }
    public static List<long> GET_FIREARM()
    {
        return new List<long>()
        {


             100416529, // 0x05FC3C11SniperRifle 
              125959754, // 0x0781FE4A CompactLauncher
              171789620, // 0x0A3D4D34 CombatPDW
              177293209, // 0x0A914799 HeavySniperMKII
              205991906, // 0x0C472FE2 HeavySniper
              317205821, // 0x12E82D3D SweeperShotgun
              324215364, // 0x13532244 MicroSMG
              487013001, // 0x1D073A89 PumpShotgun
              736523883, // 0x2BE6766B SMG
              961495388, // 0x394F415C AssaultRifleMKII
              984333226, // 0x3AABBBAA HeavyShotgun
             1119849093, // 0x42BF8A85  Minigun
              1198256469, // 0x476BF155 RayCarbine
              1305664598, // 0x4DD2DC56 GrenadeLauncherSmoke
              1432025498, // 0x555AF99A PumpShotgunMKII
              1627465347, // 0x61012683 Gusenberg 
              1649403952, // 0x624FE830 CompactRifle
              1672152130, // 0x63AB0442 HomingLauncher
              1785463520, // 0x6A6C02E0 MarksManRifleMKII
             1834241177, // 0x6D544C99  Railgun
              2017895192, // 0x7846A318 SawnOffShotgun
              2024373456, // 0x78A97CD0 SmgMKII
             2132975508, // 0x7F229F94  BullpupRifle
             2138347493, // 0x7F7497E5  Firework
              2144741730, // 0x7FD62962 CombatMG
              2210333304, // 0x83BF0278 CarbineRifle
              2228681469, // 0x84D6FAFD BullupRifleMKII
              2526821735, // 0x969C3D67 SpecialCarbineMKII
              2634544996, // 0x9D07F764 MG
              2640438543, // 0x9D61E50F BullpupShotgun
              2726580491, // 0xA284510B GrenadeLauncher
             2828843422, // 0xA89CB99E  Musket
              2937143193, // 0xAF113F99 AdvancedRifle
              2982836145, // 0xB1CA77B1 RPG
              3056410471, // 0xB62D1F67 WidowMaker
              3173288789, // 0xBD248B55 MiniSMG
              3220176749, // 0xBFEFFF6D AssaultRifle
             3231910285, // 0xC0A3098D SpecialCarbine 
              3342088282, // 0xC734385A MarksmanRifle
              3686625920, // 0xDBBD7280 CombatMGMKII
              3800352039, // 0xE284C527 AssaultShotgun
              4019527611, // 0xEF951FBB DoubleBarrelShotgun
              4024951519, // 0xEFE7E2DF AssaultSMG
              4208062921, // 0xFAD1F1C9 CarbineRifleMKII
     
              0x14E5AFD5, // TacticalSmg
              0xD1D5F52B, // TacticalRifle
              3249783761, // MilitaryRifle
             
              0x6E7DDDEC, // PrecisionRifle
              0xC78D71B4, // HeavyRifle
              0x72B66B11, // BattleRifle
            137902532, //  VintagePistol
           453432689, // Pistol
           584646201, // APPistol
                                  // JerryCan = 883325847 0x34A67B97
           0x14E5AFD5, //Tactical SMG
            1198879012, // FlareGun
            1593441988, // CombatPistol
            2285322324, // SnsPistolMKII
            2548703416, // 0x97EA20B8 DoubleActionRevolver
            2578377531, // Pistol50
             2939590305, // RayPistol
              3218215474, // SNSPistol
              3219281620, // PistolMKII
              0xC1B3C3D1, // Revolver
              3415619887, // HeavyRevolverMKII
              3523564046, // HeavyPistol
              3675956304, // MachinePistol
              3696079510, // 0xDC4DB296 MarksmanPistol
            0x57A4368C, //PericoPistol
             0x2B5EF5EC, //CeramicPistol
           0x917F6C8C, //NavyRevolver
        };
    }

    List<WeaponHash> preferredWeapons = new List<WeaponHash>
{
WeaponHash.SMG,
WeaponHash.SMGMk2,
WeaponHash.AssaultSMG,
WeaponHash.CombatPDW,
WeaponHash.MachinePistol,
WeaponHash.MiniSMG,
WeaponHash.UnholyHellbringer,
WeaponHash.TacticalSMG,
WeaponHash.PumpShotgun,
WeaponHash.PumpShotgunMk2,
WeaponHash.SawnOffShotgun,
WeaponHash.AssaultShotgun,
WeaponHash.BullpupShotgun,
WeaponHash.DoubleBarrelShotgun,
WeaponHash.SweeperShotgun,
WeaponHash.AssaultRifle,
WeaponHash.AssaultrifleMk2,
WeaponHash.CarbineRifle,
WeaponHash.CarbineRifleMk2,
WeaponHash.AdvancedRifle,
WeaponHash.SpecialCarbine,
WeaponHash.SpecialCarbineMk2,
WeaponHash.BullpupRifle,
WeaponHash.BullpupRifleMk2,
WeaponHash.CompactRifle,
WeaponHash.MilitaryRifle,
WeaponHash.HeavyRifle,
WeaponHash.MG,
WeaponHash.CombatMG,
WeaponHash.CombatMGMk2,
WeaponHash.Gusenberg,
WeaponHash.SniperRifle,
WeaponHash.HeavySniper,
WeaponHash.HeavySniperMk2,
WeaponHash.MarksmanRifle,
WeaponHash.MarksmanRifleMk2,
WeaponHash.RPG,
WeaponHash.GrenadeLauncher,
WeaponHash.GrenadeLauncherSmoke,
WeaponHash.Minigun,
WeaponHash.Firework,
WeaponHash.Railgun,
WeaponHash.HomingLauncher,
WeaponHash.CompactGrenadeLauncher,
WeaponHash.PrecisionRifle,
WeaponHash.Widowmaker
};

    private enum AllWeapons : uint
    {
        SniperRifle = 100416529, // 0x05FC3C11
        CompactLauncher = 125959754, // 0x0781FE4A
        VintagePistol = 137902532, // 0x083839C4
        CombatPDW = 171789620, // 0x0A3D4D34
        HeavySniperMKII = 177293209, // 0x0A914799
        HeavySniper = 205991906, // 0x0C472FE2
        SweeperShotgun = 317205821, // 0x12E82D3D
        MicroSMG = 324215364, // 0x13532244
        PipeWrench = 419712736, // 0x19044EE0
        Pistol = 453432689, // 0x1B06D571
        PumpShotgun = 487013001, // 0x1D073A89
        APPistol = 584646201, // 0x22D8FE39
        Cocktail = 615608432, // 0x24B17070
        SMG = 736523883, // 0x2BE6766B
        StickyBomb = 741814745, // 0x2C3731D9
        JerryCan = 883325847, // 0x34A67B97
        StunGun = 911657153, // 0x3656C8C1
        StoneHatchet = 940833800, // 0x3813FC08
        AssaultRifleMKII = 961495388, // 0x394F415C
        HeavyShotgun = 984333226, // 0x3AABBBAA
        Minigun = 1119849093, // 0x42BF8A85
        GolfClub = 1141786504, // 0x440E4788
        RayCarbine = 1198256469, // 0x476BF155
        FlareGun = 1198879012, // 0x47757124
        GrenadeLauncherSmoke = 1305664598, // 0x4DD2DC56
        Hammer = 1317494643, // 0x4E875F73
        PumpShotgunMKII = 1432025498, // 0x555AF99A
        CombatPistol = 1593441988, // 0x5EF9FEC4
        Gusenberg = 1627465347, // 0x61012683
        CompactRifle = 1649403952, // 0x624FE830
        HomingLauncher = 1672152130, // 0x63AB0442
        NightStick = 1737195953, // 0x678B81B1
        MarksManRifleMKII = 1785463520, // 0x6A6C02E0
        Railgun = 1834241177, // 0x6D544C99
        SawnOffShotgun = 2017895192, // 0x7846A318
        SmgMKII = 2024373456, // 0x78A97CD0
        BullpupRifle = 2132975508, // 0x7F229F94
        Firework = 2138347493, // 0x7F7497E5
        CombatMG = 2144741730, // 0x7FD62962
        CarbineRifle = 2210333304, // 0x83BF0278
        CrowBar = 2227010557, // 0x84BD7BFD
        BullupRifleMKII = 2228681469, // 0x84D6FAFD
        SnsPistolMKII = 2285322324, // 0x88374054
        Grenade = 2481070269, // 0x93E220BD
        PoolCue = 2484171525, // 0x94117305
        Bat = 2508868239, // 0x958A4A8F
        SpecialCarbineMKII = 2526821735, // 0x969C3D67
        DoubleActionRevolver = 2548703416, // 0x97EA20B8
        Pistol50 = 2578377531, // 0x99AEEB3B
        MG = 2634544996, // 0x9D07F764
        BullpupShotgun = 2640438543, // 0x9D61E50F
        BZ = 2694266206, // 0xA0973D5E
        GrenadeLauncher = 2726580491, // 0xA284510B
        Musket = 2828843422, // 0xA89CB99E
        ProximityMine = 2874559379, // 0xAB564B93
        AdvancedRifle = 2937143193, // 0xAF113F99
        RayPistol = 2939590305, // 0xAF3696A1
        RPG = 2982836145, // 0xB1CA77B1
        WidowMaker = 3056410471, // 0xB62D1F67
        PipeBomb = 3125143736, // 0xBA45E8B8
        MiniSMG = 3173288789, // 0xBD248B55
        SNSPistol = 3218215474, // 0xBFD21232
        PistolMKII = 3219281620, // 0xBFE256D4
        AssaultRifle = 3220176749, // 0xBFEFFF6D
        SpecialCarbine = 3231910285, // 0xC0A3098D
        Revolver = 0xC1B3C3D1, // 0xC1B3C3D1
        MarksmanRifle = 3342088282, // 0xC734385A
        HeavyRevolverMKII = 3415619887, // 0xCB96392F
        BattleAxe = 3441901897, // 0xCD274149
        HeavyPistol = 3523564046, // 0xD205520E
        MachinePistol = 3675956304, // 0xDB1AA450
        CombatMGMKII = 3686625920, // 0xDBBD7280
        MarksmanPistol = 3696079510, // 0xDC4DB296
        Machete = 3713923289, // 0xDD5DF8D9
        AssaultShotgun = 3800352039, // 0xE284C527
        DoubleBarrelShotgun = 4019527611, // 0xEF951FBB
        AssaultSMG = 4024951519, // 0xEFE7E2DF
        Hatchet = 4191993645, // 0xF9DCBF2D
        CarbineRifleMKII = 4208062921, // 0xFAD1F1C9
        TearGas = 4256991824, // 0xFDBC8A50
        CeramicPistol = 0x2B5EF5EC,
        NavyRevolver = 0x917F6C8C,
        TacticalSmg = 0x14E5AFD5,
        TacticalRifle = 3520460075,
        MilitaryRifle = 0x9D1F17E6,
        PericoPistol = 0x57A4368C,
        CandyCane = 0x6589186A,
        PrecisionRifle = 0x6E7DDDEC,
        HeavyRifle = 0xC78D71B4,
        BattleRifle = 0x72B66B11,
        TacticalSMG = 0x14E5AFD5,

    }

    private enum BigWeapons : uint
    {
        SniperRifle = 100416529, // 0x05FC3C11
        CompactLauncher = 125959754, // 0x0781FE4A
        CombatPDW = 171789620, // 0x0A3D4D34
        HeavySniperMKII = 177293209, // 0x0A914799
        HeavySniper = 205991906, // 0x0C472FE2
        SweeperShotgun = 317205821, // 0x12E82D3D
        MicroSMG = 324215364, // 0x13532244
        PumpShotgun = 487013001, // 0x1D073A89
        SMG = 736523883, // 0x2BE6766B
        AssaultRifleMKII = 961495388, // 0x394F415C
        HeavyShotgun = 984333226, // 0x3AABBBAA
        Minigun = 1119849093, // 0x42BF8A85
        RayCarbine = 1198256469, // 0x476BF155
        GrenadeLauncherSmoke = 1305664598, // 0x4DD2DC56
        PumpShotgunMKII = 1432025498, // 0x555AF99A
        Gusenberg = 1627465347, // 0x61012683
        CompactRifle = 1649403952, // 0x624FE830
        HomingLauncher = 1672152130, // 0x63AB0442
        MarksManRifleMKII = 1785463520, // 0x6A6C02E0
        Railgun = 1834241177, // 0x6D544C99
        SawnOffShotgun = 2017895192, // 0x7846A318
        SmgMKII = 2024373456, // 0x78A97CD0
        BullpupRifle = 2132975508, // 0x7F229F94
        Firework = 2138347493, // 0x7F7497E5
        CombatMG = 2144741730, // 0x7FD62962
        CarbineRifle = 2210333304, // 0x83BF0278
        BullupRifleMKII = 2228681469, // 0x84D6FAFD
        SpecialCarbineMKII = 2526821735, // 0x969C3D67
        MG = 2634544996, // 0x9D07F764
        BullpupShotgun = 2640438543, // 0x9D61E50F
        GrenadeLauncher = 2726580491, // 0xA284510B
        Musket = 2828843422, // 0xA89CB99E
        AdvancedRifle = 2937143193, // 0xAF113F99
        RPG = 2982836145, // 0xB1CA77B1
        WidowMaker = 3056410471, // 0xB62D1F67
        MiniSMG = 3173288789, // 0xBD248B55
        AssaultRifle = 3220176749, // 0xBFEFFF6D
        SpecialCarbine = 3231910285, // 0xC0A3098D
        MarksmanRifle = 3342088282, // 0xC734385A
        CombatMGMKII = 3686625920, // 0xDBBD7280
        AssaultShotgun = 3800352039, // 0xE284C527
        DoubleBarrelShotgun = 4019527611, // 0xEF951FBB
        AssaultSMG = 4024951519, // 0xEFE7E2DF
        CarbineRifleMKII = 4208062921, // 0xFAD1F1C9
        Hatchet = 4191993645,
        TacticalSmg = 0x14E5AFD5,
        TacticalRifle = 0xD1D5F52B,
        MilitaryRifle = 3249783761,
        CandyCane = 0x6589186A,
        PrecisionRifle = 0x6E7DDDEC,
        HeavyRifle = 0xC78D71B4,
        BattleRifle = 0x72B66B11,
    }

    private enum BigMelee : uint
    {
        PipeWrench = 419712736, // 0x19044EE0
        StoneHatchet = 940833800, // 0x3813FC08
        GolfClub = 1141786504, // 0x440E4788
        Hammer = 1317494643, // 0x4E875F73
        NightStick = 1737195953, // 0x678B81B1
        CrowBar = 2227010557, // 0x84BD7BFD
        PoolCue = 2484171525, // 0x94117305
        Bat = 2508868239, // 0x958A4A8F
        BattleAxe = 3441901897, // 0xCD274149
        Machete = 3713923289, // 0xDD5DF8D9
        Hatchet = 4191993645, // 0xF9DCBF2D
    }

    private enum Explosives : uint
    {
        Cocktail = 615608432, // 0x24B17070
        StickyBomb = 741814745, // 0x2C3731D9
        Grenade = 2481070269, // 0x93E220BD
        BZ = 2694266206, // 0xA0973D5E
        ProximityMine = 2874559379, // 0xAB564B93
        PipeBomb = 3125143736, // 0xBA45E8B8
        TearGas = 4256991824, // 0xFDBC8A50
    }

    private enum SmallWeapons : uint
    {
        VintagePistol = 137902532, // 0x083839C4
        Pistol = 453432689, // 0x1B06D571
        APPistol = 584646201, // 0x22D8FE39
        // JerryCan = 883325847 0x34A67B97
        StunGun = 911657153, // 0x3656C8C1
        FlareGun = 1198879012, // 0x47757124
        CombatPistol = 1593441988, // 0x5EF9FEC4
        SnsPistolMKII = 2285322324, // 0x88374054
        DoubleActionRevolver = 2548703416, // 0x97EA20B8
        Pistol50 = 2578377531, // 0x99AEEB3B
        RayPistol = 2939590305, // 0xAF3696A1
        SNSPistol = 3218215474, // 0xBFD21232
        PistolMKII = 3219281620, // 0xBFE256D4
        Revolver = 0xC1B3C3D1, // 0xC1B3C3D1
        HeavyRevolverMKII = 3415619887, // 0xCB96392F
        HeavyPistol = 3523564046, // 0xD205520E
        MachinePistol = 3675956304, // 0xDB1AA450
        MarksmanPistol = 3696079510, // 0xDC4DB296
        PericoPistol = 0x57A4368C,
        CeramicPistol = 0x2B5EF5EC,
        NavyRevolver = 0x917F6C8C,
    }


    private enum UtilityWeapons : uint
    {

        JerryCan = 883325847,
    }

    private enum Misc : uint
    {
        Unarmed = 2725352035, // 0xA2719263
    }

    private int[] components = new int[83]
    {
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_PISTOL_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_COMBATPISTOL_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_APPISTOL_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MICROSMG_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SMG_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ASSAULTRIFLE_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_CARBINERIFLE_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ADVANCEDRIFLE_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MG_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_COMBATMG_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_PUMPSHOTGUN_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SAWNOFFSHOTGUN_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ASSAULTSHOTGUN_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SNIPERRIFLE_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_HEAVYSNIPER_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MINIGUN_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_RPG_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_GRENADELAUNCHER_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_PISTOL50_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ASSAULTSMG_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_BULLPUPSHOTGUN_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SNSPISTOL_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_HEAVYPISTOL_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SPECIALCARBINE_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_BULLPUPRIFLE_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_VINTAGEPISTOL_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_FIREWORK_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MUSKET_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_LARGE_FIXED_ZOOM"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MARKSMANRIFLE_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_HEAVYSHOTGUN_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_GUSENBERG_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_RAILGUN_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_HOMINGLAUNCHER_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_COMBATPDW_CLIP_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_BASE"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_MACRO"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_MACRO_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_SMALL"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_SMALL_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_MEDIUM"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_LARGE"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SCOPE_MAX"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_PI_SUPP"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_PI_SUPP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_AR_SUPP"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_SR_SUPP"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_RAILCOVER_01"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_AR_AFGRIP"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_PI_FLSH"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_AR_FLSH"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_AT_AR_SUPP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_PISTOL_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_COMBATPISTOL_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_APPISTOL_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MICROSMG_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SMG_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ASSAULTRIFLE_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_CARBINERIFLE_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ADVANCEDRIFLE_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MG_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_COMBATPDW_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_COMBATMG_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ASSAULTSHOTGUN_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_PISTOL50_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_ASSAULTSMG_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SNSPISTOL_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_HEAVYPISTOL_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_SPECIALCARBINE_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_BULLPUPRIFLE_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_VINTAGEPISTOL_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_MARKSMANRIFLE_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_HEAVYSHOTGUN_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_GUSENBERG_CLIP_02"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_PIMP"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_BALLAS"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_DOLLAR"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_DIAMOND"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_HATE"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_LOVE"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_PLAYER"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_KING"),
    Function.Call<int>(Hash.GET_HASH_KEY, (InputArgument) "COMPONENT_KNUCKLE_VARMOD_VAGOS")
    };

    public static class Helpers
    {

       // public static ScriptSettings config = ScriptSettings.Load("scripts\\Okoniewitz\\NotUselessArmor.ini");
        public static int[] Armor = new int[10]
        {
      64729,
      10706,
      23553,
      24816,
      24817,
      24818,
      57597,
      0,
      11816,
      56604
        };
        public static int ArmorPlayer = 0;
        public static int[,] Modeles;

        public static int GetArmorType()
        {
            int armor = Game.Player.Character.Armor;
            if (armor > 80)
                return 5;
            if (armor > 60)
                return 4;
            if (armor > 40)
                return 3;
            return armor > 20 ? 2 : (armor > 0 ? 1 : 0);
        }

        public static int GetPlayerID()
        {
            if (Game.Player.Character.Model.Hash == Main.Helpers.GetHashKey("PLAYER_TWO"))
                return 2;
            Model model = Game.Player.Character.Model;
            if (model.Hash == Main.Helpers.GetHashKey("PLAYER_ONE"))
                return 1;
            model = Game.Player.Character.Model;
            return model.Hash == Main.Helpers.GetHashKey("PLAYER_ZERO") ? 0 : 3;
        }

        private static int GetHashKey(string value)
        {
            return Function.Call<int>(Hash.GET_HASH_KEY, new InputArgument[1]
            {
        (InputArgument) value
            });
        }
    }

    public class WeaponProperties
{
  private WeaponHash previousSelectedWeapon = WeaponHash.Unarmed;

  public WeaponHash Hash { get; set; }

  public int Ammo { get; set; }

  public int TintIndex { get; set; }

  public int Finish { get; set; }

  public List<WeaponComponentHash> Components { get; set; } = new List<WeaponComponentHash>();

  public override bool Equals(object obj)
  {
    return obj is WeaponProperties weaponProperties && this.Hash == weaponProperties.Hash && this.Ammo == weaponProperties.Ammo && this.TintIndex == weaponProperties.TintIndex;
  }

  public override int GetHashCode()
  {
    return this.Hash.GetHashCode() ^ this.Ammo.GetHashCode() ^ this.TintIndex.GetHashCode();
  }

}
    public static class InteriorUtils
    {
        public static int GetPlayerInteriorID()
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists()) return -1;

            return Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, player);
        }
    }

}
