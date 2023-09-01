using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanelScript : MonoBehaviour
{
    public Text currentAlgorithmText;
    public Text algorithmText;
    public Text currentPathLengthText;
    public Text shortestPathLengthText;
    public Text numberOfPathsCheckedText;
    public Text statusText;
    public TSPAlgorithm algorithmScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // // Update the text fields with real-time data
        currentAlgorithmText.text = "Current Algorithm: " + algorithmText.text;
        currentPathLengthText.text = "Current Path Length: " + algorithmScript.currentPathLength;
        shortestPathLengthText.text = "Shortest Path Length: " + algorithmScript.shortestPathLength;
        numberOfPathsCheckedText.text = "Number of Paths Checked: " + algorithmScript.pathsChecked;
        statusText.text = "Status: " + algorithmScript.status;
    }
}
