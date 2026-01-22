using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(h, v, 0).normalized * _moveSpeed * Time.deltaTime;
        transform.Translate(movement); //возможно нужно будет заменить, самая простая обработка передвижения без физики
    }

}
