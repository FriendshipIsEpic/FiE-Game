using UnityEngine;
using System.Collections;

public class UBHelper : MonoBehaviour 
{

    static UBHelper()
    {
        s_Styles = new Styles();
    }

    public static bool GroupHeader(string text, bool isExpanded)
    {
        Rect rect = GUILayoutUtility.GetRect(16f, 22f, s_Styles.header);

        s_Styles.Backup();
        s_Styles.Apply();

        if (Event.current.type == EventType.Repaint)
            s_Styles.header.Draw(rect, text, isExpanded, isExpanded, isExpanded, isExpanded);

        Event e = Event.current;
        if (e.type == EventType.MouseDown)
        {
            if (rect.Contains(e.mousePosition))
            {
                isExpanded = !isExpanded;
                e.Use();
            }
        }

        s_Styles.Revert();
        return isExpanded;
    }

    private static Styles s_Styles;
    private class Styles
    {
        public GUIStyle header = "ShurikenModuleTitle";
        public GUIStyle headerArrow = "AC RightArrow";

        internal Styles()
        {
            header.font = (new GUIStyle("Label")).font;
        }

        RectOffset m_Border;
        float m_FixedHeight;
        Vector2 m_ContentOffset;
        TextAnchor m_TextAlign;
        FontStyle m_TextStyle;
        int m_FontSize;

        public void Backup()
        {
            m_Border = s_Styles.header.border;
            m_FixedHeight = s_Styles.header.fixedHeight;
            m_ContentOffset = s_Styles.header.contentOffset;
            m_TextAlign = s_Styles.header.alignment;
            m_TextStyle = s_Styles.header.fontStyle;
            m_FontSize = s_Styles.header.fontSize;
        }

        public void Apply()
        {
            s_Styles.header.border = new RectOffset(7, 7, 4, 4);
            s_Styles.header.fixedHeight = 22;
            s_Styles.header.contentOffset = new Vector2(20f, -2f);
            s_Styles.header.alignment = TextAnchor.MiddleLeft;
            s_Styles.header.fontStyle = FontStyle.Bold;
            s_Styles.header.fontSize = 12;
        }

        public void Revert()
        {
            s_Styles.header.border = m_Border;
            s_Styles.header.fixedHeight = m_FixedHeight;
            s_Styles.header.contentOffset = m_ContentOffset;
            s_Styles.header.alignment = m_TextAlign;
            s_Styles.header.fontStyle = m_TextStyle;
            s_Styles.header.fontSize = m_FontSize;
        }
    }
}
