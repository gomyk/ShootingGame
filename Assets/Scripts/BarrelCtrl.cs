using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class BarrelCtrl : MonoBehaviour {
    public GameObject expEffect;
    private Transform tr;
    public AudioClip fireSfx;
    private AudioSource source = null;
    public Texture[] textures;

    private int hitCount = 0;
	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
        source = GetComponent<AudioSource>();
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];
	}
	void OnCollisionEnter(Collision coll)
    {
       if(coll.collider.tag =="Bullet")
        {
            Destroy(coll.gameObject);

            if(++hitCount >=6)
            {
                ExpBarrel();
                source.PlayOneShot(fireSfx, 5.0f);
            }
        }
    }

    void ExpBarrel()
    {
        Instantiate(expEffect, tr.position, Quaternion.identity);
      
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);

        foreach (Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if(rbody != null)
            {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }

        Destroy(gameObject, 5.0f);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
