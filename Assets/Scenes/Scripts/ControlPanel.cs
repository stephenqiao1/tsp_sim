using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public GameObject cityPrefab;
    public Transform citiesParent;
    public InputField numCitiesInput;
    public Button generateButton;
    public Button clearButton;
    public Camera mainCamera;
    public InputField numAntsInput;
    public Button generateAntsButton;
    public Button clearAntsButton;
    public GameObject antsPrefab;
    public Transform antsParent;
    public Transform pathsParent;

    public Text algorithmText;

    public GameObject startingCity;  // Transform of the starting city
    public List<GameObject> cities = new List<GameObject>();
    public List<GameObject> ants = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        numAntsInput.interactable = false;
        generateAntsButton.interactable = false;
        clearAntsButton.interactable = false;

        // Add listeners for button clicks
        generateButton.onClick.AddListener(GenerateCities);
        clearButton.onClick.AddListener(ClearCities);

        // Add listeners for ant-related buttons
        generateAntsButton.onClick.AddListener(GenerateAnts);
        clearAntsButton.onClick.AddListener(ClearAnts);
    }

    void GenerateCities()
    {
        ClearAllPaths();

        // Parse number of cities from input field
        int numCities = int.Parse(numCitiesInput.text);

        // Clear existing cities
        ClearCities();

        // Get camera dimensions
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // Minimum distance betwen cities
        float minDistance = 50f;

        // Generate new cities
        for (int i = 0; i < numCities; i++)
        {
            Vector3 randomPosition;
            bool positionValid = false;

            // Keep generating a new position until it is far enough from existing cities
            while (!positionValid)
            {
                randomPosition = new Vector3(
                    Random.Range(mainCamera.transform.position.x - camWidth / 2 + 200, mainCamera.transform.position.x + camWidth / 2 - 50),
                    Random.Range(mainCamera.transform.position.y - camHeight / 2 + 50, mainCamera.transform.position.y + camHeight / 2 - 50),
                    0
                );

                positionValid = true; // Assume the position is valid until proven wrong

                foreach (GameObject city in cities)
                {
                    if (Vector3.Distance(randomPosition, city.transform.position) < minDistance)
                    {
                        positionValid = false; // Position is too close to an existing city
                        break;
                    }
                }

                // if the position is valid, instantiate the city
                if (positionValid)
                {
                    GameObject newCity = Instantiate(cityPrefab, randomPosition, Quaternion.identity, citiesParent);
                    cities.Add(newCity);
                }
            }
        }

        if (algorithmText.text != "Ant Colony Optimization")
        {
            // make the first city the starting city (change its color)
            startingCity = cities[0];
            startingCity.GetComponent<SpriteRenderer>().color = Color.green;
        }

        // Calculate the distance from the starting city to all other cities
        foreach (GameObject city in cities)
        {
            // skip the starting city
            if (city == startingCity)
            {
                continue;
            }

            float distance = Vector3.Distance(startingCity.transform.position, city.transform.position);

            City cityScript = city.GetComponent<City>();
            if (cityScript != null)  // Check if the City script is attached
            {
                cityScript.distanceFromStart = distance;
            }
            else
            {
                Debug.LogError("City script not found on " + city.name);
            }
        }
    }

    void ClearAllPaths()
    {
        foreach (Transform child in pathsParent)
        {
            Destroy(child.gameObject);
        }
    }

    void ClearCities()
    {
        ClearAllPaths();
        // Destroy existing city GameObjects
        foreach (GameObject city in cities)
        {
            Destroy(city);
        }
        cities.Clear();
    }

    void GenerateAnts()
    {
        ClearAllPaths();
        // Parse number of ants from input field
        int numAnts = int.Parse(numAntsInput.text);

        // Clear existing ants
        ClearAnts();

        // Generate new ants
        for (int i = 0; i < numAnts; i++)
        {
            // Pick a random city for the ant to start at
            GameObject randomCity = cities[Random.Range(0, cities.Count)];

            // Instantiate a new ant at the position of the random city
            GameObject newAnt = Instantiate(antsPrefab, randomCity.transform.position, Quaternion.identity, antsParent);
            ants.Add(newAnt);
        }
    }

    void ClearAnts()
    {
        ClearAllPaths();
        // Destroy existing ant GameObjects 
        foreach (GameObject ant in ants)
        {
            Destroy(ant);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
