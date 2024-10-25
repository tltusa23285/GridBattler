using UnityEngine;
using GBGame.Actors;
using GBGame.Actors.ActorTypes;
using GBGame.Utilities;

namespace GBGame
{
    using WAction = IJsonObjectWrapper<ActorAction>;
    using WEnemy = IJsonObjectWrapper<EnemyActorController>;

    public class CombatManager : MonoBehaviour
    {
        public const uint TICKS_PER_SECOND = 60;
        [field: SerializeField] public CameraController CameraController { get; private set; }
        public GridManager Grid { get; private set; }
        public Actor PlayerActor { get; private set; }
        public TickManager TickManager { get; private set; }
        public JsonObjectLibrary<WAction> ActionLibrary { get; private set; }
        public JsonObjectLibrary<WEnemy> EnemyLibrary { get; private set; }

        private CombatPlayer LocalPlayer;

        private void Awake()
        {
            ActionLibrary = JsonObjectLibrary<WAction>.LoadLibrary(ActorAction.LIB_FOLDER_NAME, ActorAction.LIB_FILE_NAME);
            EnemyLibrary = JsonObjectLibrary<WEnemy>.LoadLibrary(EnemyActorController.LIB_FOLDER_NAME, EnemyActorController.LIB_FILE_NAME);
            Grid = new GridManager(this, 6, 3);
            TickManager = new TickManager(TICKS_PER_SECOND);
        }

        private void Start()
        {
            Actor.Instantiate(this, "PlayerPrefab", 1, 1, out Actor p_Actor);
            PlayerActor = p_Actor;
            LocalPlayer = new CombatPlayer((PlayerActor)PlayerActor);

            Actor.Instantiate(this, "EnemyPrefab", 4, 2, out _);
        }

        private void Update()
        {
            LocalPlayer.HandleInput();
            if (DebugMode) DebugUpdate();
            else GameUpdate();
        }

        private float TickTimer = 0;
        private void GameUpdate()
        {
            TickTimer += Time.deltaTime;
            while (TickTimer >= TickManager.TickRate)
            {
                TickTimer -= TickManager.TickRate;
                TickManager.ProgressTick();
            }
        }

        private float DebugTimer = 0;
        private bool SlowPlaying = false;
        private void DebugUpdate()
        {
            if (SlowPlaying)
            {
                DebugTimer += Time.deltaTime;
                while (DebugTimer >= SlowPlayRate)
                {
                    DebugTimer -= SlowPlayRate;
                    TickManager.ProgressTick();
                    TickManager.PrintCurrentTickEvents();
                }
            }
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                if (Input.GetKey(KeyCode.LeftControl)) SlowPlaying = !SlowPlaying;
                else if (Input.GetKey(KeyCode.LeftShift)) TickManager.ProgressToNextTickEvent();
                else TickManager.ProgressTick();
                TickManager.PrintCurrentTickEvents();
            }
        }

        #region Debug
        [Header("Debug")]
        public bool DebugMode = false;
        public uint SlowPlayTPS = 3;
        private float SlowPlayRate => 1.0f / (float)SlowPlayTPS;
        private void OnGUI()
        {
            if (!DebugMode) return;
            TickManager.DrawGui(new Rect(0, 0, 100, 100));
        }
        #endregion
    }

}