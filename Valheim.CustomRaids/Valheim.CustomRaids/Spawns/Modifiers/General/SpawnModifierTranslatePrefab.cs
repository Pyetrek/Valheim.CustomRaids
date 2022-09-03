using UnityEngine;
using Valheim.CustomRaids.Core;
using Valheim.CustomRaids.Raids.Managers;
using Valheim.CustomRaids.Spawns.Caches;

namespace Valheim.CustomRaids.Spawns.Modifiers.General;

public class SpawnModifierTranslatePrefab : ISpawnModifier
{
    private static SpawnModifierTranslatePrefab _instance;

    public static SpawnModifierTranslatePrefab Instance
    {
        get
        {
            return _instance ??= new SpawnModifierTranslatePrefab();
        }
    }

    public void Modify(SpawnContext context)
    {
        if (context.Spawn is null)
        {
            return;
        }

        if(RaidManager.TryGetRandomEvent(context.RaidConfig.Name.Value, out var randomEvent))
        {
            string sectionId = context.Config.SectionKey + '_';
            Log.LogDebug($"Got random event data for {context.RaidConfig.Name.Value}. Looking for spawn configs that start with {sectionId}");
            foreach (SpawnSystem.SpawnData spawnData in randomEvent.m_spawn)
            {
                SpawnDataCache spawnDataCache = SpawnDataCache.Get(spawnData);
                if (spawnDataCache.SpawnConfig.SectionKey.StartsWith(sectionId))
                {
                    float translateX = spawnDataCache.SpawnConfig.TranslateX.Value;
                    float translateY = spawnDataCache.SpawnConfig.TranslateY.Value;
                    float translateZ = spawnDataCache.SpawnConfig.TranslateZ.Value;
                    if (translateX == 0 && translateY == 0 && translateZ == 0)
                    {
                        Log.LogDebug($"No translate set for object {spawnDataCache.SpawnConfig.SectionKey}");
                    }
                    else
                    {
                        Log.LogDebug($"Translating object: x{translateX}, y{translateY}, z{translateZ}");
                        Vector3 position = new Vector3(
                            context.Spawn.transform.position.x + translateX,
                            0f,
                            context.Spawn.transform.position.z + translateZ
                        );
                        position.y = ZoneSystem.instance.GetGroundHeight(position) + translateY + spawnDataCache.SpawnConfig.GroundOffset.Value;
                        context.SpawnSystem.Spawn((SpawnSystem.SpawnData)spawnData, position, true);
                    }
                }
            }
        }
    }
}
