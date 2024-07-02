using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    Text text;
    Color saveAlpha;
    Color alpha;
    private float damage;

    private void Start()
    {
        moveSpeed = 2.0f;
        alphaSpeed = 2.0f;
        text = GetComponent<Text>();
        alpha = text.color;
        saveAlpha = alpha;
    }
    private void OnEnable()
    {
        StartCoroutine(DestroyTime());
    }

    private void Update()
    {
        text.text = damage.ToString();
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    public void GetInfo(float _damage)
    {
        damage = _damage;
    }

    private IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(2f);
        damage = 0;
        text.text = "";
        alpha = saveAlpha;
        UIManager.instance.InsertDamageText(this);
    }
}
