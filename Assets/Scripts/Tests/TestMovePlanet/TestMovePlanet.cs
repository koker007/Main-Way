using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovePlanet : MonoBehaviour
{
    [SerializeField]
    GameObject planet;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject SkyBox;

    void Start()
    {
        iniPlayer();
    }

    void iniPlayer() {
        player.transform.localPosition = new Vector3(planet.transform.localScale.x/2, 0, planet.transform.localScale.z / 2);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        RotateSkyBox();
    }

    void MovePlayer() {
        if (Input.GetKey(KeyCode.Keypad8)) {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x, 0, player.transform.localPosition.z + Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.Keypad5))
        {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x, 0, player.transform.localPosition.z - Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x + Time.unscaledDeltaTime, 0, player.transform.localPosition.z);
        }
        if (Input.GetKey(KeyCode.Keypad4))
        {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x - Time.unscaledDeltaTime, 0, player.transform.localPosition.z);
        }

        Vector3 pos = player.transform.localPosition;

        if (pos.x > planet.transform.localScale.x)
            pos.x -= planet.transform.localScale.x;
        else if (pos.x < 0)
            pos.x += planet.transform.localScale.x;

        if (pos.z > planet.transform.localScale.z)
            pos.z -= planet.transform.localScale.z;
        else if (pos.z < 0)
            pos.z += planet.transform.localScale.z;

        player.transform.localPosition = pos;
    }

    void RotateSkyBox() {

        Vector3 playerPos = player.transform.localPosition;
        Vector3 planetScale = planet.transform.localScale;

        //Позиция игрока в коофиценте
        float coofX = playerPos.x / planetScale.x;
        float coofZ = playerPos.z / planetScale.z;

        //Вычисляем градусы по x
        float rotX = 360 * coofZ;
        float rotZ = 360 * coofX;

        float rotY = 360 * coofZ;

        Quaternion rot = Quaternion.Euler(rotX, rotY, rotZ);
        SkyBox.transform.rotation = rot;
    }
}
