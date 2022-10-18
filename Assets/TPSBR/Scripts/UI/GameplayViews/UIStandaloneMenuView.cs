using Fusion;

namespace TPSBR.UI
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	public class UIStandaloneMenuView : UIBehaviour
	{
		/*// PRIVATE MEMBERS

		[SerializeField]
		private Sprite _activeButtonSprite;
		[SerializeField]
		private Sprite _inactiveButtonSprite;

		[SerializeField]
		private Image _ultra;
		[SerializeField]
		private Image _high;
		[SerializeField]
		private Image _medium;
		[SerializeField]
		private Image _low;

		[SerializeField]
		private Image _unlimitedFPS;
		[SerializeField]
		private Image _60FPS;
		[SerializeField]
		private Image _288FPS;

		[SerializeField]
		private Image _deathmatch;
		[SerializeField]
		private Image _elimination;
		[SerializeField]
		private Image _battleRoyale;

		[SerializeField]
		private GameObject _marine;
		[SerializeField]
		private GameObject _policeman;
		[SerializeField]
		private GameObject _soldier;

		[SerializeField]
		private GameObject _extraClients;

		[SerializeField]
		private TMP_InputField _nicknameInputField;
		[SerializeField]
		private TMP_InputField _roomNameInputField;
		[SerializeField]
		private TMP_InputField _extraClientsInputField;

		[SerializeField]
		private TextMeshProUGUI[] _sessions;
		[SerializeField]
		private GameObject[] _adminObjects;

		[SerializeField]
		private GameManager _gameManager;

		private bool          _hasStarted;
		private NetworkRunner _lobbyRunner;

		private Scene         _scene;

		// PUBLIC METHODS

		public void SetQuality(int quality)
		{
			PlayerPrefs.SetInt("QualityLevel", quality);
			QualitySettings.SetQualityLevel(quality, true);

			Refresh();
		}

		public void SetFPS(int fps)
		{
			PlayerPrefs.SetInt("TargetFrameRate", fps);
			Application.targetFrameRate = fps;

			Refresh();
		}

		public void SetAgent(int agent)
		{
			_gameManager.SetAgent(agent);

			Refresh();
		}

		public void SetGameplay(int gameplay)
		{
			_gameManager.SetGameplayMode(gameplay);

			Refresh();
		}

		public void Play(int mode)
		{
			if (int.TryParse(_extraClientsInputField.text, out int extraPeers) == true)
			{
				_gameManager.SetExtraPeers(extraPeers);
			}

			switch (mode)
			{
				case 0: { _gameManager.StartClient(); return; }
				case 1: { _gameManager.StartHost();   return; }
				case 2: { _gameManager.StartServer(); return; }
			}
		}

		public void JoinSession(int index)
		{
			_gameManager.SetSessionName(_sessions[index].text, false);
			_gameManager.StartClient();
		}

		// MONOBEHAVIOUR

		private void Awake()
		{
			for (int i = 0; i < _adminObjects.Length; ++i)
			{
				_adminObjects[i].SetActive(false);
			}
		}

		private void Start()
		{
			_nicknameInputField.text = _gameManager.Nickname;
			_roomNameInputField.text = _gameManager.Session;

			if (_gameManager.ShowUI == true)
			{
				int quality = PlayerPrefs.GetInt("QualityLevel", 3);
				if (quality != QualitySettings.GetQualityLevel())
				{
					QualitySettings.SetQualityLevel(quality, true);
				}

				Application.targetFrameRate = PlayerPrefs.GetInt("TargetFrameRate", 60);
			}

			_nicknameInputField.onValueChanged.RemoveAllListeners();
			_roomNameInputField.onValueChanged.RemoveAllListeners();

			_nicknameInputField.onValueChanged.AddListener(OnNicknameChanged);
			_roomNameInputField.onValueChanged.AddListener(OnRoomNameChanged);

			OnSessionListUpdated(null, new List<SessionInfo>());

			Refresh();

			if (_gameManager.ShowUI == false)
				return;

			FusionCallbacksHandler callbacksHandler = new FusionCallbacksHandler();
			callbacksHandler.SessionListUpdated = OnSessionListUpdated;

			_lobbyRunner = new GameObject("LobbyRunner").AddComponent<NetworkRunner>();
			_lobbyRunner.transform.parent = transform;
			_lobbyRunner.AddCallbacks(callbacksHandler);
			_lobbyRunner.JoinSessionLobby(SessionLobby.Custom, "Servers");
		}

		private void OnEnable()
		{
			_scene = FindObjectOfType<Scene>(true);
		}

		private void Update()
		{
			if (_gameManager == null || _gameManager.ShowUI == false)
			{
				if (_lobbyRunner != null)
				{
					Destroy(_lobbyRunner.gameObject);
					_lobbyRunner = null;
				}

				_nicknameInputField.onValueChanged.RemoveAllListeners();
				_roomNameInputField.onValueChanged.RemoveAllListeners();

				gameObject.SetActive(false);
				return;
			}

			if (_scene != null)
			{
				_scene.Context.Input.RequestCursorVisibility(true, ECursorStateSource.Menu);
			}
		}

		private void OnDisable()
		{
			if (_scene != null)
			{
				_scene.Context.Input.RequestCursorVisibility(false, ECursorStateSource.Menu);
			}
		}

		// PRIVATE METHODS

		private void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> list)
		{
			int i     = 0;
			int count = _sessions.Length;

			while (i < list.Count && i < count)
			{
				_sessions[i].text = list[i].Name;
				_sessions[i].transform.parent.GetComponent<CanvasGroup>().interactable = true;
				++i;
			}

			while (i < count)
			{
				_sessions[i].text = "";
				_sessions[i].transform.parent.GetComponent<CanvasGroup>().interactable = false;
				++i;
			}
		}

		private void OnNicknameChanged(string nickname)
		{
			var trimmed = nickname.Trim();
			if (trimmed != nickname)
			{
				nickname = trimmed;
				_nicknameInputField.text = nickname;
			}

			_gameManager.SetNickname(nickname);
		}

		private void OnRoomNameChanged(string roomName)
		{
			_gameManager.SetSessionName(roomName);
		}

		private void Refresh()
		{
			int quality = QualitySettings.GetQualityLevel();
			int fps     = Application.targetFrameRate;

			_ultra.sprite  = quality == 3 ? _activeButtonSprite : _inactiveButtonSprite;
			_high.sprite   = quality == 2 ? _activeButtonSprite : _inactiveButtonSprite;
			_medium.sprite = quality == 1 ? _activeButtonSprite : _inactiveButtonSprite;
			_low.sprite    = quality == 0 ? _activeButtonSprite : _inactiveButtonSprite;

			_unlimitedFPS.sprite = fps == 0   ? _activeButtonSprite : _inactiveButtonSprite;
			_60FPS.sprite        = fps == 60  ? _activeButtonSprite : _inactiveButtonSprite;
			_288FPS.sprite       = fps == 288 ? _activeButtonSprite : _inactiveButtonSprite;

			_deathmatch.sprite  = _gameManager.GameplayModeID == 0 ? _activeButtonSprite : _inactiveButtonSprite;
			_elimination.sprite = _gameManager.GameplayModeID == 1 ? _activeButtonSprite : _inactiveButtonSprite;
			_battleRoyale.sprite = _gameManager.GameplayModeID == 2 ? _activeButtonSprite : _inactiveButtonSprite;

			_marine.SetActive(_gameManager.AgentPrefabID == 0);
			_policeman.SetActive(_gameManager.AgentPrefabID == 1);
			_soldier.SetActive(_gameManager.AgentPrefabID == 2);

			bool adminObjectsVisible;

#if UNITY_EDITOR
			adminObjectsVisible = true;
#else
			adminObjectsVisible = ApplicationUtility.HasCommandLineArgument("-admin");
#endif
			for (int i = 0; i < _adminObjects.Length; ++i)
			{
				_adminObjects[i].SetActive(adminObjectsVisible);
			}
		}*/
	}
}
