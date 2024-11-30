using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameConsole
{
    /// <summary>
    /// Represents a single suggestion in the user interface. This class is used
    /// to display a suggestion in the form of text, with functionality to select
    /// and deselect the suggestion, changing its background color accordingly.
    /// </summary>
    public class SuggestionUI : MonoBehaviour
    {
        /// <summary>
        /// The text component that will display the suggestion's content.
        /// </summary>
        public TMP_Text suggestionText;

        /// <summary>
        /// The background image component of the suggestion, which changes color when selected or deselected.
        /// </summary>
        public Image backGroundImage;

        /// <summary>
        /// The default background color for the suggestion when it is not selected.
        /// </summary>
        public Color backGroundColor;

        /// <summary>
        /// The background color for the suggestion when it is selected.
        /// </summary>
        public Color selectedColor;

        /// <summary>
        /// Highlights the suggestion by changing its background color to the selected color.
        /// </summary>
        public void Select()
        {
            backGroundImage.color = selectedColor;  // Set background color to the selected color.
        }

        /// <summary>
        /// Reverts the suggestion's background color back to the default color when it is deselected.
        /// </summary>
        public void Deselect()
        {
            backGroundImage.color = backGroundColor;  // Set background color to the default color.
        }
    }
}
