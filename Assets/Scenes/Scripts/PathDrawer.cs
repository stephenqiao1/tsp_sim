using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    public GameObject pathPrefab;
    public Transform pathsParent;
    public List<Transform> cities;

    // Declare the delegate 
    public delegate void CityAddedEventHandler();

    // Declare the event using the delegate
    public static event CityAddedEventHandler OnCityAdded;

    // Function to call when a new city is added
    public void NewCityAdded()
    {
        OnCityAdded?.Invoke();
    }


    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the OnCityAdded event
        OnCityAdded += DrawPaths;
    }

    void DrawPaths() {
        for (int i = 0; i < cities.Count - 1; i++) {
            // Instantiate the PathPrefab and make it a child of 'Paths'
            GameObject newPath = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity, pathsParent);

            // Get the LineRenderer component
            LineRenderer lineRenderer = newPath.GetComponent<LineRenderer>();

            // Set the start and end positions
            lineRenderer.SetPosition(0, cities[i].position);
            lineRenderer.SetPosition(1, cities[i+1].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
