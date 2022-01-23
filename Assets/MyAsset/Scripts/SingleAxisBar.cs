using UnityEngine;

public class SingleAxisBar : MonoBehaviour
{
    float _FillRateValue = -0.51f; //progress bar starts empty
    public Material objectMaterial;

    float stepSize = 0.1f; //progress is done by this value

    // Start is called before the first frame update
    void Start()
    {
        //objectMaterial = new Material(Shader.Find("Shader Graphs/3DProgressBar")); //creating a material with the shader
        //gameObject.GetComponent<Renderer>().material = objectMaterial; //new material is applied to the game object
        //objectMaterial.SetFloat("_FillRate", _FillRateValue); //initial value is set 
    }


    public void ChangeValue(bool increase) //enables changing the value of progress bar
    {                                   //if increase param is true, the progress bar progresses otherwise it deprogresses
        if (increase)
        {
            _FillRateValue += stepSize; //progress increased
        }
        else
        {
            _FillRateValue -= stepSize; //progress decreased
        }
        objectMaterial.SetFloat("_FillRate", _FillRateValue); //Update the value of the progress bar
    }

    public void ChangePercentage(float _FillPercentage) //enables changing the value of progress bar
    {                                   //if increase param is true, the progress bar progresses otherwise it deprogresses
        float min = -0.51f;
        float max = 0.51f;

        _FillRateValue = _FillPercentage * (max - min) + min;

        objectMaterial.SetFloat("_FillRate", _FillRateValue); //Update the value of the progress bar
    }
}
