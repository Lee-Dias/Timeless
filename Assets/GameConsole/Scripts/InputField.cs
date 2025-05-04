using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace GameConsole
{
    /// <summary>
    /// Custom InputField class that handles text input, caret movement, selection, and events.
    /// </summary>
    public class InputField : MonoBehaviour
    {
        // The text component used to display the input text.
        [SerializeField] private TMP_Text textArea;

        // The image used to highlight selected text.
        [SerializeField] private Image selectionImage;

        [SerializeField] private float holdKeyThreashold = .25f;
        [SerializeField] private float delectionDelay = 0.05f;

        // Event triggered when the user submits the text (presses Enter).
        public UnityEvent<string> onSubmit;

        // Event triggered when the text changes (during typing).
        public UnityEvent<string> onTextChanged;

        // The RectTransform of the selection image.
        private RectTransform selectionImageRectTransform;

        // The actual text entered by the user.
        private string text = "";

        // Temporary preview text when in preview mode.
        private string previewText = "";

        // The current position of the caret in the text.
        private int caretPosition = 0;

        // The start position of the text selection.
        private int selectionStart = -1;

        /// The end position of the text selection.
        private int selectionEnd = -1;

        // Indicates whether the input is in preview mode.
        private bool isPreviewing = false;

        // Indicates whether all text is currently selected.
        private bool allTextSelected = false;

        // Time interval for caret blinking (in seconds).
        private float caretBlinkTime = 0.5f;

        // Timer to track the caret blinking time.
        private float lastCaretBlinkTime = 0f;

        // Indicates whether the caret is currently visible.
        private bool caretVisible = true;

        private bool canListenToInput = true;

        /// <summary>
        /// Initializes the input field and its components.
        /// </summary>
        private void Start()
        {
            if (textArea == null || selectionImage == null)
            {
                Debug.LogError("Text Area or Selection Image is not assigned!", this);
                return;
            }

            selectionImageRectTransform = selectionImage.GetComponent<RectTransform>();
            selectionImage.gameObject.SetActive(false); // Initially hide the selection image
        }

        /// <summary>
        /// Updates the input field state, handling text input, selection, and caret blinking.
        /// </summary>
        private void Update()
        {
            if (!canListenToInput) return;
            HandleTextInput();     // Handle user input
            HandleSelection();     // Handle text selection
            HandleCaretBlinking(); // Handle the caret blinking only when not previewing
        }
        private void OnEnable()
        {
            canListenToInput = false;
            StartCoroutine(ListenToInputAfterAFrame());
        }

        private IEnumerator ListenToInputAfterAFrame()
        {
            yield return null;
            canListenToInput = true;
        }

        /// <summary>
        /// Handles all text input, including typing, backspace, and special key presses.
        /// </summary>
        private void HandleTextInput()
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    // Submit text if previewing is off
                    if (isPreviewing)
                    {
                        text = previewText + " ";
                        caretPosition = text.Length; // Move caret to end of preview text
                        isPreviewing = false;
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        onSubmit?.Invoke(text); // Submit the current text
                        ClearText();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (isPreviewing)
                    {
                        text = previewText + " ";
                        caretPosition = text.Length; // Move caret to end of preview text
                        isPreviewing = false;
                    }
                }
                else if (Input.GetKey(KeyCode.Backspace))
                {
                    // Handle Backspace key with Ctrl + Backspace for word deletion
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        // Ctrl + Backspace: Delete to the previous space or start
                        if (caretPosition > 0)
                        {
                            int deleteToIndex = caretPosition - 1;

                            // Skip over consecutive spaces
                            while (deleteToIndex >= 0 && text[deleteToIndex] == ' ')
                            {
                                deleteToIndex--;
                            }

                            // Find the previous space or start
                            deleteToIndex = text.LastIndexOf(' ', deleteToIndex);
                            deleteToIndex = deleteToIndex == -1 ? 0 : deleteToIndex + 1; // Include space only if found

                            text = text.Remove(deleteToIndex, caretPosition - deleteToIndex);
                            caretPosition = deleteToIndex; // Update caret position
                        }
                    }
                    else
                    {
                        // Regular Backspace
                        if (allTextSelected)
                        {
                            ClearText();
                        }
                        else if (caretPosition > 0 && caretPosition <= text.Length)
                        {
                            if (isPreviewing)
                            {
                                ClearPreview();
                            }

                            text = text.Remove(caretPosition - 1, 1);
                            caretPosition--; // Move caret position back
                            selectionStart = -1; // Reset selection
                        }
                        allTextSelected = false;

                        StartCoroutine(HoldBackspaceKey());
                    }
                }

                else if (Input.GetKey(KeyCode.Delete))
                {
                    // Handle Delete key behavior
                    if (allTextSelected)
                    {
                        ClearText();
                    }
                    else if (caretPosition >= 0 && caretPosition < text.Length)
                    {
                        if (isPreviewing)
                        {
                            text = previewText + " ";
                            caretPosition = text.Length; // Move caret to end of preview text
                            isPreviewing = false;
                        }

                        text = text.Remove(caretPosition, 1);
                        selectionStart = -1; // Reset selection
                    }
                    allTextSelected = false;

                    StartCoroutine(HoldDeleteKey());
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) && caretPosition > 0)
                {
                    // Move caret left
                    if (allTextSelected)
                    {
                        caretPosition = 0;
                    }
                    else
                    {
                        caretPosition--;
                    }
                    UpdateTextArea();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) && caretPosition < text.Length)
                {
                    // Move caret right
                    if (allTextSelected)
                    {
                        caretPosition = text.Length;
                    }
                    else
                    {
                        caretPosition++;
                    }
                    UpdateTextArea();
                }
                else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
                {
                    // Select all text (Ctrl + A)
                    SelectAllText();
                }
                else
                {
                    string inputText = Input.inputString;

                    if (!string.IsNullOrEmpty(inputText))
                    {
                        // Handle text input (e.g., letters, numbers)
                        if (allTextSelected)
                        {
                            ClearText();
                        }

                        if (isPreviewing)
                        {
                            ClearPreview();
                        }

                        foreach (char c in inputText)
                        {
                            text = text.Insert(caretPosition, c.ToString());
                            caretPosition++;
                        }

                        allTextSelected = false;
                    }
                }

                if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
                {
                    isPreviewing = false; // Exit preview mode when typing
                    onTextChanged?.Invoke(text);
                }

                UpdateTextArea();
            }
        }

        /// <summary>
        /// Selects all the text in the input field (Ctrl + A).
        /// </summary>
        private void SelectAllText()
        {
            if (text.Length > 0)
            {
                selectionStart = 0;
                selectionEnd = text.Length;
                caretPosition = text.Length;
                allTextSelected = true;
                UpdateTextArea();
            }
            else
            {
                selectionStart = -1;
                selectionEnd = -1;
                allTextSelected = false;
            }
        }

        /// <summary>
        /// Updates the text displayed in the input field.
        /// </summary>
        private void UpdateTextArea()
        {
            string displayText;

            // Make preview text darker
            if (isPreviewing)
            {
                // Ensure the intersecting text is correctly handled
                string intersectingText = text.Substring(0, Mathf.Min(previewText.Length, text.Length));
                string remainingText = previewText.Length > text.Length ? previewText.Substring(text.Length) : "";

                if (string.IsNullOrEmpty(remainingText) || string.IsNullOrWhiteSpace(remainingText))
                {
                    displayText = $"{intersectingText}";
                }
                else
                    displayText = $"{intersectingText}<color=#808080>{remainingText}</color>";
            }
            else displayText = text;

            if (selectionStart >= 0)
            {
                selectionImage.gameObject.SetActive(true);
                UpdateSelectionImage();
            }
            else
            {
                selectionImage.gameObject.SetActive(false);
            }

            // Display caret even when previewing
            if (caretVisible)
            {
                displayText = displayText.Insert(caretPosition, "|");
            }

            // Apply rich text tags to the TMP_Text component
            textArea.text = displayText;
        }

        /// <summary>
        /// Handles caret blinking by toggling its visibility at a set interval.
        /// </summary>
        private void HandleCaretBlinking()
        {
            lastCaretBlinkTime += Time.deltaTime;

            if (lastCaretBlinkTime >= caretBlinkTime)
            {
                caretVisible = !caretVisible; // Toggle caret visibility
                lastCaretBlinkTime = 0f;     // Reset timer
                UpdateTextArea();            // Refresh text area
            }
        }

        /// <summary>
        /// Handles the text selection behavior.
        /// </summary>
        private void HandleSelection()
        {
            if (selectionStart >= 0 && selectionEnd >= 0 && selectionStart != selectionEnd)
            {
                selectionImage.gameObject.SetActive(true);
                UpdateSelectionImage();
            }
            else
            {
                selectionImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the selection image based on the selected text range.
        /// </summary>
        private void UpdateSelectionImage()
        {
            // Ensure that the text info is valid and that the indices are within bounds
            if (textArea.textInfo.characterCount > 0)
            {
                float startPos = 0f;
                float endPos = 0f;

                // Ensure the indices are within the character count
                int start = Mathf.Clamp(selectionStart, 0, textArea.textInfo.characterCount - 1);
                int end = Mathf.Clamp(selectionEnd, 0, textArea.textInfo.characterCount - 1);

                if (start < end)
                {
                    // Text is selected from selectionStart to selectionEnd
                    startPos = textArea.textInfo.characterInfo[start].origin;
                    endPos = textArea.textInfo.characterInfo[end].origin;
                }
                else if (start > end)
                {
                    // Text is selected from selectionEnd to selectionStart
                    startPos = textArea.textInfo.characterInfo[end].origin;
                    endPos = textArea.textInfo.characterInfo[start].origin;
                }

                // Adjust the selection image size and position based on the selection
                selectionImageRectTransform.sizeDelta = new Vector2(endPos - startPos, selectionImageRectTransform.rect.height);
                selectionImageRectTransform.position = textArea.transform.TransformPoint(new Vector3(startPos, 0, 0));
            }
            else
            {
                selectionImage.gameObject.SetActive(false);
            }
        }

        private IEnumerator HoldDeleteKey()
        {
            yield return new WaitForSeconds(holdKeyThreashold);

            // Check if delete is being held before creating the WaitForSeconds
            // GC likes it.
            if (!Input.GetKey(KeyCode.Delete)) yield break;

            YieldInstruction waitForDelectionDelay = new WaitForSeconds(delectionDelay);

            while (Input.GetKey(KeyCode.Delete))
            {
                if (allTextSelected)
                {
                    ClearText();
                }
                else if (caretPosition >= 0 && caretPosition < text.Length)
                {
                    if (isPreviewing)
                    {
                        text = previewText + " ";
                        caretPosition = text.Length; // Move caret to end of preview text
                        isPreviewing = false;
                    }

                    text = text.Remove(caretPosition, 1);
                    selectionStart = -1; // Reset selection
                }
                allTextSelected = false;

                UpdateTextArea();

                yield return waitForDelectionDelay;
            }
        }

        private IEnumerator HoldBackspaceKey()
        {
            yield return new WaitForSeconds(holdKeyThreashold);

            // Check if backspace is being held before creating the WaitForSeconds
            // GC likes it.
            if (!Input.GetKey(KeyCode.Backspace)) yield break;

            YieldInstruction waitForDelectionDelay = new WaitForSeconds(delectionDelay);

            while (Input.GetKey(KeyCode.Backspace))
            {
                // Regular Backspace
                if (allTextSelected)
                {
                    ClearText();
                }
                else if (caretPosition > 0 && caretPosition <= text.Length)
                {
                    if (isPreviewing)
                    {
                        ClearPreview();
                    }

                    text = text.Remove(caretPosition - 1, 1);
                    caretPosition--; // Move caret position back
                    selectionStart = -1; // Reset selection
                }
                allTextSelected = false;

                UpdateTextArea();

                yield return waitForDelectionDelay;
            }
        }

        /// <summary>
        /// Sets the text and exits preview mode.
        /// </summary>
        public void SetText(string newText)
        {
            text = newText;
            caretPosition = text.Length;
            isPreviewing = false;
            UpdateTextArea();
            onTextChanged?.Invoke(text);
        }

        /// <summary>
        /// Sets the preview text and enables preview mode.
        /// </summary>
        public void SetPreviewText(string preview)
        {
            previewText = preview;
            isPreviewing = true;
            UpdateTextArea();
        }

        /// <summary>
        /// Clears the preview text and exits preview mode.
        /// </summary>
        public void ClearPreview()
        {
            previewText = "";
            isPreviewing = false;
            UpdateTextArea();
        }

        /// <summary>
        /// Clears all text from the input field.
        /// </summary>
        public void ClearText()
        {
            text = "";
            caretPosition = 0;
            selectionStart = -1;
            UpdateTextArea();
        }

        /// <summary>
        /// Retrieves the current text from the input field.
        /// </summary>
        public string GetText()
        {
            return text;
        }
    }
}
