using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3 Direction {  get; set; }
    public float Speed { get; set; }
    public float Duration { get; set; }

    float timeAlive = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;
        timeAlive += Time.deltaTime;
        if (timeAlive >= Duration)
            Destroy(gameObject);
    }

	private void OnTriggerEnter(Collider other)
	{
		Destroy(gameObject);
	}
}
