using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using TMPro;
using System.Collections.Generic;

//Clases y varialbes para los mensajes que entran 
[System.Serializable]
public class ServerMessage
{
    public string eventName;
    public ServerData data;
}

[System.Serializable]
public class ServerData
{
    public string msg;
    public string id;
}

//Clases y mensajes para los mensajes que salen
[System.Serializable]
public class OutgoingMessage
{
    public string @event;
    public OutgoingData data;
}

[System.Serializable]
public class OutgoingData
{
    public string message;
}

[System.Serializable]
public class ClientRequest
{
    public string @event;
}


[System.Serializable]
public class PlayerListMessageWrapper
{
    public string eventName;
    public string[] data;
}







public class ChatManager : MonoBehaviour
{
    //definir mas variables
    public TextMeshProUGUI UserListText;
    public TextMeshProUGUI chatText;//texto de la UI
    public TMP_InputField messageInput;//resivir texto en la UI
    public ScrollRect chatScrollRect;//el cuadro con el scroll para bajar y ver los mensajes
    private List<string> connectedPlayers = new List<string>();

    private WebSocket ws;//instanciamos igual websocket para hacer la conexion websocket con el server
    private string myId = "";//para guardar el id nuestro 

    void Start()
    {
        if (FindFirstObjectByType<UnityMainThreadDispatcher>() == null)//esto crea el objeto de unityMainThread para agregar tareas al hilo principal del unity
        {
            gameObject.AddComponent<UnityMainThreadDispatcher>();
        }

        chatText.text += "\n[Conectando al servidor]";//mandamos este mensaje al chatText que es un objeto de la UI 

        ws = new WebSocket("ws://ucn-game-server.martux.cl:4010");//declaramos el websocket y le damos la direccion del server

        ws.OnOpen += OnWebSocketOpen;//crea el evento onOpen y le asigna la funcion onOpen... que se ejecuta cuando ocurre el evento OnOpen (osea cuando se hizo conexion con el server)
        ws.OnMessage += OnWebSocketMessage;//lo mismo aqui 

        ws.Connect();//hace la conexion con el server

        messageInput.onSubmit.AddListener(SendChatMessage);//hace al presionar enter envie el mensaje
    }

    private void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            chatText.text += "\n[Conectado al servidor]";
            Debug.Log("🔁 Pidiendo jugadores conectados...");
            PedirJugadoresConectados();
        });
    }


    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            Debug.Log("Mensaje del servidor: " + e.Data);

            try
            {
                // Reemplazamos "event" por "eventName" para evitar conflictos con C#
                string json = e.Data.Replace("\"event\":", "\"eventName\":");

                // DEBUG: Mostrar JSON recibido
                Debug.Log("JSON recibido ajustado: " + json);

                if (json.Contains("get-connected-players"))//esto se debe hacer asi por que el json viene con un array y no podemos usarlo como serverMessage
                {
                    PlayerListMessageWrapper playerList = JsonUtility.FromJson<PlayerListMessageWrapper>(json);


                    if (playerList.data != null && playerList.data.Length > 0)

                    {

                        connectedPlayers.AddRange(playerList.data); // Guardar los jugadores conectados
                        if (myId != "")
                        {
                            ActualizarListaUsuariosUI();
                        }

                    }
                    return;
                }

                // Deserializar mensaje general
                ServerMessage serverMessage = JsonUtility.FromJson<ServerMessage>(json);

                if (serverMessage.eventName == "connected-to-server")
                {
                    chatText.text += $"\n[Servidor]: {serverMessage.data.msg}";
                    myId = serverMessage.data.id;


                    ActualizarChatUI();
                }
                
                

                
                else if (serverMessage.eventName == "player-connected")
                {
                    if (serverMessage.data.id != myId)
                    {
                        connectedPlayers.Add(serverMessage.data.id); // Agregar nuevo jugador
                        chatText.text += $"\n[+] {serverMessage.data.msg}";
                        ActualizarListaUsuariosUI();
                        ActualizarChatUI();
                    }
                }
                else if (serverMessage.eventName == "player-disconnected")
                {
                    if (serverMessage.data.id != myId)
                    {
                        connectedPlayers.Remove(serverMessage.data.id); // Eliminar jugador
                        chatText.text += $"\n[-] {serverMessage.data.msg}";
                        ActualizarListaUsuariosUI();
                        ActualizarChatUI();

                    }
                }
                else if (serverMessage.eventName == "public-message")
                {
                    if (serverMessage.data.id != myId)
                    {
                        chatText.text += $"\n{serverMessage.data.id}: {serverMessage.data.msg}";
                        ActualizarChatUI();
                    }
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al procesar mensaje del servidor: " + ex.Message);
            }

        });
    }


    private void ActualizarChatUI()
    {
        chatText.ForceMeshUpdate();//puede que no sea necesario todo esto en vola tiraba error por que la otra instacia era del mismo pc
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatScrollRect.content);
        chatScrollRect.verticalNormalizedPosition = 0f;
    }

    private void ActualizarListaUsuariosUI()
    {
        UserListText.text = "";//limpia lo anterior

        UserListText.text += $"\n Tú : {myId}";
        foreach (string id in connectedPlayers)
        {
            if (id != myId) // Evitar mostrar el propio ID dos veces
            {
                UserListText.text += $"\n Usuario: {id}";
            }
        }
    }


    public void PedirJugadoresConectados()
    {
        ClientRequest request = new ClientRequest
        {
            @event = "get-connected-players",
        };

        string json = JsonUtility.ToJson(request);
        Debug.Log("📤 Enviado al servidor (get-connected-players): " + json);
        ws.Send(json);

    }



    public void SendChatMessage(string msg)
    {
        if (!string.IsNullOrEmpty(msg) && ws.ReadyState == WebSocketState.Open)//primero revisamos que sea distinto de vacio y que aun haya conexion con el server
        {
            OutgoingMessage outgoing = new OutgoingMessage//aqui estamos creando el mensaje que saldra hacia el servidor (un json)
            {
                @event = "send-public-message",//definimos el tipo de evento 
                data = new OutgoingData { message = msg }//y el mensaje
            };

            string json = JsonUtility.ToJson(outgoing);//aqui se transfroma a json el outgoing que es la clase de los mensajes que enviamos
                                                       // no es necesario pasar el id por que el websocket ya lo tiene con la primera conexion
                                                       //por eso solo mandamos data, y en la clase de data en este caso solo tiene mssg y no id y mssg como la clase de los mensajes que entran
            
            Debug.Log("Enviado al servidor: " + json);//print
            ws.Send(json);//hacemos la peticion al server para enviar un mensaje json 

            chatText.text += $"\nTú: {msg}";//mostramos el msg desde la clase ServerData
            Canvas.ForceUpdateCanvases();//hacemos el update en la UI de los cambios
            chatScrollRect.verticalNormalizedPosition = 0f;//bajamos el scroll

            messageInput.text = "";//texto vacio en el input donde escribimos
        }
    }

    void OnApplicationQuit()

    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
            Debug.Log("Conexión WebSocket cerrada al salir del juego.");
        }
    }
}
