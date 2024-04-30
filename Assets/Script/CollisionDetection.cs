using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{


    void OnTriggerEnter2D(Collider2D other)
    //rigidBody�� ���𰡿� �浹�Ҷ� ȣ��Ǵ� �Լ� �Դϴ�.
    //Collider2D other�� �ε��� ��ü�� �޾ƿɴϴ�.
    {

        if (other.gameObject.name == "ChangeMapObject") {
            SceneChangeManager sceneChangeManager = FindObjectOfType<SceneChangeManager>();
            sceneChangeManager.SceneToLoad = "field";
            SceneChangeManager.Instance.StartButton();
        }

    }
    /*
     */

}
