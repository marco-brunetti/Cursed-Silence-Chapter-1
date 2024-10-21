using UnityEngine;

namespace Game.General
{
    public class Fix_ApplyGlobalVolume : MonoBehaviour
    {
        [SerializeField] private AudioSource[] _audioSources;
        [SerializeField] private float[] _volumes;

        private float _currentGlobalVolume;

        // Start is called before the first frame update
        void Start()
        {
            _currentGlobalVolume = GameController.Instance.GlobalVolume;

            for (int i = 0; i < _audioSources.Length; i++)
            {
                _audioSources[i].volume = _volumes[i] * _currentGlobalVolume;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(_currentGlobalVolume != GameController.Instance.GlobalVolume)
            {
                _currentGlobalVolume = GameController.Instance.GlobalVolume;

                for(int i = 0; i < _audioSources.Length; i++)
                {
                    _audioSources[i].volume = _volumes[i] * _currentGlobalVolume;
                }
            }
        }
    }
}
