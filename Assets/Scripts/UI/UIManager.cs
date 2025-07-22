using Audio;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessTavernUI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip buttonAudioClip = null;
        [Header("Canvas")]
        [SerializeField] private Canvas defaultCanvas = null;
        [SerializeField] private Canvas[] canvas = new Canvas[0];
        [Header("Text")]
        [SerializeField] protected TextMeshProUGUI[] textArea = new TextMeshProUGUI[0];
        [Header("Button")]
        [SerializeField] private Button[] button = new Button[0];

        private Canvas currentActiveCanvas = null;
        private Stack<Canvas> previousCanvas = new Stack<Canvas>();

        protected virtual void Start() => currentActiveCanvas = defaultCanvas;

        /// <summary>
        /// Enable the canvas in the scene with the corresponding tag
        /// </summary>
        /// <param name="canvasTagName">the tag of the canvas</param>
        /// <param name="hidePreviousCanvas">if true, disable the previous canvas</param>
        /// <param name="savePreviousCanvas">if true (by default), save the current active canvas in the stack before enabling the target one</param>
        private void ShowCanvas(string canvasTagName, bool hidePreviousCanvas, bool savePreviousCanvas = true)
        {
            Canvas targetCanvas = FindTargetElement<Canvas>(canvas, canvasTagName);
            if (targetCanvas == null || targetCanvas == currentActiveCanvas)
                return;

            if (hidePreviousCanvas)
                currentActiveCanvas.gameObject.SetActive(false);

            if (savePreviousCanvas)
                previousCanvas.Push(currentActiveCanvas);

            targetCanvas.gameObject.SetActive(true);
            currentActiveCanvas = targetCanvas;
        }

        /// <summary>
        /// find the target ui element in the targetArray searching for a specific tag
        /// </summary>
        /// <typeparam name="T">the type of the element</typeparam>
        /// <param name="targetArray">the target array containing the elements of type T</param>
        /// <param name="targetTagName">the tag of the target element</param>
        /// <returns>the target element if found, null otherwise</returns>
        protected T FindTargetElement<T>(T[] targetArray, string targetTagName) where T : Component
        {
            foreach (T target in targetArray)
                if (target.CompareTag(targetTagName))
                    return target;

            return null;
        }

        /// <summary>
        /// Find all the target ui elements in the targetArray with the specified tag
        /// </summary>
        /// <typeparam name="T">the type of the element</typeparam>
        /// <param name="targetArray">the target array containing the elements of type T</param>
        /// <param name="targetTagName">the tag of the target elements</param>
        /// <returns>A list containing all the target elements</returns>
        protected List<T> FindTargetElements<T>(T[] targetArray, string targetTagName) where T : Component
        {
            List<T> output = new List<T>();

            foreach (T target in targetArray)
                if (target.CompareTag(targetTagName))
                    output.Add(target);

            return output;
        }

        private void ToggleButton(string targetButtonTag, bool toggle)
        {
            Button targetButton = FindTargetElement<Button>(button, targetButtonTag);

            if (targetButton == null)
                return;

            targetButton.interactable = toggle;
        }

        /// <summary>
        /// Set the text of a specific text area
        /// </summary>
        /// <param name="targetTextAreaTag">the tag of the target text area</param>
        /// <param name="text">the text assigned to the text area</param>
        public void SetText(string targetTextAreaTag, string text)
        {
            TextMeshProUGUI targetTextArea = FindTargetElement<TextMeshProUGUI>(textArea, targetTextAreaTag);

            if (targetTextArea == null)
                return;

            targetTextArea.text = text;
        }

        /// <summary>
        /// Set the text of all the text area with the specified
        /// </summary>
        /// <param name="targetTextsAreaTag">the tag of the target text areas</param>
        /// <param name="text">the text assigned to the text areas</param>
        public void SetAllTexts(string targetTextsAreaTag, string text)
        {
            List<TextMeshProUGUI> targetTextArea = FindTargetElements<TextMeshProUGUI>(textArea, targetTextsAreaTag);

            foreach(TextMeshProUGUI t in targetTextArea)
                t.text = text;
        }

        /// <summary>
        /// show the previous canvas (if present) disabling the current one
        /// </summary>
        public void ShowPreviousCanvas()
        {
            Canvas targetCanvas;
            
            if (previousCanvas.TryPop(out targetCanvas))
                ShowCanvas(targetCanvas.tag, true, false);
        }

        /// <summary>
        /// Show a canvas hiding the previous one
        /// </summary>
        /// <param name="canvasTagName">the tag of the canvas</param>
        public void ShowCanvas(string canvasTagName) => ShowCanvas(canvasTagName, true);
        /// <summary>
        /// Show a canvas on top of the previous one without hiding it
        /// </summary>
        /// <param name="canvasTagName">the tag of the canvas</param>
        public void ShowPopUpCanvas(string canvasTagName) => ShowCanvas(canvasTagName, false);
        /// <summary>
        /// Switch the current canvas with the canvasTagName canvas.
        /// This doesn't save the previous canvas on the stack.
        /// </summary>
        /// <param name="canvasTagName">the tag of the canvas</param>
        public void SwitchCanvas(string canvasTagName) => ShowCanvas(canvasTagName, true, false);

        public void EnableButton(string targetButtonTag) => ToggleButton(targetButtonTag, true);
        public void DisableButton(string targetButtonTag) => ToggleButton(targetButtonTag, false);

        public void PlayButtonSFX() => AudioPlayer.instance.PlaySFX(buttonAudioClip);
    }
}
