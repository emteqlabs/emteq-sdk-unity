using UnityEngine;

namespace EmteqLabs
{
    public class CubeController : MonoBehaviour
    {
        public float Speed = 40;
        public bool IsPaused = false;
        private Transform _cachedTransform;
        private MeshRenderer _renderer;
        private Color _defaultColour;

        private Vector3 _normalisedSpeedMultiplier = new Vector3(1, 1, -1);

        // Start is called before the first frame update
        void Awake()
        {
            _cachedTransform = this.transform;
            _renderer = GetComponent<MeshRenderer>();
            _defaultColour = _renderer.material.color;
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsPaused)
            {
                _cachedTransform.Rotate(_normalisedSpeedMultiplier.x * Speed * Time.deltaTime,
                    _normalisedSpeedMultiplier.y * 2 * Speed * Time.deltaTime, 
                    _normalisedSpeedMultiplier.z * Speed * Time.deltaTime);
            }
        }

        public void SetColour(Color colour)
        {
            _renderer.material.color = colour;
        }

        public void ResetDefault()
        {
            _renderer.material.color = _defaultColour;
            _normalisedSpeedMultiplier = new Vector3(1, 1, -1);
        }

        public void PauseRotation()
        {
            IsPaused = true;
        }

        public void ResumeRotation()
        {
            IsPaused = false;
        }

        public void SetRandomRotation()
        {
            _normalisedSpeedMultiplier = new Vector3(
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f));
        }
    }
}