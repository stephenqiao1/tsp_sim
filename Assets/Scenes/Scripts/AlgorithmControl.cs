using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlgorithmControl : MonoBehaviour
{
    public Dropdown algorithmDropdown;
    public Transform citiesPrefab;
    public Button startPauseButton;
    public Slider speedSlider;
    public Button clearButton;
    public Text algorithmText;
    public Button generateCitiesButton;
    public InputField numAntsInput;
    public Button generateAntsButton;
    public Button clearAntsButton;
    public Transform antsParent;
    private List<GameObject> cities = new List<GameObject>();
    private List<GameObject> ants = new List<GameObject>();

    private bool startPause = false;
    // Start is called before the first frame update
    void Start()
    {
        startPauseButton.interactable = false;
        // Add listeners for UI elements
        algorithmDropdown.onValueChanged.AddListener(delegate { OnAlgorithmChange(); });
        startPauseButton.onClick.AddListener(OnStartPause);
        speedSlider.onValueChanged.AddListener(delegate { OnSpeedChange(); });
        clearButton.onClick.AddListener(resetUI);
        generateCitiesButton.onClick.AddListener(generateCities);
        generateAntsButton.onClick.AddListener(populateAnts);
    }

    void generateCities() {
        populateCities();
    }
    void resetUI()
    {
        startPause = false;
        startPauseButton.GetComponentInChildren<Text>().text = "Start";
    }

    void OnAlgorithmChange()
    {
        if (algorithmText.text == "Ant Colony Optimization") {
            cities[0].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    void OnStartPause()
    {
        if (!startPause)
        {
            startPauseButton.GetComponentInChildren<Text>().text = "Pause";
        }
        else
        {
            startPauseButton.GetComponentInChildren<Text>().text = "Start";
        }
        startPause = !startPause;
    }

    void OnSpeedChange()
    {
        // Handle speed change
    }
    // Update is called once per frame
    void Update()
    {
        if (algorithmText.text == "Ant Colony Optimization" && cities.Count > 0)
        {
            numAntsInput.interactable = true;
            generateAntsButton.interactable = true;
            clearAntsButton.interactable = true;
        }
        if (algorithmText.text != "Ant Colony Optimization") {
            if (cities.Count > 0) {
                startPauseButton.interactable = true;
            }
        }
        else if (algorithmText.text == "Ant Colony Optimization") {
            if (cities.Count > 0 && ants.Count > 0) {
                startPauseButton.interactable = true;
            }
            else {
                startPauseButton.interactable = false;
            }
        }
    }

    public void populateCities()
    {
        foreach (Transform child in citiesPrefab)
        {
            cities.Add(child.gameObject);
        }
    }

    public void populateAnts() {
        foreach (Transform child in antsParent) {
            ants.Add(child.gameObject);
        }
    }
}
