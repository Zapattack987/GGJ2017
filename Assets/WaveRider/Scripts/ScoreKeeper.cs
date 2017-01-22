using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour {
    public int rafts;
    public int pirateShips;
    public int warShips;
	void Start () {
        rafts = 0;
        pirateShips = 0;
        warShips = 0;
	}
	void Update () {
        GameObject.Find("raftsNum").GetComponent<Text>().text = rafts.ToString();
        GameObject.Find("piratesNum").GetComponent<Text>().text = pirateShips.ToString();
        GameObject.Find("warNum").GetComponent<Text>().text = warShips.ToString();
    }
}
