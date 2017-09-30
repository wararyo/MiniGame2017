/****************************************************************************
 *
 * Copyright (c) 2011 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

/*---------------------------
 * Sequence Callback Defines
 *---------------------------*/
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_IOS || UNITY_TVOS || UNITY_WINRT
	#define CRIWARE_SUPPORT_SEQUENCE_CALLBACK
	/* Callback Implentation Defines */
	#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_WINRT
		#define CRIWARE_CALLBACK_IMPL_NATIVE2MANAGED
	#elif UNITY_IOS || UNITY_TVOS
		#define CRIWARE_CALLBACK_IMPL_UNITYSENDMESSAGE
	#else
		#error supported platform must select one of callback implementations
	#endif
#elif UNITY_PSP2 || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_WEBGL || UNITY_SWITCH
	#define CRIWARE_UNSUPPORT_SEQUENCE_CALLBACK
#else
	#error undefined platform if supporting sequence callback
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

public static class CriAtomPlugin
{
	#region Version Info.
	public const string criAtomUnityEditorVersion = "Ver.0.21.07";
	#endregion

	#region Editor/Runtime����

#if UNITY_EDITOR
	public static bool showDebugLog = false;
	public delegate void PreviewCallback();
	public static PreviewCallback previewCallback = null;
#endif

	public static void Log(string log)
	{
	#if UNITY_EDITOR
		if (CriAtomPlugin.showDebugLog) {
			Debug.Log(log);
		}
	#endif
	}

	/* �������J�E���^ */
	private static int initializationCount = 0;

	public static bool isInitialized { get { return initializationCount > 0; } }

    private static IntPtr GetSpatializerCoreInterfaceFromAtomOculusAudioBridge() {
        /* Ambisonic �f�[�^���Đ����邽�߂ɁA�v���b�g�t�H�[���ɂ���Ă̓u���b�W�v���O�C����K�v�Ƃ��邩������Ȃ�
         * �Ⴆ�� PC �ł� Oculus Audio �u���b�W�v���O�C�����g���B
         * �Ⴆ�� PS4 �ł� �u���b�W�v���O�C�����g��Ȃ� */
        /* �ȉ��ACRI Atom Oculus Audio �u���b�W�v���O�C�����C���|�[�g����Ă���ꍇ�̏��� */
        Type type = Type.GetType("CriAtomOculusAudio");
        if (type == null) {
            /* BridgePlugin��������Ȃ����� */
            Debug.LogError("[CRIWARE] ERROR: Cri Atom Oculus Audio Bridge Plugin is not imported.");
        } else {
            /* ���݂̃v���b�g�t�H�[���� Oculus Audio Bridge Plugin ���T�|�[�g���Ă��邩�m�F */
            System.Reflection.MethodInfo method_support_current_platform = type.GetMethod("SupportCurrentPlatform");
            if (method_support_current_platform == null)
            {
                Debug.LogError("[CRIWARE] ERROR: CriAtomOculusAudio.SupportCurrentPlatform method is not found.");
                return IntPtr.Zero;
            }
            bool current_platform_supports_oculus_audio = (bool)method_support_current_platform.Invoke(null, null);
            /* �J�����g�v���b�g�t�H�[�����T�|�[�g���Ă���Ȃ珀���B
                * �T�|�[�g���Ă��Ȃ��Ȃ�X�L�b�v�B�������� Atom �������������s�� */
            if (current_platform_supports_oculus_audio)
            {
                /* �K�v�ȃ��\�b�h�����擾 */
                System.Reflection.MethodInfo method_get_spatializer_core_interface = type.GetMethod("GetSpatializerCoreInterface");
                if (method_get_spatializer_core_interface == null)
                {
                    Debug.LogError("[CRIWARE] ERROR: CriAtomOculusAudio.GetSpatializerCoreInterface method is not found.");
                    return IntPtr.Zero;
                }
                /* Spatilalizer �̏������ɕK�v�ȏ����擾 */
                return (IntPtr)method_get_spatializer_core_interface.Invoke(null, null);
            }
        }
        return IntPtr.Zero;
    }

    public static void SetConfigParameters(int max_virtual_voices,
		int max_voice_limit_groups, int max_categories,
		int num_standard_memory_voices, int num_standard_streaming_voices,
		int num_hca_mx_memory_voices, int num_hca_mx_streaming_voices,
		int output_sampling_rate, int num_asr_output_channels,
		bool uses_in_game_preview, float server_frequency,
		int max_parameter_blocks,  int categories_per_playback,
		int num_buses, bool vr_mode)
    {
        IntPtr spatializer_core_interface = IntPtr.Zero;
        /* Ambisonic �f�[�^�̍Đ��ɕK�v�ȏ������p�����[�^���擾���� */
        if (vr_mode) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID 
            spatializer_core_interface = CriAtomPlugin.GetSpatializerCoreInterfaceFromAtomOculusAudioBridge();
#endif
        }
        criAtomUnity_SetConfigParameters(max_virtual_voices,
			max_voice_limit_groups, max_categories,
			num_standard_memory_voices, num_standard_streaming_voices,
			num_hca_mx_memory_voices, num_hca_mx_streaming_voices,
			output_sampling_rate, num_asr_output_channels,
			uses_in_game_preview, server_frequency,
			max_parameter_blocks, categories_per_playback,
			num_buses, vr_mode,
            spatializer_core_interface);
	}

	public static void SetConfigAdditionalParameters_PC(long buffering_time_pc)
	{
		criAtomUnity_SetConfigAdditionalParameters_PC(buffering_time_pc);
	}

	public static void SetConfigAdditionalParameters_IOS(uint buffering_time_ios, bool override_ipod_music_ios)
	{
		criAtomUnity_SetConfigAdditionalParameters_IOS(buffering_time_ios, override_ipod_music_ios);
	}

	public static void SetConfigAdditionalParameters_ANDROID(int num_low_delay_memory_voices, int num_low_delay_streaming_voices,
															 int sound_buffering_time,		  int sound_start_buffering_time,
                                                             IntPtr android_context)
	{
		criAtomUnity_SetConfigAdditionalParameters_ANDROID(num_low_delay_memory_voices, num_low_delay_streaming_voices,
														   sound_buffering_time,		sound_start_buffering_time,
														   android_context);
	}

	public static void SetConfigAdditionalParameters_VITA(int num_atrac9_memory_voices, int num_atrac9_streaming_voices, int num_mana_decoders)
	{
		#if !UNITY_EDITOR && UNITY_PSP2
		criAtomUnity_SetConfigAdditionalParameters_VITA(num_atrac9_memory_voices, num_atrac9_streaming_voices, num_mana_decoders);
		#endif
	}

	public static void SetConfigAdditionalParameters_PS4(int num_atrac9_memory_voices, int num_atrac9_streaming_voices,
														 bool use_audio3d, int num_audio3d_memory_voices, int num_audio3d_streaming_voices)
	{
		#if !UNITY_EDITOR && UNITY_PS4
		criAtomUnity_SetConfigAdditionalParameters_PS4(num_atrac9_memory_voices, num_atrac9_streaming_voices,
														use_audio3d, num_audio3d_memory_voices, num_audio3d_streaming_voices);
		#endif
	}

	public static void SetConfigAdditionalParameters_WEBGL(int num_webaudio_voices)
	{
		#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
		criAtomUnity_SetConfigAdditionalParameters_WEBGL(num_webaudio_voices);
		#endif
	}

	public static void SetMaxSamplingRateForStandardVoicePool(int sampling_rate_for_memory, int sampling_rate_for_streaming)
	{
		criAtomUnity_SetMaxSamplingRateForStandardVoicePool(sampling_rate_for_memory, sampling_rate_for_streaming);
	}

	public static int GetRequiredMaxVirtualVoices(CriAtomConfig atomConfig)
	{
		/* �o�[�`�����{�C�X�́A�S�{�C�X�v�[���̃{�C�X�̍��v�l���������Ȃ��Ă͂Ȃ�Ȃ� */
		int requiredVirtualVoices = 0;

		requiredVirtualVoices += atomConfig.standardVoicePoolConfig.memoryVoices;
		requiredVirtualVoices += atomConfig.standardVoicePoolConfig.streamingVoices;
		requiredVirtualVoices += atomConfig.hcaMxVoicePoolConfig.memoryVoices;
		requiredVirtualVoices += atomConfig.hcaMxVoicePoolConfig.streamingVoices;

		#if UNITY_ANDROID
		requiredVirtualVoices += atomConfig.androidLowLatencyStandardVoicePoolConfig.memoryVoices;
		requiredVirtualVoices += atomConfig.androidLowLatencyStandardVoicePoolConfig.streamingVoices;
		#elif UNITY_PSP2
		requiredVirtualVoices += atomConfig.vitaAtrac9VoicePoolConfig.memoryVoices;
		requiredVirtualVoices += atomConfig.vitaAtrac9VoicePoolConfig.streamingVoices;
		#endif

		return requiredVirtualVoices;
	}

	public static void InitializeLibrary()
	{
		/* �������J�E���^�̍X�V */
		CriAtomPlugin.initializationCount++;
		if (CriAtomPlugin.initializationCount != 1) {
			return;
		}

		/* CriWareInitializer�����s�ς݂��ǂ������m�F */
		bool initializerWorking = CriWareInitializer.IsInitialized();
		if (initializerWorking == false) {
			Debug.Log("[CRIWARE] CriWareInitializer is not working. "
				+ "Initializes Atom by default parameters.");
		}

		/* �ˑ����C�u�����̏����� */
		CriFsPlugin.InitializeLibrary();

		/* ���C�u�����̏����� */
		CriAtomPlugin.criAtomUnity_Initialize();

		/* CriAtomServer�̃C���X�^���X�𐶐� */
		#if UNITY_EDITOR
		/* �Q�[���v���r���[���̂ݐ������� */
		if (UnityEngine.Application.isPlaying) {
			CriAtomServer.CreateInstance();
		}
		#else
		CriAtomServer.CreateInstance();
		#endif

		/* CriAtomListener �̋��L�l�C�e�B�u���X�i�[�𐶐� */
		CriAtomListener.CreateSharedNativeListener();
	}

	public static void FinalizeLibrary()
	{
		/* �������J�E���^�̍X�V */
		CriAtomPlugin.initializationCount--;
		if (CriAtomPlugin.initializationCount < 0) {
			Debug.LogError("[CRIWARE] ERROR: Atom library is already finalized.");
			return;
		}
		if (CriAtomPlugin.initializationCount != 0) {
			return;
		}

		/* CriAtomListener �̋��L�l�C�e�B�u���X�i�[��j�� */
		CriAtomListener.DestroySharedNativeListener();

		/* CriAtomServer�̃C���X�^���X��j�� */
		CriAtomServer.DestroyInstance();

		/* ���C�u�����̏I�� */
		CriAtomPlugin.criAtomUnity_Finalize();

		/* �ˑ����C�u�����̏I�� */
		CriFsPlugin.FinalizeLibrary();
	}

	public static void Pause(bool pause)
	{
		if (isInitialized) {
			criAtomUnity_Pause(pause);
		}
	}

	private static float timeSinceStartup = Time.realtimeSinceStartup;
	private static CriWare.CpuUsage cpuUsage;
	public static CriWare.CpuUsage GetCpuUsage()
	{
		float currentTimeSinceStartup = Time.realtimeSinceStartup;
		if (currentTimeSinceStartup - timeSinceStartup > 1.0f) {
			CriAtomEx.PerformanceInfo info;
			CriAtomEx.GetPerformanceInfo(out info);

			cpuUsage.last = info.lastServerTime * 100.0f / info.averageServerInterval;
			cpuUsage.average = info.averageServerTime * 100.0f / info.averageServerInterval;
			cpuUsage.peak = info.maxServerTime * 100.0f / info.averageServerInterval;

			CriAtomEx.ResetPerformanceMonitor();
			timeSinceStartup = currentTimeSinceStartup;
		}
		return cpuUsage;
	}

    [DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
    private static extern void criAtomUnity_SetConfigParameters(int max_virtual_voices,
        int max_voice_limit_groups, int max_categories,
        int num_standard_memory_voices, int num_standard_streaming_voices,
        int num_hca_mx_memory_voices, int num_hca_mx_streaming_voices,
        int output_sampling_rate, int num_asr_output_channels,
        bool uses_in_game_preview, float server_frequency,
        int max_parameter_blocks, int categories_per_playback,
        int num_buses, bool use_ambisonics,
        IntPtr spatializer_core_interface);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_PC(long buffering_time_pc);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_IOS(uint buffering_time_ios, bool override_ipod_music_ios);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_ANDROID(int num_low_delay_memory_voices, int num_low_delay_streaming_voices,
																				  int sound_buffering_time,		   int sound_start_buffering_time,
																				  IntPtr android_context);
	#if !UNITY_EDITOR && UNITY_PSP2
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_VITA(int num_atrac9_memory_voices, int num_atrac9_streaming_voices, int num_mana_decoders);
	#endif

    #if !UNITY_EDITOR && UNITY_PS4
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_PS4(int num_atrac9_memory_voices, int num_atrac9_streaming_voices,
																			  bool use_audio3d, int num_audio3d_memory_voices, int num_audio3d_streaming_voices);
	#endif

	#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_WEBGL(int num_webaudio_voices);
	#endif

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_Initialize();

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_Finalize();

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_Pause(bool pause);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	public static extern uint criAtomUnity_GetAllocatedHeapSize();

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	public static extern void criAtomUnity_ControlDataCompatibility(int code);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	public static extern void criAtomUnitySeqencer_SetEventCallback(IntPtr cbfunc, string gameobj_name, string func_name, string separator_string);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomUnity_SetMaxSamplingRateForStandardVoicePool(int sampling_rate_for_memory, int sampling_rate_for_streaming);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	public static extern void criAtomUnity_BeginLeCompatibleMode();

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	public static extern void criAtomUnity_EndLeCompatibleMode();

	#endregion
}

[Serializable]
public class CriAtomCueSheet
{
	public string name = "";
	public string acbFile = "";
	public string awbFile = "";
	public CriAtomExAcb acb;
	public bool IsLoading { get { return acb == null; } }

#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
	public CriAtomExAcbAsync async;
#endif
}

/// \addtogroup CRIATOM_UNITY_COMPONENT
/// @{
/**
 * <summary>�T�E���h�Đ��S�̂𐧌䂷�邽�߂̃R���|�[�l���g�ł��B</summary>
 * \par ����:
 * �e�V�[���ɂЂƂp�ӂ��Ă����K�v������܂��B<br/>
 * UnityEditor���CRI Atom�E�B���h�E���g�p���� CriAtomSource ���쐬�����ꍇ�A
 * �uCRIWARE�v�Ƃ������O�����I�u�W�F�N�g�Ƃ��Ď����I�ɍ쐬����܂��B�ʏ�̓��[�U�[���쐬����K�v�͂���܂���B
 */
[AddComponentMenu("CRIWARE/CRI Atom")]
public class CriAtom : MonoBehaviour
{

	/* @cond DOXYGEN_IGNORE */
	public string acfFile = "";
#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
	private byte[] acfData = null;
	private bool acfIsLoading = false;
#endif
	public CriAtomCueSheet[] cueSheets = {};
	public string dspBusSetting = "";
	public bool dontDestroyOnLoad = false;
	private static CriAtom instance {
		get; set;
	}
#if CRIWARE_SUPPORT_SEQUENCE_CALLBACK
	private static CriAtomExSequencer.EventCbFunc eventUserCbFunc = null;
	#if CRIWARE_CALLBACK_IMPL_UNITYSENDMESSAGE
	//  no use of  event queue since event is directly passed  from native to managed
	#elif CRIWARE_CALLBACK_IMPL_NATIVE2MANAGED
	private static Queue<string> eventQueue = new Queue<string>();
	#endif
#endif
	/* @endcond */

	#region Functions

	/**
	 * <summary>DSP�o�X�ݒ�̃A�^�b�`</summary>
	 * <param name="settingName">DSP�o�X�ݒ�̖��O</param>
	 * \par ����:
	 * DSP�o�X�ݒ肩��DSP�o�X���\�z���ăT�E���h�����_���ɃA�^�b�`���܂��B<br/>
	 * ���ݐݒ蒆��DSP�o�X�ݒ��؂�ւ���ꍇ�́A��x�f�^�b�`���Ă���ăA�^�b�`���Ă��������B
	 * <br/>
	 * \attention
	 * �{�֐��͊������A�^�̊֐��ł��B<br/>
	 * �{�֐������s����ƁA���΂炭�̊�Atom���C�u�����̃T�[�o�������u���b�N����܂��B<br/>
	 * �����Đ����ɖ{�֐������s����ƁA���r�؂ꓙ�̕s�����������\�������邽�߁A
	 * �{�֐��̌Ăяo���̓V�[���̐؂�ւ�蓙�A���וϓ������e�ł���^�C�~���O�ōs���Ă��������B<br/>
	 * \sa CriAtom::DetachDspBusSetting
	 */
	public static void AttachDspBusSetting(string settingName)
	{
		CriAtom.instance.dspBusSetting = settingName;
		if (!String.IsNullOrEmpty(settingName)) {
			CriAtomEx.AttachDspBusSetting(settingName);
		} else {
			CriAtomEx.DetachDspBusSetting();
		}
	}

	/**
	 * <summary>DSP�o�X�ݒ�̃f�^�b�`</summary>
	 * \par ����:
	 * DSP�o�X�ݒ���f�^�b�`���܂��B<br/>
	 * <br/>
	 * \attention
	 * �{�֐��͊������A�^�̊֐��ł��B<br/>
	 * �{�֐������s����ƁA���΂炭�̊�Atom���C�u�����̃T�[�o�������u���b�N����܂��B<br/>
	 * �����Đ����ɖ{�֐������s����ƁA���r�؂ꓙ�̕s�����������\�������邽�߁A
	 * �{�֐��̌Ăяo���̓V�[���̐؂�ւ�蓙�A���וϓ������e�ł���^�C�~���O�ōs���Ă��������B<br/>
	 * \sa CriAtom::DetachDspBusSetting
	 */
	public static void DetachDspBusSetting()
	{
		CriAtom.instance.dspBusSetting = "";
		CriAtomEx.DetachDspBusSetting();
	}

	/**
	 * <summary>�L���[�V�[�g�̎擾</summary>
	 * <param name="name">�L���[�V�[�g��</param>
	 * <returns>�L���[�V�[�g�I�u�W�F�N�g</returns>
	 * \par ����:
	 * �����Ɏw�肵���L���[�V�[�g�������ɓo�^�ς݂̃L���[�V�[�g�I�u�W�F�N�g���擾���܂��B<br/>
	 */
	public static CriAtomCueSheet GetCueSheet(string name)
	{
		return CriAtom.instance.GetCueSheetInternal(name);
	}

	/**
	 * <summary>�L���[�V�[�g�̒ǉ�</summary>
	 * <param name="name">�L���[�V�[�g��</param>
	 * <param name="acbFile">ACB�t�@�C���p�X</param>
	 * <param name="awbFile">AWB�t�@�C���p�X</param>
	 * <param name="binder">�o�C���_�I�u�W�F�N�g(�I�v�V����)</param>
	 * <returns>�L���[�V�[�g�I�u�W�F�N�g</returns>
	 * \par ����:
	 * �����Ɏw�肵���t�@�C���p�X�������ɃL���[�V�[�g�̒ǉ����s���܂��B<br/>
	 * �����ɕ����̃L���[�V�[�g��o�^���邱�Ƃ��\�ł��B<br/>
	 * <br/>
	 * �e�t�@�C���p�X�ɑ΂��đ��΃p�X���w�肵���ꍇ�� StreamingAssets �t�H���_����̑��΂Ńt�@�C�������[�h���܂��B<br/>
	 * ��΃p�X�A���邢��URL���w�肵���ꍇ�ɂ͎w�肵���p�X�Ńt�@�C�������[�h���܂��B<br/>
	 * <br/>
	 * CPK�t�@�C���Ƀp�b�L���O���ꂽACB�t�@�C����AWB�t�@�C������L���[�V�[�g��ǉ�����ꍇ�́A
	 * binder������CPK���o�C���h�����o�C���_���w�肵�Ă��������B<br/>
	 * �Ȃ��A�o�C���_�@�\��ADX2LE�ł͂����p�ɂȂ�܂���B<br/>
	 */
	public static CriAtomCueSheet AddCueSheet(string name, string acbFile, string awbFile, CriFsBinder binder = null)
	{
		CriAtomCueSheet cueSheet = CriAtom.instance.AddCueSheetInternal(name, acbFile, awbFile, binder);
		if (Application.isPlaying) {
		#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
			CriAtom.instance.LoadAcbFileAsync(cueSheet, binder, acbFile, awbFile);
		#else
			cueSheet.acb = CriAtom.instance.LoadAcbFile(binder, acbFile, awbFile);
		#endif
		}
		return cueSheet;
	}

	/**
	 * <summary>�L���[�V�[�g�̒ǉ��i����������̓ǂݍ��݁j</summary>
	 * <param name="name">�L���[�V�[�g��</param>
	 * <param name="acbData">ACB�f�[�^</param>
	 * <param name="awbFile">AWB�t�@�C���p�X</param>
	 * <param name="awbBinder">AWB�p�o�C���_�I�u�W�F�N�g(�I�v�V����)</param>
	 * <returns>�L���[�V�[�g�I�u�W�F�N�g</returns>
	 * \par ����:
	 * �����Ɏw�肵���f�[�^�ƃt�@�C���p�X��񂩂�L���[�V�[�g�̒ǉ����s���܂��B<br/>
	 * �����ɕ����̃L���[�V�[�g��o�^���邱�Ƃ��\�ł��B<br/>
	 * <br/>
	 * �t�@�C���p�X�ɑ΂��đ��΃p�X���w�肵���ꍇ�� StreamingAssets �t�H���_����̑��΂Ńt�@�C�������[�h���܂��B<br/>
	 * ��΃p�X�A���邢��URL���w�肵���ꍇ�ɂ͎w�肵���p�X�Ńt�@�C�������[�h���܂��B<br/>
	 * <br/>
	 * CPK�t�@�C���Ƀp�b�L���O���ꂽAWB�t�@�C������L���[�V�[�g��ǉ�����ꍇ�́A
	 * awbBinder������CPK���o�C���h�����o�C���_���w�肵�Ă��������B<br/>
	 * �Ȃ��A�o�C���_�@�\��ADX2LE�ł͂����p�ɂȂ�܂���B<br/>
	 */
	public static CriAtomCueSheet AddCueSheet(string name, byte[] acbData, string awbFile, CriFsBinder awbBinder = null)
	{
		CriAtomCueSheet cueSheet = CriAtom.instance.AddCueSheetInternal(name, "", awbFile, awbBinder);
		if (Application.isPlaying) {
		#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
			CriAtom.instance.LoadAcbDataAsync(cueSheet, acbData, awbBinder, awbFile);
		#else
			cueSheet.acb = CriAtom.instance.LoadAcbData(acbData, awbBinder, awbFile);
		#endif
		}
		return cueSheet;
	}

	/**
	 * <summary>�L���[�V�[�g�̍폜</summary>
	 * <param name="name">�L���[�V�[�g��</param>
	 * \par ����:
	 * �ǉ��ς݂̃L���[�V�[�g���폜���܂��B<br/>
	 */
	public static void RemoveCueSheet(string name)
	{
		CriAtom.instance.RemoveCueSheetInternal(name);
	}

	/**
	 * <summary>�L���[�V�[�g�̃��[�h�����`�F�b�N</summary>
	 * \par ����:
	 * �S�ẴL���[�V�[�g�̃��[�h�������`�F�b�N���܂��B<br/>
	 */
	public static bool CueSheetsAreLoading {
		get {
			if (CriAtom.instance == null) {
				return false;
			}
			foreach (var cueSheet in CriAtom.instance.cueSheets) {
				if (cueSheet.IsLoading) {
					return true;
				}
			}
			return false;
		}
	}

	/**
	 * <summary>ACB�I�u�W�F�N�g�̎擾</summary>
	 * <param name="cueSheetName">�L���[�V�[�g��</param>
	 * <returns>ACB�I�u�W�F�N�g</returns>
	 * \par ����:
	 * �w�肵���L���[�V�[�g�ɑΉ�����ACB�I�u�W�F�N�g���擾���܂��B<br/>
	 */
	public static CriAtomExAcb GetAcb(string cueSheetName)
	{
		foreach (var cueSheet in CriAtom.instance.cueSheets) {
			if (cueSheetName == cueSheet.name) {
				return cueSheet.acb;
			}
		}
		Debug.LogWarning(cueSheetName + " is not loaded.");
		return null;
	}

	/**
	 * <summary>�J�e�S�����w��ŃJ�e�S���{�����[����ݒ肵�܂��B</summary>
	 * <param name="name">�J�e�S����</param>
	 * <param name="volume">�{�����[��</param>
	 */
	public static void SetCategoryVolume(string name, float volume)
	{
		CriAtomExCategory.SetVolume(name, volume);
	}

	/**
	 * <summary>�J�e�S��ID�w��ŃJ�e�S���{�����[����ݒ肵�܂��B</summary>
	 * <param name="id">�J�e�S��ID</param>
	 * <param name="volume">�{�����[��</param>
	 */
	public static void SetCategoryVolume(int id, float volume)
	{
		CriAtomExCategory.SetVolume(id, volume);
	}

	/**
	 * <summary>�J�e�S�����w��ŃJ�e�S���{�����[�����擾���܂��B</summary>
	 * <param name="name">�J�e�S����</param>
	 * <returns>�{�����[��</returns>
	 */
	public static float GetCategoryVolume(string name)
	{
		return CriAtomExCategory.GetVolume(name);
	}
	/**
	 * <summary>�J�e�S��ID�w��ŃJ�e�S���{�����[�����擾���܂��B</summary>
	 * <param name="id">�J�e�S��ID</param>
	 * <returns>�{�����[��</returns>
	 */
	public static float GetCategoryVolume(int id)
	{
		return CriAtomExCategory.GetVolume(id);
	}

	/**
	 * <summary>�o�X���擾��L���ɂ��܂��B</summary>
	 * <param name="sw">true: �擾��L���ɂ���B false: �擾�𖳌��ɂ���</param>
	 */
	public static void SetBusAnalyzer(bool sw)
	{
	#if !UNITY_EDITOR && UNITY_PSP2
		// unsupported
	#else
		if (sw) {
			CriAtomExAsr.AttachBusAnalyzer(50, 1000);
		} else {
			CriAtomExAsr.DetachBusAnalyzer();
		}
	#endif
	}

	/**
	 * <summary>�o�X�����擾���܂��B</summary>
	 * <param name="bus">DSP�o�X�ԍ�</param>
	 * <returns>DSP�o�X���</returns>
	 */
	public static CriAtomExAsr.BusAnalyzerInfo GetBusAnalyzerInfo(int bus)
	{
		CriAtomExAsr.BusAnalyzerInfo info;
	#if !UNITY_EDITOR && UNITY_PSP2
		info = new CriAtomExAsr.BusAnalyzerInfo(null);
	#else
		CriAtomExAsr.GetBusAnalyzerInfo(bus, out info);
	#endif
		return info;
	}

	#endregion // Functions

	/* @cond DOXYGEN_IGNORE */
	#region Functions for internal use

	private void Setup()
	{
		CriAtom.instance = this;

		CriAtomPlugin.InitializeLibrary();

		if (!String.IsNullOrEmpty(this.acfFile)) {
			string acfPath = Path.Combine(CriWare.streamingAssetsPath, this.acfFile);
			CriAtomEx.RegisterAcf(null, acfPath);
		}

		if (!String.IsNullOrEmpty(dspBusSetting)) {
			AttachDspBusSetting(dspBusSetting);
		}

		foreach (var cueSheet in this.cueSheets) {
			cueSheet.acb = this.LoadAcbFile(null, cueSheet.acbFile, cueSheet.awbFile);
		}

		if (this.dontDestroyOnLoad) {
			GameObject.DontDestroyOnLoad(this.gameObject);
		}
	}

	private void Shutdown()
	{
		foreach (var cueSheet in this.cueSheets) {
			if (cueSheet.acb != null) {
				cueSheet.acb.Dispose();
				cueSheet.acb = null;
			}
		#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
			if (cueSheet.async != null) {
				cueSheet.async.Dispose();
				cueSheet.async = null;
			}
		#endif
		}
		CriAtomPlugin.FinalizeLibrary();
		CriAtom.instance = null;
	}

	private void Awake()
	{
		if (CriAtom.instance != null) {
			if (CriAtom.instance.acfFile != this.acfFile) {
				var obj = CriAtom.instance.gameObject;
				CriAtom.instance.Shutdown();
				CriAtomEx.UnregisterAcf();
				GameObject.Destroy(obj);
				return;
			}
			if (CriAtom.instance.dspBusSetting != this.dspBusSetting) {
				CriAtom.AttachDspBusSetting(this.dspBusSetting);
			}
			CriAtom.instance.MargeCueSheet(this.cueSheets, this.dontRemoveExistsCueSheet);
			GameObject.Destroy(this.gameObject);
		}
	}

	private void OnEnable()
	{
	#if UNITY_EDITOR
		if (CriAtomPlugin.previewCallback != null) {
			CriAtomPlugin.previewCallback();
		}
	#endif
		if (CriAtom.instance != null) {
			return;
		}

	#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
		this.SetupAsync();
	#else
		this.Setup();
	#endif
	}

	private void OnDestroy()
	{
		if (this != CriAtom.instance) {
			return;
		}
		this.Shutdown();
	}

	private void Update()
	{
#if CRIWARE_SUPPORT_SEQUENCE_CALLBACK
	#if CRIWARE_CALLBACK_IMPL_UNITYSENDMESSAGE
		// no need to invoke the delegate since it will be invoked via the callback directly.
	#elif CRIWARE_CALLBACK_IMPL_NATIVE2MANAGED
		if (eventUserCbFunc != null) {
			int numQues = eventQueue.Count;
			string msg;

			for (int i=0;i<numQues;i++) {
				/* �L���[�C���O���ꂽ�C�x���g������΂�������s���� */
				lock (((ICollection)eventQueue).SyncRoot)
				{
					msg = eventQueue.Dequeue();	/* �f�L���[�̏u�Ԃ����r��������s�� */
				}
				eventUserCbFunc(msg);
			}
		}
	#endif
#endif
	}

	public CriAtomCueSheet GetCueSheetInternal(string name)
	{
		for (int i = 0; i < this.cueSheets.Length; i++) {
			CriAtomCueSheet cueSheet = this.cueSheets[i];
			if (cueSheet.name == name) {
				return cueSheet;
			}
		}
		return null;
	}

	public CriAtomCueSheet AddCueSheetInternal(string name, string acbFile, string awbFile, CriFsBinder binder)
	{
		var cueSheets = new CriAtomCueSheet[this.cueSheets.Length + 1];
		this.cueSheets.CopyTo(cueSheets, 0);
		this.cueSheets = cueSheets;

		var cueSheet = new CriAtomCueSheet();
		this.cueSheets[this.cueSheets.Length - 1] = cueSheet;
		if (String.IsNullOrEmpty(name)) {
			cueSheet.name = Path.GetFileNameWithoutExtension(acbFile);
		} else {
			cueSheet.name = name;
		}
		cueSheet.acbFile = acbFile;
		cueSheet.awbFile = awbFile;
		return cueSheet;
	}

	public void RemoveCueSheetInternal(string name)
	{
		int index = -1;
		for (int i = 0; i < this.cueSheets.Length; i++) {
			if (name == this.cueSheets[i].name) {
				index = i;
			}
		}
		if (index < 0) {
			return;
		}

		var cueSheet = this.cueSheets[index];
		if (cueSheet.acb != null) {
			cueSheet.acb.Dispose();
			cueSheet.acb = null;
		}
	#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
		if (cueSheet.async != null) {
			cueSheet.async.Dispose();
			cueSheet.async = null;
		}
	#endif

		var cueSheets = new CriAtomCueSheet[this.cueSheets.Length - 1];
		Array.Copy(this.cueSheets, 0, cueSheets, 0, index);
		Array.Copy(this.cueSheets, index + 1, cueSheets, index, this.cueSheets.Length - index - 1);
		this.cueSheets = cueSheets;
	}

	public bool dontRemoveExistsCueSheet = false;

	/*
	 * newDontRemoveExistsCueSheet == false �̏ꍇ�A�Â��L���[�V�[�g��o�^�������āA�V�����L���[�V�[�g�̓o�^���s���B
	 * �����������L���[�V�[�g�̍ēo�^�͉������
	 */
	private void MargeCueSheet(CriAtomCueSheet[] newCueSheets, bool newDontRemoveExistsCueSheet)
	{
		if (!newDontRemoveExistsCueSheet) {
			for (int i = 0; i < this.cueSheets.Length; ) {
				int index = Array.FindIndex(newCueSheets, sheet => sheet.name == this.cueSheets[i].name);
				if (index < 0) {
					CriAtom.RemoveCueSheet(this.cueSheets[i].name);
				} else {
					i++;
				}
			}
		}

		foreach (var sheet1 in newCueSheets) {
			var sheet2 = this.GetCueSheetInternal(sheet1.name);
			if (sheet2 == null) {
				CriAtom.AddCueSheet(null, sheet1.acbFile, sheet1.awbFile, null);
			}
		}
	}

	private CriAtomExAcb LoadAcbFile(CriFsBinder binder, string acbFile, string awbFile)
	{
		if (String.IsNullOrEmpty(acbFile)) {
			return null;
		}

		string acbPath = acbFile;
		if ((binder == null) && CriWare.IsStreamingAssetsPath(acbPath)) {
			acbPath = Path.Combine(CriWare.streamingAssetsPath, acbPath);
		}

		string awbPath = awbFile;
		if (!String.IsNullOrEmpty(awbPath)) {
			if ((binder == null) && CriWare.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.streamingAssetsPath, awbPath);
			}
		}

		return CriAtomExAcb.LoadAcbFile(binder, acbPath, awbPath);
	}

	private CriAtomExAcb LoadAcbData(byte[] acbData, CriFsBinder binder, string awbFile)
	{
		if (acbData == null) {
			return null;
		}

		string awbPath = awbFile;
		if (!String.IsNullOrEmpty(awbPath)) {
			if ((binder == null) && CriWare.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.streamingAssetsPath, awbPath);
			}
		}

		return CriAtomExAcb.LoadAcbData(acbData, binder, awbPath);
	}

#if (!UNITY_EDITOR || UNITY_EDITOR_OSX) && UNITY_WEBGL
	private void SetupAsync()
	{
		CriAtom.instance = this;

		CriAtomPlugin.InitializeLibrary();

		if (this.dontDestroyOnLoad) {
			GameObject.DontDestroyOnLoad(this.gameObject);
		}

		if (!String.IsNullOrEmpty(this.acfFile)) {
			this.acfIsLoading = true;
			StartCoroutine(RegisterAcfAsync(this.acfFile, this.dspBusSetting));
		}

		foreach (var cueSheet in this.cueSheets) {
			StartCoroutine(LoadAcbFileCoroutine(cueSheet, null, cueSheet.acbFile, cueSheet.awbFile));
		}
	}

	private IEnumerator RegisterAcfAsync(string acfFile, string dspBusSetting)
	{
	#if UNITY_EDITOR
		string acfPath = "file://" + CriWare.streamingAssetsPath + "/" + acfFile;
	#else
		string acfPath = CriWare.streamingAssetsPath + "/" + acfFile;
	#endif
		using (var req = new WWW(acfPath)) {
			yield return req;
			this.acfData = req.bytes;
		}
		CriAtomEx.RegisterAcf(this.acfData);

		if (!String.IsNullOrEmpty(dspBusSetting)) {
			AttachDspBusSetting(dspBusSetting);
		}
		this.acfIsLoading = false;
	}

	private void LoadAcbFileAsync(CriAtomCueSheet cueSheet, CriFsBinder binder, string acbFile, string awbFile)
	{
		if (String.IsNullOrEmpty(acbFile)) {
			return;
		}

		StartCoroutine(LoadAcbFileCoroutine(cueSheet, binder, acbFile, awbFile));
	}

	private IEnumerator LoadAcbFileCoroutine(CriAtomCueSheet cueSheet, CriFsBinder binder, string acbPath, string awbPath)
	{
		if ((binder == null) && CriWare.IsStreamingAssetsPath(acbPath)) {
			acbPath = Path.Combine(CriWare.streamingAssetsPath, acbPath);
		}

		if (!String.IsNullOrEmpty(awbPath)) {
			if ((binder == null) && CriWare.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.streamingAssetsPath, awbPath);
			}
		}

		while (this.acfIsLoading) {
			yield return new WaitForEndOfFrame();
		}

		using (var async = CriAtomExAcbAsync.LoadAcbFile(binder, acbPath, awbPath)) {
			cueSheet.async = async;
			while (true) {
				var status = async.GetStatus();
				if (status == CriAtomExAcbAsync.Status.Complete) {
					cueSheet.acb = async.MoveAcb();
					break;
				} else if (status == CriAtomExAcbAsync.Status.Error) {
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			cueSheet.async = null;
		}
	}

	private void LoadAcbDataAsync(CriAtomCueSheet cueSheet, byte[] acbData, CriFsBinder awbBinder, string awbFile)
	{
		StartCoroutine(LoadAcbDataCoroutine(cueSheet, acbData, awbBinder, awbFile));
	}

	private IEnumerator LoadAcbDataCoroutine(CriAtomCueSheet cueSheet, byte[] acbData, CriFsBinder awbBinder, string awbPath)
	{
		if (!String.IsNullOrEmpty(awbPath)) {
			if ((awbBinder == null) && CriWare.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.streamingAssetsPath, awbPath);
			}
		}

		while (this.acfIsLoading) {
			yield return new WaitForEndOfFrame();
		}

		using (var async = CriAtomExAcbAsync.LoadAcbData(acbData, awbBinder, awbPath)) {
			cueSheet.async = async;
			while (true) {
				var status = async.GetStatus();
				if (status == CriAtomExAcbAsync.Status.Complete) {
					cueSheet.acb = async.MoveAcb();
					break;
				} else if (status == CriAtomExAcbAsync.Status.Error) {
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			cueSheet.async = null;
		}
	}
#endif

#if CRIWARE_SUPPORT_SEQUENCE_CALLBACK
	#if CRIWARE_CALLBACK_IMPL_UNITYSENDMESSAGE
	public void EventCallbackFromNative(string eventString)
	{
		if (eventUserCbFunc != null) {
			eventUserCbFunc(eventString);
		}
	}
	#elif CRIWARE_CALLBACK_IMPL_NATIVE2MANAGED
	[AOT.MonoPInvokeCallback(typeof(CriAtomExSequencer.EventCbFunc))]
	public static void EventCallbackFromNative(string eventString)
	{
		if (eventUserCbFunc != null) {
			/* �C�x���g�̃L���[�C���O */
			/* ���l) iOS�ȊO��CRI Atom���̃X���b�h����Ăяo����邽�� */
			lock (((ICollection)eventQueue).SyncRoot)
			{
				eventQueue.Enqueue(eventString);
			}
		}
	}
	#endif
#endif

	/* �v���O�C�������pAPI */
	public static void SetEventCallback(CriAtomExSequencer.EventCbFunc func, string separator)
	{
#if CRIWARE_SUPPORT_SEQUENCE_CALLBACK
		/* �l�C�e�B�u�v���O�C���Ɋ֐��|�C���^��o�^ */
		IntPtr ptr = IntPtr.Zero;
		eventUserCbFunc = func;
		if (func != null) {
	#if CRIWARE_CALLBACK_IMPL_UNITYSENDMESSAGE
			ptr = IntPtr.Zero;
	#elif CRIWARE_CALLBACK_IMPL_NATIVE2MANAGED
			CriAtomExSequencer.EventCbFunc delegateObject;
			delegateObject = new CriAtomExSequencer.EventCbFunc(CriAtom.EventCallbackFromNative);
			ptr = Marshal.GetFunctionPointerForDelegate(delegateObject);
	#endif
			CriAtomPlugin.criAtomUnitySeqencer_SetEventCallback(ptr, CriAtom.instance.gameObject.name, "EventCallbackFromNative", separator);
		}
#elif CRIWARE_UNSUPPORT_SEQUENCE_CALLBACK
		Debug.LogError("This platform does not support event callback feature.");
#endif
	}

	#endregion
	/* @endcond */

}

/// @}
/* end of file */
