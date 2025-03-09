using System.Linq;
using UnityEngine;
using SystemMiami.CombatSystem;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace SystemMiami.Management
{
    public class ObstacleManager : Singleton<ObstacleManager>
    {
        // INSPECTOR READ ONLY
        // just so we can see what's in the dictionary
        [SerializeField, ReadOnly] private ObstacleColorSetSO staticUndamageableColors;
        [SerializeField, ReadOnly] private ObstacleColorSetSO staticDamageableColors;
        [SerializeField, ReadOnly] private ObstacleColorSetSO dynamicUndamageableColors;
        [SerializeField, ReadOnly] private ObstacleColorSetSO dynamicDamageableColors;

        private Dictionary<ObstacleType, ObstacleColorSetSO> obstacleColors = new();

        private void OnEnable()
        {
            obstacleColors = new()
            {
                { ObstacleType.STATIC_UNDAMAGEABLE, staticUndamageableColors },
                { ObstacleType.STATIC_DAMAGEABLE, staticDamageableColors },
                { ObstacleType.DYNAMIC_UNDAMAGEABLE, dynamicUndamageableColors },
                { ObstacleType.DYNAMIC_DAMAGEABLE, dynamicDamageableColors },
            };

            LoadSetsFromResources();

            staticUndamageableColors = obstacleColors[ObstacleType.STATIC_UNDAMAGEABLE];
            staticDamageableColors = obstacleColors[ObstacleType.STATIC_DAMAGEABLE];
            dynamicUndamageableColors = obstacleColors[ObstacleType.DYNAMIC_UNDAMAGEABLE];
            dynamicDamageableColors = obstacleColors[ObstacleType.DYNAMIC_DAMAGEABLE];
        }

        public ObstacleColorSetSO StaticUndamageableColors {
            get { return obstacleColors[ObstacleType.STATIC_UNDAMAGEABLE]; }
            private set { obstacleColors[ObstacleType.STATIC_UNDAMAGEABLE] = value; }
        }
        public ObstacleColorSetSO StaticDamageableColors {
            get { return obstacleColors[ObstacleType.STATIC_DAMAGEABLE]; }
            private set { obstacleColors[ObstacleType.STATIC_DAMAGEABLE] = value; }
        }
        public ObstacleColorSetSO DynamicUndamageableColors {
            get { return obstacleColors[ObstacleType.DYNAMIC_UNDAMAGEABLE]; }
            private set { obstacleColors[ObstacleType.DYNAMIC_UNDAMAGEABLE] = value; }
        }
        public ObstacleColorSetSO DynamicDamageableColors {
            get { return obstacleColors[ObstacleType.DYNAMIC_DAMAGEABLE]; }
            private set { obstacleColors[ObstacleType.DYNAMIC_DAMAGEABLE] = value; }
        }

        public ObstacleColorSetSO GetColorSetByType(ObstacleType type)
        {
            return type switch
            {
                ObstacleType.STATIC_UNDAMAGEABLE    => StaticUndamageableColors,
                ObstacleType.STATIC_DAMAGEABLE      => StaticDamageableColors,
                ObstacleType.DYNAMIC_UNDAMAGEABLE   => DynamicUndamageableColors,
                ObstacleType.DYNAMIC_DAMAGEABLE     => DynamicDamageableColors,
                _                                   => StaticUndamageableColors
            };
        }

        private void LoadSetsFromResources()
        {
            ObstacleColorSetSO[] colorSets = Resources
                    .LoadAll("ObstacleColorSets", typeof(ObstacleColorSetSO))
                    .Cast<ObstacleColorSetSO>()
                    .ToArray();

            Assert.IsTrue(colorSets.Length > 0);
            Assert.IsTrue(colorSets.Length >= obstacleColors.Count);

            List<ObstacleColorSetSO> getSetsOfType(ObstacleType type)
            {
                List<ObstacleColorSetSO> result
                        = colorSets.Where(set => set.ApplicableType == type).ToList();

                Assert.IsTrue(result.Count > 0);

                return result;
            }

            Dictionary<ObstacleType, List<ObstacleColorSetSO>> sorted = new();
            sorted[ObstacleType.STATIC_UNDAMAGEABLE]
                = getSetsOfType(ObstacleType.STATIC_UNDAMAGEABLE);

            sorted[ObstacleType.STATIC_DAMAGEABLE]
                = getSetsOfType(ObstacleType.STATIC_DAMAGEABLE);

            sorted[ObstacleType.DYNAMIC_UNDAMAGEABLE]
                = getSetsOfType(ObstacleType.DYNAMIC_UNDAMAGEABLE);

            sorted[ObstacleType.DYNAMIC_DAMAGEABLE]
                = getSetsOfType(ObstacleType.DYNAMIC_DAMAGEABLE);

            for (int i = 0; i < obstacleColors.Count; i++)
            {
                ObstacleType key = (ObstacleType)i;

                // Just uses the first thin in there.
                // This can be adjusted to find a specific
                // color set if need be.
                obstacleColors[key] = sorted[key][0];
            }
        }
    }
}
