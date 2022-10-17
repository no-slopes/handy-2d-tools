using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using H2DT.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Interactions
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class DirectionalTrigger : HandyComponent
    {
        #region Inspector

        #endregion

        #region Fields

        protected BoxCollider2D _boxCollider;
        protected bool _subjectInside;

        // Actions
        private UnityAction FromRightAction;
        private UnityAction FromLeftAction;
        private UnityAction FromAboveAction;
        private UnityAction FromBelowAction;

        #endregion

        #region Properties

        protected abstract Collider2D subjectCollider { get; }

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            FindComponent<BoxCollider2D>(ref _boxCollider);
            _boxCollider.isTrigger = true;

            LoadActions();
        }

        protected void LoadActions()
        {
            System.Type stateType = this.GetType();
            MethodInfo mi;

            mi = stateType.GetMethod("FromRight", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                FromRightAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("FromLeft", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                FromLeftAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("FromAbove", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                FromAboveAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("FromBelow", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                FromBelowAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;
        }

        #endregion

        #region Logic

        protected void EvaluateDirection()
        {
            if (subjectCollider == null) { return; }

            //From Right
            if (subjectCollider.bounds.center.x < _boxCollider.bounds.center.x)
            {
                FromRightAction?.Invoke();
            }

            // From Left
            if (subjectCollider.bounds.center.x > _boxCollider.bounds.center.x)
            {
                FromLeftAction?.Invoke();
            }

            //From Above
            if (subjectCollider.bounds.center.y < _boxCollider.bounds.center.y)
            {
                FromAboveAction?.Invoke();
            }

            // From Bellow
            if (subjectCollider.bounds.center.y > _boxCollider.bounds.center.y)
            {
                FromBelowAction?.Invoke();
            }
        }

        #endregion

        #region Collision Callbacks

        protected void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if (_subjectInside || otherCollider != subjectCollider) return;

            _subjectInside = true;
        }

        protected void OnTriggerExit2D(Collider2D otherCollider)
        {
            if (!_subjectInside || otherCollider != subjectCollider) return;

            _subjectInside = false;
            EvaluateDirection();
        }

        #endregion

    }
}
