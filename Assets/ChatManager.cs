using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WebSocketSharp;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Collections;


#region Modelos

[System.Serializable]
public class ServerMessage { public string eventName; public string msg; public OnlinePlayer data; }

[System.Serializable]
public class OutgoingLoginMessage { public string @event; public LoginData data; }

[System.Serializable]
public class OutgoingMessage { public string @event; public OutgoingData data; }

[System.Serializable]
public class OutgoingData { public string message; }

[System.Serializable]
public class LoginData { public string gameKey; }

[System.Serializable]
public class PublicMessage { public string eventName; public PublicMessageData data; }

[System.Serializable]
public class PublicMessageData { public string playerId; public string playerName; public string playerMsg; }

[System.Serializable]
public class ClientRequest { public string @event; }

[System.Serializable]
public class PlayerListMessageWrapper { public string eventName; public string status; public string msg; public List<OnlinePlayer> data; }

[System.Serializable]
public class OnlinePlayer { public string id; public string name; public GameData game; public string status; }

[System.Serializable]
public class GameData { public string id; public string name; public string team; }

[System.Serializable]
public class MatchRequest { public string @event; public MatchRequestData data; }

[System.Serializable]
public class MatchRequestData { public string playerId; }

[System.Serializable]
public class MatchRequestReceivedData { public string playerId; public string matchId; }

[System.Serializable]
public class ServerMatchRequest { public string eventName; public string msg; public MatchRequestReceivedData data; }

[System.Serializable]
public class SimpleEvent { public string @event; }

[System.Serializable]
public class PlayerStatusChangedMessage { public string eventName; public string msg; public PlayerStatusChangedData data; }

[System.Serializable]
public class PlayerStatusChangedData { public string playerId; public string playerStatus; }

[System.Serializable]
public class ConnectMatchMessage { public string @event = "connect-match"; public ConnectMatchData data; }

[System.Serializable]
public class ConnectMatchData { public string matchId; }

[System.Serializable]
public class PlayersReadyMessage { public string eventName; public string msg; public MatchReadyData data; }

[System.Serializable]
public class MatchReadyData { public string matchId; }

[System.Serializable]
public class MatchStartMessage { public string eventName; public string msg; public MatchReadyData data; }

[System.Serializable]
public class PrivateMessageRequest { public string @event = "send-private-message"; public PrivateMessageData data; }

[System.Serializable]
public class PrivateMessageData { public string playerId; public string message; }

#endregion



public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance { get; private set; }

    [Header("UI References")]
    public Transform UserListContainer;
    public GameObject UserEntryPrefab;
    public TextMeshProUGUI chatText;
    public TMP_InputField messageInput;
    public ScrollRect chatScrollRect;

    [Header("Context Menu")]
    public GameObject contextMenuPanel;
    public Button inviteButton;
    public Button privateMsgButton;

    [Header("Match Request Popup")]
    public GameObject matchRequestPopup;
    public TextMeshProUGUI popupText;
    public Button acceptMatchButton;
    public Button rejectMatchButton;

    [Header("Match Rematch Button")]
    public Button botonRevancha;

    [Header("Match Connect Panel")]
    public Button roomButton;
    public Button pingButton;

    private List<OnlinePlayer> connectedPlayers = new List<OnlinePlayer>();
    private WebSocket ws;
    private string myId = "";
    private string nombreUsuario = "";
    private string selectedPlayerId = "";
    private string selectedPlayerName = "";
    private string pendingMatchId = "";
    private bool enviarPrivado = false;
    private bool yaJugamosUnaPartida = false;






    [SerializeField] private SlowMovementUpgrade slowDebuffSO;
    [SerializeField] private AutoDamageUpgrade autoDamageDebuffSO;
    [SerializeField] private DowngradeSkillUpgrade downgradeSkillDebuffSO;
    [SerializeField] private PlayerLose playerLose;


    private GameObject player; // referencia al jugador

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (FindFirstObjectByType<UnityMainThreadDispatcher>() == null)
            gameObject.AddComponent<UnityMainThreadDispatcher>();

        // Conexión WebSocket
<<<<<<< Updated upstream
        ws = new WebSocket("ws://ucn-game-server.martux.cl:4010/?gameId=F&playerName=ElNochi");
=======
        ws = new WebSocket("ws://ucn-game-server.martux.cl:4010/?gameId=F&playerName=ElNochi22");
>>>>>>> Stashed changes
        ws.OnOpen += OnWebSocketOpen;
        ws.OnMessage += OnWebSocketMessage;
        ws.Connect();

        // Ocultar elementos al inicio
        contextMenuPanel.SetActive(false);
        roomButton.gameObject.SetActive(false);
        pingButton.gameObject.SetActive(false);
        botonRevancha.gameObject.SetActive(false);


        // Listener del input
        if (messageInput != null)
            messageInput.onSubmit.AddListener(OnSubmitMessage);

        // Listener del botón de revancha
        botonRevancha.onClick.RemoveAllListeners();
        botonRevancha.onClick.AddListener(EnviarSolicitudRematch);

        // Botones conexión y ping
        roomButton.onClick.AddListener(ConectarAPartida);
        pingButton.onClick.AddListener(EnviarPingMatch);

        // Menú contextual
        inviteButton.onClick.AddListener(() =>
        {
            EnviarSolicitudPartida(selectedPlayerId, selectedPlayerName);
            contextMenuPanel.SetActive(false);
        });

        privateMsgButton.onClick.AddListener(() =>
        {
            enviarPrivado = true;
            contextMenuPanel.SetActive(false);
            messageInput?.ActivateInputField();
        });

        // Popup de solicitud de partida
        acceptMatchButton.onClick.RemoveAllListeners();
        acceptMatchButton.onClick.AddListener(() =>
        {
            AceptarPartida();
            matchRequestPopup.SetActive(false);
            roomButton.gameObject.SetActive(true);
        });

        rejectMatchButton.onClick.RemoveAllListeners();
        rejectMatchButton.onClick.AddListener(() =>
        {
            RechazarPartida();
            matchRequestPopup.SetActive(false);
        });



        // Jugador
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogWarning("No se encontró el objeto del jugador.");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }







    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameplayScene" || contextMenuPanel == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    contextMenuPanel.GetComponent<RectTransform>(), Input.mousePosition, null))
            {
                contextMenuPanel.SetActive(false);
            }
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameplayScene")
        {
            chatText = null;
            chatScrollRect = null;
            UserListContainer = null;

            if (botonRevancha != null)
                botonRevancha.gameObject.SetActive(false);
        }
        else if (scene.name == "ChatScene")
        {
            // Reasignar referencias de UI
            chatText = GameObject.Find("ChatText")?.GetComponent<TextMeshProUGUI>();
            chatScrollRect = GameObject.FindObjectOfType<ScrollRect>();
            UserListContainer = GameObject.Find("UserListContainer")?.transform;
            messageInput = GameObject.Find("MessageInput")?.GetComponent<TMP_InputField>();

            if (messageInput != null)
            {
                messageInput.onSubmit.RemoveAllListeners();
                messageInput.onSubmit.AddListener(OnSubmitMessage);
            }

            // ✅ Reasignar botón de revancha
            GameObject botonObj = GameObject.Find("BotonRevancha");
            if (botonObj != null)
            {
                botonRevancha = botonObj.GetComponent<Button>();
                botonRevancha.gameObject.SetActive(true);
                botonRevancha.onClick.RemoveAllListeners();
                botonRevancha.onClick.AddListener(EnviarSolicitudRematch);
            }
            else
            {
                Debug.LogWarning("No se encontró el botón de revancha al volver al lobby.");
            }

            // ✅ Reasignar botón de ping ("Jugar")
            GameObject pingObj = GameObject.Find("ButtonPlay");
            if (pingObj != null)
            {
                pingButton = pingObj.GetComponent<Button>();
                pingButton.gameObject.SetActive(false); // oculto por defecto
                pingButton.onClick.RemoveAllListeners();
                pingButton.onClick.AddListener(EnviarPingMatch);
                Debug.Log("✅ pingButton reasignado en escena ChatScene");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró ButtonPlay al volver al lobby.");
            }

            if (UserListContainer == null)
                Debug.LogWarning("UserListContainer no fue encontrado en el lobby.");

            PedirJugadoresConectados();
        }
    }













    private void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (chatText != null)
                chatText.text += "\n Conectado al servidor";
        });
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            Debug.Log("Mensaje del servidor: " + e.Data);

            try
            {
                string json = e.Data.Replace("\"event\":", "\"eventName\":");

                if (json.Contains("online-players"))
                {
                    PlayerListMessageWrapper playerList = JsonUtility.FromJson<PlayerListMessageWrapper>(json);
                    if (playerList.data != null)
                    {
                        connectedPlayers.Clear();
                        connectedPlayers.AddRange(playerList.data);
                        ActualizarListaUsuariosUI();
                    }
                    return;
                }

                if (json.Contains("match-request-received"))
                {
                    ServerMatchRequest matchMsg = JsonUtility.FromJson<ServerMatchRequest>(json);

                    string senderId = matchMsg.data.playerId;
                    string matchId = matchMsg.data.matchId;

                    string senderName = senderId;
                    OnlinePlayer sender = connectedPlayers.Find(p => p.id == senderId);
                    if (sender != null) senderName = sender.name;

                    popupText.text = $"Invitación de partida de: {senderName}";
                    matchRequestPopup.SetActive(true);
                    pendingMatchId = matchId;

                    acceptMatchButton.onClick.RemoveAllListeners();
                    acceptMatchButton.onClick.AddListener(() =>
                    {
                        AceptarPartida();
                        matchRequestPopup.SetActive(false);
                        roomButton.gameObject.SetActive(true); // Mostrar botón Jugar
                    });

                    rejectMatchButton.onClick.RemoveAllListeners();
                    rejectMatchButton.onClick.AddListener(() =>
                    {
                        RechazarPartida();
                        matchRequestPopup.SetActive(false);
                    });

                    return;
                }

                if (json.Contains("player-status-changed"))
                {
                    PlayerStatusChangedMessage statusMessage = JsonUtility.FromJson<PlayerStatusChangedMessage>(json);
                    OnlinePlayer jugador = connectedPlayers.Find(p => p.id == statusMessage.data.playerId);
                    if (jugador != null)
                    {
                        jugador.status = statusMessage.data.playerStatus;
                        Debug.Log($"Estado actualizado de {jugador.name} a {jugador.status}");
                        ActualizarListaUsuariosUI();
                    }
                    return;
                }

                ServerMessage serverMessage = JsonUtility.FromJson<ServerMessage>(json);

                if (serverMessage.eventName == "connected-to-server")
                {
                    myId = serverMessage.data.id;
                    login();
                    PedirJugadoresConectados();
                }
                else if (serverMessage.eventName == "login")
                {
                    myId = serverMessage.data.id;
                    nombreUsuario = serverMessage.data.name;
                }
                else if (serverMessage.eventName == "player-connected")
                {
                    if (serverMessage.data.id != myId)
                    {
                        if (chatText != null)
                            chatText.text += $"\n [+] {serverMessage.data.name} se ha conectado";
                        PedirJugadoresConectados();
                    }
                }
                else if (serverMessage.eventName == "player-disconnected")
                {
                    if (serverMessage.data.id != myId)
                    {
                        if (chatText != null)
                            chatText.text += $"\n [-] {serverMessage.data.name} se ha desconectado";
                        PedirJugadoresConectados();
                    }
                }
                else if (serverMessage.eventName == "public-message")
                {
                    PublicMessage mensajePublico = JsonUtility.FromJson<PublicMessage>(json);

                    // Evitar duplicado si es un eco del mismo jugador
                    if (mensajePublico.data != null && mensajePublico.data.playerId != myId)
                    {
                        if (chatText != null)
                            chatText.text += $"\n {mensajePublico.data.playerName}: {mensajePublico.data.playerMsg}";
                        ActualizarChatUI();
                    }

                    return;
                }


                else if (serverMessage.eventName == "private-message")
                {
                    PublicMessage privado = JsonUtility.FromJson<PublicMessage>(json);
                    if (chatText != null)
                        chatText.text += $"\n {privado.data.playerName} [Mensaje Privado]: {privado.data.playerMsg}";
                    ActualizarChatUI();
                    return;
                }

                else if (serverMessage.eventName == "match-accepted")
                {
                    string playerName = ExtraerNombreDesdeMensaje(serverMessage.msg);
                    if (chatText != null)
                        chatText.text += $"\n {playerName} aceptó tu invitación.";

                    // Extraer matchId del JSON
                    string jsonData = e.Data.Replace("\"event\":", "\"eventName\":");
                    var matchAccepted = JsonUtility.FromJson<ServerMatchRequest>(jsonData);
                    pendingMatchId = matchAccepted.data.matchId;

                    // Mostrar botón Jugar también en esta instancia
                    roomButton.gameObject.SetActive(true);

                }
                else if (serverMessage.eventName == "match-rejected")
                {
                    string playerName = ExtraerNombreDesdeMensaje(serverMessage.msg);
                    if (chatText != null)
                        chatText.text += $"\n {playerName} rechazó tu invitación.";

                }
                else if (serverMessage.eventName == "players-ready")
                {
                    PlayersReadyMessage readyMessage = JsonUtility.FromJson<PlayersReadyMessage>(json);
                    string matchId = readyMessage.data.matchId;

                    if (chatText != null)
                        chatText.text += $"\n Ambos jugadores están listos. Partida ID: {matchId}";

                    // Siempre ocultar botón de conexión (por si quedó activo)
                    if (roomButton != null)
                        roomButton.gameObject.SetActive(false);

                    // Mostrar botón para enviar ping
                    if (pingButton != null)
                    {
                        pingButton.gameObject.SetActive(true);
                        Debug.Log("Mostrando pingButton porque ambos jugadores están listos.");
                    }
                    else
                    {
                        Debug.LogWarning("pingButton es null en players-ready.");
                    }

                    // Guardar matchId si no estaba asignado
                    if (string.IsNullOrEmpty(pendingMatchId))
                        pendingMatchId = matchId;
                }


                else if (serverMessage.eventName == "ping-match")
                {

                    if (chatText != null)
                        chatText.text += "\n Ping recibido correctamente. Esperando al otro jugador...";
                }
                else if (serverMessage.eventName == "match-start")
                {
                    if (chatText != null)
                        chatText.text += "\n ¡La partida ha comenzado!";
                    ActualizarChatUI();

                    // Ocultar botón de ping al iniciar partida
                    if (pingButton != null)
                        pingButton.gameObject.SetActive(false);

                    Debug.Log("Recibido evento match-start. Cargando escena GameplayScene...");
                    yaJugamosUnaPartida = true;

                    StartCoroutine(CargarGameplayDesdeChat());
                }

                else if (serverMessage.eventName == "receive-game-data")
                {
                    int debuffCode = Random.Range(1, 4); // 1 a 3

                    string debuff = "";
                    if (debuffCode == 1) debuff = "slow";
                    else if (debuffCode == 2) debuff = "auto-damage";
                    else if (debuffCode == 3) debuff = "downgrade-skill";

                    Debug.Log("Haz recibido un debuff");
                    AplicarDebuff(debuff);
                    ActualizarChatUI();

                }
                else if (serverMessage.eventName == "game-ended")
                {
                    Debug.Log("Partida finalizada, has perdido.");
                    StartCoroutine(EsperarYMostrarDerrota());

                }
                else if (serverMessage.eventName == "rematch-request")
                {
                    
                }
                else if (serverMessage.eventName == "close-match")
                {
                    Debug.Log("🚪 El otro jugador se ha salido. Cerrando la partida...");

                    // Mensaje opcional para el chat
                    if (chatText != null)
                        chatText.text += "\n El otro jugador abandonó la partida.";

                    ActualizarChatUI();

                    // Cargar escena principal directamente
                    SceneManager.LoadScene("MainMenu");
                }









                ActualizarChatUI();
                ActualizarListaUsuariosUI();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al procesar mensaje del servidor: " + ex.Message);
            }
        });
    }


    public void login()
    {
        var loginMessage = new OutgoingLoginMessage
        {
            @event = "login",
            data = new LoginData { gameKey = "WNZINM" }
        };

        ws.Send(JsonUtility.ToJson(loginMessage));
    }

    public void PedirJugadoresConectados()
    {
        ClientRequest request = new ClientRequest { @event = "online-players" };
        ws.Send(JsonUtility.ToJson(request));
    }

    public void SendChatMessage(string msg)
    {
        if (!string.IsNullOrEmpty(msg) && ws.ReadyState == WebSocketState.Open)
        {
            var outgoing = new OutgoingMessage
            {
                @event = "send-public-message",
                data = new OutgoingData { message = msg }
            };

            ws.Send(JsonUtility.ToJson(outgoing));

            if (chatText != null)
                chatText.text += $"\n Tú: {msg}";

            ActualizarChatUI();
            messageInput.text = "";
        }
    }

    public void EnviarSolicitudPartida(string targetPlayerId, string playerName)
    {
        MatchRequest request = new MatchRequest
        {
            @event = "send-match-request",
            data = new MatchRequestData { playerId = targetPlayerId }
        };

        ws.Send(JsonUtility.ToJson(request));

        if (chatText != null)
            chatText.text += $"\n Invitación enviada a {playerName}";

        ActualizarChatUI();
    }

    private void OnSubmitMessage(string msg)
    {
        if (enviarPrivado)
        {
            EnviarMensajePrivado(msg);
            enviarPrivado = false;
        }
        else
        {
            SendChatMessage(msg);
        }
    }

    public void EnviarMensajePrivado(string mensaje)
    {
        if (string.IsNullOrEmpty(mensaje) || string.IsNullOrEmpty(selectedPlayerId))
        {
            Debug.LogWarning("No se puede enviar mensaje privado vacío o sin destinatario.");
            return;
        }

        var privateMsg = new PrivateMessageRequest
        {
            data = new PrivateMessageData
            {
                playerId = selectedPlayerId,
                message = mensaje
            }
        };

        ws.Send(JsonUtility.ToJson(privateMsg));

        if (chatText != null)
            chatText.text += $"\n Tú a {selectedPlayerName}: {mensaje}";

        ActualizarChatUI();
        messageInput.text = "";
    }

    public void AceptarPartida()
    {
        var message = new SimpleEvent { @event = "accept-match" };
        ws.Send(JsonUtility.ToJson(message));

        if (chatText != null)
            chatText.text += "\n Has aceptado la partida.";

        ActualizarChatUI();
    }

    public void RechazarPartida()
    {
        var message = new SimpleEvent { @event = "reject-match" };
        ws.Send(JsonUtility.ToJson(message));

        if (chatText != null)
            chatText.text += "\n Has rechazado la partida.";

        ActualizarChatUI();
    }

    public void ConectarAPartida()
    {
        if (string.IsNullOrEmpty(pendingMatchId))
        {
            Debug.LogError("No hay matchId pendiente.");
            return;
        }

        var message = new ConnectMatchMessage
        {
            data = new ConnectMatchData { matchId = pendingMatchId }
        };

        string json = JsonUtility.ToJson(message);
        Debug.Log("Enviando connect-match: " + json);
        ws.Send(json);

        if (chatText != null)
            chatText.text += "\n Te estás conectando a la partida...";

        ActualizarChatUI();

        roomButton.gameObject.SetActive(false);
    }

    public void EnviarPingMatch()
    {
        var message = new SimpleEvent { @event = "ping-match" };
        string json = JsonUtility.ToJson(message);
        Debug.Log("Enviando ping-match: " + json);
        ws.Send(json);

        if (chatText != null)
            chatText.text += "\n Enviando ping para sincronizar...";

        ActualizarChatUI();
    }

    public void EnviarSenalDebuff()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            string json = "{\"event\":\"send-game-data\"}";
            Debug.Log("Enviando señal de debuff al otro jugador...");
            ws.Send(json);
        }
        else
        {
            Debug.LogWarning("No se pudo enviar la señal: WebSocket no está conectado.");
        }
    }



    private string ExtraerNombreDesdeMensaje(string msg)
    {
        int start = msg.IndexOf('\'') + 1;
        int end = msg.IndexOf('\'', start);
        return msg.Substring(start, end - start);
    }


    private void ActualizarListaUsuariosUI()
    {
        if (SceneManager.GetActiveScene().name == "GameplayScene")
            return;

        foreach (Transform child in UserListContainer)
            Destroy(child.gameObject);

        foreach (OnlinePlayer player in connectedPlayers)
        {
            if (player.id == myId) continue;

            GameObject entry = Instantiate(UserEntryPrefab, UserListContainer);
            TextMeshProUGUI text = entry.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{player.name} - {player.status}";

            Transform background = entry.transform.Find("Background");
            if (background == null)
            {
                Debug.LogError("El prefab UserEntry no tiene un hijo llamado 'Background'.");
                continue;
            }

            EventTrigger trigger = background.GetComponent<EventTrigger>();
            if (trigger == null) trigger = background.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            var entryClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            string playerId = player.id;
            string playerName = player.name;

            entryClick.callback.AddListener((eventData) =>
            {
                PointerEventData pointer = (PointerEventData)eventData;
                if (pointer.button == PointerEventData.InputButton.Right)
                {
                    selectedPlayerId = playerId;
                    selectedPlayerName = playerName;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        (RectTransform)contextMenuPanel.transform.parent,
                        Input.mousePosition,
                        null,
                        out Vector2 localPoint);

                    contextMenuPanel.GetComponent<RectTransform>().anchoredPosition = localPoint;
                    contextMenuPanel.SetActive(true);
                }
            });

            trigger.triggers.Add(entryClick);
        }
    }


    private IEnumerator CargarGameplayDesdeChat()
    {
        yield return new WaitForSeconds(1f);


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Prefab/scenes/GameplayScene", LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void AplicarDebuff(string tipo)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No se puede aplicar debuff: jugador no encontrado.");
            return;
        }

        if (tipo == "slow" && slowDebuffSO != null)
        {
            slowDebuffSO.Apply(player);
            Debug.Log("Debuff enviado");
        }
        else if (tipo == "auto-damage" && autoDamageDebuffSO != null)
        {
            autoDamageDebuffSO.Apply(player);
            Debug.Log("Debuff enviado");
        }
        else if (tipo == "downgrade-skill" && downgradeSkillDebuffSO != null)
        {
            downgradeSkillDebuffSO.Apply(player);
            Debug.Log("Debuff enviado");
        }
        else
        {
            Debug.LogWarning("Debuff no reconocido o ScriptableObject no asignado: " + tipo);
        }
    }

    public void FinalizarPartida()
    {
        var message = new SimpleEvent { @event = "finish-game" };
        string json = JsonUtility.ToJson(message);
        Debug.Log("Enviando evento finish-game...");
        ws.Send(json);
    }

    private IEnumerator EsperarYMostrarDerrota()
    {
        yield return new WaitForSeconds(0.5f); // permitir que todo cargue

        Debug.Log(" Buscando PlayerLose en objetos desactivados...");

        // ✅ Busca el componente PlayerLose en toda la escena, incluso si está desactivado
        PlayerLose playerLose = null;

        foreach (PlayerLose pl in Resources.FindObjectsOfTypeAll<PlayerLose>())
        {
            if (pl.gameObject.scene.name != null && pl.gameObject.name == "GameOverPanel")
            {
                playerLose = pl;
                break;
            }
        }

        if (playerLose != null)
        {
            Debug.Log(" PlayerLose encontrado en GameOverPanel, mostrando derrota.");
            playerLose.Lose();
        }
        else
        {
            Debug.LogWarning(" No se encontró el PlayerLose en GameOverPanel.");
        }
    }

    public void EnviarSolicitudRematch()
    {
        var message = new SimpleEvent { @event = "send-rematch-request" };
        ws.Send(JsonUtility.ToJson(message));
        Debug.Log("🔁 Evento send-rematch-request enviado.");

        if (chatText != null)
            chatText.text += "\n Has solicitado una revancha. Esperando al otro jugador...";
    }



    public void SendRawMessage(string json)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(json);
        }
        else
        {
            Debug.LogWarning("No se pudo enviar mensaje: WebSocket no está conectado.");
        }
    }










    private void ActualizarChatUI()
    {
        if (chatText == null || chatScrollRect == null) return;

        chatText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatScrollRect.content);
        chatScrollRect.verticalNormalizedPosition = 0f;
    }



    void OnApplicationQuit()

    {
        Disconnect();
    }


    public void Disconnect()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
            Debug.Log("Conexión WebSocket cerrada al salir del juego.");
        }
    }
}
