using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(
        menuName = "Obstacles/Color Set",
        fileName = "New Obstacle Color Set")]
    public class ObstacleColorSetSO : ScriptableObject
    {
        [SerializeField] private ObstacleType applicableType;
        [SerializeField] private HighlightableStructSet<Color> untargetedColors = new(Color.white, Color.white);
        [SerializeField] private HighlightableStructSet<Color> targetedColors = new (Color.white, Color.white);

        public ObstacleType ApplicableType {
            get { return applicableType; }
        }

        public HighlightableStructSet<Color> UntargetedColors {
            get
            {
                return new(
                    untargetedColors.Normal,
                    untargetedColors.Highlighted
                );
            }
        }

        public HighlightableStructSet <Color> TargetedColors {
            get
            {
                return new(
                    targetedColors.Normal,
                    targetedColors.Highlighted
                );
            }
        }
    }
}
