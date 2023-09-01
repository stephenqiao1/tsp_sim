using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCity : MonoBehaviour
{
    public GameObject city;
    public Transform citiesParent; 

    // Update is called once per frame
    void Update()
    {
        // Check for a mouse click
        if (Input.GetMouseButtonDown(0)) {
            // Convert mouse position to world point
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            mousePos.z = 0; // Set the distance from the camera

            // Instantiate the prefab at the clicked position
            GameObject newCity = Instantiate(city, mousePos, Quaternion.identity);

            newCity.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            newCity.transform.SetParent(citiesParent);
        }
    }
}
