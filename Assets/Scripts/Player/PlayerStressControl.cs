using UnityEngine;

namespace Player
{
    public class PlayerStressControl : MonoBehaviour
    {
        private bool _addStress;

        private float _maxStressLevel;
        private float _minStressLevel;
        private float _currentStressLevel;
        private float _targetStressLevel;
        private float _stressDisipateSpeed;

        private void Start()
        {
            PlayerData playerData = PlayerController.Instance.PlayerData;
            _maxStressLevel = playerData.MaxStressLevel;
            _minStressLevel = playerData.MinStressLevel;
            _stressDisipateSpeed = playerData.StressDisipateSpeed;
            _currentStressLevel = _minStressLevel;
        }

        public void ManageStress(PlayerData playerData)
        {
            if (_addStress && _currentStressLevel >= _maxStressLevel - 0.05f)
            {
                _targetStressLevel = _minStressLevel;
                _addStress = false;
            }

            if (_currentStressLevel < _targetStressLevel)
            {
                _currentStressLevel = Mathf.Lerp(_currentStressLevel, _targetStressLevel, Time.deltaTime * 5);
            }
            else
            {
                _currentStressLevel = Mathf.Lerp(_currentStressLevel, _targetStressLevel,
                    _stressDisipateSpeed * Time.deltaTime);
            }

            _currentStressLevel = Mathf.Clamp(_currentStressLevel, _minStressLevel, _maxStressLevel);
        }

        public void AddStress()
        {
            _targetStressLevel = _maxStressLevel;
            _addStress = true;
        }

        public float StressLevel()
        {
            return _currentStressLevel;
        }
    }
}