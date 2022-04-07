using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject redKey;
    [SerializeField] GameObject greenKey;
    [SerializeField] GameObject blueKey;

    [SerializeField] GameObject redText;
    [SerializeField] GameObject greenText;
    [SerializeField] GameObject blueText;

    public void Arrange(List<int> inventory)
    {
        int reds = FindAmount(0);
        int greens = FindAmount(1);
        int blues = FindAmount(2);

        if (reds > 0)
        {
            redKey.SetActive(true);
            redText.SetActive(true);
            if (reds == 1) redText.GetComponent<TextMeshProUGUI>().text = "";
            else redText.GetComponent<TextMeshProUGUI>().text = "x" + reds;
        }
        else
        {
            redKey.SetActive(false);
            redText.SetActive(false);
        }

        if (greens > 0)
        {
            greenKey.SetActive(true);
            greenText.SetActive(true);
            if (greens == 1) greenText.GetComponent<TextMeshProUGUI>().text = "";
            else greenText.GetComponent<TextMeshProUGUI>().text = "x" + greens;
        }
        else
        {
            greenKey.SetActive(false);
            greenText.SetActive(false);
        }

        if (blues > 0)
        {
            blueKey.SetActive(true);
            blueText.SetActive(true);
            if (blues == 1) blueText.GetComponent<TextMeshProUGUI>().text = "";
            else blueText.GetComponent<TextMeshProUGUI>().text = "x" + blues;
        }
        else
        {
            blueKey.SetActive(false);
            blueText.SetActive(false);
        }

        int FindAmount(int wanted)
        {
            int count = 0;

            foreach (int item in inventory)
            {
                if (item == wanted) count++;
            }

            return count;
        }
    }
}
