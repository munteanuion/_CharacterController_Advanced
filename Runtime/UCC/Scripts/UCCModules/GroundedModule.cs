using System;
using UnityEngine;

namespace UniversalCharacterController.Scripts.Modules
{
    [Serializable]
    public class GroundedModuleInspector
    {
        [Tooltip("Sphere offset from player origin for the grounded check")]
        public float groundedOffset = -0.14f;

        [Tooltip("Sphere radius for ground check. Should match the CharacterController radius")]
        public float groundedRadius = 0.25f;

        [Tooltip("Layers the character uses as ground")]
        public LayerMask groundLayers;
    }

    
    
    public class GroundedModule
    {
        private GroundedModuleInspector _data;

        
        
        public void Initialize(GroundedModuleInspector data)
        {
            _data = data;
        }

        
        
        public bool Check(Vector3 position)
        {
            Vector3 spherePosition = new Vector3(position.x, position.y - _data.groundedOffset, position.z);
            return Physics.CheckSphere(spherePosition, _data.groundedRadius, _data.groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        public void DrawGizmos(Vector3 position, bool grounded, float groundedOffset, float groundedRadius)
        {
            Color groundedColor = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color airborneColor = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? groundedColor : airborneColor;
            Gizmos.DrawSphere(
                new Vector3(position.x, position.y - groundedOffset, position.z),
                groundedRadius);
        }
    }
}

