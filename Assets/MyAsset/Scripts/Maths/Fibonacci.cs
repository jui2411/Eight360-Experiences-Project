using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Fibonacci : MonoBehaviour
{
    public enum FibonacciNum
    {
        one = 1,
        two = 2,
        three = 3,
        five = 5,
        eight = 8,
        thirteen = 13,
        twentyOne = 21,
        thirtyFour = 34,
        fiftyFive = 55,
        eightyNine = 89,
        oneFourFour = 144,
        twoThreeThree = 233,
        threeSevenSeven = 377,
        sixOneZero = 610,
        nineEightSeven = 987,
        oneFiveNineSeven = 1597
    }

    [Header("Visual Settings")]
    public Color defaultColor;
    public Color highlightColor;
    public float FibonacciScale = 1f;
    [SerializeField] private GameObject pointObj;

    [Header("Fibonacci Settings")]
    [SerializeField] private FibonacciNum highlight;
    [SerializeField] private int highlightOffset;
    [Range(0f, 1.6180339f)]
    public float turnFraction;
    public int numPoints;
    public bool evenSpacing = false;
    [Range(-5f, 5f)]
    public float pow = 2;
    public float plotPointSize = 2f;
    public bool threeDimensional = false;

    [Header("Play Settings")]
    public bool autoPlayTF = false;
    public bool autoPlayPow = false;
    [Range(-0.0001f, 0.0001f)]
    public float TFIncrements = 0.00001f;
    [Range(-0.05f, 0.05f)]
    public float powIncrements = 0.01f;

    //private variables
    private Transform holder;
    private float maxPow = 5f;

    // Start is called before the first frame update
    void Start()
    {
        autoPlayTF = false;
        autoPlayPow = false;
        GenerateFibonacci();
    }

    // Update is called once per frame
    void Update()
    {
        LimitVariables();

        if (autoPlayTF)
        {
            IncrementTurnFactor(TFIncrements);
            GenerateFibonacci();
        }

        if(autoPlayPow)
        {
            IncrementPow(powIncrements);
            GenerateFibonacci();
        }
    }

    private void LimitVariables()
    {
        if (pow < -maxPow) pow = -maxPow;
        if (pow > maxPow) pow = maxPow;
    }

    public void IncrementTurnFactor(float value)
    {
        turnFraction += value;
    }

    public void IncrementPow(float value)
    {
        pow += value;
    }

    public void GenerateFibonacci()
    {

        GenerateHolder();

        Color colour;

        for (int i = 0; i < numPoints; i++)
        {
            float dst;

            if (!evenSpacing)
            {
                dst = i / (numPoints - FibonacciScale); // distance will vary from 0 to 1 over the course of the loop
            } else
            {
                dst = Mathf.Pow(i / (numPoints - FibonacciScale), pow);
            }

            float x;
            float y;

            if ((i + highlightOffset) % ((int)highlight) == 0)
            {
                colour = highlightColor;
            }
            else
            {
                colour = defaultColor;

            }

            if (!threeDimensional)
            {
                float angle = 2 * Mathf.PI * turnFraction * i;
                x = dst * Mathf.Cos(angle);
                y = dst * Mathf.Sin(angle);

                PlotPoint(x, y, colour);

            } else
            {
                float t = i / (numPoints - 1f);
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = 2 * Mathf.PI * turnFraction * i;

                x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);

                Plot3DPoint(x, y, z, colour);
            }

            
        }

    }

    void GenerateHolder()
    {
        //Create Holder
        string holderName = "Generated Fibonacci";
        if (transform.Find(holderName)) DestroyImmediate(transform.Find(holderName).gameObject);

        holder = new GameObject(holderName).transform;
        holder.transform.SetParent(transform);
    }

    public void PlotPoint(float x, float y, Color selectedColor)
    {

        GameObject gameObject = Instantiate(pointObj, transform.position, transform.rotation);
        gameObject.name = "Point";
        gameObject.transform.SetParent(holder, false);
        gameObject.GetComponent<Renderer>().sharedMaterial.color = selectedColor;

        gameObject.transform.SetParent(holder);
        gameObject.transform.position = new Vector3(x, y, 0);
        gameObject.transform.localScale = new Vector3(plotPointSize, plotPointSize, plotPointSize);
    }

    public void Plot3DPoint(float x, float y, float z, Color selectedColor)
    {

        GameObject gameObject;
        if (pointObj != null)
        {
            gameObject = Instantiate(pointObj, transform.position, transform.rotation);
            gameObject.GetComponent<Renderer>().sharedMaterial.color = selectedColor;
        } else
        {
            gameObject = new GameObject();
        }
        gameObject.name = "Point";
        gameObject.transform.SetParent(holder, false);
        

        gameObject.transform.SetParent(holder);
        gameObject.transform.position = new Vector3(x, y, z);
        gameObject.transform.localScale = new Vector3(plotPointSize, plotPointSize, plotPointSize);
    }


}


