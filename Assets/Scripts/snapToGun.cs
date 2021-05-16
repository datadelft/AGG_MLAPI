using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapToGun : MonoBehaviour
{
    private Transform placeOnGun;
    public GameObject weaponAnchor;
    private activeWeapon weaponScript;

    private void Start()
    {
        weaponScript = weaponAnchor.GetComponent<activeWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!weaponScript.leftHandLocked || !weaponScript.rightHandLocked) // at least 1 hand is not yet locked
        {
            // find target to snap to on gun   (placeOnGun) 
            if (this.gameObject.name == "RightArmMoverTarget")
            {
                tag = "rightHandTarget";
                GetChildObject(weaponAnchor.transform, tag);
                weaponScript.rightHandLocked = true; // reset state
            }
            else if (this.gameObject.name == "LeftArmMoverTarget")
            {
                tag = "leftHandTarget";
                GetChildObject(weaponAnchor.transform, tag);
                weaponScript.leftHandLocked = true; // reset state
            }
            else
            {
                Debug.LogError("Set name for ArmMoverTarget to either RightArmMoverTarget or LeftArmMoverTarget");
            }
        }

        if (placeOnGun)
        {
            // Debug.Log("Placing hand on gun");
            Vector3 newRotation = new Vector3(placeOnGun.localEulerAngles.x, placeOnGun.localEulerAngles.x, placeOnGun.localEulerAngles.z);
            this.transform.localEulerAngles = newRotation;
            this.transform.position = placeOnGun.position;
        }
    }


    public void GetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                if (child.parent.gameObject.activeSelf) // check if the weapon is active.
                {
                    placeOnGun = child;
                    Debug.Log("New hand location found place on gun at " + placeOnGun.name);
                }
            }
            if (child.childCount > 0) // do it recursively
            {
                GetChildObject(child, _tag);
            }
        }
    }

}
