using UnityEngine;

public class UIImageProgress : MonoBehaviour
{

    private float OrgSize = 0;

    // Start is called before the first frame update
    private float mProgress = 0;
    public float progress
    {
        get { return mProgress; }
        set
        {
            Vector2 size = GetComponent<RectTransform>().sizeDelta;
            if (OrgSize == 0)
            {
                OrgSize = size.x;
            }
            mProgress = value;
            size.x = Mathf.Clamp(value, 0, 1) * OrgSize;
            GetComponent<RectTransform>().sizeDelta = size;
        }
    }
}
