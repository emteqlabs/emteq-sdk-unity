using UnityEngine;

namespace EmteqLabs
{
    [RequireComponent(typeof(Renderer), typeof(Collider))]
    public class InteractiveCube : MonoBehaviour
    {
        private Material _material;
        private float _speed = 100f;
        private bool _rotate = false;
        [SerializeField]
        private Color _startingColor;
        [SerializeField]
        private Color _animationColor;
        
        private void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        private void Update()
        {
            if (_rotate)
            {
                transform.Rotate(0, Time.deltaTime * _speed, 0, Space.World);
            }
        }

        public void Animate()
        {
            _material.color = _animationColor;
            _rotate = true;
        }
        
        public void ResetAnimation()
        {
            _material.color = _startingColor;
            _rotate = false;
        }
    }
}
