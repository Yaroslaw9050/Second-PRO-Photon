using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.IO;
public class Aim : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private List<GameObject> allTargets;
    [SerializeField] private GameObject targetCylinder;
    [SerializeField] private float range;
    private PlayerInput inputs;
    private PhotonView pv;
    private CharacterController controller;
    private GameObject targetObj;
    private bool canSearch = true;
    private int targetCount;

    private void Awake()
    {
        inputs = new PlayerInput();
        controller = GetComponent<CharacterController>();
        pv = GetComponentInParent<PhotonView>();
    }
    private void OnEnable()
    {
        inputs.PlayerController.Enable();
    }
    private void OnDisable()
    {
        inputs.PlayerController.Disable();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    public void SetTargetStatus(bool isTarget)
    {
        targetCylinder.SetActive(isTarget);
    }
    private void SelectTarget()
    {
        if(controller.velocity == Vector3.zero)
        {
            if(canSearch)
            InvokeRepeating("Calculate",0f, 0.5f);
        }
        else
        {
            if(targetObj == null) return;

            targetObj.GetComponent<Aim>().SetTargetStatus(false);
            targetObj = null;

            canSearch = true;
            CancelInvoke();
        }
    }

    private void Calculate()
    {
        canSearch = false;
        allTargets.Clear();

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, range, transform.position, range);

        foreach(RaycastHit hit in hits)
        {
            GameObject tempObj = hit.collider.gameObject;

            if(tempObj.GetComponent<PlayerController>() && !tempObj.GetComponentInParent<PhotonView>().IsMine)
            {
                allTargets.Add(tempObj);
            }
            else continue;
        }
        SelectNewTarget();
    }
    private void SelectNewTarget()
    {
        foreach(GameObject obj in allTargets)
        {
            obj.GetComponent<Aim>().SetTargetStatus(false);
        }
        if(targetCount >= allTargets.Count)
        {
            targetCount = 0;
        }
        targetObj = allTargets[targetCount];
        targetObj.GetComponent<Aim>().SetTargetStatus(true);
    }
}
