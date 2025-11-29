using Meta.XR.Movement.Retargeting;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

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
    

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        //spawnPoint登録
        spawnPoint = GameObject.FindGameObjectWithTag("spawnPoint").transform.position;
        
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
        if (PhotonObject == null)
        {
            Debug.LogError("PhotonObject is not set in the inspector.");
            return;
        }
        
        //VR・MR・神視点別にプレイヤー生成
        if (PlayerMode.GetSelectedPlayMode() == PlayerMode.PlayMode.MR)
        {
            camera = Instantiate(Resources.Load<GameObject>("Camera/MROrigin"),spawnPoint, Quaternion.identity);
            player = PhotonNetwork.Instantiate("MRPlayerPsys", spawnPoint, Quaternion.identity);
            room.SetActive(false);
        } else if (PlayerMode.GetSelectedPlayMode() == PlayerMode.PlayMode.VR)
        {
            camera = Instantiate(Resources.Load<GameObject>("Camera/VROrigin"),spawnPoint, Quaternion.identity);
            player = PhotonNetwork.Instantiate("VRPlayerPsys", spawnPoint, Quaternion.identity);
            
            // MetaXRMovementSDK用の文
            // if (player != null)
            // {
            //     CharacterRetargeter retargeter = player.GetComponent<CharacterRetargeter>();
            //     if (retargeter != null)
            //     {
            //         retargeter.enabled = true;
            //         Debug.Log("PlayerのCharacterRetargeterを有効化しました");
            //     }
            //     else
            //     {
            //         Debug.LogWarning("PlayerにCharacterRetargeterが見つかりません");
            //     }
            // }
            // else
            // {
            //     Debug.LogWarning("player が null です");
            // }
            
            passThrough.SetActive(false);
        } else if (PlayerMode.GetSelectedPlayMode() == PlayerMode.PlayMode.GOD)
        {
            
        }

        if (PlayerMode.GetSelectedPlayMode() != PlayerMode.PlayMode.GOD)
        {
            CreatePhotonAvatar createPhotonAvatar = player.GetComponent<CreatePhotonAvatar>();
            createPhotonAvatar.ExecuteCreatePhotonAvatar();
        }
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
}

