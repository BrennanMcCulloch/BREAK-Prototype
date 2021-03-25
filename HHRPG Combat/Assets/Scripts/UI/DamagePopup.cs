using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private static Transform damagePopup;
    

    public static DamagePopup Create(Vector3 position, string thingToDisplay, float r, float g, float b)
    {
        AssetDatabase.ImportAsset("Assets/Prefabs/UI/damagePopup.prefab");
        //GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/UI/damagePopup.prefab");
        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/damagePopup.prefab", typeof(GameObject));
        GameObject damage = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        DamagePopup damagePopupTransform = damage.GetComponent<DamagePopup>();
        damagePopupTransform.Setup(thingToDisplay, r, g, b);

        return damagePopupTransform;
    }

    private TextMeshPro textMesh;
    private float disappear;
    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(string affinity, float r, float g, float b)
    {
        textMesh.SetText(affinity);
        textMesh.color = new Color(r, g, b, 1);
        textColor = textMesh.color;
        disappear = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveYSpeed = 2;
        transform.position += new Vector3(0, moveYSpeed * Time.deltaTime, 0);

        disappear -= Time.deltaTime;
        if(disappear < 0)
        {
            //Start disappearing
            float disappearSpeed = 1;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
