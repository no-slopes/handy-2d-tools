using H2DT.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static H2DT.Utils.Math;

namespace H2DT.UI.Prefs.Audio
{
    [AddComponentMenu("Handy 2D Tools/UI/Prefs/Audio/VolumeChanger")]
    public class UISoundVolumeChanger : HandyComponent
    {
        #region Inspector

        [Header("Sound")]
        [Space]
        [SerializeField]
        protected AudioHandler _handler;

        [Header("Configuration")]
        [Space]
        [SerializeField]
        protected bool _useSlider;

        [SerializeField]
        protected bool _useButtons;

        [SerializeField]
        protected float _maxVolume = 10f;


        [Header("UI Objects")]
        [Space]
        [SerializeField]
        protected TMP_Text _labelText;

        [SerializeField]
        protected TMP_Text _valueText;

        [SerializeField]
        protected Slider _slider;

        [SerializeField]
        protected Button _minusButton;

        [SerializeField]
        protected Button _plusButton;

        #endregion

        #region Fields

        protected RectTransform _rectTransform;

        protected float _displayValue;

        #endregion

        #region  Properties

        #endregion

        #region Getters

        public TMP_Text labelText => _labelText;
        public RectTransform rectTransform => _rectTransform;

        #endregion

        #region Mono

        protected void Awake()
        {
            if (!_useSlider)
            {
                Destroy(_slider.gameObject);
                _slider = null;
            }
            else
            {
                _slider.maxValue = _maxVolume;
            }

            if (_useButtons)
            {
                _minusButton.gameObject.SetActive(_useButtons);
                _plusButton.gameObject.SetActive(_useButtons);
            }

            float displayValue = ConvertScale(_handler.volume, 1, 0, _maxVolume);
            ChangeDisplayValue(displayValue);

            _labelText.text = _handler.uiLabel;
            FindComponent<RectTransform>(ref _rectTransform);
        }

        protected void OnEnable()
        {
            _slider?.onValueChanged.AddListener(OnSliderValueChange);
            _handler.volumeChanged.AddListener(OnHandlerVolumeChange);

            if (_useButtons)
            {
                _minusButton.onClick.AddListener(OnMinus);
                _plusButton.onClick.AddListener(OnPlus);
            }
        }

        protected void OnDisable()
        {
            _slider?.onValueChanged.RemoveListener(OnSliderValueChange);
            _handler.volumeChanged.RemoveListener(OnHandlerVolumeChange);

            if (_useButtons)
            {
                _minusButton.onClick.RemoveListener(OnMinus);
                _plusButton.onClick.RemoveListener(OnPlus);
            }
        }

        #endregion

        #region Logic

        protected void ChangeVolume(float volume)
        {
            _handler.volume = ConvertScale(volume, _maxVolume, 0, 1);
        }

        protected void ChangeDisplayValue(float value)
        {
            if (value <= 0)
            {
                value = 0;
                if (_useButtons)
                {
                    _minusButton.enabled = false;
                    _plusButton.enabled = true;
                }
            }
            else if (value >= _maxVolume)
            {
                value = _maxVolume;
                if (_useButtons)
                {
                    _minusButton.enabled = true;
                    _plusButton.enabled = false;

                }
            }
            else if (value > 0 && value < _maxVolume)
            {
                if (_useButtons)
                {
                    _minusButton.enabled = true;
                    _plusButton.enabled = true;
                }
            }

            if (_displayValue != value)
            {
                if (_slider != null)
                    _slider.value = value;
            }

            _displayValue = value;
            _valueText.text = _displayValue.ToString();
        }

        #endregion

        #region UI Callbacks

        protected void OnMinus()
        {
            float newValue = _displayValue - (_maxVolume / 10);
            ChangeDisplayValue(newValue);
            ChangeVolume(newValue);
        }

        protected void OnPlus()
        {
            float newValue = _displayValue + (_maxVolume / 10);
            ChangeDisplayValue(newValue);
            ChangeVolume(newValue);
        }

        protected void OnSliderValueChange(float value)
        {
            ChangeDisplayValue(value);
            ChangeVolume(value);
        }

        private void OnHandlerVolumeChange(float normalizedVolume)
        {
            ChangeDisplayValue(normalizedVolume * _maxVolume);
        }

        #endregion
    }
}
