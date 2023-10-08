using UnityEngine;
using UnityEngine.Audio;

namespace SavedSettings
{
    /// <summary>
    /// Helper class for changing common settings in games.
    /// Attach to an object in the first scene of your game to load the settings when the game loads.
    /// Once you have changed a setting, you can save it so it's loaded next session by calling SaveSettings()
    /// Note that not all settings are available on all platforms. You can't change fullscreen/resolution/vSync on console or IOS.
    /// </summary>
    public class SettingsHelper : MonoBehaviour
    {
        #region Constants

        const string SETTINGS_PATH = "Settings";
        public static float ParticleMultiplier = 1f;
        public static float ParticleLifeTimeMultiplier = 1f;
        public static AudioMixer _MasterAudioMixer;
        public const string MASTER_MIXER_STRING = "MasterMixer";
        const float VOLUME_DIFFERENCE = 80f;

        #endregion

        #region Static Methods

        /// <summary>
        /// The volume for all audio players using the Master Audio Mixer.
        /// 0 - 100
        /// </summary>
        public static float MasterVolume
        {
            get
            {
                if (_MasterAudioMixer == null)
                {
                    _MasterAudioMixer = Resources.Load<AudioMixer>(MASTER_MIXER_STRING);
                }
                float volume = 0f;
                _MasterAudioMixer.GetFloat("MasterVolume", out volume);
                return (volume + VOLUME_DIFFERENCE) * 0.01f;
            }
            set
            {
                if (_MasterAudioMixer == null)
                {
                    _MasterAudioMixer = Resources.Load<AudioMixer>(MASTER_MIXER_STRING);
                }
                _MasterAudioMixer.SetFloat("MasterVolume", (value * 100f) - VOLUME_DIFFERENCE);
            }
        }

        /// <summary>
        /// The volume for audio players using the Master Audio Mixer's "MusicVolume".
        /// 0 - 100
        /// </summary>
        public static float MusicVolume
        {
            get
            {
                if (_MasterAudioMixer == null)
                {
                    _MasterAudioMixer = Resources.Load<AudioMixer>(MASTER_MIXER_STRING);
                }
                float volume = 0f;
                _MasterAudioMixer.GetFloat("MusicVolume", out volume);
                return (volume + VOLUME_DIFFERENCE) * 0.01f;
            }
            set
            {
                if (_MasterAudioMixer == null)
                {
                    _MasterAudioMixer = Resources.Load<AudioMixer>(MASTER_MIXER_STRING);
                }
                _MasterAudioMixer.SetFloat("MusicVolume", (value * 100f) - VOLUME_DIFFERENCE);
            }
        }

        /// <summary>
        /// The volume for audio players that use the Master Audio Mixer's "SoundEffectVolume".
        /// 0 - 100
        /// </summary>
        public static float SoundEffectVolume
        {
            get
            {
                if (_MasterAudioMixer == null)
                {
                    _MasterAudioMixer = Resources.Load<AudioMixer>(MASTER_MIXER_STRING);
                }
                float volume = 0f;
                _MasterAudioMixer.GetFloat("SoundEffectVolume", out volume);
                return (volume + VOLUME_DIFFERENCE) * 0.01f;
            }
            set
            {
                if (_MasterAudioMixer == null)
                {
                    _MasterAudioMixer = Resources.Load<AudioMixer>(MASTER_MIXER_STRING);
                }
                _MasterAudioMixer.SetFloat("SoundEffectVolume", (value * 100f) - VOLUME_DIFFERENCE);
            }
        }

        /// <summary>
        /// Mipmaps
        /// 0 (default) = full size
        /// 1 = 1/2 size
        /// 2 = 1/4 size
        /// 3 = 1/8 size
        /// </summary>
        public static int TextureQuality
        {
            get
            {
                return QualitySettings.globalTextureMipmapLimit;
            }
            set
            {
                QualitySettings.globalTextureMipmapLimit = value;
            }
        }

        /// <summary>
        /// Multi-stampling level.
        /// Options are: 0, 2, 4, 8
        /// </summary>
        public static int AntiAliasing
        {
            get
            {
                return QualitySettings.antiAliasing;
            }
            set
            {
                QualitySettings.antiAliasing = value;
            }
        }

        /// <summary>
        /// Sets the screen to full screen or windowed.
        /// </summary>
        public static bool FullScreen
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            get
            {
                return Screen.fullScreen;
            }
            set
            {
                Screen.fullScreen = value;
            }
#endif
        }

        /// <summary>
        /// Sets the current game window resolution.
        /// </summary>
        public static Resolution ResolutionOfWindow
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            get
            {
                return Screen.currentResolution;
            }
            set
            {
                Screen.SetResolution(value.width, value.height, FullScreen);
            }
#endif
        }

        /// <summary>
        /// Enables or disables VSync.
        /// </summary>
        public static bool VSync
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            get
            {
                return QualitySettings.vSyncCount > 0;
            }
            set
            {

                if (value)
                {
                    QualitySettings.vSyncCount = 1;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                }
            }
#endif
        }

        /// <summary>
        /// Sets the quality of shadows in the game.
        /// ShadowQuality.All = soft and hard shadows.
        /// ShadowQuality.HardOnly = hard shadows only.
        /// ShadowQuality.Disable = shadows disabled.
        /// </summary>
        public static ShadowQuality ShadowQuality
        {
            get
            {
                return QualitySettings.shadows;
            }
            set
            {
                QualitySettings.shadows = value;
            }
        }

        /// <summary>
        /// Sets the resolution of shadows being used.
        /// ShadowResolution.Low
        /// ShadowResolution.Medium
        /// ShadowResolution.Hight
        /// ShadowResolution.VeryHigh
        /// </summary>
        public static ShadowResolution ShadowsResolution
        {
            get
            {
                return QualitySettings.shadowResolution;
            }
            set
            {
                QualitySettings.shadowResolution = value;
            }
        }

        /// <summary>
        /// Sets the number of shadow cascades used for shadow mapping.
        /// Please see https://docs.unity3d.com/Manual/DirLightShadows.html
        /// Valid values are 0, 2, and 4
        /// </summary>
        public static int ShadowCascades
        {
            get
            {
                return QualitySettings.shadowCascades;
            }
            set
            {
                QualitySettings.shadowCascades = value;
            }
        }

        /// <summary>
        /// Sets if soft particles are enabled.
        /// </summary>
        public static bool SoftParticles
        {
            get
            {
                return QualitySettings.softParticles;
            }
            set
            {
                QualitySettings.softParticles = value;
            }
        }

        /// <summary>
        /// Discard's the current settings for the last settings saved.
        /// </summary>
        public static void DiscardChanges()
        {
            TextureQuality = _LastData.textureQuality;
            AntiAliasing = _LastData.antiAliasing;
            ShadowQuality = _LastData.shadowQuality;
            ShadowsResolution = _LastData.shadowsResolution;
            ShadowCascades = _LastData.shadowCascades;
            SoftParticles = _LastData.softParticles;
            MasterVolume = _LastData.masterVolume;
            MusicVolume = _LastData.musicVolume;
            SoundEffectVolume = _LastData.soundEffectVolume;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            FullScreen = _LastData.fullScreen;
            Resolution newRest = new Resolution();
            newRest.width = _LastData.resolution.x;
            newRest.height = _LastData.resolution.y;
            newRest.refreshRate = _LastData.resolution.z;
            ResolutionOfWindow = newRest;
            VSync = _LastData.vSync;
#endif
        }

        /// <summary>
        /// Resets the current settings to the initial default settings.
        /// </summary>
        public static void ResetData()
        {
            TextureQuality = _DefaultData.textureQuality;
            AntiAliasing = _DefaultData.antiAliasing;
            ShadowQuality = _DefaultData.shadowQuality;
            ShadowsResolution = _DefaultData.shadowsResolution;
            ShadowCascades = _DefaultData.shadowCascades;
            SoftParticles = _DefaultData.softParticles;
            MasterVolume = _DefaultData.masterVolume;
            MusicVolume = _DefaultData.musicVolume;
            SoundEffectVolume = _DefaultData.soundEffectVolume;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            FullScreen = _DefaultData.fullScreen;
            Resolution newRest = new Resolution();
            newRest.width = _DefaultData.resolution.x;
            newRest.height = _DefaultData.resolution.y;
            newRest.refreshRate = _DefaultData.resolution.z;
            ResolutionOfWindow = newRest;
            VSync = _DefaultData.vSync;
#endif
            Save();
        }

        /// <summary>
        /// Saves the current settings so they are loaded the next time the game is loaded.
        /// </summary>
        public static void Save()
        {
            SettingsData data = new SettingsData();
            data.masterVolume = MasterVolume;
            data.musicVolume = MusicVolume;
            data.soundEffectVolume = SoundEffectVolume;
            data.textureQuality = TextureQuality;
            data.antiAliasing = AntiAliasing;
            data.shadowQuality = ShadowQuality;
            data.shadowsResolution = ShadowsResolution;
            data.shadowCascades = ShadowCascades;
            data.softParticles = SoftParticles;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            data.fullScreen = FullScreen;
            data.resolution = new Vector3Int(ResolutionOfWindow.width, ResolutionOfWindow.height, ResolutionOfWindow.refreshRate);
            data.vSync = VSync;
#endif
            _LastData = data;
            SaveHelper.Save(SETTINGS_PATH, JsonUtility.ToJson(data), 1f);
        }

        #endregion

        #region Load Settings

        [System.Serializable]
        struct SettingsData
        {
            public float masterVolume;
            public float musicVolume;
            public float soundEffectVolume;
            public int textureQuality;
            public int antiAliasing;
            public ShadowQuality shadowQuality;
            public ShadowResolution shadowsResolution;
            public int shadowCascades;
            public bool softParticles;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            public bool fullScreen;
            public Vector3Int resolution;
            public bool vSync;
#endif
        }


        /// <summary>
        /// Load settings.
        /// </summary>
        void Awake()
        {
            //Save default settings
            _DefaultData.masterVolume = MasterVolume;
            _DefaultData.musicVolume = MusicVolume;
            _DefaultData.soundEffectVolume = SoundEffectVolume;
            _DefaultData.textureQuality = TextureQuality;
            _DefaultData.antiAliasing = AntiAliasing;
            _DefaultData.shadowQuality = ShadowQuality;
            _DefaultData.shadowsResolution = ShadowsResolution;
            _DefaultData.shadowCascades = ShadowCascades;
            _DefaultData.softParticles = SoftParticles;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
            _DefaultData.fullScreen = FullScreen;
            _DefaultData.resolution = new Vector3Int(ResolutionOfWindow.width, ResolutionOfWindow.height, ResolutionOfWindow.refreshRate);
            _DefaultData.vSync = VSync;
#endif

            SaveData loaded = SaveHelper.Load(SETTINGS_PATH);
            if (string.IsNullOrEmpty(loaded.data))
            {
                Save();
                enabled = false;
            }
            else
            {
                try
                {
                    enabled = true;
                    _LastData = JsonUtility.FromJson<SettingsData>(loaded.data);
                    // Volume values are set in Start() because of a Unity bug. Unity will not set any AudioMixer values during Awake().
                    TextureQuality = _LastData.textureQuality;
                    AntiAliasing = _LastData.antiAliasing;
                    ShadowQuality = _LastData.shadowQuality;
                    ShadowsResolution = _LastData.shadowsResolution;
                    ShadowCascades = _LastData.shadowCascades;
                    SoftParticles = _LastData.softParticles;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
                    FullScreen = _LastData.fullScreen;
                    if (_LastData.resolution.x > 1 && _LastData.resolution.y > 1 && _LastData.resolution.z > 1)
                    {
                        Resolution newRest = new Resolution();
                        newRest.width = _LastData.resolution.x;
                        newRest.height = _LastData.resolution.y;
                        newRest.refreshRate = _LastData.resolution.z;
                        ResolutionOfWindow = newRest;
                    }
                    VSync = _LastData.vSync;
#endif
                }
#if UNITY_EDITOR
                catch
                {
                    Debug.LogError("Failed to load settings data.");
                }
#else
            catch { }
#endif
            }
        }
        static SettingsData _LastData;
        static SettingsData _DefaultData;

        /// <summary>
        /// Unity Bug: Unity will not set any AudioMixer values during Awake().
        /// So we have to set them here.
        /// </summary>
        private void Start()
        {
            MasterVolume = _LastData.masterVolume;
            MusicVolume = _LastData.musicVolume;
            SoundEffectVolume = _LastData.soundEffectVolume;
        }

        #endregion
    }
}