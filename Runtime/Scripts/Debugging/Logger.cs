using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace H2DT.Debugging
{

    /// <summary>
    /// Extend from this class to create your custom Logger
    /// and use its utillities
    /// </summary>
    public abstract class Logger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected bool m_shouldLog = true;

        [Header("Colors")]
        [SerializeField] protected Color m_successColor = Color.green; // Green
        [SerializeField] protected Color m_warningColor = Color.yellow; // Yellow
        [SerializeField] protected Color m_dangerColor = Color.red; // Red

        protected static GameObject m_logger;

        /// <summary>
        /// The success color's Hexadecimal code
        /// </summary>
        /// <returns> The hex code string </returns>
        protected string successHEX => "#" + ColorUtility.ToHtmlStringRGB(m_successColor);

        /// <summary>
        /// The warning color's Hexadecimal code
        /// </summary>
        /// <returns> The hex code string </returns>
        protected string warningHEX => "#" + ColorUtility.ToHtmlStringRGB(m_warningColor);

        /// <summary>
        /// The danger color's Hexadecimal code
        /// </summary>
        /// <returns> The hex code string </returns>
        protected string dangerHEX => "#" + ColorUtility.ToHtmlStringRGB(m_dangerColor);

        /// <summary>
        /// The white color's Hexadecimal code
        /// </summary>
        /// <returns> The hex code string </returns>
        protected string whiteHEX => "#FFFFFF";

        /// <summary>
        /// The Logger's root method for logging. Use this if you are creating a custom 
        /// Logger.
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="color">The hex code of the color your logged text shoul have</param>
        /// <param name="sender">The object where the log is comming from if any</param>
        protected virtual void DoLog(string message, string color = "#FFFFFF", Object sender = null)
        {
            if (!m_shouldLog) return;

            if (sender != null)
            {
                Debug.Log($"<color={color}> {message} </color>", sender);

            }
            else
            {
                Debug.Log($"<color={color}> {message} </color>");
            }
        }

        /// <summary>
        /// The Logger's root method for logging. Use this if you are creating a custom 
        /// Logger.
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="color">The hex code of the color your logged text shoul have</param>
        /// <param name="sender">The object where the log is comming from if any</param>
        protected virtual void DoLogWarning(string message, string color = "#FFFFFF", Object sender = null)
        {
            if (!m_shouldLog) return;

            if (sender != null)
            {
                Debug.LogWarning($"<color={color}> {message} </color>", sender);

            }
            else
            {
                Debug.LogWarning($"<color={color}> {message} </color>");
            }
        }

        /// <summary>
        /// The Logger's root method for logging. Use this if you are creating a custom 
        /// Logger.
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="color">The hex code of the color your logged text shoul have</param>
        /// <param name="sender">The object where the log is comming from if any</param>
        protected virtual void DoLogError(string message, string color = "#FFFFFF", Object sender = null)
        {
            if (!m_shouldLog) return;

            if (sender != null)
            {
                Debug.LogError($"<color={color}> {message} </color>", sender);

            }
            else
            {
                Debug.LogError($"<color={color}> {message} </color>");
            }
        }

        public static GameObject FindOrInstantiate<T>()
        {
            var loggerName = typeof(T).Name;
            m_logger = GameObject.Find(loggerName);
            if (!m_logger) { m_logger = new GameObject(loggerName); }

            var log = m_logger.GetComponent<T>();
            if (log != null) { m_logger.AddComponent(typeof(T)); }

            return m_logger;
        }
    }
}