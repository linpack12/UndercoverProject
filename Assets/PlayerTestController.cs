using UnityEngine;
public class PlayerMover : MonoBehaviour
{
    public float speed = 5f;
    private SuspiciousActionComponent _suspicion;

    [SerializeField] private ActionData testData;

    private void Awake()
    {
        _suspicion = GetComponent<SuspiciousActionComponent>();
    }

    void Update()
    {
        var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.Translate(dir.normalized * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _suspicion.MarkSuspicious(testData);
            Debug.Log("Player triggered suspicious action");
        }
    }
}