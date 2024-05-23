using UnityEngine;
using UnityEngine.AddressableAssets;
using WAction = IJsonObjectWrapper<ActorAction>;
using WEnemy = IJsonObjectWrapper<EnemyActorController>;

public class CombatManager : MonoBehaviour
{
    [field:SerializeField] public CameraController CameraController { get; private set; }
    public GridManager Grid {  get; private set; }
    public PlayerActor Player { get; private set; }
    public TickManager TickManager { get; private set; }
    public JsonObjectLibrary<WAction> ActionLibrary { get; private set; }
    public JsonObjectLibrary<WEnemy> EnemyLibrary { get; private set; }

    private void Awake()
    {
        ActionLibrary = JsonObjectLibrary<WAction>.LoadLibrary(ActorAction.LIB_FOLDER_NAME, ActorAction.LIB_FILE_NAME);
        EnemyLibrary = JsonObjectLibrary<WEnemy>.LoadLibrary(EnemyActorController.LIB_FOLDER_NAME, EnemyActorController.LIB_FILE_NAME);
        Grid = new GridManager(this, 6, 3);
        TickManager = new TickManager(60);
    }

    private void Start()
    {
        GameObject player = Addressables.LoadAssetAsync<GameObject>("PlayerPrefab").WaitForCompletion();

        GameObject go = Instantiate(player);
        Player = go.GetComponent<PlayerActor>();
        Player.Spawn(this, 1, 1);

        if (EnemyLibrary.GetItem("BasicArcher", out WEnemy result))
        {
            EnemyActorController con = result.Object;
            GameObject enemy = Addressables.LoadAssetAsync<GameObject>(con.Prefab).WaitForCompletion(); 
            
            go = Instantiate(enemy);
            go.GetComponent<Actor>().Spawn(this, 4, 2);
            go.GetComponent<EnemyActor>().SetController(con);
        }
    }

    private float TickTimer = 0;
    private void Update()
    {
        TickTimer += Time.deltaTime;
        while (TickTimer >= TickManager.TickRate)
        {
            TickTimer -= TickManager.TickRate;
            TickManager.ProgressTick();
        }
    }
}
