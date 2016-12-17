﻿using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {
    public enum MonsterState { idle,trace,attack,die};
    public MonsterState monsterState = MonsterState.idle;
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;
    public float traceDist = 25.0f;
    public float attackDist = 5.0f;
    private bool isDie = false;

    private int hp = 200;
    public GameObject bloodEffect;
    public GameObject bloodDecal;
    private GameUI gameUI;
	// Use this for initialization
	void Start () {
        monsterTr = this.gameObject.GetComponent<Transform>();

        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();
      //  nvAgent.destination = playerTr.position;

        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator CheckMonsterState()
    {
        while(!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if(dist<=attackDist)
            {
                monsterState = MonsterState.attack;
            }
            else if(dist <= traceDist)
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }
    }
    IEnumerator MonsterAction()
    {
        while(!isDie)
        {
            switch(monsterState)
            {
                case MonsterState.idle:
                        nvAgent.Stop();
                    animator.SetBool("isTrace", false);
                        break;
                    
                case MonsterState.trace:
                        nvAgent.destination = playerTr.position;
                        nvAgent.Resume();
                    animator.SetBool("isAttack", false);
                    animator.SetBool("isTrace", true);
                        break;

                case MonsterState.attack:
                    animator.SetBool("isAttack", true);
                    break;
            }
            yield return null;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Bullet")
        {
            CreateBloodEffect(coll.transform.position);
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if(hp <= 0)
            {
                MonsterDie();
            }
            Destroy(coll.gameObject);
            animator.SetTrigger("isHit");
        }
    }
    void MonsterDie()
    {
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.Stop();
        animator.SetTrigger("isDie");

        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
        gameUI.DispScore(50);
    }
    void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f);

        Vector3 decalPos = monsterTr.position + (Vector3.up*0.2f);
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);

        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        Destroy(blood2, 5.0f);
        
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();

        nvAgent.Stop();
        animator.SetTrigger("isPlayerDie");
    }
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
}
