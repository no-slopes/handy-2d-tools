using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using H2DT.NaughtyAttributes;
using System;
using H2DT.Debugging;

namespace H2DT
{
    public abstract class HandyComponent : MonoBehaviour
    {
        #region Components

        /// <summary>
        /// Tries finding a component among the object's components, or, if you provide a subject, among the subject's components.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="subject"></param>
        /// <typeparam name="T"></typeparam>
        protected virtual void FindComponent<T>(ref T component, GameObject subject = null)
        {
            if (subject != null)
            {
                component = subject.GetComponent<T>();
                if (component == null)
                    Log.Warning($"Component {GetType().Name} might not work properly. It it could not find any {typeof(T).Name} among the {subject.name} components.");
                return;
            }

            component = GetComponent<T>();
            if (component == null)
                Log.Warning($"Component {GetType().Name} might not work properly.  It needs an {typeof(T).Name} but it could not find any.");
        }

        /// <summary>
        /// Tries finding a component among the object's components, the object's children, or, if you provide a subject, among the subject's components.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="subject"></param>
        /// <typeparam name="T"></typeparam>
        protected virtual void FindComponentInChildren<T>(ref T component, GameObject subject = null)
        {
            if (subject != null)
            {
                component = subject.GetComponentInChildren<T>();
                if (component == null)
                    Log.Warning($"Component {GetType().Name} might not work properly. It it could not find any {typeof(T).Name} among the {subject.name} components.");
                return;
            }

            component = GetComponentInChildren<T>();
            if (component == null)
                Log.Warning($"Component {GetType().Name} might not work properly.  It needs an {typeof(T).Name} but it could not find any.");
        }

        /// <summary>
        /// Tries finding a component among the object's components, the object's parent, or, if you provide a subject, among the subject's components.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="subject"></param>
        /// <typeparam name="T"></typeparam>
        protected virtual void FindComponentInParent<T>(ref T component, GameObject subject = null)
        {
            if (subject != null)
            {
                component = subject.GetComponent<T>();
                if (component == null)
                    Log.Warning($"Component {GetType().Name} might not work properly. It it could not find any {typeof(T).Name} among the {subject.name} components.");
                return;
            }

            component = GetComponentInParent<T>();
            if (component == null)
                Log.Warning($"Component {GetType().Name} might not work properly.  It needs an {typeof(T).Name} but it could not find any.");
        }

        /// <summary>
        /// Seeks for a component among the object's components, its children components or its parent components.
        /// </summary>
        /// <param name="marked"></param>
        /// <param name="component"></param>
        /// <param name="subject"></param>
        /// <typeparam name="T"></typeparam>
        protected virtual void SeekComponent<T>(bool marked, ref T component)
        {
            if (!marked) return;

            component = GetComponentInParent<T>();

            if (component != null) return;

            component = GetComponentInChildren<T>();

            if (component == null)
                Log.Warning($"Component {GetType().Name} might not work properly.  It is marked to seek for {typeof(T).Name} but it could not find any.");
        }

        #endregion
    }
}
