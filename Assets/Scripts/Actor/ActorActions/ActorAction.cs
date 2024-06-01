using GBGame.Data;
using GBGame.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBGame.Actors
{
    [System.Serializable]
    public abstract class ActorAction : IJsonObject
    {
        public const string LIB_FOLDER_NAME = "ActorActions";
        public const string LIB_FILE_NAME = "ActionLibrary";

        string IJsonObject.JsonID => ActionID;
        public string ActionID;

        protected Actor Caller { get; private set; }
        protected CombatManager Com => Caller.Com;
        protected GridManager Grid => Caller.Com.Grid;

        [Header("Visual Info")]
        public ActionInfoData Info;

        public void Setup(Actor actor)
        {
            Caller = actor;
            OnSetup();
        }

        public void SetDown(Actor actor)
        {
            CancelAction();
            OnSetdown();
        }

        protected virtual void OnSetup() { }
        protected virtual void OnSetdown() { }

        #region Action Registration
        /// <summary>
        /// Executes this action
        /// </summary>
        /// <param name="caller"></param>
        public void Execute() => Execute(out _);
        /// <summary>
        /// Executes this action, returning how long the action should take to resolve in full
        /// </summary>
        public abstract void Execute(out uint ticksToResolve);


        private HashSet<TickManager.TickCancelToken> CancelTokens = new HashSet<TickManager.TickCancelToken>();

        /// <summary>
        /// Generates a schedued action to be registered to the tick manager
        /// </summary>
        /// <returns></returns>
        private ScheduledAction GenAction(Action action, string msg = default)
        {
            return new ScheduledAction(
                action,
                $"[{Caller.ActorId}] Performing [{this.ActionID}] {msg}"
                );
        }

        /// <summary>
        /// Registers this action to the occur on the next tick
        /// </summary>
        /// <param name="msg">Additional information to be tacked on to the message log</param>
        protected void AddNext(Action action, string msg = default)
        {
            Com.TickManager.RegisterToNextTick(GenAction(action, msg), out TickManager.TickCancelToken token);
            CancelTokens.Add(token);
        }

        /// <summary>
        /// Registers this action to occur X ticks form now
        /// </summary>
        /// <param name="ticks">Ticks from now that this action will execute</param>
        /// <param name="msg">Additional information to be tacked on to the message log</param>
        protected void AddFuture(in uint ticks, Action action, string msg = default)
        {
            Com.TickManager.RegisterToFutureTick(ticks, GenAction(action, msg), out TickManager.TickCancelToken token);
            CancelTokens.Add(token);
        }

        /// <summary>
        /// Cancels all functions of this action
        /// </summary>
        public void CancelAction()
        {
            foreach (var item in CancelTokens) item.ToCancel.Invoke();
            CancelTokens.Clear();
            ActionCancelled();
        }

        /// <summary>
        /// Any extra logic to occur when this action is canceled
        /// </summary>
        protected virtual void ActionCancelled() { }
        #endregion
    }
}