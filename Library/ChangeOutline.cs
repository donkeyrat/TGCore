using UnityEngine;

namespace TGCore.Library
{
    public class ChangeOutline : MonoBehaviour
    {
        public void Start()
        {
            var outline = transform.root.GetComponent<Outline>();
            if (outline)
            {
                outline.OutlineMode = outlineMode;
                outline.SetHighlightColor(outlineColor);
                outline.OutlineWidth = outlineWidth;
            }
        }

        public Outline.Mode outlineMode;

        public Color outlineColor = Color.white;

        [Range(0f, 10f)]
        public float outlineWidth = 1f;
    }
}