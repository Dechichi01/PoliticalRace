using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private Transform tEnemy; //main character transform
    private Animation aEnemy; //character animation

    Vector3 velocity;
    float gravity = 15f;

    float timeToDie = 1f;
    float timeOnHit;

    void Start () {
        tEnemy = transform;
        aEnemy = this.transform.Find("EnemyRotation/EnemyMesh/Prisoner").GetComponent<Animation>() as Animation;

        StartCoroutine("playIdleAnimations");
    }

    IEnumerator playIdleAnimations()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!aEnemy.IsPlaying("Idle_1") && !aEnemy.IsPlaying("Idle_2"))
            {
                aEnemy.GetComponent<Animation>().Play("Idle_1");
                aEnemy.PlayQueued("Idle_2", QueueMode.CompleteOthers);
            }
        }       
    }

    IEnumerator launchEnemy()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            tEnemy.Translate(velocity * Time.fixedDeltaTime);
            velocity += Vector3.down * gravity *Time.fixedDeltaTime;

            if (Time.time > (timeOnHit + timeToDie))
            {
                Destroy(gameObject);
                StopAllCoroutines();
            }
        }

    }

    public void KillEnemy()
    {
        float rotY = Random.Range(-20f, 20f);
        velocity = Quaternion.Euler(28, rotY, 0)* (-Vector3.forward*35f);
        StopCoroutine("playIdleAnimations");
        StartCoroutine("launchEnemy");
        aEnemy.Play("jump");

        timeOnHit = Time.time;
    }

	void FixedUpdate () {
        return;	
	}
}
