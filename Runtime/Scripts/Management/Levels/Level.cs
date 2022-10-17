using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using H2DT.Management.Player;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Levels
{
    public abstract class Level : HandyComponent
    {
        #region Inspector

        [Header("Handling")]
        [Space]
        [SerializeField]
        private LevelHandler _levelHandler;

        [SerializeField]
        private LevelInfo _levelInfo;

        [Header("Initialization")]
        [Space]
        [SerializeField]
        private bool _shouldInitializeSelf;

        [SerializeField]
        private List<GameObject> _initializationSubjectObjects = new List<GameObject>();

        [SerializeField]
        private List<GameObject> _leaveSubjectObjects = new List<GameObject>();

        [Header("Anchoring")]
        [Space]
        [SerializeField]
        private List<SubLevelAnchor> _sublevelAnchors = new List<SubLevelAnchor>();

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent<Level> LevelInitializedEvent;

        [SerializeField]
        public UnityEvent<Level> LevelLeftEvent;

        #endregion

        #region Fields 

        private List<ILevelInitializationSubject> _initializationSubjects = new List<ILevelInitializationSubject>();
        private List<ILevelLeaveSubject> _leaveSubjects = new List<ILevelLeaveSubject>();

        // Actions
        private UnityAction BeforeInitializationAction;
        private UnityAction AfterInitializationAction;

        private UnityAction BeforeLeaveAction;
        private UnityAction AfterLeaveAction;

        #endregion

        #region Properties

        protected LevelHandler levelHandler => _levelHandler;
        protected List<SubLevelAnchor> sublevelAnchors => _sublevelAnchors;

        #endregion

        #region Getters

        public LevelInfo levelInfo => _levelInfo;
        public bool shouldSelfInitialize => _shouldInitializeSelf;

        #endregion

        #region Mono

        protected virtual async void Awake()
        {
            if (_shouldInitializeSelf)
                await Initialize();
        }

        #endregion

        #region Actions

        /// <summary>
        /// Loading actions for children.
        /// </summary>
        private void LoadActions()
        {
            System.Type stateType = this.GetType();
            MethodInfo mi;

            mi = stateType.GetMethod("BeforeInitialization", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                BeforeInitializationAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("AfterInitialization", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                AfterInitializationAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("BeforeLeave", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                BeforeLeaveAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("AfterLeave", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                AfterLeaveAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;
        }

        #endregion

        #region Logic

        public virtual async Task Initialize()
        {
            LoadActions();
            LoadInitializationSubjects(); // Loads all Initialization subjects components from objects list.
            LoadLeaveSubjects();

            BeforeInitializationAction?.Invoke();

            await PerformInitializationTasks();// Performs all Initialization tasks

            AfterInitializationAction?.Invoke();

            LevelInitializedEvent.Invoke(this);
        }

        public virtual async Task Leave(object trail = null)
        {
            BeforeLeaveAction?.Invoke();

            await PerformLeaveTasks();
            await _levelHandler.Leave(trail);

            AfterLeaveAction?.Invoke();

            LevelLeftEvent.Invoke(this);
        }

        public virtual async Task Leave(LevelInfo levelInfo, object trail = null)
        {
            if (levelInfo == null)
            {
                await Leave(trail);
                return;
            }

            BeforeLeaveAction?.Invoke();

            await PerformLeaveTasks();
            await _levelHandler.Leave(levelInfo, trail);

            AfterLeaveAction?.Invoke();

            LevelLeftEvent.Invoke(this);
        }

        #endregion

        #region Initialization Subjects

        private void LoadInitializationSubjects()
        {
            _initializationSubjectObjects.ForEach(loadSubjectObject =>
            {
                ILevelInitializationSubject initializationSubject = loadSubjectObject.GetComponent<ILevelInitializationSubject>();
                _initializationSubjects.Add(initializationSubject);
            });
        }

        private async Task PerformInitializationTasks()
        {
            List<Task> loadLevelTasks = new List<Task>();

            foreach (ILevelInitializationSubject levelLoadSubject in _initializationSubjects)
            {
                loadLevelTasks.Add(levelLoadSubject.LevelInitializationTask());
            }

            await Task.WhenAll(loadLevelTasks);
        }

        #endregion

        #region Leave Subjects

        private void LoadLeaveSubjects()
        {
            _leaveSubjectObjects.ForEach(leaveSubjectObject =>
            {
                ILevelLeaveSubject initializationSubject = leaveSubjectObject.GetComponent<ILevelLeaveSubject>();
                _leaveSubjects.Add(initializationSubject);
            });
        }

        private async Task PerformLeaveTasks()
        {
            List<Task> leaveLevelTasks = new List<Task>();

            foreach (ILevelLeaveSubject levelLeaveSubject in _leaveSubjects)
            {
                leaveLevelTasks.Add(levelLeaveSubject.LevelLeaveTask());
            }

            await Task.WhenAll(leaveLevelTasks);
        }

        #endregion
    }
}
