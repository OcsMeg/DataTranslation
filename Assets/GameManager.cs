using Meta.XR.Movement.Retargeting;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject PhotonObject;
    public GameObject PhotonFailureObject;
    private GameObject camera;
    private GameObject player;
    private Vector3 FirstCameraPosition;
    public Vector3 spawnPoint;
    [SerializeField] private GameObject room;
    [SerializeField] private GameObject passThrough;

    // ★ ルームのカスタムプロパティで使うキー
    private const string ROOM_USER_LIST_KEY = "UserList";

    void Start()
    {
        // ★ ここでユーザ名を Photon に渡しておく
        //    自分のシステム側のユーザ名を使ってください
        //    例：UserNameManager.CurrentUserName など
        PhotonNetwork.NickName = PlayerMode.GetPlayerName();
        // ↑ 実際は上の行を自分のユーザ名変数に書き換えてください

        PhotonNetwork.ConnectUsingSettings();

        // spawnPoint登録
        spawnPoint = GameObject.FindGameObjectWithTag("spawnPoint").transform.position;
    }
    
    void Update()
    {
        // エディタ実行時に L キーを押したら、現在の参加者リストをログに出す
        if (Input.GetKeyDown(KeyCode.L))
        {
            var userList = GetRoomUserNameList();

            Debug.Log("===== [Debug] 現在のユーザ名リスト =====");

            if (userList.Count == 0)
            {
                Debug.Log("ユーザリストが空です（まだ誰も入室していないか、CustomProperties 未設定）");
            }
            else
            {
                foreach (var name in userList)
                {
                    Debug.Log($"ユーザ: {name}");
                }
            }

            // おまけ：Photon が持っている生の PlayerList も確認したい場合
            Debug.Log("===== [Debug] PhotonNetwork.PlayerList =====");
            foreach (var p in PhotonNetwork.PlayerList)
            {
                Debug.Log($"ActorNumber: {p.ActorNumber}, NickName: {p.NickName}");
            }
        }
    }


    // ルームに参加する処理
    public override void OnConnectedToMaster()
    {
        // 固定ルーム "SampleRoomName" に参加
        PhotonNetwork.JoinRoom("SampleRoomName");
    }

    // ルーム参加に失敗した場合(通常，指定したルーム名が存在しなかった場合)の処理
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // ルーム参加に失敗した場合はルームを新規作成（最大8人まで）
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom("SampleRoomName", roomOptions);
    }

    // ルーム参加に成功した時の処理
    public override void OnJoinedRoom()
    {
        // ★ まず自分の名前をルームのユーザリストに追加
        AddUserNameToRoomList(PhotonNetwork.NickName);

        if (PhotonObject == null)
        {
            Debug.LogError("PhotonObject is not set in the inspector.");
            return;
        }

        // VR・MR・神視点別にプレイヤー生成
        if (PlayerMode.GetSelectedPlayMode() == PlayerMode.PlayMode.MR)
        {
            camera = Instantiate(Resources.Load<GameObject>("Camera/MROrigin"), spawnPoint, Quaternion.identity);
            player = PhotonNetwork.Instantiate("MRPlayerPsys", spawnPoint, Quaternion.identity);
            room.SetActive(false);
        }
        else if (PlayerMode.GetSelectedPlayMode() == PlayerMode.PlayMode.VR)
        {
            camera = Instantiate(Resources.Load<GameObject>("Camera/VROrigin"), spawnPoint, Quaternion.identity);
            player = PhotonNetwork.Instantiate("VRPlayerPsys", spawnPoint, Quaternion.identity);
            passThrough.SetActive(false);
        }
        else if (PlayerMode.GetSelectedPlayMode() == PlayerMode.PlayMode.GOD)
        {

        }

        if (PlayerMode.GetSelectedPlayMode() != PlayerMode.PlayMode.GOD)
        {
            CreatePhotonAvatar createPhotonAvatar = player.GetComponent<CreatePhotonAvatar>();
            createPhotonAvatar.ExecuteCreatePhotonAvatar();
        }
    }

    // ★ 他のプレイヤーがルームから抜けた時にユーザリストから削除
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        RemoveUserNameFromRoomList(otherPlayer.NickName);
    }

    // OnDisconnectedという名前だがルーム切断時のみではなく接続失敗時にも実行する処理
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogError("Disconnected from Photon: " + cause.ToString());

        // 接続に失敗した場合、PhotonFailureObject(本記事ではCube) を表示する
        if (PhotonFailureObject != null)
        {
            // ローカルにオブジェクトを Instantiate する例（PhotonNetwork.Instantiate は使用できないため）
            Instantiate(PhotonFailureObject, new Vector3(0f, 3f, 0f), Quaternion.identity);
        }
        else
        {
            Debug.LogError("PhotonFailureObject is not set in the inspector.");
        }
    }

    /// <summary>
    /// ルームの CustomProperties に自分のユーザ名を追加
    /// </summary>
    private void AddUserNameToRoomList(string userName)
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        // 既存の UserList を取得
        var room = PhotonNetwork.CurrentRoom;

        System.Collections.Generic.List<string> userList = new System.Collections.Generic.List<string>();

        if (room.CustomProperties != null &&
            room.CustomProperties.ContainsKey(ROOM_USER_LIST_KEY))
        {
            string[] current = room.CustomProperties[ROOM_USER_LIST_KEY] as string[];
            if (current != null)
            {
                userList.AddRange(current);
            }
        }

        // 重複チェック
        if (!userList.Contains(userName))
        {
            userList.Add(userName);
        }

        // CustomProperties を更新
        Hashtable hash = new Hashtable();
        hash[ROOM_USER_LIST_KEY] = userList.ToArray();
        room.SetCustomProperties(hash);
    }

    /// <summary>
    /// ルームの CustomProperties から指定ユーザ名を削除
    /// </summary>
    private void RemoveUserNameFromRoomList(string userName)
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        var room = PhotonNetwork.CurrentRoom;
        if (room.CustomProperties == null ||
            !room.CustomProperties.ContainsKey(ROOM_USER_LIST_KEY))
        {
            return;
        }

        string[] current = room.CustomProperties[ROOM_USER_LIST_KEY] as string[];
        if (current == null) return;

        var userList = new System.Collections.Generic.List<string>(current);

        if (userList.Contains(userName))
        {
            userList.Remove(userName);

            Hashtable hash = new Hashtable();
            hash[ROOM_USER_LIST_KEY] = userList.ToArray();
            room.SetCustomProperties(hash);
        }
    }

    /// <summary>
    /// いつでも呼び出して、最新のユーザ名リストを取れるようにしておく
    /// </summary>
    public static System.Collections.Generic.List<string> GetRoomUserNameList()
    {
        if (PhotonNetwork.CurrentRoom == null ||
            PhotonNetwork.CurrentRoom.CustomProperties == null ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(ROOM_USER_LIST_KEY))
        {
            return new System.Collections.Generic.List<string>();
        }

        string[] current = PhotonNetwork.CurrentRoom.CustomProperties[ROOM_USER_LIST_KEY] as string[];
        if (current == null) return new System.Collections.Generic.List<string>();

        return new System.Collections.Generic.List<string>(current);
    }

    /// <summary>
    /// ルームの CustomProperties が更新されたときに呼ばれる
    /// → UI のユーザリストを更新したいときに使える
    /// </summary>
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.ContainsKey(ROOM_USER_LIST_KEY))
        {
            var list = GetRoomUserNameList();
            Debug.Log("=== Room User List Updated ===");
            foreach (var name in list)
            {
                Debug.Log($"User: {name}");
            }
        }
    }
}
