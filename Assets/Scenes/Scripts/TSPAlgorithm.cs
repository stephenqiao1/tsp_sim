using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TSPAlgorithm : MonoBehaviour
{
    public GameObject cityPrefab;
    public GameObject linePrefab;
    public Transform citiesPrefab;
    public Transform pathsParent;
    public Transform antsParent;
    public Text selectedAlgorithm;
    public Button startButton;
    public Button generateCitiesButton;
    public Button clear;
    public Slider speedSlider;
    public float currentPathLength;
    public float shortestPathLength;
    public int pathsChecked = 0;
    private bool startPause = false; // if startPause is false, the simulation hasnt started yet
    private float speed = 0.5f;
    private float dstPower = 4f;

    public GameObject antPrefab;

    public string status;

    private List<GameObject> cities = new List<GameObject>();

    private List<GameObject> ants = new List<GameObject>();

    private GameObject currentLine;
    private Coroutine bruteForceCoroutine;  // Store the Coroutine reference
    private Coroutine nearestNeighborCoroutine;
    private Coroutine antColonyOptimizationCoroutine;

    private Dictionary<Transform, GameObject> antLines = new Dictionary<Transform, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        generateCitiesButton.onClick.AddListener(populateCities);
        startButton.onClick.AddListener(startAlgorithm);
        clear.onClick.AddListener(clearPath);
        speedSlider.onValueChanged.AddListener(delegate { OnSpeedChange(); });
    }

    void OnSpeedChange()
    {
        speed = speedSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for user input to start an algorithm

        // Run the algorithm and update visualization
    }

    public void populateCities()
    {
        foreach (Transform child in citiesPrefab)
        {
            cities.Add(child.gameObject);
        }

        if (cities.Count > 0)
        {
            startButton.interactable = true;
        }
    }

    public void clearPath()
    {
        Destroy(currentLine);
        cities.Clear();
        pathsChecked = 0;

        // Disable the start button as there are no cities
        startButton.interactable = false;
    }

    public void startAlgorithm()
    {
        currentPathLength = 0;
        shortestPathLength = 0;
        pathsChecked = 0;
        if (!startPause)
        {
            status = "Running";
            if (selectedAlgorithm.text == "Brute Force Algorithm")
            {
                bruteForceCoroutine = StartCoroutine(BruteForceTSP());
            }
            else if (selectedAlgorithm.text == "Nearest Neighbor Algorithm")
            {
                nearestNeighborCoroutine = StartCoroutine(NearestNeighborTSP());
            }
            else if (selectedAlgorithm.text == "Ant Colony Optimization")
            {
                Debug.Log("Ant colony optimization");
                antColonyOptimizationCoroutine = StartCoroutine(AntColonyOptimizationTSP());
            }
            else
            {
                return;
            }
            Debug.Log("algorithm started");
            startPause = true;
        }
        else
        {
            status = "Paused";
            if (bruteForceCoroutine != null)
            {
                StopCoroutine(bruteForceCoroutine);
            }
            else if (nearestNeighborCoroutine != null)
            {
                StopCoroutine(nearestNeighborCoroutine);
            }
            Debug.Log("Algorithm paused");
            startPause = false;
        }
    }

    public void DrawPath(List<GameObject> path)
    {
        // Destroy the old line if it exists
        if (currentLine != null)
        {
            Destroy(currentLine);
        }

        // Create a new GameObject to hold the LineRenderer component
        currentLine = new GameObject("GeneratedLine");
        currentLine.transform.SetParent(pathsParent);

        // Add a LineRenderer component to the new GameObject
        LineRenderer lineRenderer = currentLine.AddComponent<LineRenderer>();

        // Configure LineRenderer properties
        lineRenderer.positionCount = path.Count;
        lineRenderer.material = new Material(Shader.Find("UI/Default"));

        // Set the sorting layer and order in layer
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 1;

        lineRenderer.material.color = Color.white;
        Color currentColor = lineRenderer.material.color;
        currentColor.a = 0.1f;
        lineRenderer.material.color = currentColor;

        lineRenderer.textureMode = LineTextureMode.Tile;

        // turn off shadows
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;

        // Set corner vertices to maximum value 
        lineRenderer.numCornerVertices = 100;

        // Set end cap vertices to maximum value
        lineRenderer.numCapVertices = 100;

        // Set the positions of the LineRenderer based on the city positions
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i].transform.position);
        }
    }

    public void DrawAntPath(Transform ant, List<GameObject> path)
    {
        // Destroy the old line if it exists
        if (antLines.ContainsKey(ant))
        {
            Destroy(antLines[ant]);
            antLines.Remove(ant);
        }

        // Create a new GameObject to hold the LineRenderer component
        GameObject newAntLine = new GameObject("GeneratedLine");
        newAntLine.transform.SetParent(pathsParent);
        antLines[ant] = newAntLine;

        // Add a LineRenderer component to the new GameObject
        LineRenderer lineRenderer = newAntLine.AddComponent<LineRenderer>();

        // Configure LineRenderer properties
        lineRenderer.positionCount = path.Count;
        lineRenderer.material = new Material(Shader.Find("UI/Default"));

        // Set the sorting layer and order in layer
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 1;

        lineRenderer.material.color = Color.white;
        Color currentColor = lineRenderer.material.color;
        currentColor.a = 0.1f;
        lineRenderer.material.color = currentColor;

        lineRenderer.textureMode = LineTextureMode.Tile;

        // turn off shadows
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;

        // Set corner vertices to maximum value 
        lineRenderer.numCornerVertices = 100;

        // Set end cap vertices to maximum value
        lineRenderer.numCapVertices = 100;

        // Set the positions of the LineRenderer based on the city positions
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i].transform.position);
        }
    }

    public IEnumerator BruteForceTSP()
    {
        List<GameObject> bestPath = new List<GameObject>();
        float shortestDistance = float.MaxValue;

        // initialize list of paths
        List<List<GameObject>> allPaths = new List<List<GameObject>>();

        // Generate all possible paths
        GenerateAllPaths(cities, 0, allPaths);

        Debug.Log(allPaths.Count);

        foreach (List<GameObject> path in allPaths)
        {
            float distance = CalculateTotalDistance(path);
            currentPathLength = distance;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                bestPath = path;
            }

            shortestPathLength = shortestDistance;

            // Visualize the current path
            DrawPath(path);

            pathsChecked++;

            // Wait for a short period before visualizing the next path
            yield return new WaitForSeconds(speed);
        }
        status = "Completed";
        startPause = false;
        startButton.GetComponentInChildren<Text>().text = "Start";
    }
    public IEnumerator NearestNeighborTSP()
    {
        List<GameObject> visitedCities = new List<GameObject>();
        List<GameObject> remainingCities = new List<GameObject>(cities);

        // start from the first city
        GameObject currentCity = cities[0];
        visitedCities.Add(currentCity);
        remainingCities.Remove(currentCity);

        Debug.Log(cities.Count);

        while (remainingCities.Count > 0)
        {
            // Find the nearest unvisited city
            GameObject nearestCity = FindNearestNeighbor(currentCity, remainingCities);

            // Move to the nearest City
            visitedCities.Add(nearestCity);
            remainingCities.Remove(nearestCity);

            // Update the current city
            currentCity = nearestCity;

            Debug.Log(visitedCities.Count);

            // Visualize the current path
            DrawPath(visitedCities);

            yield return new WaitForSeconds(speed);
        }

        // Return to the starting city to complete the tour 
        visitedCities.Add(visitedCities[0]);

        // Visualize the final path
        DrawPath(visitedCities);
        pathsChecked++;
        status = "Completed";
        startPause = false;
        startButton.GetComponentInChildren<Text>().text = "Start";
    }

    private GameObject FindNearestNeighbor(GameObject currentCity, List<GameObject> remainingCities)
    {
        float shortestDistance = float.MaxValue;
        GameObject nearestCity = null;

        foreach (GameObject city in remainingCities)
        {
            float distance = CalculateDistance(currentCity, city);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestCity = city;
            }
        }
        currentPathLength += shortestDistance;
        shortestPathLength += shortestDistance;

        return nearestCity;
    }

    private float CalculateDistance(GameObject currentCity, GameObject city)
    {
        return Vector3.Distance(currentCity.transform.position, city.transform.position);
    }

    private void GenerateAllPaths(List<GameObject> cities, int start, List<List<GameObject>> allPermutations)
    {
        if (start == cities.Count - 1)
        {
            Debug.Log("Path is added");
            allPermutations.Add(new List<GameObject>(cities));
            return;
        }

        for (int i = start; i < cities.Count; i++)
        {
            // Swap
            GameObject temp = cities[start];
            cities[start] = cities[i];
            cities[i] = temp;

            // Recursive
            GenerateAllPaths(cities, start + 1, allPermutations);

            // Backtrack
            temp = cities[start];
            cities[start] = cities[i];
            cities[i] = temp;
        }
    }

    private float CalculateTotalDistance(List<GameObject> path)
    {
        float totalDistance = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(path[i].transform.position, path[i + 1].transform.position);
        }

        return totalDistance;
    }

    public IEnumerator AntColonyOptimizationTSP()
    {
        // Initialize pheromone levels for each edge
        Dictionary<string, float> pheromones = new Dictionary<string, float>();

        // Initialize pheromones for all edges
        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                string edgeKey = i.ToString() + "-" + j.ToString();
                pheromones[edgeKey] = 1.0f;
            }
        }

        Dictionary<Transform, List<GameObject>> antRemainingCities = new Dictionary<Transform, List<GameObject>>();

        // Initialize a dictionary to store visited cities for each ant
        Dictionary<Transform, List<GameObject>> antVisitedCities = new Dictionary<Transform, List<GameObject>>();

        // Initialize the remaining and visited cities for each ant based on their starting positions
        foreach (Transform ant in antsParent)
        {
            GameObject currentCity = cities.Find(city => city.transform.position == ant.position);
            antRemainingCities[ant] = new List<GameObject>(cities);
            antVisitedCities[ant] = new List<GameObject> { currentCity };
            antRemainingCities[ant].Remove(currentCity);
        }

        while (true)
        {
            bool allAntsVisitedAllCities = true; // Flag to check if all ants have visited all cities

            foreach (Transform ant in antsParent)
            {
                // Skip if this ant has already visited all cities
                if (antRemainingCities[ant].Count == 0)
                {
                    Debug.Log("Ant has visited all cities");
                    continue;
                }

                // Since at least one ant has not visited all cities, set the flag to false
                allAntsVisitedAllCities = false;

                Debug.Log("Ant has not visited all cities yet");

                // Get the current city based on the ant's position
                GameObject currentCity = cities.Find(city => city.transform.position == ant.position);

                // Calculate probabilities and find the next city for this ant
                List<double> cityDesirabilities = CalculateCityDesirability(currentCity, antRemainingCities[ant], dstPower);
                List<double> cityProbabilities = CalculateProbabilityList(cityDesirabilities);
                GameObject nextCity = FindNextCity(antRemainingCities[ant], cityProbabilities);

                // Move the ant to the next city
                ant.position = nextCity.transform.position;

                // Update the visited cities for this ant
                antVisitedCities[ant].Add(nextCity);

                // Update the remaining cities for this ant
                antRemainingCities[ant].Remove(nextCity);
            }

            // Check the stopping condition
            if (allAntsVisitedAllCities)
            {
                break;
            }

            yield return new WaitForSeconds(speed);
        }

        yield return new WaitForSeconds(1);

        // Start the animation for each ant
        foreach (Transform ant in antsParent)
        {
            antVisitedCities[ant].Reverse();
            StartCoroutine(AnimatePath(ant, antVisitedCities[ant]));
        }

        status = "Completed";
        startPause = false;
        startButton.GetComponentInChildren<Text>().text = "Start";
    }

    public IEnumerator AnimatePath(Transform ant, List<GameObject> path)
    {
        // Create a list of paths to hold the Current retraced path
        List<GameObject> retracedPath = new List<GameObject>();

        // Create a new GameObject to hold the LineRenderer component
        GameObject newLine = new GameObject("GeneratedLine");
        newLine.transform.SetParent(pathsParent);

        // Add a LineRenderer component to the new GameObject
        LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

        // Configure LineRenderer properties
        lineRenderer.positionCount = 2; // Start and end point
        lineRenderer.material = new Material(Shader.Find("UI/Default"));
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 1;
        lineRenderer.material.color = Color.white;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.numCornerVertices = 100;
        lineRenderer.numCapVertices = 100;

        Vector3 startPos = ant.position;
        Vector3 endPos;

        for (int i = 0; i < path.Count; i++)
        {
            endPos = path[i].transform.position;
            float journeyLength = Vector3.Distance(startPos, endPos);
            float startTime = Time.time;

            float distanceCovered = (Time.time - startTime) * 400f;
            float fractionOfJourney = distanceCovered / journeyLength;

            lineRenderer.SetPosition(0, startPos); // Set the starting point of the line

            while (fractionOfJourney < 1)
            {
                distanceCovered = (Time.time - startTime) * 400f;
                fractionOfJourney = distanceCovered / journeyLength;

                // Move the ant towards the next city
                ant.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);

                // Update the end point of the line to the ant's current position
                lineRenderer.SetPosition(1, ant.position);

                yield return null;
            }

            retracedPath.Add(path[i]);

            // Update start position for the next loop
            startPos = endPos;
            DrawAntPath(ant, retracedPath);
        }
    }

    private List<double> CalculateCityDesirability(GameObject currentCity, List<GameObject> remainingCities, double dstPower)
    {
        List<double> cityDesirability = new List<double>();

        foreach (GameObject city in remainingCities)
        {
            float distance = CalculateDistance(currentCity, city);

            double desirability = System.Math.Pow(1 / distance, dstPower);

            cityDesirability.Add(desirability);
        }

        return cityDesirability;
    }

    private List<double> CalculateProbabilityList(List<double> list)
    {
        List<double> probabilityList = new List<double>();

        foreach (double desirability in list)
        {
            double probability = desirability / list.Sum();

            probabilityList.Add(probability);
        }

        return probabilityList;
    }

    private GameObject FindNextCity(List<GameObject> remainingCites, List<double> probabilityList)
    {
        // Generate a random float between 0 and 1
        System.Random rand = new System.Random();
        double randomValue = rand.NextDouble();

        // Initialize a running sum 
        double runningSum = 0;

        // Initialize the selected city
        GameObject selectedCity = null;

        // Iterate through the list to find the corresponding city 
        for (int i = 0; i < probabilityList.Count; i++)
        {
            runningSum += probabilityList[i];
            if (runningSum >= randomValue)
            {
                selectedCity = remainingCites[i];
                return selectedCity;
            }
        }

        return null;
    }
    void ClearAllPaths()
    {
        foreach (Transform child in pathsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
